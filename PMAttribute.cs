﻿using System;
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
    class PMAttribute
    {
        protected List<Player> exceptions;
        protected bool invokeOnExceptions;
        protected PMAttribute(List<Player> exceptions = default(List<Player>), bool invokeOnExceptions = false)
        {
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
            if (exceptions.Contains(player)) return;
            if (invokeOnExceptions)
                if(exceptions.Contains(player))
                    handleInitialize(player);
        }
        public void onBegin(List<Player> players)
        {
            handleBegin(filterPlayers(players));
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
    }
    class AutoBegin : PMAttribute
    {
        int hideRulesMS = 5000;
        public AutoBegin(int timeMs, int hideRulesMS = 5000, List<Player> exceptions = default(List<Player>), bool invokeOnExceptions = false):
            base(exceptions, invokeOnExceptions)
        {
        }
        protected override void handleStart(List<Player> players)
        {
            GameMode.currentPlayMode.RequestBegin(GameMode.GetPlayers(), 0);
        }
        protected override void handleInitialize(Player player) {
            var pm = GameMode.currentPlayMode;
            player.addTask(p => {
                if(pm.Equals(GameMode.currentPlayMode))
                {
                    GameMode.currentPlayMode.HideRules(p);
                }
            }, hideRulesMS);
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
                    player.Score += points;
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