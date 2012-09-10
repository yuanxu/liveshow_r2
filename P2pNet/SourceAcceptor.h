#pragma once
#include "ace/Asynch_Acceptor.h"
#include "ace/INET_Addr.h"
#include "SourcePin.h"

/************************************************************************/
/* ý��Դ������                                                                     */
/************************************************************************/
class SourceAcceptor :
	public ACE_Asynch_Acceptor<SourcePin>
{
public:
	SourceAcceptor(PeerMgr &mgr);
	~SourceAcceptor(void);


	//************************************
	// Method:    start
	// FullName:  PeerAcceptor::start
	// Access:    public 
	// Returns:   int
	// Qualifier: ��������
	//************************************
	int start(ACE_INET_Addr& addr_);

	//************************************
	// Method:    stop
	// FullName:  PeerAcceptor::stop
	// Access:    public 
	// Returns:   void
	// Qualifier:��ֹͣ����
	//************************************
	void stop();

	//************************************
	// Method:    make_handler
	// FullName:  PeerAcceptor::make_handler
	// Access:    public 
	// Returns:   Peer*
	// Qualifier: ���������Ŀ������NULL�ܾ�����
	// Parameter: void
	//************************************
	virtual SourcePin* make_handler(void);

	virtual int validate_connection(const ACE_Asynch_Accept::Result& result, const ACE_INET_Addr &remote, const ACE_INET_Addr& local);


private:

	PeerMgr &mgr_;
};
