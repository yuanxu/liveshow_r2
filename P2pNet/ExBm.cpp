#include "StdAfx.h"
#include "ExBm.h"
#include "PeerMgr.h"
#include "Peer.h"
#include "cmdExBm.h"

ExBm::ExBm(PeerMgr *p):timer_id_(0),lpMgr_(p),ACE_Handler()
{
}

ExBm::~ExBm(void)
{
}

void ExBm::handle_time_out (const ACE_Time_Value &tv,const void *arg)
{
	//ACE_DEBUG ((LM_DEBUG, "%T Exchange BufferMap called.\n"));
	//向已经连接的节点交换BM信息
	lpMgr_->peer_list_.openCursor();
	Peer* p =NULL;
	//char* lpBm =mgr_.buffer_.bm_.get_bytes();
	CmdExBm cmdEx;
	cmdEx.lpData_ = (char*)lpMgr_->buffer_.bm_.get_bytes();
	while (true)
	{
		p = lpMgr_->peer_list_.next();
		if (p == NULL)
		{
			break;
		}
		if (!p->is_self_ && p->isConnected && !p->is_server_)
		{
			p->putQ(cmdEx.serialize(p->get_next_sequence(),lpMgr_->chId_,lpMgr_->peerId_));
		}
	}
	lpMgr_->peer_list_.closeCursor();
	delete[] cmdEx.lpData_;
//	return 0;
}

