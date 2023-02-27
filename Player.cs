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
        public Ability vability = null;
        public Ability pability = null;
        public CustomSpectator spectator;
        private double internalScore=0;

        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);
            SendClientMessage("Siema " + this.Name + ". Milej zabawy :)");
            SendClientMessage("Wpisz '/zasady' w chacie by poznac reguly danej rozgrywki.");
        }
        public override void OnSpawned(SpawnEventArgs e)
        {
            base.OnSpawned(e);
            Money = (int)internalScore;
            Score = (int)internalScore;
            GameMode.currentPlayMode.OverwriteSpawnBehaviour(this);
        }
        public override void OnDeath(DeathEventArgs e)
        {
            base.OnDeath(e);
            ResetAbilities();
            if (e.Killer != null) 
                GameMode.currentPlayMode.OverwriteKillBehaviour(this, e.Killer); 
            GameMode.currentPlayMode.OverwriteDeathBehaviour(this);
        }
        private void ResetAbilities()
        {
            if (vability != null) vability.DetachFromPlayer(this);
            if (pability != null) pability.DetachFromPlayer(this);
        }
        public void ClearPlayer()
        {
            ResetWeapons();
            ToggleControllable(true);
            ToggleSpectating(false);
            ResetScore();
            ResetAbilities();
            Health = 100;
            foreach (int i in Enum.GetValues(typeof(WeaponSkill)))
                SetSkillLevel((WeaponSkill)i, 200);
            PutCameraBehindPlayer();
        }
        public void AddScore(double scoreToAdd)
        {
            internalScore = (internalScore + scoreToAdd);
            GiveMoney((int)scoreToAdd);
            Score = (int)internalScore;
            GameMode.currentPlayMode.PlayerScoreChanged(this, internalScore);
        }
        public void SetScore(double newScore)
        {
            internalScore = newScore;
            Money = (int)internalScore;
            Score = (int)newScore;
            GameMode.currentPlayMode.PlayerScoreChanged(this, internalScore);
        }
        public void ResetScore()
        {
            internalScore = 0;
            ResetMoney();
            this.Score = 0;
            GameMode.currentPlayMode.PlayerScoreChanged(this, internalScore);
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
            if (vability != null && InAnyVehicle) vability.OverwriteKeyStateChangeEvent(e, this);
            if (pability != null && !InAnyVehicle) pability.OverwriteKeyStateChangeEvent(e, this);
        }
        public bool InArea(Vector3 position, float area, bool ignoreZ = false)
        {
            if (Math.Abs(position.X - this.Position.X) < area && 
                Math.Abs(position.Y - this.Position.Y) < area && 
                ((Math.Abs(position.Z - this.Position.Z) < area) || ignoreZ))
                return true;
            return false;
        }
        public List<Player> GetCloseByPlayersList(float area, bool ignoreZ = false)
        {
            List<Player> toRet = new List<Player>();
            foreach (var pl in GameMode.GetPlayers())
            {
                if (pl.IsConnected && pl.IsAlive && pl.InArea(Position, area, ignoreZ))
                {
                    toRet.Add(pl);
                }
            }
            return toRet;
        }
        public Vector3 GetInFrontPosition(double distance, double highOffset)
        {
            return new Vector3(
                Position.X + distance * Math.Sin(Angle) * (180 / Math.PI),
                Position.Y + distance * Math.Cos(Angle) * (180 / Math.PI),
                Position.Z + highOffset);
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
                GameMode.currentPlayMode.RequestBegin(GameMode.GetPlayers(), time);
            }
        }

        [CommandGroup("a")]
        class AdminCommandGroup
        {
            [Command("spawn")]
            private static void LoadMap(BasePlayer sender, BasePlayer player, int vid)
            {
                BaseVehicle.Create((VehicleModelType)vid, ((Player)player).GetInFrontPosition(),);
                //GameMode.gm.setCurrentModeByName(mapName);
            }
            [Command("begin")]
            private static void BeginMode(BasePlayer sender, int time)
            {
                GameMode.currentPlayMode.RequestBegin(GameMode.GetPlayers(), time);
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