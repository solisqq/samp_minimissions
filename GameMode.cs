using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Events;

namespace partymode
{
    public class GameMode : BaseMode
    {
        
        public static float version = 0.1f;
        public static List<Player> players = new List<Player>();
        public static List<PlayMode> playModes = new List<PlayMode>();
        public static PlayMode currentPlayMode;

        private static DerbyPM derbyMode = new DerbyPM();
        private static FreeRoamPM freeMode = new FreeRoamPM();
        private static HideSeekPlayMode hideSeek = new HideSeekPlayMode();
        public static GameMode gm;
        public static object SAMP { get; internal set; }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            createPClasses();
            gm = this;
            playModes.Add(derbyMode);
            playModes.Add(freeMode);
            Console.WriteLine("Party GameMode by SolisQQ. Version: " + version + "A.");
            SetGameModeText("Party mode by SolisQQ");
            EnableStuntBonusForAll(false);
            DisableInteriorEnterExits();
            setCurrentMode(hideSeek);
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
            if (currentPlayMode != null) currentPlayMode.Finish(players);
            currentPlayMode = mode;
            ResetGameWorldToDefault();
            mode.Start(players);
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