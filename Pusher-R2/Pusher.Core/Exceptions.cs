using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Pusher.Core
{
    public class PusherException : ApplicationException
    {
        public PusherException() : base() { }
        public PusherException(string Message) : base(Message) { }
    }

    /// <summary>
    /// δ�ҵ�Ƶ����Ӧ�������ļ�
    /// </summary>
    public class NoConfigFileException : PusherException
    {
        public NoConfigFileException() : base("δ�ҵ������ļ�") { }
    }

    public class IncorrectConfigFileException : PusherException
    {
        public IncorrectConfigFileException() : base("�����ļ���ʽ����ȷ") { }
        public IncorrectConfigFileException(string Message) : base(String.Format( "�����ļ���ʽ����ȷ:{0}",Message)) { }
    }
}