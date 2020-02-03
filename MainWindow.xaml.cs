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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Deployment;
using System.IO;
using System.Timers;
using System.ComponentModel;
using System.Windows.Threading;
using EPC232.communication;
using EPC232.model;
using EPC232.game.elements;
using EPC232.button.dependant.ops;
using EPC232.audio.gui.windows;
using System.Threading;

/*******************************************************************************
 * 
 * File: MainWindow.xaml.cs
 * 
 * Authors: Borna Loncaric
 * 
 * Device name: EPC232
 * Ver: Full
 * 
 * Description:
 * 
 * Main klasa - treba sadrzavati samo one metode koje su potrebne za neposredne
 * izmjene prikaznih prozora i adaptere/kompozitne pristupne klase
 * 
 ******************************************************************************/
/*****************************************************************************
 * 
 * Change log:
 * 
 * 28.8.2019.
 * -   Dodana export funkcionalnost, nakon toga program se moze buildati,
 *     no nece raditi. Konstantno izbacuje "NullObjectReference" internal 
 *     e-rror.
 *     
 * 29.8.2019.
 * -   Testiranje funkcionalnosti export metode; problem ocito nije u klasi
 *     'ListExportDialogueBox.xaml' jer se i bez nje dogadja isti problem.
 * -   Slutim da je problem u referenciranju i prosljedjivanju
 *     ObservableCollection tipa varijable --> TOCNO
 * -   izmjenjen 'folderBrowserDialog' u 'saveFileDialog', potrebniju i
 *     ispravniju verziju
 *     
 * 30.8.2019.
 * -   Prerade na path varijabli u 'ListExportDialogueBox.xaml'
 * -   Izmjenjena vrsta plInfoHome i Guests varijabli u ValuePlayers iz
 *     ObservableCollection<string> u string[] zbog rusenja i upozorenja
 *     'NullReferenceError'
 * 
 *****************************************************************************/

