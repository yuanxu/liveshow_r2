#include "StdAfx.h"
#include "CmdPushPeers.h"
#include "ace/INET_Addr.h"
#include "PeerList.h"
#include "Peer.h"
#include "PeerMgr.h"


CmdPushPeers::CmdPushPeers(PeerList &plist):CmdBase(),peer_list_(plist)
{
	cmd_hdr_.cmd_ = cmd_push_peers;
}

CmdPushPeers::~CmdPushPeers(void)
{
}

void CmdPushPeers::deserialize( ACE_Message_Block* lmb_ )
{
	StreamReader reader(lmb_);
	ACE_OS::memcpy(&cmd_hdr_,reader.read(SIZE_OF_CMD_HEADER),SIZE_OF_CMD_HEADER);

	
	for(u_int i=0;i< (cmd_hdr_.len_ -  SIZE_OF_CMD_HEADER) / (4+4+2/*peerId+address+port*/) ; i++)
	{
		PeerInfoPtr pInfo = new PeerInfo();
		pInfo->peerId_ = reader.read_uint32();
		pInfo->address_ = reader.read_uint32();
		pInfo->port_number_ = reader.read_uint16();
		peers_.push_back(pInfo);
	}
}

ACE_Message_Block* CmdPushPeers::serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId)
{
	CmdBase::serialize(seqId,chId,peerId);
	StreamWriter wr;
	wr.write((char *)&cmd_hdr_,SIZE_OF_CMD_HEADER);
	peer_list_.openCursor();
	PeerPtr p ;
	
	while (true)
	{
		p = peer_list_.next();
		if (p == NULL)
		{
			break;
		}
		wr.write_uint32(p->peerId_);// peer Id
		wr.write_uint32(p->addr_.get_ip_address());//IPv4 only!!!!
		wr.write_uint16(p->tcp_port_);//

	}
	peer_list_.closeCursor();

	return wr.get_message_block();
}

int CmdPushPeers::process( PeerMgr &mgr,Peer &peer )
{

	//把数据存入peer_info_list_中	
	for (vector<PeerInfoPtr>::iterator it = peers_.begin() ; it != peers_.end() ; it++)
	{
		PeerInfoPtr p = *it;
		if ( !mgr.peer_list_.exist(p->peerId_) && !mgr.peer_info_list_.exits(p->peerId_)) //在本地的peer列表中没有找到此节点
		{
			//添加到本地peer列表中
			p->ttl_ = ACE_OS::gettimeofday();
			p->ttl_= p->ttl_.sec()+mgr.config_.peer_info_to_live_;
			mgr.peer_info_list_.insert(p);
		}
		else
		{
			delete p;
		}
	}
	return LS_SUCCEEDED;

}