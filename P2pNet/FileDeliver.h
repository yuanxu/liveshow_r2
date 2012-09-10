#pragma once
#include "ideliver.h"
#include <fstream>

class PeerMgr;
using namespace std;
class FileDeliver :
	public IDeliver
{
private:
	ofstream * lpFile;
public:
	FileDeliver(void);
	FileDeliver(PeerMgr *p);
	~FileDeliver(void);

	void got_one_second_data(SEGMENT_ID segId,ACE_Message_Block* mb_);
};
