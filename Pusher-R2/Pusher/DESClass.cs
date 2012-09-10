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
// 作者：薛立鹏
// Email：XueLiPeng@onest.net 
// 日期：2003/2/18
//----------------------------------------------------------------


using System;

namespace Ankh.Pusher.Security
{
	/// <summary>
	/// DES加密类
	/// </summary>
	public class DES
	{
		public enum TYPE_ENMU	{ENCRYPT,DECRYPT};
		// initial permutation IP
		static byte [] IP_Table = 
		{
			58, 50, 42, 34, 26, 18, 10, 2,	 60, 52, 44, 36, 28, 20, 12, 4,
			62, 54, 46, 38, 30, 22, 14, 6,	 64, 56, 48, 40, 32, 24, 16, 8,
			57, 49, 41, 33, 25, 17,  9, 1,	 59, 51, 43, 35, 27, 19, 11, 3,
			61, 53, 45, 37, 29, 21, 13, 5,	 63, 55, 47, 39, 31, 23, 15, 7
		};
		// final permutation IP^-1 
		static  byte[] IPR_Table = 
		{
			40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31,
			38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29,
			36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27,
			34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41,  9, 49, 17, 57, 25
		};
		// expansion operation matrix
		static byte[] E_Table = 
		{
			32,  1,  2,  3,  4,  5,  4,  5,  6,  7,  8,  9,
			8,  9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17,
			16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25,
			24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32,  1
		};
		// 32-bit permutation function P used on the output of the S-boxes 
		static  byte[] P_Table = 
		{
			16, 7, 20, 21, 29, 12, 28, 17, 1,  15, 23, 26, 5,  18, 31, 10,
			2,  8, 24, 14, 32, 27, 3,  9,  19, 13, 30, 6,  22, 11, 4,  25
		};
		// permuted choice table (key) 
		static  byte[] PC1_Table = 
		{
			57, 49, 41, 33, 25, 17,  9,  1, 58, 50, 42, 34, 26, 18,
			10,  2, 59, 51, 43, 35, 27, 19, 11,  3, 60, 52, 44, 36,
			63, 55, 47, 39, 31, 23, 15,  7, 62, 54, 46, 38, 30, 22,
			14,  6, 61, 53, 45, 37, 29, 21, 13,  5, 28, 20, 12,  4
		};
		// permuted choice key (table) 
		static   byte[] PC2_Table = 
		{
			14, 17, 11, 24,  1,  5,  3, 28, 15,  6, 21, 10,
			23, 19, 12,  4, 26,  8, 16,  7, 27, 20, 13,  2,
			41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48,
			44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32
		};
		// number left rotations of pc1 
		static  byte [] LOOP_Table = 
		{
			1,1,2,2,2,2,2,2,1,2,2,2,2,2,2,1
		};
		// The (in)famous S-boxes 
				
