#include "StdAfx.h"
#include "PeerMgr.h"
#include "ace/Proactor.h"
#include "CmdJoin.h"
#include "HttpdAcceptor.h"
#include "HttpdPeerList.h"
#include "FileDeliver.h"
#include "ProxyHelloTimer.h"

PeerMgr::PeerMgr(void)
	:local_addr_(),peer_list_(),peer_acceptor_(*this),version_(10),chId_(0),peerId_(0),lpSourcePin_(NULL),buffer_(),lp_ms_acceptor_(NULL)
	,primary_task_(this),Udp_task_(*this),lpExBm_(NULL),config_(),is_server_(false),lpScheduler_(NULL),lpProactor_(NULL),lpConnector_(NULL),server_addr_()
	,lpSyncIdTimer_(NULL),peer_info_list_(),lpmCache_(NULL),lpMonitorTimer_(NULL),proxy_server_addr_(),is_behind_nat_(false),lpHelloTimer_(NULL),tcp_port_(18086),udp_port_(18086)
{
	lpProactor_ = ACE_Proactor::instance();
	
	
}

PeerMgr::~PeerMgr(void)
{
	lpProactor_ = NULL;
}

void PeerMgr::common_init(const ACE_UINT16 listen_port,const PeerID pid)
{

	local_addr_.set_port_number(listen_port);
	peer_acceptor_.start(local_addr_);

	buffer_.init(0);
	
	//把自身节点加入
	Peer *p = new Peer(true,this);
	p->peerId_ = pid;
	p->addr_.set_port_number(listen_port);
	//TODO:set address
	p->peerId_ = peerId_;
	peer_list_.join(p);


	//启动Udp监听
	Udp_task_.activate();

	//启动ExBm、Ping
	ACE_Time_Value tv;
	tv.msec(200);
	lpExBm_ = new ExBm(this);
	
	lpExBm_->timer_id_ =lpProactor_->schedule_timer(*lpExBm_,NULL,tv,config_.ex_bm_timer_interval_);


	lpmCache_ = new mCacheTimer(*this);
	tv.sec(5);
	lpmCache_->timer_id_ = lpProactor_->schedule_timer(*lpmCache_,NULL,tv,config_.mCache_timer_interval_);
}

void PeerMgr::svr_start(const ACE_UINT32 ChID,const PeerID peerId, const ACE_UINT16 listen_port ,const ACE_UINT16 source_port)
{

	lp_ms_acceptor_ = new SourceAcceptor(*this);

	is_server_ =true;
	peerId_ = peerId;
	chId_ = ChID;

	//启动媒体源的监听
	ACE_INET_Addr ms_addr_;
	ms_addr_.set_port_number(source_port);
	lp_ms_acceptor_->start(ms_addr_);
	
	common_init(listen_port,peerId);

	primary_task_.activate();//启动工作线程
	
	lpHttpdAcceptor_ = NULL;
}

void PeerMgr::clt_start(const ACE_UINT32 chId, const std::string serverIp )
{
	is_server_ =false;
	chId_ = chId;
	server_addr_.string_to_addr(serverIp.c_str());


	config_.max_peer_list_size_ = config_.client_max_peer_list_size_;//客户端运行时，比服务器端多2倍的连接容量.重置此变量的用法

	if(	clt_connect(chId) <0)
	{
		//TODO:通知上层应用。无法链接服务器。
		return ;
	}
	common_init(tcp_port_,peerId_);

	lpConnector_ = new PeerConnector(*this);
	lpConnector_->open(1,lpProactor_);

	primary_task_.activate();//启动工作线程

	//连接到服务器
	

	connect_to_server();


	//启动Schedule、SyncId
	lpScheduler_ = new Scheduler(*this);
	ACE_Time_Value tv;
	tv.msec(500);
	lpScheduler_->timer_id_ =lpProactor_->schedule_timer(*lpScheduler_,NULL,tv,config_.scheduler_timer_interval_);

	lpSyncIdTimer_ = new SyncIdTimer(*this,Udp_task_);

	lpSyncIdTimer_->timer_id_ = lpProactor_->schedule_timer(*lpSyncIdTimer_,NULL,tv,config_.sync_id_timer_interval_);

	//如果是lowId，启动向Proxy的问答线程

	if (is_behind_nat_)
	{
		lpHelloTimer_ = new ProxyHelloTimer(*this,Udp_task_);
		ACE_Time_Value tv;
		tv.sec(15);//15s
		lpHelloTimer_->timer_id_ = lpProactor_->schedule_timer(*lpHelloTimer_,NULL,tv,tv);
	}

	//启动本地web server
	lpHttpdAcceptor_ = new HttpdAcceptor(this);
	ACE_INET_Addr httpd_addr_(18087);
	lpHttpdAcceptor_->start(httpd_addr_);
	//设置本地的接收端列表
	buffer_.lp_httpd_list_ = new HttpdPeerList();

	//buffer_.lp_httpd_list_->join(new FileDeliver(this));

	tv.msec(500);
	tv.sec(10);//10.5s
	lpMonitorTimer_ = new MonitorTimer(*this);
	lpMonitorTimer_->timer_id_ = lpProactor_->schedule_timer(*lpMonitorTimer_,NULL,tv,tv);
	//到目前为止客户端启动完毕
}

