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
        public DerbyPM() : base("derby")
        { 
            rewardTimer = new Timer(1000);
            rewardTimer.Elapsed += HandleReward;
            scoreLimit = 2000;
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
