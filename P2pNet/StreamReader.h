#pragma once
#include "ace/Message_Block.h"
class StreamReader
{
public:
	StreamReader(ACE_Message_Block* mb);
	StreamReader(char buf[],ssize_t len);
	~StreamReader(void);


	//************************************
	// Method:    read_byte
	// FullName:  StreamReader::read_byte
	// Access:    public 
	// Returns:   byte
	// Qualifier: 读取1字节
	//************************************
	char read_byte();

	//************************************
	// Method:    read_uint16
	// FullName:  StreamReader::read_uint16
	// Access:    public 
	// Returns:   ACE_VERSIONED_NAMESPACE_NAME::ACE_UINT16
	// Qualifier:　读取uint16
	//************************************
	ACE_UINT16 read_uint16();

	//************************************
	// Method:    read_uint32
	// FullName:  StreamReader::read_uint32
	// Access:    public 
	// Returns:   ACE_VERSIONED_NAMESPACE_NAME::ACE_UINT32
	// Qualifier:　读取uint32
	//************************************
	ACE_UINT32 read_uint32();

	//************************************
	// Method:    read_uint64
	// FullName:  StreamReader::read_uint64
	// Access:    public 
	// Returns:   ACE_VERSIONED_NAMESPACE_NAME::ACE_UINT64
	// Qualifier: 读取uint64
	//************************************
	ACE_UINT64 read_uint64();

	//************************************
	// Method:    read
	// FullName:  StreamReader::read_bytes
	// Access:    public 
	// Returns:   char*
	// Qualifier:
	// Parameter: size_t len
	//************************************
	char* read(size_t len);
private:
	ACE_Message_Block *mb_;
};
