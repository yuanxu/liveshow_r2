#include "StdAfx.h"
#include "HttpdPeerList.h"

HttpdPeerList::HttpdPeerList(void):mutex_(NULL),vec_()
{
}

HttpdPeerList::~HttpdPeerList(void)
{
	while (!vec_.empty())
	{
		IDeliver *p = *vec_.begin();

		delete p;
	}
}

void HttpdPeerList::join( IDeliver* p )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	vec_.push_back(p);
}

void HttpdPeerList::leave( IDeliver* p )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	for (vector<IDeliver*>::iterator it = vec_.begin() ; it != vec_.end() ; it++)
	{
		if (*it == p)
		{
			vec_.erase(it);
			return;
		}
	}
}

void HttpdPeerList::deliver(SEGMENT_ID segId, ACE_Message_Block *mb_ )
{
	if (mb_ != NULL)
	{
		ACE_ASSERT(mb_->base()[0] != '\0');
	}
	
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	for (vector<IDeliver*>::iterator it = vec_.begin() ; it != vec_.end() ; it++)
	{
		(*it)->got_one_second_data(segId,mb_ == NULL?NULL: mb_->duplicate());
	}
	if (mb_ != NULL)
	{
		mb_->release();
	}
	
}