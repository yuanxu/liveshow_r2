#pragma once
#include "ace/Asynch_IO.h"
#include "ace/Asynch_Acceptor.h"
#include "HttpdPeer.h"
class PeerMgr;

class HttpdAcceptor : public ACE_Asynch_Acceptor<HttpdPeer>
{
public:
	HttpdAcceptor(void);
	HttpdAcceptor(PeerMgr *p);
	~HttpdAcceptor(void);



	//************************************
	// Method:    start
	// FullName:  PeerAcceptor::start
	// Access:    public 
	// Returns:   int
	// Qualifier: 启动监听
	//************************************
	int start(ACE_INET_Addr& addr_);

	//************************************
	// Method:    stop
	// FullName:  PeerAcceptor::stop
	// Access:    public 
	// Returns:   void
	// Qualifier:　停止监听
	//************************************
	void stop();

	//************************************
	// Method:    make_handler
	// FullName:  PeerAcceptor::make_handler
	// Access:    public 
	// Returns:   Peer*
	// Qualifier: 检查连接数目，返回NULL拒绝连接
	// Parameter: void
	//************************************
	virtual HttpdPeer* make_handler(void);

	virtual int validate_connection(const ACE_Asynch_Accept::Result& result, const ACE_INET_Addr &remote, const ACE_INET_Addr& local);

private:
	PeerMgr *lpMgr_;
};
