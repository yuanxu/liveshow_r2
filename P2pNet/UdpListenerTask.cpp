#include "StdAfx.h"
#include "UdpListenerTask.h"
#include "PeerMgr.h"
#include "ace/INET_Addr.h"
#include "ace/SOCK_Dgram.h"
#include "ace/Basic_Types.h"

#include "UdpCmdSyncId.h"
#include "UdpCmdSyncIdR.h"
#include "UdpCmdHelloR.h"

UdpListenerTask::UdpListenerTask(PeerMgr &p):mgr_(p),isRunning_(false),udp(p.local_addr_)
{
	
}

UdpListenerTask::~UdpListenerTask(void)
{
}

int UdpListenerTask::svc()
{
	isRunning_ = true;
	int ret= udp.open(mgr_.local_addr_,2,0,1);
	
	ret =ACE_OS::last_error();
	char buffer[128];
	ACE_Time_Value tv;
	tv.msec(300);

	ACE_INET_Addr remote_addr_;
	try
	{

	while (isRunning_)
	{
		ssize_t ret = udp.recv(buffer,sizeof(buffer),remote_addr_,0,&tv);
		if (ret > 0) //接收成功
		{
			CmdHeader *lpHdr_ =reinterpret_cast<CmdHeader*>(buffer);
			UdpCmdBase *lpCmd = NULL;
			switch(lpHdr_->cmd_)
			{
				case udp_sync_id:
					lpCmd = new  UdpCmdSyncId();
					break;
				case udp_sync_id_r: 
					lpCmd = new UdpCmdSyncIdR();
					break;
				case udp_hello_r://proxy server的反馈
					lpCmd = new UdpCmdHelloR();
					break;
				case udp_callback_r://call back 反馈
					break;
			}
			if (lpCmd != NULL)
			{
				lpCmd->deserialize(buffer,ret);
				lpCmd->process(mgr_,*this,remote_addr_);
				delete lpCmd; //这里必须销毁数据
			}
		}
		else
		{
			int err_no = ACE_OS::last_error();
			if (err_no == ETIME)
			{
				continue;
			}
			else
			{
				ACE_ERROR((LM_ERROR,ACE_TEXT("%p\n"),
					ACE_TEXT("Udp Listener svc")));;
			}
		}
	}
	}
	catch (...)
	{
		
		//ACE_ERROR ((LM_ERROR, "(EE)"));
		ACE_ERROR ((LM_ERROR, "%p\n","UdpListenerTask::svc.Erro Code%d \n ",ACE_OS::last_error()));
		
	}
	
		try
		{
			udp.close();
			isRunning_ = false;
		}
		catch(...){}
		
		ACE_DEBUG((LM_DEBUG,"UdpListenerTask::svc exit\n"));
	return 0;
}

int UdpListenerTask::close( u_long flags /* = 0 */ )
{
	isRunning_ = false;
	return 0;
}

void UdpListenerTask::send( char* buf_,size_t len_ ,ACE_INET_Addr &remote_addr_)
{
	if (!isRunning_)
	{
		return;
	}
	ACE_Time_Value tv;
	tv.msec(300);
	ssize_t iret =udp.send(buf_,len_,remote_addr_,0,&tv);
	if (iret <=0)
	{
		iret = ACE_OS::last_error();
	}
}

void UdpListenerTask::send( ACE_Message_Block &msg,ACE_INET_Addr &remote_addr_ )
{
	send(msg.rd_ptr(),msg.length(),remote_addr_);
}

void UdpListenerTask::send_to_proxy_server( ACE_Message_Block &msg )
{
	send(msg,mgr_.proxy_server_addr_);
}