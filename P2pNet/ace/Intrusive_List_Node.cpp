// $Id: Intrusive_List_Node.cpp 69051 2005-10-28 16:14:56Z ossama $

#ifndef ACE_INTRUSIVE_LIST_NODE_CPP
#define ACE_INTRUSIVE_LIST_NODE_CPP

#include "ace/Intrusive_List_Node.h"

#if !defined (ACE_LACKS_PRAGMA_ONCE)
# pragma once
#endif /* ACE_LACKS_PRAGMA_ONCE */

#if !defined (__ACE_INLINE__)
#include "ace/Intrusive_List_Node.inl"
#endif /* __ACE_INLINE__ */

ACE_BEGIN_VERSIONED_NAMESPACE_DECL

template<class T>
ACE_Intrusive_List_Node<T>::ACE_Intrusive_List_Node (void)
  : prev_ (0)
  , next_ (0)
{
}

ACE_END_VERSIONED_NAMESPACE_DECL

#endif /* ACE_INTRUSIVE_LIST_NODE_CPP */
