#pragma once
#include "ideliver.h"
#include "ace/Asynch_IO.h"
#include "ace/Semaphore.h"
#include <list>

class HttpdPeerList;
using namespace std;

class HttpdPeer :
	public IDeliver,public ACE_Service_Handler
{
public:
	HttpdPeer(PeerMgr* p);
	HttpdPeer();
	~HttpdPeer(void);
	void got_one_second_data(SEGMENT_ID segId,ACE_Message_Block* mb_);

	//************************************
	// Method:    open
	// FullName:  Peer::open
	// Access:    virtual protected 
	// Returns:   void
	// Qualifier: 新连接建立后调用
	// Parameter: ACE_HANDLE h
	// Parameter: ACE_Message_Block&
	//************************************
	virtual void open(ACE_HANDLE h,ACE_Message_Block&);

	/**
	*	读完成时调用
	*/
	virtual void handle_read_stream(const ACE_Asynch_Read_Stream::Result &result);

	/*
	*　写完成时调用
	*/
	virtual void handle_write_stream(const ACE_Asynch_Write_Stream::Result &result);

	HttpdPeerList *lp_httpd_list_;
private:
	ACE_Asynch_Read_Stream reader_;
	ACE_Asynch_Write_Stream writer_;
	void init_read();
	void init_write();
	
	ACE_Thread_Mutex mutex_;
	bool connect_succeed_;
	
	
	bool bIsIniting_ ;
	list<ACE_Message_Block*> mq_;
	void putQ(ACE_Message_Block *mb);
};
