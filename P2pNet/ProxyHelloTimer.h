#pragma once
#include "ace/asynch_io.h"

class PeerMgr;
class UdpListenerTask;

class ProxyHelloTimer :
	public ACE_Handler
{
public:
	ProxyHelloTimer(PeerMgr &p,UdpListenerTask &udp_task_);
	~ProxyHelloTimer(void);

	virtual void handle_time_out (const ACE_Time_Value &tv,const void *arg);

	long timer_id_;

private:

	PeerMgr &mgr_;
	UdpListenerTask &UdpTask_;
};