void PeerMgr::stop()
{
	if (lpSourcePin_!=NULL)
	{
		//删除源连接点
		delete lpSourcePin_;
		lpSourcePin_ = NULL;
	}

	lpProactor_->cancel_timer(lpExBm_->timer_id_);
	lpProactor_->cancel_timer(lpmCache_->timer_id_);
	
	delete lpExBm_;
	delete lpmCache_;
	lpExBm_ = NULL;
	lpmCache_ = NULL;

	Udp_task_.close(0);
	primary_task_.close(0);	

	if (!is_server_)
	{
		lpProactor_->cancel_timer(lpMonitorTimer_->timer_id_);
		lpProactor_->cancel_timer(lpScheduler_->timer_id_);
		lpProactor_->cancel_timer(lpSyncIdTimer_->timer_id_);
		if(lpHelloTimer_ != NULL)
		{
			lpProactor_->cancel_timer(lpHelloTimer_->timer_id_);
			delete lpProactor_;
			lpProactor_ = NULL;
		}
		
		lpConnector_->cancel();
		lpHttpdAcceptor_->stop();
		delete lpMonitorTimer_;
		lpMonitorTimer_ = NULL;
		delete lpHttpdAcceptor_;
		lpHttpdAcceptor_ = NULL;
		delete buffer_.lp_httpd_list_;
		buffer_.lp_httpd_list_ = NULL;
		delete lpConnector_;
		lpConnector_ = NULL;
	}
	else
	{
		lp_ms_acceptor_->stop();
		delete lp_ms_acceptor_;
	}
	
	peer_info_list_.destory();
	peer_list_.destory();

	buffer_.destory();

	
}

void PeerMgr::connect_to_server()
{
	CmdJoin cmd;
	cmd.version_ = version_;
	cmd.tcp_port_ = tcp_port_;
	cmd.udp_port_ = udp_port_;
	ACE_Message_Block* mb=cmd.serialize(0,chId_,peerId_);//向服务器投递Join指令

	lpConnector_->make_connect(0,server_addr_,mb); //服务器节点默认给1。这个值在cmd_join_r中被修正

}

int PeerMgr::clt_connect( ACE_UINT32 chId )
{

	ACE_INET_Addr* addr_array; 
	size_t count = 0; 

	if (ACE::get_ip_interfaces(count, addr_array) != 0)
		return -1; 



	char address[INET6_ADDRSTRLEN];//可以装下IPv6地址（46），IPv4为INET_ADDRSTRLEN（16）
	int connect_len_ = SIZE_OF_CMD_HEADER + count*4;
	char connect_buf_[128]; //连接命令的缓冲

	ACE_OS::memset(connect_buf_,0,sizeof(connect_buf_));
	int ioffset =0;
	int connect_cmd_ =udp_connect;
	ACE_OS::memcpy(connect_buf_+ioffset,&connect_cmd_,sizeof(int)); //udp_connect;
	ioffset+=4;
	ioffset+=4;//seq
	ACE_OS::memcpy(connect_buf_+ioffset,&chId,sizeof(chId));

	ioffset = SIZE_OF_CMD_HEADER;
	ACE_INET_Addr* addr_array2 = addr_array;
	while (count--)
	{ 
		if (!addr_array2->is_loopback())
		{
		
			ACE_UINT32 ip =addr_array2[0].get_ip_address();
			ACE_OS::memcpy(connect_buf_+ioffset,&ip,sizeof(int));
			addr_array2->addr_to_string(address, sizeof (address)); 
			ACE_OS::printf("%s\n", address);
			ioffset+=4;
		}
		++addr_array2;
	}

	ACE_OS::memcpy(connect_buf_+SIZE_OF_CMD_HEADER -4,&ioffset,4);
	
	//peerId_ = addr_array[0].get_ip_address();

	local_addr_ =  addr_array[0];
	delete[] addr_array;//记得要delete[] addr_array;

	ACE_SOCK_Dgram udp ;
	udp.open(ACE_Addr::sap_any);
	int iResend = 0;
	while(iResend<3)
	{
		ssize_t sent =	udp.send(connect_buf_,connect_len_,proxy_server_addr_);
		if (sent == -1)
		{
			int err =ACE_OS::last_error();
			//ERROR
		}
		else
		{
			//重发机制
			ACE_Time_Value tv;
			tv.msec(300 * (1+iResend*0.5));
			ACE_INET_Addr addr;
			char buf[SIZE_OF_CMD_HEADER+sizeof(ACE_UINT32)*2];
			ssize_t recv_count = udp.recv(buf,sizeof(buf),addr,0,&tv);
			if (recv_count>0)
			{
				ACE_UINT32 ip;
				memcpy(&ip,buf+SIZE_OF_CMD_HEADER,sizeof(ACE_UINT32));
				remote_addr_.set(18086,ip);
				memcpy(&peerId_,buf+SIZE_OF_CMD_HEADER+sizeof(ACE_UINT32),sizeof(ACE_UINT32));
				if(IsLowID(peerId_))
				{
					is_behind_nat_ = true;
				}
				else
				{
					is_behind_nat_ = false;
				}
				break;
			}
			else
			{
				//ERROR
			}
		}
		iResend++;
	}
	udp.close();
	if (peerId_ > 0)//successed
	{
		return 0;
	}
	else
	{
		return -2;
	}
}

void PeerMgr::set_proxy_server( std::string proxySrv)
{
	proxy_server_addr_.string_to_addr(proxySrv.c_str());
}