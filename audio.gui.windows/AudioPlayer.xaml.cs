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
using EPC232.button.dependant.ops;
using EPC232.communication;
using EPC232.model;

namespace EPC232.audio.gui.windows
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class AudioPlayer : Window
    {

        /******************************************************************************
         * Object instances
        ******************************************************************************/

        GameStatusRefresh gsr = new GameStatusRefresh();
        CommsParseMessage cpm = new CommsParseMessage();
        ValueChecks checks = new ValueChecks();
        ValueStorage storage = new ValueStorage();
        ValueLEDs leds = new ValueLEDs();

        AnswersSerial answersSerial = new AnswersSerial();
        AnswerSerialVSActions asva = new AnswerSerialVSActions();
        //TimerClass tc = new TimerClass();
        ButtonCommandClass bcc = new ButtonCommandClass();

        AudioButtonOps abo = new AudioButtonOps();

        private readonly Brush brushMouseHover = Brushes.RoyalBlue;
        private Brush brushItemDisabled = Brushes.Gray;

        public AudioPlayer()
        {
            InitializeComponent();

            ///////////////////////////////////////////////////////////////////////////
            // Constructor inits

            abo.setCpm(cpm);
            abo.setStorage(storage);

            asva.setChecks(checks);
            asva.setLeds(leds);
            asva.setStorage(storage);

            answersSerial.setAsva(asva);
            answersSerial.setGsr(gsr);
            answersSerial.setLeds(leds);
            answersSerial.setStorage(storage);

            bcc.setCpm(cpm);
            bcc.setGsr(gsr);
            bcc.setChecks(checks);
            bcc.setStorage(storage);

            cpm.setLeds(leds);
            cpm.setStorage(storage);

            gsr.setCpm(cpm);
        }

        private void I_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            char name = btn.Name[1];
            //inputs_buttons_color(name);
            abo.buttons_inputs_command(btn.Name);
            player_com_text_change(name);
        }

        private void C_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            char name = btn.Name[1];
            player_com_text_change(name);
        }

        private void MI_Click(object sender, RoutedEventArgs e)
        {
            MenuItem btn1 = (MenuItem)sender;
            string btnName = btn1.Name.Substring(3);
            char name = btnName[1];
            //inputs_buttons_color(name);
            abo.buttons_inputs_command(btnName);
            player_com_text_change(name);
        }

        #region Player text change
        public void player_com_text_change(char name)
        {
            switch (name)
            {
                case '1':
                    COMPORT.Text = "COM1";
                    break;
                case '2':
                    COMPORT.Text = "COM2";
                    break;
                case '3':
                    COMPORT.Text = "COM3";
                    break;
                case '4':
                    //COMPORT.Text = "    ";
                    break;
                default:
                    break;
            }
        }
        #endregion

        //PLAYER
        public void P_Click_fire(string btnName)
        {
            if (COMPORT.Text.Length < 3)
            {
                return;
            }

            switch (COMPORT.Text[3])
            {
                case '1':
                    abo.buttons_player_commands(1, btnName);
                    break;
                case '2':
                    abo.buttons_player_commands(2, btnName);
                    break;
                case '3':
                    abo.buttons_player_commands(3, btnName);
                    break;
                default:
                    break;
            }
            //buttonClickRefreshers();
        }
        private void P_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            P_Click_fire(btn.Name);
            //buttonClickRefreshers();
        }
        private void MP_Click(object sender, RoutedEventArgs e)
        {
            if (COMPORT.Text.Length < 3)
            {
                return;
            }
            MenuItem btn = (MenuItem)sender;
            switch (COMPORT.Text[3])
            {
                case '1':
                    abo.buttons_player_commands(1, btn.Name.Substring(3));
                    break;
                case '2':
                    abo.buttons_player_commands(2, btn.Name.Substring(3));
                    break;
                case '3':
                    abo.buttons_player_commands(3, btn.Name.Substring(3));
                    break;
                default:
                    break;
            }
            //buttonClickRefreshers();
        }

    }
}
