#include "StdAfx.h"
#include "BufferMap.h"

#include "ace/Thread_Mutex.h"
#include "ace/Guard_T.h"
#include "ace/OS.h"
#include "ace/Log_Msg.h"

BufferMap::BufferMap(void):start_seg_id_(0),bits_(),mutex_()
{
}

BufferMap::~BufferMap(void)
{
	
}


int BufferMap::set_bit( SEGMENT_ID segId )
{
	if (segId < start_seg_id_ || segId >= start_seg_id_ + MAX_SEGMENT_COUNT)//out of range
	{
		return -1;
	}
	else
	{
		ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
		bits_.set(segId - start_seg_id_);
		return 0;
	}
}

bool BufferMap::get_bit( SEGMENT_ID segId )
{
	if (segId < start_seg_id_ || segId >= start_seg_id_ + MAX_SEGMENT_COUNT) // out of range
	{
		return false;
	}
	else
	{
		
		return bits_.at(segId - start_seg_id_);
	}
}

const char* BufferMap::get_bytes()
{
	char* p;
	ACE_NEW_NORETURN(p,char[BM_SIZE]);
	int loc_ = 0; //位置指针
	
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	ACE_OS::memcpy(p+loc_,&start_seg_id_,sizeof(SEGMENT_ID));
	loc_ += sizeof(SEGMENT_ID);

	bitset<8> bs_t;
	for(int i =0;i < MAX_SEGMENT_COUNT ;i+=8)
	{
		bs_t.reset();
		for(int j=0;j<8;j++)
		{
			bs_t[j] = bits_[i];
		}
		u_char v = static_cast<u_char>(bs_t.to_ulong());
		ACE_OS::memcpy(p+loc_,&v,1);
		loc_ ++;
	}

	return p;
}

void BufferMap::from_bytes( const char* p )
{
	int loc_ =0;
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);

	ACE_OS::memcpy(&start_seg_id_ ,p + loc_,sizeof(SEGMENT_ID));
	loc_ = sizeof(SEGMENT_ID);
	bits_.reset();
	for(int i =0;i < BM_SIZE - sizeof(SEGMENT_ID) ; i++ )
	{
		u_char c ;
		ACE_OS::memcpy(&c,p+loc_,1);
		loc_++;
		bitset<8> bs_t(c);
		for(int j = 0; j < 8 ;j++)
		{
			bits_[i*8 +j ] = bs_t[j];
		}
	}
}

void BufferMap::time_step()
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);

	start_seg_id_ ++; //向前移动一秒


	//ACE_DEBUG ((LM_DEBUG, "********************\n"));
	//ACE_DEBUG ((LM_DEBUG, "bufer time_step called\n"));

	if (bits_[0])
	{
		//ACE_DEBUG ((LM_DEBUG, "%s = %b\n", "start_id:", start_seg_id_ - 1));
	}
	else
	{
		ACE_DEBUG ((LM_DEBUG, "%s = %b BUT HAS NO DATA!!!\n", "start_id:", start_seg_id_ - 1));
	}
	//ACE_DEBUG ((LM_DEBUG, "********************\n"));

	for(int i = 0; i < MAX_SEGMENT_COUNT - 1;i++)
	{
		bits_[i] = bits_[i+1];
	}
	bits_.reset(MAX_SEGMENT_COUNT -1 );
}

void BufferMap::init( SEGMENT_ID seqId )
{
	start_seg_id_ = seqId;
	bits_.reset();
}