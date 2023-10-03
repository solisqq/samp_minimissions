using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using SampSharp.GameMode;
using System.Text;
using System.Timers;

namespace partymode
{
    class DerbyPM : PlayMode
    {
        Timer rewardTimer;
        //private Abilities.AbilitiesRandomSpawner abilitiesSpawner;
        public DerbyPM() : 
            base(
                "derby", 
                new CustomSpectator(new Vector3(2120.6001, -1816.6, 195.39999), 60, 5000),
                "Tryb rozgrywki: ~g~Derby ~w~~n~Zrzuć przeciwników z dachu.~n~Wygrywa ten który jak najdłużej utrzyma~n~ się w pojeździe na dachu!",
                new List<Vector3>
                {
                    new Vector3(2120.6001,-1816.6,195.39999),
                    new Vector3(2121.5,-1815.9,195.3),
                    new Vector3(2121.5,-1815.9,195.3),
                    new Vector3(2121.6001,-1813.5,195.3),
                    new Vector3(2120.6001,-1814.2,195.39999),
                    new Vector3(2120,-1815.1,195.3),
                    new Vector3(2121.5,-1814.9,195.3),
                    new Vector3(2121.3999,-1817.2,195.3),
                    new Vector3(2121.3999,-1817.2,195.3),
                    new Vector3(2121.5,-1812.6,195.3),
                    new Vector3(2119.2,-1812.5,195.3),
                    new Vector3(2119,-1817.1,195.3),
                    new Vector3(2119.2,-1815.6,195.3),
                    new Vector3(2119.3,-1813.7,195.3),
                    new Vector3(2120.8999,-1811.9,195.3),
                    new Vector3(2122.3999,-1814.5,195.3),
                }, 
                0, 0, 1) 
        { 
            rewardTimer = new Timer(1000);
            rewardTimer.Elapsed += HandleReward;
            scoreLimit = 2000;
            abilitiesSpawner.Setup(new[] {
                    (Ability)Abilities.jumpVAbility,
                    (Ability)Abilities.plantMineVAbility,
                    (Ability)Abilities.repairVAbility,
                    (Ability)Abilities.speedUpVAbility
                }, new[] {
                    new Vector3(2119.1191, -1781.5833, 195.30429),
                    new Vector3(2153.7998, -1792.4531, 190.85109),
                    new Vector3(2121.884, -1804.1135, 199.15529),
                    new Vector3(2108.6484, -1829.274, 194.30704),
                },
                8000, 8000);
        }

        private void HandleReward(object sender, ElapsedEventArgs e)
        {
            foreach(var player in GameMode.GetPlayers())
            {
                if(player.IsConnected && player.IsAlive && begin)
                {
                    if (player.InAnyVehicle) player.AddScore(7);
                    else player.AddScore(5);
                }
            }
        }
        public override void InitializeStatics()
        {
            CreateObject(3816, 2120.6001, -1805.1, 185.8, 0, 0, 0);
            CreateObject(1660, 2120.5, -1811.1, 193.8, 0, 0, 0);
            CreateObject(1632, 2155.1001, -1792.4, 189.8, 0, 0, 272);
            CreateObject(3434, 2119.7, -1779, 208.60001, 0, 0, 0);
            CreateObject(16442, 2089.3, -1795.5, 190.39999, 0, 0, 0);
            //CreateObject(1226, 2113.3, -1789.3, 201.10001, 0, 0, 0);
            int [] carsIds = new [] { 402, 542, 535, 541, 565, 494, 451, 601, 470, 602, 545, 560 };
            Random rand = new Random();
            CreateVehicle(carsIds[rand.Next(carsIds.Length)], 2089.5, -1830.8, 189.39999, 274);
            CreateVehicle(carsIds[rand.Next(carsIds.Length)], 2089.6001, -1808, 189.39999, 270);
            CreateVehicle(carsIds[rand.Next(carsIds.Length)], 2090.2, -1780.3, 189.60001, 270);
            CreateVehicle(carsIds[rand.Next(carsIds.Length)], 2089.8, -1788.8, 189.89999, 270);
            CreateVehicle(carsIds[rand.Next(carsIds.Length)], 2115.8999, -1776.5, 178.60001, 270);
            CreateVehicle(carsIds[rand.Next(carsIds.Length)], 2089.6001, -1801.3, 189.2, 268);
            CreateVehicle(carsIds[rand.Next(carsIds.Length)], 2115.3999, -1776.5, 179, 267.995);
            CreateVehicle(carsIds[rand.Next(carsIds.Length)], 2090, -1817.8, 189.89999, 270);
            CreateVehicle(carsIds[rand.Next(carsIds.Length)], 2120.1001, -1803.5, 198.89999, 0);
            CreateVehicle(carsIds[rand.Next(carsIds.Length)], 2151.8999, -1829.2, 189.10001, 0);
        }

        public override void OverwriteDeathBehaviour(Player player)
        {
            base.OverwriteDeathBehaviour(player);
            player.AddScore(-30);
            foreach (var closePlayer in player.GetCloseByPlayersList(10, true))
                if (closePlayer!=player)
                    closePlayer.AddScore(50);
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
            if (player.Position.Z < 180 && player.Position.Z > 130 && player.Health>0)
            {
                player.Health = 0;
            }
        }

        protected override void OnEnd(List<Player> players)
        {
            
            rewardTimer.Stop();
        }

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
            rewardTimer.Start();
        }
        
    }
}
