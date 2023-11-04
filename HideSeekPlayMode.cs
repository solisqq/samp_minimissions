using SampSharp.GameMode;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using static partymode.StopGameRules;

namespace partymode
{
    class HideSeekPlayMode : PlayMode
    {
        Timer rewardTimer;
        private Player seeker=null;
        FreezTillBegin freezeTillBegin;
        StopGameRules stopRules;

        const int seekerTime = 20000;
        const int checkpointTime = 10000;

        public HideSeekPlayMode() :base("hideseek")
        {
            rewardTimer = new Timer(5000);
            rewardTimer.Elapsed += HandleReward;
            minimumPlayersCount = 2;
        }

        private void HandleReward(object sender, ElapsedEventArgs e)
        {
            foreach(var player in GameMode.GetPlayers())
            {
                if (stopRules.isFinished(player)) continue;
                if (seeker == player)
                {
                    player.AddScore(-10);
                    if (seeker.Score <= 0)
                    {
                        seeker.SetScore(0);
                        stopRules.customRule();
                    }
                } else
                {
                    player.Health -= 0.1f;
                }
            }
        }

        public override void OverwriteKillBehaviour(Player killed, BasePlayer killer)
        {
            base.OverwriteKillBehaviour(killed, killer);
            if (killer==seeker) killer.Score += 100;
            ((Player)killed).SetScore(0);
        }

        protected override void OnEnd(List<Player> players)
        {
            rewardTimer.Stop();
            stopRules = null;
            freezeTillBegin = null;
            foreach (var pl in GameMode.GetPlayers())
            {
                pl.DisableCheckpoint();
            }
        }
        public override void onPlayerFinished(Player player)
        {
            base.onPlayerFinished(player);
            player.DisableCheckpoint();
        }
        protected override void OnStart(List<Player> players)
        {
            stopRules = new StopGameRules(this);
            stopRules.addRule(StopGameRules.StopRule.WaitAfterConnect, 1);
            stopRules.addRule(StopGameRules.StopRule.WaitAfterDeath, 1);
            stopRules.addRule(StopGameRules.StopRule.MinPlayers, 2);
            stopRules.addRule(StopGameRules.StopRule.TimeLimit, 60000 * 8);
            freezeTillBegin = new FreezTillBegin();
            addAttribute(stopRules);
            addAttribute(freezeTillBegin);
            GameMode.Instance.ShowPlayerMarkers(SampSharp.GameMode.Definitions.PlayerMarkersMode.Off);
            GameMode.Instance.ShowNameTags(true);
            GameMode.Instance.SetNameTagDrawDistance(10.0f);
        }
        protected override void Begin(List<Player> players)
        {
            var ran = new Random();
            seeker = players[ran.Next(players.Count)];
            freezeTillBegin.addException(seeker);
            seeker.SetScore(2000);
            BasePlayer.GameTextForAll("~g~"+seeker.Name+" szuka!", seekerTime, 3);
            var pos = SpawnPositions[newRandomSpawnPosition];
            seeker.Position = new Vector3(pos.X, pos.Y, pos.Z+100);
            seeker.GiveWeapon(SampSharp.GameMode.Definitions.Weapon.M4, 1000);
            seeker.GiveWeapon(SampSharp.GameMode.Definitions.Weapon.Sawedoff, 100);
            seeker.ToggleControllable(false);
            Player.GlobalCountdown(seekerTime/1000, "Trwa aktywacja checkpointa", SeekerStart);
            GameMode.globalMsg("Ukryj sie!", "" +
                "Gracz ~g~"+seeker.Name+"~w~ zostal wylosowany jako poszukujacy. " +
                "Masz czas na ukrycie sie zdala od miejsca spawnu. Po skonczeniu odliczania, " +
                "gracz poszukujacy bedzie mogl Cie zabic!", seekerTime-1000);
        }

