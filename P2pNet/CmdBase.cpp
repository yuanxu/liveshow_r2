#include "StdAfx.h"
#include "CmdBase.h"

CmdBase::CmdBase(void)//:mb_(NULL)
{
}

CmdBase::~CmdBase(void)
{
}

ACE_Message_Block* CmdBase::serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId)
{
	//cmd_hdr_.seqId_ = 
	
	cmd_hdr_.seqId_ =seqId;
	cmd_hdr_.chId_ = chId;
	cmd_hdr_.peerId_ = peerId;
	return NULL;
}

ACE_Message_Block* UdpCmdBase::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	cmd_hdr_.seqId_ =seqId;
	cmd_hdr_.chId_ = chId;
	cmd_hdr_.peerId_ = peerId;
	return NULL;
}