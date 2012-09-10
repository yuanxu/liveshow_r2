#include "StdAfx.h"
#include "CmdAskForDisconnect.h"
#include "PeerMgr.h"

CmdAskForDisconnect::CmdAskForDisconnect(void)
{
	cmd_hdr_.cmd_ = cmd_ask_for_disconnect;
}

CmdAskForDisconnect::~CmdAskForDisconnect(void)
{
}

int CmdAskForDisconnect::process( PeerMgr &mgr,Peer &peer )
{
	peer.disConnect();
	return LS_SUCCEEDED;
}

void CmdAskForDisconnect::deserialize( ACE_Message_Block* lmb_ )
{
	StreamReader reader(lmb_);
	//¶Á³öÍ·

	ACE_OS::memcpy(&cmd_hdr_,reader.read(SIZE_OF_CMD_HEADER),SIZE_OF_CMD_HEADER);

}

ACE_Message_Block* CmdAskForDisconnect::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	CmdBase::serialize(seqId,chId,peerId);

	StreamWriter wr;
	wr.write((char*)&cmd_hdr_,SIZE_OF_CMD_HEADER);

	return wr.get_message_block();
}