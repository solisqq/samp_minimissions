using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace partymode
{
    public class CustomSpectator
    {
        
        protected Vector3 mainSpectatePosition;
        protected Vector3 mainSpectatePositionLookAt;
        public bool spectatePlayerOnly = false;
        public bool spectateMapOnly = false;
        private Timer switchTimer;
        public Player currentPlayerSpectating = null;
        private double lookAtZOffset = 0;

        public List<Player> getSpectators()
        {
            List<Player> spectators = new List<Player>();
            foreach (var player in GameMode.GetPlayers())
            {
                if (player.spectator != null && player.IsConnected)
                {
                    spectators.Add(player);
                }
            }
            return spectators;
        }

        public List<Player> getNonSpectators()
        {
            List<Player> spectators = new List<Player>();
            foreach (var player in GameMode.GetPlayers())
            {
                if (player.spectator == null && player.IsConnected)
                {
                    spectators.Add(player);
                }
            }
            return spectators;
        }

        public CustomSpectator(Vector3 mainPosition, double lookAtZOffset, double switchSpeed) {
            mainSpectatePosition = new Vector3(mainPosition.X, mainPosition.Y, mainPosition.Z + lookAtZOffset);
            mainSpectatePositionLookAt = mainPosition;
            this.lookAtZOffset = lookAtZOffset;
            switchTimer = new Timer(switchSpeed);
            switchTimer.Elapsed += HandleChange;
            
        }
        public CustomSpectator(Vector3 mainPosition, Vector3 mainPositionLookAt, double switchSpeed)
        {
            mainSpectatePosition = mainPosition;
            mainSpectatePositionLookAt = mainPositionLookAt;
            switchTimer = new Timer(switchSpeed);
            switchTimer.Elapsed += HandleChange;
        }

        private void HandleChange(Object sender, ElapsedEventArgs eventArgs)
        {
            if ((currentPlayerSpectating == null || spectatePlayerOnly) && !spectateMapOnly) {
                currentPlayerSpectating = PickUpNextPlayerToSpectate();
                foreach (var spect in getSpectators())
                    if (spect.IsConnected)
                        if (currentPlayerSpectating != null)
                        {
                            SpectatePlayer(spect);
                            //Console.WriteLine("Spectating: " + currentPlayerSpectating.Name + ".");
                        }
                        else
                        {
                            SpectateMap(spect);
                            //Console.WriteLine("No1 to spectate.");
                        }
            }
            else
            {
                foreach (var spect in getSpectators())
                    if (spect.IsConnected)
                        SpectateMap(spect);
                //Console.WriteLine("Spectating map.");
                currentPlayerSpectating = null;
            }
        }

        public void EnableSpectatingForPlayer(Player player)
        {
            player.spectator = this;
            var playerPos = new Vector3(mainSpectatePositionLookAt.X, mainSpectatePositionLookAt.Y, mainSpectatePositionLookAt.Z + lookAtZOffset + 5);
            player.Position = playerPos;
            player.ToggleControllable(false);
            if (currentPlayerSpectating == null)
            {
                SpectateMap(player);
            }
            else
            {
                SpectatePlayer(player);
            }
        }
        public void Enable()
        {
            switchTimer.Start();
        }
        public void Disable()
        {
            foreach (var spect in getSpectators())
            {
                spect.ToggleControllable(true);
                spect.ToggleSpectating(false);
                spect.PutCameraBehindPlayer();
                spect.spectator = null;
            }
        }
        private bool SpectatePlayer(Player player)
        {
            if (currentPlayerSpectating == null)
            {
                SpectateMap(player);
                return false;
            }
            player.PutCameraBehindPlayer();
            player.ToggleSpectating(true);
            player.SpectatePlayer(currentPlayerSpectating);
            return true;
        }
        private void SpectateMap(Player player)
        {
            //player.PutCameraBehindPlayer();
            player.ToggleSpectating(false);
            player.CameraPosition = mainSpectatePosition;
            player.SetCameraLookAt(mainSpectatePositionLookAt);
        }
        public Player PickUpNextPlayerToSpectate()
        {
            List<Player> availablePlayers = getNonSpectators();
            if (availablePlayers.Count == 0) return null;
            var r = new Random();
            return availablePlayers[r.Next(availablePlayers.Count)];
        }
    }
}
