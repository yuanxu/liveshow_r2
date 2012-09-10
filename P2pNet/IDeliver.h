#pragma once
#include "ace/Message_Block.h"

#include "types.h"

class PeerMgr;
/************************************************************************/
/* Deliver����                                                                     */
/************************************************************************/
class IDeliver
{
public:
	IDeliver(PeerMgr *p);
	IDeliver();
	virtual ~IDeliver(void);
	virtual void got_one_second_data(SEGMENT_ID segId,ACE_Message_Block* mb_) = 0;
protected:
	PeerMgr *lpMgr;
	bool delivered_header_;
	//�����·�ʱ���ڱ��������һ�����·�����
	SEGMENT_ID sentinel_;
};
