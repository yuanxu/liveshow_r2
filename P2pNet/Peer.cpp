#include "StdAfx.h"
#include "Peer.h"
#include "ace/OS.h"
#include "ace/Asynch_Connector.h"
#include "ace/Message_Block.h"
#include "ace/Guard_T.h"
#include "PeerMgr.h"
#include "CmdBase.h"
#include "CmdJoin.h"
#include "CmdJoinR.h"
#include "CmdPushHeader.h"
#include "CmdPushPeers.h"
#include "CmdPushData.h"
#include "CmdApplyData.h"
#include "CmdPing.h"
#include "CmdAskForDisconnect.h"
#include "CmdExBm.h"
#include "ace/Proactor.h"
#include "PeerConnector.h"

Peer::Peer(bool IsMySelf,PeerMgr *p):ACE_Service_Handler(),lpMgr_(p), isConnected(false),peerId_(0), is_self_(IsMySelf),mq_mutex_(),mq_(),seqId_(100),bm_(),is_server_(false)
	,applied_segs_(),lpConnector_(NULL),is_joined_(false),sent_seg_id_(0),is_behind_nat_(false),tcp_port_(18086),udp_port_(18086)
{
	if (!IsMySelf) //����ڵ�ֻ�Ǵ�һ����ַ��Ϣ�����贴������
	{
		ACE_NEW_NORETURN(lpMb_,ACE_Message_Block(PEER_BUF_SIZE));
	}
	else
	{
		lpMb_=NULL;
	}
}

Peer::~Peer(void)
{
	if (!is_self_)
	{

		disConnect();	

		//�ȴ�����δ��ɵĲ������
		if(this->handle()!=ACE_INVALID_HANDLE)
		{
			ACE_OS::closesocket(this->handle());
			
		}
		if (is_joined_)
		{
			lpMgr_->peer_list_.leave(this); //�뿪
		}
		
		if (lpMb_ != NULL)
		{
			lpMb_->release();
			lpMb_ = NULL;
		}
	}
	else
	{
		if (is_joined_)
		{
			lpMgr_->peer_list_.leave(this); //�뿪
		}

	}
}

//************************************
// Method:    handle_read_stream
// FullName:  Peer::handle_read_stream
// Access:    public 
// Returns:   void
// Qualifier:
// Parameter: const ACE_Asynch_Read_Stream::Result &result
//************************************
void Peer::handle_read_stream( const ACE_Asynch_Read_Stream::Result &result )
{

	ACE_Message_Block &mb = result.message_block();
	if (!result.success() || result.bytes_transferred() == 0)
	{
		delete this;
	}
	else
	{
		flux_.BytesReadLast = result.bytes_transferred();
		flux_.BytesRead += result.bytes_transferred();
		flux_.Calculate();
		if(mb.length() < SIZE_OF_CMD_HEADER) //û�еõ�ͷ
		{
			//������
			init_read();
		}
		else
		{
			while (mb.length() >= SIZE_OF_CMD_HEADER) //���ܻ���ճ������������Ҫһ��ѭ�����������ÿ����Ϣ
			{
				CmdHeader *lpHdr_ = (CmdHeader*)mb.rd_ptr();
				assert(lpHdr_->cmd_ != cmd_unknow);
				assert(lpHdr_->len_ > 0);
				assert(lpHdr_->len_ <= PEER_BUF_SIZE);
				if (lpHdr_->len_ > mb.length()) //δ��������
				{
					break; //������
				}
				else
				{
					//��������
					if(process(mb.duplicate()) == LS_FAILED)
					{
						delete this;
						return ;
					}
					//�ƶ�����ָ��
					mb.rd_ptr(lpHdr_->len_);
				}	
			}

			//�������ݵ�ͷ��
			size_t len = mb.length();
			mb.crunch();
			assert(mb.length() == len);
			init_read();//��ȡ��һ������
		}
	}
}

void Peer::handle_write_stream( const ACE_Asynch_Write_Stream::Result &result )
{

#if 0
	ACE_DEBUG ((LM_DEBUG, "%s = %s\n", "message_block", result.message_block ().rd_ptr ()));
#endif

	ACE_Message_Block& mb=result.message_block();
	size_t unsent_data;

	if (result.success())
	{
		
		unsent_data = result.bytes_to_write() - result.bytes_transferred();
		flux_.BytesSentLast = result.bytes_transferred();
		flux_.BytesSent += result.bytes_transferred();
		flux_.Calculate();
		
		if (unsent_data != 0) //δд��
		{
			// Reset pointers
			result.message_block ().rd_ptr (result.bytes_transferred ());
			if (handle() == ACE_INVALID_HANDLE)
			{
				return ;
			}
			// Duplicate the message block and retry remaining data
			if (writer_.write (*result.message_block ().duplicate (),
				unsent_data) == -1)
			{
				ACE_ERROR ((LM_ERROR,
					"%p\n",
					"ACE_Asynch_Write_Stream::write"));
				result.message_block().release();
				return;
			}
			result.message_block().release();
		}
		else
		{

			//���ͳɹ�
			ACE_Guard<ACE_Thread_Mutex> guard(mq_mutex_);
			(*mq_.begin())->release();
			mq_.erase(mq_.begin());

			if (isConnected && !mq_.empty() ) // && !mq.empty()
			{
				init_write();	//������һ��send
			}	
		}
	}   

//	mb.release();

}