		static   byte[,,] S_Box =  
		{
			// S1 
			{
				{14, 4,	13,	 1,  2, 15, 11,  8,  3, 10,  6, 12,  5,  9,  0,  7},
				{0, 15,  7,  4, 14,  2, 13,  1, 10,  6, 12, 11,  9,  5,  3,  8},
				{4,  1, 14,  8, 13,  6,  2, 11, 15, 12,  9,  7,  3, 10,  5,  0},
				{15, 12,  8,  2,  4,  9,  1,  7,  5, 11,  3, 14, 10,  0,  6, 13}
			},
			
			// S2 
			{
				{15,  1,  8, 14,  6, 11,  3,  4,  9,  7,  2, 13, 12,  0,  5, 10},
				{3, 13,  4,  7, 15,  2,  8, 14, 12,  0,  1, 10,  6,  9, 11,  5},
				{0, 14,  7, 11, 10,  4, 13,  1,  5,  8, 12,  6,  9,  3,  2, 15},
				{13,  8, 10,  1,  3, 15,  4,  2, 11,  6,  7, 12,  0,  5, 14,  9}
			},
			// S3 
			{
				{10,  0,  9, 14,  6,  3, 15,  5,  1, 13, 12,  7, 11,  4,  2,  8},
				{13,  7,  0,  9,  3,  4,  6, 10,  2,  8,  5, 14, 12, 11, 15,  1},
				{13,  6,  4,  9,  8, 15,  3,  0, 11,  1,  2, 12,  5, 10, 14,  7},
				{1, 10, 13,  0,  6,  9,  8,  7,  4, 15, 14,  3, 11,  5,  2, 12}
			},
			// S4 
			{
				{7, 13, 14,  3,  0,  6,  9, 10,  1,  2,  8,  5, 11, 12,  4, 15},
				{13,  8, 11,  5,  6, 15,  0,  3,  4,  7,  2, 12,  1, 10, 14,  9},
				{10,  6,  9,  0, 12, 11,  7, 13, 15,  1,  3, 14,  5,  2,  8,  4},
				{3, 15,  0,  6, 10,  1, 13,  8,  9,  4,  5, 11, 12,  7,  2, 14}
			},
			
			// S5 
			{
				{2, 12,  4,  1,  7, 10, 11,  6,  8,  5,  3, 15, 13,  0, 14,  9},
				{14, 11,  2, 12,  4,  7, 13,  1,  5,  0, 15, 10,  3,  9,  8,  6},
				{4,  2,  1, 11, 10, 13,  7,  8, 15,  9, 12,  5,  6,  3,  0, 14},
				{11,  8, 12,  7,  1, 14,  2, 13,  6, 15,  0,  9, 10,  4,  5,  3}
			},
			
			// S6 
			{
				{12,  1, 10, 15,  9,  2,  6,  8,  0, 13,  3,  4, 14,  7,  5, 11},
				{10, 15,  4,  2,  7, 12,  9,  5,  6,  1, 13, 14,  0, 11,  3,  8},
				{9, 14, 15,  5,  2,  8, 12,  3,  7,  0,  4, 10,  1, 13, 11,  6},
				{4,  3,  2, 12,  9,  5, 15, 10, 11, 14,  1,  7,  6,  0,  8, 13}
			},
			
			// S7 
			{
				{4, 11,  2, 14, 15,  0,  8, 13,  3, 12,  9,  7,  5, 10,  6,  1},
				{13,  0, 11,  7,  4,  9,  1, 10, 14,  3,  5, 12,  2, 15,  8,  6},
				{1,  4, 11, 13, 12,  3,  7, 14, 10, 15,  6,  8,  0,  5,  9,  2},
				{6, 11, 13,  8,  1,  4, 10,  7,  9,  5,  0, 15, 14,  2,  3, 12}
			},
			
			// S8 
			{
				{13,  2,  8,  4,  6, 15, 11,  1, 10,  9,  3, 14,  5,  0, 12,  7},
				{1, 15, 13,  8, 10,  3,  7,  4, 12,  5,  6, 11,  0, 14,  9,  2},
				{7, 11,  4,  1,  9, 12, 14,  2,  0,  6, 10, 13, 15,  3,  5,  8},
				{2,  1, 14,  7,  4, 10,  8, 13, 15, 12,  9,  0,  3,  5,  6, 11}
			}
			
		};

		byte [][] SubKey = null;// 16圈子密钥
		bool m_bSetKeyed;

		public DES()
		{

			SubKey = new byte[16][];
			for(int i=0; i<16; i++)
			{
				SubKey[i] = new byte[48];
			}	
			m_bSetKeyed = false;

		}
	
		/// <summary>
		/// 把字符转产成 bit 数组（用无符号字符型存放的 ）
		/// </summary>
		/// <param name="Out">bit 数组</param>
		/// <param name="In">字符</param>
		/// <param name="bits">bit 数组的长度</param>
		private void ByteToBit(byte[] Out, byte[] In, int bits)
		{
			for(int i=0; i<bits; i++)
			{
				Out[i] = (byte)((In[i/8]>>(i%8)) & 1);
			}
		}		

