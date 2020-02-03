using EPC232.communication;
using EPC232.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace EPC232.game.elements
{
    class TimerClass
    {


        ValueStorage storage;
        ValueChecks checks;
        GameStatusRefresh gsr;
        CommsParseMessage cpm;

        /***********************************************************************
         * Getters & setters
         **********************************************************************/

        public void setCpm(CommsParseMessage cpmIn)
        {
            this.cpm = cpmIn;
        }

        public CommsParseMessage getCpm()
        {
            return cpm;
        }

        public void setGsr(GameStatusRefresh gsrIn)
        {
            this.gsr = gsrIn;
        }

        public GameStatusRefresh getGsr()
        {
            return gsr;
        }

        public void setChecks(ValueChecks checksIn)
        {
            this.checks = checksIn;
        }

        public ValueChecks getChecks()
        {
            return checks;
        }

        public void setStorage(ValueStorage storageIn)
        {
            this.storage = storageIn;
        }

        public ValueStorage getStorage()
        {
            return storage;
        }

        /***********************************************************************
        * Timer thingies
        **********************************************************************/

        // Timer variables
        private DispatcherTimer timer;
        public DispatcherTimer OnShotTimer;
        private DispatcherTimer _onShotTimerGong;
        public Timer exitCounterTimer = new Timer();
        private int exitCounter = 0;
        public int exitCounterTimerValue { get { return this.exitCounter; } set { this.exitCounter = value; } }

        /*** METHODS ***/

        public void timer_Stop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= timer_Tick;
                checks.VSTimerStarted = false;
            }
        }

        public void timer_Run()
        {
            checks.VSTimerStarted = true;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);       // 1 bio na 4. mjestu, reprezentirao 1 sekundu
            timer.Tick += timer_Tick;
            timer.Start();
        }
        string lastgTime = "";

        // sinkronizacija u trenutku starta programa
        public void timer_Tick(object sender, EventArgs e)
        {
            if (checks.VSTimerStarted == true)
            {
                gsr.status_game_TP();
                gsr.status_game_AT();
                if (lastgTime == storage.parse_gameTime)         // Treba ubaciti provjeru sa gameTime.Text varijablom
                {
                    gsr.status_game_leds();
                }
                lastgTime = storage.parse_gameTime;
            }
        }

        public void OnShotTimer_Fire()
        {
            if (OnShotTimer == null)
            {
                OnShotTimer = new DispatcherTimer();
                OnShotTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);     // bila 1 sec
                OnShotTimer.Tick += OnShotTimer_Tick;
                OnShotTimer.Start();
            }
        }
        public void OnShotTimer_Tick(object sender, EventArgs e)
        {
            gsr.status_game_refresh_all();
            OnShotTimer.Stop();
            OnShotTimer.Tick -= OnShotTimer_Tick;
            OnShotTimer = null;
        }

        public void OnShotTimerGong_Fire()
        {
            if (_onShotTimerGong == null)
            {
                _onShotTimerGong = new DispatcherTimer();
                _onShotTimerGong.Interval = new TimeSpan(0, 0, 0, 2, 0);
                _onShotTimerGong.Tick += OnShotTimerGong_Tick;
                _onShotTimerGong.Start();
            }
        }
        public void OnShotTimerGong_Tick(object sender, EventArgs e)
        {
            cpm.SendMsg("IN=4", 0);
            _onShotTimerGong.Stop();
            _onShotTimerGong.Tick -= OnShotTimerGong_Tick;
            _onShotTimerGong = null;
        }
        public void OnShotTimerGong_Stop()
        {
            if (_onShotTimerGong != null)
            {
                _onShotTimerGong.Stop();
                _onShotTimerGong.Tick -= OnShotTimerGong_Tick;
                _onShotTimerGong = null;
            }

        }

        public void exitCounterCheck()
        {

            if (exitCounterTimerValue == 1)
            {
                exitCounterTimer.Start();
                if (exitCounterTimer.Interval == 1000)
                {
                    exitCounterTimer.Stop();
                    exitCounterTimerValue = 0;
                }
            }
            else if (exitCounterTimerValue >= 3 && exitCounterTimer.Interval <= 1000)
            {
                exitCounterTimer.Stop();
                exitCounterTimerValue = 0;
                checks.gameEntered = false;
            }

        }

    }
}
