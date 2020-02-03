using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPC232.communication;
using EPC232.model;
using EPC232.game.elements;

namespace EPC232.audio.gui.windows
{
    class AudioButtonOps
    {

        CommsParseMessage cpm;
        ValueStorage storage;
        TimerClass tc;

        /***********************************************************************
         * Getters & setters
         **********************************************************************/

        public void setCpm(CommsParseMessage inputCpm)
        {
            cpm = inputCpm;
        }

        public void setStorage(ValueStorage inputStorage)
        {
            storage = inputStorage;
        }

        public void setTimerClass(TimerClass tcIn)
        {
            tc = tcIn;
        }

        /***********************************************************************
         * Button commands - AUDIO
         **********************************************************************/

        #region Buttons - Settings Commands
        public void buttons_settings_command(string buttonName)
        {
            switch (buttonName)
            {
                case "S1":
                    cpm.SendMsg("I?=1", 0);
                    break;
                case "S2":
                    cpm.SendMsg("I?=2", 0);
                    break;
                case "S3":
                    cpm.SendMsg("I?=3", 0);
                    break;
                case "S4":
                    cpm.SendMsg("I?=4", 0);
                    break;
                case "S5":
                    cpm.SendMsg("I?=5", 0);
                    break;
            }
        }
        #endregion


        #region Buttons - Music Player Commands
        public void buttons_player_commands(int serialNumber, string buttonName)
        {
            int res = -1;
            string cmd = "";
            switch (serialNumber)
            {
                case 1:
                    res = 0;
                    cmd = "MP=1-";
                    break;
                case 2:
                    res = 0;
                    cmd = "MP=2-";
                    break;
                case 3:
                    res = 0;
                    cmd = "MP=3-";
                    break;
            }

            if (res == 0)
            {
                switch (buttonName)
                {

                    #region General player operands
                    case "PLAY":
                        cpm.SendMsg(cmd + "PY", 0);
                        break;
                    case "STOP":
                        cpm.SendMsg(cmd + "ST", 0);
                        break;
                    case "REV":
                        cpm.SendMsg(cmd + "RV", 0);
                        break;
                    case "FWD":
                        cpm.SendMsg(cmd + "FW", 0);
                        break;
                    case "PREV":
                        cpm.SendMsg(cmd + "PV", 0);
                        break;
                    case "NEXT":
                        cpm.SendMsg(cmd + "NX", 0);
                        break;
                    case "MUTE":
                        cpm.SendMsg(cmd + "MU", 0);
                        break;
                    case "RANDOM":
                        cpm.SendMsg(cmd + "RA", 0);
                        break;
                    case "REPEAT":
                        cpm.SendMsg(cmd + "RE", 0);
                        break;
                    #endregion

                    #region Source select + CD & USB ops
                    case "SOURCE":
                        cpm.SendMsg(cmd + "CD", 0);
                        break;
                    case "FOLDER":
                        cpm.SendMsg(cmd + "FO", 0);
                        break;
                    case "EJECT":
                        cpm.SendMsg(cmd + "EJ", 0);
                        break;
                    #endregion

                    #region Radio operations
                    case "BAND":
                        cpm.SendMsg(cmd + "BD", 0);
                        break;
                    case "TUNEDN":
                        cpm.SendMsg(cmd + "TD", 0);
                        break;
                    case "TUNEUP":
                        cpm.SendMsg(cmd + "TU", 0);
                        break;
                    case "PROG":
                        cpm.SendMsg(cmd + "PG", 0);
                        break;
                    case "MEMORY":
                        cpm.SendMsg(cmd + "ME", 0);
                        break;
                    case "PENTER":
                        cpm.SendMsg(cmd + "EN", 0);
                        break;
                    case "PPLUS":
                        cpm.SendMsg(cmd + "AU", 0);
                        break;
                    case "PMINUS":
                        cpm.SendMsg(cmd + "AD", 0);
                        break;
                    #endregion

                    #region Number buttons (0-9)
                    case "KEY0":
                        cpm.SendMsg(cmd + "K0", 0);
                        break;
                    case "KEY1":
                        cpm.SendMsg(cmd + "K1", 0);
                        break;
                    case "KEY2":
                        cpm.SendMsg(cmd + "K2", 0);
                        break;
                    case "KEY3":
                        cpm.SendMsg(cmd + "K3", 0);
                        break;
                    case "KEY4":
                        cpm.SendMsg(cmd + "K4", 0);
                        break;
                    case "KEY5":
                        cpm.SendMsg(cmd + "K5", 0);
                        break;
                    case "KEY6":
                        cpm.SendMsg(cmd + "K6", 0);
                        break;
                    case "KEY7":
                        cpm.SendMsg(cmd + "K7", 0);
                        break;
                    case "KEY8":
                        cpm.SendMsg(cmd + "K8", 0);
                        break;
                    case "KEY9":
                        cpm.SendMsg(cmd + "K9", 0);
                        break;
                    #endregion

                }
            }

        }
        #endregion


        #region Buttons - Inputs Commands
        public void buttons_inputs_command(string buttonName)
        {
            switch (buttonName)
            {
                case "I1":
                    tc.OnShotTimerGong_Stop();
                    cpm.SendMsg("IN=1", 0);
                    cpm.SendMsg("I?=1", 0);
                    break;
                case "I2":
                    tc.OnShotTimerGong_Stop();
                    cpm.SendMsg("IN=2", 0);
                    cpm.SendMsg("I?=2", 0);
                    break;
                case "I3":
                    tc.OnShotTimerGong_Stop();
                    cpm.SendMsg("IN=3", 0);
                    cpm.SendMsg("I?=3", 0);
                    break;
                case "I4":
                    if (Properties.Settings.Default.IsGongEnabled == true)
                    {
                        cpm.SendMsg("IN=5", 0);
                        cpm.SendMsg("GN=5", 0);
                        tc.OnShotTimerGong_Fire();
                    }
                    else
                    {
                        cpm.SendMsg("IN=4", 0);
                    }
                    cpm.SendMsg("I?=4", 0);
                    break;
            }
        }
        #endregion


        #region Slider Commands
        public void sliders_command(string sliderName, int value)
        {
            string val;
            switch (sliderName)
            {
                case "GAIN":
                    val = (value + 1).ToString();
                    cpm.SendMsg("IG=" + storage._settingsIndex + "-" + val, 0);
                    break;
                case "VOL":
                    val = (14 - value).ToString();
                    cpm.SendMsg("IV=" + storage._settingsIndex + "-" + val, 0);
                    break;
                case "BASS":
                    val = (value + 1).ToString();
                    cpm.SendMsg("IB=" + storage._settingsIndex + "-" + val, 0);
                    break;
                case "TREB":
                    val = (value + 1).ToString();
                    cpm.SendMsg("IT=" + storage._settingsIndex + "-" + val, 0);
                    break;
                case "ATTL":
                    val = (18 - value).ToString();
                    cpm.SendMsg("IL=" + storage._settingsIndex + "-" + val, 0);
                    break;
                case "ATTR":
                    val = (18 - value).ToString();
                    cpm.SendMsg("IR=" + storage._settingsIndex + "-" + val, 0);
                    break;
                case "MVOL":
                    val = (14 - value).ToString();
                    cpm.SendMsg("IV=" + storage._inputIndex + "-" + val, 0);
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}
