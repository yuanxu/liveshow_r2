#pragma once
#include <bitset>
#include "types.h"
#include "ace/Thread_Mutex.h"

using namespace std;
class BufferMap
{
public:
	BufferMap(void);
	~BufferMap(void);

	//************************************
	// Method:    set_bit
	// FullName:  BufferMap::set_bit
	// Access:    public 
	// Returns:   int:success(0),failed(-1)
	// Qualifier:
	// Parameter: SegID segId
	//************************************
	int set_bit(SEGMENT_ID segId);


	//************************************
	// Method:    get_bit
	// FullName:  BufferMap::get_bit
	// Access:    public 
	// Returns:   bool
	// Qualifier:
	// Parameter: SegID segId
	//************************************
	bool get_bit(SEGMENT_ID segId);

	//************************************
	// Method:    get_bytes
	// FullName:  BufferMap::get_bytes
	// Access:    public 
	// Returns:   const char*
	// Qualifier: ת�����ֽ�����ע�⣺ʹ�÷�Ӧȷ����ȷ�ͷŷ��ص�����
	//************************************
	const char* get_bytes();

	//************************************
	// Method:    from_bytes
	// FullName:  BufferMap::from_bytes
	// Access:    public 
	// Returns:   void
	// Qualifier:�����ֽ����ָ�
	// Parameter: const char* p
	//************************************
	void from_bytes(const char* p);

	//************************************
	// Method:    time_step
	// FullName:  BufferMap::time_step
	// Access:    public 
	// Returns:   void
	// Qualifier: ����bufferMap��start_seg_id_++��������Ӧ����
	//************************************
	void time_step();

	void init(SEGMENT_ID seqId);

	size_t count()
	{
		return bits_.count();
	}

private:
	bitset<MAX_SEGMENT_COUNT> bits_;
	SEGMENT_ID start_seg_id_;
	ACE_Thread_Mutex mutex_; //��������Ϊpublic����ΪschedulerҪ�õ���
};