		/// <summary>
		/// 对密匙进行换位转换，同时去掉校验位。
		/// </summary>
		/// <param name="Out">被转换的密匙</param>
		/// <param name="In">已经转换的密匙</param>
		/// <param name="Table">转换的规则</param>
		/// <param name="len">转换后的密匙的长度</param>
		private void Transform(byte[] Out, byte [] In, byte[] Table, int len)
		{
			byte []  Tmp = new byte [len];

			for(int i=0; i<len; i++)
			{
				Tmp[i] = In[ Table[i]-1 ];
			}

			for(int i=0; i<len; i++)
			{
				Out[i] = Tmp[i];
			}
		}
		/// <summary>
		/// 实现循环左移的功能
		/// </summary>
		/// <param name="In">被处理的数据</param>
		/// <param name="len">被处理数据的总长度</param>
		/// <param name="loop">循环左移的位数</param>
		private void RotateL(byte[] In, int len, int loop)
		{
			byte [] Tmp = new byte[256];
			In.CopyTo(Tmp,0); 
			
			int i = 0;
			for(; i<len-loop; i++)
			{
				In[i] = Tmp[i+loop];
			}

			for(int j=0; j<loop; j++)
			{
				In[i+j] = Tmp[j];
			}
		}

	
		private void Des_SetKey(byte[] Key)
		{
			byte[] K = new byte[64];
			byte[] KL = new byte[28];
			byte[] KR = new byte[28];

			
			ByteToBit(K, Key, 64);  //把密码转换为bit数组
			Transform(K, K, PC1_Table, 56); //bit 数组的位调换，同时完成验证码的去除
			for(int i=0; i<16; i++) 
			{
				string szK = DebugOut(K);

				KL = this.ArraySub(K,0,28);
				string szKL = DebugOut(KL);
				KR = this.ArraySub(K,28,28);
				string  szKR = this.DebugOut(KR);
				RotateL(KL, 28, LOOP_Table[i]);
				RotateL(KR, 28, LOOP_Table[i]);
				K = this.ArrayJoin(KL,KR);
				string szKK = this.DebugOut(K);
				Transform(SubKey[i], K, PC2_Table, 48);

				string szKKK = DebugOut(K); 

				string szSubK =  DebugOut(SubKey[i]);
				

			}

			string [] subkey = new string[16];
			for(int j=0; j<16; j++)
			{
				subkey[j] = DebugOut(this.SubKey[j]);
			}
			m_bSetKeyed = true;

		}

		/// <summary>
		/// 连接数组
		/// </summary>
		/// <param name="head">数组头</param>
		/// <param name="tail">数组的尾巴</param>
		/// <returns>连接好的数组</returns>
		private byte[] ArrayJoin(byte[] head, byte[] tail)
		{
			byte[] t = new byte[head.Length + tail.Length ];
			head.CopyTo(t,0);
			tail.CopyTo(t,head.Length);
			return t;
		}

		/// <summary>
		/// 获取子数组
		/// </summary>
		/// <param name="source">数组源</param>
		/// <param name="nStart">开始位置，从0开始计数</param>
		/// <param name="nLength">子数组的长度</param>
		/// <returns>子数组</returns>
		private byte[] ArraySub(byte [] source,int nStart, int nLength)
		{
			if(nLength> source.Length || nStart>source.Length || nStart<0)
			{
				return null;
			}
			
			byte[] t = new byte[nLength];

			try
			{
				for(int i=0; i<nLength; i++)
				{
					t[i] = source[nStart+i];
				}
			}
			catch
			{
			}

			return t;

		}

