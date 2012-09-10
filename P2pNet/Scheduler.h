#pragma once
#include "ace/Asynch_IO.h"
#include "types.h"
#include <vector>

class Peer;
class PeerMgr;

using namespace std;

#define MAX_SUPPLIER 1

class Scheduler :
	public ACE_Handler
{
public:
	Scheduler(PeerMgr &mgr);
	~Scheduler(void);
	
	virtual void handle_time_out(const ACE_Time_Value &current_time, const void *act /* = 0 */);
	long timer_id_;
private:

	PeerMgr &mgr_;
};
