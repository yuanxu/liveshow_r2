#include "StdAfx.h"
#include "MediaHeader.h"
#include "ace/OS.h"
#include <string>
using namespace std;
MediaHeader::MediaHeader(void):len_(0),data_(NULL)
{
}

MediaHeader::~MediaHeader(void)
{
	clear();
}

void MediaHeader::set_data(const char* pData,size_t len )
{
	if (len_ != 0 )
	{
		clear();
	}
	ACE_NEW_NORETURN(data_,char[len]);
	len_ = len;
	ACE_OS::memcpy(data_,pData,len);
}

void MediaHeader::clear()
{
	if (data_ == NULL)
	{
		return;
	}
	delete[] data_;
	data_ =NULL;
	len_ = 0;
}

ACE_UINT32 MediaHeader::get_chunk_size()
{
	ACE_UINT32 ret =0;
	ACE_OS::memcpy(&ret,data_ + find(asf_filePropObjID) + HDR_ASF_CHUNKLENGTH_4,4);
	return ret;
}

ACE_UINT32 MediaHeader::get_bit_rate()
{
	ACE_UINT32 ret =0;
	ACE_OS::memcpy(&ret,data_ + find(asf_filePropObjID) + HDR_ASF_BITRATE,4);
	return ret;
}


size_t MediaHeader::find(const unsigned char p[])
{
	string sSrc(data_,len_);
	string sSub((char*)p);
	return sSrc.find(sSub);
	
}

size_t MediaHeader::get_segment_size()
{

	return (get_bit_rate() /8 /get_chunk_size())* get_chunk_size();
}