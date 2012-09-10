#pragma once
#include "cmdbase.h"
class PeerMgr;
class UdpListenerTask;

class UdpCmdConnect :
	public UdpCmdBase
{
public:
	UdpCmdConnect(void);
	~UdpCmdConnect(void);
	char address[INET6_ADDRSTRLEN];

	
	//����ָ��
	virtual int process(PeerMgr &mgr,UdpListenerTask &udp_task_,ACE_INET_Addr &remote_addr_);

	//�����л�����
	virtual void deserialize(const char* buffer_,ssize_t len_);
	//���л�����
	virtual ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId) ;
};
