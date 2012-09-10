#pragma once
#include "ace/Message_Block.h"
#include "ace/Basic_Types.h"
#define BUF_SIZE 512

class StreamWriter
{
public:
	StreamWriter(ACE_Message_Block **mb);
	StreamWriter();
	~StreamWriter(void);

	//************************************
	// Method:    write_byte
	// FullName:  StreamWriter::write_byte
	// Access:    public 
	// Returns:   void
	// Qualifier:
	// Parameter: char c
	//************************************
	void write_byte( char c);

	//************************************
	// Method:    write_uint16
	// FullName:  StreamWriter::write_uint16
	// Access:    public 
	// Returns:   void
	// Qualifier:
	// Parameter: ACE_UINT16 v
	//************************************
	void write_uint16( ACE_UINT16 v);

	//************************************
	// Method:    write_uint32
	// FullName:  StreamWriter::write_uint32
	// Access:    public 
	// Returns:   void
	// Qualifier:
	// Parameter: ACE_UINT32 v
	//************************************
	void write_uint32( ACE_UINT32 v);

	//************************************
	// Method:    write_uint64
	// FullName:  StreamWriter::write_uint64
	// Access:    public 
	// Returns:   void
	// Qualifier:
	// Parameter: ACE_UINT64 v
	//************************************
	void write_uint64( ACE_UINT64 v);

	//************************************
	// Method:    write
	// FullName:  StreamWriter::write
	// Access:    public 
	// Returns:   void
	// Qualifier:
	// Parameter: char* p
	// Parameter: size_t len
	//************************************
	void write(const char* p,size_t len);

	 ACE_Message_Block* get_message_block();
private:
	ACE_Message_Block *mb_;
};
