using EPC232.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPC232.communication
{
    class CommsParseMessage
    {

            // Object initializations

        ValueStorage storage;
        ValueLEDs leds;

        #region Getters & setters
        /***********************************************************************
         * Getters & setters
         **********************************************************************/

        public void setLeds(ValueLEDs ledsIn)
        {
            this.leds = ledsIn;
        }

        public ValueLEDs getLeds()
        {
            return leds;
        }

        public ValueStorage getValueStorage()
        {
            return storage;
        }
        public void setStorage(ValueStorage storage)
        {
            this.storage = storage;
        }
        #endregion

        /***********************************************************************
         * TCP
         **********************************************************************/

        // TCP variables
        private ipClient _client;


            // Client socket values
        string host = Properties.Settings.Default.EPCAddress;
        int port = Properties.Settings.Default.EPCPort;


        //////////////////////////////////////////////////////////
        
        public ipClient GetIpClient()
        {
            return _client;
        }

        public void setIpClient(ipClient client)
        {
            this._client = client;
        }

        //////////////////////////////////////////////////////////
        
        
        public void SendMsg(string cmd, int delay)
        {
            var msg = new CmndMsg
            {
                DelayAfter = delay,
                Cmd = cmd
            };
            if (_client.QueueMsg(msg) == false)
            {
                leds.chkLedConnection = false;
            } else
            {
                leds.chkLedConnection = true;
            }

        }

        /*
        public void TcpClient_Init()
        {
            //_logs = new List<LogData>();
            //var host = Properties.Settings.Default.EPCAddress;
            //var port = Properties.Settings.Default.EPCPort;
            _client = new ipClient(host, port);
            //_client.NewLogMsg += Client_NewLogMsg;
        }
        */
    }
}
