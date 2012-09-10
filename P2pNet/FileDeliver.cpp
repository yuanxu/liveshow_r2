#include "StdAfx.h"
#include "FileDeliver.h"
#include "PeerMgr.h"
FileDeliver::FileDeliver(void):IDeliver(NULL)
{
}

FileDeliver::FileDeliver( PeerMgr *p ):IDeliver(p)
{
	lpFile= new ofstream("1.asf" ,ios::out|ios::binary);
}
FileDeliver::~FileDeliver(void)
{
	lpFile->close();
}

void FileDeliver::got_one_second_data( SEGMENT_ID segId,ACE_Message_Block* mb_ )
{
	if (mb_ == NULL) //sync Id
	{
		sentinel_ = segId;
	}
	else //Push Data
	{
		if (!delivered_header_ && lpMgr->mhdr_.len_>0)
		{
			//Push Header
			lpFile->write(lpMgr->mhdr_.data_,lpMgr->mhdr_.len_);
			
			delivered_header_ = true;
		}
		
		ACE_ASSERT(*mb_->rd_ptr() == (char)0x82 && *(mb_->rd_ptr()+1) == (char)0x0  && *(mb_->rd_ptr()+2) == (char)0x0  );
		if ( segId == sentinel_)
		{
			sentinel_++;
			lpFile->write(mb_->rd_ptr(),mb_->length());
			ACE_ASSERT(*mb_->rd_ptr() == (char)0x82 && *(mb_->rd_ptr()+1) == (char)0x0  && *(mb_->rd_ptr()+2) == (char)0x0  );
		}
		mb_->release();
	}
}
