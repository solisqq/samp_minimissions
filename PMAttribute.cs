using partymode.Widgets;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using static partymode.WeaponLevel;

namespace partymode
{
    public class PMAttribute
    {
        protected List<Player> exceptions = new List<Player>();
        protected bool invokeOnExceptions;
        protected PMAttribute(List<Player> exceptions = default(List<Player>), bool invokeOnExceptions = false)
        {
            if(exceptions != default(List<Player>))
                this.exceptions = exceptions;
            this.invokeOnExceptions = invokeOnExceptions;
        }
        public void onStart(List<Player> players)
        {
            handleStart(filterPlayers(players));
        }
        public void onFinish(List<Player> players) {
            handleFinish(filterPlayers(players));
        }
        public void onInitialize(Player player)
        {
            if (invokeOnExceptions && exceptions.Contains(player))
                handleInitialize(player);
            else if (!invokeOnExceptions && !exceptions.Contains(player))
                handleInitialize(player);
        }
        public void onScoreChanged(Player player)
        {
            if (invokeOnExceptions && exceptions.Contains(player))
                handleScoreChange(player);
            else if (!invokeOnExceptions && !exceptions.Contains(player))
                handleScoreChange(player);
        }
        public void onJoin(Player player)
        {
            if (invokeOnExceptions && exceptions.Contains(player))
                handleJoin(player);
            else if (!invokeOnExceptions && !exceptions.Contains(player))
                handleJoin(player);
        }
        public void onPlayerFinished(Player player)
        {
            if (invokeOnExceptions && exceptions.Contains(player))
                handlePlayerFinished(player);
            else if (!invokeOnExceptions && !exceptions.Contains(player))
                handlePlayerFinished(player);
        }
        public void onSpawn(Player player)
        {
            if (invokeOnExceptions && exceptions.Contains(player))
                handleSpawn(player);
            else if (!invokeOnExceptions && !exceptions.Contains(player))
                handleSpawn(player);
        }
        public void onEnterRaceCheckpoint(Player player)
        {
            if (invokeOnExceptions && exceptions.Contains(player))
                handleEnterRaceCheckpoint(player);
            else if (!invokeOnExceptions && !exceptions.Contains(player))
                handleEnterRaceCheckpoint(player);
        }
        public void onBegin(List<Player> players)
        {
            handleBegin(filterPlayers(players));
        }
        public void OnGamePlayFinish(List<Player> players)
        {
            handleGamePlayFinish(filterPlayers(players));
        }
        protected List<Player> filterPlayers(List<Player> players)
        {
            if (invokeOnExceptions)
                return (from exc in exceptions
                       where exc.IsConnected
                       select exc).ToList();
            return (from notexc in players.Except(exceptions)
                   where notexc.IsConnected
                   select notexc).ToList();
        }
        
        protected virtual void handleStart(List<Player> players) { }
        protected virtual void handleInitialize(Player player) { }
        protected virtual void handleBegin(List<Player> players) { }
        protected virtual void handleFinish(List<Player> players) { }
        protected virtual void handleSpawn(Player player) { }
        protected virtual void handleJoin(Player player) { }
        protected virtual void handleEnterRaceCheckpoint(Player player) { }
        protected virtual void handleGamePlayFinish(List<Player> players) { }
        protected virtual void handlePlayerFinished(Player player) { }
        protected virtual void handleScoreChange(Player player) { }
    }
    class AutoBegin : PMAttribute
    {
        int timeSec;
        public AutoBegin(int timeSec, List<Player> exceptions = default(List<Player>), bool invokeOnExceptions = false):
            base(exceptions, invokeOnExceptions)
        {
            this.timeSec = timeSec;
        }
        protected override void handleStart(List<Player> players)
        {
            GameMode.currentPlayMode.RequestBegin(players, timeSec);
        }
    }
    class FreezTillBegin : PMAttribute
    {
        protected override void handleSpawn(Player player)
        {
            if (GameMode.currentPlayMode.currentState != PlayMode.PlayModeState.BEGAN)
                player.ToggleControllable(false);
        }
        protected override void handleStart(List<Player> players)
        {
            foreach (Player p in players)
            {
                p.ToggleControllable(false);
            }
        }
        protected override void handleBegin(List<Player> players)
        {
            foreach (Player p in players)
            {
                p.ToggleControllable(true);
            }
        }
    }