		/// <summary>
		/// 加密解密运算
		/// </summary>
		/// <param name="Out">处理完的结果</param>
		/// <param name="In">被处理数据</param>
		/// <param name="Type">用于区分，是加密还是解密</param>
		private void Des_Run(byte [] Out, byte [] In, TYPE_ENMU Type)
		{
			byte[] M = new byte[64];
			byte[]Tmp = new byte[32];
			byte[]Li =new byte[32];
			byte[] Ri =new byte[32];
			
			ByteToBit(M, In, 64);//转换成bit数组
			Transform(M, M, IP_Table, 64);//进行位交换

			string szBitF = this.DebugOut(M);
			Li = this.ArraySub(M,0,32);
			Ri = this.ArraySub(M,32,32); 
			
			if( Type == TYPE_ENMU.ENCRYPT  ) //加密运算
			{
				for(int i=0; i<16; i++) 
				{
					Ri.CopyTo(Tmp,0);
					string szRi0 = this.DebugOut(Ri);
					F_func(Ri, SubKey[i]);// 对32位的数据做复杂转换
					string szRi1 = this.DebugOut(Ri);
					Xor(Ri, Li, 32); 
					string szRi2 = this.DebugOut(Ri);
					string szLi1 = this.DebugOut(Li);

					Tmp.CopyTo(Li,0);

					string szRi4 = this.DebugOut(Ri);
					string szLi4 = this.DebugOut(Li);
					
				}
			}
			else//解密运算
			{
				for(int i=15; i>=0; i--) 
				{
					Li.CopyTo(Tmp,0);
					F_func(Li, SubKey[i]);
					Xor(Li, Ri, 32);
					Tmp.CopyTo(Ri,0);
				}
			}
			string szRi5 = this.DebugOut(Ri);
			string szLi5 = this.DebugOut(Li);

			M = this.ArrayJoin(Li,Ri); 
			string szM1 = this.DebugOut(M);
			Transform(M, M, IPR_Table, 64);
			string szM2 = this.DebugOut(M);
			BitToByte(Out, M, 64);

			string szOut4 = this.DebugOut(Out);
		}

		/// <summary>
		/// 把bit流转换成字节
		/// </summary>
		/// <param name="Out">字节</param>
		/// <param name="In">bit 流</param>
		/// <param name="bits">bit 流的长度</param>
		private void BitToByte(byte[] Out, byte [] In, int bits)
		{
			byte [] temp = new byte[(bits+7)/8];

			for(int i=0; i<bits; i++)
			{
				temp[i/8] |= (byte)(In[i]<<(i%8));
			}

			temp.CopyTo(Out,0); 
		}

		/// <summary>
		/// bit流的异或运算
		/// </summary>
		/// <param name="InA">目标a,同时运算的结果存于此变量</param>
		/// <param name="InB">目标b</param>
		/// <param name="len">长度</param>
		private  void Xor(byte[] InA, byte [] InB, int len)
		{
			for(int i=0; i<len; i++)
			{
				InA[i] ^= InB[i];
			}
		}

		/// <summary>
		/// 对32位的数据做复杂转换
		/// </summary>
		/// <param name="In">输入、输出数据</param>
		/// <param name="Ki">子密匙</param>
		private void F_func(byte[] In, byte [] Ki)
		{
			byte [] MR = new byte[48];

			string xlpIn0 = this.DebugOut(In);
			Transform(MR, In, E_Table, 48);//扩充
			string xlpMR1 = this.DebugOut(MR);
			string xlpIn1 = this.DebugOut(In);



			string xlpKi0 = this.DebugOut(Ki);
			Xor(MR, Ki, 48);//和子密匙作异或运算
			string xlpKi1 = this.DebugOut(Ki);
			string xlpMR3 = this.DebugOut(MR);
			S_func(In, MR); //利用密盒转换数据
			string xlpMR2 = this.DebugOut(MR);
			string xlpIn2 = this.DebugOut(In);
			Transform(In, In, P_Table, 32);//数据的换位
		}
		
