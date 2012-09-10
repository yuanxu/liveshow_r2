#include "StdAfx.h"
#include "StreamReader.h"
#include "ace/OS.h"
StreamReader::StreamReader(ACE_Message_Block* mb):mb_(mb)
{
}

StreamReader::StreamReader( char buf[],ssize_t len )
{
	mb_=new ACE_Message_Block(buf,len);
}
StreamReader::~StreamReader(void)
{
	//mb_->release();
}

char StreamReader::read_byte()
{
	char r;
	ACE_OS::memcpy(&r,mb_->rd_ptr(),1);
	mb_->rd_ptr(1);
	return r;
}

ACE_VERSIONED_NAMESPACE_NAME::ACE_UINT16 StreamReader::read_uint16()
{
	ACE_UINT16 r;
	ACE_OS::memcpy(&r,mb_->rd_ptr(),2);
	mb_->rd_ptr(2);
	return r;
}

ACE_VERSIONED_NAMESPACE_NAME::ACE_UINT32 StreamReader::read_uint32()
{
	ACE_UINT32 r;
	ACE_OS::memcpy(&r,mb_->rd_ptr(),4);
	mb_->rd_ptr(4);
	return r;
}

ACE_VERSIONED_NAMESPACE_NAME::ACE_UINT64 StreamReader::read_uint64()
{
	ACE_UINT64 r;
	ACE_OS::memcpy(&r,mb_->rd_ptr(),8);
	mb_->rd_ptr(8);
	return r;
}

char* StreamReader::read( size_t len )
{
	char* p ;
	ACE_NEW_NORETURN(p,char[len]);

	ACE_OS::memcpy(p,mb_->rd_ptr(),len);
	mb_->rd_ptr(len);
	return p;
}