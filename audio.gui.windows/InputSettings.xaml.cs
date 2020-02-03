using EPC232.communication;
using EPC232.game.elements;
using EPC232.button.dependant.ops;
using EPC232.model;
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

namespace EPC232.audio.gui.windows
{
    /// <summary>
    /// Interaction logic for InputSettings.xaml
    /// </summary>
    public partial class InputSettings : Window
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
        TimerClass tc = new TimerClass();
        ButtonCommandClass bcc = new ButtonCommandClass();

        AudioPlayer pl = new AudioPlayer();

        AudioButtonOps abo = new AudioButtonOps();

        private readonly Brush brushMouseHover = Brushes.RoyalBlue;
        private Brush brushItemDisabled = Brushes.Gray;

        public InputSettings()
        {
            InitializeComponent();

            ///////////////////////////////////////////////////////////////////////////
            // Constructor inits

            abo.setCpm(cpm);
            abo.setStorage(storage);
            abo.setTimerClass(tc);

            asva.setChecks(checks);
            asva.setLeds(leds);
            asva.setStorage(storage);

            answersSerial.setAsva(asva);
            answersSerial.setGsr(gsr);
            answersSerial.setLeds(leds);
            answersSerial.setStorage(storage);

            bcc.setCpm(cpm);
            bcc.setGsr(gsr);
            bcc.setTc(tc);
            bcc.setChecks(checks);
            bcc.setStorage(storage);

            cpm.setLeds(leds);
            cpm.setStorage(storage);

            gsr.setCpm(cpm);

            tc.setCpm(cpm);
            tc.setGsr(gsr);
            tc.setChecks(checks);
            tc.setStorage(storage);

        }


