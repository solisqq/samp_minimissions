using SampSharp.GameMode;
using System;
using System.Collections.Generic;

namespace partymode
{
    public class Race
    {
        public event EventHandler Finished;
        public event EventHandler AllFinished;

        List<Vector3> checkpoints;
        List<Player> playersFinished = new List<Player>();
        Dictionary<Player, int> currentCheckpointId = new Dictionary<Player, int>();
        
        public Race(List<Vector3> checkpointsPositions)
        {
            checkpoints = checkpointsPositions;
            if(checkpointsPositions.Count<2)
                throw new Exception("Minimum three checkpoints positions must be passed to instatiate Race class.");
        }
        private void OnEnterCheckpoint(object sender, EventArgs args)
        {
            var player = (Player)sender;
            var checkpointId = currentCheckpointId[player];
            if (checkpointId >= checkpoints.Count)
            {
                
                OnRaceFinish(player);
                player.DisableRaceCheckpoint();
                return;
            }
            if(checkpointId+2==checkpoints.Count)
            {
                player.SetRaceCheckpoint(SampSharp.GameMode.Definitions.CheckpointType.Finish,
                    checkpoints[0], new Vector3(0, 0, 0), 15.0f);
            } else
            {
                player.SetRaceCheckpoint(SampSharp.GameMode.Definitions.CheckpointType.Normal,
                    checkpoints[0], new Vector3(0, 0, 0), 15.0f);
            }
            currentCheckpointId[player] = checkpointId + 1;
        }
        public virtual void OnRaceFinish(Player player)
        {
            //Event handlers not wokring
            /*EventHandler handler = Finished;
            handler(player, EventArgs.Empty);*/
            playersFinished.Add(player);
            player.SetScore(1000.0 / 1);
            foreach(var pl in GameMode.GetPlayers())
                if (!playersFinished.Contains(pl)) 
                    return;
            /*EventHandler allFinishedHandler = AllFinished;
            allFinishedHandler(playersFinished, EventArgs.Empty);*/
        }
        public virtual void OnRaceBegin(Player player)
        {
            player.SetRaceCheckpoint(SampSharp.GameMode.Definitions.CheckpointType.Normal,
                checkpoints[0], new Vector3(0, 0, 0), 15.0f);
            currentCheckpointId.Add(player, 0);
            player.EnterRaceCheckpoint += OnEnterCheckpoint;
        }
        public virtual void Begin(List<Player> players)
        {
            foreach(var player in players) {
                OnRaceBegin(player);
            }
        }
    }
}
