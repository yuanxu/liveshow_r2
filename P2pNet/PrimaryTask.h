#pragma once
#include "ace/Task.h"
class PeerMgr;
/*
*	����������(�߳�)
*
*/
class PrimaryTask :
	public ACE_Task_Base
{
public:
	PrimaryTask(PeerMgr *p);
	~PrimaryTask(void);
	virtual int svc();
	virtual int close(u_long flags /* = 0 */);

	PeerMgr * lpMgr_;
};

