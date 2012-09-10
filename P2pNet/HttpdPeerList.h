#pragma once
#include "ace/Message_Block.h"
#include "ace/Thread_Mutex.h"
#include "ace/Guard_T.h"
#include <vector>
#include "IDeliver.h"

using namespace std;

class HttpdPeerList
{
public:
	HttpdPeerList(void);
	~HttpdPeerList(void);

	void join(IDeliver* p);
	void leave(IDeliver* p);
	void deliver(SEGMENT_ID segId,ACE_Message_Block *mb_);
	size_t size(){return vec_.size();};
private:
	vector<IDeliver *> vec_;
	ACE_Thread_Mutex mutex_;
};
