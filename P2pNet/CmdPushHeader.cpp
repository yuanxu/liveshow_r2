#include "StdAfx.h"
#include "CmdPushHeader.h"
#include "PeerMgr.h"


CmdPushHeader::CmdPushHeader(void)
{
	cmd_hdr_.cmd_ = cmd_push_header;
}
CmdPushHeader::CmdPushHeader(MediaHeader& hdr)
{
	cmd_hdr_.cmd_ = cmd_push_header;
	mhdr_.set_data(hdr.data_,hdr.len_);
	
	//mhdr_.len_=hdr.len_;
}

CmdPushHeader::~CmdPushHeader(void)
{
}

ACE_Message_Block* CmdPushHeader::serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId)
{
	CmdBase::serialize(seqId,chId,peerId);
	StreamWriter wr;
	wr.write((char*)&cmd_hdr_,SIZE_OF_CMD_HEADER);
	wr.write(mhdr_.data_,mhdr_.len_);
	return wr.get_message_block();
}

void CmdPushHeader::deserialize( ACE_Message_Block* lmb_ )
{
	StreamReader reader(lmb_);
	//∂¡≥ˆÕ∑

	ACE_OS::memcpy(&cmd_hdr_,reader.read(SIZE_OF_CMD_HEADER),SIZE_OF_CMD_HEADER);
	mhdr_.len_ = cmd_hdr_.len_ - SIZE_OF_CMD_HEADER;
	mhdr_.set_data(lmb_->rd_ptr(),mhdr_.len_);
	lmb_->rd_ptr(mhdr_.len_);
	
}

int CmdPushHeader::process( PeerMgr &mgr,Peer &peer )
{
	//…Ë÷√segment size

	mgr.mhdr_.set_data(mhdr_.data_,mhdr_.len_);
	return LS_SUCCEEDED;
}