// P2pNet.cpp : ���� DLL Ӧ�ó������ڵ㡣
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



// �����ѵ�����Ĺ��캯����
// �й��ඨ�����Ϣ������� P2pNet.h
CP2pNet::CP2pNet()
{
	return;
}