    class PlayerFinishedBehaviour : PMAttribute
    {
        public EndGameDialog endGameDialog;
        private PlayMode currentMode;
        public PlayerFinishedBehaviour(PlayMode mode)
        {
            currentMode = mode;
            endGameDialog = new EndGameDialog();
        }
        List<Player> finishedPlayers = new List<Player>();
        protected override void handleStart(List<Player> players)
        {
            finishedPlayers.Clear();
            endGameDialog.hideToAll();
        }
        protected override void handleSpawn(Player player)
        {
            if ((currentMode.currentState == PlayMode.PlayModeState.BEGAN && finishedPlayers.Contains(player)) || 
                currentMode.currentState==PlayMode.PlayModeState.FINISHED)
                currentMode.SetupPlayerAfterEliminated(player);
        }
        protected override void handleFinish(List<Player> players)
        {
            endGameDialog.hideToAll();
        }
        protected override void handlePlayerFinished(Player player)
        {
            if (finishedPlayers.Contains(player)) return;
            finishedPlayers.Add(player);
            if(currentMode.currentState==PlayMode.PlayModeState.BEGAN)
            {
                currentMode.SetupPlayerAfterPlayModeStop(player);
                endGameDialog.refreshData();
                endGameDialog.show(player);
            }
        }
        protected override void handleGamePlayFinish(List<Player> players) 
        {
            var sortedPlayers = GameMode.GetPlayersSortedScore();
            for(int i=0; i< sortedPlayers.Count; i++)
            {
                var player = sortedPlayers[i];
                player.Score = GameMode.getPointsFromPlace(i);
                player.infoDialog.addToOverallScore(player.Score);
                currentMode.SetupPlayerAfterPlayModeStop(player);
            }
            endGameDialog.showToAll();
        }
    }

    class OverTimeReward : PMAttribute
    {
        System.Timers.Timer rewardTimer;
        int points;
        Func<Player, bool> condition;
        public OverTimeReward(
            int intervalMS, int points, Func<Player, bool> condition = null,
            List<Player> exceptions = default(List<Player>),
            bool invokeOnExceptions = false) :
            base(exceptions, invokeOnExceptions)
        {
            rewardTimer = new System.Timers.Timer();
            rewardTimer.Interval = intervalMS;
            rewardTimer.Elapsed += addPoints;
            rewardTimer.AutoReset = true;
            this.points = points;
        }

        private void addPoints(object sender, System.Timers.ElapsedEventArgs e)
        {
            var players = filterPlayers(GameMode.GetPlayers());
            foreach (var player in players)
            {
                if(condition == null || condition(player)) 
                    player.AddScore(points);
            }
        }

        protected override void handleBegin(List<Player> players)
        {
            rewardTimer.Start();
        }
        protected override void handleFinish(List<Player> players)
        {
            rewardTimer.Stop();
        }
        protected override void handleGamePlayFinish(List<Player> players)
        {
            rewardTimer.Stop();
        }
    }

    class StopGameRules : PMAttribute
    {
        System.Timers.Timer timeLimitTimer;
        System.Timers.Timer halfLimitTimer;
        System.Timers.Timer oneMinuteTimer;
        System.Timers.Timer _30secondsTimer;
        System.Timers.Timer _10secondsTimer;

        int scoreLimit = -1;

        bool stopOnAllFinished = false;
        List<Player> finishedPlayers = new List<Player>();
        public enum StopRule
        {
            ScoreLimit = 0,
            TimeLimit = 1,
            AllFinished = 2,
            CustomTrigger = 3
        }
        PlayMode mode;
        public StopGameRules(PlayMode mode) : base() {
            this.mode = mode;
            timeLimitTimer = new System.Timers.Timer();
            halfLimitTimer = new System.Timers.Timer();
            oneMinuteTimer = new System.Timers.Timer();
            _30secondsTimer = new System.Timers.Timer();
            _10secondsTimer = new System.Timers.Timer();

            timeLimitTimer.AutoReset = false;
            halfLimitTimer.AutoReset = false;
            oneMinuteTimer.AutoReset = false;
            _30secondsTimer.AutoReset = false;
            _10secondsTimer.AutoReset = false;

            timeLimitTimer.Elapsed += (p,e)=> mode.StopGamePlay();
            halfLimitTimer.Elapsed += (p, e) => BasePlayer.GameTextForAll("~w~Zostala ~g~polowa~w~ czasu", 6000, 6);
            oneMinuteTimer.Elapsed += (p, e) => BasePlayer.GameTextForAll("~w~Zostala ~b~minuta", 6000, 6);
            _30secondsTimer.Elapsed += (p, e) => BasePlayer.GameTextForAll("~w~Zostalo ~y~30~w~ sekund", 6000, 6);
            _10secondsTimer.Elapsed += (p, e) => BasePlayer.GameTextForAll("~w~Zostalo ~r~10~w~ sekund", 8000, 6);
        }

