#include "StdAfx.h"
#include "ProxyHelloTimer.h"
#include "PeerMgr.h"
#include "UdpCmdHello.h"

ProxyHelloTimer::ProxyHelloTimer( PeerMgr &p,UdpListenerTask &udp_task_ ):mgr_(p),UdpTask_(udp_task_)
{

}
ProxyHelloTimer::~ProxyHelloTimer(void)
{
}

void ProxyHelloTimer::handle_time_out( const ACE_Time_Value &tv,const void *arg )
{
	UdpCmdHello cmd;
	static ACE_UINT32 seqId_ = 0;
	if (seqId_ == 0xffffffff)
	{
		seqId_ = 0;
	}
	else
	{
		seqId_++;
	}
	ACE_Message_Block* mb = cmd.serialize(seqId_,mgr_.chId_,mgr_.peerId_);
	UdpTask_.send_to_proxy_server(*mb);
}