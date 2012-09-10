#include "StdAfx.h"
#include "PeerConnector.h"
#include "Peer.h"
#include "PeerMgr.h"
#include "ace/Guard_T.h"
#include "CmdJoin.h"
PeerConnector::PeerConnector(PeerMgr& mpgr):mgr_(mpgr),mutex_(),map_()
{
}

PeerConnector::~PeerConnector(void)
{
}

Peer* PeerConnector::make_handler( void )
{
	Peer* p = new Peer(false,&mgr_);
	p->lpConnector_ = this;
	return p;
}

int PeerConnector::validate_connection( const ACE_Asynch_Connect::Result& result, const ACE_INET_Addr &remote, const ACE_INET_Addr& local )
{

	return 0;
}

//////////////////////////////////////////////////////////////////////////
//TODO:下边的方法还需要仔细确认。要么可以用PeerID做主键，这样可以合并PeerInfo的定义，要么这种机制就有问题
//
//////////////////////////////////////////////////////////////////////////

//************************************
// Method:    connect
// FullName:  PeerConnector::connect
// Access:    public 
// Returns:   void
// Qualifier: 此方法将remote_addr_作为主键，mb_作为值压入map_中。
// Parameter: ACE_INET_Addr &remote_addr_
// Parameter: ACE_Message_Block *mb_
//************************************
int PeerConnector::make_connect(PeerID peerId, ACE_INET_Addr &remote_addr_,ACE_Message_Block *mb_ )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	PeerInfoPtr pinfo = new PeerInfo();
	pinfo->peerId_ = peerId;
	pinfo->address_ = remote_addr_.get_ip_address();
	pinfo->mb_ = mb_;
	map_.insert(map<ACE_UINT32,PeerInfoPtr>::value_type(pinfo->address_,pinfo));
	int iret =connect(remote_addr_);
	if(iret != 0)
	{
		iret = ACE_OS::last_error();
	}
	return iret;
}

int PeerConnector::make_connect( PeerID peerId,ACE_INET_Addr &remote_addr_ )
{
	CmdJoin cmd;
	cmd.version_ = mgr_.version_;
	cmd.tcp_port_ = mgr_.tcp_port_;
	cmd.udp_port_ = mgr_.udp_port_;
	ACE_Message_Block* mb=cmd.serialize(0,mgr_.chId_,mgr_.peerId_);//向服务器投递Join指令
	return make_connect(peerId,remote_addr_,mb);
}

PeerInfoPtr PeerConnector::get_message( ACE_UINT32 remote_address_ )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	map<ACE_UINT32,PeerInfoPtr>::iterator it = map_.begin();
	while (it != map_.end())
	{
		if (it->first == remote_address_)
		{
			break;
		}
		else
		{
			it++;
		}
	}
	if (it == map_.end())
	{
		return NULL;
	}
	else
	{
		PeerInfo *pinfo = it->second;
		map_.erase(it);
		return pinfo;
	}
}
