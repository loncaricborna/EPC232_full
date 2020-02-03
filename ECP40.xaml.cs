using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.ComponentModel;
using System.Windows.Threading;

using System.Windows.Media.Animation;

namespace EPC232
{
    /// <summary>
    /// Interaction logic for ECP40.xaml
    /// </summary>
    public partial class ECP40 : Window
    {
        private ipClient client;

        public ECP40(string host, int port)
        {
            InitializeComponent();
            client = new ipClient(host, port);
            client.NewLogMsg += client_NewLogMsg;
            tmr_Run();
            Connection.Focusable = true;
            Connection.Focus();

        }

        private void client_NewLogMsg(object sender, LogMsg e)
        {
            var da = new LogData
            {
                Time = e.Time,
                Cmd = e.Cmd,
                Answer = e.Answer
            };

            void Action()
            {
                parse_answer(da);
            }
            Main.Dispatcher.BeginInvoke((Action)Action);
        }
        private void parse_answer(LogData da)
        {
            try
            {
                if (!da.Cmd[1].Equals('?')) return;
                switch (da.Cmd[0])
                {
                    case 'X':
                        parse_answer_tcpclient(da.Cmd);
                        break;
                    case 'K':
                        answer_populate_keys(da.Answer);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                //ignor
            }


        }
        private void parse_answer_tcpclient(string cmd)
        {
            if (cmd.Substring(3, 2) == "DA")
            {
                clntErr = false;
                Ellipse_Fill(Connection, '1');
            }
            else
            {
                clntErr = true;
                Ellipse_Fill(Connection, '0');
            }
        }
        private void answer_populate_keys(string cmd)
        {
            var ch = cmd.ToCharArray();
            button_setFlash(K00, ch[0 + 3]);
            button_setFlash(K01, ch[1 + 3]);
            button_setFlash(K02, ch[2 + 3]);
            button_setFlash(K03, ch[3 + 3]);

            button_setFlash(K05, ch[5 + 3]);
            button_setFlash(K06, ch[6 + 3]);
            button_setFlash(K07, ch[7 + 3]);
            button_setFlash(K08, ch[8 + 3]);
            button_setFlash(K09, ch[9 + 3]);
            button_setFlash(K10, ch[10 + 3]);

            button_setFlash(K12, ch[12 + 3]);
            button_setFlash(K13, ch[13 + 3]);
            button_setFlash(K14, ch[14 + 3]);
            button_setFlash(K15, ch[15 + 3]);


            button_setFlash(K16, ch[16 + 3]);
            button_setFlash(K17, ch[17 + 3]);
            button_setFlash(K18, ch[18 + 3]);
            button_setFlash(K19, ch[19 + 3]);

            button_setFlash(K28, ch[28 + 3]);
            button_setFlash(K29, ch[29 + 3]);
            button_setFlash(K30, ch[30 + 3]);
            button_setFlash(K31, ch[31 + 3]);

            button_setFlash(K32, ch[32 + 3]);//PLC communication

            if (ch[33 + 3] == '1') // plc
            {
                button_setFlash(KXX, '0');
                button_setFlash(K24, '1');
                button_setFlash(K23, '0');
            }
            else if (ch[33 + 3] == '2') //rucno
            {
                button_setFlash(KXX, '0');
                button_setFlash(K24, '0');
                button_setFlash(K23, '1');
            }
            else // sve off
            {
                button_setFlash(KXX, '1');
                button_setFlash(K24, '0');
                button_setFlash(K23, '0');
            }
        }

        private void button_setFlash(Button btn, char ch)
        {
            var str = ch.ToString();

            if (!int.TryParse(str, out var val)) return;
            var duration = -1;
            switch (val)
            {
                case 0:
                    duration = 0;
                    break;
                case 1:
                    duration = 1000;
                    break;
                case 2:
                    duration = 500;
                    break;
                case 3:
                    duration = 100;
                    break;
                default:
                    break;
            }
            if (duration == 0)
            {
                btn.Background = new SolidColorBrush(Colors.Black);
            }
            else if (duration >= 1000)
            {
                btn.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                var ca = new ColorAnimation();
                ca = new ColorAnimation
                {
                    From = Colors.Red,
                    To = Colors.Black,
                    Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
                    RepeatBehavior = RepeatBehavior.Forever,
                    AutoReverse = true
                };
                btn.Background = new SolidColorBrush(Colors.Red);
                btn.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }
        }


        private void G_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            button_setFlash(btn, '1');
            var btnName = "KI=" + btn.Name.Substring(1);
            SendMsg(btnName, 0);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Dispose();
                client.DisposeThreads();
            }
            client = null;
            tmr_Stop();
        }


        #region Periodic timer for Sending
        private DispatcherTimer tmr;
        private void tmr_Stop()
        {
            if (tmr != null)
            {
                tmr.Stop();
                tmr.Tick -= tmr_Tick;
            }
        }
        private void tmr_Run()
        {
            tmr = new DispatcherTimer();
            tmr.Interval = new TimeSpan(0, 0, 0, 2, 0);
            tmr.Tick += tmr_Tick;
            tmr.Start();
        }
        bool clntErr = false;
        private void tmr_Tick(object sender, EventArgs e)
        {
            if (clntErr == false)
            {
                SendMsg("K?=", 0);
            }
        }

        #endregion

        private void Ellipse_Fill(Ellipse eps, char flag)
        {
            eps.Fill = flag == '1' ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
        }
        private void SendMsg(string cmd, int delay)
        {
            var msg = new CmndMsg
            {
                DelayAfter = delay,
                Cmd = cmd
            };
            if (client.QueueMsg(msg) == false)
            {
                Ellipse_Fill(Connection, '0');
            }
        }

    }
}
