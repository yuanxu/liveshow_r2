#include "StdAfx.h"

#include "UdpCmdConnect.h"
UdpCmdConnect::UdpCmdConnect(void)
{
	cmd_hdr_.cmd_ = udp_connect;
}

UdpCmdConnect::~UdpCmdConnect(void)
{
}


int UdpCmdConnect::process(PeerMgr &mgr, UdpListenerTask &udp_task_, ACE_INET_Addr &remote_addr_)
{
return 0;
}
void UdpCmdConnect::deserialize(const char *buffer_, ssize_t len_)
{

}

ACE_Message_Block* UdpCmdConnect::serialize(ACE_UINT32 seqId, ACE_UINT32 chId, PeerID peerId)
{
return NULL;
}