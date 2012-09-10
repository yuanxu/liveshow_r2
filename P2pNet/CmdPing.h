#pragma once
#include "cmdbase.h"

class CmdPing :
	public CmdBase
{
public:
	CmdPing(void);
	~CmdPing(void);

	int process(PeerMgr &mgr,Peer &peer);

	void deserialize(ACE_Message_Block* lmb_);
	ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId);

	PeerID source_peer_id_;
	ACE_UINT32 source_peer_ip_;
	ACE_UINT16 source_peer_port_;
	char ttl_,tol_;
};
