#pragma once
#include "Peer.h"
#include "ace/Thread_Mutex.h"
#include "ace/Guard_T.h"
#include "ace/Refcounted_Auto_Ptr.h"
#include <vector>
#include "types.h"

/**
*	节点列表
*/

using namespace std;

typedef /*ACE_Refcounted_Auto_Ptr<Peer,ACE_Thread_Mutex>*/ Peer* PeerPtr;

class PeerList
{
public:
	PeerList(void);
	~PeerList(void);
	
	//************************************
	// Method:    join
	// FullName:  PeerList::join
	// Access:    public 
	// Returns:   void
	// Qualifier: 加入到节点列表
	// Parameter: PeerPtr p
	//************************************
	int join(PeerPtr p);

	//************************************
	// Method:    leave
	// FullName:  PeerList::leave
	// Access:    public 
	// Returns:   void
	// Qualifier:　从节点列表移除
	// Parameter: PeerPtr p
	//************************************
	void leave(PeerPtr p);

	//************************************
	// Method:    destory
	// FullName:  PeerList::destory
	// Access:    public 
	// Returns:   void
	// Qualifier: 销毁列表，并释放所有节点
	//************************************
	void destory();


	//************************************
	// Method:    next
	// FullName:  PeerList::next
	// Access:    public 
	// Returns:   PeerPtr
	// Qualifier: 获取下一个可用的节点，如果没有下一下可用节点，则返回NULL。在执行此操作期间列表锁定
	//************************************
	PeerPtr next();

	//************************************
	// Method:    resetCursor
	// FullName:  PeerList::resetCursor
	// Access:    public 
	// Returns:   void
	// Qualifier: 复位当前节点指针到最初位置，并锁定列表
	//************************************
	void openCursor();

	//************************************
	// Method:    closeCursor
	// FullName:  PeerList::closeCursor
	// Access:    public 
	// Returns:   void
	// Qualifier: 关闭光标。释放列表锁定
	//************************************
	void closeCursor();

	//************************************
	// Method:    resetCursor
	// FullName:  PeerList::resetCursor
	// Access:    public 
	// Returns:   void
	// Qualifier: 复位光标。不处理锁定
	//************************************
	void resetCursor();
	
	//************************************
	// Method:    getPeer
	// FullName:  PeerList::getPeer
	// Access:    public 
	// Returns:   PeerPtr
	// Qualifier: 获取指定Id的节点。如果未找到则返回NULL
	// Parameter: PeerID id
	//************************************
	PeerPtr getPeer(PeerID id);

	size_t size();

	bool exist(PeerID id);

	void sort();
	static bool less_second(const Peer & m1, const Peer & m2) {
		return m1.flux_.CurrentReadKBps < m2.flux_.CurrentReadKBps;
	}

	//************************************
	// Method:    pop_back
	// FullName:  PeerList::pop_back
	// Access:    public 
	// Returns:   void
	// Qualifier:断开最后一个节点
	//************************************
	void pop_back();


	//************************************
	// Method:    pop_begin
	// FullName:  PeerList::pop_begin
	// Access:    public 
	// Returns:   void
	// Qualifier:断开第一个节点
	//************************************
	void pop_begin();
private:
	ACE_Thread_Mutex* mutex_;
	vector<PeerPtr> vec_;
	size_t currCursor_;
};