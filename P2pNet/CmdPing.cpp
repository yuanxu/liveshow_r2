#include "StdAfx.h"
#include "CmdPing.h"
#include "StreamReader.h"
#include "StreamWriter.h"
#include "types.h"
#include "ace/Message_Block.h"
#include "PeerMgr.h"
#include "ace/OS.h"

CmdPing::CmdPing(void):ttl_(0),tol_(0),source_peer_id_(0),source_peer_ip_(0),source_peer_port_(0)
{
	cmd_hdr_.cmd_ = cmd_ping;
}

CmdPing::~CmdPing(void)
{
}

int CmdPing::process( PeerMgr &mgr,Peer &peer )
{
	ttl_ --;
	tol_ ++;
	int iret = LS_SUCCEEDED;
	if (ttl_ > 0 && source_peer_id_ != mgr.peerId_ ) //ttl>0，消息的原始节点不是本节点
	{
		PeerInfoPtr pInfo = mgr.peer_info_list_.get_peerInfo(source_peer_id_);
		if (!mgr.peer_list_.exist(source_peer_id_) && !mgr.peer_info_list_.exits(source_peer_id_)  ) //本机没有原始发送方的信息
		{

				mgr.peer_info_list_.insert(source_peer_id_,source_peer_ip_,source_peer_port_);
				pInfo = mgr.peer_info_list_.get_peerInfo(source_peer_id_);
				pInfo->is_behind_nat_ = IsLowID(source_peer_id_);
					
		}
		if (pInfo != NULL)
		{
			pInfo->ttl_ = ACE_OS::gettimeofday();
			pInfo->ttl_.sec(pInfo->ttl_.sec()+mgr.config_.peer_info_to_live_);//
		}
		//返回Pong
		//广播出去
		CmdPing cmd;
		cmd.source_peer_id_ = source_peer_id_;
		cmd.source_peer_ip_ = source_peer_ip_;
		cmd.source_peer_port_ = source_peer_port_;
		cmd.ttl_ = ttl_;
		cmd.tol_ = tol_;
		ACE_Message_Block *pmb_ = cmd.serialize(peer.get_next_sequence(),mgr.chId_,mgr.peerId_);
		mgr.peer_list_.openCursor();
		PeerPtr p ;
		while ((p =mgr.peer_list_.next()) != NULL)
		{
			if (!p->is_self_ && p->peerId_ != source_peer_id_ && p->peerId_ != peer.peerId_  && !( p->is_behind_nat_ && IsLowID(source_peer_id_))) //不发给自己，也不发给消息的原始发送者。也不能产生回路。
			{
				p->putQ(pmb_->duplicate());
			}
			
		}
		mgr.peer_list_.closeCursor();
		pmb_->release();
	}
	return iret;
}

void CmdPing::deserialize( ACE_Message_Block* lmb_ )
{
	StreamReader reader(lmb_);
	//读出头

	ACE_OS::memcpy(&cmd_hdr_,reader.read(SIZE_OF_CMD_HEADER),SIZE_OF_CMD_HEADER);
	source_peer_id_ = reader.read_uint32();
	source_peer_ip_ = reader.read_uint32();
	source_peer_port_ = reader.read_uint16();
	ttl_ = reader.read_byte();
	tol_ = reader.read_byte();

}

ACE_Message_Block* CmdPing::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	CmdBase::serialize(seqId,chId,peerId);

	StreamWriter wr;
	wr.write((char*)&cmd_hdr_,SIZE_OF_CMD_HEADER);
	wr.write_uint32(source_peer_id_);
	wr.write_uint32(source_peer_ip_);
	wr.write_uint16(source_peer_port_);
	wr.write_byte(ttl_);
	wr.write_byte(tol_);
	return wr.get_message_block();
}