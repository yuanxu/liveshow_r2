#include "StdAfx.h"
#include "CmdExBm.h"
#include "StreamWriter.h"
#include "StreamReader.h"
#include "Peer.h"

CmdExBm::CmdExBm():lpData_(NULL)
{
	cmd_hdr_.cmd_ = cmd_ex_bm;
}

CmdExBm::~CmdExBm(void)
{
}

ACE_Message_Block* CmdExBm::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	CmdBase::serialize(seqId,chId,peerId);
	StreamWriter wr;
	wr.write((char*)&cmd_hdr_,SIZE_OF_CMD_HEADER);
	
	wr.write(lpData_,BM_SIZE);

	return wr.get_message_block();
}

void CmdExBm::deserialize( ACE_Message_Block* lmb_ )
{
	ACE_OS::memcpy(&cmd_hdr_,lmb_->rd_ptr(),SIZE_OF_CMD_HEADER);
	lmb_->rd_ptr(SIZE_OF_CMD_HEADER);
	
	lpData_ =new char[BM_SIZE];
	ACE_OS::memcpy(lpData_,lmb_->rd_ptr(),BM_SIZE);
	lmb_->rd_ptr(BM_SIZE);
}

int CmdExBm::process( PeerMgr &mgr,Peer &peer )
{
	peer.bm_.from_bytes(lpData_);
	//处理完后。数据将不可用
	delete[] lpData_;
	lpData_ = NULL;
	return LS_SUCCEEDED;	
}