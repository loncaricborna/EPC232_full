using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using EPC232.model;

namespace EPC232.game.elements
{
    /// <summary>
    /// Interaction logic for GamePlayersInput.xaml
    /// </summary>
    public partial class GamePlayersInput : Window
    {
        int iHome = 0;
        int iGuests = 0;

        string insertionString = "";

        ValuePlayers player;
        ListExportDialogueBox ledb;
        ListImportDialogueBox lidb;

        private ObservableCollection<string> plInfoHome = new ObservableCollection<string>();
        private ObservableCollection<string> plInfoGuests = new ObservableCollection<string>();

        DispatcherTimer thread = new DispatcherTimer();

        //ListExportDialogueBox ledb;// = new ListExportDialogueBox();

        #region Getters & setters
        /***********************************************************************
         * Getters & setters
         **********************************************************************/

        public void setPlayer(ValuePlayers playerIn)
        {
            player = playerIn;
            //player.allowedPPT = 12; // Initial default value, vjerojatno rijesava problem. Smisliti upravljanje

                // Stvaranje listi prebaceno u glavnu inicijalizaciju programa
            //player.plInfoGuests = new string[player.allowedPPT];    // tu rijesiti problem!
            //player.plInfoHome = new string[player.allowedPPT];      // -||-
        }

        public void setLedb(ListExportDialogueBox ledb)
        {
            this.ledb = ledb;
        }

        public void setLidb(ListImportDialogueBox lidb)
        {
            this.lidb = lidb;
        }

        #endregion

        public GamePlayersInput()//ValuePlayers playerIn)
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            // source bio player.plInfo**
            //listBoxHome.ItemsSource = plInfoHome;
            //listBoxGuests.ItemsSource = plInfoGuests;
            //threading();
        }

        #region Click - INSERT functions

            // Add home player
        private void InsertHome_Click(object sender, RoutedEventArgs e)
        {
            if (insertNumber.Text != "" || inName.Text != "")
            {
                int insertNumber;
                int insertName;
                bool isDigitCheck = int.TryParse(this.insertNumber.Text, out insertNumber);
                bool isNameValid = int.TryParse(inName.Text, out insertName);

                if (isDigitCheck == false || isNameValid == true)
                {
                    MessageBox.Show("Invalid input!", "Warning!", MessageBoxButton.OK);
                }
                else
                {
                    if (player.isHomeListFull == false && insertNumber < 100 && insertNumber > 0)
                    {
                        
                        insertionString = insertNumber.ToString() + ", " + inName.Text;

                        plInfoHome.Add(insertionString);
                        player.plInfoHome[iHome] = insertionString;
                        iHome++;

                        inName.Text = string.Empty;
                        this.insertNumber.Text = string.Empty;

                        if (iHome == player.allowedPPT)
                        {
                            player.isHomeListFull = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("List population limit achieved", "Error", MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("Input empty.", "Warning!", MessageBoxButton.OK);
            }
        }

            // Add guest player
        private void InsertGuests_Click(object sender, RoutedEventArgs e)
        {
            if (insertNumber.Text != "" || inName.Text != "")
            {
                int insertNumber;
                int insertName;
                bool isDigitCheck = int.TryParse(this.insertNumber.Text, out insertNumber);
                bool isNameValid = int.TryParse(inName.Text, out insertName);

                if (isDigitCheck == false || isNameValid == true)
                {
                    MessageBox.Show("Invalid input!", "Warning!", MessageBoxButton.OK);
                }
                else
                {
                    if (player.isGuestsListFull == false && insertNumber < 99 && insertNumber > 0)
                    {
                        
                        insertionString = insertNumber.ToString() + ", " + inName.Text;

                        plInfoGuests.Add(insertionString);
                        player.plInfoGuests[iGuests] = insertionString;
                        iGuests++;

                        inName.Text = string.Empty;
                        this.insertNumber.Text = string.Empty;

                        if (iGuests == player.allowedPPT)
                        {
                            player.isGuestsListFull = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("List population limit achieved.", "Error", MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("Input empty.", "Warning!", MessageBoxButton.OK);
            }
        }
        #endregion

        #region Click - DELETE functions
        private void deleteGuests_ButtonClick(object sender, RoutedEventArgs e)
        {
            for (int i=0; i<listBoxGuests.Items.Count; i++)
            {
                if (listBoxGuests.SelectedIndex == i)
                {
                    player.plInfoGuests[i] = plInfoGuests[i] = null;     // bio iz player.

                    iGuests--;
                }
            }
        }

        private void deleteHome_ButtonClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < listBoxHome.Items.Count; i++)
            {
                if (listBoxHome.SelectedIndex == i)
                {
                    player.plInfoHome[i] = plInfoHome[i] = null;    // bio iz player.

                    iHome--;
                }
            }
        }
        #endregion

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxGuests.Items.Count==0 || listBoxHome.Items.Count==0)
            {
                var msg = MessageBox.Show("Lists not loaded correctly.", "Error", MessageBoxButton.OK);
            } else
            {
                //Close();
                Hide();
            }
        }

        private void declineButton_Click(object sender, RoutedEventArgs e)
        {
            OnClose();
            Hide();
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ledb.Show();
            } catch (Exception exc)
            {
                var msg = MessageBox.Show("GPI Error code: " + exc.StackTrace.ToString(), "Error", MessageBoxButton.OK);
            }
            
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lidb.Show();
            }
            catch (Exception exc)
            {
                var msg = MessageBox.Show("GPI Error code: " + exc.StackTrace.ToString(), "Error", MessageBoxButton.OK);
            }
        }

        public void refreshList()
        {
            try
            {
                if (player.plInfoGuests.Length!=0)
                {
                    var msg = MessageBox.Show("Number of items: " + player.plInfoGuests.Length, "Notice", MessageBoxButton.OK);

                    for (int i = 0; i<player.plInfoGuests.Length; i++)    // tu je problem neki lol
                    {
                        plInfoGuests.Insert(i, player.plInfoGuests[i]);
                        //plInfoGuests[i] = player.plInfoGuests[i];
                        //var msg = MessageBox.Show("List item: " + plInfoGuests[i], "Notice", MessageBoxButton.OK);
                    }
                }
                
                if (player.plInfoHome.Length!=0)
                {
                    var msg = MessageBox.Show("Number of items: " + player.plInfoHome.Length, "Notice", MessageBoxButton.OK);

                    for (int i = 0; i<player.plInfoHome.Length; i++)
                    {
                        plInfoHome.Insert(i, player.plInfoHome[i]);// = player.plInfoHome[i];
                        //var msg = MessageBox.Show("List item: " + plInfoHome[i], "Notice", MessageBoxButton.OK);
                    }
                }                
            } catch (Exception burek)
            {
                var msg = MessageBox.Show("List cannot be filled.\n\nGPI Error code: " + burek.StackTrace.ToString(), "Error", MessageBoxButton.OK);
            }
            
        }

            // Depracted function
        /*private void convertListInfo(string[] inputString, ObservableCollection<string> inputCollection)
        {
            for (int i=0; i<inputCollection.Count; i++)
            {
                inputString[i] = inputCollection[i].ToString();
            }
        }*/

        public void threading()
        {
            thread.Interval = new TimeSpan(0, 0, 0, 0, 200);
            //thread.Tick += TimerTick;
            thread.Start();
            listBoxHome.ItemsSource = plInfoHome;
            listBoxGuests.ItemsSource = plInfoGuests;
        }

        private void OnClose()
        {
            thread.Stop();
        }

    }
}
