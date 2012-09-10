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
	* ���ӽ����
	* 0:�ɹ�
	* 1:��汾���ͱ��ܾ�
	* 2:����������ӹ��౻�ܾ��������ᷢ��peers��Ϣ��media header��Ϣ���ͻ������յ�header��Ӧ�����Ͽ������������
	* 3:ChannelID��ƥ��
	*/
	char result_;

	/**
	* �����ӷ��汾��
	*/
	ACE_UINT16 version_;

	//�����ӷ�������ID
	PeerID ServerId_;

	//��������ǰ��segmnet id
	SEGMENT_ID start_id_;

	//���������Ƿ��Ƿ�����
	char is_server_;

};
