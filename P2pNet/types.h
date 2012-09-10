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
	//δ֪
	cmd_unknow		= 0x000000,

	//����ָ��
	cmd_join		= 0x000001,

	//����ͷ
	cmd_push_header	= 0x000002,

	//���ͽڵ�
	cmd_push_peers	= 0x000003,

	//��������
	cmd_apply_data	= 0x000004,

	//��������
	cmd_push_data	= 0x000005,

	//����buffer map
	cmd_ex_bm		= 0x000006,

	//ping
	cmd_ping		= 0x000007,

	//Ҫ��Ͽ�����
	cmd_ask_for_disconnect = 0x000008,

	//����ָ��
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
*	��Ϣͷ����
*/
typedef struct  
{
	/*
	*	ָ��
	*/
	CmdEnum cmd_;

	/*
	*	���к�
	*/
	ACE_UINT32 seqId_;

	/*
	*	Ƶ��Id
	*/
	ACE_UINT32 chId_;

	/*
	* ���ͽڵ�Id
	*/
	PeerID peerId_;

	/*
	*	����
	*/
	ACE_UINT32 len_;
} CmdHeader;

/*
*	ָ��ͷ����
*/
#define SIZE_OF_CMD_HEADER	sizeof(CmdHeader)
#define MAX_SEGMENT_COUNT  64		// Segmnet��������
#define BM_SIZE	(MAX_SEGMENT_COUNT /8 + sizeof(SEGMENT_ID))		//Buffer Map���� 7.5+2	������ȷ��MAX_SEGMENT_COUNT % 8 ==0
#define LS_SUCCEEDED -1
#define LS_FAILED 0


#define MAX_LOWID		0x1000000 //����LowID
#define IsHighID(id)	(id > MAX_LOWID) //�Ƿ���HighID
#define IsLowID(id)		(id <= MAX_LOWID)


