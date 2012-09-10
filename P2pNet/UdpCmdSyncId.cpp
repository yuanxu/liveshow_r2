#include "StdAfx.h"
#include "UdpCmdSyncId.h"
#include "StreamWriter.h"
#include "ace/Message_Block.h"
#include "UdpCmdSyncIdR.h"
#include "PeerMgr.h"
#include "ace/SOCK_Dgram.h"

UdpCmdSyncId::UdpCmdSyncId(void):UdpCmdBase()
{
	cmd_hdr_.cmd_ = udp_sync_id;
}

UdpCmdSyncId::~UdpCmdSyncId(void)
{
}

int UdpCmdSyncId::process( PeerMgr &mgr,UdpListenerTask &udp_task_,ACE_INET_Addr &remote_addr_ )
{
	UdpCmdSyncIdR cmdr(cmd_hdr_) ;
	cmdr.start_seg_id_ = mgr.buffer_.start_seg_id_;
	ACE_Message_Block *mb_ = cmdr.serialize(cmdr.cmd_hdr_.seqId_,cmd_hdr_.chId_,cmd_hdr_.peerId_);
	//udp.send(mb_->rd_ptr(),mb_->length(),remote_addr_);
	udp_task_.send(mb_->rd_ptr(),mb_->length(),remote_addr_);
	mb_->release();
	return LS_SUCCEEDED;
}

void UdpCmdSyncId::deserialize(const char* buffer_,ssize_t len_ )
{
	ACE_OS::memcpy(&cmd_hdr_,buffer_,SIZE_OF_CMD_HEADER);
}

ACE_Message_Block* UdpCmdSyncId::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	UdpCmdBase::serialize(seqId,chId,peerId);
	StreamWriter wr;
	wr.write((char*)&cmd_hdr_,SIZE_OF_CMD_HEADER);

	return wr.get_message_block();
}
