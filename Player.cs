using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using System;
using SampSharp.GameMode;
using System.Collections.Generic;

namespace partymode
{
    [PooledType]
    public class Player : BasePlayer
    {
        public Ability ability = null;
        public CustomSpectator spectator;
        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);
            GameMode.players.Add(this);
            SendClientMessage("Siema " + this.Name + ". Milej zabawy :)");
            SendClientMessage("Wpisz '/zasady' w chacie by poznac reguly danej rozgrywki.");
        }
        public override void OnSpawned(SpawnEventArgs e)
        {
            base.OnSpawned(e);
            GameMode.currentPlayMode.OverwriteSpawnBehaviour(this);
        }
        public override void OnDisconnected(DisconnectEventArgs e)
        {
            base.OnDisconnected(e);
            GameMode.players.Remove(this);
        }
        public override void OnDeath(DeathEventArgs e)
        {
            base.OnDeath(e);
            if (e.Killer != null) 
                GameMode.currentPlayMode.OverwriteKillBehaviour(this, e.Killer); 
            GameMode.currentPlayMode.OverwriteDeathBehaviour(this);
        }
        public void ClearPlayer()
        {
            ResetMoney();
            ResetWeapons();
            ToggleControllable(true);
            ToggleSpectating(false);
            if (ability!=null)
                ability.DetachFromPlayer(this);
            foreach (int i in Enum.GetValues(typeof(WeaponSkill)))
                SetSkillLevel((WeaponSkill)i, 200);
            PutCameraBehindPlayer();
        }
        public override void OnUpdate(PlayerUpdateEventArgs e)
        {
            base.OnUpdate(e);
            GameMode.currentPlayMode.OverwriteUpdateBehaviour(this);
            Abilities.GlobalUpdate(this);
        }
        public override void OnKeyStateChanged(KeyStateChangedEventArgs e)
        {
            base.OnKeyStateChanged(e);
            if (ability != null)
                ability.OverwriteKeyStateChangeEvent(e, this);
        }
        public bool InArea(Vector3 position, float area)
        {
            this.SendClientMessage(
                (position.X - this.Position.X).ToString("0.000") + " " +
                (position.Y - this.Position.Y).ToString("0.000") + " " +
                (position.Z - this.Position.Z).ToString("0.000"));
            if (Math.Abs(position.X - this.Position.X) < area && Math.Abs(position.Y - this.Position.Y) < area && Math.Abs(position.Z - this.Position.Z) < area)
                return true;
            return false;
        }
        public List<Player> GetCloseByPlayersList(float area)
        {
            List<Player> toRet = new List<Player>();
            foreach (var pl in GameMode.players)
            {
                if (pl.IsConnected && pl.IsAlive && pl.InArea(Position, area))
                {
                    toRet.Add(pl);
                }
            }
            return toRet;
        }

        [CommandGroup("control")]
        class ControlCommandGroup
        {
            [Command("load")]
            private static void LoadMap(BasePlayer sender, string mapName)
            {
                
                GameMode.gm.setCurrentModeByName(mapName);
            }
            [Command("begin")]
            private static void BeginMode(BasePlayer sender, int time)
            {
                GameMode.currentPlayMode.RequestBegin(GameMode.players, time);
            }
        }

        [CommandGroup("debug")]
        class DebugCommandGroup
        {
            [Command("pos")]
            private static void DebugPosition(BasePlayer sender)
            {
                Console.WriteLine(sender.Position);
            }
        }
    }
}