		/// <summary>
		/// 从密盒中取出相对应的数据。把48位的数据转换为32位的数据
		/// </summary>
		/// <param name="Out">48位的输入数据</param>
		/// <param name="In">32位的输出数据</param>
		private void S_func(byte [] Out, byte[] In)
		{
			byte []  OutRet = new byte[0];

			int nOutIndex = 0;
			int nInIndex = 0;

			for(byte i=0,j,k; i<8; i++,nOutIndex+=6,nInIndex+=4)
			{
				byte[] InTmp = this.ArraySub(In,i*6,6) ;
				byte[] OutTmp = new byte[4];

				j = (byte)((InTmp[0]<<1) + InTmp[5]);
				k = (byte)((InTmp[1]<<3) + (InTmp[2]<<2) + (InTmp[3]<<1) + InTmp[4]);

				byte [] bTmp = new byte[1];
				bTmp[0] = S_Box[i,j,k];

				ByteToBit(OutTmp, bTmp, 4);

				OutRet = this.ArrayJoin(OutRet, OutTmp); 
			}
			OutRet.CopyTo(Out,0); 
			string xlpOut  = this.DebugOut(Out);
		}

		public string DebugOut(byte[] In)
		{
			string szRet = "";
			for(int i = 0; i<In.Length; i++)
			{
				szRet += string.Format("{0,5:X}",In[i]);
			}
			return szRet;

		}

		public void SetKey(byte[] byKey)
		{
			if(byKey.Length !=8)
			{
				throw new SelfException("密码必须为8个字符"); 
			}
			else
			{
				this.Des_SetKey(byKey);
				m_bSetKeyed = true;
			}
		}//end func

		/// <summary>
		/// 对数据加密，采用ASCII处理文本
		/// </summary>
		/// <param name="Source"></param>
		/// <returns></returns>
		public string EncryptData(string Source)
		{
			byte[] binData = System.Text.Encoding.ASCII.GetBytes(Source);
			byte[] boutData = EncryptData(binData);
			return Convert.ToBase64String(boutData);
		}
		/// <summary>
		/// 对数据加密
		/// </summary>
		/// <param name="byInData">被加密数据</param>
		/// <returns>加密的结果</returns>
		public byte[] EncryptData(byte[] byInData)
		{
			
			if(m_bSetKeyed)
			{
				byte[] byOutData =    new byte[8*((byInData.Length+7)/8)];
				byte[] byInDateTemp = new byte[8*((byInData.Length+7)/8)];
				byInData.CopyTo(byInDateTemp,0);				


				for(int k=0; k<byInDateTemp.Length/8; k++)
				{
					byte[] byElem = this.ArraySub(byInDateTemp,k*8,8);
					byte[] byRet = new byte[8];					

					this.Des_Run(byRet,byElem,DES.TYPE_ENMU.ENCRYPT);
					byRet.CopyTo(byOutData, k*8);
				}
				return byOutData;
			}
			else
			{
				throw new SelfException("没有设置密码！不能加密！");
			}
		}//end func

		/// <summary>
		/// 对数据解密，采用ASCII处理文本
		/// </summary>
		/// <param name="Source"></param>
		/// <returns></returns>
		public string DecryptData(string Source)
		{
			byte[] binData = Convert.FromBase64String(Source);
			byte[] boutData = DecryptData(binData);
			return System.Text.Encoding.ASCII.GetString(boutData);
		}

		/// <summary>
		/// 对数据解密
		/// </summary>
		/// <param name="byarrInData">被解密数据</param>
		/// <returns>解密的结果</returns>
		public byte[] DecryptData(byte[] byarrInData)
		{
			if(m_bSetKeyed)
			{
				byte[] byInDataTemp = new byte[8*((byarrInData.Length+7)/8)];
				byte[] byOutData = new byte[8*((byarrInData.Length+7)/8)];

				byarrInData.CopyTo(byInDataTemp,0);

				for(int i=0; i<byInDataTemp.Length/8; i++)
				{
					byte[] byElem = this.ArraySub(byInDataTemp,i*8,8);

					byte[] byOut = new byte[8];
					this.Des_Run(byOut, byElem,DES.TYPE_ENMU.DECRYPT); 

					byOut.CopyTo(byOutData,i*8);
				}
				return byOutData;
			}
			else
			{
				throw new SelfException("没有设置密码！不能解密！");
			}
		}


		
	}

	public class SelfException:ApplicationException
	{
		public SelfException(string Message):base(Message)
		{
		}
	}

}
