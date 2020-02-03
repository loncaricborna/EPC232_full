using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

/*******************************************************************************
 * 
 * File: ValueStorage.cs
 * 
 * Authors: Borna Loncaric
 * 
 * Device name: EPC232
 * Ver: Full
 * 
 * Description:
 * 
 * Model klasa - služi za spremanje i prosljeđivanje vrijednosti iz uređaja
 * ka softveru i obratno, ovisno o upravljanju od strane korisnika.
 * 
 * Sastoji se samo od get i set metoda.
 * 
 ******************************************************************************/

namespace EPC232.model
{
    class ValueStorage
    {

        /*** Game time & mode variables ***/
        public string parse_gameMode { get; set; }
        public string parse_gameTime { get; set; }
        public string parse_gamePeriod { get; set; }
        public string parse_periodDirection { get; set; }
        public string parse_actionTime { get; set; }

        /*** Scoring system ***/

        // Timeout
        public int parse_timeOutLedCheck { get; set; }
        public int parse_timeOutGuests { get; set; }
        public int parse_timeOutHome { get; set; }
        public int parse_timeOutTime { get; set; }

        // Scores
        public int parse_scoreHome { get; set; }
        public int parse_scoreGuests { get; set; }

        // Bonus
        public int parse_bonusHome { get; set; }
        public int parse_bonusGuests { get; set; }

        // Ball possession
        public string parse_ballPossession { get; set; }


        /*** Program elements ***/

        // Setting & input tabs
        public char _settingsIndex { get; set; }
        public char _inputIndex { get; set; }

        // Slider values
        public string sliderName { get; set; }
        public int sliderValue { get; set; }

        // Network values
        public string hostName { get; set; }
        public int hostPort { get; set; }

    }

    class ValueChecks
    {

        public bool VSTimerStarted { get; set; }
        public bool MPLabTimerStarted { get; set; }
        public bool gameEntered { get; set; }
        public bool MPLabTimeOutTimerStarted { get; set; }
        public bool editCheck { get; set; }

    }

    class ValueLEDs
    {

        // Relay & connection LED
        private bool chkLedConn;
        private bool chkLedRly1;
        private bool chkLedRly2;
        public bool chkLedConnection { get { return this.chkLedConn; } set { this.chkLedConn = value; } }
        public bool chkLedRelay1 { get { return this.chkLedRly1; } set { this.chkLedRly1 = value; } }
        public bool chkLedRelay2 { get { return this.chkLedRly2; } set { this.chkLedRly2 = value; } }

        // Timeout LEDS

        // Home

        public bool chkLedHTimeout1 { get; set; }
        public bool chkLedHTimeout2 { get; set; }
        public bool chkLedHTimeout3 { get; set; }

        // Guests

        public bool chkLedGTimeout1 { get; set; }
        public bool chkLedGTimeout2 { get; set; }
        public bool chkLedGTimeout3 { get; set; }


        // Options & commands relevant LEDs

        public bool chkLedStartTime { get; set; }
        public bool chkLedUpDown { get; set; }
        public bool chkLedEdit { get; set; }

    }

    public class ValuePlayers
    {
        // Allowed players per team
        public int allowedPPT { get; set; }
        public bool isHomeListFull { get; set; }
        public bool isGuestsListFull { get; set; }
        public bool isPlayerNamesEnabled { get; set; }

        // Number will be dependant of the 'allowedPPT' variable
        public string[] plInfoHome { get; set; }
        public string[] plInfoGuests { get; set; }

        // Team name variables
        public string teamHomeName { get; set; }
        public string teamGuestsName { get; set; }
    }

}