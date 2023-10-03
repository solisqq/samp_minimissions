using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using System;
using SampSharp.GameMode;
using System.Collections.Generic;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;

namespace partymode
{
    [PooledType]
    public class Player : BasePlayer
    {
        public class CountDown
        {
            private int currentCountdown = 0;
            private string countDownFinishText = "";
            private System.Timers.Timer timer = new System.Timers.Timer(1000);
            private Action finishAction = null;
            Player player = null;
            public CountDown(Player player) {
                this.player = player;
                timer.Elapsed += PlayerCountdownTimer_Elapsed;
            }
            public void Setup(int time, string textOnFinish, Action onFinish=null)
            {
                finishAction = onFinish;
                if (time <= 0) {
                    HandleFinish();
                    return;
                }
                currentCountdown = time;
                player.GameText("~r~" + currentCountdown.ToString(), 999, 6);
                timer.Start();
            }
            private void HandleFinish()
            {
                if(finishAction!=null) finishAction.Invoke();
                timer.Stop();
                player.GameText(countDownFinishText, 999, 6);
                finishAction = null;
                countDownFinishText = "";
                currentCountdown = 0;
            }
            private void PlayerCountdownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                currentCountdown -= 1;
                if (currentCountdown > 0) player.GameText("~r~" + currentCountdown.ToString(), 999, 6);
                else
                {
                    HandleFinish();
                    return;
                }
            }
        }

        public Ability vability = null;
        public Ability pability = null;
        public bool vabilityWarmedOff = true;
        public bool pabilityWarmedOff = true;
        public CustomSpectator spectator;
        private double internalScore=0;
        List<PlayerItem> items = new List<PlayerItem>();
        public CountDown countDown;

        public Player()
        {
            countDown = new CountDown(this);
        }

        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);
            SendClientMessage("Siema " + this.Name + ". Milej zabawy :)");
            SendClientMessage("Wpisz '/zasady' w chacie by poznac reguly danej rozgrywki.");
            if(this.IsAdmin) SendClientMessage("Witaj ponownie adminie!");
        }

        public override void OnSpawned(SpawnEventArgs e)
        {
            base.OnSpawned(e);
            Interior = 0;
            Money = (int)internalScore;
            Score = (int)internalScore;
            PutCameraBehindPlayer();
/*            var data = Database.instance.get(new List<string>() { "skin" }, this.Name);
            this.Skin = Convert.ToInt32(data["skin"]);*/
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
                Position.X + distance * Math.Sin(-Angle * Math.PI / 180),
                Position.Y + distance * Math.Cos(-Angle * Math.PI / 180),
                Position.Z + highOffset);
        }
        public override void OnRequestClass(RequestClassEventArgs e)
        {
            var skin = Database.instance.get<int>("skin", this.Name);
            var skinId = 1;
            if (skin.Key != false) skinId = skin.Value;
            this.SetSpawnInfo(0, skinId, new Vector3(349.0453f, 193.2271f, 1014.1797f), 286.25f);
            this.Spawn();
        }
        public override void OnEnterVehicle(EnterVehicleEventArgs e)
        {
            base.OnEnterVehicle(e);
            if (vability != null) vability.OnPlayerEnterVehicle(e);
            if (pability != null) pability.OnPlayerEnterVehicle(e);
        }
        public override void OnExitVehicle(PlayerVehicleEventArgs e)
        {
            base.OnExitVehicle(e);
            if (vability != null) vability.OnPlayerExitVehicle(e);
            if (pability != null) pability.OnPlayerExitVehicle(e);
        }
        public static void GlobalCountdown(int timeInSeconds, string textOnFinish, Action actionOnFinish=null)
        {
            if(timeInSeconds==0)
            {
                GameTextForAll(textOnFinish, 3000, 5);
                if(actionOnFinish!=null) actionOnFinish.Invoke();
                return;
            }
            GameTextForAll("~g~" + timeInSeconds.ToString(), 999, 6);
            StaticTimer.RunAsync(new TimeSpan(0, 0, 1), () => {
                timeInSeconds -= 1;
                GlobalCountdown(timeInSeconds, textOnFinish, actionOnFinish);
            });
        }
    }
}