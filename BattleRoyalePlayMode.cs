using System;
using System.Collections.Generic;
using System.Text;

namespace partymode
{
    using SampSharp.GameMode.World;
    using System;
    using System.Collections.Generic;
    using SampSharp.GameMode;
    using System.Text;
    using System.Timers;

    namespace partymode
    {
        class BattleRoyalePM : PlayMode
        {
            public BattleRoyalePM() : base("battle")
            {

            }

            public override void InitializeStatics()
            {
            }

            public override void OverwriteDeathBehaviour(Player player)
            {

            }

            public override void OverwriteKillBehaviour(Player killed, BasePlayer killer)
            {
                killer.Score += 50;
            }

            public override bool OverwriteSpawnBehaviour(Player player)
            {
                base.OverwriteSpawnBehaviour(player);
                player.GiveWeapon(SampSharp.GameMode.Definitions.Weapon.Parachute, 1);
                if (currentState!=PlayModeState.BEGAN)
                {
                    player.ToggleControllable(false);
                    return false;
                }
                return true;
            }
            public override void OverwriteUpdateBehaviour(Player player)
            {
            }

            protected override void OnEnd(List<Player> players)
            {
            }

            protected override void OnStart(List<Player> players)
            {
                foreach (var player in players)
                {
                    player.ToggleControllable(false);
                }
            }
            protected override void Begin(List<Player> players)
            {
                foreach (var player in players)
                {
                    player.ToggleControllable(true);
                }
            }

        }
    }

}
