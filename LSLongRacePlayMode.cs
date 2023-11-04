using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using SampSharp.GameMode;
using System.Text;
using System.Timers;

namespace partymode
{
    class LSLongRacePlayMode : PlayMode
    {
        Race race;
        //private Abilities.AbilitiesRandomSpawner abilitiesSpawner;
        public LSLongRacePlayMode() : 
            base(
                "lsrace")
        {
            
        }

        public override void InitializeStatics()
        {

        }

        public override void OverwriteDeathBehaviour(Player player)
        {
            base.OverwriteDeathBehaviour(player);
        }

        public override void OverwriteKillBehaviour(Player killed, BasePlayer killer)
        {
            base.OverwriteKillBehaviour(killed, killer);
        }

        public override bool OverwriteSpawnBehaviour(Player player)
        {
            base.OverwriteSpawnBehaviour(player);
            return true;
        }
        public override void OverwriteUpdateBehaviour(Player player)
        {
            base.OverwriteUpdateBehaviour(player);
        }

        protected override void OnEnd(List<Player> players){}

        protected override void OnStart(List<Player> players)
        {
            var raceData = new List<Vector3> {
                new Vector3(2464.6001,-1658.8,13.3),
                new Vector3(2399.6001,-1658.8,13.4),
                new Vector3(2342.7,-1614.7,20.3),
                new Vector3(2342.7,-1421.4,23.8),
                new Vector3(2320.7,-1386.9,23.9),//4
                new Vector3(2305.6001,-1327.6,23.8),
                new Vector3(2303.5,-1178,25.8),
                new Vector3(2287.5,-1128.3,26.8),
                new Vector3(2286.8999,-987.5,26.7),
                new Vector3(2279.6001,-823.5,31),
                new Vector3(2164.3,-668.20001,51.6),//10
                new Vector3(2033.8,-546.5,78.9),
                new Vector3(2040.4,-478.5,73.3),
                new Vector3(2034.7,-420.70001,76.4),
                new Vector3(2120.3,-381.10001,66.5),
                new Vector3(2345.2,-390.60001,67.9),//15
                new Vector3(2533.5,-425.60001,79.5),
                new Vector3(2396.2,-647.20001,127.2),
                new Vector3(2506.5,-842.79999,92),
                new Vector3(2504.3,-956.70001,82.3),
                new Vector3(2427.3999,-971.70001,78.5),//20
                new Vector3(2338.6001,-1036.3,52.9),
                new Vector3(2326.2,-1077.5,50),
                new Vector3(2228.6001,-1027,58.6),
                new Vector3(2102.9321,-988.2875,53.900448),
                new Vector3(1957.6616,-1009.8463,35.994396),
                new Vector3(1918.7,-1022.7,34.7),
                new Vector3(1876.6566,-1016.6975,36.1),
                new Vector3(1822.9,-985,37.7),
                new Vector3(1729.8,-958.29999,48.2),
                new Vector3(1670.1,-984.40002,53),
                new Vector3(1610.2,-1156.9,56.5),
                new Vector3(1596,-1371.5,28.6),
                new Vector3(1593.8,-1667.8,28.5),
                new Vector3(1651,-1960.2,23.8),
                new Vector3(1565.9,-2103.5,16.3),
                new Vector3(1361.6,-2155,13.4),
                new Vector3(1316.3,-2257.3,13.4),
                new Vector3(1289.8,-2195.3,21.5),
                new Vector3(1400.5,-2193,18.9),
                new Vector3(1399,-2233.3,13.5),
                new Vector3(1470.9,-2220.3999,13.4),
                new Vector3(1490.6,-2220.3,13.8)
            };
            addAttribute(new FreezTillBegin());
            var race = new Race(raceData);
            addAttribute(race);
            var finishRules = new StopGameRules(this);
            finishRules.addRule(StopGameRules.StopRule.AllFinished, 1);
            // 6 minutes
            finishRules.addRule(StopGameRules.StopRule.TimeLimit, (60000*6) / GameMode.fastScoreMult);
            addAttribute(finishRules);
            race.Finished += Race_Finished;
            foreach (var player in players)
            {
                player.ToggleControllable(false);
            }
        }

        private void Race_Finished(object sender, EventArgs e)
        {
            var player = sender as Player;
            if (player != null && player.IsConnected)
            {
                onPlayerFinished(player);
            }
        }

        /*        protected override void Begin(List<Player> players) {
           foreach(var player in players)
           {
               player.ToggleControllable(true);
           }
           race.Begin(GameMode.GetPlayers());
       }*/
        public override void OverwriteJoinBehaviour(bool begin, Player player)
        {
        }
        public override bool isAbleToStart()
        {
            if (GameMode.GetPlayers().Count > 0)
                return true;
            return false;
        }
    }
}
