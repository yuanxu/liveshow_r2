#pragma once
#include "cmdbase.h"
#include "MediaHeader.h"

class CmdPushHeader :
	public CmdBase
{
public:
	CmdPushHeader(void);
	CmdPushHeader(MediaHeader& hdr);
	~CmdPushHeader(void);

	virtual ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId);
	virtual void deserialize(ACE_Message_Block* lmb_);
	virtual int process(PeerMgr &mgr,Peer &peer);

private:
	MediaHeader mhdr_;
};
