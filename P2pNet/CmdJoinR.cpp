#include "StdAfx.h"
#include "CmdJoinR.h"
#include "StreamWriter.h"
#include "StreamReader.h"
#include "PeerMgr.h"

CmdJoinR::CmdJoinR(CmdHeader& hdr):CmdRespBase(hdr),result_(0),version_(0),ServerId_(0),is_server_(0)
{
	cmd_hdr_.cmd_ = cmd_join_r;
}


CmdJoinR::CmdJoinR( CmdHeader& hdr,char result,ACE_UINT16 ver ):CmdRespBase(hdr),result_(result),version_(ver),ServerId_(0),is_server_(0)
{

}

CmdJoinR::CmdJoinR():result_(0),version_(0),ServerId_(0)
{
		cmd_hdr_.cmd_ = cmd_join_r;
}
CmdJoinR::~CmdJoinR(void)
{
}

ACE_Message_Block* CmdJoinR::serialize()
{
	
	StreamWriter wr;
	wr.write((char*)&cmd_hdr_,SIZE_OF_CMD_HEADER);
	wr.write_byte(result_);
	wr.write_uint16(version_);
	wr.write_uint32(ServerId_);
	wr.write_uint32(start_id_);
	wr.write_byte(is_server_);
	return wr.get_message_block();
}

void CmdJoinR::deserialize( ACE_Message_Block* lmb_ )
{
	StreamReader reader(lmb_);
	//读出头

	ACE_OS::memcpy(&cmd_hdr_,reader.read(SIZE_OF_CMD_HEADER),SIZE_OF_CMD_HEADER);

	result_ = reader.read_byte();
	version_ = reader.read_uint16();
	ServerId_ = reader.read_uint32();

	start_id_ = reader.read_uint32();
	is_server_ = reader.read_byte();
}

int CmdJoinR::process( PeerMgr &mgr,Peer &peer )
{
//TODO:向上层应用程序通知机制
	int iret = LS_SUCCEEDED;
	if (peer.peerId_ == 1)
	{
		peer.peerId_ = ServerId_;
	}
	switch(result_)
	{
	case 0: //success
	case 2://服务器连接过多　。服务器会发送ask for exit指令
		{
			if (mgr.buffer_.start_seg_id_ == 0)
			{
				mgr.buffer_.sync_Id(start_id_ + 3 );
				
			}
			else if (mgr.is_server_)
			{
				mgr.buffer_.init(start_id_);

			}
			if (mgr.peer_list_.join(&peer) == LS_FAILED)//failed
			{
				iret = LS_FAILED;
				//delete &peer;
			}
		}
		break;
	case 1://版本过低
		//peer.disConnect();
		{
			//delete &peer;
			iret = LS_FAILED;
		}
		break;
	case 3://chId不匹配
		//peer.disConnect();
		{
			//delete &peer;
			iret = LS_FAILED;
		}
		break;
	}
	return iret;
}