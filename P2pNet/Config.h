#pragma once
#include "ace/Time_Value.h"
class Config
{
public:
	Config(void);
	~Config(void);

	ACE_Time_Value scheduler_timer_interval_;
	ACE_Time_Value ex_bm_timer_interval_;
	ACE_Time_Value sync_id_timer_interval_;
	ACE_Time_Value mCache_timer_interval_;
	/**
	* 
	*/
	int max_peer_queue_size_;

	/***
	* max_peer_list_size_ .服务器的最大链接数量
	*/
	int max_peer_list_size_;

	int client_max_peer_list_size_;
	/**
	* pinfo_list_.size最大值
	*/
	int max_mCache_size_;

	
	int max_disconnect_count_;
	int max_connect_count_;
	int peer_info_to_live_;
};
