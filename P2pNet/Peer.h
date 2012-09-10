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
*	节点对象
*
*/
class PeerConnector;

class Peer : public ACE_Service_Handler
{
public:
	Peer(bool IsMySelf=false,PeerMgr* mgr=NULL);
	~Peer(void);

	/*
	*	打开节点
	*/
	void connect();

	/*
	*	关闭节点
	*/
	void disConnect();


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

	//************************************
	// Method:    putQ
	// FullName:  Peer::putQ
	// Access:    public 
	// Returns:   void
	// Qualifier: 消息入栈
	// Parameter: OutMessage& out_msg
	//************************************
	void putQ(ACE_Message_Block* out_msg);

	void set_peerMgr(PeerMgr* p);

	//************************************
	// Method:    addresses
	// FullName:  Peer::addresses
	// Access:    public 
	// Returns:   void
	// Qualifier: 在此方法可以得到远端的IP地址
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
		if (flux_.StartTime.sec() +3> ACE_OS::gettimeofday().sec()) //刚刚启动3秒内不进行比对。
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
	

	//监听的端口
	ACE_UINT16 udp_port_,tcp_port_;
protected:
	

	/**
	*	读完成时调用
	*/
	virtual void handle_read_stream(const ACE_Asynch_Read_Stream::Result &result);

	/*
	*　写完成时调用
	*/
	virtual void handle_write_stream(const ACE_Asynch_Write_Stream::Result &result);


public:
	/**
	*	是否已经连接到远程端点
	*/
	bool isConnected;

	/**
	*	端点Id
	*/
	PeerID peerId_;

	/*
	*	buffer map
	*/
	//BufferMap bm_;

	/**
	*	远程端点地址
	*/
	ACE_INET_Addr addr_;

	/*
	*	流量统计
	*/
	FluxStatistic flux_;


	/*
	*	是否是自身节点
	*/
	bool is_self_;

	/*
	* 是否是到服务器的连接
	*/
	bool is_server_;
	
	ACE_UINT32 get_next_sequence();

	//本端的BufferMap
	BufferMap bm_;


	PeerConnector *lpConnector_;

	/*
	* 是否已经加入到peer_list_中
	*/
	bool is_joined_ ;
private:
	/**
	*/
	//Buffer& buffer_;
	
	/*
	* 消息处理函数.0失败。-1成功
	*/
	int process( ACE_Message_Block *mb );


	/*
	* 已经确认的本机具有的Segment列表
	*/
	list<SEGMENT_ID> applied_segs_;


	ACE_Asynch_Read_Stream reader_;
	ACE_Asynch_Write_Stream writer_;
	//待发送数据的列队
	vector<ACE_Message_Block*> mq_;
	ACE_Thread_Mutex mq_mutex_;
	PeerMgr* lpMgr_;
	
	/*
	* 主消息缓冲。默认大小128KB,即:最大媒体码率为128*8/1024=1Mbps.
	* 128KB由PEER_BUF_SIZE确定
	*/
	ACE_Message_Block* lpMb_;


	ACE_UINT32 seqId_;
	int init_read();
	int init_write();

	//已发送的数据
	SEGMENT_ID sent_seg_id_;
};
