#include "StdAfx.h"
#include "Buffer.h"
#include "ace/OS.h"
#include "ace/Guard_T.h"
#include "HttpdPeerList.h"
#include "ace/Message_Block.h"

Buffer::Buffer(void) : start_seg_id_(0),mutex_(NULL),map_(),bm_(),lp_httpd_list_(NULL)
{
}

Buffer::~Buffer(void)
{
	//TODO:清除所有map中的数据

}

void Buffer::put_data( SEGMENT_ID segId,const char* p,size_t len )
{
	if (segId < start_seg_id_)
	{
		return; //过期的数据，丢弃
	}
	else if (segId >= start_seg_id_ + MAX_SEGMENT_COUNT-1) //out range
	{
		//adjust?
		sync_Id(segId-MAX_SEGMENT_COUNT + 1);//新的startId
	}
	if (!bm_.get_bit(segId))
	{
		ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
		ACE_Message_Block* mb_;
		ACE_NEW_NORETURN(mb_,ACE_Message_Block(len));
		ACE_OS::memcpy(mb_->wr_ptr(),p,len);
		mb_->wr_ptr(len);
		map_.insert(map<SEGMENT_ID,ACE_Message_Block*>::value_type(segId,mb_));
		bm_.set_bit(segId);

		if (lp_httpd_list_ != NULL)
		{
			lp_httpd_list_->deliver(segId,mb_->duplicate());
		}
	} //如果已有数据，则自动丢弃
}

void Buffer::put_data( SEGMENT_ID segId,ACE_Message_Block* mb_ )
{
	if (segId < start_seg_id_)
	{
		return; //过期的数据，丢弃
	}
	else if (segId >=start_seg_id_ + MAX_SEGMENT_COUNT-1 && !bm_.get_bit(segId)) //out range
	{
		//adjust?
		sync_Id(segId-MAX_SEGMENT_COUNT + 1);//新的startId
	}
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	if (!bm_.get_bit(segId))
	{
		map_.insert(map<SEGMENT_ID,ACE_Message_Block*>::value_type(segId,mb_));
		bm_.set_bit(segId);
		if (lp_httpd_list_ != NULL)
		{
			lp_httpd_list_->deliver(segId,mb_->duplicate());
		}

	} //如果已有数据，则自动丢弃
	else
	{
		mb_->release();
	}
}
ACE_Message_Block* Buffer::get_data( SEGMENT_ID segId )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	if (bm_.get_bit(segId))
	{
		map<SEGMENT_ID,ACE_Message_Block*>::iterator it = map_.find(segId);
		//这里应该是一定可以找到！
		ACE_ASSERT(it!=map_.end());

		return it->second->duplicate();
	}
	else
	{
		return NULL;
	}
}

void Buffer::sync_Id( SEGMENT_ID segId )
{
	//if (segId <= start_seg_id_)
	//{

	//	return;
	//}
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	if (bm_.count() == 0 )
	{
		start_seg_id_ = segId;
		bm_.init(segId);
		if (lp_httpd_list_ != NULL)
		{
			lp_httpd_list_->deliver(segId,NULL);
		}
		return;
	}
	while (segId > start_seg_id_ )
	{
		if (bm_.get_bit(start_seg_id_)) //有数据
		{
			map<SEGMENT_ID,ACE_Message_Block*>::iterator it = map_.find(start_seg_id_);
			//deliver
			if (lp_httpd_list_ != NULL && lp_httpd_list_->size() >0 )
			{
				lp_httpd_list_->deliver(start_seg_id_, it->second->duplicate());
			}
			it->second->release();  //释放掉messace block
			
			map_.erase(start_seg_id_);
		}
		else
		{
			//deliver
			if (lp_httpd_list_ != NULL )
			{
				lp_httpd_list_->deliver(start_seg_id_ , NULL);
			}
		}
		start_seg_id_++;
		bm_.time_step();
	}	
}

void Buffer::init(SEGMENT_ID seqId)
{
	bm_.init(seqId);
}


void Buffer::destory()
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	while (!map_.empty())
	{
		map_.begin()->second->release();
		map_.erase(map_.begin());
	}
	start_seg_id_ = 0;
}