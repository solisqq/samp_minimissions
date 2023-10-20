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
        public static string version = "0.2310.10";
        public static Dictionary<string, PlayMode> playModes = new Dictionary<string, PlayMode>();
        public static PlayMode currentPlayMode;
        public static GameMode gm;
        public static object SAMP { get; internal set; }
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
            setCurrentModeByName("freeroam");
            serverInfoMSG = new TCPMsg("server_info", (JsonElement element) =>
            {
                return "players_online: \"" + GetPlayers().Count.ToString() + "\", current_mode: \"" + currentPlayMode.cmd + "\"";
            });
        }

        public bool setCurrentModeByName(String name)
        {
            if(!playModes.ContainsKey(name)) return false;
            if (currentPlayMode != null) currentPlayMode.Finish(GetPlayers());
            currentPlayMode = playModes[name];
            ResetGameWorldToDefault();
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
                playersToRet.Add((Player)bp);
            return playersToRet;
        }
    }
}