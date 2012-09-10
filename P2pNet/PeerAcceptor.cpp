#include "StdAfx.h"
#include "PeerAcceptor.h"
#include "PeerMgr.h"

PeerAcceptor::PeerAcceptor(PeerMgr &mgr):mgr_(mgr)
{
	
}

PeerAcceptor::~PeerAcceptor(void)
{
	stop();
}

void PeerAcceptor::stop()
{
	this->cancel();
}

int PeerAcceptor::start(ACE_INET_Addr &addr_)
{

	if( 0 != open(addr_ ,
		0,	//bytes_to_read
		1,	//pass_address
		5, //MAX block count
		1,	//reuse_addr
		mgr_.lpProactor_ ,	//proactor
		1))	//validate_new_connection
	{
		ACE_ERROR_RETURN((LM_ERROR,ACE_TEXT("%p\n"),ACE_TEXT("PeerAcceptor open")),1);
	}
	else
	{
		return 0;
	}

}


//************************************
// Method:    make_handler
// FullName:  PeerAcceptor::make_handler
// Access:    public 
// Returns:   Peer*
// Qualifier: 检查连接数目，返回NULL拒绝连接
// Parameter: void
//************************************
Peer* PeerAcceptor::make_handler( void )
{
	//TODO:连接数量检查

	Peer *p = new Peer();
	p->set_peerMgr(&mgr_);

	return p;
}




int PeerAcceptor::validate_connection( const ACE_Asynch_Accept::Result& result, const ACE_INET_Addr &remote, const ACE_INET_Addr& local )
{

	if (!mgr_.is_server_ && mgr_.peer_list_.size() > mgr_.config_.max_peer_list_size_)
	{
		return -1;
	}
	else
	{
		return 0;
	}
}


