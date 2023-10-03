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

namespace partymode
{
    public class GameMode : BaseMode
    {
        
        public static string version = "0.2310.02";
        public static List<PlayMode> playModes = new List<PlayMode>();
        public static PlayMode currentPlayMode;
        private static DerbyPM derbyMode = new DerbyPM();
        private static FreeRoamPM freeMode = new FreeRoamPM();
        private static HideSeekPlayMode hideSeek = new HideSeekPlayMode();
        private static LSLongRacePlayMode lsraceMode = new LSLongRacePlayMode();
        public static GameMode gm;
        public static object SAMP { get; internal set; }

        TCPMsg serverInfoMSG; 

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            createPClasses();
            gm = this;
            playModes.Add(derbyMode);
            playModes.Add(freeMode);
            playModes.Add(hideSeek);
            playModes.Add(lsraceMode);
            Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": Party GameMode by SolisQQ. Version: " + version+"");
            SetGameModeText("Party mode by SolisQQ");
            EnableStuntBonusForAll(false);
            DisableInteriorEnterExits();
            setCurrentMode(lsraceMode);
            serverInfoMSG = new TCPMsg("server_info", getServerInfoJSON);
        }
        private string getServerInfoJSON(JsonElement element)
        {
            return "players_online: \"" + GetPlayers().Count.ToString()+ "\", current_mode: \"" + currentPlayMode.cmd+"\"";
        }
        private void createPClasses()
        {
            int [] skins = { 
                1, 2, 3, 6, 7, 8, 10, 11, 
                12, 15, 16, 21, 23, 26, 27, 29, 
                31, 32, 33, 34, 35, 40, 41, 43, 
                44, 49, 53, 54, 55, 59, 60, 62, 
                64, 72, 73, 75, 76, 78, 79, 83, 
                85, 91, 93, 98, 100, 101, 127, 128, 
                129, 131, 137, 141, 142, 150, 151, 155, 
                156, 160, 161, 162, 169, 191, 192, 193, 
                198, 203, 209, 212, 213, 214, 215, 216, 
                217, 249, 250, 264, 277, 278 };

            foreach (int skin in skins)
            {
                AddPlayerClass(skin, new Vector3(1759.0189f, -1898.1260f, 13.5622f), 266.4503f);
            }
        }
        private void setCurrentMode(PlayMode mode)
        {
            if (currentPlayMode != null) currentPlayMode.Finish(GetPlayers());
            currentPlayMode = mode;
            ResetGameWorldToDefault();
            mode.Start(GetPlayers());
        }
        public bool setCurrentModeByName(String name)
        {
            var mode = playModes.Find(m => m.cmd == name);
            if (mode == null) return false;
            setCurrentMode(mode);
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
                playersToRet.Add((Player)bp);
            return playersToRet;
        }
    }
}