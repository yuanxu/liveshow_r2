#pragma once
#include "cmdbase.h"

class CmdJoinR :
	public CmdRespBase
{
public:
	CmdJoinR(CmdHeader& hdr);
	CmdJoinR();
	CmdJoinR(CmdHeader& hdr,char result,ACE_UINT16 ver);
	~CmdJoinR(void);

	virtual ACE_Message_Block* serialize();
	virtual void deserialize(ACE_Message_Block* lmb_);
	virtual int process(PeerMgr &mgr,Peer &peer);

	/**
	* 连接结果。
	* 0:成功
	* 1:因版本过低被拒绝
	* 2:因服务器连接过多被拒绝，但随后会发送peers信息及media header信息。客户端在收到header后应主动断开与服务器连接
	* 3:ChannelID不匹配
	*/
	char result_;

	/**
	* 被连接方版本号
	*/
	ACE_UINT16 version_;

	//被连接服务器的ID
	PeerID ServerId_;

	//服务器当前的segmnet id
	SEGMENT_ID start_id_;

	//被连接者是否是服务器
	char is_server_;

};