        // control Slider From Menu
        #region Slider controls - value changing via mouse control
        private void MI_SL_UP_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            switch (mi.Name)
            {
                case "MS_VOL_UP":
                    VOL.Value++;
                    break;
                case "MI_MVOL_UP":
                    MVOL.Value++;
                    break;
                default:
                    break;
            }

        }
        private void MI_SL_DN_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            switch (mi.Name)
            {
                case "MS_VOL_DN":
                    VOL.Value--;
                    break;
                case "MI_MVOL_DN":
                    MVOL.Value--;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Slider value change - visual
        private void AS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sd = (Slider)sender;
            int value = (int)sd.Value;
            Silder_text_change(sd.Name, value);
            abo.sliders_command(sd.Name, value);
            if (storage._settingsIndex == storage._inputIndex)
            {
                if (sd.Name == "VOL")
                {
                    Slider_value_change("MVOL", 13 - value);
                }
            }
        }
        private void MVOL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sd = (Slider)sender;
            int value = (int)sd.Value;
            Silder_text_change(sd.Name, value);
            abo.sliders_command(sd.Name, value);
            if (storage._settingsIndex == storage._inputIndex)
            {
                if (sd.Name == "MVOL")
                {
                    Slider_value_change("VOL", 13 - value);
                }
            }

        }
        #endregion

        // Gong checkbox
        #region Gong Checkbox
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Properties.Settings.Default.IsGongEnabled = (bool)cb.IsChecked;
            Properties.Settings.Default.Save();
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Properties.Settings.Default.IsGongEnabled = (bool)cb.IsChecked;
            Properties.Settings.Default.Save();
        }
        #endregion

        private void ContextMenu_visibility_toggle(ref ContextMenu m, ref Grid g)
        {
            m.Placement = System.Windows.Controls.Primitives.PlacementMode.Relative;
            m.PlacementTarget = g;
            m.IsOpen = true;
            m.Visibility = System.Windows.Visibility.Visible;
        }

        //settings
        private void S_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            settings_buttons_color(btn.Name[1]);
            abo.buttons_settings_command(btn.Name);
        }
        private void MS_Click(object sender, RoutedEventArgs e)
        {
            MenuItem btn1 = (MenuItem)sender;
            string btnName = btn1.Name.Substring(3);
            char name = btnName[1];

            settings_buttons_color(btnName[1]);
            abo.buttons_settings_command(btnName);
        }

        //INPUT
        private void I_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            char name = btn.Name[1];
            inputs_buttons_color(name);
            abo.buttons_inputs_command(btn.Name);
        }

        private void C_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            char name = btn.Name[1];
            pl.player_com_text_change(name);
        }

        private void MI_Click(object sender, RoutedEventArgs e)
        {
            MenuItem btn1 = (MenuItem)sender;
            string btnName = btn1.Name.Substring(3);
            char name = btnName[1];
            inputs_buttons_color(name);
            abo.buttons_inputs_command(btnName);
            pl.player_com_text_change(name);
        }
        private void IMUTE_Click(object sender, RoutedEventArgs e)
        {
            switch (storage._inputIndex)
            {
                case '1':
                    cpm.SendMsg("IV=1-14", 0);
                    break;
                case '2':
                    cpm.SendMsg("IV=2-14", 0);
                    break;
                case '3':
                    cpm.SendMsg("IV=3-14", 0);
                    break;
                case '4':
                    cpm.SendMsg("IV=4-14", 0);
                    cpm.SendMsg("IV=5-14", 0);
                    break;
            }
            Slider_value_change("VOL", 14);
            Slider_value_change("MVOL", 14);
        }

        #region Button & slider color and visibility options

        #region Slider visibility
        private void settings_sliders_show_hide(char name)
        {
            switch (name)
            {
                case '1':
                case '2':
                case '3':
                    SVOL.Visibility = System.Windows.Visibility.Visible;
                    SGAIN.Visibility = System.Windows.Visibility.Visible;
                    SBASS.Visibility = System.Windows.Visibility.Visible;
                    STREB.Visibility = System.Windows.Visibility.Visible;
                    SATTR.Visibility = System.Windows.Visibility.Visible;
                    SATTL.Visibility = System.Windows.Visibility.Visible;
                    break;
                case '4':
                    SVOL.Visibility = System.Windows.Visibility.Visible;
                    SGAIN.Visibility = System.Windows.Visibility.Visible;
                    SBASS.Visibility = System.Windows.Visibility.Visible;
                    STREB.Visibility = System.Windows.Visibility.Visible;
                    SATTR.Visibility = System.Windows.Visibility.Visible;
                    SATTL.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case '5':
                    SVOL.Visibility = System.Windows.Visibility.Visible;
                    SGAIN.Visibility = System.Windows.Visibility.Visible;
                    SBASS.Visibility = System.Windows.Visibility.Visible;
                    STREB.Visibility = System.Windows.Visibility.Visible;
                    SATTR.Visibility = System.Windows.Visibility.Hidden;
                    SATTL.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 'H':
                    SVOL.Visibility = System.Windows.Visibility.Hidden;
                    SGAIN.Visibility = System.Windows.Visibility.Hidden;
                    SBASS.Visibility = System.Windows.Visibility.Hidden;
                    STREB.Visibility = System.Windows.Visibility.Hidden;
                    SATTR.Visibility = System.Windows.Visibility.Hidden;
                    SATTL.Visibility = System.Windows.Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Settings buttons color
        private void settings_buttons_color(char name)
        {
            switch (name)
            {
                case '1':
                    S1.Background = brushMouseHover;
                    S2.Background = Brushes.White;
                    S3.Background = Brushes.White;
                    S4.Background = Brushes.White;
                    S5.Background = Brushes.White;
                    storage._settingsIndex = '1';
                    break;
                case '2':
                    S1.Background = Brushes.White;
                    S2.Background = brushMouseHover;
                    S3.Background = Brushes.White;
                    S4.Background = Brushes.White;
                    S5.Background = Brushes.White;
                    storage._settingsIndex = '2';
                    break;
                case '3':
                    S1.Background = Brushes.White;
                    S2.Background = Brushes.White;
                    S3.Background = brushMouseHover;
                    S4.Background = Brushes.White;
                    S5.Background = Brushes.White;
                    storage._settingsIndex = '3';
                    break;
                case '4':
                    S1.Background = Brushes.White;
                    S2.Background = Brushes.White;
                    S3.Background = Brushes.White;
                    S4.Background = brushMouseHover;
                    S5.Background = Brushes.White;
                    storage._settingsIndex = '4';
                    break;
                case '5':
                    S1.Background = Brushes.White;
                    S2.Background = Brushes.White;
                    S3.Background = Brushes.White;
                    S4.Background = Brushes.White;
                    S5.Background = brushMouseHover;
                    storage._settingsIndex = '5';
                    break;
                case 'H':
                    S1.Background = Brushes.White;
                    S2.Background = Brushes.White;
                    S3.Background = Brushes.White;
                    S4.Background = Brushes.White;
                    S5.Background = Brushes.White;
                    storage._settingsIndex = 'H';
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Input button color
        private void inputs_buttons_color(char name)
        {
            switch (name)
            {
                case '1':
                    I1.Background = brushMouseHover;
                    I2.Background = Brushes.White;
                    I3.Background = Brushes.White;
                    I4.Background = Brushes.White;
                    storage._inputIndex = '1';
                    break;
                case '2':
                    I1.Background = Brushes.White;
                    I2.Background = brushMouseHover;
                    I3.Background = Brushes.White;
                    I4.Background = Brushes.White;
                    storage._inputIndex = '2';
                    break;
                case '3':
                    I1.Background = Brushes.White;
                    I2.Background = Brushes.White;
                    I3.Background = brushMouseHover;
                    I4.Background = Brushes.White;
                    storage._inputIndex = '3';
                    break;
                case '4':
                    I1.Background = Brushes.White;
                    I2.Background = Brushes.White;
                    I3.Background = Brushes.White;
                    I4.Background = brushMouseHover;
                    storage._inputIndex = '4';
                    break;
                case 'H':
                    I1.Background = Brushes.White;
                    I2.Background = Brushes.White;
                    I3.Background = Brushes.White;
                    I4.Background = Brushes.White;
                    storage._inputIndex = 'H';
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Slider text change
        private void Silder_text_change(string sliderName, int value)
        {
            switch (sliderName)
            {
                case "GAIN":
                    TGAIN.Text = TdadBText.Gain[value];
                    break;
                case "VOL":
                    TVOL.Text = TdadBText.Volume[13 - value];
                    break;
                case "BASS":
                    TBASS.Text = TdadBText.Tone[value];
                    break;
                case "TREB":
                    TTREB.Text = TdadBText.Tone[value];
                    break;
                case "ATTL":
                    TATTL.Text = TdadBText.Attenuation[17 - value];
                    break;
                case "ATTR":
                    TATTR.Text = TdadBText.Attenuation[17 - value];
                    break;
                case "MVOL":
                    TMVOL.Text = TdadBText.Volume[13 - value];
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Slider values - transition FX
        private void Slider_value_change(string sliderName, int value)
        {

            switch (sliderName)
            {
                case "GAIN":
                    GAIN.ValueChanged -= AS_ValueChanged;
                    GAIN.Value = value;
                    GAIN.ValueChanged += AS_ValueChanged;
                    Silder_text_change(sliderName, value);
                    break;
                case "VOL":
                    VOL.ValueChanged -= AS_ValueChanged;
                    VOL.Value = 13 - value;
                    VOL.ValueChanged += AS_ValueChanged;
                    Silder_text_change(sliderName, (int)VOL.Value);
                    break;
                case "BASS":
                    BASS.ValueChanged -= AS_ValueChanged;
                    BASS.Value = value;
                    BASS.ValueChanged += AS_ValueChanged;
                    Silder_text_change(sliderName, value);
                    break;
                case "TREB":
                    TREB.ValueChanged -= AS_ValueChanged;
                    TREB.Value = value;
                    TREB.ValueChanged += AS_ValueChanged;
                    Silder_text_change(sliderName, value);
                    break;
                case "ATTL":
                    ATTL.ValueChanged -= AS_ValueChanged;
                    ATTL.Value = 17 - value;
                    ATTL.ValueChanged += AS_ValueChanged;
                    Silder_text_change(sliderName, (int)ATTL.Value);
                    break;
                case "ATTR":
                    ATTR.ValueChanged -= AS_ValueChanged;
                    ATTR.Value = 17 - value;
                    ATTR.ValueChanged += AS_ValueChanged;
                    Silder_text_change(sliderName, (int)ATTR.Value);
                    break;
                case "MVOL":
                    MVOL.ValueChanged -= MVOL_ValueChanged;
                    MVOL.Value = 13 - value;
                    MVOL.ValueChanged += MVOL_ValueChanged;
                    Silder_text_change(sliderName, (int)MVOL.Value);
                    break;
                default:
                    break;
            }
        }
        #endregion

        // End entire region
        #endregion

    }
}
