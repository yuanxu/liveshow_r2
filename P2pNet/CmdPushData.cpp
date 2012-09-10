#include "StdAfx.h"
#include "CmdPushData.h"
#include "StreamWriter.h"
#include "StreamReader.h"
#include "ace/OS.h"
#include "PeerMgr.h"

CmdPushData::CmdPushData(void):seg_id_(0),mb_(NULL)
{
	cmd_hdr_.cmd_ = cmd_push_data;
}

CmdPushData::~CmdPushData(void)
{
	
}

int CmdPushData::process( PeerMgr &mgr,Peer &peer )
{
	mgr.buffer_.put_data(seg_id_,mb_->duplicate());
	mb_->release();
	peer.pop_applied_seg_id(seg_id_);
	return LS_SUCCEEDED;
}

void CmdPushData::deserialize( ACE_Message_Block* lmb_ )
{

	ACE_OS::memcpy(&cmd_hdr_,lmb_->rd_ptr(),SIZE_OF_CMD_HEADER);
	lmb_->rd_ptr(SIZE_OF_CMD_HEADER);

	ACE_OS::memcpy(&seg_id_ ,lmb_->rd_ptr(),sizeof(SEGMENT_ID));
	lmb_->rd_ptr(sizeof(SEGMENT_ID));

	size_t seg_size = cmd_hdr_.len_ - SIZE_OF_CMD_HEADER - sizeof(SEGMENT_ID);
	assert(mb_ == NULL);
	ACE_NEW_NORETURN(mb_,ACE_Message_Block(seg_size)) ;//申请新的segmnet的内存

	//复制segment数据
	ACE_OS::memcpy(mb_->wr_ptr(),lmb_->rd_ptr(),seg_size);
	mb_->wr_ptr(seg_size);
	lmb_->rd_ptr(seg_size);
}

ACE_Message_Block* CmdPushData::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	CmdBase::serialize(seqId,chId,peerId);

	assert( cmd_hdr_.cmd_ == cmd_push_data);
	cmd_hdr_.len_ = SIZE_OF_CMD_HEADER + sizeof(SEGMENT_ID) + mb_->length();

	ACE_Message_Block *lpMb_;
	ACE_NEW_NORETURN(lpMb_,ACE_Message_Block(SIZE_OF_CMD_HEADER+sizeof(SEGMENT_ID))); //header+segmentid

	//write header
	ACE_OS::memcpy(lpMb_->wr_ptr(),&cmd_hdr_,SIZE_OF_CMD_HEADER);
	lpMb_->wr_ptr(SIZE_OF_CMD_HEADER);

	//write segment id
	ACE_OS::memcpy(lpMb_->wr_ptr(),&seg_id_,sizeof(SEGMENT_ID));
	lpMb_->wr_ptr(sizeof(SEGMENT_ID));

	lpMb_->cont(mb_);
	return lpMb_;
}