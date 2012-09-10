#pragma once
#include "ace/task.h"
#include "ace/SOCK_Dgram.h"
#include "ace/Message_Block.h"
/*
*	Udp¼àÌýÏß³Ì
*/

class PeerMgr;

class UdpListenerTask :
	public ACE_Task_Base
{
public:
	UdpListenerTask(PeerMgr &p);
	~UdpListenerTask(void);

	virtual int svc();
	virtual int close(u_long flags /* = 0 */);

	void send(char* buf_,size_t len_ ,ACE_INET_Addr &remote_addr_);
	void send(ACE_Message_Block &msg,ACE_INET_Addr &remote_addr_);
	void send_to_proxy_server(ACE_Message_Block &msg);
private:
	PeerMgr &mgr_;
	bool isRunning_;
	ACE_SOCK_Dgram udp;
};
