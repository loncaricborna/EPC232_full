using EPC232.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPC232.communication
{
    class AnswerSerialVSActions
    {

        ValueStorage storage;
        ValueChecks checks;
        ValueLEDs leds;
        ValuePlayers gamePlayers;

        #region Getters & setters

        /***********************************************************************
         * Getters & setters
         **********************************************************************/

        public void setChecks(ValueChecks checksIn)
        {
            this.checks = checksIn;
        }

        public void setLeds(ValueLEDs ledsIn)
        {
            this.leds = ledsIn;
        }

        public void setStorage(ValueStorage storageIn)
        {
            this.storage = storageIn;
        }

        public void setGamePlayers(ValuePlayers playersIn)
        {
            this.gamePlayers = playersIn;
        }

        #endregion

        /***********************************************************************
         * 
         *                            --COMMANDS--
         * 
         **********************************************************************/

        #region Key press event parse
        /***********************************************************************
         * Parse key press
         * --
         * Prosljedjivanje virtualnog pritiska tipke
         **********************************************************************/
        public void parse_answer_game_KY(string answer)
        {

            leds.chkLedStartTime = (answer[0] != '0');      // Ako odgovor nije nula, onda je true
            checks.VSTimerStarted = (answer[0] != '0');

            leds.chkLedEdit = (answer[1] != '0');
            /*if (answer[1] != '0')
            {
                checks.editCheck = true;
            }
            else checks.editCheck = false;*/

            leds.chkLedUpDown = (answer[2] != '0');

        }
        #endregion


        // Parses game time period
        public void parse_answer_game_TP(string answer)
        {
            //storage.parse_gameTime = answer.Substring(0, 2) + ":" + answer.Substring(2, 2) + "." + answer.Substring(4, 2);   // gameTime.Text   
            //if (storage.parse_gameMode!="FOOTBALL")
            //{
            string periodDir = storage.parse_periodDirection;

            if (answer.Substring(0, 2) == "00" && periodDir=="DN")
            {
                storage.parse_gameTime = answer.Substring(2, 2) + "." + answer.Substring(4, 2);
            }
            else if (periodDir == "UP")
            {
                storage.parse_gameTime = answer.Substring(0, 2) + ":" + answer.Substring(2, 2);   // gameTime.Text
            } else {
                storage.parse_gameTime = answer.Substring(0, 2) + ":" + answer.Substring(2, 2) + "." + answer.Substring(4, 2);
            }
                storage.parse_gamePeriod = answer.Substring(6, 1);      // gamePeriod.Text
            //}
        }

        #region Period time direction check
        /***********************************************************************
        * PD
        * ---
        * Prosljedjivanje smjera trajanja vremena u periodu
        ***********************************************************************/
        public void parse_answer_game_PD(string answer)
        {
            if (answer=="DN")
            {
                storage.parse_periodDirection = "DN";
            } else
            {
                storage.parse_periodDirection = "UP";
            }
        }
        #endregion

        #region Edit toggled
        /***********************************************************************
        * EC - Edit check
        * ---
        * Provjerava da li je pritisnuta edit tipka
        ***********************************************************************/
        public void parse_answer_game_EC(string answer)
        {
            if (answer == "1")
            {
                checks.editCheck = true;
            } else if (answer== "0")
            {
                checks.editCheck = false;
            }
        }
        #endregion

        #region Bonus parse
        /***********************************************************************
        * BO - Bonus parse
        * ---
        * Prosljedjivanje bonusa
        ***********************************************************************/
        public void parse_answer_game_B(string answer)              // dodane dvije metode za parsiranje bonusa i timeouta
        {                                                           // Added by Borna, 11.9.2018.
            int x, y;
            
            if (storage.parse_gameMode == "WATER_POLO" || storage.parse_gameMode == "BASKETBALL" ||
                storage.parse_gameMode == "VOLLEYBALL")
            {
                parse_answer_game_SR(answer);
            } else
            {
                Int32.TryParse((answer.Substring(0, 2)), out x);
                storage.parse_bonusHome = x;

                Int32.TryParse((answer.Substring(2, 2)), out y);
                storage.parse_bonusGuests = y;
            }
            
        }
        #endregion


        // Parse timeout value
        public void parse_answer_game_TO(string answer)
        {
            string GTO;
            string HTO;

            HTO = answer.Substring(0, 2);
            GTO = answer.Substring(2, 2);

            switch (GTO)
            {
                case "00":
                    storage.parse_timeOutGuests = 0;
                    leds.chkLedGTimeout1 = false;
                    leds.chkLedGTimeout2 = false;
                    leds.chkLedGTimeout3 = false;
                    break;
                case "01":
                    storage.parse_timeOutGuests = 1;
                    leds.chkLedGTimeout1 = true;
                    break;
                case "02":
                    storage.parse_timeOutGuests = 2;
                    leds.chkLedGTimeout2 = true;
                    break;
                case "03":
                    storage.parse_timeOutGuests = 3;
                    leds.chkLedGTimeout3 = true;
                    break;
                default:
                    storage.parse_timeOutGuests = 0;
                    leds.chkLedGTimeout1 = false;
                    leds.chkLedGTimeout2 = false;
                    leds.chkLedGTimeout3 = false;
                    break;
            }

            switch (HTO)
            {
                case "00":
                    storage.parse_timeOutHome = 0;
                    leds.chkLedHTimeout1 = false;
                    leds.chkLedHTimeout2 = false;
                    leds.chkLedHTimeout3 = false;
                    break;
                case "01":
                    storage.parse_timeOutHome = 1;
                    leds.chkLedHTimeout1 = true;
                    break;
                case "02":
                    storage.parse_timeOutHome = 2;
                    leds.chkLedHTimeout2 = true;
                    break;
                case "03":
                    storage.parse_timeOutHome = 3;
                    leds.chkLedHTimeout3 = true;
                    break;
                default:
                    storage.parse_timeOutHome = 0;
                    leds.chkLedHTimeout1 = false;
                    leds.chkLedHTimeout2 = false;
                    leds.chkLedHTimeout3 = false;
                    break;
            }


        }

            // Parse timeout time
        public void parse_answer_game_TOT(string answer)
        {

        }

        #region Result value parse
        /***********************************************************************
        * RS - Result parse
        * ---
        * Prosljedjivanje vrijednosti sveukupnog rezultata
        /**********************************************************************/
        public void parse_answer_game_RS(string answer)
        {
            int x, y;

            Int32.TryParse((answer.Substring(0, 3)), out x);
            storage.parse_scoreHome = x;

            Int32.TryParse((answer.Substring(3, 3)), out y);
            storage.parse_scoreGuests = y;

        }
        #endregion

        #region SET result (Volleyball)
        /***********************************************************************
        * SE - SET result
        * ---
        * Prosljedjivanje SET vrijednosti za Volleyball(!) mod.
        * SET vrijednosti prosljedjuju se u SCORE text box.
        /**********************************************************************/
        public void parse_answer_game_SE(string answer)
        {
            int x, y;

            Int32.TryParse((answer.Substring(0, 2)), out x);
            storage.parse_scoreHome = x;

            Int32.TryParse((answer.Substring(2, 2)), out y);
            storage.parse_scoreGuests = y;

        }
        #endregion

        #region Game mode parse
        /***********************************************************************
         * Parse game mode
         * --
         * Prosljedjivanje nacina igre
         **********************************************************************/
        public void parse_answer_game_GM(string answer)
        {

            string gameModeParser = answer.Substring(0, 3);

            switch (gameModeParser)
            {
                case "000":
                    storage.parse_gameMode = "FOOTBALL";
                    gamePlayers.allowedPPT = 23;    // 11 + max 12 subs
                    break;
                case "001":
                    storage.parse_gameMode = "BASKETBALL";
                    gamePlayers.allowedPPT = 12;    // 5 + max 7 subs
                    break;
                case "002":
                    storage.parse_gameMode = "FUTSAL";
                    gamePlayers.allowedPPT = 14;    // 5 + max 9 subs (7 in US)
                    break;
                case "003":
                    storage.parse_gameMode = "HANDBALL";
                    gamePlayers.allowedPPT = 14;    // 7 + max 7 subs
                    break;
                case "004":
                    storage.parse_gameMode = "HOCKEY";
                    gamePlayers.allowedPPT = 18;    // 6 + max 12 subs + 2 none-skaters (total of 20)
                    break;
                case "005":
                    storage.parse_gameMode = "VOLLEYBALL";
                    gamePlayers.allowedPPT = 12;    // 6 + max 6 subs
                    break;
                case "006":
                    storage.parse_gameMode = "WATER POLO";
                    gamePlayers.allowedPPT = 13;    // 7 + max 6 subs (4 subs for Olympic)
                    break;
                case "007":
                    storage.parse_gameMode = "NONE";
                    break;
                default:
                    break;
            }

        }
        #endregion

        #region Main timer sync check (T/F)
        /***********************************************************************
         * Parse timer synchronization value (true/false check)
         * --
         * Provjera da li je game timer pokrenut ili nije
         **********************************************************************/
        public void parse_answer_game_TM(string answer)
        {
            string answerParse = answer.Substring(0, 2);

            if (answerParse == "FA")
            {
                checks.MPLabTimerStarted = false;
            }
            else if (answerParse == "TR")
            {
                checks.MPLabTimerStarted = true;
            }
        }
        #endregion

        #region Ball possession check
        /***********************************************************************
         * Parse service check
         * --
         * Provjera posjeda lopte
         **********************************************************************/
        public void parse_answer_game_SR(string answer)
        {
            string answerParse = answer.Substring(0, 4);

            if (answerParse == "NONE")
            {
                storage.parse_ballPossession = "NONE";
            }
            else if (answerParse == "HOME")
            {
                storage.parse_ballPossession = "HOME";
            }
            else if (answerParse == "GSTS")
            {
                storage.parse_ballPossession = "GUESTS";
            }
            else { }
        }
        #endregion

        /***********************************************************************
         * Parse timeout timer synchronization value (true/false check)
         * --
         * Provjera da li je timeout timer pokrenuti ili ne
         **********************************************************************/
        public void parse_answer_game_TTC(string answer)
        {
            string answerParse = answer.Substring(0, 2);

            if (answerParse == "FA")
            {
                checks.MPLabTimeOutTimerStarted = false;
            }
            else if (answerParse == "TR")
            {
                checks.MPLabTimeOutTimerStarted = true;
            }

        }

        #region Attack time
        /***********************************************************************
         * Parse attack time current value
         * --
         * Prosljedjivanje vrijednosti vremena napada
         **********************************************************************/
        public void parse_answer_game_actionTime(string answer)
        {
            if (answer.Substring(0,1)=="0")
            {
                if (answer.Substring(1,1)=="0")
                {
                    storage.parse_actionTime = "--";
                } else
                {
                    storage.parse_actionTime = answer.Substring(1, 1);
                }
            } else
            {
                storage.parse_actionTime = answer.Substring(0, 2);
            }
        }
        #endregion

    }
}
