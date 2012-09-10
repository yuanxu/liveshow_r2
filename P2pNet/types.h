#pragma once
#include "ace/Basic_Types.h"
//#pragma pack(1)
/**
* Segment Id
*/
typedef ACE_UINT32 SEGMENT_ID;

/**
*
*/

typedef ACE_UINT32 PeerID;

enum CmdEnum : ACE_UINT32
{
	//未知
	cmd_unknow		= 0x000000,

	//加入指令
	cmd_join		= 0x000001,

	//推送头
	cmd_push_header	= 0x000002,

	//推送节点
	cmd_push_peers	= 0x000003,

	//申请数据
	cmd_apply_data	= 0x000004,

	//推送数据
	cmd_push_data	= 0x000005,

	//交换buffer map
	cmd_ex_bm		= 0x000006,

	//ping
	cmd_ping		= 0x000007,

	//要求断开连接
	cmd_ask_for_disconnect = 0x000008,

	//加入指令
	cmd_join_r		= 0x80000001,

	//ping
	cmd_ping_r		= 0x000007,
	
	cmd_pong		= 0x80000007,


	/************************************************************************/
	/*  media source                                                                     */
	/***********************************************************************/
	ms_push_header	= 0x00010001,
	ms_push_data	= 0x00010002,
	ms_push_data_end =0x00010003,


	/************************************************************************/
	/* Udp Commnad                                                                     */
	/************************************************************************/
	udp_sync_id	  = 0x00020001,
	udp_sync_id_r	  = 0x80020001,
	udp_connect	  = 0x00020002,
	udp_connect_r = 0x80020002,
	udp_hello		  = 0x00020003,
	udp_hello_r	  = 0x80020003,
	udp_callback  = 0x00020004,
	udp_callback_r = 0x80020004
};

/**
*	消息头定义
*/
typedef struct  
{
	/*
	*	指令
	*/
	CmdEnum cmd_;

	/*
	*	序列号
	*/
	ACE_UINT32 seqId_;

	/*
	*	频道Id
	*/
	ACE_UINT32 chId_;

	/*
	* 发送节点Id
	*/
	PeerID peerId_;

	/*
	*	长度
	*/
	ACE_UINT32 len_;
} CmdHeader;

/*
*	指令头长度
*/
#define SIZE_OF_CMD_HEADER	sizeof(CmdHeader)
#define MAX_SEGMENT_COUNT  64		// Segmnet中最大包数
#define BM_SIZE	(MAX_SEGMENT_COUNT /8 + sizeof(SEGMENT_ID))		//Buffer Map长度 7.5+2	。必须确保MAX_SEGMENT_COUNT % 8 ==0
#define LS_SUCCEEDED -1
#define LS_FAILED 0


#define MAX_LOWID		0x1000000 //最大的LowID
#define IsHighID(id)	(id > MAX_LOWID) //是否是HighID
#define IsLowID(id)		(id <= MAX_LOWID)


