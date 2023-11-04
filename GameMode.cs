using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Events;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Linq;
using partymode.partymode;
using partymode.Widgets;

namespace partymode
{
    public class GameMode : BaseMode
    {
        public static int fastScoreMult = 1;
        public static int maxPointsInPlayMode = 2000;
        public static string version = "0.2310.30";
        public static Dictionary<string, PlayMode> playModes = new Dictionary<string, PlayMode>();
        public static PlayMode currentPlayMode;
        public static GameMode gm;
        public static bool autoStart = false;
        static List<System.Timers.Timer> timers = new List<System.Timers.Timer>();
        public static object SAMP { get; internal set; }
        System.Timers.Timer reselectGameModeTimer = new System.Timers.Timer(15000);
        TCPMsg serverInfoMSG;

        DerbyPM derby = new DerbyPM();
        FreeRoamPM freeRoam = new FreeRoamPM();
        BattleRoyalePM battleRoyale = new BattleRoyalePM();
        HideSeekPlayMode hideAndSeek = new HideSeekPlayMode();
        LSLongRacePlayMode lsLongRace = new LSLongRacePlayMode();

        TDialog msgDialog;
        TLabel msgDialogTitle;
        TLabel msgDialogLabel;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            gm = this;
            Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": Party GameMode by SolisQQ. Version: " + version + "");
            SetGameModeText("Party mode by SolisQQ");
            EnableStuntBonusForAll(false);
            DisableInteriorEnterExits();
            createMsgDialog();
            toggleAutoStart(true);
            if (autoStart) setCurrentMode();
            else setCurrentMode(hideAndSeek);
            
            serverInfoMSG = new TCPMsg("server_info", (JsonElement element) =>
            {
                return "players_online: \"" + GetPlayers().Count.ToString() + "\", current_mode: \"" + currentPlayMode.cmd + "\"";
            });
            reselectGameModeTimer.Elapsed += (p, e) => setCurrentMode();
            reselectGameModeTimer.AutoReset = false;
        }
        public static void toggleAutoStart(bool state)
        {
            autoStart = state;
            foreach (var pm in playModes)
            {
                if (autoStart)
                {
                    pm.Value.PlayModeFinished += handleAutoStart;
                    pm.Value.autoBegin = 20;
                }
                else
                {
                    pm.Value.PlayModeFinished -= handleAutoStart;
                    pm.Value.autoBegin = 0;
                }
            }
        }
        private static void handleAutoStart(object sender, EventArgs a)
        {
            gm.reselectGameModeTimer.Start();
            Player.GameTextForAll("~w~Wczytywanie kolejnej ~g~rozgrywki...", 15000, 6);
        }
        private void createMsgDialog()
        {
            msgDialog = new TDialog(new IGlobalTD(), new SampSharp.GameMode.Vector2(20, 100),
                TDialog.VerticalAlignment.Top, TDialog.HorizontalAlignment.Left, TLabel.DefaultColors.Background);
            msgDialogTitle = new TLabel(new IGlobalTD(), TLabel.DefaultTextStyles.PlayMode,
                new TLabel.ContentStyle(TextDrawAlignment.Left, 32), new Tuple<int, int, int, int>(2, 0, 2, 0), "");
            msgDialogLabel = new TLabel(new IGlobalTD(), TLabel.DefaultTextStyles.DefaultText,
                new TLabel.ContentStyle(TextDrawAlignment.Left, 48), new Tuple<int, int, int, int>(2, 0, 2, 0), "");
            msgDialog.addChild(msgDialogTitle);
            msgDialog.addChild(msgDialogLabel);
        }
        public static void globalMsg(string title, string data, int timeMS)
        {
            gm.msgDialogTitle.setText(title);
            gm.msgDialogLabel.setText(data);
            foreach(var player in GetPlayers())
                gm.msgDialog.show(player, Math.Min(30000, timeMS));
        }
        public bool setCurrentModeByName(String name)
        {
            if (!playModes.ContainsKey(name)) return false;
            setCurrentMode(playModes[name]);
            return true;
        }

        public bool setCurrentMode(PlayMode playMode = default(PlayMode))
        {
            if (playMode == default(PlayMode))
            {
                if (!autoStart) return false;
                Random rand = new Random();
                playMode = playModes.ElementAt(rand.Next(0, playModes.Count)).Value;
                while (!playMode.isAbleToStart() || playMode == currentPlayMode)
                    playMode = playModes.ElementAt(rand.Next(0, playModes.Count)).Value;
            }
            reselectGameModeTimer.Stop();
            
            if (currentPlayMode != null) currentPlayMode.Finish(GetPlayers());
            ResetGameWorldToDefault();
            currentPlayMode = playMode;
            currentPlayMode.Start(GetPlayers());
            return true;
        }

        private void ResetGameWorldToDefault()
        {
            Server.SetWeather(2);
            Server.SetWorldTime(11);
            ShowPlayerMarkers(PlayerMarkersMode.Global);
            ShowNameTags(true);
            SetNameTagDrawDistance(40.0f);
            UsePlayerPedAnimations();
        }

        public static List<Player> GetPlayers()
        {
            List<Player> playersToRet = new List<Player>();
            foreach (var bp in BasePlayer.All)
                if(bp.IsConnected)
                    playersToRet.Add((Player)bp);
            return playersToRet;
        }
        public static List<Player> GetPlayersSortedScore(List<Player> players = default(List<Player>))
        {
            if (players == default(List<Player>)) 
                players = GetPlayers();
            return players.OrderBy(player => player.Score).ToList();
        }
        public static int getPointsFromPlace(int place)
        {
            if (place == 0) return 1000;
            else if (place == 1) return 850;
            else if (place == 2) return 750;
            else if (place > 2 && place <= 8) return 600 - ((place-3) * 25);
            else if(place > 8 && place <= 16) return 450 - ((place-9) * 10);
            else if(place > 16 && place <= 32) return 370 - ((place-17)*4);
            else if(place > 32 && place <= 64) return 300 - ((place-33)*2);
            return 200;
        }
        public static void addTask(Action action, int delayMS)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = delayMS;
            timer.AutoReset = false;
            timer.Elapsed += (o, e) =>
            {
                action.Invoke();
                timers.Remove(timer);
                timer.Dispose();
            };
            timer.Start();
        }
    }
}