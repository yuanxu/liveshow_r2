#include "stdafx.h"
#include "ace/OS_main.h"
#include "PeerMgr.h"
#include <iostream>
int ACE_TMAIN (int argc, ACE_TCHAR *argv[])
{
	
	PeerMgr mgr;
	
	using namespace std;
	char r ;
	while (true)
	{
	
		cout << "Please pick up the program's role. s for Server and c for Client" << endl ;
		
		cin >> r;
		if (r == 's' || r == 'S')
		{
			cout << "Now,Running as Server." << endl;
				mgr.svr_start(10001,8888,18086,18255);
				break;
		}
		else if (r == 'c' || r == 'C')
		{
			cout << "Now,Running as Client." << endl;
			mgr.set_proxy_server("192.168.0.40:8255");
			mgr.clt_start(10001,"192.168.0.40:18086");
			break;
		}
	}
	cout << endl << "Press 'q' to exit." << endl;
	while (true)
	{
		cin >> r;
		if (r =='q' || r == 'Q')
		{
			mgr.stop();
			break;
		}
		Sleep(50);
	}
	return 0;
}