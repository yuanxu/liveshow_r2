// -*- C++ -*-
//
// $Id: OS_NS_signal.inl 76184 2006-12-27 14:07:14Z johnnyw $

#include "ace/OS_NS_macros.h"
#include "ace/OS_NS_errno.h"

ACE_BEGIN_VERSIONED_NAMESPACE_DECL

namespace ACE_OS
{

ACE_INLINE int
kill (pid_t pid, int signum)
{
  ACE_OS_TRACE ("ACE_OS::kill");
#if defined (ACE_LACKS_KILL)
  ACE_UNUSED_ARG (pid);
  ACE_UNUSED_ARG (signum);
  ACE_NOTSUP_RETURN (-1);
#else
  ACE_OSCALL_RETURN (::kill (pid, signum), int, -1);
#endif /* ACE_LACKS_KILL */
}

ACE_INLINE int
pthread_sigmask (int how, const sigset_t *nsp, sigset_t *osp)
{
#if defined (ACE_HAS_PTHREADS_STD) && !defined (ACE_LACKS_PTHREAD_SIGMASK)
  int result;
  ACE_OSCALL_RETURN (ACE_ADAPT_RETVAL (::pthread_sigmask (how, nsp, osp),
                                       result),
                     int,
                     -1);
#else /* !ACE_HAS_PTHREADS_STD && !ACE_LACKS_PTHREAD_SIGMASK */
  ACE_UNUSED_ARG (how);
  ACE_UNUSED_ARG (nsp);
  ACE_UNUSED_ARG (osp);
  ACE_NOTSUP_RETURN (-1);
#endif /* ACE_HAS_PTHREADS_STD && !ACE_LACKS_PTHREAD_SIGMASK */
}

ACE_INLINE int
sigaction (int signum, const ACE_SIGACTION *nsa, ACE_SIGACTION *osa)
{
  ACE_OS_TRACE ("ACE_OS::sigaction");
  if (signum == 0)
    return 0;
#if defined (ACE_WIN32) && !defined (ACE_HAS_WINCE)
  struct sigaction sa;

  if (osa == 0)
    osa = &sa;

  if (nsa == 0)
    {
      osa->sa_handler = ::signal (signum, SIG_IGN);
      ::signal (signum, osa->sa_handler);
    }
  else
    osa->sa_handler = ::signal (signum, nsa->sa_handler);
  return osa->sa_handler == SIG_ERR ? -1 : 0;
#elif defined (ACE_LACKS_SIGACTION)
  ACE_UNUSED_ARG (nsa);
  ACE_UNUSED_ARG (osa);
  ACE_NOTSUP_RETURN (-1);
#elif !defined (ACE_HAS_SIGACTION_CONSTP2)
  ACE_OSCALL_RETURN (::sigaction (signum,
                                  const_cast<ACE_SIGACTION*> (nsa),
                                  osa),
                     int, -1);
#else
  ACE_OSCALL_RETURN (::sigaction (signum, nsa, osa), int, -1);
#endif /* ACE_WIN32 !ACE_HAS_WINCE */
}

ACE_INLINE int
sigaddset (sigset_t *s, int signum)
{
  ACE_OS_TRACE ("ACE_OS::sigaddset");
#if defined (ACE_LACKS_SIGSET) || defined (ACE_LACKS_SIGSET_DEFINITIONS)
  if (s == 0)
    {
      errno = EFAULT;
      return -1;
    }
  else if (signum < 1 || signum >= ACE_NSIG)
    {
      errno = EINVAL;
      return -1;                 // Invalid signum, return error
    }
  *s |= (1 << (signum - 1)) ;
  return 0 ;
#else
  ACE_OSCALL_RETURN (::sigaddset (s, signum), int, -1);
#endif /* ACE_LACKS_SIGSET || ACE_LACKS_SIGSET_DEFINITIONS */
}

ACE_INLINE int
sigdelset (sigset_t *s, int signum)
{
#if defined (ACE_LACKS_SIGSET) || defined (ACE_LACKS_SIGSET_DEFINITIONS)
  if (s == 0)
    {
      errno = EFAULT;
      return -1;
    }
  else if (signum < 1 || signum >= ACE_NSIG)
    {
      errno = EINVAL;
      return -1;                 // Invalid signum, return error
    }
  *s &= ~(1 << (signum - 1)) ;
  return 0;
#else
  ACE_OSCALL_RETURN (::sigdelset (s, signum), int, -1);
#endif /* ACE_LACKS_SIGSET || ACE_LACKS_SIGSET_DEFINITIONS */
}

ACE_INLINE int
sigemptyset (sigset_t *s)
{
#if defined (ACE_LACKS_SIGSET) || defined (ACE_LACKS_SIGSET_DEFINITIONS)
  if (s == 0)
    {
      errno = EFAULT;
      return -1;
    }
  *s = 0 ;
  return 0;
#else
  ACE_OSCALL_RETURN (::sigemptyset (s), int, -1);
#endif /* ACE_LACKS_SIGSET || ACE_LACKS_SIGSET_DEFINITIONS */
}

ACE_INLINE int
sigfillset (sigset_t *s)
{
#if defined (ACE_LACKS_SIGSET) || defined (ACE_LACKS_SIGSET_DEFINITIONS)
  if (s == 0)
    {
      errno = EFAULT;
      return -1;
    }
  *s = ~(sigset_t) 0;
  return 0 ;
#else
  ACE_OSCALL_RETURN (::sigfillset (s), int, -1);
#endif /* ACE_LACKS_SIGSET || ACE_LACKS_SIGSET_DEFINITIONS */
}

ACE_INLINE int
sigismember (sigset_t *s, int signum)
{
#if defined (ACE_LACKS_SIGSET) || defined (ACE_LACKS_SIGSET_DEFINITIONS)
  if (s == 0)
    {
      errno = EFAULT;
      return -1;
    }
  else if (signum < 1 || signum >= ACE_NSIG)
    {
      errno = EINVAL;
      return -1;                 // Invalid signum, return error
    }
  return ((*s & (1 << (signum - 1))) != 0) ;
#else
#  if defined (ACE_HAS_SIGISMEMBER_BUG)
  if (signum < 1 || signum >= ACE_NSIG)
    {
      errno = EINVAL;
      return -1;                 // Invalid signum, return error
    }
#  endif /* ACE_HAS_SIGISMEMBER_BUG */
  ACE_OSCALL_RETURN (::sigismember (s, signum), int, -1);
#endif /* ACE_LACKS_SIGSET || ACE_LACKS_SIGSET_DEFINITIONS */
}

ACE_INLINE ACE_SignalHandler
signal (int signum, ACE_SignalHandler func)
{
  if (signum == 0)
    return 0;
  else
# if defined (ACE_WIN32) && !defined (ACE_HAS_WINCE) || !defined (ACE_LACKS_UNIX_SIGNALS)
#  if !defined (ACE_HAS_TANDEM_SIGNALS) && !defined (ACE_HAS_LYNXOS_SIGNALS)
    return ::signal (signum, func);
#  else
    return (ACE_SignalHandler) ::signal (signum, (void (*)(int)) func);
#  endif /* !ACE_HAS_TANDEM_SIGNALS */
#else
    // @@ WINCE: Don't know how to implement signal on WinCE (yet.)
    ACE_UNUSED_ARG (signum);
    ACE_UNUSED_ARG (func);
    ACE_NOTSUP_RETURN (0);     // Should return SIG_ERR but it is not defined on WinCE.
#endif /* defined (ACE_WIN32) && !defined (ACE_HAS_WINCE) || !defined (ACE_LACKS_UNIX_SIGNALS) */
}

ACE_INLINE int
sigprocmask (int how, const sigset_t *nsp, sigset_t *osp)
{
#if defined (ACE_LACKS_SIGSET) || defined (ACE_LACKS_SIGSET_DEFINITIONS)
  ACE_UNUSED_ARG (how);
  ACE_UNUSED_ARG (nsp);
  ACE_UNUSED_ARG (osp);
  ACE_NOTSUP_RETURN (-1);
#else
  ACE_OSCALL_RETURN (::sigprocmask (how, nsp, osp), int, -1);
#endif /* ACE_LACKS_SIGSET || ACE_LACKS_SIGSET_DEFINITIONS */
}

ACE_INLINE int
sigsuspend (const sigset_t *sigset)
{
#if defined (ACE_HAS_SIGSUSPEND)
  sigset_t s;

  if (sigset == 0)
    {
      sigset = &s;
      ACE_OS::sigemptyset (&s);
    }
  ACE_OSCALL_RETURN (::sigsuspend (sigset), int, -1);
#else
  ACE_UNUSED_ARG (sigset);
  ACE_NOTSUP_RETURN (-1);
#endif /* ACE_HAS_SIGSUSPEND */
}

}  /* end namespace ACE_OS */

ACE_END_VERSIONED_NAMESPACE_DECL
