#include "StdAfx.h"
#include "StreamWriter.h"
#include "ace/os.h"
#include "types.h"

StreamWriter::StreamWriter(ACE_Message_Block **mb):mb_(*mb)
{
	if (mb_ == NULL)
	{
		ACE_NEW_NORETURN(mb_,ACE_Message_Block(BUF_SIZE));
	}
}

StreamWriter::StreamWriter()
{
	ACE_NEW_NORETURN(mb_,ACE_Message_Block(BUF_SIZE));
}
StreamWriter::~StreamWriter(void)
{
	size_t length=mb_->length();
	mb_->size(length);//重置mb大小
	mb_->reset();//重置指针
	mb_->wr_ptr(SIZE_OF_CMD_HEADER - 4); //移动到len_位置
	write_uint32(length);
	mb_->reset();//重置指针
	mb_->wr_ptr(length);
	//mb_->release();
}

 void StreamWriter::write(const  char* p,size_t len )
{
	if (mb_->space()<len)
	{
		mb_->size(mb_->capacity()+len);
	}

	ACE_OS::memcpy(mb_->wr_ptr(),p,len);
	mb_->wr_ptr(len);
}

void StreamWriter::write_byte( char c )
{
	write(&c,1);
}

void StreamWriter::write_uint16( ACE_UINT16 v )
{
	write((char*)&v,2);
}

 void StreamWriter::write_uint32( ACE_UINT32 v )
{
	write((char*)&v,4);
}

  void StreamWriter::write_uint64( ACE_UINT64 v )
{
	write((char*)&v,8);
}

 ACE_Message_Block* StreamWriter::get_message_block()
{
	return mb_;
}