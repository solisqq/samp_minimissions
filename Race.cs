using SampSharp.GameMode;
using System;
using System.Collections.Generic;

namespace partymode
{
    public class Race : PMAttribute
    {
        public event EventHandler Finished;
        public event EventHandler AllFinished;

        List<Vector3> checkpoints;
        List<Player> playersFinished = new List<Player>();
        
        public Race(List<Vector3> checkpointsPositions):
            base()
        {
            checkpoints = checkpointsPositions;
            if(checkpointsPositions.Count<2)
                throw new Exception("Minimum three checkpoints positions must be passed to instatiate Race class.");
        }
        protected override void handleEnterRaceCheckpoint(Player player)
        {
            player.AddScore(10);
            if (player.raceCheckpointId >= checkpoints.Count-1)
            {
                OnRaceFinish(player);
                return;
            }
            player.raceCheckpointId++;
            if (player.raceCheckpointId >= checkpoints.Count-1)
            {
                player.SetRaceCheckpoint(SampSharp.GameMode.Definitions.CheckpointType.Finish,
                    checkpoints[Math.Min(checkpoints.Count, player.raceCheckpointId)], new Vector3(0, 0, 0), 15.0f);
            } else
            {
                player.SetRaceCheckpoint(SampSharp.GameMode.Definitions.CheckpointType.Normal,
                    checkpoints[Math.Min(checkpoints.Count, player.raceCheckpointId)], checkpoints[Math.Min(checkpoints.Count, player.raceCheckpointId+1)], 15.0f);
            }
        }
        public virtual void OnRaceFinish(Player player)
        {
            /*EventHandler handler = Finished;*/
            player.Score = GameMode.getPointsFromPlace(playersFinished.Count);
            Finished.Invoke(player, EventArgs.Empty);
            Player.SendClientMessageToAll(
                "Player "+player.Name+" finished race as "+ (playersFinished.Count+1).ToString()+".");
            /*handler.Invoke();*/
            player.DisableRaceCheckpoint();
            //Event handlers not wokring
            /*EventHandler handler = Finished;
            handler(player, EventArgs.Empty);*/
            /*player.raceCheckpointId = 0;
            playersFinished.Add(player);
            player.SetScore(1000.0 / 1);
            foreach(var pl in GameMode.GetPlayers())
                if (!playersFinished.Contains(pl)) 
                    return;*/
            /*EventHandler allFinishedHandler = AllFinished;
            allFinishedHandler(playersFinished, EventArgs.Empty);*/
        }
        private void startForPlayer(Player player)
        {
            player.raceCheckpointId = 0;
            player.SetRaceCheckpoint(SampSharp.GameMode.Definitions.CheckpointType.Normal,
                checkpoints[0], new Vector3(0, 0, 0), 15.0f);
        }
        protected override void handleSpawn(Player player)
        {
            startForPlayer(player);
        }
        protected override void handleBegin(List<Player> players)
        {
            foreach (var player in players)
                startForPlayer(player);
        }
    }
}
