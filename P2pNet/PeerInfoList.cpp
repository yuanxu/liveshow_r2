#include "StdAfx.h"
#include "PeerInfoList.h"

PeerInfoList::PeerInfoList(void):mutex_(NULL),map_(),curr_it_(map_.end())
{
}

PeerInfoList::~PeerInfoList(void)
{
	destory();
}

void PeerInfoList::insert( PeerInfoPtr p )
{
	
	if (!exits(p->peerId_))
	{
		ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
		map_.insert(map_entity::value_type(p->peerId_,p));
	}
}

void PeerInfoList::insert( PeerID id,ACE_UINT32 addr,ACE_UINT16 port )
{
	PeerInfoPtr p = new PeerInfo();
	p->peerId_ = id;
	p->address_ = addr;
	p->port_number_ = port;
	insert(p);
}

void PeerInfoList::remove( PeerID id )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	map_entity::iterator it = map_.find(id);
	if (it != map_.end())
	{
		delete it->second;
		map_.erase(it);
	}
}

bool PeerInfoList::exits( PeerID id )
{

	return map_.find(id) != map_.end();
}

size_t PeerInfoList::size()
{
	return map_.size();
}

void PeerInfoList::destory()
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	while ( !map_.empty())
	{
		delete map_.begin()->second;
		map_.erase(map_.begin());
	}
}

PeerInfoPtr PeerInfoList::get_peerInfo( PeerID id )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	map_entity::iterator it = map_.find(id);
	return it == map_.end() ? NULL :it->second ;
}

void PeerInfoList::open_cursor()
{
	mutex_.acquire();
	curr_it_ = map_.begin();
}

void PeerInfoList::close_cursor()
{
	mutex_.release();
	curr_it_ = map_.end();
}

void PeerInfoList::reset_cursor()
{
	curr_it_ = map_.begin();
}

PeerInfoPtr PeerInfoList::next()
{
	PeerInfoPtr p = NULL;
	if (curr_it_ == map_.end())
	{
		return p;
	}
	else
	{
		p = curr_it_->second;
		curr_it_ ++;
		return p;
	}
}