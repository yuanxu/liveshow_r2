#pragma once
#include "ace\Thread_Mutex.h"
#include "ace\guard_t.h"
#include "ace\OS.h"
#include "ace\Time_Value.h"

class FluxStatistic
{
public:
	FluxStatistic(void);
	~FluxStatistic(void);
private:
	ACE_Thread_Mutex mutex_;

public:
	volatile LONG BytesRead,
		BytesSent,
		/*StartTime,*/
		BytesReadLast,
		BytesSentLast
		/*,StartTimeLast*/;
	volatile LONG AverageSentKBps,AverageReadKBps,CurrentSentKBps,CurrentReadKBps;
	ACE_Time_Value StartTime,StartTimeLast;
	//initialize 
	void Init()
	{
		BytesRead = BytesSent=BytesReadLast=BytesSentLast = 0;
		StartTimeLast = StartTime = ACE_OS::gettimeofday();// GetTickCount(); //GetTickCount是一个win32函数，影响移植性
		AverageSentKBps = 0,AverageReadKBps = 0,CurrentSentKBps = 0,CurrentReadKBps = 0;
	}

	//Calculate statistic data
	void Calculate()
	{
		ACE_Guard<ACE_Thread_Mutex> guard_(mutex_);
		ULONG    elapsed;

		ACE_Time_Value tick = ACE_OS::gettimeofday();//GetTickCount();

		elapsed = (tick.msec() - StartTime.msec()) / 1000;

		if (elapsed == 0)
			return;

		AverageSentKBps = BytesSent / elapsed /1024;
		//InterlockedExchange(&AverageSentKBps,BytesSent / elapsed /1024);
		//printf("Average BPS sent: %lu [%lu]\n", bps, gBytesSent);

		AverageReadKBps = BytesRead / elapsed /1024;
		//InterlockedExchange(&AverageReadKBps,BytesRead / elapsed /1024);
		//printf("Average BPS read: %lu [%lu]\n", bps, gBytesRead);

		elapsed = (tick.msec() - StartTimeLast.msec()) / 1000;

		if (elapsed == 0)
			return;

		CurrentSentKBps = BytesSentLast / elapsed /1024;
		//InterlockedExchange(&CurrentSentKBps, BytesSentLast / elapsed /1024);
		//printf("Current BPS sent: %lu\n", bps);

		CurrentReadKBps = BytesReadLast / elapsed /1024;
		//InterlockedExchange(&CurrentReadKBps,BytesReadLast / elapsed /1024);
		//printf("Current BPS read: %lu\n", bps);

		BytesSentLast=0;
		BytesReadLast=0;
		//InterlockedExchange(&BytesSentLast, 0);
		//InterlockedExchange(&BytesReadLast, 0);

		StartTimeLast = tick;
	}
};
