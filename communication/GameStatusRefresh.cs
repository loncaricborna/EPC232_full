using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPC232.communication;
using EPC232.model;

/*******************************************************************************
 * 
 * File: GameStatusRefresh.cs
 * 
 * Authors: Borna Loncaric
 * 
 * Device name: EPC232
 * Ver: Full
 * 
 * Description:
 * 
 * Klasa mrežnog prosljeđivanja upita uređaju.
 * 
 ******************************************************************************/

namespace EPC232
{
    class GameStatusRefresh
    {

        CommsParseMessage cpm;
        ValueStorage storage;

        /***********************************************************************
         * Getters & setters
         **********************************************************************/

        public CommsParseMessage getCpm()
        {
            return cpm;
        }

        public void setCpm(CommsParseMessage cpm)
        {
            this.cpm = cpm;
        }

        public void setStorage(ValueStorage storage)
        {
            this.storage = storage;
        }
        
        /**********************************************************************/

        public void status_refresh_all()
        {
            status_relay_leds();
            status_game_refresh_all();

        }
        public void status_game_refresh_all()
        {
            status_game_mode();
            status_game_leds();
            status_game_PD();
            status_game_TP();                     
            status_game_R();
            status_game_BO();
            status_game_TO();
            status_game_SR();
            status_timer();
            status_edit_check();
            if (storage.parse_gameMode == "BASKETBALL" || storage.parse_gameMode == "WATER POLO")
            {
                status_game_AT();
            } else
            {
                storage.parse_actionTime = " ";
            }
        }
            // Relay check inquiry
        public void status_relay_leds()
        {
            cpm.SendMsg("R?=", 0);
        }
            // "LED" toggle circles in the GUI
        public void status_game_leds()
        {
            cpm.SendMsg("G?=KY", 0);
        }

            // Time and period status message
        public void status_game_TP()
        {
            cpm.SendMsg("G?=TP", 0);
        }

            // Result
        public void status_game_R()
        {
            cpm.SendMsg("G?=RS", 0);
        }

            // Timeout - count
        public void status_game_TO()       // timeout send query
        {
            cpm.SendMsg("G?=TO", 0);
        }

            // Bonus - count
        public void status_game_BO()       // bonus send query
        {
            cpm.SendMsg("G?=BO", 0);
        }

            // Period direction (up/down)
        public void status_game_PD()
        {
            cpm.SendMsg("G?=PD", 0);
        }

            // Service & ball possession - UNUSED
        public void status_game_SR()
        {
            cpm.SendMsg("G?=SR", 0);        // service/ball possession query
        }

            // Game mode
        public void status_game_mode()
        {
            cpm.SendMsg("G?=GM", 0);            // game send query
        }

            // Game time status
            // Added by Borna, 14.12.2018.
        public void status_timer()
        {
            cpm.SendMsg("G?=TM", 0);
        }

            // Game edit status(toggled yes/no)
        public void status_edit_check()
        {
            cpm.SendMsg("G?=EC", 0);
        }

            // Attack time status
        public void status_game_AT()
        {
            cpm.SendMsg("G?=AT", 0);
        }
        
    }
}
