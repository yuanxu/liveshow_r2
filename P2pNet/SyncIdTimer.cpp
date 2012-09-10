#include "StdAfx.h"
#include "SyncIdTimer.h"
#include "ace/SOCK_Dgram.h"
#include "UdpCmdSyncId.h"
#include "PeerMgr.h"
#include "UdpListenerTask.h"
SyncIdTimer::SyncIdTimer(PeerMgr &p,UdpListenerTask &p2):ACE_Handler(),mgr_(p),UdpTask_(p2)
{
	
}

SyncIdTimer::~SyncIdTimer(void)
{

}

void SyncIdTimer::handle_time_out( const ACE_Time_Value &tv,const void *arg )
{
	UdpCmdSyncId cmd;
	static ACE_UINT32 seqId_ = 0;
	if (seqId_ == 0xffffffff)
	{
		seqId_ = 0;
	}
	else
	{
		seqId_++;
	}
	ACE_Message_Block *mb_ =cmd.serialize(seqId_,mgr_.chId_,mgr_.peerId_);
	
	UdpTask_.send(*mb_,mgr_.server_addr_);
	mb_->release();
}