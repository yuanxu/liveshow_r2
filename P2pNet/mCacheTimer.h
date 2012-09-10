#pragma once
#include "ace/asynch_io.h"

class PeerMgr;

class mCacheTimer :
	public ACE_Handler
{
public:
	mCacheTimer(PeerMgr &p);
	~mCacheTimer(void);

	virtual void handle_time_out (const ACE_Time_Value &tv,const void *arg);

	long timer_id_;

private:

	PeerMgr &mgr_;
};
