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
        public DerbyPM() : base("derby")
        { 
            scoreLimit = 2000;
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
          
        }

        protected override void OnStart(List<Player> players)
        {
            // Reward over time (points added over time)
            addAttribute(new OverTimeReward(4000, 10, (Player player) => { return (player.IsAlive && player.IsConnected && begin); }));
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
        }
        
    }
}
