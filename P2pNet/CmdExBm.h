#pragma once
#include "cmdbase.h"
#include "BufferMap.h"

class CmdExBm :
	public CmdBase
{
public:
	CmdExBm();
	~CmdExBm(void);

	virtual ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId);
	virtual void deserialize(ACE_Message_Block* lmb_);
	virtual int process(PeerMgr &mgr,Peer &peer);
	char *lpData_;
	
};
