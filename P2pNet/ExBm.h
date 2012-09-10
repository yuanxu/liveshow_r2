#pragma once
#include "ace/Asynch_IO.h"
class PeerMgr;
class ExBm :
	public ACE_Handler
{
public:
	ExBm(PeerMgr *p);
	~ExBm(void);

	virtual void handle_time_out (const ACE_Time_Value &tv,const void *arg);
	
	long timer_id_;

private:

	PeerMgr *lpMgr_;
};
