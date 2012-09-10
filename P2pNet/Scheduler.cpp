#include "StdAfx.h"
#include "Scheduler.h"
#include "PeerMgr.h"
#include "Peer.h"
#include "CmdApplyData.h"


class Supplier
{
public:
	Supplier():seg_id_(0),peers_()
	{}
	~Supplier()
	{};
	SEGMENT_ID seg_id_;
	vector<Peer*> peers_;
	bool operator =(const Supplier &p)
	{
		return p.seg_id_ == this->seg_id_;
	}
} ;
typedef Supplier* SupplierPtr;

class PeerData
{
public:
	PeerData():seg_ids_(),peer_(NULL)
	{

	}
	~PeerData()
	{

	}
	Peer* peer_;
	vector<SEGMENT_ID> seg_ids_;
	//��Ҫ����=?
	bool operator=(const PeerData &p) const
	{
		return p.peer_->peerId_ == peer_->peerId_;
	}
} ;
typedef PeerData* PeerDataPtr;


Scheduler::Scheduler(PeerMgr &mgr):mgr_(mgr),timer_id_(0),ACE_Handler()
{
}

Scheduler::~Scheduler(void)
{
}

/*

	�㷨˵����
	���裺
		1��ȷ����Ҫ���������
		2��ȷ��ÿ��Segment���ܵ��ṩ��
		3����ÿ��Segment���ṩ�߽������ȼ����򣻲�ȷ�����յ��ṩ��
		4��������ȷ�����ṩ����������

	TODO:
		��δ����Ƿǳ��ؼ��ģ�Ŀǰ��߻��кܶ����������ݸ��ƹ��̣�Ҫ��취�Ż�
*/
void Scheduler::handle_time_out( const ACE_Time_Value &current_time, const void *act /* = 0 */ )
{
	//ACE_DEBUG ((LM_DEBUG, "%T Scheduler called.\n"));
	vector<SupplierPtr> expected_ids_;
	//����BufferMap
	SEGMENT_ID start_seg_id_= mgr_.buffer_.start_seg_id_;
	mgr_.buffer_.mutex_.acquire();
	//Step 1��ȷ����Ҫ���������
	for (int i = 0 ; i < MAX_SEGMENT_COUNT ; i ++)
	{
		if (!mgr_.buffer_.bm_.get_bit(start_seg_id_ + i)) //û����������Ҫ�����
		{
			SupplierPtr p = new Supplier();
			p->seg_id_ = start_seg_id_ + i;
			expected_ids_.push_back(p);
		}
	}

	mgr_.buffer_.mutex_.release();


	//�����µĳ����б�������mgr_.peer_list_������Ұָ��
	
	//Step 2��ȷ��ÿ�����ݵ��ṩ��

	mgr_.peer_list_.openCursor();
	PeerPtr peer = NULL;
	while (true)
	{
		peer = mgr_.peer_list_.next();
		if (peer == NULL)
		{
			break;
		}
		for (vector<SupplierPtr>::iterator it = expected_ids_.begin() ; it != expected_ids_.end(); it++)
		{
			 //����Ҫ������ /*&& û����˽ڵ������������*/
			if (!peer->is_self_ && peer->bm_.get_bit((*it)->seg_id_) && !peer->has_applied((*it)->seg_id_))
			{
				(*it)->peers_.push_back(peer);
			}
		}
	}
	
	//Step 3��ȷ�����յ��ṩ��

	//mgr_.peer_list_.resetCursor(); //��λ���

	//�����ݽṹ�д����ÿ��Peer���������.
	vector<PeerDataPtr> vtPeerData;

	for (vector<SupplierPtr>::iterator it = expected_ids_.begin() ; it != expected_ids_.end(); it++)
	{
		if(!(*it)->peers_.empty()) //���ṩ��
		{
			//���ṩ������
			sort((*it)->peers_.begin(),(*it)->peers_.end());
			//������������
			int i = 0;
			//it2ָ��PeerPtr
			for (vector<PeerPtr>::iterator it2 = (*it)->peers_.begin() ; it2 != (*it)->peers_.end() && i<MAX_SUPPLIER ; it2++)
			{
				PeerDataPtr pdp = NULL;
				PeerDataPtr pdp_t = new PeerData();
				pdp_t->peer_=*it2;
				//�����ڵȷ���ָ����������Ƿ��д˽ڵ�
				
				vector<PeerDataPtr>::iterator it_find;
				for(it_find = vtPeerData.begin(); it_find != vtPeerData.end(); it_find++)
				{
					PeerPtr peer = (*it_find)->peer_;
					if (peer->peerId_ == pdp_t->peer_->peerId_ )
					{
						break;
					}
				}//= find(vtPeerData.begin(),vtPeerData.end(),pdp_t);
				delete pdp_t; 
				if (it_find == vtPeerData.end()) //û��
				{
					pdp = new PeerData();
					pdp->peer_ = *it2;
					vtPeerData.push_back(pdp);
				}
				else
				{
					pdp = *it_find;
				}
				pdp->seg_ids_.push_back((*it)->seg_id_);//����������б�
				i++;
			}
		}
	}
	
	//Step 4����������

	for (vector<PeerDataPtr>::iterator it = vtPeerData.begin() ; it != vtPeerData.end() ; it++)
	{
		CmdApplyData cmd;
		cmd.seg_ids_ = (*it)->seg_ids_;
		PeerPtr p =(*it)->peer_;
		for (vector<SEGMENT_ID>::iterator it2 = cmd.seg_ids_.begin() ; it2 != cmd.seg_ids_.end() ; it2++)
		{
			p->push_applied_seg_id(*it2);
		}
		p->putQ(cmd.serialize(p->get_next_sequence(),mgr_.chId_,mgr_.peerId_));
	}
	
	mgr_.peer_list_.closeCursor(); //�رչ��

	//��������
	while(!expected_ids_.empty())
	{
		delete *expected_ids_.begin();
		expected_ids_.erase(expected_ids_.begin());
	}
	while (!vtPeerData.empty())
	{
		delete *vtPeerData.begin();
		vtPeerData.erase(vtPeerData.begin());
	}
}

