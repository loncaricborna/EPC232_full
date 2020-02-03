using EPC232.communication;
using EPC232.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPC232
{
    public class CmndMsg
    {
        public string Cmd { get; set; }
        public int DelayAfter { get; set; }
    }

    /*
     *          LogMsg 
     */

    // Dobivanje poruka iz TcpClient.cs klase
    // Pretvara podatke dobivene s TCP-a u format citljiv aplikaciji
    public class LogMsg : System.EventArgs          
    {
        
        public string Cmd { get; set; }
        public string Answer { get; set; }
        public TimeSpan Time { get; set; }
    } 
                                                
    public class LogData
    {
        public int Id { get; set; }
        public string Cmd { get; set; }
        public string Answer { get; set; }
        public TimeSpan Time { get; set; }
    }

}
