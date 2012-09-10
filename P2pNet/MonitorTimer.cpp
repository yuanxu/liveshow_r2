#include "StdAfx.h"
#include "MonitorTimer.h"
#include "PeerMgr.h"
#include <iostream>
MonitorTimer::MonitorTimer(PeerMgr &p):mgr_(p)
{
}

MonitorTimer::~MonitorTimer(void)
{
}

void MonitorTimer::handle_time_out( const ACE_Time_Value &tv,const void *arg )
{
	if (mgr_.peer_list_.size() <= 1)//û�����ӵ������ڵ�
	{
		std::cout << "ReReconnecting..." << std::endl;
		mgr_.connect_to_server();
	}
}