        public void addRule(StopRule rule, int value)
        {
            if(rule==StopRule.ScoreLimit)
            {
                scoreLimit = value;
            }
            else if(rule==StopRule.TimeLimit) {
                timeLimitTimer.Interval = value;
                halfLimitTimer.Interval = value / 2;
                oneMinuteTimer.Interval = Math.Abs(value-60000);
                _30secondsTimer.Interval = Math.Abs(value-30000);
                _10secondsTimer.Interval = Math.Abs(value-10000);
                timeLimitTimer.Start();
                halfLimitTimer.Start();
                oneMinuteTimer.Start();
                _30secondsTimer.Start();
                _10secondsTimer.Start();
            }
            else if (rule==StopRule.AllFinished)
            {
                stopOnAllFinished = true;
            }
        }
        protected override void handleScoreChange(Player player)
        {
            if (scoreLimit < 0) return;
            else if(scoreLimit > 0 && player.Score >= scoreLimit)
                mode.StopGamePlay();
            scoreLimit = -1;
            timeLimitTimer.Stop();
            halfLimitTimer.Stop();
            oneMinuteTimer.Stop();
            _30secondsTimer.Stop();
            _10secondsTimer.Stop();
        }
        protected override void handlePlayerFinished(Player player)
        {
            if (!stopOnAllFinished || finishedPlayers.Contains(player)) return;
            finishedPlayers.Add(player);
            foreach(var pl in GameMode.GetPlayers())
                if (!finishedPlayers.Contains(pl)) 
                    return;
            mode.StopGamePlay();
        }
        protected override void handleBegin(List<Player> players)
        {
            finishedPlayers.Clear();
        }
        protected override void handleStart(List<Player> players)
        {
            finishedPlayers.Clear();
        }
        protected override void handleFinish(List<Player> players)
        {
            timeLimitTimer.Stop();
            halfLimitTimer.Stop();
            oneMinuteTimer.Stop();
            _30secondsTimer.Stop();
            _10secondsTimer.Stop();
            scoreLimit = -1;
            stopOnAllFinished = false;
        }
    }


    class WeaponLevel : PMAttribute
    {
        public enum LevelType {
            MinLevelAll = 0,
            MaxLevelAll = 1,
            MaxLevelNoDual = 2
        }
        LevelType type;
        public WeaponLevel(
            LevelType type, 
            List<Player> exceptions = default(List<Player>), 
            bool invokeOnExceptions = false):
            base(exceptions, invokeOnExceptions)
        {
            this.type = type;
        }
        protected override void handleInitialize(Player player)
        {
            int uziColtSawed = 1000;
            int overall = 1000;
            if (type == LevelType.MinLevelAll)
            {
                uziColtSawed = 0;
                overall = 0;
            }
            else if (type == LevelType.MaxLevelNoDual)
                uziColtSawed = 500;
            for (int i = 0; i < 10; i++)
                if (i == 6 || i == 0 || i == 4) player.SetSkillLevel((SampSharp.GameMode.Definitions.WeaponSkill)i, uziColtSawed);
                else player.SetSkillLevel((SampSharp.GameMode.Definitions.WeaponSkill)i, overall);
        }
        protected override void handleFinish(List<Player> players)
        {
            foreach(Player player in players)
                for (int i = 0; i < 10; i++)
                    player.SetSkillLevel((SampSharp.GameMode.Definitions.WeaponSkill)i, 10);
        }
        
    }

    /*class CooldownRespawn : PMAttribute
    {
        public void onRespawn(Player player)
        {
            player.ToggleControllable(false);
        }
    }*/
}
