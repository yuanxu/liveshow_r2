#pragma once
#include "cmdbase.h"


class CmdPushData :
	public CmdBase
{
public:
	CmdPushData(void);
	~CmdPushData(void);


	int process(PeerMgr &mgr,Peer &peer);

	void deserialize(ACE_Message_Block* lmb_);
	ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId);

	SEGMENT_ID seg_id_;
	ACE_Message_Block *mb_;
};
