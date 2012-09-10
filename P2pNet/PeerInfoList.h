#pragma once
#include "types.h"
#include <map>
#include "ace/Thread_Mutex.h"
#include "ace/Guard_T.h"
#include "PeerInfo.h"
using namespace std;

/************************************************************************/
/* ���mCache���ݡ���������peer_list_���ظ�
/************************************************************************/
class PeerInfoList
{
public:
	PeerInfoList(void);
	~PeerInfoList(void);
	void insert(PeerInfoPtr p);
	void insert(PeerID id,ACE_UINT32 addr,ACE_UINT16 port);
	void remove(PeerID id);
	bool exits(PeerID id);
	size_t size();
	void destory();
	PeerInfoPtr get_peerInfo(PeerID id);

	void open_cursor();
	void close_cursor();
	void reset_cursor();
	PeerInfoPtr next();
private:
	ACE_Thread_Mutex mutex_;
	typedef map<PeerID,PeerInfoPtr> map_entity;
	map_entity map_;
	map_entity::iterator curr_it_;
};
