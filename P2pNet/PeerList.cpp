#include "StdAfx.h"
#include "PeerList.h"
#include <algorithm>

PeerList::PeerList(void)
:mutex_(NULL),vec_(),currCursor_(0)
{
	mutex_=new ACE_Thread_Mutex();
}

PeerList::~PeerList(void)
{
	delete mutex_;
}

void PeerList::openCursor()
{
	mutex_->acquire_read();
	currCursor_ = 0;
}

void PeerList::closeCursor()
{
	mutex_->release();
	
}

PeerPtr PeerList::next()
{
	if (currCursor_ < vec_.size() )
	{
		return vec_[currCursor_++];
	}
	else
	{
		return NULL;	
	}
}

void PeerList::pop_back()
{
	delete vec_.back();
}

int PeerList::join( PeerPtr p )
{
	if (getPeer(p->peerId_) != NULL && p->peerId_ != 0) //已经存在了
	{
		ACE_DEBUG ((LM_DEBUG, "********************\n"));
		ACE_DEBUG ((LM_DEBUG, "peer_list.join called\n"));
		ACE_DEBUG ((LM_DEBUG, "peer(%d) has already existed \n", p->peerId_));
		ACE_DEBUG ((LM_DEBUG, "********************\n"));
		//delete p;
		return LS_FAILED;
	}
	ACE_Guard<ACE_Thread_Mutex> guard(*mutex_);
	vec_.push_back(p);
	ACE_DEBUG ((LM_DEBUG, "********************\n"));
	ACE_DEBUG ((LM_DEBUG, "peer_list.join called\n"));
	ACE_DEBUG ((LM_DEBUG, "peer(%d) has joined to peer_list \n", p->peerId_));
	ACE_DEBUG ((LM_DEBUG, "********************\n"));
	p->is_joined_ = true;
	return LS_SUCCEEDED;
}

void PeerList::leave( PeerPtr p )
{
	ACE_Guard<ACE_Thread_Mutex> guard(*mutex_);
	for (vector<PeerPtr>::iterator it = vec_.begin() ; it != vec_.end() ; it++)
	{
		if(*it == p)
		{
			vec_.erase(it);
			ACE_DEBUG ((LM_DEBUG, "********************\n"));
			ACE_DEBUG ((LM_DEBUG, "peer_list.leave called\n"));
			ACE_DEBUG ((LM_DEBUG, "peer(%d) has left from peer_list \n", p->peerId_));
			ACE_DEBUG ((LM_DEBUG, "********************\n"));
			return;
		}
	}
}

void PeerList::destory()
{
	//ACE_Guard<ACE_Thread_Mutex> guard(*mutex_);
	while (!vec_.empty())
	{
		delete *vec_.begin();

//		vec_.erase(vec_.begin());
	}
}

PeerPtr PeerList::getPeer( PeerID id )
{
	ACE_Guard<ACE_Thread_Mutex> guard(*mutex_);
	for (vector<PeerPtr>::iterator it = vec_.begin() ; it != vec_.end() ; it++)
	{
		if ((*it)->peerId_ == id)
		{
			return *it;
		}
	}
	return NULL;
}

size_t PeerList::size()
{
	return vec_.size();
}

void PeerList::resetCursor()
{
	currCursor_ = 0;
}

bool PeerList::exist( PeerID id )
{
	return getPeer(id) != NULL;
}

void PeerList::sort()
{
	ACE_Guard<ACE_Thread_Mutex> guard(*mutex_);
	std::sort(vec_.begin(),vec_.end(),&PeerList::less_second); //倒序排列。能力强排在前边
}

void PeerList::pop_begin()
{
	delete *(vec_.begin()+1);
}