namespace EPC232
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// POTREBNO JOS NAPRAVITI
    /// 
    /// - instant refresh svih stanja pritiskom bilo kojeg gumba, bilo kojom akcijom
    /// napravljenom od strane korisnika
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Initialization variables & values
        // Class instances - NETWORKING
        private UdpDiscovery udpServerDiscoverer;
        private ipClient _client;

        string host = Properties.Settings.Default.EPCAddress;
        int port = Properties.Settings.Default.EPCPort;

        /******************************************************************************
         * Object instances
        ******************************************************************************/

        GameStatusRefresh gsr = new GameStatusRefresh();
        CommsParseMessage cpm = new CommsParseMessage();
        ValueChecks checks = new ValueChecks();
        ValueStorage storage = new ValueStorage();
        ValueLEDs leds = new ValueLEDs();
        ValuePlayers gamePlayers = new ValuePlayers();

        GamePlayersInput gpi = new GamePlayersInput();// (gamePlayers);

        AnswersSerial answersSerial = new AnswersSerial();
        AnswerSerialVSActions asva = new AnswerSerialVSActions();
        TimerClass tc = new TimerClass();
        ButtonCommandClass bcc = new ButtonCommandClass();
        AudioButtonOps abo = new AudioButtonOps();

        AudioPlayer pl = new AudioPlayer();
        //GamePlayersInput gpi = new GamePlayersInput();

        ListExportDialogueBox ledb = new ListExportDialogueBox();
        ListImportDialogueBox lidb = new ListImportDialogueBox();

        /******************************************************************************
         * Variable instances
        ******************************************************************************/

        private List<LogData> _logs;
        private int _logsCount;

        // Brush variables
        private readonly Brush brushMouseHover = Brushes.RoyalBlue;
        private Brush brushItemDisabled = Brushes.Gray;
        #endregion

        /******************************************************************************
        * Program initialization methods
        ******************************************************************************/

        #region Initialization methods (Mains)

        // Main window initialization
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Properties.Settings.Default.Reload();

                this.Title = "EPC232 - Full";// + PublishVersion;
                Version.Text = "EPC232 ver. " + PublishVersion + "\r\n" + "ELAK d.o.o. Kastav" + "\r\n" + "elak@elak.hr";
                if (EPC232.Properties.Settings.Default.ECP40IsActive == false)
                {
                    this.ecp.Visibility = Visibility.Hidden;
                }
                //GongEnable.IsChecked = (bool)Properties.Settings.Default.IsGongEnabled;
                //checks.applicationOn = true;
                //applicationCheck = checks.applicationOn;            

            } catch (Exception exc)
            {
                var msg = MessageBox.Show("Error: " + exc.StackTrace.ToString(), "Warning!", MessageBoxButton.OK);
            }


        }

        // Main window - methods run on startup
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            
            DispatcherTimer timerGuiRefresh = new DispatcherTimer();

            // Network initialization
            _client = new ipClient(host, port);
            TcpClientRun();
            udpServerDiscoverer = new UdpDiscovery();
            udpServerDiscoverer.NewLogMsg += UdpServerDiscoverer_NewLogMsg;

            ///////////////////////////////////////////////////////////////////////////
            // Constructor inits

            abo.setCpm(cpm);
            abo.setStorage(storage);
            abo.setTimerClass(tc);

            asva.setChecks(checks);
            asva.setLeds(leds);
            asva.setStorage(storage);
            asva.setGamePlayers(gamePlayers);

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
            cpm.setIpClient(_client);

            gpi.setPlayer(gamePlayers);
            gpi.setLedb(ledb);
            gpi.setLidb(lidb);

            gsr.setCpm(cpm);
            gsr.setStorage(storage);

            ledb.setValuePlayers(gamePlayers);
            lidb.setPlayers(gamePlayers);
            lidb.setGpi(gpi);

            tc.setCpm(cpm);
            tc.setGsr(gsr);
            tc.setChecks(checks);
            tc.setStorage(storage);

            ///////////////////////////////////////////////////////////////////////////    

            relayStates_get();

                // Set initial default value
            gamePlayers.allowedPPT = 12;
                // Summon lists
            gamePlayers.plInfoGuests = new string[gamePlayers.allowedPPT];
            gamePlayers.plInfoHome = new string[gamePlayers.allowedPPT];

            // Proceed to program
            commandslogs.ItemsSource = _logs;
            _logsCount = 0;
            checks.VSTimerStarted = false;
            tc.OnShotTimer = null;
            tc.timer_Run();
            LEDConnection.Focusable = true;
            LEDConnection.Focus();
            //gameEnteredColorChange();
            elipseFillCheck();

            GTIMEOUT.Content = "TIME" + "\n" + "OUT";
            HTIMEOUT.Content = "TIME" + "\n" + "OUT";
            START.Content = "START/" + "\n" + "STOP";

            // Timer dispatcher thread
            timerGuiRefresh.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timerGuiRefresh.Tick += TimerTick;
            timerGuiRefresh.Start();

        }

        private string PublishVersion
        {
            get
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    var ver = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                    return $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
                }
                else
                    return "--";
            }
        }

        #endregion

        /******************************************************************************
        * THREADING !!!
        ******************************************************************************/

        private void TimerTick(object sender, EventArgs e)
        {
            buttonClickRefreshers();
            //gsr.status_game_TP(); // unused
        }
  

        /******************************************************************************
        * Regular refresh methods
        ******************************************************************************/

        private void relayStates_get()
        {
            /////////////////////// DUE TO REMOVE
            //I1.Content = S1.Content = C1.Content = Properties.Settings.Default.IN1;
            //I2.Content = S2.Content = C2.Content = Properties.Settings.Default.IN2;
            //I3.Content = S3.Content = C3.Content = Properties.Settings.Default.IN3;
            //////////////////////////////////////
            R1.Content = Properties.Settings.Default.RELAY1;
            R2.Content = Properties.Settings.Default.RELAY2;
        }

        private void elipseFillCheck()
        {

                // Connection LED
            LEDConnection.Fill = leds.chkLedConnection == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);

                // Relay LED
            LedRLY1.Fill = leds.chkLedRelay1 == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
            LedRLY2.Fill = leds.chkLedRelay2 == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);

                // Timeout LED - Home
            LedHTimeout1.Fill = leds.chkLedHTimeout1 == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
            LedHTimeout2.Fill = leds.chkLedHTimeout2 == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
            LedHTimeout3.Fill = leds.chkLedHTimeout3 == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);

                // Timeout LED - Guests
            LedGTimeout1.Fill = leds.chkLedGTimeout1 == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
            LedGTimeout2.Fill = leds.chkLedGTimeout2 == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
            LedGTimeout3.Fill = leds.chkLedGTimeout3 == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);

                // LED up/down - true is up, false is down
            LedUp.Fill = leds.chkLedUpDown == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
            LedDown.Fill = leds.chkLedUpDown == false ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);

                // LED edit + start time
            LedStartTime.Fill = leds.chkLedStartTime == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
            LedEdit.Fill = leds.chkLedEdit == true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);

        }

            // Refreshes the textboxes on button click
        private void buttonClickRefreshers()
        {
            // Extern
            //gsr.status_refresh_all();

            // Local
            elipseFillCheck();

            // Textbox refreshers
            txtGameMode.Text = storage.parse_gameMode;

            txtGamePeriod.Text = storage.parse_gamePeriod;
            txtGameTime.Text = storage.parse_gameTime;
            txtAttackTime.Text = storage.parse_actionTime;
            if (storage.parse_gameMode == "BASKETBALL" || storage.parse_gameMode=="WATER POLO")
            {
                txtAttackTime.Visibility = Visibility.Visible;
            } else
            {
                txtAttackTime.Visibility = Visibility.Hidden;
            }

            txtGBonus.Text = storage.parse_bonusGuests.ToString();
            txtHBonus.Text = storage.parse_bonusHome.ToString();

            txtGGScore.Text = storage.parse_scoreGuests.ToString();
            txtGHScore.Text = storage.parse_scoreHome.ToString();

            gameEnteredLimiters();
            gameDependantSwitches();

        }

        #region Special game-related methods & functions
        // Limiters upon entering a game
        private void gameEnteredLimiters()
        {

            if (checks.gameEntered == true)
            {
                ENTER.IsEnabled = false;
                //START.IsEnabled = true;
                EDIT.IsEnabled = true;
                MINUITS.IsEnabled = true;
                UPDOWN.IsEnabled = true;
                SECONDS.IsEnabled = true;
            } else
            {
                ENTER.IsEnabled = true;
                //START.IsEnabled = false;
                EDIT.IsEnabled = false;
                MINUITS.IsEnabled = false;
                UPDOWN.IsEnabled = false;
                SECONDS.IsEnabled = false;
            }

            if (checks.MPLabTimerStarted == true)
            {
                EXIT.IsEnabled = false;
                LOAD.IsEnabled = false;
                ENTER.IsEnabled = false;
                PERIOD.IsEnabled = false;
                GTIMEOUT.IsEnabled = false;
                HTIMEOUT.IsEnabled = false;
            } else
            {
                EXIT.IsEnabled = true;
                LOAD.IsEnabled = true;
                ENTER.IsEnabled = true;
                PERIOD.IsEnabled = true;
                GTIMEOUT.IsEnabled = true;
                HTIMEOUT.IsEnabled = true;
            }

        }

            // Switches button names on sport change
        private void gameDependantSwitches()
        {

            switch(storage.parse_gameMode)
            {
                case "FOOTBALL":
                    GBONUS.IsEnabled = false;
                    HBONUS.IsEnabled = false;
                    GTIMEOUT.IsEnabled = false;
                    HTIMEOUT.IsEnabled = false;

                    GBONUS.Content = "/";
                    HBONUS.Content = "/";
                    lblPossessionGuests.Visibility = Visibility.Hidden;
                    lblPossessionHome.Visibility = Visibility.Hidden;
                    break;
                case "BASKETBALL":
                    GBONUS.IsEnabled = true;
                    HBONUS.IsEnabled = true;
                    GTIMEOUT.IsEnabled = true;
                    HTIMEOUT.IsEnabled = true;

                    GBONUS.Content = "    BALL\nPOSSESS.";      // razmaci zbog centriranja na prikazu
                    HBONUS.Content = "    BALL\nPOSSESS.";

                    checkBallPossession();
                    break;
                case "FUTSAL":
                    GBONUS.IsEnabled = true;
                    HBONUS.IsEnabled = true;
                    GTIMEOUT.IsEnabled = true;
                    HTIMEOUT.IsEnabled = true;

                    GBONUS.Content = "BONUS";
                    HBONUS.Content = "BONUS";
                    lblPossessionGuests.Visibility = Visibility.Hidden;
                    lblPossessionHome.Visibility = Visibility.Hidden;
                    break;
                case "HANDBALL":
                    GBONUS.IsEnabled = false;
                    HBONUS.IsEnabled = false;
                    GTIMEOUT.IsEnabled = true;
                    HTIMEOUT.IsEnabled = true;

                    GBONUS.Content = "/";
                    HBONUS.Content = "/";
                    lblPossessionGuests.Visibility = Visibility.Hidden;
                    lblPossessionHome.Visibility = Visibility.Hidden;
                    break;
                case "HOCKEY":
                    GBONUS.IsEnabled = false;
                    HBONUS.IsEnabled = false;
                    GTIMEOUT.IsEnabled = true;
                    HTIMEOUT.IsEnabled = true;

                    GBONUS.Content = "/";
                    HBONUS.Content = "/";
                    lblPossessionGuests.Visibility = Visibility.Hidden;
                    lblPossessionHome.Visibility = Visibility.Hidden;
                    break;
                case "WATER POLO":
                    GBONUS.IsEnabled = true;
                    HBONUS.IsEnabled = true;

                    GBONUS.Content = "    BALL\nPOSSESS.";
                    HBONUS.Content = "    BALL\nPOSSESS.";
                    //lblPossessionGuests.Visibility = Visibility.Visible;
                    //lblPossessionHome.Visibility = Visibility.Visible;
                    checkBallPossession();
                    break;
            }

            if (storage.parse_gameMode == "VOLLEYBALL")
            {
                GFOUL.IsEnabled = false;
                HFOUL.IsEnabled = false;

                lblPenalty.Text = "SETS";
                GFOUL.Content = "SET";        // nema foula, stoga ce biti SET
                HFOUL.Content = "SET";

                lblBonusG.Text = "SCORE";
                lblBonusH.Text = "SCORE";
                lblScoreGuests.Text = "SET";
                lblScoreHome.Text = "SET";

                GBONUS.Content = "    BALL\nPOSSESS.";
                HBONUS.Content = "    BALL\nPOSSESS.";
                //lblPossessionGuests.Visibility = Visibility.Visible;
                //lblPossessionHome.Visibility = Visibility.Visible;
                checkBallPossession();
            } else
            {
                GFOUL.IsEnabled = true;
                HFOUL.IsEnabled = true;

                lblBonusG.Text = "BONUS";
                lblBonusH.Text = "BONUS";
                lblScoreGuests.Text = "SCORE";
                lblScoreHome.Text = "SCORE";

                lblPenalty.Text = "PENALTY";
                GFOUL.Content = "FOUL";
                HFOUL.Content = "FOUL";
            }

            if (storage.parse_gameMode == "BASKETBALL")
            {
                gridBasketballScore.Visibility = Visibility.Visible;
            }
            else
            {
                gridBasketballScore.Visibility = Visibility.Hidden;
            }

        }

            // Ball possession switch
        private void checkBallPossession()
        {
            switch(storage.parse_ballPossession)
            {
                case "NONE":
                    lblPossessionGuests.Visibility = Visibility.Hidden;
                    lblPossessionHome.Visibility = Visibility.Hidden;
                    break;
                case "HOME":
                    lblPossessionGuests.Visibility = Visibility.Hidden;
                    lblPossessionHome.Visibility = Visibility.Visible;
                    break;
                case "GUESTS":
                    lblPossessionGuests.Visibility = Visibility.Visible;
                    lblPossessionHome.Visibility = Visibility.Hidden;
                    break;
            }
        }

        #endregion

        /////////////////////////////////////////////////////


        #region Click events

        //GAME
        private void G_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bcc.buttons_game_commands(btn.Name);
            buttonClickRefreshers();

                // Check if the user wants to input players
            if (btn.Name =="ENTER" && checks.gameEntered==true)
            {
                var msg = MessageBox.Show("Would you like to insert player names?", "Option", MessageBoxButton.YesNo);

                    // 1st option - open player list import
                if (msg == MessageBoxResult.Yes)
                {
                        // Check if widow is already summoned
                    if (gpi.IsActive)
                    {
                        var msge = MessageBox.Show("Window already open.", "Warning", MessageBoxButton.OK);
                        return;
                    } else
                    {
                        gpi.Show();
                        gpi.threading();
                        gamePlayers.isPlayerNamesEnabled = true;
                    }                    

                    // 2nd option - list won't be imported
                } else if (msg == MessageBoxResult.No || gamePlayers.plInfoGuests.Length==0 || gamePlayers.plInfoHome.Length==0)
                {
                    gamePlayers.isPlayerNamesEnabled = false;
                } else if (msg == MessageBoxResult.No && gamePlayers.plInfoGuests.Length!=0)
                {
                    gamePlayers.plInfoGuests = null;
                }
                else if (msg == MessageBoxResult.No && gamePlayers.plInfoHome.Length != 0)
                {
                    //gamePlayers.plInfoHome.Clear();
                    gamePlayers.plInfoGuests = null;
                }
            }

        }
        private void MG_Click(object sender, RoutedEventArgs e)
        {
            MenuItem btn = (MenuItem)sender;
            bcc.buttons_game_commands(btn.Name.Substring(3));
            buttonClickRefreshers();
        }

        //RELAY
        private void R_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            buttons_relay_commands(btn.Name);
            buttonClickRefreshers();
        }
        private void MR_Click(object sender, RoutedEventArgs e)
        {
            MenuItem btn = (MenuItem)sender;
            buttons_relay_commands(btn.Name.Substring(3));
            buttonClickRefreshers();
        }

        #region Toggle ECP40 unit
        private void ecp_Click(object sender, RoutedEventArgs e)
        {
            ECP40 ecp = new ECP40(EPC232.Properties.Settings.Default.ECP40Address,
                EPC232.Properties.Settings.Default.ECP40Port);
            ecp.Show();
        }
        #endregion

        #region Toggle Audio interface
        private void toggleAudio_Click(object sender, RoutedEventArgs e)
        {
            InputSettings audioInput = new InputSettings();
            audioInput.Show();
        }
        #endregion

        #region Toggle Media player interface
        private void togglePlayer_Click(object sender, RoutedEventArgs e)
        {
            AudioPlayer player = new AudioPlayer();
            player.Show();
        }
        #endregion

        // End entire region
        #endregion

        private void ContextMenu_visibility_toggle(ref ContextMenu m, ref Grid g)
        {
            m.Placement = System.Windows.Controls.Primitives.PlacementMode.Relative;
            m.PlacementTarget = g;
            m.IsOpen = true;
            m.Visibility = System.Windows.Visibility.Visible;
        }

        #region Keypad press method forwarding
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {

                case Key.NumPad0:
                case Key.D0:
                    pl.P_Click_fire("KEY0");
                    break;

                case Key.NumPad1:
                case Key.D1:
                    pl.P_Click_fire("KEY1");
                    break;

                case Key.NumPad2:
                case Key.D2:
                    pl.P_Click_fire("KEY2");
                    break;

                case Key.NumPad3:
                case Key.D3:
                    pl.P_Click_fire("KEY3");
                    break;

                case Key.NumPad4:
                case Key.D4:
                    pl.P_Click_fire("KEY4");
                    break;

                case Key.NumPad5:
                case Key.D5:
                    pl.P_Click_fire("KEY5");
                    break;

                case Key.NumPad6:
                case Key.D6:
                    pl.P_Click_fire("KEY6");
                    break;

                case Key.NumPad7:
                case Key.D7:
                    pl.P_Click_fire("KEY7");
                    break;

                case Key.NumPad8:
                case Key.D8:
                    pl.P_Click_fire("KEY8");
                    break;

                case Key.NumPad9:
                case Key.D9:
                    pl.P_Click_fire("KEY9");
                    break;

                case Key.Enter:
                    pl.P_Click_fire("PENTER");
                    break;
                case Key.MediaPlayPause:
                    pl.P_Click_fire("PLAY");
                    break;
                case Key.MediaNextTrack:
                    pl.P_Click_fire("NEXT");
                    break;
                case Key.MediaPreviousTrack:
                    pl.P_Click_fire("PREV");
                    break;

                case Key.Add:
                    //MVOL.Value++;
                    // --> rijesiti preko modela - ValueStorage varijabla
                    break;
                case Key.Subtract:
                    //MVOL.Value--;
                    // isto kao i Add
                    break;
                case Key.Delete:
                    commandslogs.Visibility = (commandslogs.Visibility == System.Windows.Visibility.Hidden) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                    break;
                /*case Key.A:
                    ContextMenu_visibility_toggle(ref MenuGame, ref GridGame);
                    break;
                case Key.I:
                    ContextMenu_visibility_toggle(ref MenuInput, ref GridInput);
                    break;
                case Key.Y:
                    ContextMenu_visibility_toggle(ref MenuPlayer, ref GridPlayer);
                    break;
                case Key.S:
                    ContextMenu_visibility_toggle(ref MenuSettings, ref GridSettings);
                    break;
                case Key.R:
                    ContextMenu_visibility_toggle(ref MenuRelay, ref GridRelay);
                    break;*/
                default:
                    break;
            }
        }
        #endregion


        #region Button & slider color and visibility options

        /****************** DUE TO REMOVE
        #region Buttons - transition color change (enter/exit)

        
        private void gameEnteredColorChange()
        {
            if (checks.gameEntered == true)
            {
                EXIT.Background = Brushes.Red;
                ENTER.Background = Brushes.White;
                LOAD.IsEnabled = false;
                LOAD.Background = Brushes.Gray;
            }
            else
            {
                EXIT.Background = Brushes.White;
                ENTER.Background = Brushes.Green;
                LOAD.IsEnabled = true;
                LOAD.Background = Brushes.White;
            }
        }

        #endregion
        **********************************/

        #region Buttons - Relay command + color
        private void buttons_relay_commands(string buttonName)
        {
            SolidColorBrush sv;
            switch (buttonName)
            {
                case "R1":
                    sv = (SolidColorBrush)LedRLY1.Fill;
                    if (sv.Color == Colors.Black)
                    {
                        cpm.SendMsg("RL=1-1", 0);
                    }
                    else
                    {
                        cpm.SendMsg("RL=1-0", 0);
                    }
                    cpm.SendMsg("R?=", 0);
                    break;
                case "R2":
                    sv = (SolidColorBrush)LedRLY2.Fill;
                    if (sv.Color == Colors.Black)
                    {
                        cpm.SendMsg("RL=2-1", 0);
                    }
                    else
                    {
                        cpm.SendMsg("RL=2-0", 0);
                    }
                    cpm.SendMsg("R?=", 0);
                    break;
            }
        }
        #endregion

        /******************************* DUE TO REMOVE
        #region Buttons - dim on game start
        private void buttonDim_onStart()
        {
            bool checkyallbitches = checks.VSTimerStarted;

            EXIT.IsEnabled = !checkyallbitches;
            EXIT.Background = (checkyallbitches == true) ? Brushes.Gray : Brushes.White;
            ENTER.IsEnabled = !checkyallbitches;
            ENTER.Background = (checkyallbitches == true) ? Brushes.Gray : Brushes.White;
            AROW_UP.IsEnabled = !checkyallbitches;
            AROW_UP.Background = (checkyallbitches == true) ? Brushes.Gray : Brushes.Black;
            AROW_DN.IsEnabled = !checkyallbitches;
            AROW_DN.Background = (checkyallbitches == true) ? Brushes.Gray : Brushes.Black;
            AROW_LT.IsEnabled = !checkyallbitches;
            AROW_LT.Background = (checkyallbitches == true) ? Brushes.Gray : Brushes.Black;
            AROW_RT.IsEnabled = !checkyallbitches;
            AROW_RT.Background = (checkyallbitches == true) ? Brushes.Gray : Brushes.Black;
            PERIOD.IsEnabled = !checkyallbitches;
            PERIOD.Background = (checkyallbitches == true) ? Brushes.Gray : Brushes.Yellow;
            GTIMEOUT.IsEnabled = !checkyallbitches;
            GTIMEOUT.Background = (checkyallbitches == true) ? Brushes.Gray : Brushes.Blue;
            HTIMEOUT.IsEnabled = !checkyallbitches;
            HTIMEOUT.Background = (checkyallbitches == true) ? Brushes.Gray : Brushes.White;
        }

        #endregion
        **********************************/

        // End entire region
        #endregion


        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(sender is MenuItem mi)) return;
                if (mi.Name != "Connect") return;
                if (gridDevices.SelectedItem == null) return;
                var ip = (gridDevices.SelectedItem as UdpClinetMsg)?.IpAddress;
                var port = (gridDevices.SelectedItem as UdpClinetMsg)?.MasterPort;
                if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port))
                {
                    return;
                }
                Properties.Settings.Default.EPCAddress = ip;
                int.TryParse(port, out var num);
                Properties.Settings.Default.EPCPort = num;
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
                //ignore
            }
        }

        #region TCP/UDP commands & init

        private void ApplyFilter()
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(commandslogs.ItemsSource);
            cv.Filter = null;
            cv.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
            cv.Refresh();
        }

        public void TcpClientRun()
        {
            _logs = new List<LogData>();
            //var host = Properties.Settings.Default.EPCAddress;
            //var port = Properties.Settings.Default.EPCPort;
            //_client = cpm.GetIpClient();
            _client.NewLogMsg += Client_NewLogMsg;
        }

        #region Client log messages
        private void Client_NewLogMsg(object sender, LogMsg e)
        {
            var da = new LogData
            {
                Time = e.Time,
                Cmd = e.Cmd,
                Answer = e.Answer
            };

            void Action()
            {
                answersSerial.parse_answer(da);
            }
            Main.Dispatcher.BeginInvoke((Action)Action);

            da.Id = _logsCount + 1;
            _logsCount++;
            _logs.Add(da);
            var maxcount = Properties.Settings.Default.LogsMaxCount;
            if (_logs.Count > maxcount)
            {
                _logs.RemoveAt(0);
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(ApplyFilter));
        }

        private void UdpServerDiscoverer_NewLogMsg(object sender, UdpClinetMsg e)
        {
            void Action()
            {
                if ((e.MasterPort == Properties.Settings.Default.EPCPort.ToString()) && (e.IpAddress == Properties.Settings.Default.EPCAddress.ToString()))
                {
                    leds.chkLedConnection = true;
                }
                else
                {
                    leds.chkLedConnection = false;
                }

                //elipseFill(LEDConnection, serialParse.chkLedConnection);
                elipseFillCheck();

                // Create list of devices that ensure connection
                var lst = new List<UdpClinetMsg> { e };
                gridDevices.ItemsSource = lst;
                gridDevices.UpdateLayout();
            }
            Main.Dispatcher.BeginInvoke((Action)Action);
        }
        #endregion

        #endregion

        
        #region Application shutdown methods
        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Current.Shutdown(0);
            Application.Current.Exit += Current_Exit;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
            Application.Current.Exit += Current_Exit;
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            if (_client != null)
            {
                _client.Dispose();
                _client.DisposeThreads();
            }
            _client = null;
            Environment.Exit(0);
        }

        #endregion


    }
}
