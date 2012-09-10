#include "StdAfx.h"
#include "SourcePin.h"
#include "PeerMgr.h"
#include "StreamReader.h"
#include "ace/OS.h"
#include "ace/Proactor.h"
#include "ace/Proactor_Impl.h"

SourcePin::SourcePin():mb_(SRC_BUF_SIZE),lpMgr_(NULL)
{
}

SourcePin::~SourcePin(void)
{
	lpMgr_->lpSourcePin_=NULL;
	ACE_DEBUG ((LM_DEBUG,
		"A source pin has exited.\n"));

}

void SourcePin::open( ACE_HANDLE new_handle, ACE_Message_Block &message_block )
{
	ACE_DEBUG ((LM_DEBUG,
			"A source pin has connected.\n"));

	handle(new_handle);
	//初始化读写工厂类
	if(reader_.open(*this) !=0 || writer_.open(*this) != 0)
	{
		ACE_ERROR((LM_ERROR,ACE_TEXT("%p\n"),
			ACE_TEXT("SourcePin open")));;
		delete this;
		return;
	}
	//连接时附带了数据
	if (message_block.length()!=0)
	{
		// Duplicate the message block so that we can keep it around.
		ACE_Message_Block &duplicate =
			*message_block.duplicate ();

		// Fake the result so that we will get called back.
		ACE_Asynch_Read_Stream_Result_Impl *fake_result =
			ACE_Proactor::instance ()->create_asynch_read_stream_result (this->proxy (),
			this->handle_,
			duplicate,
			SRC_BUF_SIZE,
			0,
			ACE_INVALID_HANDLE,
			0,
			0);

		size_t bytes_transferred = message_block.length ();

		// <complete> for Accept would have already moved the <wr_ptr>
		// forward. Update it to the beginning position.
		duplicate.wr_ptr (duplicate.wr_ptr () - bytes_transferred);

		// This will call the callback.
		fake_result->complete (message_block.length (),
			1,
			0);

		// Zap the fake result.
		delete fake_result;
	}
	//发起一个读操作
	if (reader_.read(mb_,mb_.space()) != 0 )
	{
		ACE_ERROR((LM_ERROR,ACE_TEXT("%p\n"),
			ACE_TEXT("SourcePin begin read")));

		delete this;
		return;
	}


	//现在mb已经被proactive管理
	return;
}

void SourcePin::handle_read_stream( const ACE_Asynch_Read_Stream::Result &result )
{
	//ACE_DEBUG ((LM_DEBUG,
	//	"source pin handle_read_stream called\n"));

	//// Reset pointers.
	//result.message_block ().rd_ptr ()[result.bytes_transferred ()] = '\0';

	//ACE_DEBUG ((LM_DEBUG, "********************\n"));
	//ACE_DEBUG ((LM_DEBUG, "%s = %d\n", "bytes_to_read", result.bytes_to_read ()));
	//ACE_DEBUG ((LM_DEBUG, "%s = %d\n", "handle", result.handle ()));
	//ACE_DEBUG ((LM_DEBUG, "%s = %d\n", "bytes_transfered", result.bytes_transferred ()));
	//ACE_DEBUG ((LM_DEBUG, "%s = %d\n", "act", (u_long) result.act ()));
	//ACE_DEBUG ((LM_DEBUG, "%s = %d\n", "success", result.success ()));
	//ACE_DEBUG ((LM_DEBUG, "%s = %d\n", "completion_key", (u_long) result.completion_key ()));
	//ACE_DEBUG ((LM_DEBUG, "%s = %d\n", "error", result.error ()));
	//ACE_DEBUG ((LM_DEBUG, "********************\n"));
	if (!result.success() || result.bytes_transferred() == 0) 
	{
		
		//result.message_block().release();
		delete this;
		return;
	}
	static bool got_hdr_=false;
	static SEGMENT_ID segId=0;
	ACE_Message_Block& mb =result.message_block();
	//刚才读到的是头
	CmdHeader hdr_;
	if (!got_hdr_)
	{
		
		size_t len=mb.length();
		if (len < SIZE_OF_CMD_HEADER)
		{
			reader_.read(mb, mb.space());
		}
		else
		{
			//ACE_ASSERT(len == SIZE_OF_CMD_HEADER);
			ACE_OS::memcpy(&hdr_,mb.rd_ptr(),SIZE_OF_CMD_HEADER);
			//读取包体
			//reader_.read(mb,hdr_.len_ - SIZE_OF_CMD_HEADER);
			got_hdr_=true;
		}
	}
	if(got_hdr_) //包体
	{
		
		ACE_OS::memcpy(&hdr_,mb.rd_ptr() ,SIZE_OF_CMD_HEADER);
		
		size_t data_len_ = mb.length() ;

		if (data_len_ < hdr_.len_) //继续读
		{
			reader_.read(mb,mb.space());
		}
		else
		{
			result.message_block().rd_ptr(SIZE_OF_CMD_HEADER);
			hdr_.len_ -= SIZE_OF_CMD_HEADER;
	
			switch(hdr_.cmd_)
			{
			case ms_push_header://header
				lpMgr_->mhdr_.set_data(mb.rd_ptr(),hdr_.len_);//
				break;
			case ms_push_data_end:
			case ms_push_data://data
			
				lpMgr_->buffer_.put_data(segId++,mb.rd_ptr(),hdr_.len_);
				break;
			default:
				ACE_ASSERT(1==0);
				break;
			}
			mb.rd_ptr(hdr_.len_);
			mb.crunch();
			/*mb.rd_ptr(mb.base());
			mb.wr_ptr(mb.base());*/
			got_hdr_=false;
			if(reader_.read(mb, mb.space()) ==-1)
			{
				ACE_ERROR((LM_ERROR,ACE_TEXT("%N:%l:%p\n"),
					ACE_TEXT("SourcePin has Got data,and begin read")));

				delete this;
				return;
			}
		}
	}
}

void SourcePin::handle_write_stream( const ACE_Asynch_Write_Stream::Result &result )
{

}

void SourcePin::set_peerMgr( PeerMgr* p )
{
	lpMgr_ = p;
}