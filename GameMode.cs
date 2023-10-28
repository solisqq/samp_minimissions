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

namespace partymode
{
    public class GameMode : BaseMode
    {
        public static int maxPointsInPlayMode = 2000;
        public static string version = "0.2310.26";
        public static Dictionary<string, PlayMode> playModes = new Dictionary<string, PlayMode>();
        public static PlayMode currentPlayMode;
        public static GameMode gm;
        public static bool autoStart = true;
        static List<System.Timers.Timer> timers = new List<System.Timers.Timer>();
        public static object SAMP { get; internal set; }
        System.Timers.Timer reselectGameModeTimer = new System.Timers.Timer(15000);
        TCPMsg serverInfoMSG;

        DerbyPM derby = new DerbyPM();
        FreeRoamPM freeRoam = new FreeRoamPM();
        BattleRoyalePM battleRoyale = new BattleRoyalePM();
        HideSeekPlayMode hideAndSeek = new HideSeekPlayMode();
        LSLongRacePlayMode lsLongRace = new LSLongRacePlayMode();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            // Check if needed
            /*AddPlayerClass(1, new Vector3(1759.0189f, -1898.1260f, 13.5622f), 266.4503f);*/
            gm = this;
            Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": Party GameMode by SolisQQ. Version: " + version + "");
            SetGameModeText("Party mode by SolisQQ");
            EnableStuntBonusForAll(false);
            DisableInteriorEnterExits();
            if(autoStart) setCurrentMode();
            else setCurrentMode(freeRoam);
            foreach (var playMode in playModes)
            {
                playMode.Value.PlayModeFinished += currentPlayMode.PlayModeFinished += (p, e) =>
                {
                    if (autoStart)
                    {
                        reselectGameModeTimer.Start();
                        Player.GameTextForAll("~w~Wczytywanie kolejnej ~g~rozgrywki...", 15000, 6);
                    }
                };
            }
            serverInfoMSG = new TCPMsg("server_info", (JsonElement element) =>
            {
                return "players_online: \"" + GetPlayers().Count.ToString() + "\", current_mode: \"" + currentPlayMode.cmd + "\"";
            });
            reselectGameModeTimer.Elapsed += (p, e) => setCurrentMode();
            reselectGameModeTimer.AutoReset = false;
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