void Peer::open( ACE_HANDLE h,ACE_Message_Block& )
{
	handle(h); //����handler
	//���뵽peer_List_�У�����cmd_connect��_cmd_joinָ������peerId
	
	//lpMgr_->peer_list_.join(this);
	

	//��ʼ����д������
	if(reader_.open(*this) !=0 || writer_.open(*this) != 0)
	{
		ACE_ERROR((LM_ERROR,ACE_TEXT("%p\n"),
			ACE_TEXT("Peer open")));;
		delete this;
		return;
	}
	flux_.Init();
	//����һ��������
	init_read();

	isConnected = true;
	if (lpConnector_ != NULL) //����PeerConnector������������ӡ�����Ҫ����Ƿ��Ѿ����ڵȷ�����Ϣ
	{
		PeerInfoPtr pInfo = lpConnector_->get_message(addr_.get_ip_address());
		if (pInfo!= NULL)
		{
			peerId_ = pInfo->peerId_;
			putQ(pInfo->mb_);
			delete pInfo;
			lpMgr_->peer_info_list_.remove(peerId_); //���ӳɹ��󣬴�mCache��ȥ��
		}
		

	}
	//����mb�Ѿ���proactive����
	return;
}

void Peer::connect()
{
	if (isConnected)
	{
		return;
	}
	ACE_Asynch_Connector<Peer> connector;
	connector.open(0,lpMgr_->lpProactor_);
	connector.connect(addr_);
}

void Peer::disConnect()
{
	if (is_self_)
	{
		return;
		isConnected = false;
	}
	reader_.cancel();
	writer_.cancel();
	isConnected = false;
}

void Peer::putQ( ACE_Message_Block* out_msg )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mq_mutex_);
	if (mq_.size() >= lpMgr_->config_.max_peer_queue_size_) //����������Ĵ�������Ϣ����
	{
		//��Ϊ��һ����ϢҲ���Ѿ����ڱ�����״̬������Ϊ�ڶ�����Ϣ������pushdata����Ϣ�壬�����Ƴ���������Ϣ��
		ACE_Message_Block *lpMbt_= *(mq_.begin()+2);
		lpMbt_->release();
		mq_.erase(mq_.begin()+2);
	}
	mq_.push_back(out_msg);
	if (isConnected && mq_.size() == 1)
	{
		//Ͷ��һ��д
		init_write();
		//writer_.write(*out_msg,out_msg->size());
	}
	
}

void Peer::set_peerMgr( PeerMgr* p )
{
	lpMgr_ = p;
}
int Peer::process( ACE_Message_Block *mb )
{
	CmdHeader* lpHdr = reinterpret_cast<CmdHeader*>(mb->rd_ptr());
	
	CmdBase* lpCmd =NULL;
	bool isJoin = false;
	switch(lpHdr->cmd_)
	{
	case cmd_join:
		{
			lpCmd = new CmdJoin();
			isJoin =true;
		}
		break;
	case cmd_join_r: //������ͬһ�̳�·���ϣ�����Ҫ��������һ��
		{
			CmdJoinR cmd;
			cmd.deserialize(mb);
			int iret = cmd.process(*lpMgr_,*this);
			mb->release();
			peerId_ = cmd.ServerId_;
			if (cmd.is_server_ == '\1')
			{
				is_server_ = true;
			}
			else
			{
				is_server_ = false;
			}
			return iret;
		}
		break;
	case cmd_push_peers:
		lpCmd = new CmdPushPeers(lpMgr_->peer_list_);
		break;
	case cmd_push_header:
		lpCmd = new CmdPushHeader();
		break;
	case cmd_ex_bm:
		lpCmd = new CmdExBm();
		break;
	case cmd_apply_data:
			lpCmd = new CmdApplyData();
		break;
	case cmd_push_data:
		{
			//TODO:������ܻ��в�����ͻ��Σ��
			SEGMENT_ID seg_id_ ;
			ACE_OS::memcpy(&seg_id_,mb->rd_ptr()+SIZE_OF_CMD_HEADER,sizeof(seg_id_));

			lpCmd = new CmdPushData();
		}
		
		break;
	case cmd_ask_for_disconnect:
		lpCmd = new CmdAskForDisconnect();
		break;
	case cmd_ping:
		lpCmd = new CmdPing();
		break;
	}
	int iret = -1;
	if (lpCmd != NULL)
	{
		lpCmd->deserialize(mb);
		if (isJoin)
		{
			peerId_ = lpCmd->cmd_hdr_.peerId_; //����peerId .��Ϊ�����ӷţ������������ӹ�����peer��IDֵ��
		}
		iret = lpCmd->process(*lpMgr_,*this);
		mb->release();
		delete lpCmd;
	}
	return iret;
}

