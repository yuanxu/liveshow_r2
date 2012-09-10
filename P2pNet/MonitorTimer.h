#pragma once
#include "ace/Asynch_IO.h"

class PeerMgr;

class MonitorTimer :
	public ACE_Handler
{
public:
	MonitorTimer(PeerMgr &p);
	~MonitorTimer(void);

	virtual void handle_time_out (const ACE_Time_Value &tv,const void *arg);

	long timer_id_;

private:

	PeerMgr &mgr_;
};
