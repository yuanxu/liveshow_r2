#include "StdAfx.h"
#include "UdpCmdSyncIdR.h"
#include "PeerMgr.h"
#include "StreamWriter.h"
#include "ace/OS.h"

UdpCmdSyncIdR::UdpCmdSyncIdR(void):UdpCmdRespBase(),start_seg_id_(0)
{
	cmd_hdr_.cmd_ = udp_sync_id_r;
}

UdpCmdSyncIdR::UdpCmdSyncIdR( CmdHeader &hdr ):UdpCmdRespBase(hdr),start_seg_id_(0)
{

}
UdpCmdSyncIdR::~UdpCmdSyncIdR(void)
{
}

int UdpCmdSyncIdR::process( PeerMgr &mgr,UdpListenerTask &udp_task_,ACE_INET_Addr &remote_addr_ )
{
	mgr.buffer_.sync_Id(start_seg_id_);
	return LS_SUCCEEDED;
}

void UdpCmdSyncIdR::deserialize(const char* buffer_,ssize_t len_ )
{
	ACE_OS::memcpy(&cmd_hdr_,buffer_,SIZE_OF_CMD_HEADER);
	ACE_OS::memcpy(&start_seg_id_,buffer_ + SIZE_OF_CMD_HEADER ,sizeof(SEGMENT_ID));
}

ACE_Message_Block* UdpCmdSyncIdR::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	UdpCmdBase::serialize(seqId,chId,peerId);
	StreamWriter wr;
	wr.write((char*)&cmd_hdr_,SIZE_OF_CMD_HEADER);
	wr.write_uint32(start_seg_id_);
	return wr.get_message_block();
}