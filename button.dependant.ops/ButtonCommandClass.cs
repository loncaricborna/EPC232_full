using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPC232.game.elements;
using EPC232.communication;
using EPC232.model;

/*******************************************************************************
 * 
 * File: ButtonCommandClass.cs
 * 
 * Authors: Borna Loncaric
 * 
 * Device name: EPC232
 * Ver: Full
 * 
 * Description:
 * 
 * Class regarding button press operations, mostly parsing through to the unit.
 * 
 ******************************************************************************/

namespace EPC232.button.dependant.ops
{
    class ButtonCommandClass
    {

        GameStatusRefresh gsr;
        CommsParseMessage cpm;
        ValueChecks checks;
        ValueStorage storage;

        TimerClass tc;

        #region Getters & setters
        /***********************************************************************
         * Getters & setters
         **********************************************************************/

        public void setGsr(GameStatusRefresh gsrIn)
        {
            this.gsr = gsrIn;
        }

        /***********************************************************************/

        public void setCpm(CommsParseMessage cpmIn)
        {
            this.cpm = cpmIn;
        }

        /***********************************************************************/

        public void setTc(TimerClass tcIn)
        {
            this.tc = tcIn;
        }

        /***********************************************************************/

        public void setChecks(ValueChecks checksIn)
        {
            this.checks = checksIn;
        }

        /***********************************************************************/

        public void setStorage(ValueStorage storageIn)
        {
            this.storage = storageIn;
        }
        #endregion

        /***********************************************************************
         * Button commands - GAME
         **********************************************************************/

        public void buttons_game_commands(string buttonName)
        {

            switch (buttonName)
            {
                // General operands
                case "EDIT":
                    cpm.SendMsg("KY=ED", 0);
                    gsr.status_edit_check();
                    gsr.status_game_leds();
                    break;
                case "ENTER":
                    cpm.SendMsg("KY=EN", 0);
                    gsr.status_game_refresh_all();
                    checks.gameEntered = true;
                    break;
                case "EXIT":
                    cpm.SendMsg("KY=EX", 0);
                    cpm.SendMsg("KY=EX", 0);
                    cpm.SendMsg("KY=EX", 0);
                    tc.OnShotTimer_Fire();
                    //tc.exitCounterTimerValue++;
                    tc.exitCounterCheck();
                    break;
                // NO LONGER USED
                /*case "LOAD":
                    cpm.SendMsg("KY=EX", 0);
                    cpm.SendMsg("KY=EX", 0);
                    cpm.SendMsg("KY=EX", 0);
                    tc.OnShotTimer_Fire();
                    checks.gameEntered = false;
                    break;*/

                // Arrow operands
                case "AROW_UP":
                    cpm.SendMsg("KY=AU", 0);
                    gsr.status_game_TP();
                    break;
                case "AROW_DN":
                    cpm.SendMsg("KY=AD", 0);
                    gsr.status_game_TP();
                    break;
                case "AROW_LT":                         // dodane ARROW LEFT i ARROW RIGHT tipke s upitom za mod igre
                    cpm.SendMsg("KY=AL", 0);                // By Borna, 18.9.2018.
                    gsr.status_game_mode();
                    gsr.status_game_TP();
                    break;
                case "AROW_RT":
                    cpm.SendMsg("KY=AR", 0);
                    gsr.status_game_mode();
                    gsr.status_game_TP();
                    break;

                #region Game scoring operands
                // Game scoring operands
                case "HSCORE":
                    cpm.SendMsg("KY=HO", 0);
                    gsr.status_game_R();
                    break;
                case "GSCORE":
                    cpm.SendMsg("KY=GO", 0);
                    gsr.status_game_R();
                    break;
                case "HTIMEOUT":                        // Dodane timeout i bonus tipke - 11.9.2018., by Borna
                    cpm.SendMsg("KY=HT", 0);
                    gsr.status_game_TO();
                    break;
                case "GTIMEOUT":
                    cpm.SendMsg("KY=GT", 0);
                    gsr.status_game_TO();
                    break;
                case "HBONUS":
                    cpm.SendMsg("KY=HB", 0);
                    gsr.status_game_BO();
                    break;
                case "GBONUS":
                    cpm.SendMsg("KY=GB", 0);
                    gsr.status_game_BO();
                    break;
                case "HFOUL":                           // Added by Borna, 12.4.2019.
                    if (storage.parse_gameMode == "VOLLEYBALL")
                    {
                        cpm.SendMsg("KY=HS", 0);
                    } else
                    {
                        cpm.SendMsg("KY=HF", 0);
                    }                
                    break;
                case "GFOUL":
                    if (storage.parse_gameMode == "VOLLEYBALL")
                    {
                        cpm.SendMsg("KY=GS", 0);
                    }
                    else
                    {
                        cpm.SendMsg("KY=GF", 0);
                    }
                    break;

                #region Basketball +2 and +3 score values
                case "HSCORE_2":
                    cpm.SendMsg("KY=HO", 0);
                    cpm.SendMsg("KY=HO", 0);
                    gsr.status_game_R();
                    break;
                case "GSCORE_2":
                    cpm.SendMsg("KY=GO", 0);
                    cpm.SendMsg("KY=GO", 0);
                    gsr.status_game_R();
                    break;
                case "HSCORE_3":
                    cpm.SendMsg("KY=HO", 0);
                    cpm.SendMsg("KY=HO", 0);
                    cpm.SendMsg("KY=HO", 0);
                    gsr.status_game_R();
                    break;
                case "GSCORE_3":
                    cpm.SendMsg("KY=GO", 0);
                    cpm.SendMsg("KY=GO", 0);
                    cpm.SendMsg("KY=GO", 0);
                    gsr.status_game_R();
                    break;
                #endregion
                #endregion

                // Game settings operands
                case "START":
                    cpm.SendMsg("KY=ST", 0);
                    gsr.status_game_PD();
                    gsr.status_timer();             // Added by Borna, 14.12.2018.
                    gsr.status_game_leds();
                    break;
                case "PERIOD":
                    cpm.SendMsg("KY=PR", 0);
                    gsr.status_game_PD();
                    gsr.status_game_TP();
                    break;
                case "UPDOWN":
                    cpm.SendMsg("KY=UD", 0);
                    gsr.status_game_leds();
                    break;
                case "MINUITS":
                    cpm.SendMsg("KY=EM", 0);
                    gsr.status_game_TP();
                    break;
                case "SECONDS":
                    cpm.SendMsg("KY=ES", 0);
                    gsr.status_game_TP();
                    break;
                case "ALARM":
                    cpm.SendMsg("KY=AM", 0);
                    break;

                #region Action time operands
                // Action time operands             // Added on 2.8.2019.
                case "startStopAT":
                    cpm.SendMsg("KY=3S", 0);
                    gsr.status_game_AT();
                    break;
                case "resetAT":
                    cpm.SendMsg("KY=3R", 0);
                    gsr.status_game_AT();
                    break;
                case "resetAT14":
                    cpm.SendMsg("KY=34", 0);
                    gsr.status_game_AT();
                    break;

                #endregion
            }
        }
    }
}
