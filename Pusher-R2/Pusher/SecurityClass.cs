//----------------------------------------------------------------
// ��Ȩ (C) 2003-2003 
// �����ѳ����޿Ƽ����޹�˾��������Ȩ
// ��������ҵĿ�ĵ�ʹ��Ȩ����
//
// 
// ����˵����
//		�����ѳ����޿Ƽ����޹�˾��ȫģ��
// 
// 
// ���ߣ�Է��
// Email��YuanXu@onest.net 
// ���ڣ�2004/12/6
//----------------------------------------------------------------

using System;

namespace Ankh.Pusher.Security
{
	/// <summary>
	/// IOSר�ü���/������
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
		/// ��������
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
		/// ��������
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
		/// ����Դ�ַ��������ַ�����"\0"��ߵ�ȥ��
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
