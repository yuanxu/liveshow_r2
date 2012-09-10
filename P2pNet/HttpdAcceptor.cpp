#include "StdAfx.h"
#include "HttpdAcceptor.h"
#include "PeerMgr.h"
#include "HttpdPeerList.h"
HttpdAcceptor::HttpdAcceptor(void) : lpMgr_(NULL)
{
}

HttpdAcceptor::HttpdAcceptor(PeerMgr *p ):lpMgr_(p)
{

}
HttpdAcceptor::~HttpdAcceptor(void)
{
}

int HttpdAcceptor::start( ACE_INET_Addr& addr_ )
{
	open(addr_ ,0,0,1,1,lpMgr_->lpProactor_);
	return 0;
}

void HttpdAcceptor::stop()
{
	cancel();
	
}

HttpdPeer* HttpdAcceptor::make_handler( void )
{
	HttpdPeer *p = new HttpdPeer(lpMgr_);
	p->lp_httpd_list_ = lpMgr_->buffer_.lp_httpd_list_;
	lpMgr_->buffer_.lp_httpd_list_->join(p);
	return p;
}

int HttpdAcceptor::validate_connection( const ACE_Asynch_Accept::Result& result, const ACE_INET_Addr &remote, const ACE_INET_Addr& local )
{
	return 0;
}