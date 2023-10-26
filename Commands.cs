using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace partymode
{
    class Commands
    {
        [CommandGroup("a")]
        class AdminCommandGroup
        {
            public class AdminPermissionChecker : IPermissionChecker
            {
                public string Message { get { return "Brakuje Ci uprawnien do uzycia tej komendy."; } }
                public bool Check(BasePlayer player) {
                    var checkAdmin = Database.instance.get<int>("admin", "samp_player", "name", player.Name,0);
                    return player.IsAdmin || (checkAdmin.Key==true && checkAdmin.Value>3); 
                }
            }
            [Command("spawn", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void SpawnCar(BasePlayer sender, BasePlayer player, int vid)
            {
                GameMode.currentPlayMode.CreateVehicle(vid, ((Player)player).GetInFrontPosition(2, 2), player.Angle + 90, true);
            }
            [Command("spawn", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void SpawnCar(BasePlayer sender, int vid)
            {
                GameMode.currentPlayMode.CreateVehicle(vid, ((Player)sender).GetInFrontPosition(2, 1), sender.Angle + 90, true);
            }
            [Command("obj", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void CreateObject(BasePlayer sender, BasePlayer player, int oid)
            {
                var loc = ((Player)player).GetInFrontPosition(2, 5);
                GameMode.currentPlayMode.CreateObject(oid, loc.X, loc.Y, loc.Z, 0, 0, player.Rotation.Z);
            }
            [Command("obj", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void CreateObject(BasePlayer sender, int oid)
            {
                var loc = ((Player)sender).GetInFrontPosition(2, 5);
                GameMode.currentPlayMode.CreateObject(oid, loc.X, loc.Y, loc.Z, 0, 0, sender.Rotation.Z);
            }
            [Command("hp", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void SetHealth(BasePlayer sender, BasePlayer player, float value)
            {
                player.Health = value;
            }
            [Command("hp", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void SetHealth(BasePlayer sender, float value)
            {
                sender.Health = value;
            }
            [Command("kill", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void KillSomeone(BasePlayer sender, BasePlayer player)
            {
                player.Health = 0;
            }
            [Command("kill", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void KillSomeone(BasePlayer sender)
            {
                sender.Health = 0;
            }
            [Command("weapon", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void SpawnWeapon(BasePlayer sender, BasePlayer player, int wid, int ammo)
            {
                player.GiveWeapon((Weapon)wid, ammo);
            }
            [Command("weapon", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void SpawnWeapon(BasePlayer sender, int wid, int ammo)
            {
                sender.GiveWeapon((Weapon)wid, ammo);
            }
            [Command("score", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void SetScore(BasePlayer sender, BasePlayer player, int score)
            {
                ((Player)player).SetScore(score);
            }
            [Command("score", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void SetScore(BasePlayer sender, int score)
            {
                ((Player)sender).SetScore(score);
            }
            [Command("load", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void LoadMap(BasePlayer sender, string mapName)
            {
                GameMode.gm.setCurrentModeByName(mapName, false);
            }
            [Command("begin", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void BeginMode(BasePlayer sender, int time)
            {
                GameMode.currentPlayMode.RequestBegin(GameMode.GetPlayers(), time);
            }
            [Command("help", PermissionChecker = typeof(AdminPermissionChecker))]
            private static void HelpCommand(BasePlayer sender)
            {
                sender.SendClientMessage("Available ~g~commands~w~:");
                sender.SendClientMessage("spawn, obj, kill, hp, weapon, score, load, begin");
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
            [Command("cpos")]
            private static void DebugCarPosition(BasePlayer sender)
            {
                if(sender.InAnyVehicle)
                {
                    Console.WriteLine(
                        "model," +
                        sender.Vehicle.Position.X.ToString()+","+
                        sender.Vehicle.Position.Y.ToString()+","+
                        sender.Vehicle.Position.Z.ToString()+","+
                        "0,0");
                }
                
            }
        }
    }
}