ACE_UINT32 Peer::get_next_sequence()
{
	ACE_Guard<ACE_Thread_Mutex> guard(mq_mutex_);
	if (seqId_ == 0xFFFFFFFF)
	{
		seqId_ =0;
	}
	return seqId_ ++;
}

int Peer::init_read()
{
	//Ͷ���µ�ͷ����
	 size_t byte_to_read =0;
	 if (lpMb_->length() == 0)
	 {
		 byte_to_read = SIZE_OF_CMD_HEADER;
	 }
	 else
	 {
		 if (lpMb_->length()< SIZE_OF_CMD_HEADER)
		 {
			 byte_to_read = SIZE_OF_CMD_HEADER - lpMb_->length();
		 }
		 else
		 {
			 CmdHeader *lpHdr = (CmdHeader*)lpMb_->rd_ptr();
			 if (lpMb_->length() < lpHdr->len_)
			 {
				 byte_to_read = lpHdr->len_ - lpMb_->length();
			 }
		 }
	 }
	 assert(byte_to_read != 0);
	int ret=reader_.read(*lpMb_,byte_to_read) ;
	if(ret !=  0)
	{
		ACE_ERROR((LM_ERROR,ACE_TEXT("%p\n"),
			ACE_TEXT("Peer hand_read_stream")));
		int err = ACE_OS::last_error();
		delete this;
		return -1;
	}
	return 0;
}

int Peer::init_write()
{
	ACE_Message_Block *mb;
lblStart:
	if (mq_.empty() || this->handle() == ACE_INVALID_HANDLE)
	{
		return 0;
	}
	else
	{
		mb =* mq_.begin();
		if (mb->length() == sizeof(SEGMENT_ID)) //��push data��Ϣ�����⴦��
		{
			SEGMENT_ID seg_id_;
			ACE_OS::memcpy(&seg_id_,mb->rd_ptr(),sizeof(SEGMENT_ID));

			//��ջ
			(*mq_.begin())->release();
			mq_.erase(mq_.begin());
			mb = NULL;

			ACE_Message_Block *lpSeg = this->lpMgr_->buffer_.get_data(seg_id_);
			if(lpSeg == NULL) //�ѹ���
			{
				goto lblStart; //������һ������
			}
			else
			{
				if (sent_seg_id_ >= seg_id_)
				{
					lpSeg->release();
					goto lblStart;
				}
				else
				{
					sent_seg_id_ = seg_id_;
				}	
				CmdPushData cmd;
				cmd.mb_ = lpSeg;
				cmd.seg_id_ = seg_id_;

				ACE_ASSERT(lpSeg != NULL);

				//���л������ص��Ǹ�����Ϣ
				mb = cmd.serialize(get_next_sequence(),lpMgr_->chId_,lpMgr_->peerId_);

				mq_.insert(mq_.begin(),lpSeg->duplicate()); // ��Ϣ��
				mb->cont(NULL); //�𿪸�����Ϣ
				mq_.insert(mq_.begin(),mb); //��Ϣͷ
			}
			lpSeg->release();
		}

		if(writer_.write(*mb,mb->length()) == -1)
		{
			ACE_ERROR((LM_ERROR,ACE_TEXT("%p\n"),
				ACE_TEXT("Peer init_write")));;
			return -1;
		}
		else
		{
			return 0;
		}
	
	}
}

void Peer::addresses( const ACE_INET_Addr &remote_address, const ACE_INET_Addr &local_address )
{
	addr_.set(remote_address);
}

void Peer::push_applied_seg_id( SEGMENT_ID id )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mq_mutex_); //���︴��mq_mutexҲ������
	applied_segs_.push_back(id);
}

void Peer::pop_applied_seg_id( SEGMENT_ID id )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mq_mutex_); //���︴��mq_mutexҲ������
	list<SEGMENT_ID>::iterator it = find(applied_segs_.begin(),applied_segs_.end(),id);
	if (it !=applied_segs_.end() )
	{
		applied_segs_.erase(it);
	}
}

bool Peer::has_applied( SEGMENT_ID id )
{
		ACE_Guard<ACE_Thread_Mutex> guard(mq_mutex_); //���︴��mq_mutexҲ������
		 return find(applied_segs_.begin(),applied_segs_.end(),id) != applied_segs_.end();
}

