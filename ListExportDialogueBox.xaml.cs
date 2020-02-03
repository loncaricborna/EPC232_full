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

        public void setValuePlayers(ValuePlayers inPlayer)
        {
            players = inPlayer;
        }

        public ListExportDialogueBox()
        {
            InitializeComponent();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (chkBoxGuests.IsChecked==false && chkBoxHome.IsChecked==false)
            {
                var msg = System.Windows.MessageBox.Show("No list selected.", "Error", MessageBoxButton.OK);
            } else
            {
                ObservableCollection<string> exportGuests = players.plInfoGuests;
                ObservableCollection<string> exportHome = players.plInfoHome;

                if (chkBoxGuests.IsChecked==true && chkBoxGuests.IsChecked==true)
                {
                    try
                    {
                        var folderBrowserDialog = new FolderBrowserDialog();
                        folderBrowserDialog.Description = "Select where to save guest player list: ";

                        DialogResult resG = folderBrowserDialog.ShowDialog();
                        if (resG == System.Windows.Forms.DialogResult.OK)
                        {
                            path = folderBrowserDialog.SelectedPath + ".epl";

                            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<string>));
                            using (StreamWriter sw = new StreamWriter(path))
                            {
                                xs.Serialize(sw, exportGuests);
                            }
                        }
                        //////////////////////////////////////////////
                        ///
                        folderBrowserDialog.Description = "Select where to save home player list: ";

                        DialogResult resH = folderBrowserDialog.ShowDialog();
                        if (resH == System.Windows.Forms.DialogResult.OK)
                        {
                            path = folderBrowserDialog.SelectedPath + ".epl";

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
                    try
                    {
                        var folderBrowserDialog = new FolderBrowserDialog();
                        folderBrowserDialog.Description = "Select where to save guest player list: ";

                        DialogResult resG = folderBrowserDialog.ShowDialog();
                        if (resG == System.Windows.Forms.DialogResult.OK)
                        {
                            path = folderBrowserDialog.SelectedPath + ".epl";

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
                    try
                    {
                        var folderBrowserDialog = new FolderBrowserDialog();
                        folderBrowserDialog.Description = "Select where to save home player list: ";

                        DialogResult resH = folderBrowserDialog.ShowDialog();
                        if (resH == System.Windows.Forms.DialogResult.OK)
                        {
                            path = folderBrowserDialog.SelectedPath + ".epl";

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

                Environment.Exit(0);
            }
        }
    }
}
