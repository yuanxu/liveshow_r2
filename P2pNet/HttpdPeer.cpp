#include "StdAfx.h"
#include "HttpdPeer.h"
#include "ace/Guard_T.h"
#include "ace/OS.h"
#include "HttpdPeerList.h"
#include "PeerMgr.h"
#include "MediaHeader.h"
#include "ace/Guard_T.h"

const char* HTTP_RESPONSE =  "HTTP/1.0 200 OK\r\nServer: LiveShow(R2 ACE)\r\nCache-Control: no-cache\r\n\r\n";

HttpdPeer::HttpdPeer(PeerMgr*p ):IDeliver(p),connect_succeed_(false),lp_httpd_list_(NULL),bIsIniting_(false),mq_(),mutex_()
{
}

HttpdPeer::HttpdPeer():IDeliver(),connect_succeed_(false),lp_httpd_list_(NULL),bIsIniting_(false),mq_(),mutex_()
{

}
HttpdPeer::~HttpdPeer(void)
{
	if(this->handle()!=ACE_INVALID_HANDLE)
	{
		reader_.cancel();
		writer_.cancel();
		ACE_OS::closesocket(this->handle());	
	}
	lp_httpd_list_->leave(this);
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	while (!mq_.empty())
	{
		mq_.front()->release();
		mq_.pop_front();
	}
}

void HttpdPeer::got_one_second_data(SEGMENT_ID segId, ACE_Message_Block* mb_ )
{
	if (connect_succeed_)
	{
		if (mb_ == NULL) //sync Id
		{
			sentinel_ = segId;
		}
		else //Push Data
		{
			ACE_ASSERT(mb_->base()[0] != '\0');
			if (/*current_mb_ == NULL &&*/ segId == sentinel_) //这里有漏洞。需要在httpdpeer中增加待下发队列功能
			{
				sentinel_++;
				if (mb_ != NULL)
				{
					//current_mb_ = mb_->duplicate();
					//init_write();
					putQ(mb_->duplicate());
					init_write();
				}
			}
			mb_->release();
		}		
	}
}

void HttpdPeer::open( ACE_HANDLE h,ACE_Message_Block& )
{
	handle(h); //保存handler

	//初始化读写工厂类
	if(reader_.open(*this) !=0 || writer_.open(*this) != 0)
	{
		ACE_ERROR((LM_ERROR,ACE_TEXT("%p\n"),
			ACE_TEXT("HttpPeer open")));;
		delete this;
		return;
	}
	//发起一个读操作
	init_read();

	//现在mb已经被proactive管理
	return;
}

void HttpdPeer::handle_read_stream( const ACE_Asynch_Read_Stream::Result &result )
{

	if (!result.success() || result.bytes_transferred() == 0)
	{
		ACE_ERROR ((LM_ERROR,
			"%p ",
			"HttpdPeer::Read"));
		ACE_OS::printf("%d\n",ACE_OS::last_error());
		delete this;
		
	}
	else
	{
		//write response
		if (connect_succeed_)
		{
			init_read();
			return;
		}
		ACE_Message_Block *lpMb_ = NULL;
		ACE_NEW_NORETURN(lpMb_,ACE_Message_Block(HTTP_RESPONSE,ACE_OS::strlen(HTTP_RESPONSE)));
		lpMb_->wr_ptr(ACE_OS::strlen(HTTP_RESPONSE));
		putQ(lpMb_ );
		init_write();

		init_read();
		connect_succeed_ = true;
		bIsIniting_ =true;
		sentinel_ =0;
	}
}

void HttpdPeer::handle_write_stream( const ACE_Asynch_Write_Stream::Result &result )
{
	ACE_Message_Block& mb=result.message_block();
	size_t unsent_data;

	if (result.success())
	{

		unsent_data = result.bytes_to_write() - result.bytes_transferred();
		if (unsent_data != 0) //未写完
		{
			// Reset pointers
			result.message_block ().rd_ptr (result.bytes_transferred ());

			// Duplicate the message block and retry remaining data
			if (writer_.write (*result.message_block ().duplicate (),
				unsent_data) == -1)
			{
				ACE_ERROR ((LM_ERROR,
					"%p ",
					"HttpPeer::Write"));
				ACE_OS::printf("%d\n",ACE_OS::last_error());
				result.message_block().release();
				return;
			}
			result.message_block().release();
		}
		else
		{
			ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
			mb.release();

			if (bIsIniting_)
			{
				if (!delivered_header_ && lpMgr->mhdr_.len_>0)
				{
					//Push Header

					ACE_Message_Block *lpMb = NULL;
					ACE_NEW_NORETURN(lpMb,ACE_Message_Block(lpMgr->mhdr_.data_,lpMgr->mhdr_.len_));
					lpMb->wr_ptr(lpMgr->mhdr_.len_);
					
					putQ(lpMb);
					
					//ACE_Time_Value tv;
					//tv.msec(300);//
					//ACE_OS::sleep(tv);
					delivered_header_ = true;
				}
				else
				{
					
					sentinel_ = lpMgr->buffer_.start_seg_id_;	
					while(lpMgr->buffer_.get_data(sentinel_)!= NULL)
					{
						ACE_Message_Block *mb_ = lpMgr->buffer_.get_data(sentinel_);
						if (mb_!=NULL)
						{
							mq_.push_back(mb_->duplicate());
						}
						
						sentinel_++;
					}
				
					bIsIniting_ = false;
					
				}
			}
			
			init_write();
			
		}
	}   

}

void HttpdPeer::init_read()
{
	ACE_Message_Block *mb_;
	ACE_NEW_NORETURN(mb_,ACE_Message_Block(512));
	reader_.read(*mb_,512);
}

void HttpdPeer::init_write()
{
	if (mq_.empty() )
	{
		return;
	}
	ACE_Message_Block *mb_ = mq_.front();
	mq_.pop_front();

	ACE_ASSERT(mb_->length()>0);
	if(writer_.write(*mb_,mb_->length()) == -1)
	{
		ACE_ERROR((LM_ERROR,ACE_TEXT("%p "),
			ACE_TEXT("HttpdPeer init_write")));;
		ACE_OS::printf("%d\n",ACE_OS::last_error());
	//	return -1;
	}
}

void HttpdPeer::putQ( ACE_Message_Block *mb )
{
	ACE_Guard<ACE_Thread_Mutex> guard(mutex_);
	mq_.push_back(mb);
}