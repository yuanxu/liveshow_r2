// -*- C++ -*-
//
// $Id: RW_Thread_Mutex.inl 69051 2005-10-28 16:14:56Z ossama $

ACE_BEGIN_VERSIONED_NAMESPACE_DECL

ACE_INLINE int
ACE_RW_Thread_Mutex::tryacquire_write_upgrade (void)
{
// ACE_TRACE ("ACE_RW_Thread_Mutex::tryacquire_write_upgrade");
  return ACE_OS::rw_trywrlock_upgrade (&this->lock_);
}

ACE_INLINE
ACE_RW_Thread_Mutex::~ACE_RW_Thread_Mutex (void)
{
}

ACE_END_VERSIONED_NAMESPACE_DECL
