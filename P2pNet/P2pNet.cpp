// P2pNet.cpp : 定义 DLL 应用程序的入口点。
//

#include "stdafx.h"
#include "P2pNet.h"


#ifdef _MANAGED
#pragma managed(push, off)
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif



// 这是已导出类的构造函数。
// 有关类定义的信息，请参阅 P2pNet.h
CP2pNet::CP2pNet()
{
	return;
}
