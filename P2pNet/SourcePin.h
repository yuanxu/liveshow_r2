#pragma once
#include "ace\asynch_io.h"
#include "ace\Message_Block.h"

class Buffer;
class PeerMgr;
/*
*
*	ý��Դ���ӵ㡣ֻ������Ϊ����������ʱ��Ч����ÿ������������Ӧһ��Դ���ӵ㡣Ψһ��������SoucePinAcceptor��֤
*
*/
class SourcePin :
	public ACE_Service_Handler
{
public:
	SourcePin();
	~SourcePin(void);
	virtual void handle_read_stream(const ACE_Asynch_Read_Stream::Result &result);
	virtual void handle_write_stream(const ACE_Asynch_Write_Stream::Result &result);
	virtual void open(ACE_HANDLE new_handle, ACE_Message_Block &message_block);

	void set_peerMgr(PeerMgr* p);
private:
	Buffer *lpBuf_;
	//256KB = 256*8=2048Kbps=2Mbps
	#define  SRC_BUF_SIZE 256*1024 

	ACE_Asynch_Read_Stream reader_;
	ACE_Asynch_Write_Stream writer_;
	ACE_Message_Block mb_;
	PeerMgr *lpMgr_;
};
