#pragma once
#include "cmdbase.h"
#include <vector>
using namespace std;

class CmdApplyData :
	public CmdBase
{
public:
	CmdApplyData(void);
	~CmdApplyData(void);

	virtual ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId);
	virtual void deserialize(ACE_Message_Block* lmb_);
	virtual int process(PeerMgr &mgr,Peer &peer);

	vector<SEGMENT_ID> seg_ids_;
};
