#include "StdAfx.h"
#include "Config.h"
#include "types.h"

Config::Config(void):scheduler_timer_interval_(),ex_bm_timer_interval_(),sync_id_timer_interval_(),max_mCache_size_(0),mCache_timer_interval_(0)
	,max_peer_list_size_(0),max_disconnect_count_(0),peer_info_to_live_(25)
{
	scheduler_timer_interval_.msec(300);//s
	ex_bm_timer_interval_.sec(1);//1s
	sync_id_timer_interval_.sec(21);//21s
	mCache_timer_interval_.sec(2);//2s
	max_peer_queue_size_ = MAX_SEGMENT_COUNT * 1.2; 

	max_mCache_size_ = 128;
	max_peer_list_size_ = 3+1; //plus 1 because self is in the peer_list too.
	client_max_peer_list_size_ = max_peer_list_size_*3; //客户端的最大peer链接数量，是服务器的3倍
	max_disconnect_count_= max_peer_list_size_ / 4 ; //25%
	max_connect_count_ = 2;
}

Config::~Config(void)
{
}
