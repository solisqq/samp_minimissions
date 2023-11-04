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
            addAttribute(new OverTimeReward(3000, 12 * GameMode.fastScoreMult, (Player player) => { return (player.IsAlive && player.IsConnected && currentState == PlayModeState.BEGAN); }));
            addAttribute(new OverTimeReward(5000, 6 * GameMode.fastScoreMult, (Player player) => { return (player.IsAlive && player.IsConnected && currentState == PlayModeState.BEGAN && player.InAnyVehicle); }));
            addAttribute(new FreezTillBegin());
            var stopRule = new StopGameRules(this);
            stopRule.addRule(StopGameRules.StopRule.ScoreLimit, 2000);
            addAttribute(stopRule);
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
