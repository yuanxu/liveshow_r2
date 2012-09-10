#include "StdAfx.h"
#include "UdpCmdHelloR.h"
#include "StreamReader.h"
#include "PeerMgr.h"
UdpCmdHelloR::UdpCmdHelloR(void):call_back_cmd_()
{
	cmd_hdr_.cmd_ =udp_hello_r;
}

UdpCmdHelloR::~UdpCmdHelloR(void)
{
	while (!call_back_cmd_.empty())
	{
		delete *call_back_cmd_.begin();
		call_back_cmd_.erase(call_back_cmd_.begin());
	}
}

int UdpCmdHelloR::process( PeerMgr &mgr,UdpListenerTask &udp_task_,ACE_INET_Addr &remote_addr_ )
{
	for (vector<CallBackCmdInfo*>::iterator it = call_back_cmd_.begin() ; it!= call_back_cmd_.end() ; it++)
	{
		CallBackCmdInfo * info = *it;
		if (info->chId_ == mgr.chId_ && !mgr.peer_list_.exist(info->peer_id_)) //同一频道，且没有链接。发起主动链接
		{
			ACE_INET_Addr remote_addr_(info->port_,info->ip_);
			mgr.lpConnector_->make_connect(info->peer_id_,remote_addr_);
		}
	}
	return LS_SUCCEEDED;
}

void UdpCmdHelloR::deserialize( const char* buffer_,ssize_t len_ )
{
	StreamReader reader((char*)buffer_,len_);
	//读出头

	ACE_OS::memcpy(&cmd_hdr_,reader.read(SIZE_OF_CMD_HEADER),SIZE_OF_CMD_HEADER);
	len_ -= SIZE_OF_CMD_HEADER;
	for (int i =0 ;i< len_ / sizeof(CallBackCmdInfo) ; i++)
	{
		CallBackCmdInfo *info = new CallBackCmdInfo();
		info->cmd_ = reader.read_uint32();
		info->peer_id_ = reader.read_uint32();
		info->ip_ = reader.read_uint32();
		info->port_ = reader.read_uint16();
		info->chId_ = reader.read_uint32();
		call_back_cmd_.push_back(info);
	}
}

ACE_Message_Block* UdpCmdHelloR::serialize( ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId )
{
	//客户端不需要
	return NULL;
}