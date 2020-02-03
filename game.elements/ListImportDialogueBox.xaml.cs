using EPC232.model;
using System;
using System.Collections.Generic;
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
using System.Xml;
using System.Xml.Serialization;

namespace EPC232.game.elements
{
    /// <summary>
    /// Interaction logic for ListImportDialogueBox.xaml
    /// </summary>
    public partial class ListImportDialogueBox : Window

    {
        String path = "";

        ValuePlayers player;
        GamePlayersInput gpi;

        string[] importGuests = new string[12];
        string[] importHome = new string[12];

        public void setPlayers(ValuePlayers player)
        {
            this.player = player;
        }

        public void setGpi(GamePlayersInput gpi)
        {
            this.gpi = gpi;
        }

        public ListImportDialogueBox()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
        }

        private void BtnImportGuests_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var ofd = new OpenFileDialog();

                ofd.Title = "Select a file to import";
                ofd.DefaultExt = "*.epl";
                ofd.Filter = "Elak player list(*.epl)|*.epl|Text file (*.txt)|*.txt";

                    // Open file browsing dialog
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = Path.GetFullPath(ofd.FileName);
                }

                XmlSerializer xmlSer = new XmlSerializer(typeof(string[]));
                System.IO.StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open));

                // Try to load up the XML list
                try
                {
                    
                    importGuests = (string[])xmlSer.Deserialize(sr);

                    for (int i = 0; i < importGuests.Length; i++)
                    {
                        player.plInfoGuests[i] = importGuests[i];//loadDoc.Attributes.Item(i).ToString();
                        var msge = System.Windows.MessageBox.Show(player.plInfoGuests[i], "Warning!", MessageBoxButton.OK);

                    }
                    gpi.refreshList();

                } catch (Exception isus)
                {
                    var msg = System.Windows.MessageBox.Show("LIDB internal error code: \n\n" + isus, "Warning!", MessageBoxButton.OK);
                }
                                
                
            } catch (Exception er)
            {
                var msg = System.Windows.MessageBox.Show("LIDB external error code: \n\n" + er, "Warning!", MessageBoxButton.OK);
            }
        }

        private void BtnImportHome_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Title = "Select a file to import";
                ofd.DefaultExt = "*.epl";
                ofd.Filter = "Elak player list(*.epl)|*.epl|Text file (*.txt)|*.txt";

                    // Open file browsing dialog
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = Path.GetFullPath(ofd.FileName);
                }

                XmlSerializer xmlSer = new XmlSerializer(typeof(string[]));
                System.IO.StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open));

                // Try to load up the XML list
                try
                {
                    importHome = (string[])xmlSer.Deserialize(sr);

                    for (int i = 0; i < importHome.Length; i++)  // attribute count kurac - NullRefference?
                    {
                        player.plInfoHome[i] = importHome[i];
                        var msge = System.Windows.MessageBox.Show(player.plInfoHome[i], "Warning!", MessageBoxButton.OK);

                    }

                    gpi.refreshList();  // predstavlja neki problem, provjeriti koji (ArgumentOutOfBounds + NRE) 

                } catch (Exception xe)
                {
                    var msg = System.Windows.MessageBox.Show("LIDB internal error code: \n\n" + xe, "Warning!", MessageBoxButton.OK);
                }
                
                
            }
            catch (Exception er)
            {
                var msg = System.Windows.MessageBox.Show("LIDB external error code: \n\n" + er, "Warning!", MessageBoxButton.OK);
            }
        }

        private void hideWindow(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
