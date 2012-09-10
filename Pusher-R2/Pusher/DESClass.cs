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
// ���ߣ�Ѧ����
// Email��XueLiPeng@onest.net 
// ���ڣ�2003/2/18
//----------------------------------------------------------------


using System;

namespace Ankh.Pusher.Security
{
	/// <summary>
	/// DES������
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

		byte [][] SubKey = null;// 16Ȧ����Կ
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
		/// ���ַ�ת���� bit ���飨���޷����ַ��ʹ�ŵ� ��
		/// </summary>
		/// <param name="Out">bit ����</param>
		/// <param name="In">�ַ�</param>
		/// <param name="bits">bit ����ĳ���</param>
		private void ByteToBit(byte[] Out, byte[] In, int bits)
		{
			for(int i=0; i<bits; i++)
			{
				Out[i] = (byte)((In[i/8]>>(i%8)) & 1);
			}
		}		

		/// <summary>
		/// ���ܳ׽��л�λת����ͬʱȥ��У��λ��
		/// </summary>
		/// <param name="Out">��ת�����ܳ�</param>
		/// <param name="In">�Ѿ�ת�����ܳ�</param>
		/// <param name="Table">ת���Ĺ���</param>
		/// <param name="len">ת������ܳ׵ĳ���</param>
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
		/// ʵ��ѭ�����ƵĹ���
		/// </summary>
		/// <param name="In">�����������</param>
		/// <param name="len">���������ݵ��ܳ���</param>
		/// <param name="loop">ѭ�����Ƶ�λ��</param>
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

			
			ByteToBit(K, Key, 64);  //������ת��Ϊbit����
			Transform(K, K, PC1_Table, 56); //bit �����λ������ͬʱ�����֤���ȥ��
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
		/// ��������
		/// </summary>
		/// <param name="head">����ͷ</param>
		/// <param name="tail">�����β��</param>
		/// <returns>���Ӻõ�����</returns>
		private byte[] ArrayJoin(byte[] head, byte[] tail)
		{
			byte[] t = new byte[head.Length + tail.Length ];
			head.CopyTo(t,0);
			tail.CopyTo(t,head.Length);
			return t;
		}

		/// <summary>
		/// ��ȡ������
		/// </summary>
		/// <param name="source">����Դ</param>
		/// <param name="nStart">��ʼλ�ã���0��ʼ����</param>
		/// <param name="nLength">������ĳ���</param>
		/// <returns>������</returns>
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
		/// ���ܽ�������
		/// </summary>
		/// <param name="Out">������Ľ��</param>
		/// <param name="In">����������</param>
		/// <param name="Type">�������֣��Ǽ��ܻ��ǽ���</param>
		private void Des_Run(byte [] Out, byte [] In, TYPE_ENMU Type)
		{
			byte[] M = new byte[64];
			byte[]Tmp = new byte[32];
			byte[]Li =new byte[32];
			byte[] Ri =new byte[32];
			
			ByteToBit(M, In, 64);//ת����bit����
			Transform(M, M, IP_Table, 64);//����λ����

			string szBitF = this.DebugOut(M);
			Li = this.ArraySub(M,0,32);
			Ri = this.ArraySub(M,32,32); 
			
			if( Type == TYPE_ENMU.ENCRYPT  ) //��������
			{
				for(int i=0; i<16; i++) 
				{
					Ri.CopyTo(Tmp,0);
					string szRi0 = this.DebugOut(Ri);
					F_func(Ri, SubKey[i]);// ��32λ������������ת��
					string szRi1 = this.DebugOut(Ri);
					Xor(Ri, Li, 32); 
					string szRi2 = this.DebugOut(Ri);
					string szLi1 = this.DebugOut(Li);

					Tmp.CopyTo(Li,0);

					string szRi4 = this.DebugOut(Ri);
					string szLi4 = this.DebugOut(Li);
					
				}
			}
			else//��������
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
		/// ��bit��ת�����ֽ�
		/// </summary>
		/// <param name="Out">�ֽ�</param>
		/// <param name="In">bit ��</param>
		/// <param name="bits">bit ���ĳ���</param>
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
		/// bit�����������
		/// </summary>
		/// <param name="InA">Ŀ��a,ͬʱ����Ľ�����ڴ˱���</param>
		/// <param name="InB">Ŀ��b</param>
		/// <param name="len">����</param>
		private  void Xor(byte[] InA, byte [] InB, int len)
		{
			for(int i=0; i<len; i++)
			{
				InA[i] ^= InB[i];
			}
		}

		/// <summary>
		/// ��32λ������������ת��
		/// </summary>
		/// <param name="In">���롢�������</param>
		/// <param name="Ki">���ܳ�</param>
		private void F_func(byte[] In, byte [] Ki)
		{
			byte [] MR = new byte[48];

			string xlpIn0 = this.DebugOut(In);
			Transform(MR, In, E_Table, 48);//����
			string xlpMR1 = this.DebugOut(MR);
			string xlpIn1 = this.DebugOut(In);



			string xlpKi0 = this.DebugOut(Ki);
			Xor(MR, Ki, 48);//�����ܳ����������
			string xlpKi1 = this.DebugOut(Ki);
			string xlpMR3 = this.DebugOut(MR);
			S_func(In, MR); //�����ܺ�ת������
			string xlpMR2 = this.DebugOut(MR);
			string xlpIn2 = this.DebugOut(In);
			Transform(In, In, P_Table, 32);//���ݵĻ�λ
		}
		
		/// <summary>
		/// ���ܺ���ȡ�����Ӧ�����ݡ���48λ������ת��Ϊ32λ������
		/// </summary>
		/// <param name="Out">48λ����������</param>
		/// <param name="In">32λ���������</param>
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
				throw new SelfException("�������Ϊ8���ַ�"); 
			}
			else
			{
				this.Des_SetKey(byKey);
				m_bSetKeyed = true;
			}
		}//end func

		/// <summary>
		/// �����ݼ��ܣ�����ASCII�����ı�
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
		/// �����ݼ���
		/// </summary>
		/// <param name="byInData">����������</param>
		/// <returns>���ܵĽ��</returns>
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
				throw new SelfException("û���������룡���ܼ��ܣ�");
			}
		}//end func

		/// <summary>
		/// �����ݽ��ܣ�����ASCII�����ı�
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
		/// �����ݽ���
		/// </summary>
		/// <param name="byarrInData">����������</param>
		/// <returns>���ܵĽ��</returns>
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
				throw new SelfException("û���������룡���ܽ��ܣ�");
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
