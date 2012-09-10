//----------------------------------------------------------------
// 版权 (C) 2003-2003 
// 北京佳诚无限科技有限公司保留所有权
// 保留非商业目的的使用权利。
//
// 
// 功能说明：
//		北京佳诚无限科技有限公司安全模块
// 
// 
// 作者：苑旭
// Email：YuanXu@onest.net 
// 日期：2004/12/6
//----------------------------------------------------------------

using System;

namespace Ankh.Pusher.Security
{
	/// <summary>
	/// IOS专用加密/解密类
	/// </summary>
	public sealed class SecurityClass
	{
		private SecurityClass()
		{
		}

		private static byte[] Key ;//

		static SecurityClass()
		{
			Key = new byte[]{(byte)20,(byte)240,(byte)0,(byte)88,(byte)253,(byte)9,(byte)120,(byte)30};
		}
		
		/// <summary>
		/// 加密数据
		/// </summary>
		/// <param name="Source"></param>
		/// <returns></returns>
		public static string Encrypt(string Source)
		{
			DES des = new DES();
			des.SetKey(Key);
			return des.EncryptData(TrimString(Source));
		}

		/// <summary>
		/// 解密数据
		/// </summary>
		/// <param name="Source"></param>
		/// <returns></returns>
		public static string Decrypt(string Source)
		{
			DES des = new DES();
			des.SetKey(Key);
			return TrimString(des.DecryptData(Source));
		}
		/// <summary>
		/// 整理源字符串，将字符串中"\0"后边的去掉
		/// </summary>
		/// <param name="sourceString"></param>
		/// <returns></returns>
		private static string TrimString(string sourceString)
		{
			try
			{
				return sourceString.Substring(0,sourceString.IndexOf("\0"));
			}
			catch
			{
				return sourceString;
			}
			//return s;
		}
	}
}
