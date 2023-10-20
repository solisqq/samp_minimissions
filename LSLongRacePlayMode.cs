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
                "racels")/*,
                new CustomSpectator(new Vector3(2494.8, -1665.3, 13.3), 60, 5000),
                "Tryb rozgrywki: ~g~Wyscig LS~w~~n~Wystartuj i ukoncz pierwszy/a.~n~Mozesz zmieniac pojazdy lub nie~n~- Twoja decyzja!",
                new List<Vector3>
                {
                    new Vector3(2494.8,-1665.3,13.3),
                    new Vector3(2496.1001,-1665.8,13.3),
                    new Vector3(2496.1001,-1667,13.3),
                    new Vector3(2495.8999,-1668.8,13.3),
                    new Vector3(2495.8999,-1668.8,13.3),
                    new Vector3(2495.8999,-1668.8,13.3),
                    new Vector3(2494.1001,-1670.6,13.3),
                    new Vector3(2491.5,-1669,13.3),
                    new Vector3(2493.3999,-1668.4,13.3),
                    new Vector3(2493.7,-1666.7,13.3),
                    new Vector3(2491.5,-1667.1,13.3),
                    new Vector3(2491.8999,-1665.4,13.3),
                    new Vector3(2489.2,-1665.4,13.3),
                    new Vector3(2488.7,-1666.6,13.3),
                    new Vector3(2488.6001,-1669.9,13.3),
                    new Vector3(2491.8999,-1671.3,13.3),
                    new Vector3(2488,-1668.2,13.3),
                    new Vector3(2490.8,-1664,13.3),
                    new Vector3(2494.3,-1663.1,13.3),
                    new Vector3(2497.1001,-1671,13.3),
                    new Vector3(2497.1001,-1671,13.3),
                    new Vector3(2487.8,-1672.4,13.3),
                    new Vector3(2494.3,-1672.9,13.3),
                    new Vector3(2485.5,-1668.9,13.3),
                    new Vector3(2485.6001,-1665.8,13.3),
                    new Vector3(2488,-1663.8,13.3),
                    new Vector3(2498.8999,-1668.9,13.3),
                    new Vector3(2518.3,-1656.1,15.3),
                }, 
                0, 0, 1)*/
        {
            race = new Race(new List<Vector3> {
                new Vector3(2464.6001,-1658.8,13.3),
                new Vector3(2399.6001,-1658.8,13.4),
                new Vector3(2342.7,-1614.7,20.3),
                new Vector3(2342.7,-1421.4,23.8),
                new Vector3(2320.7,-1386.9,23.9),
                new Vector3(2305.6001,-1327.6,23.8),
                new Vector3(2303.5,-1178,25.8),
                new Vector3(2287.5,-1128.3,26.8),
                new Vector3(2286.8999,-987.5,26.7),
                new Vector3(2279.6001,-823.5,31),
                new Vector3(2164.3,-668.20001,51.6),
                new Vector3(2033.8,-546.5,78.9),
                new Vector3(2040.4,-478.5,73.3),
                new Vector3(2034.7,-420.70001,76.4),
                new Vector3(2120.3,-381.10001,66.5),
                new Vector3(2345.2,-390.60001,67.9),
                new Vector3(2533.5,-425.60001,79.5),
                new Vector3(2396.2,-647.20001,127.2),
                new Vector3(2506.5,-842.79999,92),
                new Vector3(2504.3,-956.70001,82.3),
                new Vector3(2427.3999,-971.70001,78.5),
                new Vector3(2338.6001,-1036.3,52.9),
                new Vector3(2326.2,-1077.5,50),
                new Vector3(2228.6001,-1027,58.6),
                new Vector3(2164,-1024.7,62.4),
                new Vector3(2210.8999,-1053.2,69.8),
                new Vector3(2196.8,-1058.5,75.2),
                new Vector3(2130.3999,-1026.6,73),
                new Vector3(1918.7,-1022.7,34.7),
                new Vector3(1836.7,-1028.5,36.1),
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
                new Vector3(1490.6,-2220.3,33.8)
            });
        }

        public override void InitializeStatics()
        {
            CreateVehicle(549, 2501.5, -1680.1, 13.2, 0);
            CreateVehicle(549, 2497.8999, -1679.9, 13.2, 0);
            CreateVehicle(475, 2490.7, -1680.3, 13.2, 0);
            CreateVehicle(475, 2494.2002, -1680.2998, 13.2, 0);
            CreateVehicle(412, 2483.5, -1680.2, 13.3, 0);
            CreateVehicle(412, 2486.7002, -1680.4004, 13.3, 0);
            CreateVehicle(419, 2504.2, -1674.4, 13.3, 88);
            CreateVehicle(419, 2504.2, -1671.7, 13.3, 88);
            CreateVehicle(534, 2504.6001, -1668.8, 13.2, 88);
            CreateVehicle(534, 2504.6001, -1665.8, 13.2, 88);
            CreateVehicle(535, 2498, -1658.1, 13.2, 170);
            CreateVehicle(535, 2494.8999, -1657.8, 13.2, 170);
            CreateVehicle(536, 2491.2, -1657.9, 13.2, 170);
            CreateVehicle(536, 2487.8999, -1657.6, 13.2, 170);
            CreateVehicle(567, 2484.3999, -1657.1, 13.3, 170);
            CreateVehicle(567, 2480.7, -1656.8, 13.3, 170);
            CreateVehicle(576, 2480.1001, -1679.5, 13.1, 0);
            CreateVehicle(445, 2503.8, -1662.9, 13.4, 88);
            CreateVehicle(445, 2503.8, -1659.8, 13.4, 90);
            CreateVehicle(424, 2056.7, -484.70001, 72.9, 0);
            CreateVehicle(424, 2054.1001, -484.79999, 72.5, 0);
            CreateVehicle(424, 2050.3999, -485.60001, 72.4, 0);
            CreateVehicle(424, 2046.5996, -487.59961, 72.2, 0);
            CreateVehicle(424, 2057.1006, -488.90039, 72.2, 0);
            CreateVehicle(424, 2053.6006, -491.2998, 72.2, 0);
            CreateVehicle(424, 2049.2, -493.39999, 72.3, 0);
            CreateVehicle(424, 2005.2998, -508, 72.8, 0);
            CreateVehicle(444, 2005.7, -509.39999, 72.1, 0);
            CreateVehicle(568, 2055.3999, -480.29999, 74.1, 0);
            CreateVehicle(568, 2050.3999, -480.70001, 73.3, 0);
            CreateVehicle(568, 2046.2002, -481.90039, 72.8, 0);
            CreateVehicle(568, 2050.8999, -500.60001, 73.6, 0);
            CreateVehicle(568, 2058.8999, -495.60001, 72.8, 0);
            CreateVehicle(568, 2044.8, -504.5, 74.1, 0);
            CreateVehicle(468, 2055, -468.70001, 77.1, 0);
            CreateVehicle(468, 2053.5, -469.10001, 76.7, 0);
            CreateVehicle(468, 2051.6001, -469.29999, 76.4, 0);
            CreateVehicle(468, 2049.1001, -469, 76.1, 0);
            CreateVehicle(468, 2047.1, -469.29999, 75.8, 0);
            CreateVehicle(468, 2044.8, -469.39999, 75.4, 0);
            CreateVehicle(468, 2046.4, -465.10001, 76.7, 0);
            CreateVehicle(468, 2049.8999, -464.79999, 77.2, 0);
            CreateVehicle(468, 2052.8999, -464.29999, 77.7, 0);
            CreateVehicle(468, 2047.3, -473.20001, 74.8, 0);
            CreateVehicle(468, 2052.1006, -473.2998, 75.4, 0);
            CreateVehicle(468, 2049, -476.70001, 74.1, 0);
            CreateVehicle(411, 1790, -1042.3, 40.3, 0);
            CreateVehicle(415, 1795.2, -1041, 39.7, 0);
            CreateVehicle(415, 1801.7, -1039.9, 39, 0);
            CreateVehicle(415, 1807.7002, -1038.7002, 38.4, 0);
            CreateVehicle(429, 1785, -1036.3, 40.5, 0);
            CreateVehicle(429, 1793.4, -1036.8, 39.6, 280);
            CreateVehicle(429, 1792.7, -1034.3, 39.6, 279.998);
            CreateVehicle(429, 1801.5, -1034.3, 38.8, 279.998);
            CreateVehicle(451, 1812.9, -1037.2, 37.8, 320);
            CreateVehicle(451, 1817.5, -1036.4, 37.4, 319.999);
            CreateVehicle(451, 1807.2, -1032.7, 38.2, 247.999);
            CreateVehicle(451, 1813.1, -1031.5, 37.7, 247.994);
            CreateVehicle(451, 1818.9, -1030, 37.2, 247.994);
            CreateVehicle(477, 1782.8, -1043.4, 41.2, 320);
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
            if(!begin)
            {
                player.ToggleControllable(false);
                return false;
            }
            return true;
        }
        public override void OverwriteUpdateBehaviour(Player player)
        {
            base.OverwriteUpdateBehaviour(player);
        }

        protected override void OnEnd(List<Player> players){}

        protected override void OnStart(List<Player> players)
        {
            foreach (var player in players)
            {
                player.ToggleControllable(false);
            }
        }
        protected override void Begin(List<Player> players) {
            foreach(var player in players)
            {
                player.ToggleControllable(true);
            }
            race.Begin(GameMode.GetPlayers());
        }
        
    }
}
