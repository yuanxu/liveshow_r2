#pragma once
#include "BufferMap.h"
#include "types.h"
#include "ace/Message_Block.h"
#include "ace/Thread_Mutex.h"
#include <map>

using namespace std;

class HttpdPeerList;

class Buffer
{
public:
	Buffer(void);
	~Buffer(void);

	
	void put_data(SEGMENT_ID segId,const char* p,size_t len);

	void put_data(SEGMENT_ID segId,ACE_Message_Block* mb_);

	SEGMENT_ID start_seg_id_;

	ACE_Message_Block* get_data(SEGMENT_ID segId);

	void sync_Id(SEGMENT_ID segId);
	void destory();

	void init(SEGMENT_ID segId);

	BufferMap bm_;
	ACE_Thread_Mutex mutex_;

	HttpdPeerList *lp_httpd_list_;
private:
	//´æ´¢segment
	map<SEGMENT_ID,ACE_Message_Block*> map_;

};
