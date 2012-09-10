#pragma once
#include "ace/Basic_Types.h"
#include "ace/Message_Block.h"
#include "ace/OS.h"
#include "ace/INET_Addr.h"
#include "types.h"
#include "StreamWriter.h"
#include "StreamReader.h"

class PeerMgr;
class Peer;

class CmdBase
{
public:
	CmdBase(void);
	virtual ~CmdBase(void);

	CmdHeader cmd_hdr_;
	//ACE_Message_Block* mb_;

	//处理指令
	virtual int process(PeerMgr &mgr,Peer &peer)=0;

	//反序列化数据
	virtual void deserialize(ACE_Message_Block* lmb_)=0;
	//序列化数据
	virtual ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId);
};
//////////////////////////////////////////////////////////////////////////
class CmdRespBase 
{
public:
	CmdRespBase(CmdHeader& hdr):cmd_hdr_()
	{
		ACE_OS::memcpy(&cmd_hdr_,&hdr,SIZE_OF_CMD_HEADER);
		cmd_hdr_.cmd_ = (CmdEnum)(0x80000000 | cmd_hdr_.cmd_);
	}
	CmdRespBase():cmd_hdr_()
	{

	}
	virtual ~CmdRespBase()
	{}
	//序列化数据
	virtual ACE_Message_Block* serialize()=0 ;


	CmdHeader cmd_hdr_;
	//ACE_Message_Block* mb_;

	//处理指令
	virtual int process(PeerMgr &mgr,Peer &peer)=0;

	//反序列化数据
	virtual void deserialize(ACE_Message_Block* lmb_)=0;

};

//////////////////////////////////////////////////////////////////////////
class UdpListenerTask;
class UdpCmdBase
{
public:
	UdpCmdBase(void):cmd_hdr_()
	{};
	virtual ~UdpCmdBase(void)
	{};

	CmdHeader cmd_hdr_;
	//ACE_Message_Block* mb_;

	//处理指令
	virtual int process(PeerMgr &mgr,UdpListenerTask &udp_task_,ACE_INET_Addr &remote_addr_)=0;

	//反序列化数据
	virtual void deserialize(const char* buffer_,ssize_t len_)=0;
	//序列化数据
	virtual ACE_Message_Block* serialize(ACE_UINT32 seqId,ACE_UINT32 chId,PeerID peerId) ;
};
class UdpCmdRespBase :public UdpCmdBase
{
public:
	UdpCmdRespBase(void):UdpCmdBase()
	{
	};
	UdpCmdRespBase(CmdHeader &hdr):UdpCmdBase()
	{
		ACE_OS::memcpy(&cmd_hdr_,&hdr,SIZE_OF_CMD_HEADER);
		cmd_hdr_.cmd_ = (CmdEnum)(0x80000000 | cmd_hdr_.cmd_);
	};
	virtual ~UdpCmdRespBase(void)
	{
	};
};