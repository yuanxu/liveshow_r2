using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Ankh.Pusher.Core
{
    internal class AsfLiveSource : MediaSource
    {
        private bool bIsRunning = false;
        public override void Start()
        {
            bIsRunning = true;
            
            HttpWebRequest request = null; 
            HttpWebResponse response = null;
            try
            {
                request = GetRequest(true);
                response = (HttpWebResponse)request.GetResponse();
                ProcessWmeResponse(response);
                response.Close();
            }
            catch (WebException )
            {
                if (response != null)
                {
                    try
                    {
                        response.Close();
                        request.Abort();
                    }
                    catch {  }
                }
                throw;
            }
        }
        

        public override void Stop()
        {
            bIsRunning = false;
        }


        private HttpWebRequest GetRequest(bool isFirstRequest)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://{0}:{1}/",Profile.LiveSrvIP,Profile.LiveSrvPort));
            request.SendChunked = false;
            request.UserAgent = "NSPlayer/4.1.0.3925";
            request.Headers.Add("Pragma", "no-cache,rate=1.000000,stream-time=0,stream-offset=0:0,request-context=" + (isFirstRequest ? "1" : "2") + ",max-duration=0");
            request.Headers.Add("Pragma", "xClientGUID={2200AD50-2C39-46c0-AE0A-2CA76D8C766D}");
            request.Method = "GET";
            request.ContentLength = 0;
            request.Accept = "*/*";
            request.KeepAlive = false;
            return request;
        }
        private void ProcessWmeResponse(HttpWebResponse response)
        {
            System.IO.Stream stream = response.GetResponseStream();
            System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
            MediaHeader mHeader = new MediaHeader();
            MediaBody mBody =new MediaBody();
            //mBody.AsfHeader = mHeader.AsfHeader;
            mBody.FileID = 1;
            

            uint iCounter = 1;
            
            try
            {
                
                while (bIsRunning)
                {
                    AsfChunk chunk = new AsfChunk();
                    chunk.Cmd  = reader.ReadUInt16();
                    chunk.Length = reader.ReadUInt16(); //length;
                    chunk.SeqNumber = reader.ReadUInt32();
                    chunk.Unknown = reader.ReadUInt16();//unknown
                    chunk.ConfirmLength = reader.ReadUInt16();
                    //chunk.SeqNumber = iCounter;

                    //System.Diagnostics.Debug.Assert(chunk.Length == chunk.ConfirmLength);
                    if (chunk.Length != chunk.ConfirmLength)
                        throw new WebException();
                    int ird = 0;
                    chunk.Length -= 8;//只保留真实数据的长度
                    byte[] data = new byte[chunk.Length];
                    chunk.ConfirmLength -= 8;
                    while (ird < chunk.Length)
                    {
                        ird += reader.Read(data, 0 + ird, chunk.Length - ird );
                    }
                    
                    switch (chunk.Cmd)
                    {
                        case 0x4824://header
                            mHeader.FileID = 1;
                            mHeader.AsfHeader.SeqNumber= chunk.SeqNumber;
                            mHeader.AsfHeader.Data = data;

                            GetNextSequenceID();//1

                            mHeader.SequenceID =  GetCurrentSequenceID();
                            mBody.AsfHeader = mHeader.AsfHeader;
                            OnGetHeader(new MediaDataArgs(mHeader));
                            
                            break;
                        case 0x4424://data
                            System.Diagnostics.Debug.Assert(chunk.Length <= mHeader.AsfHeader.MinPacketSize);

                            
                            //fix padding data

                            if (data.Length < mHeader.AsfHeader.MaxPacketSize) //需要
                            {
                                int iPaddingOffset = 0;
                                int iPaddingLength = 0;
                                int iParserStart = 0;
                                //Error Correction
                                if ((data[0] & 0x80) == 0x80) //Error Correction 存在
                                {
                                    iPaddingOffset += (data[0] & 0x0f) + 1/*first byte*/;
                                    iParserStart = iPaddingOffset;
                                }
                                int c;
                                //取得Packet Length type
                                c = (data[iParserStart] & 0x60) >> 5;
                                switch (c)
                                {
                                    case 0: // does not exist
                                        break;
                                    case 1: // byte
                                        iPaddingOffset += 1;
                                        //fix length
                                        break;
                                    case 2: //word
                                        iPaddingOffset += 2;
                                        //fix length
                                        Array.Copy(BitConverter.GetBytes((ushort)mHeader.AsfHeader.MaxPacketSize), 0,data,iPaddingOffset,2);
                                        break;
                                    case 3: //dword
                                        iPaddingOffset += 4;
                                        //fix length
                                        Array.Copy(BitConverter.GetBytes(mHeader.AsfHeader.MaxPacketSize), 0, data, iPaddingOffset -2, 4);
                                        break;
                                }
                                //取得Sequence type
                                c = (data[iParserStart] & 0x06) >> 1;
                                switch (c)
                                {
                                    case 0: // does not exist
                                        break;
                                    case 1: // byte
                                        iPaddingOffset += 1;
                                        break;
                                    case 2: //word
                                        iPaddingOffset += 2;
                                        break;
                                    case 3: //dword
                                        iPaddingOffset += 4;
                                        break;
                                }
                                //取得Padding Length type
                                c = (data[iParserStart] & 0x18) >> 3;
                                switch (c)
                                {
                                    case 0: // does not exist
                                        break;
                                    case 1: // byte
                                        
                                        iPaddingLength = 1;
                                        break;
                                    case 2: //word
                                        
                                        iPaddingLength = 2;
                                        break;
                                    case 3: //dword
                                        
                                        iPaddingLength = 4;
                                        break;
                                }
                                int iPaddings = (int)(mHeader.AsfHeader.MaxPacketSize - data.Length);
                                iPaddingOffset += 2;//skip payload parser information
                                switch (iPaddingLength)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        data[iPaddingOffset] = (byte)iPaddings;
                                        break;
                                    case 2:
                                        Array.Copy(BitConverter.GetBytes((ushort)iPaddings), 0, data, iPaddingOffset, iPaddingLength);
                                        break;
                                    case 3:
                                        Array.Copy(BitConverter.GetBytes((uint)iPaddings), 0, data, iPaddingOffset, iPaddingLength);
                                        break;
                                }
                                chunk.Data = new byte[mHeader.AsfHeader.MaxPacketSize];
                                //chunk.Data.Initialize();
                                 Array.Copy(data, chunk.Data, data.Length);
                                 chunk.Length = (ushort)chunk.Data.Length;
                            }
                            else
                            {
                                chunk.Data = data;
                            }

                            mBody.AsfChunks.Add(chunk);
                            if (mBody.AsfChunks.Count >= mHeader.AsfHeader.MaxBitrate /8 / mHeader.AsfHeader.MaxPacketSize )
                            {
                                mBody.SequenceID = GetNextSequenceID();
                                mBody.AsfHeader = mHeader.AsfHeader;
                                mBody.FileID = mHeader.FileID;

                                OnGetOneSecondData(new MediaDataArgs(mBody));

                                mBody = new MediaBody();
                               
                                //mBody.SequenceID = GetNextSequenceID();
                                
                            }
                            iCounter++;                
                            break;
                    }
                }
            }
            catch (EndOfStreamException)
            {
                return;
            }
        }
    }
}
