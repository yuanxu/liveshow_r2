#pragma once
#include "Peer.h"
#include "ace/Thread_Mutex.h"
#include "ace/Guard_T.h"
#include "ace/Refcounted_Auto_Ptr.h"
#include <vector>
#include "types.h"

/**
*	�ڵ��б�
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
	// Qualifier: ���뵽�ڵ��б�
	// Parameter: PeerPtr p
	//************************************
	int join(PeerPtr p);

	//************************************
	// Method:    leave
	// FullName:  PeerList::leave
	// Access:    public 
	// Returns:   void
	// Qualifier:���ӽڵ��б��Ƴ�
	// Parameter: PeerPtr p
	//************************************
	void leave(PeerPtr p);

	//************************************
	// Method:    destory
	// FullName:  PeerList::destory
	// Access:    public 
	// Returns:   void
	// Qualifier: �����б����ͷ����нڵ�
	//************************************
	void destory();


	//************************************
	// Method:    next
	// FullName:  PeerList::next
	// Access:    public 
	// Returns:   PeerPtr
	// Qualifier: ��ȡ��һ�����õĽڵ㣬���û����һ�¿��ýڵ㣬�򷵻�NULL����ִ�д˲����ڼ��б�����
	//************************************
	PeerPtr next();

	//************************************
	// Method:    resetCursor
	// FullName:  PeerList::resetCursor
	// Access:    public 
	// Returns:   void
	// Qualifier: ��λ��ǰ�ڵ�ָ�뵽���λ�ã��������б�
	//************************************
	void openCursor();

	//************************************
	// Method:    closeCursor
	// FullName:  PeerList::closeCursor
	// Access:    public 
	// Returns:   void
	// Qualifier: �رչ�ꡣ�ͷ��б�����
	//************************************
	void closeCursor();

	//************************************
	// Method:    resetCursor
	// FullName:  PeerList::resetCursor
	// Access:    public 
	// Returns:   void
	// Qualifier: ��λ��ꡣ����������
	//************************************
	void resetCursor();
	
	//************************************
	// Method:    getPeer
	// FullName:  PeerList::getPeer
	// Access:    public 
	// Returns:   PeerPtr
	// Qualifier: ��ȡָ��Id�Ľڵ㡣���δ�ҵ��򷵻�NULL
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
	// Qualifier:�Ͽ����һ���ڵ�
	//************************************
	void pop_back();


	//************************************
	// Method:    pop_begin
	// FullName:  PeerList::pop_begin
	// Access:    public 
	// Returns:   void
	// Qualifier:�Ͽ���һ���ڵ�
	//************************************
	void pop_begin();
private:
	ACE_Thread_Mutex* mutex_;
	vector<PeerPtr> vec_;
	size_t currCursor_;
};