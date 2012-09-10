#include "StdAfx.h"
#include "SourceAcceptor.h"
#include "PeerMgr.h"

SourceAcceptor::SourceAcceptor(PeerMgr &mgr):mgr_(mgr)
{
}

SourceAcceptor::~SourceAcceptor(void)
{
	stop();
}


void SourceAcceptor::stop()
{
	this->cancel();
	
}

int SourceAcceptor::start(ACE_INET_Addr &addr_)
{

	if( 0 != open(addr_ ,
		0,	//bytes_to_read
		0,	//pass_address
		1, //MAX block count
		1,	//reuse_addr
		0,	//proactor
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
SourcePin* SourceAcceptor::make_handler( void )
{
	//连接数量检查
	if (mgr_.lpSourcePin_!=NULL)
	{
		return NULL;
	}
	else
	{
		SourcePin *p = new SourcePin();
		p->set_peerMgr(&mgr_);
		mgr_.lpSourcePin_ = p;
		return p;
	}
}




int SourceAcceptor::validate_connection( const ACE_Asynch_Accept::Result& result, const ACE_INET_Addr &remote, const ACE_INET_Addr& local )
{
	return 0;
}