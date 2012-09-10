#pragma once
#include "ace/Message_Block.h"

#include "types.h"

class PeerMgr;
/************************************************************************/
/* Deliver基类                                                                     */
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
	//数据下发时的哨兵。存放下一个等下发数据
	SEGMENT_ID sentinel_;
};
