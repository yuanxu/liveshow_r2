#pragma once
#include "cmdbase.h"
#include <vector>

using namespace std;
class UdpCmdHelloR :
	public UdpCmdBase
{
public:
	UdpCmdHelloR(void);
	~UdpCmdHelloR(void);

	struct CallBackCmdInfo
	{
	public:
		ACE_UINT32 cmd_;
		PeerID peer_id_;
		ACE_UINT32 ip_;
		ACE_UINT16  port_;
		ACE_UINT32 chId_;
	
	};

	vector<CallBackCmdInfo*> call_back_cmd_;
	//����ָ��
	virtual int process(PeerMgr &mgr,UdpListenerTask &udp_task_,ACE_INET_Addr &remote_addr_);

	//�����л�����
	virtual void deserialize(const char* buffer_,ssize_t len_);
	//���л�����
	virtual ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId) ;
};
