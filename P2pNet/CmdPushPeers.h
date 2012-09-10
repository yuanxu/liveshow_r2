#pragma once
#include "cmdbase.h"
#include <vector>
#include "PeerInfo.h"

class PeerList;

using namespace std;
class CmdPushPeers :
	public CmdBase
{
public:
	CmdPushPeers(PeerList &plist);
	~CmdPushPeers(void);

	virtual void deserialize(ACE_Message_Block* lmb_);
	virtual ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId);
	virtual int process(PeerMgr &mgr,Peer &peer);
private:
	/*
	指向本机PeerList的引用
	*/
	PeerList &peer_list_;


	vector<PeerInfoPtr> peers_;
};
