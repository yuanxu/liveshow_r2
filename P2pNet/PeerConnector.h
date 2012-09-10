#pragma once
#include "ace/Asynch_Connector.h"
#include "ace/INET_Addr.h"
#include "ace/Message_Block.h"
#include "ace/Thread_Mutex.h"
#include <map>
#include "types.h"
#include "PeerInfo.h"

class PeerMgr;
class Peer;

using namespace std;
class PeerConnector :
	public ACE_Asynch_Connector<Peer>
{
public:
	PeerConnector(PeerMgr &mgr);
	~PeerConnector(void);

	//************************************
	// Method:    make_handler
	// FullName:  PeerAcceptor::make_handler
	// Access:    public 
	// Returns:   Peer*
	// Qualifier: 检查连接数目，返回NULL拒绝连接
	// Parameter: void
	//************************************
	virtual Peer* make_handler(void);

	virtual int validate_connection(const ACE_Asynch_Connect::Result& result, const ACE_INET_Addr &remote, const ACE_INET_Addr& local);


	//************************************
	// Method:    connect
	// FullName:  PeerConnector::make_connect
	// Access:    public 
	// Returns:   int
	// Qualifier: 建立一个新的连接.
	// Parameter: ACE_INET_Addr &remote_addr_
	// Parameter: ACE_Message_Block *mb_
	//************************************
	int make_connect(PeerID peerId,ACE_INET_Addr &remote_addr_,ACE_Message_Block *mb_);

	int make_connect(PeerID peerId,ACE_INET_Addr &remote_addr_);

	PeerInfoPtr get_message(ACE_UINT32 remote_address_);
private:

	PeerMgr &mgr_;
	map<ACE_UINT32,PeerInfoPtr> map_;
	ACE_Thread_Mutex mutex_;
};
