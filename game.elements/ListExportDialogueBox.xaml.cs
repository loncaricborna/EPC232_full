using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using EPC232.model;

namespace EPC232.game.elements
{
    /// <summary>
    /// Interaction logic for ListExportDialogueBox.xaml
    /// </summary>
    public partial class ListExportDialogueBox : Window
    {

        ValuePlayers players;

        string path;
        ObservableCollection<string> exportGuests = new ObservableCollection<string>();
        ObservableCollection<string> exportHome = new ObservableCollection<string>();

        public void setValuePlayers(ValuePlayers inPlayer)
        {
            players = inPlayer;
        }

        public ListExportDialogueBox()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
        }

        #region Export function
        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (chkBoxGuests.IsChecked==false && chkBoxHome.IsChecked==false)
            {
                var msg = System.Windows.MessageBox.Show("No list selected.", "Error", MessageBoxButton.OK);
            } else
            {
                

                if (chkBoxGuests.IsChecked==true && chkBoxGuests.IsChecked==true)
                {
                    // Add both list team names
                    exportGuests.Add(players.teamGuestsName);
                    exportHome.Add(players.teamHomeName);

                    // Write guests into a singular object
                    for (int i = 1; i <= players.plInfoGuests.Length; i++)
                    {
                        string toAdd = players.plInfoGuests[i];
                        exportGuests.Add(toAdd);
                    }
                        // Write home into a singular object
                    for (int i = 1; i <= players.plInfoHome.Length; i++)
                    {
                        string toAdd = players.plInfoHome[i];
                        exportHome.Add(toAdd);
                    }

                    try
                    {
                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Title = "Select where to save guest player list: ";
                        saveFileDialog.DefaultExt = "*.epl";
                        saveFileDialog.OverwritePrompt = true;
                        saveFileDialog.Filter = "Elak player list(*.epl)|*.epl|Text file (*.txt)|*.txt";

                        DialogResult resG = saveFileDialog.ShowDialog();
                        if (resG == System.Windows.Forms.DialogResult.OK)
                        {
                            path = saveFileDialog.FileName;

                            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<string>));
                            using (StreamWriter sw = new StreamWriter(path))
                            {
                                xs.Serialize(sw, exportGuests);
                            }
                        }
                        //////////////////////////////////////////////
                        ///
                        saveFileDialog.Title = "Select where to save home player list: ";
                        saveFileDialog.DefaultExt = "*.epl";
                        saveFileDialog.OverwritePrompt = true;
                        saveFileDialog.Filter = "Elak player list(*.epl)|*.epl|Text file (*.txt)|*.txt";

                        DialogResult resH = saveFileDialog.ShowDialog();
                        if (resH == System.Windows.Forms.DialogResult.OK)
                        {
                            path = saveFileDialog.FileName;

                            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<string>));
                            using (StreamWriter sw = new StreamWriter(path))
                            {
                                xs.Serialize(sw, exportHome);
                            }
                        }

                    } catch (Exception exc)
                    {
                        var msg = System.Windows.MessageBox.Show("Warning: " + exc.StackTrace.ToString(), "Error", 
                            MessageBoxButton.OK);
                    }
                } else if (chkBoxGuests.IsChecked==true)
                {
                        // First, add team name
                    exportGuests.Add(players.teamGuestsName);

                        // Then add the players
                    for (int i = 1; i <= players.plInfoGuests.Length; i++)
                    {
                        string toAdd = players.plInfoGuests[i];
                        exportGuests.Add(toAdd);
                    }

                    try
                    {
                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Title = "Select where to save guest player list: ";
                        saveFileDialog.DefaultExt = "*.epl";
                        saveFileDialog.OverwritePrompt = true;
                        saveFileDialog.Filter = "Elak player list(*.epl)|*.epl|Text file (*.txt)|*.txt";

                        DialogResult resG = saveFileDialog.ShowDialog();
                        if (resG == System.Windows.Forms.DialogResult.OK)
                        {
                            path = saveFileDialog.FileName;

                            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<string>));
                            using (StreamWriter sw = new StreamWriter(path))
                            {
                                xs.Serialize(sw, exportGuests);
                            }
                        }
                    } catch (Exception exc)
                    {
                        var msg = System.Windows.MessageBox.Show("Warning: " + exc.StackTrace.ToString(), "Error",
                            MessageBoxButton.OK);
                    }
                } else if (chkBoxHome.IsChecked==true)
                {
                        // First, add team name
                    exportHome.Add(players.teamHomeName);

                        // Then add the players
                    for (int i = 1; i <= players.plInfoHome.Length; i++)
                    {
                        string toAdd = players.plInfoHome[i];
                        exportHome.Add(toAdd);
                    }

                    try
                    {
                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Title = "Select where to save home player list: ";
                        saveFileDialog.DefaultExt = "*.epl";
                        saveFileDialog.OverwritePrompt = true;
                        saveFileDialog.Filter = "Elak player list(*.epl)|*.epl|Text file (*.txt)|*.txt";

                        DialogResult resH = saveFileDialog.ShowDialog();
                        if (resH == System.Windows.Forms.DialogResult.OK)
                        {
                            path = saveFileDialog.FileName;

                            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<string>));
                            using (StreamWriter sw = new StreamWriter(path))
                            {
                                xs.Serialize(sw, exportHome);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        var msg = System.Windows.MessageBox.Show("Warning: " + exc.StackTrace.ToString(), "Error",
                            MessageBoxButton.OK);
                    }
                }

                Hide();     // bilo Close()
            }
        }
        #endregion

        private void hideWindow(object source, EventArgs e)
        {
            Hide();
        }

    }
}
