using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPC232.model;

/*******************************************************************************
 * 
 * File: AnswersSerial.cs
 * 
 * Authors: Borna Loncaric
 * 
 * Device name: EPC232
 * Ver: Full
 * 
 * Description:
 * 
 * Klasa iscitavanja odgovora EPC uredjaja, kao i odgovarajucim reakcijama 
 * racunalnog programa
 * 
 ******************************************************************************/

namespace EPC232.communication
{
    class AnswersSerial
    {

        GameStatusRefresh gsr;
        AnswerSerialVSActions asva;

        ValueLEDs leds;
        ValueStorage storage;

        #region Getters & setters
        /***********************************************************************
         * Getters & setters
         **********************************************************************/

        public void setGsr(GameStatusRefresh gsrIn)
        {
            this.gsr = gsrIn;
        }

        public GameStatusRefresh getGsr()
        {
            return gsr;
        }

        public void setAsva(AnswerSerialVSActions asvaIn)
        {
            this.asva = asvaIn;
        }

        public AnswerSerialVSActions getAsva()
        {
            return asva;
        }

        public void setLeds(ValueLEDs ledsIn)
        {
            this.leds = ledsIn;
        }

        public ValueLEDs getLeds()
        {
            return leds;
        }

        public void setStorage(ValueStorage storageIn)
        {
            this.storage = storageIn;
        } 

        public ValueStorage getStorage()
        {
            return storage;
        }

        /*************************************************************************/
        #endregion

        public void parse_answer(LogData da)
        {
            try
            {
                if (!da.Cmd[1].Equals('?')) return;
                switch (da.Cmd[0])
                {
                    case 'X':
                        parse_answer_tcpclient(da.Cmd);
                        break;
                    case 'I':
                        parse_answer_input(da.Answer);
                        break;
                    case 'R':
                        parse_answer_relay_leds(da.Answer);
                        break;
                    case 'G':
                        parse_answer_game(da.Answer);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        public void parse_answer_tcpclient(string cmd)
        {
            if (cmd.Substring(3, 2) == "DA")
            {
                gsr.status_refresh_all();
            }
        }

        // Input za sada ignoriran
        #region Audio matrix commands

        public void parse_answer_input(string answer)
        {
            //I?=1,0,0,0,0,0,0,1 
            var index = answer[3];
            var values = answer.Split(new char[] { ',' });
            if (index == storage._inputIndex)

            {
                if (int.TryParse(values[2], out var val))
                {
                    //Slider_value_change("MVOL", val);
                    storage.sliderName = "MVOL";
                    storage.sliderValue = val;
                }

            }

            if (index != storage._settingsIndex) return;
            {
                if (values.Length <= 6) return;

                if (int.TryParse(values[1], out var val))
                {
                    //Slider_value_change("GAIN", val);
                    storage.sliderName = "GAIN";
                    storage.sliderValue = val;
                }

                if (int.TryParse(values[2], out val))
                {
                    //Slider_value_change("VOL", val);
                    storage.sliderName = "VOL";
                    storage.sliderValue = val;
                }

                if (int.TryParse(values[3], out val))
                {
                    //Slider_value_change("BASS", val);
                    storage.sliderName = "BASS";
                    storage.sliderValue = val;
                }

                if (int.TryParse(values[4], out val))
                {
                    //Slider_value_change("TREB", val);
                    storage.sliderName = "TREB";
                    storage.sliderValue = val;
                }
                if (int.TryParse(values[5], out val))
                {
                    //Slider_value_change("ATTL", val);
                    storage.sliderName = "ATTL";
                    storage.sliderValue = val;
                }
                if (int.TryParse(values[6], out val))
                {
                    //Slider_value_change("ATTR", val);
                    storage.sliderName = "ATTR";
                    storage.sliderValue = val;
                }

                //settings_sliders_show_hide(storage._settingsIndex);       // od sada se priziva u MainWindow odmah iza parse_answer
            }

        }
        #endregion

        #region Relay LED
        public void parse_answer_relay_leds(string answer)
        {
            //"R?=0,0\n\0"
            if (answer[3] == '1')
            {
                leds.chkLedRelay1 = true;
            }
            else if (answer[3] == '0')
            {
                leds.chkLedRelay1 = false;
            }

            if (answer[5] == '1')
            {
                leds.chkLedRelay2 = true;
            }
            else if (answer[5] == '0')
            {
                leds.chkLedRelay2 = false;
            }

        }
        #endregion

        #region Parse Game related answers

        public void parse_answer_game(string answer)
        {
            //"G?=KY000\n\0"
            // SEU
            // S : Start E Edit U Up or Down
            // answer is null TODO 
            if (answer == null) return;

            var key = answer.Substring(3, 2);
            var key3 = answer.Substring(3, 3);

            var action = answer.Substring(5);
            var action3 = answer.Substring(6);

            switch (key)
            {
                case "KY":  // Key press
                    asva.parse_answer_game_KY(action);
                    break;
                case "TP":  // Time period
                    asva.parse_answer_game_TP(action);
                    break;
                case "RS":  // Result set
                    asva.parse_answer_game_RS(action);
                    break;
                case "BO":  // Bonus
                    asva.parse_answer_game_B(action);
                    break;
                case "TO":  // Timeout
                    asva.parse_answer_game_TO(action);
                    break;
                case "GM":  // Game mode
                    asva.parse_answer_game_GM(action);
                    break;
                case "TM":  
                    asva.parse_answer_game_TM(action);
                    break;
                case "PD":
                    asva.parse_answer_game_PD(action);
                    break;
                case "SR":
                    asva.parse_answer_game_SR(action);
                    break;
                case "SE":  // Volleyball set
                    asva.parse_answer_game_SE(action);
                    break;
                case "EC":  // Edit flag check
                    asva.parse_answer_game_EC(action);
                    break;
                case "AT":
                    asva.parse_answer_game_actionTime(action);
                    break;
                default:
                    break;
            }

            switch (key3)
            {
                case "TOT": // Timeout time
                    asva.parse_answer_game_TOT(action3);
                    break;
                case "TTC": // Timeout check
                    asva.parse_answer_game_TTC(action3);
                    break;
                default:
                    break;
            }

        }

        #endregion

    }
}
