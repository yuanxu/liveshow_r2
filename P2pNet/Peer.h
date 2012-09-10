#pragma once

#include "ace/INET_Addr.h"
#include "ace/Asynch_IO.h"
#include "ace/Message_Block.h"
#include "ace/Thread_Mutex.h"
#include "ace/Refcounted_Auto_Ptr.h"
#include "FluxStatistic.h"
#include "BufferMap.h"
#include "types.h"
#include <vector>
#include <list>
#define PEER_BUF_SIZE (128*1024)

using namespace std;

class PeerMgr;

/**
*	�ڵ����
*
*/
class PeerConnector;

class Peer : public ACE_Service_Handler
{
public:
	Peer(bool IsMySelf=false,PeerMgr* mgr=NULL);
	~Peer(void);

	/*
	*	�򿪽ڵ�
	*/
	void connect();

	/*
	*	�رսڵ�
	*/
	void disConnect();


	//************************************
	// Method:    open
	// FullName:  Peer::open
	// Access:    virtual protected 
	// Returns:   void
	// Qualifier: �����ӽ��������
	// Parameter: ACE_HANDLE h
	// Parameter: ACE_Message_Block&
	//************************************
	virtual void open(ACE_HANDLE h,ACE_Message_Block&);

	//************************************
	// Method:    putQ
	// FullName:  Peer::putQ
	// Access:    public 
	// Returns:   void
	// Qualifier: ��Ϣ��ջ
	// Parameter: OutMessage& out_msg
	//************************************
	void putQ(ACE_Message_Block* out_msg);

	void set_peerMgr(PeerMgr* p);

	//************************************
	// Method:    addresses
	// FullName:  Peer::addresses
	// Access:    public 
	// Returns:   void
	// Qualifier: �ڴ˷������Եõ�Զ�˵�IP��ַ
	// Parameter: const ACE_INET_Addr &remote_address
	// Parameter: const ACE_INET_Addr &local_address
	//************************************
	virtual void addresses(const ACE_INET_Addr &remote_address, const ACE_INET_Addr &local_address);

	bool operator = (const Peer &p)
	{
		return p.peerId_ == this->peerId_;
	}
	bool operator < (const Peer &p)
	{
		if (flux_.StartTime.sec() +3> ACE_OS::gettimeofday().sec()) //�ո�����3���ڲ����бȶԡ�
		{
			return false;
		}
		return   this->flux_.CurrentReadKBps < p.flux_.CurrentReadKBps;
	}

	// islow id
	bool is_behind_nat_; 
	
	void push_applied_seg_id(SEGMENT_ID id);
	void pop_applied_seg_id(SEGMENT_ID id);
	bool has_applied(SEGMENT_ID id);
	

	//�����Ķ˿�
	ACE_UINT16 udp_port_,tcp_port_;
protected:
	

	/**
	*	�����ʱ����
	*/
	virtual void handle_read_stream(const ACE_Asynch_Read_Stream::Result &result);

	/*
	*��д���ʱ����
	*/
	virtual void handle_write_stream(const ACE_Asynch_Write_Stream::Result &result);


public:
	/**
	*	�Ƿ��Ѿ����ӵ�Զ�̶˵�
	*/
	bool isConnected;

	/**
	*	�˵�Id
	*/
	PeerID peerId_;

	/*
	*	buffer map
	*/
	//BufferMap bm_;

	/**
	*	Զ�̶˵��ַ
	*/
	ACE_INET_Addr addr_;

	/*
	*	����ͳ��
	*/
	FluxStatistic flux_;


	/*
	*	�Ƿ�������ڵ�
	*/
	bool is_self_;

	/*
	* �Ƿ��ǵ�������������
	*/
	bool is_server_;
	
	ACE_UINT32 get_next_sequence();

	//���˵�BufferMap
	BufferMap bm_;


	PeerConnector *lpConnector_;

	/*
	* �Ƿ��Ѿ����뵽peer_list_��
	*/
	bool is_joined_ ;
private:
	/**
	*/
	//Buffer& buffer_;
	
	/*
	* ��Ϣ������.0ʧ�ܡ�-1�ɹ�
	*/
	int process( ACE_Message_Block *mb );


	/*
	* �Ѿ�ȷ�ϵı������е�Segment�б�
	*/
	list<SEGMENT_ID> applied_segs_;


	ACE_Asynch_Read_Stream reader_;
	ACE_Asynch_Write_Stream writer_;
	//���������ݵ��ж�
	vector<ACE_Message_Block*> mq_;
	ACE_Thread_Mutex mq_mutex_;
	PeerMgr* lpMgr_;
	
	/*
	* ����Ϣ���塣Ĭ�ϴ�С128KB,��:���ý������Ϊ128*8/1024=1Mbps.
	* 128KB��PEER_BUF_SIZEȷ��
	*/
	ACE_Message_Block* lpMb_;


	ACE_UINT32 seqId_;
	int init_read();
	int init_write();

	//�ѷ��͵�����
	SEGMENT_ID sent_seg_id_;
};
