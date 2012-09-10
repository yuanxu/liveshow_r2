#include "StdAfx.h"
#include "mCacheTimer.h"
#include "PeerMgr.h"
#include "CmdPing.h"
#include "ace/Message_Block.h"
#include "CmdJoin.h"
#include "UdpCmdCallBack.h"
mCacheTimer::mCacheTimer(PeerMgr &p):mgr_(p),timer_id_(0)
{
}

mCacheTimer::~mCacheTimer(void)
{
}

void mCacheTimer::handle_time_out( const ACE_Time_Value &tv,const void *arg )
{
	CmdPing cmd;
	cmd.source_peer_id_ = mgr_.peerId_;
	cmd.source_peer_ip_ = mgr_.local_addr_.get_ip_address();
	cmd.source_peer_port_ = mgr_.tcp_port_;
	cmd.ttl_ = 6;
	cmd.tol_ = 0;
	static ACE_UINT32 seqId_ = 0;
	if (seqId_ == 0xffffffff)
	{
		seqId_ = 0;
	}
	seqId_ ++;

	//断开低效节点

	if (mgr_.peer_list_.size() >= mgr_.config_.max_peer_list_size_ )
	{
		mgr_.peer_list_.sort();//倒序排序
		for (int i = 0 ; i < mgr_.config_.max_disconnect_count_ && mgr_.peer_list_.size()>1 ; i++)
		{
			mgr_.peer_list_.pop_back() ;/**/
		}
	}

	// Ping
	
	ACE_Message_Block *pmb_ = cmd.serialize(seqId_,mgr_.chId_,mgr_.peerId_);
	mgr_.peer_list_.openCursor();
	PeerPtr p ;
	while ((p =mgr_.peer_list_.next()) != NULL)
	{
		if (!p->is_self_ ) //不发给自己
		{
			p->putQ(pmb_->duplicate());
		}
	}
	mgr_.peer_list_.closeCursor();
	pmb_->release();

	//检查mCache，不能有过期的
	vector<PeerInfoPtr> vecpInfo_;
	mgr_.peer_info_list_.open_cursor();
	PeerInfoPtr p2;
	int iConnect = 0;
	while ((p2 = mgr_.peer_info_list_.next()) != NULL)
	{
		if (p2->ttl_ < ACE_OS::gettimeofday())
		{
			vecpInfo_.push_back(p2);
		}
		else if (!mgr_.is_server_ && iConnect < mgr_.config_.max_connect_count_ && !p2->is_connecting_) //连接
		{
			
			p2->is_connecting_ = true;
			iConnect++;
			if (p2->is_behind_nat_) //如果是nat后边，则调用proxy 发起连接指令
			{
				if (!mgr_.is_behind_nat_)
				{
					UdpCmdCallBack cmd;
					ACE_Message_Block *msg = cmd.serialize(mgr_,seqId_,mgr_.chId_,mgr_.peerId_);
					mgr_.Udp_task_.send_to_proxy_server(*msg);
				}
				else //双方都是在nat后边，nothing to do
				{

				}
			}
			else
			{
				ACE_INET_Addr addr(p2->port_number_,p2->address_);
				mgr_.lpConnector_->make_connect(p2->peerId_,addr); 
			}
		}
	}
	//删除节点
	mgr_.peer_info_list_.close_cursor();
	while (!vecpInfo_.empty())
	{
		p2 = *vecpInfo_.begin();
		mgr_.peer_info_list_.remove(p2->peerId_);
		vecpInfo_.erase(vecpInfo_.begin());
	}

	
}