#pragma once
#include "cmdbase.h"
#include "types.h"
#include "ace/Message_Block.h"

class CmdAskForDisconnect :
	public CmdBase
{
public:
	CmdAskForDisconnect(void);
	~CmdAskForDisconnect(void);

	int process(PeerMgr &mgr,Peer &peer);

	void deserialize(ACE_Message_Block* lmb_);
	ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId);
};
