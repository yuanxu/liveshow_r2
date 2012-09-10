#include "StdAfx.h"
#include "CmdJoin.h"
#include "ace/OS.h"
#include "CmdPushHeader.h"
#include "CmdPushPeers.h"
#include "CmdJoinR.h"
#include "CmdExBm.h"
#include "PeerMgr.h"
#include "CmdAskForDisconnect.h"

CmdJoin::CmdJoin(void):CmdBase()
{
	cmd_hdr_.cmd_ = cmd_join;
}

CmdJoin::~CmdJoin(void)
{
}

int CmdJoin::process(PeerMgr &mgr ,Peer &peer)
{
	//�������״̬
	int iret = LS_SUCCEEDED;

	peer.tcp_port_ = tcp_port_;
	peer.udp_port_ = udp_port_;

	//����join_r
	CmdJoinR jr(cmd_hdr_,0,mgr.version_);
	jr.ServerId_ = mgr.peerId_;
	jr.start_id_ = mgr.buffer_.start_seg_id_;
	jr.is_server_ = (char)mgr.is_server_;
	if (mgr.chId_ != cmd_hdr_.chId_)
	{
		jr.result_ = 3;//channel Id��ƥ��
		peer.putQ(jr.serialize());
		
	}
	else
	{

		peer.putQ(jr.serialize());

		//�����Ƿ�������붼Ҫ����push_peers
		if (mgr.is_server_)
		{
			CmdPushPeers cpprs(mgr.peer_list_);
			peer.putQ(cpprs.serialize(peer.get_next_sequence(),mgr.chId_,mgr.peerId_));
			//����ͷ
			CmdPushHeader cphdr(mgr.mhdr_);
			peer.putQ(cphdr.serialize(peer.get_next_sequence(),mgr.chId_,mgr.peerId_));
		}
		peer.is_behind_nat_ = IsLowID(peer.peerId_);
		if (mgr.peer_list_.size() < mgr.config_.max_peer_list_size_  && !(mgr.is_server_ && peer.is_behind_nat_)) //�������
		{
			////����������ӡ�����뵽�������б��С���Peer��open���Ѿ�����
			////PeerPtr p =&peer;

			if(mgr.peer_list_.join(&peer) == LS_SUCCEEDED) //succeed
			{
				//����������ӣ�����ExBm��Ϣ
				CmdExBm exBm;
				exBm.lpData_ = mgr.mhdr_.data_;
				peer.putQ(exBm.serialize(peer.get_next_sequence(),mgr.chId_,mgr.peerId_));
			}
			else
			{
				iret = LS_FAILED;
				//delete &peer;
			}
			
		}//�����������������ask for disconnect��Ϣ
		else
		{
			CmdAskForDisconnect cmd;
			peer.putQ(cmd.serialize(peer.get_next_sequence(),mgr.chId_,mgr.peerId_));
		}

	}
	return iret;
}

ACE_Message_Block* CmdJoin::serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId)
{
	CmdBase::serialize(seqId,chId,peerId);

	StreamWriter wr;
	wr.write((char*)&cmd_hdr_,SIZE_OF_CMD_HEADER);

	wr.write_uint16(tcp_port_);
	wr.write_uint16(udp_port_);
	wr.write_uint16(version_);

	//���뱾����IP��ַ�б�.what's this mean?

	return wr.get_message_block();
}

void CmdJoin::deserialize( ACE_Message_Block* lmb_ )
{
	StreamReader reader(lmb_);
	//����ͷ

	ACE_OS::memcpy(&cmd_hdr_,reader.read(SIZE_OF_CMD_HEADER),SIZE_OF_CMD_HEADER);

	tcp_port_	=reader.read_uint16();
	udp_port_	=reader.read_uint16();
	version_		= reader.read_uint16();
	
}