        private void SeekerStart()
        {
            if (seeker != null && seeker.IsConnected && seeker.IsAlive)
            {
                GameMode.globalMsg("Koniec odliczania", "Gracz ~g~" + seeker.Name + "~w~ rozpoczyna poszukiwania. " +
                "Czas na ukrycie minal. Rozpoczeto odliczanie do aktywacji miejsca zaklepania (checkpoint)...", checkpointTime-1000);
                seeker.Position = new Vector3(seeker.Position.X, seeker.Position.Y, seeker.Position.Z - 100);
                seeker.ToggleControllable(true);
                Player.GlobalCountdown(checkpointTime/1000, "Checkpoint aktywowano", ActivateCheckpoint);
                rewardTimer.Start();
            }
            else
            {
                BasePlayer.GameTextForAll("~g~" + seeker.Name + " wyszedl z gry!", 10000, 3);
                stopRules.customRule();
            }
        }
        public override void OverwriteDisconnectedBehaviour(Player player)
        {
            if(player==seeker)
            {
                BasePlayer.GameTextForAll("~g~" + seeker.Name + " wyszedl z gry!", 10000, 3);
                stopRules.customRule();
            }
            base.OverwriteDisconnectedBehaviour(player);
        }
        private void ActivateCheckpoint()
        {
            GameMode.globalMsg("Aktywowano zaklepanie", "Czerwony punkt na mapie (checkpoint) to miejsce zaklepania. " +
                "Jezeli sie do niego dostaniesz, gracz poszukujacy nie bedzie Cie juz mogl zabic. " +
                "Zaklep sie, by wygrac!", 20000);
            foreach (var pl in GameMode.GetPlayers())
            {
                pl.SetCheckpoint(new Vector3(-1140.8, -1170.5, 129.2), 10);
            }
        }
        public override void OverwriteEnterCheckpoint(Player player)
        {
            base.OverwriteEnterCheckpoint(player);
            if (player == seeker) return;
            player.SetScore(seeker.Score);
            stopRules.customRule(player);
        }
        public override void PlayerScoreChanged(Player player, double newScore)
        {
        }
        public override bool isAbleToStart()
        {
            if (GameMode.GetPlayers().Count > 1)
                return true;
            return false;
        }
        public override void InitializeStatics()
        {
            /*CreateObject(671, -1155.1, -1230.7, 130.89999, 0, 0, 0);
            CreateObject(647, -1162.9, -1223.5, 130.10001, 0, 0, 0);
            CreateObject(647, -1168.7, -1226.8, 132.7, 0, 0, 0);
            CreateObject(647, -1186.5, -1229.1, 134.39999, 0, 0, 0);
            CreateObject(647, -1203, -1230.7, 134.39999, 0, 0, 0);
            CreateObject(647, -1200.4, -1225.8, 131.3, 0, 0, 0);
            CreateObject(647, -1177.6, -1225.4, 131.5, 0, 0, 0);
            CreateObject(647, -1175.8, -1232.7, 137.8, 0, 0, 0);
            CreateObject(647, -1210.5, -1229.4, 133.5, 0, 0, 0);
            CreateObject(647, -1198.3, -1237.7, 138.60001, 0, 0, 0);
            CreateObject(647, -1213.5, -1240.5, 138.60001, 0, 0, 0);
            CreateObject(647, -1235.1, -1226.7, 130.60001, 0, 0, 0);
            CreateObject(647, -1217.9, -1230.5, 133.60001, 0, 0, 0);
            CreateObject(647, -1230.4, -1248.1, 139.3, 0, 0, 0);
            CreateObject(647, -1178.2, -1225.6, 131.60001, 0, 0, 0);
            CreateObject(647, -1165, -1231.8, 137, 0, 0, 0);
            CreateObject(647, -1145.8, -1273.9, 144.8, 0, 0, 0);
            CreateObject(647, -1179.2, -1248.3, 148.10001, 0, 0, 0);
            CreateObject(647, -1152.8, -1240.2, 139.2, 0, 0, 0);
            CreateObject(647, -1174.9, -1242.5, 145.60001, 0, 0, 0);
            CreateObject(647, -1162.5, -1245.3, 148.8, 0, 0, 0);
            CreateObject(647, -1182.2, -1238, 141, 0, 0, 0);
            CreateObject(647, -1223.9, -1182, 129.8, 0, 0, 0);
            CreateObject(647, -1256.1, -1199.7, 126.6, 0, 0, 0);
            CreateObject(647, -1221.2, -1203.1, 130.10001, 0, 0, 0);
            CreateObject(647, -1251, -1219.6, 128.3, 0, 0, 0);
            CreateObject(647, -1218.3, -1215.8, 130.10001, 0, 0, 0);
            CreateObject(647, -1224.1, -1225.9, 130.8, 0, 0, 0);
            CreateObject(647, -1235.4, -1246.8, 138.5, 0, 0, 0);
            CreateObject(647, -1227.5, -1238.4, 135.60001, 0, 0, 0);
            CreateObject(647, -1236.6, -1196.1, 128, 0, 0, 0);
            CreateObject(647, -1239.5, -1210.7, 128.10001, 0, 0, 0);
            CreateObject(647, -1218.4, -1169.3, 130.10001, 0, 0, 0);
            CreateObject(647, -1221.1, -1179.6, 130.10001, 0, 0, 0);
            CreateObject(647, -1219.6, -1192.5, 130.10001, 0, 0, 0);
            CreateObject(647, -1225.9, -1197, 129.5, 0, 0, 0);
            CreateObject(647, -1228.9, -1180.7, 129.10001, 0, 0, 0);
            CreateObject(647, -1240.2, -1191.7, 127.5, 0, 0, 0);
            CreateObject(647, -1234.2, -1174.6, 128.39999, 0, 0, 0);
            CreateObject(647, -1222.5, -1161.4, 130, 0, 0, 0);
            CreateObject(647, -1201.5, -1147.2, 130.10001, 0, 0, 0);
            CreateObject(1337, -1179.53955, -1141.22876, 128.71875, 0, 0, 0);
            CreateObject(657, -1160, -1227.3, 131.3, 0, 0, 0);
            CreateObject(657, -1165.6, -1235.9, 138.8, 0, 0, 0);
            CreateObject(657, -1192.4, -1245.7, 141.8, 0, 0, 0);
            CreateObject(657, -1192.4, -1236.6, 136.10001, 0, 0, 0);
            CreateObject(657, -1183.7, -1232.4, 135.3, 0, 0, 0);
            CreateObject(657, -1176.3, -1228.5, 132.39999, 0, 0, 0);
            CreateObject(657, -1172.5, -1234.8, 137.8, 0, 0, 0);
            CreateObject(657, -1167.4, -1229.7, 133.39999, 0, 0, 0);
            CreateObject(657, -1154.7, -1237.8, 136.89999, 0, 0, 0);
            CreateObject(657, -1147.9, -1237.9, 131.39999, 0, 0, 0);
            CreateObject(657, -1187.3, -1224.4, 128.8, 0, 0, 0);
            CreateObject(657, -1197.4, -1231.7, 133.10001, 0, 0, 0);
            CreateObject(657, -1186.8, -1246.4, 143.10001, 0, 0, 0);
            CreateObject(657, -1173.2, -1246.4, 146.60001, 0, 0, 0);
            CreateObject(657, -1161.1, -1243.3, 145.10001, 0, 0, 0);
            CreateObject(657, -1159.9, -1235.6, 138.39999, 0, 0, 0);
            CreateObject(657, -1148.9, -1250.4, 143.10001, 0, 0, 0);
            CreateObject(657, -1143.9, -1248.5, 137.5, 0, 0, 0);
            CreateObject(657, -1137.2, -1229.6, 128.2, 0, 0, 0);
            CreateObject(657, -1151.6, -1227.6, 128.2, 0, 0, 0);
            CreateObject(657, -1171.7, -1225.2, 129.39999, 0, 0, 0);
            CreateObject(657, -1215.4, -1225.5, 129.3, 0, 0, 0);
            CreateObject(657, -1211.4, -1236.6, 135.7, 0, 0, 0);
            CreateObject(657, -1208.7, -1247, 140.39999, 0, 0, 0);
            CreateObject(657, -1227.9, -1219.4, 127.6, 0, 0, 0);
            CreateObject(657, -1220.7, -1208.7, 128.2, 0, 0, 0);
            CreateObject(657, -1220.5, -1198.9, 128.2, 0, 0, 0);
            CreateObject(657, -1231.9, -1189, 126.9, 0, 0, 0);
            CreateObject(657, -1250.9, -1200.9, 127.7, 0, 0, 0);
            CreateObject(657, -1251, -1208.5, 135.8, 0, 0, 0);
            CreateObject(657, -1251.4, -1189.2, 124.1, 0, 0, 0);
            CreateObject(657, -1255.6, -1192.8, 131.3, 0, 0, 0);
            CreateObject(657, -1234.3, -1233.5, 139.89999, 0, 0, 0);
            CreateObject(657, -1157.7, -1213.3, 128.2, 0, 0, 0);
            CreateObject(681, -1157.8, -1214.9, 128.2, 0, 0, 0);
            CreateObject(688, -1207, -1227.6, 130.60001, 0, 0, 0);
            CreateObject(688, -1180.7, -1237.6, 139.3, 0, 0, 0);
            CreateObject(688, -1232.3, -1231.8, 132.60001, 0, 0, 0);
            CreateObject(688, -1244.7, -1210.2, 126, 0, 0, 0);
            CreateObject(688, -1234.1, -1192.1, 126.6, 0, 0, 0);
            CreateObject(688, -1181.4, -1229.4, 133.10001, 0, 0, 0);
            CreateObject(688, -1157.9, -1213.7, 128.7, 0, 0, 0);
            CreateObject(672, -1194.5, -1225.8, 129.5, 0, 0, 0);
            CreateObject(672, -1181.7, -1225.8, 130, 0, 0, 0);
            CreateObject(672, -1200.9, -1246.5, 141.89999, 0, 0, 0);
            CreateObject(672, -1223.3, -1239.9, 134.60001, 0, 0, 0);
            CreateObject(672, -1225.8, -1221.9, 127.9, 0, 0, 0);
            CreateObject(672, -1223.6, -1188.5, 128, 0, 0, 0);
            CreateObject(672, -1235.4, -1180.6, 126.4, 0, 0, 0);
            CreateObject(672, -1226.3, -1174.6, 127.6, 0, 0, 0);
            CreateObject(672, -1221, -1161.9, 128.2, 0, 0, 0);
            CreateObject(672, -1225.8, -1167.1, 127.7, 0, 0, 0);
            CreateObject(672, -1220.8, -1174.5, 128.2, 0, 0, 0);
            CreateObject(672, -1219.4, -1182.9, 128.2, 0, 0, 0);
            CreateObject(672, -1207.8, -1142.4, 128.2, 0, 0, 0);
            CreateObject(672, -1221.8, -1149.5, 128.2, 0, 0, 0);
            CreateObject(672, -1173.8, -1145.5, 128.2, 0, 0, 0);
            CreateObject(655, -1166.9, -1147, 128.2, 0, 0, 0);
            CreateObject(655, -1166.4, -1127.1, 128.2, 0, 0, 0);
            CreateObject(655, -1184.1, -1124, 128.2, 0, 0, 0);
            CreateObject(655, -1155.1, -1145.2, 128.2, 0, 0, 0);
            CreateObject(655, -1137.1, -1142.7, 128.2, 0, 0, 0);
            CreateObject(655, -1136.9, -1126.3, 128.2, 0, 0, 0);
            CreateObject(655, -1159.1, -1124.6, 128.2, 0, 0, 0);
            CreateObject(655, -1143.3, -1142.4, 128.2, 0, 0, 0);
            CreateObject(691, -1148.6, -1143.2, 128.2, 0, 0, 0);
            CreateObject(691, -1174.8, -1128.7, 128.2, 0, 0, 0);
            CreateObject(691, -1136.4, -1226.4, 128.2, 0, 0, 0);
            CreateObject(691, -1138, -1247.4, 131.8, 0, 0, 0);
            CreateObject(691, -1114.4, -1224.4, 128.2, 0, 0, 0);
            CreateObject(691, -1095.1, -1209.5, 128.2, 0, 0, 0);
            CreateObject(691, -1097.6, -1159.5, 128.2, 0, 0, 0);
            CreateObject(691, -1104.9, -1179.6, 128.2, 0, 0, 0);
            CreateObject(691, -1120.1, -1160.9, 128.2, 0, 0, 0);
            CreateObject(691, -1106.1, -1137.5, 128.2, 0, 0, 0);
            CreateObject(691, -1204.8, -1190.1, 128.2, 0, 0, 0);
            CreateObject(707, -1197.1, -1158.7, 128.2, 0, 0, 0);
            CreateObject(707, -1103.3, -1195.1, 128.2, 0, 0, 0);
            CreateObject(707, -1161.1, -1256.5, 154.10001, 0, 0, 0);
            CreateObject(707, -1212.6, -1273.4, 146.7, 0, 0, 0);
            CreateObject(707, -1222.4, -1251.4, 139.2, 0, 0, 0);
            CreateObject(707, -1086, -1226.6, 128.2, 0, 0, 0);
            CreateObject(707, -1136, -1152.6, 128.2, 0, 0, 0);
            CreateObject(12917, -1118.8, -1209.2, 128.2, 0, 0, 0);
            CreateObject(12918, -1132.6, -1189.8, 128.2, 0, 0, 300);
            CreateObject(14875, -1192.3, -1202.7, 129, 0, 0, 0);
            CreateObject(17057, -1179.1, -1187.9, 128.2, 0, 0, 300);
            CreateObject(17063, -1155.5, -1095.4, 128.2, 0, 0, 0);
            CreateObject(1451, -1083.4, -1203.5, 129, 0, 0, 0);
            CreateObject(1458, -1066.5, -1223.1, 128.2, 0, 0, 0);
            CreateObject(744, -1174.4, -1258.6, 151.5, 0, 0, 0);
            CreateObject(744, -1188.5, -1255.7998, 148.2, 0, 0, 0);
            CreateObject(10830, -1187.2, -1292.7, 159.10001, 0, 0, 225);
            CreateObject(3637, -1201.2, -1270.5, 156, 0, 0, 0);
            CreateObject(12912, -1038.6, -1110.9, 139.7, 0, 0, 0);
            CreateObject(919, -1132.7, -1269.1, 133, 0, 0, 0);
            CreateObject(11326, -1115.1, -1275.1, 131.5, 0, 0, 0);
            CreateObject(11480, -1081, -1274.3, 130.39999, 0, 0, 0);
            CreateObject(18283, -1116.8, -1297.2, 128.2, 0, 0, 0);*/
        }
    }
}
