#pragma once
#include "ace/asynch_io.h"
#include "ace/INET_Addr.h"
#include "ace/SOCK_Dgram.h"
class PeerMgr;
class UdpListenerTask;
class SyncIdTimer :
	public ACE_Handler
{
public:
	SyncIdTimer(PeerMgr &p,UdpListenerTask &udp_task_);
	~SyncIdTimer(void);

	virtual void handle_time_out (const ACE_Time_Value &tv,const void *arg);

	long timer_id_;

private:

	PeerMgr &mgr_;
	UdpListenerTask &UdpTask_;
};
