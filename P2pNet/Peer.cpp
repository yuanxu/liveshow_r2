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
	if (!IsMySelf) //自身节点只是存一个地址信息，无需创建缓冲
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

		//等待所有未完成的操作完成
		if(this->handle()!=ACE_INVALID_HANDLE)
		{
			ACE_OS::closesocket(this->handle());
			
		}
		if (is_joined_)
		{
			lpMgr_->peer_list_.leave(this); //离开
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
			lpMgr_->peer_list_.leave(this); //离开
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
		if(mb.length() < SIZE_OF_CMD_HEADER) //没有得到头
		{
			//继续读
			init_read();
		}
		else
		{
			while (mb.length() >= SIZE_OF_CMD_HEADER) //可能会有粘包现象，所以需要一个循环处理读到的每个消息
			{
				CmdHeader *lpHdr_ = (CmdHeader*)mb.rd_ptr();
				assert(lpHdr_->cmd_ != cmd_unknow);
				assert(lpHdr_->len_ > 0);
				assert(lpHdr_->len_ <= PEER_BUF_SIZE);
				if (lpHdr_->len_ > mb.length()) //未读完数据
				{
					break; //继续读
				}
				else
				{
					//处理数据
					if(process(mb.duplicate()) == LS_FAILED)
					{
						delete this;
						return ;
					}
					//移动数据指针
					mb.rd_ptr(lpHdr_->len_);
				}	
			}

			//推送数据到头部
			size_t len = mb.length();
			mb.crunch();
			assert(mb.length() == len);
			init_read();//读取下一组数据
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
		
		if (unsent_data != 0) //未写完
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

			//发送成功
			ACE_Guard<ACE_Thread_Mutex> guard(mq_mutex_);
			(*mq_.begin())->release();
			mq_.erase(mq_.begin());

			if (isConnected && !mq_.empty() ) // && !mq.empty()
			{
				init_write();	//发起下一个send
			}	
		}
	}   

//	mb.release();

}

void Peer::open( ACE_HANDLE h,ACE_Message_Block& )
{
	handle(h); //保存handler
	//加入到peer_List_中，随后的cmd_connect或_cmd_join指令会更新peerId
	
	//lpMgr_->peer_list_.join(this);
	

	//初始化读写工厂类
	if(reader_.open(*this) !=0 || writer_.open(*this) != 0)
	{
		ACE_ERROR((LM_ERROR,ACE_TEXT("%p\n"),
			ACE_TEXT("Peer open")));;
		delete this;
		return;
	}
	flux_.Init();
	//发起一个读操作
	init_read();

	isConnected = true;
	if (lpConnector_ != NULL) //中由PeerConnector发起的主动连接。这里要检查是否已经存在等发送消息
	{
		PeerInfoPtr pInfo = lpConnector_->get_message(addr_.get_ip_address());
		if (pInfo!= NULL)
		{
			peerId_ = pInfo->peerId_;
			putQ(pInfo->mb_);
			delete pInfo;
			lpMgr_->peer_info_list_.remove(peerId_); //连接成功后，从mCache中去掉
		}
		

	}
	//现在mb已经被proactive管理
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
	if (mq_.size() >= lpMgr_->config_.max_peer_queue_size_) //如果超过最大的待处理消息数量
	{
		//因为第一个消息也许已经处于被发送状态，又因为第二个消息可能是pushdata的消息体，所以移除第三个消息。
		ACE_Message_Block *lpMbt_= *(mq_.begin()+2);
		lpMbt_->release();
		mq_.erase(mq_.begin()+2);
	}
	mq_.push_back(out_msg);
	if (isConnected && mq_.size() == 1)
	{
		//投递一个写
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
	case cmd_join_r: //不是在同一继承路线上，所以要单独处理一下
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
			//TODO:这里可能会有并发冲突的危险
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
			peerId_ = lpCmd->cmd_hdr_.peerId_; //更新peerId .作为被连接放，更新主动连接过来的peer的ID值。
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
	//投递新的头操作
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
		if (mb->length() == sizeof(SEGMENT_ID)) //是push data消息，特殊处理
		{
			SEGMENT_ID seg_id_;
			ACE_OS::memcpy(&seg_id_,mb->rd_ptr(),sizeof(SEGMENT_ID));

			//出栈
			(*mq_.begin())->release();
			mq_.erase(mq_.begin());
			mb = NULL;

			ACE_Message_Block *lpSeg = this->lpMgr_->buffer_.get_data(seg_id_);
			if(lpSeg == NULL) //已过期
			{
				goto lblStart; //处理下一个数据
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

				//序列化。返回的是复合消息
				mb = cmd.serialize(get_next_sequence(),lpMgr_->chId_,lpMgr_->peerId_);

				mq_.insert(mq_.begin(),lpSeg->duplicate()); // 消息体
				mb->cont(NULL); //拆开复合消息
				mq_.insert(mq_.begin(),mb); //消息头
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
	ACE_Guard<ACE_Thread_Mutex> guard(mq_mutex_); //这里复用mq_mutex也许不合适
	applied_segs_.push_back(id);
}

void Peer::pop_applied_seg_id( SEGMENT_ID id )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mq_mutex_); //这里复用mq_mutex也许不合适
	list<SEGMENT_ID>::iterator it = find(applied_segs_.begin(),applied_segs_.end(),id);
	if (it !=applied_segs_.end() )
	{
		applied_segs_.erase(it);
	}
}

bool Peer::has_applied( SEGMENT_ID id )
{
		ACE_Guard<ACE_Thread_Mutex> guard(mq_mutex_); //这里复用mq_mutex也许不合适
		 return find(applied_segs_.begin(),applied_segs_.end(),id) != applied_segs_.end();
}

