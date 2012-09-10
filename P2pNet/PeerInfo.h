#pragma once
#include "types.h"
#include "ace/Message_Block.h"
#include "ace/Time_Value.h"
#include "ace/OS.h"

/*
节点信息
*/
class PeerInfo
{
public:
	PeerInfo():peerId_(0),address_(0),port_number_(0),mb_(NULL),ttl_(),is_connecting_(false),is_behind_nat_(false)
	{
		ttl_ = ACE_OS::gettimeofday();
		ttl_.sec(ttl_.sec()+10);//
	};
	~PeerInfo()
	{

	};
	PeerID peerId_;
	ACE_UINT32 address_;
	ACE_UINT16 port_number_;
	bool is_behind_nat_;
	//peer_connector中有用到
	ACE_Message_Block *mb_; 
	ACE_Time_Value ttl_;
	bool operator =(const PeerInfo& p)
	{
		return this->peerId_ == p.peerId_;
	};
	bool operator <(const PeerInfo& p)
	{
		return this->peerId_ < p.peerId_;
	}
	bool is_connecting_;
};
typedef PeerInfo* PeerInfoPtr ;
