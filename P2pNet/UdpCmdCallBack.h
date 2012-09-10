#pragma once
#include "cmdbase.h"
class PeerMgr;

class UdpCmdCallBack :
	public UdpCmdBase
{
public:
	UdpCmdCallBack(void);
	~UdpCmdCallBack(void);

	//处理指令
	virtual int process(PeerMgr &mgr,UdpListenerTask &udp_task_,ACE_INET_Addr &remote_addr_);

	//反序列化数据
	virtual void deserialize(const char* buffer_,ssize_t len_);
	//序列化数据
	virtual ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId) ;
	 ACE_Message_Block* serialize(PeerMgr &mgr_,ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId) ;
};
