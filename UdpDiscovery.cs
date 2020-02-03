using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Threading;


namespace EPC232
{

    public class UdpClinetMsg : System.EventArgs
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public string MasterPort { get; set; }
    }

    public class UdpDiscovery
    {
        public delegate void OnNewLogMsg(object sender, UdpClinetMsg e);
        public event OnNewLogMsg NewLogMsg;

        public struct UdpState
        {
            public System.Net.IPEndPoint Ep;
            public System.Net.Sockets.UdpClient UdpClient;
        }
        private UdpState _globalUdp;

        public UdpDiscovery()
        {
            try
            {
                _globalUdp.UdpClient = new UdpClient();
                _globalUdp.Ep = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("255.255.255.255"), 30303);
                var bindEp = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 30303);
                var discoverMsg = Encoding.ASCII.GetBytes("Discovery: Gdje ste?");

                _globalUdp.UdpClient.Client.Bind(bindEp);
                _globalUdp.UdpClient.EnableBroadcast = true;
                _globalUdp.UdpClient.MulticastLoopback = false;

                _globalUdp.UdpClient.BeginReceive(ReceiveCallback, _globalUdp);
                _globalUdp.UdpClient.Send(discoverMsg, discoverMsg.Length, new System.Net.IPEndPoint(System.Net.IPAddress.Parse("255.255.255.255"), 30303));

                var discoverTimer = new System.Windows.Threading.DispatcherTimer();
                discoverTimer.Tick += new EventHandler(discoverTimer_Tick);
                discoverTimer.Interval = new TimeSpan(0, 0, 5);
                discoverTimer.Start();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void discoverTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                var discoverMsg = Encoding.ASCII.GetBytes("Discovery: Gdje ste?");
                _globalUdp.UdpClient.Send(discoverMsg, discoverMsg.Length, new System.Net.IPEndPoint(System.Net.IPAddress.Parse("255.255.255.255"), 30303));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var myUdp = (UdpState)ar.AsyncState;
                var receiveString = Encoding.ASCII.GetString(myUdp.UdpClient.EndReceive(ar, ref myUdp.Ep));
                receiveString = myUdp.Ep.Address.ToString() + "\n" + receiveString.Replace("\r\n", "\n");
                myUdp.UdpClient.BeginReceive(ReceiveCallback, myUdp);
                var strsplits = receiveString.Split('\n');
                if (strsplits.Length != 5) return;
                if (!strsplits[1].Contains("EPC232")) return;
                var ds = new UdpClinetMsg
                {
                    IpAddress = strsplits[0],
                    Name = strsplits[1],
                    MacAddress = strsplits[2],
                    MasterPort = strsplits[3]
                };
                if (NewLogMsg == null) return;
                NewLogMsg(this, ds);
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }
}
