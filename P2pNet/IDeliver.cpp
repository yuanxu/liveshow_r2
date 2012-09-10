#include "StdAfx.h"
#include "IDeliver.h"

IDeliver::IDeliver():lpMgr(NULL),delivered_header_(false),sentinel_(0)
{
}
IDeliver::IDeliver(PeerMgr *p):lpMgr(p),delivered_header_(false)
{
}

IDeliver::~IDeliver(void)
{
}
