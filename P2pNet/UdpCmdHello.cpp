#include "StdAfx.h"
#include "UdpCmdHello.h"

UdpCmdHello::UdpCmdHello(void)
{
	cmd_hdr_.cmd_ = udp_hello;
}

UdpCmdHello::~UdpCmdHello(void)
{
}

int UdpCmdHello::process( PeerMgr &mgr,UdpListenerTask &udp_task_,ACE_INET_Addr &remote_addr_ )
{
	return LS_SUCCEEDED;//�ͻ��˲���Ҫ���������Ϣ
}

void UdpCmdHello::deserialize( const char* buffer_,ssize_t len_ )
{
	//�ͻ��˲���Ҫ���������Ϣ
}

ACE_Message_Block* UdpCmdHello::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	UdpCmdBase::serialize(seqId,chId,peerId);
	StreamWriter wr;
	wr.write((char*)&cmd_hdr_,SIZE_OF_CMD_HEADER);

	return wr.get_message_block();
}

