using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Collections.Concurrent;
using System.Security.Permissions;

namespace EPC232
{
    class ipClient : IDisposable
    {
        public delegate void OnNewLogMsg(object sender, LogMsg e);
        public event OnNewLogMsg NewLogMsg;

        private ConcurrentQueue<CmndMsg> msgStream;

        private Thread _cmdThread;
        private string _hostName;
        private int _hostPort;


        private bool _isDisposed;
        private bool _isConnected;

        public ipClient(string host, int port)
        {
            this._isDisposed = false;
            this._hostName = host;
            this._hostPort = port;
            msgStream = new ConcurrentQueue<CmndMsg>();
            Send_ConnectionState("Initial", false);
            _cmdThread = new Thread(new ThreadStart(ClientTask));
            _cmdThread.Start();
        }

        ~ipClient()
        {
            Dispose(true);
        }

        #region Communication In and out of thread
        public bool QueueMsg(CmndMsg msg)
        {
            if (this._isConnected != true) return false;
            msgStream.Enqueue(msg);
            return true;
        }

        private void Send_ConnectionState(string answer, Boolean connectionState)
        {
            this._isConnected = connectionState;
            var tps = new TimeSpan(1);
            var msg = this._isConnected == true ? "X?=DA" : "X?=NE";
            SendNewLogMsg(tps, msg, answer);
        }
        private void SendNewLogMsg(TimeSpan tsp, string command, string answer)
        {
            if (NewLogMsg == null) return;
            var d = new LogMsg
            {
                Time = tsp,
                Cmd = command,
                Answer = answer.Replace('\n', ' ')
            };
            NewLogMsg(this, d);
        }

        //private TcpClient client;
        //private NetworkStream clientStream;
        private void ClientTask()
        {
            

            while (true)
            {
                TcpClient client;
                NetworkStream clientStream;

                try
                {
                    client = new TcpClient
                    {
                        ReceiveTimeout = 10000,
                        SendTimeout = 10000
                    };
                    client.Connect(_hostName, _hostPort);
                    clientStream = client.GetStream();
                    Send_ConnectionState("ThreadStart", true);
                    while (true)
                    {
                        while (msgStream.IsEmpty)
                        {
                            Thread.Sleep(10);
                        }

                        if (this._isConnected == true)
                        {
                            CmndMsg msg = new CmndMsg();
                            if (msgStream.TryDequeue(out msg))
                            {
                                long tFrom;
                                Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg.Cmd + "\r\n\0");
                                tFrom = DateTime.Now.Ticks;

                                clientStream.Write(data, 0, data.Length);
                                Thread.Sleep(1);

                                var responseData = new StringBuilder();
                                do
                                {
                                    var numberOfBytesRead = clientStream.Read(data, 0, data.Length);
                                    responseData.AppendFormat("{0}", Encoding.ASCII.GetString(data, 0, numberOfBytesRead));
                                }
                                while (clientStream.DataAvailable);
                                var tps = TimeSpan.FromTicks(DateTime.Now.Ticks - tFrom);
                                SendNewLogMsg(tps, msg.Cmd, responseData.ToString());
                            }
                        }

                    }
                }
                catch (ArgumentNullException e)
                {
                    Send_ConnectionState(e.Message.ToString(), false);
                    client = null;
                    clientStream = null;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Send_ConnectionState(e.Message.ToString(), false);
                    client = null;
                    clientStream = null;
                }
                catch (SocketException e)
                {
                    Send_ConnectionState(e.Message.ToString(), false);
                    client = null;
                    clientStream = null;
                }
                catch (IOException e)
                {
                    Send_ConnectionState(e.Message.ToString(), false);
                    client = null;
                    clientStream = null;
                }
                catch (ObjectDisposedException e)
                {
                    Send_ConnectionState(e.Message.ToString(), false);
                    client = null;
                    clientStream = null;
                }
                catch (Exception e)
                {
                    Send_ConnectionState(e.Message.ToString(), false);
                    client = null;
                    clientStream = null;
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }
        #endregion

        #region Dispose Functions
        private void Dispose(bool disposing)
        {
            if (_isDisposed || !disposing) return;
            DisposeThreads();
            _isDisposed = true;
        }
        public void Dispose()
        {
            _isDisposed = true;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public void DisposeThreads()
        {
            _cmdThread?.Abort();
        }
        #endregion
    }

}
