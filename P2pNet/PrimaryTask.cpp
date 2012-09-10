#include "StdAfx.h"
#include "PrimaryTask.h"
#include "ace/Proactor.h"
#include "PeerMgr.h"

PrimaryTask::PrimaryTask(PeerMgr *p):ACE_Task_Base(),lpMgr_(p)
{
}

PrimaryTask::~PrimaryTask(void)
{
}

int PrimaryTask::svc()
{
	lpMgr_->lpProactor_->run_event_loop();
	return 0;
}

int PrimaryTask::close( u_long flags /* = 0 */ )
{
	lpMgr_->lpProactor_->end_event_loop();
	
	//this->wait();
	return 0;
}