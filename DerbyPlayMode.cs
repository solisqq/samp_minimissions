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
            addAttribute(new OverTimeReward(3000, 300, (Player player) => { return (player.IsAlive && player.IsConnected && currentState == PlayModeState.BEGAN); }));
            addAttribute(new FreezTillBegin());
        }
        protected override void Begin(List<Player> players) {}
        public override bool isAbleToStart()
        {
            if (GameMode.GetPlayers().Count > 0)
                return true;
            return false;
        }
    }
}
