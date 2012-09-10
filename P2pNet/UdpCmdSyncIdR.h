#pragma once
#include "cmdbase.h"

class UdpCmdSyncIdR :
	public UdpCmdRespBase
{
public:
	UdpCmdSyncIdR(void);
	UdpCmdSyncIdR(CmdHeader &hdr);
	~UdpCmdSyncIdR(void);

	//����ָ��
	virtual int process(PeerMgr &mgr,UdpListenerTask &udp_task_,ACE_INET_Addr &remote_addr_);

	//�����л�����
	virtual void deserialize(const char* buffer_,ssize_t len_);
	//���л�����
	virtual ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId) ;

	SEGMENT_ID start_seg_id_;
};
