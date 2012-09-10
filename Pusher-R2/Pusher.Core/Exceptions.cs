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
    /// 未找到频道对应的配置文件
    /// </summary>
    public class NoConfigFileException : PusherException
    {
        public NoConfigFileException() : base("未找到配置文件") { }
    }

    public class IncorrectConfigFileException : PusherException
    {
        public IncorrectConfigFileException() : base("配置文件格式不正确") { }
        public IncorrectConfigFileException(string Message) : base(String.Format( "配置文件格式不正确:{0}",Message)) { }
    }
}