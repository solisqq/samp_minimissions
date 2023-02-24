using SampSharp.GameMode;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace partymode
{
    public abstract class PlayMode
    {
        public readonly String cmd;
        protected List<GlobalObject> staticObjects = new List<GlobalObject>();
        protected List<BaseVehicle> staticVehicles = new List<BaseVehicle>();
        public Vector3 startPosition { get; private set; }
        public Vector3 startRotation { get; private set; }
        private Timer beginCountDown;
        private int beginCountDownCounter;
        protected CustomSpectator spectator;

        public PlayMode(String command, CustomSpectator spec, Vector3 startPos, double rotX, double rotY, int randomizePosition = 0)
        {
            GameMode.playModes.Add(this);
            cmd = command;
            SetStartPosition(startPos, rotX, rotY, randomizePosition);
            this.spectator = spec;
            beginCountDown = new Timer(1000);
            beginCountDown.AutoReset = false;
            beginCountDown.Elapsed += BeginCountDown_Elapsed;
        }
        private void StartCountDown(int timeInSec)
        {
            beginCountDown.AutoReset = true;
            beginCountDownCounter = timeInSec;
            beginCountDown.Start();
        }

        private void BeginCountDown_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (beginCountDownCounter > 0)
            {
                BasePlayer.GameTextForAll("~r~"+ beginCountDownCounter.ToString(), 999, 6);
                beginCountDownCounter--;
            } else
            {
                BasePlayer.GameTextForAll("~g~Start", 3000, 6);
                beginCountDown.AutoReset = false;
                beginCountDown.Stop();
                Begin(GameMode.players);
            }
        }

        protected GlobalObject CreateObject(int id, double x, double y, double z, double rotx, double roty, double rotz)
        {
            var obj = new GlobalObject(id, new Vector3(x, y, z), new Vector3(rotx, roty, rotz));
            staticObjects.Add(obj);
            return obj;
        }
        private void SetStartPosition(Vector3 startPos, double rotX, double rotY, int randomize = 0)
        {
            double offsetX = 0;
            double offsetY = 0;
            if (randomize>0)
            {
                var r = new Random();
                offsetX = (r.NextDouble()-0.5) * randomize * 2;
                offsetY = (r.NextDouble()-0.5) * randomize * 2;
            }
            startPosition = startPos;
            startRotation = new Vector3(rotX, rotY, 0);
        }
        public List<GlobalObject> getObjects()
        {
            return staticObjects;
        }
        public List<BaseVehicle> getVehicles()
        {
            return staticVehicles;
        }
        protected BaseVehicle CreateVehicle(int id, double x, double y, double z, double rotation)
        {
            Random r = new Random();
            var car = BaseVehicle.CreateStatic(
                (SampSharp.GameMode.Definitions.VehicleModelType)id,
                new Vector3(x,y,z), (float)rotation,
                r.Next(0,127),
                r.Next(0,127),
                0);
            staticVehicles.Add(car);
            return car;
        }
        public void Start(List<Player> players)
        {
            InitializeStatics();
            foreach (var player in players)
            {
                player.Position = startPosition;
                player.Rotation = startRotation;
                player.ClearPlayer();
            }
            spectator.Enable();
            OnStart(players);
        }
        public void RequestBegin(List<Player> players, int time)
        {
            StartCountDown(time);
        }
        protected virtual void Begin(List<Player> players) { }
        public void Finish(List<Player> players)
        {
            OnEnd(players);
            spectator.Disable();
            foreach (var veh in staticVehicles)
            {
                veh.Dispose();
            }
            foreach (var obj in staticObjects)
            {
                obj.Dispose();
            }
            staticVehicles.Clear();
            staticObjects.Clear();
            Abilities.CleanUp();
        }
        public abstract void InitializeStatics();
        protected abstract void OnStart(List<Player> players);
        protected abstract void OnEnd(List<Player> players);
        public virtual void OverwriteDeathBehaviour(Player player) { }
        public virtual bool OverwriteSpawnBehaviour(Player player) {
            player.Position = startPosition;
            player.Rotation = startRotation;
            return false; 
        }
        public virtual void OverwriteKillBehaviour(Player killed, BasePlayer killer) { }
        public virtual void OverwriteUpdateBehaviour(Player player) { }
        public virtual void TurnOnSpectate(Player player)
        {
            if(spectator!=null)
            {
                spectator.EnableSpectatingForPlayer(player);
            }
        }
    }

    public class Spectator
    {
    }
}
