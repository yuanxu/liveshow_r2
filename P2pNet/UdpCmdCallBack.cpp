#include "StdAfx.h"
#include "UdpCmdCallBack.h"
#include "StreamWriter.h"
#include "PeerMgr.h"

UdpCmdCallBack::UdpCmdCallBack(void)
{
	cmd_hdr_.cmd_ = udp_callback;
}

UdpCmdCallBack::~UdpCmdCallBack(void)
{
}

void UdpCmdCallBack::deserialize( const char* buffer_,ssize_t len_ )
{
	//客户端不需要
}


int UdpCmdCallBack::process( PeerMgr &mgr,UdpListenerTask &udp_task_,ACE_INET_Addr &remote_addr_ )
{
	return LS_SUCCEEDED;
}

ACE_Message_Block* UdpCmdCallBack::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	return NULL;
}

ACE_Message_Block* UdpCmdCallBack::serialize(PeerMgr& mgr_,ACE_UINT32 seqId, ACE_UINT32 chId, PeerID peerId)
{
	UdpCmdBase::serialize(seqId,chId,peerId);
	StreamWriter wr;
	wr.write((char *)&cmd_hdr_,SIZE_OF_CMD_HEADER);
	wr.write_uint32(udp_callback);
	wr.write_uint32(peerId);
	wr.write_uint32(mgr_.remote_addr_.get_ip_address());
	wr.write_uint16(mgr_.remote_addr_.get_port_number());
	wr.write_uint32(chId);
	return wr.get_message_block();
}