using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using System;
using SampSharp.GameMode;
using System.Collections.Generic;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using System.Threading;
using System.Numerics;
using Microsoft.VisualBasic.FileIO;
using SampSharp.GameMode.Display;
using partymode.Widgets;

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
        public bool initializedAfterConnection = false;
        public Dictionary<string, TDialog> dialogs = new Dictionary<string, TDialog>();
        public PlayerDialog infoDialog;
        private double internalScore=0;
        List<PlayerItem> items = new List<PlayerItem>();
        public CountDown countDown;
        List<System.Timers.Timer> timers = new List<System.Timers.Timer>();

        public int raceCheckpointId=0;

        public Player()
        {
            countDown = new CountDown(this);
        }
        public void addDialog(string dialogName, TDialog dialog)
        {
            dialogs.Add(dialogName, dialog);
        }
        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);
            SendClientMessage("Siema " + this.Name + ". Milej zabawy :)");
            SendClientMessage("Wpisz '/zasady' w chacie by poznac reguly danej rozgrywki.");
            if(this.IsAdmin) SendClientMessage("Witaj ponownie adminie!");
            infoDialog = new PlayerDialog(this);
            infoDialog.show(this);
        }
        public void cleanUpTimers()
        {
            foreach (var timer in timers) timer.Stop();
            timers.Clear();
        }
        public void addTask(Action<Player> action, int delayMS)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = delayMS;
            timer.AutoReset = false;
            timer.Elapsed += (o, e) =>
            {
                if (this.IsConnected && this.IsAlive)
                    action.Invoke(this);
                timers.Remove(timer);
                timer.Dispose();
            };
            timer.Start();
        }

        public void showDialog(string dialog)
        {
            if(dialogs.ContainsKey(dialog))
                dialogs[dialog].show(this);
        }

        public void hideDialog(string dialog)
        {
            if (dialogs.ContainsKey(dialog))
                dialogs[dialog].hide(this);
        }

        public override void OnSpawned(SpawnEventArgs e)
        {
            var skin = Database.instance.get<int>("skin", "samp_player", "name", this.Name, 0);
            if (skin.Key != false) this.Skin = skin.Value;
            base.OnSpawned(e);
            Interior = 0;
            Money = (int)internalScore;
            Score = (int)internalScore;
            PutCameraBehindPlayer();
            
            GameMode.currentPlayMode.OverwriteSpawnBehaviour(this);
        }

        public override void OnEnterRaceCheckpoint(EventArgs e)
        {
            base.OnEnterCheckpoint(e);
            GameMode.currentPlayMode.OverwriteEnterRaceCheckpoint(this);
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
            this.Skin = Database.instance.get<int>("skin", "samp_player", "name", this.Name, 1).Value;
            ToggleControllable(true);
            ToggleSpectating(false);
            ResetScore();
            ResetAbilities();
            Health = 100;
            foreach (int i in Enum.GetValues(typeof(WeaponSkill)))
                SetSkillLevel((WeaponSkill)i, 200);
            PutCameraBehindPlayer();
            raceCheckpointId = 0;
        }

        public void AddScore(double scoreToAdd)
        {
            internalScore = (internalScore + scoreToAdd);
            GiveMoney((int)scoreToAdd);
            Score = (int)internalScore;
            GameMode.currentPlayMode.PlayerScoreChanged(this, internalScore);
        }
        public void SetScore(double newScore, bool dontTriggerChange = false)
        {
            internalScore = newScore;
            Money = (int)internalScore;
            Score = (int)newScore;
            if(!dontTriggerChange) GameMode.currentPlayMode.PlayerScoreChanged(this, internalScore);
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
        public bool InArea(SampSharp.GameMode.Vector3 position, float area, bool ignoreZ = false)
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
        public SampSharp.GameMode.Vector3 GetInFrontPosition(double distance, double highOffset)
        {
            return new SampSharp.GameMode.Vector3(
                Position.X + distance * Math.Sin(-Angle * Math.PI / 180),
                Position.Y + distance * Math.Cos(-Angle * Math.PI / 180),
                Position.Z + highOffset);
        }
        public override void OnRequestClass(RequestClassEventArgs e)
        {
            var skin = Database.instance.get<int>("skin", "samp_player", "name", this.Name, 0);
            var skinId = 1;
            if (skin.Key != false) skinId = skin.Value;
            this.SetSpawnInfo(0, skinId, new SampSharp.GameMode.Vector3(349.0453f, 193.2271f, 1014.1797f), 286.25f);
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
            GameMode.addTask(() =>
            {
                timeInSeconds -= 1;
                GlobalCountdown(timeInSeconds, textOnFinish, actionOnFinish);
            }, 1000);
            /*StaticTimer.RunAsync(new TimeSpan(0, 0, 1), );*/
        }
    }
}