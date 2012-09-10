#include "StdAfx.h"
#include "CmdApplyData.h"
#include "StreamWriter.h"
#include "StreamReader.h"
#include "ace/OS.h"
#include "Peer.h"
#include <algorithm>

CmdApplyData::CmdApplyData(void)
{
	cmd_hdr_.cmd_ = cmd_apply_data;
}

CmdApplyData::~CmdApplyData(void)
{
}

ACE_Message_Block* CmdApplyData::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	CmdBase::serialize(seqId,chId,peerId);
	StreamWriter wr;
	wr.write((char*)&cmd_hdr_,SIZE_OF_CMD_HEADER);
	for (vector<SEGMENT_ID>::iterator it = seg_ids_.begin() ; it != seg_ids_.end() ; it++)
	{
		wr.write_uint32(*it);
	}
	return wr.get_message_block();
}

void CmdApplyData::deserialize( ACE_Message_Block* lmb_ )
{
	StreamReader rdr(lmb_);
	ACE_OS::memcpy(&cmd_hdr_,rdr.read(SIZE_OF_CMD_HEADER),SIZE_OF_CMD_HEADER);

	while (lmb_->length() > 0)
	{
		seg_ids_.push_back(rdr.read_uint32());
	}
}

int CmdApplyData::process( PeerMgr &mgr,Peer &peer )
{
	//²úÉúpush_dataÃüÁî
	
	sort(seg_ids_.begin(),seg_ids_.end());
	for (vector<SEGMENT_ID>::iterator it = seg_ids_.begin() ; it != seg_ids_.end() ; it++)
	{
		ACE_Message_Block* mb_;
		ACE_NEW_NORETURN(mb_,ACE_Message_Block(sizeof(SEGMENT_ID)));
		ACE_OS::memcpy(mb_->wr_ptr(),&(*it),sizeof(SEGMENT_ID));
		mb_->wr_ptr(sizeof(SEGMENT_ID));
		peer.putQ(mb_);
	}

	return LS_SUCCEEDED;
}