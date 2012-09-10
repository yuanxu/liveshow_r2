#pragma once
#include "cmdbase.h"
#include "types.h"
#include "ace/Thread_Mutex.h"

class CmdJoin :
	public CmdBase
{
public:
	CmdJoin(void);
	~CmdJoin(void);


	int process(PeerMgr &mgr,Peer &peer);

	void deserialize(ACE_Message_Block* lmb_);
	ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId);

	ACE_UINT16 tcp_port_,udp_port_;
	ACE_UINT16 version_;


};
