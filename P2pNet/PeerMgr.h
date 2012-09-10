#pragma once
#include "ace/INET_Addr.h"
#include "PeerList.h"
#include "PeerAcceptor.h"
#include "PeerConnector.h"
#include "MediaHeader.h"
#include "SourceAcceptor.h"
#include "SourcePin.h"
#include "Buffer.h"
#include <string>
#include "PrimaryTask.h"
#include "ExBm.h"
#include "Scheduler.h"
#include "Config.h"
#include "UdpListenerTask.h"
#include "SyncIdTimer.h"
#include "PeerInfoList.h"
#include "mCacheTimer.h"
#include <map>
#include "MonitorTimer.h"


using namespace std;
class HttpdAcceptor;
class PeerInfoList;
class ProxyHelloTimer;

class PeerMgr
{
public:
	PeerMgr(void);
	~PeerMgr(void);

	//************************************
	// Method:    svr_start
	// FullName:  PeerMgr::svr_start
	// Access:    public 
	// Returns:   void
	// Qualifier: 以服务器启动
	// Parameter: const ACE_UINT16 listen_port
	//************************************
	void svr_start(const ACE_UINT32 ChID,const PeerID peerId,const ACE_UINT16 listen_port,const ACE_UINT16 source_port);


	//************************************
	// Method:    set_proxy_server
	// FullName:  PeerMgr::set_proxy_server
	// Access:    public 
	// Returns:   void
	// Qualifier: 设置代理服务器地址
	// Parameter: std::string
	//************************************
	void set_proxy_server(std::string);

	//************************************
	// Method:    clt_start
	// FullName:  PeerMgr::clt_start
	// Access:    public 
	// Returns:   void
	// Qualifier: 以客户端启动
	// Parameter: const std::string serverIp
	// Parameter: const ACE_UINT16 server_port
	//************************************
	void clt_start(const ACE_UINT32 chId,const std::string serverIp);
	
	//************************************
	// Method:    stop
	// FullName:  PeerMgr::stop
	// Access:    public 
	// Returns:   void
	// Qualifier:　停止
	//************************************
	void stop();


	//************************************
	// Method:    connect_to_server
	// FullName:  PeerMgr::connect_to_server
	// Access:    public 
	// Returns:   void
	// Qualifier: 连接到服务器。作为客户机运行时的参数。
	//************************************
	void connect_to_server();

	//************************************
	//* 版本                              
	//************************************
	ACE_UINT16 version_;

	//已连接节点列表
	PeerList peer_list_;
	//未连接的侯选节点列表
	PeerInfoList peer_info_list_;
	//媒体文件头
	MediaHeader mhdr_;

	//本节点的ID
	PeerID peerId_;
	//当前频道ID
	ACE_UINT32 chId_;
	//节目源连接。只有在服务器模式可用
	SourcePin *lpSourcePin_;
	//主缓冲区
	Buffer buffer_;
	//配置文件
	Config config_;

	ACE_Proactor *lpProactor_;

	//节目源服务器地址。推送端口与Sync端口必须一致。当运行于客户端方式时有效
	ACE_INET_Addr server_addr_, proxy_server_addr_;

	//本地监听地址
	ACE_INET_Addr local_addr_;
	//本地的外网端口
	ACE_INET_Addr remote_addr_;
	//是否运行于服务器模式
	bool is_server_;

	//节点连接器
	PeerConnector *lpConnector_;
	bool is_behind_nat_;

	//Udp监听器线程
	UdpListenerTask Udp_task_;

	ACE_UINT16 tcp_port_,udp_port_;

private:
	/*
	* 适用于客户端与服务器端的通用初始化代码
	*/
	void common_init(const ACE_UINT16 listen_port,PeerID pid);

	int clt_connect(ACE_UINT32 ChId);
	
	//主节点连接tcp端口监听器
	PeerAcceptor peer_acceptor_;
	

	//媒体源监听器
	SourceAcceptor *lp_ms_acceptor_;
	

	//主线程
	PrimaryTask primary_task_;

	ExBm *lpExBm_;
	SyncIdTimer *lpSyncIdTimer_;
	mCacheTimer *lpmCache_;

	MonitorTimer *lpMonitorTimer_;

	//调度器
	Scheduler *lpScheduler_;
	
	HttpdAcceptor *lpHttpdAcceptor_;
	
	ProxyHelloTimer *lpHelloTimer_;
};
