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
    using System.Drawing;
    using System.Reflection.Metadata;
    using System.Numerics;
    using System.Reflection;

    namespace partymode
    {
        class BattleRoyalePM : PlayMode
        {
            public class Zone
            {
                const int outerZoneObjModel = 18657;
                /*const int innerZoneObjModel = 19084;*/

                public PointF currentMid;
                public float height;
                public float currentRadius;
                System.Timers.Timer closeZoneTimer;
                System.Timers.Timer closingTimer;
                bool isClosing = false;

                List<GlobalObject> objects = new List<GlobalObject>();

                public Zone(SampSharp.GameMode.Vector3 initMidPoint, float initRadius, int closeTime=30000, int closingInterval=90000)
                {
                    currentMid = new PointF(initMidPoint.X, initMidPoint.Y);
                    height = initMidPoint.Z;
                    currentRadius = initRadius;
                    closeZoneTimer = new System.Timers.Timer();
                    closingTimer = new System.Timers.Timer();
                    closingTimer.AutoReset = false;
                    closeZoneTimer.AutoReset = false;
                    closeZoneTimer.Interval = closingInterval;
                    closingTimer.Interval = closeTime;
                    closeZoneTimer.Elapsed += (p, e) => {
                        Player.GameTextForAll("Zamykanie strefy", 3000, 6);
                        randomizeInside();
                        closingTimer.Start();
                        isClosing = true;
                    };
                    closingTimer.Elapsed += (p, e) =>
                    {
                        Player.GameTextForAll("Strefa zamknieta", 3000, 6);
                        var circle = getCircle(currentMid, currentRadius);
                        createObjects(circle);
                        closeZoneTimer.Start();
                        isClosing = false;
                    };
                }
                public void begin()
                {
                    closeZoneTimer.Start();
                    var circle = getCircle(currentMid, currentRadius);
                    createObjects(circle);
                }
                public void end()
                {
                    closingTimer.Stop();
                    closeZoneTimer.Stop();
                }
                private void createObjects(List<PointF> points)
                {
                    foreach (var obj in objects)
                    {
                        GameMode.currentPlayMode.DestroyObject(obj);
                    }
                    objects.Clear();
                    foreach (var point in points)
                    {
                        objects.Add(
                            GameMode.currentPlayMode.CreateObject(
                                outerZoneObjModel, point.X, point.Y, height+40, 270, 0, 0));
                    }
                }
                private void randomizeInside()
                {
                    Random random = new Random();
                    int angle = random.Next(0,350);
                    float nextRadius = currentRadius/2;
                    var randomPoint = pointOnCircle(currentMid, nextRadius, angle);
                    var points = getCircle(randomPoint, currentRadius);
                    currentMid = randomPoint;
                    moveTowardsInner(points);
                    currentRadius = nextRadius;
                    closingTimer.Start();
                }
                private void moveTowardsInner(List<PointF> points)
                {
                    foreach(var obj in objects)
                    {
                        var distance = Math.Sqrt(Math.Pow(currentMid.X - obj.Position.X, 2) + Math.Pow(currentMid.Y - obj.Position.Y, 2));
                        obj.Move(new SampSharp.GameMode.Vector3(currentMid.X, currentMid.Y, height + 40), (float)((450.0 / closingTimer.Interval) * distance));
                    }
                }

                private PointF pointOnCircle(PointF mid, float radius, float angle)
                {
                    return new PointF(
                        (float)(mid.X + (radius * Math.Cos(angle))),
                        (float)(mid.Y + (radius * Math.Sin(angle))));
                }

                private List<PointF> getCircle(PointF point, float radius)
                {
                    int amountOfPoints = (int)radius;
                    double step = 360.0f / amountOfPoints;
                    List<PointF> points = new List<PointF>();
                    for (int i = 0; i < amountOfPoints; i++)
                        points.Add(pointOnCircle(point, radius, (float)step * i));
                    
                    return points;
                }

                public bool isPlayerOutsideZone(Player player)
                {
                    double tempRadius = currentRadius;
                    if (isClosing)
                    {
                        tempRadius = currentRadius * 2;
                    }
                    var distance = Math.Sqrt(Math.Pow(player.Position.X - currentMid.X, 2) + Math.Pow(player.Position.Y - currentMid.Y, 2));
                    if (distance > tempRadius)
                    {
                        return true;
                    }
                    
                    return false;
                }
            }
            System.Timers.Timer tickTimer = new System.Timers.Timer();
            Zone zone;

            public BattleRoyalePM() : base("battle")
            {
                tickTimer.AutoReset = true;
                tickTimer.Interval = 1000;
                tickTimer.Elapsed += (p, e) => { Tick();  };
            }

            public override void InitializeStatics()
            {
            }

            public override void OverwriteDeathBehaviour(Player player)
            {

            }

            public override void OverwriteKillBehaviour(Player killed, BasePlayer killer)
            {
                killer.Score += 100;
            }

            private void Tick()
            {
                if (currentState != PlayMode.PlayModeState.BEGAN) return;
                foreach (var player in GameMode.GetPlayers())
                {
                    if (Math.Abs(zone.height - player.Position.Z) > 50) continue;
                    if (zone.isPlayerOutsideZone(player))
                    {
                        player.Health -= 40 / zone.currentRadius;
                        player.GameText("~w~Wracaj do ~r~strefy~w~!", 1000, 6);
                    } else player.AddScore(3);
                }
            }

            public override bool OverwriteSpawnBehaviour(Player player)
            {
                base.OverwriteSpawnBehaviour(player);
                player.GiveWeapon(SampSharp.GameMode.Definitions.Weapon.Parachute, 1);
                return true;
            }

            protected override void OnEnd(List<Player> players)
            {
                zone.end();
                zone = null;
                SampSharp.GameMode.SAMP.Server.SetWorldTime(15);
                tickTimer.Stop();
            }

            protected override void OnStart(List<Player> players)
            {
                zone = new Zone(new SampSharp.GameMode.Vector3(220.5665, 1905.2751, 17.640625), 320, 30000, 90000);
                SampSharp.GameMode.SAMP.Server.SetWorldTime(24);
                var winAtt = new StopGameRules(this);
                addAttribute(new FreezTillBegin());
                winAtt.addRule(StopGameRules.StopRule.TimeLimit, 60000 * 8);
                foreach (var player in players)
                {
                    player.ToggleControllable(false);
                    player.GiveWeapon(SampSharp.GameMode.Definitions.Weapon.Parachute, 1);
                }
            }
            protected override void Begin(List<Player> players)
            {
                zone.begin();
                tickTimer.Start();
            }
            public override bool isAbleToStart()
            {
                if (GameMode.GetPlayers().Count > 1)
                    return true;
                return false;
            }
        }
    }

}
