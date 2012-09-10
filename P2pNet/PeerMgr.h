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
	// Qualifier: �Է���������
	// Parameter: const ACE_UINT16 listen_port
	//************************************
	void svr_start(const ACE_UINT32 ChID,const PeerID peerId,const ACE_UINT16 listen_port,const ACE_UINT16 source_port);


	//************************************
	// Method:    set_proxy_server
	// FullName:  PeerMgr::set_proxy_server
	// Access:    public 
	// Returns:   void
	// Qualifier: ���ô����������ַ
	// Parameter: std::string
	//************************************
	void set_proxy_server(std::string);

	//************************************
	// Method:    clt_start
	// FullName:  PeerMgr::clt_start
	// Access:    public 
	// Returns:   void
	// Qualifier: �Կͻ�������
	// Parameter: const std::string serverIp
	// Parameter: const ACE_UINT16 server_port
	//************************************
	void clt_start(const ACE_UINT32 chId,const std::string serverIp);
	
	//************************************
	// Method:    stop
	// FullName:  PeerMgr::stop
	// Access:    public 
	// Returns:   void
	// Qualifier:��ֹͣ
	//************************************
	void stop();


	//************************************
	// Method:    connect_to_server
	// FullName:  PeerMgr::connect_to_server
	// Access:    public 
	// Returns:   void
	// Qualifier: ���ӵ�����������Ϊ�ͻ�������ʱ�Ĳ�����
	//************************************
	void connect_to_server();

	//************************************
	//* �汾                              
	//************************************
	ACE_UINT16 version_;

	//�����ӽڵ��б�
	PeerList peer_list_;
	//δ���ӵĺ�ѡ�ڵ��б�
	PeerInfoList peer_info_list_;
	//ý���ļ�ͷ
	MediaHeader mhdr_;

	//���ڵ��ID
	PeerID peerId_;
	//��ǰƵ��ID
	ACE_UINT32 chId_;
	//��ĿԴ���ӡ�ֻ���ڷ�����ģʽ����
	SourcePin *lpSourcePin_;
	//��������
	Buffer buffer_;
	//�����ļ�
	Config config_;

	ACE_Proactor *lpProactor_;

	//��ĿԴ��������ַ�����Ͷ˿���Sync�˿ڱ���һ�¡��������ڿͻ��˷�ʽʱ��Ч
	ACE_INET_Addr server_addr_, proxy_server_addr_;

	//���ؼ�����ַ
	ACE_INET_Addr local_addr_;
	//���ص������˿�
	ACE_INET_Addr remote_addr_;
	//�Ƿ������ڷ�����ģʽ
	bool is_server_;

	//�ڵ�������
	PeerConnector *lpConnector_;
	bool is_behind_nat_;

	//Udp�������߳�
	UdpListenerTask Udp_task_;

	ACE_UINT16 tcp_port_,udp_port_;

private:
	/*
	* �����ڿͻ�����������˵�ͨ�ó�ʼ������
	*/
	void common_init(const ACE_UINT16 listen_port,PeerID pid);

	int clt_connect(ACE_UINT32 ChId);
	
	//���ڵ�����tcp�˿ڼ�����
	PeerAcceptor peer_acceptor_;
	

	//ý��Դ������
	SourceAcceptor *lp_ms_acceptor_;
	

	//���߳�
	PrimaryTask primary_task_;

	ExBm *lpExBm_;
	SyncIdTimer *lpSyncIdTimer_;
	mCacheTimer *lpmCache_;

	MonitorTimer *lpMonitorTimer_;

	//������
	Scheduler *lpScheduler_;
	
	HttpdAcceptor *lpHttpdAcceptor_;
	
	ProxyHelloTimer *lpHelloTimer_;
};
