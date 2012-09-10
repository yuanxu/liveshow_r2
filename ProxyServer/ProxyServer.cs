using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using LiveShow.Message;
using System.Configuration;

namespace LiveShow
{
    
    class ProxyServer
    {
        public ProxyServer()
        {
            m_Peers = new Dictionary<uint, IDictionary<int,UdpCmd>>();
            
        }

        /// <summary>
        /// 保存所有到此服务器登记的客户端信息
        /// </summary>
        private IDictionary<uint, IDictionary<int,UdpCmd>> m_Peers;
        private bool m_ShowEcho = true;
        private bool m_IsRunning = false;

        public bool ShowEcho
        {
            get { return m_ShowEcho; }
            set { m_ShowEcho = value; }
        }
        public void Start()
        {

            UdpClient udp = new UdpClient(int.Parse(ConfigurationManager.AppSettings["port"]));//
            m_IsRunning = true;
            udp.BeginReceive(new AsyncCallback(ReceiveCall), udp);
            if(m_ShowEcho)
                System.Console.WriteLine("Proxy Server is Running.");
            while (m_IsRunning)
            {
                System.Threading.Thread.Sleep(10);
            }
        }
        
        private void ReceiveCall(IAsyncResult result)
        {
            UdpClient udp = (UdpClient)result.AsyncState;
            StateObject stateObject = new StateObject();
            try
            {
                stateObject.buffer = udp.EndReceive(result, ref  stateObject.remoteEP);
                stateObject.udp = udp;
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ProcessCall), stateObject);
            }catch(SocketException e)
            {
                if(e.ErrorCode == 10054)
                {
                }
            }
            udp.BeginReceive(new AsyncCallback(ReceiveCall), udp);
        }
        private void SendCall(IAsyncResult result)
        {
            UdpClient udp = (UdpClient)result.AsyncState;
            udp.EndSend(result);
        }
        private uint LowID = 0x1000000;
        private void ProcessCall(object state)
        {
            StateObject stateObject = (StateObject)state;
            ProxyCommand Cmd =(ProxyCommand) BitConverter.ToUInt32(stateObject.buffer, 0);
            if (m_ShowEcho)
            {
                System.Console.WriteLine(string.Format("Get Command:{0} IP:{1} @ {2}",Cmd.ToString(),stateObject.remoteEP.ToString(),DateTime.Now));
            }
            switch (Cmd)
            {
                case ProxyCommand.Connect:
                    {
                        UdpConnect req = new UdpConnect();
                        req.FromBytes(stateObject.buffer);
                        UdpConnectR resp = new UdpConnectR();
                        byte[] sremotebyts=  stateObject.remoteEP.Address.GetAddressBytes();
                        Array.Reverse(sremotebyts);
                        uint remoteIp = BitConverter.ToUInt32(sremotebyts, 0);
                        
                        foreach (uint ip in req.Ips)
                        {
                            if(ip ==remoteIp)
                            {
                                //Public Node
                                resp.peer_id_ = ip;
                                resp.RemoteIp = ip;
                                break;
                            }
                      
                        }
                        //private node
                        if (resp.peer_id_ == 0)
                        {
                            resp.RemoteIp =BitConverter.ToUInt32( stateObject.remoteEP.Address.GetAddressBytes(),0);
                            resp.peer_id_ = LowID--;
                        }
                        resp.ToBytes();
                        stateObject.udp.BeginSend(resp.Buffer, resp.Buffer.Length, stateObject.remoteEP, new AsyncCallback(SendCall), stateObject.udp);
                    }
                    break;
                case ProxyCommand.Hello:
                    {
                        UdpHelloReq req = new UdpHelloReq();
                        req.FromBytes(stateObject.buffer);

                        UdpHelloResp resp = new UdpHelloResp();
                        resp.SeqID = req.SeqID;
                        lock (m_Peers)
                        {
                            if (!m_Peers.ContainsKey(req.PeerId))
                            {
                                m_Peers.Add(req.PeerId, new Dictionary<int,UdpCmd>());
                            }
                            
                        }
                        resp.Cmds = m_Peers[req.PeerId];
                        resp.PeerId = req.PeerId;
                        
                        resp.ToBytes();
                        m_Peers[req.PeerId].Clear();

                        stateObject.udp.BeginSend(resp.Buffer, resp.Buffer.Length, stateObject.remoteEP, new AsyncCallback(SendCall), stateObject.udp);
                    }
                    break;
                case ProxyCommand.Callback:
                    {
                        UdpCallBackReq req = new UdpCallBackReq();
                        req.FromBytes(stateObject.buffer);
                        lock (m_Peers)
                        {
                            if (m_Peers.ContainsKey(req.PeerId))
                            {
                                string key = req.Body.PeerID.ToString() + req.Body.ChId.ToString() + req.Body.Cmd.ToString();
                                if(!m_Peers[req.PeerId].ContainsKey(key.GetHashCode()))
                                    m_Peers[req.PeerId].Add(key.GetHashCode(),req.Body);
                            }
                        }
                    }
                    break;
                case ProxyCommand.Leave:
                    break;

            }
            
        }
        public void Stop()
        {
            m_IsRunning = false;
        }
    }

    class StateObject
    {
        public UdpClient udp;
        public IPEndPoint remoteEP;
        public byte[] buffer;
    }
}
