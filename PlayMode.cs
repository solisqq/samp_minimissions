using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace partymode
{
    public abstract class PlayMode
    {
        public readonly String cmd;
        protected bool begin = false;
        private string rules = "";
        protected CustomSpectator spectator;
        protected List<Vector3> SpawnPositions;
        protected bool autoBegin = false;
        protected List<GlobalObject> staticObjects = new List<GlobalObject>();
        protected List<BaseVehicle> staticVehicles = new List<BaseVehicle>();
        protected Abilities.AbilitiesRandomSpawner abilitiesSpawner = new Abilities.AbilitiesRandomSpawner();
        public Vector3 startRotation { get; private set; }
        private System.Timers.Timer beginCountDown;
        private int beginCountDownCounter;
        private int _newRandomSpawnPosition = 0;
        protected int newRandomSpawnPosition { 
            get {
                if (_newRandomSpawnPosition >= SpawnPositions.Count)
                    _newRandomSpawnPosition = 0;
                return _newRandomSpawnPosition++;
            } 
        }
        protected int minimumPlayersCount = 0;
        private double _scoreLimit=-1;
        public double scoreLimit { get { return _scoreLimit; } set { _scoreLimit = value; } }
        TextDraw infoTd;

        public PlayMode(
            String command, 
            CustomSpectator spec,
            string textRules, 
            List<Vector3> startPositions, 
            double rotX, 
            double rotY, 
            int randomizePosition = 0)
        {
            GameMode.playModes.Add(this);
            cmd = command;
            SetStartPosition(startPositions, rotX, rotY, randomizePosition);
            this.spectator = spec;
            beginCountDown = new System.Timers.Timer(1000);
            beginCountDown.AutoReset = false;
            beginCountDown.Elapsed += BeginCountDown_Elapsed;
            rules = textRules;
            InitializeInfoTD();
            
        }
        private void InitializeInfoTD()
        {
            infoTd = TextDraw.Create(TDID.value);
            infoTd.Hide();
            infoTd.Alignment = SampSharp.GameMode.Definitions.TextDrawAlignment.Left;
            infoTd.BackColor = SampSharp.GameMode.SAMP.Color.Black;
            infoTd.Font = SampSharp.GameMode.Definitions.TextDrawFont.Normal;
            infoTd.LetterSize = new Vector2(0.35, 1.2);
            infoTd.ForeColor = SampSharp.GameMode.SAMP.Color.White;
            infoTd.BoxColor = SampSharp.GameMode.SAMP.Color.Black;
            infoTd.UseBox = true;
            infoTd.Text = "";
            infoTd.Position = new Vector2(250, 120.0);
            infoTd.Width = 400;
        }
        private void StartCountDown(int timeInSec)
        {
            if (timeInSec == 0)
            {
                BeginCountDown_Elapsed();
                return;
            }
            beginCountDown.AutoReset = true;
            beginCountDownCounter = timeInSec;
            beginCountDown.Start();
        }
        
        private void BeginCountDown_Elapsed(object sender = null, ElapsedEventArgs e=null)
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
                begin = true;
                foreach(var player in GameMode.GetPlayers())
                    HideRules(player);
                
                Begin(GameMode.GetPlayers());
                abilitiesSpawner.StartSpawning();
            }
        }
        public GlobalObject CreateObject(int id, double x, double y, double z, double rotx, double roty, double rotz)
        {
            var obj = new GlobalObject(id, new Vector3(x, y, z), new Vector3(rotx, roty, rotz));
            staticObjects.Add(obj);
            return obj;
        }
        public GlobalObject DestroyObject(GlobalObject obj)
        {
            staticObjects.Remove(obj);
            obj.Dispose();
            return obj;
        }
        private void SetStartPosition(List<Vector3> startPositions, double rotX, double rotY, int randomize = 0)
        {
            SpawnPositions = startPositions;
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
        public BaseVehicle CreateVehicle(int id, Vector3 vect, double rotation, bool nonStatic = false)
        {
            Random r = new Random();
            BaseVehicle car;
            if (nonStatic)
            {
                car = BaseVehicle.Create(
                        (SampSharp.GameMode.Definitions.VehicleModelType)id,
                        vect, (float)rotation,
                        r.Next(0, 127),
                        r.Next(0, 127));
            }
            else
            {
                car = BaseVehicle.CreateStatic(
                        (SampSharp.GameMode.Definitions.VehicleModelType)id,
                        vect, (float)rotation,
                        r.Next(0, 127),
                        r.Next(0, 127),
                        300);
            }

            staticVehicles.Add(car);
            return car;
        }
        public BaseVehicle CreateVehicle(int id, double x, double y, double z, double rotation, bool nonStatic=false)
        {
            return CreateVehicle(id, new Vector3(x, y, z), rotation, nonStatic);
        }
        public void Start(List<Player> players)
        {
            InitializeStatics();
            foreach (var player in players)
            {
                InitializePlayer(player);
            }
            spectator.Enable();
            OnStart(players);
            if (autoBegin)
            {
                RequestBegin(GameMode.GetPlayers(), 0);
            }
        }
        
        public void InitializePlayer(Player player)
        {
            player.Position = SpawnPositions[newRandomSpawnPosition];
            player.Rotation = startRotation;
            player.SetSpawnInfo(player.Team, player.Skin, player.Position, player.Rotation.Z);
            player.ClearPlayer();
            if (!begin)
            {
                HideRules(player);
                DisplayRules(player);
                if (autoBegin) StaticTimer.RunAsync(TimeSpan.FromSeconds(5), () => HideRules(player));
            }
        }
        public void DisplayRules(BasePlayer player)
        {
            infoTd.Text = rules;
            infoTd.Show(player);
        }
        public void HideRules(BasePlayer player)
        {
            infoTd.Hide(player);
        }
        
        public void RequestBegin(List<Player> players, int time)
        {
            if (GameMode.GetPlayers().Count < minimumPlayersCount)
            {
                BasePlayer.SendClientMessageToAll("Nie mozna rozpoczac rozgrywki.");
                BasePlayer.SendClientMessageToAll("Co najmniej dwóch graczy jest potrzebnych");
                BasePlayer.SendClientMessageToAll("Oczekiwanie na więcej graczy...");
                return;
            }
            
            StartCountDown(time);
        }
        protected virtual void Begin(List<Player> players) { }
        public void Finish(List<Player> players)
        {
            OnEnd(players);
            abilitiesSpawner.StopSpawning();
            begin = false;
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
            foreach(var player in players) HideRules(player);
            Abilities.CleanUp();
        }
        public abstract void InitializeStatics();
        protected abstract void OnStart(List<Player> players);
        protected abstract void OnEnd(List<Player> players);
        public virtual void OverwriteDeathBehaviour(Player player) { }
        public virtual bool OverwriteSpawnBehaviour(Player player) {
            InitializePlayer(player);
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
        public virtual void StopGamePlay(String reason)
        {
            begin = false;
            if(spectator!=null)
            {
                spectator.spectateMapOnly = true;
                foreach(var player in GameMode.GetPlayers())
                {
                    spectator.EnableSpectatingForPlayer(player);
                    //infoTd.Text = reason;
                    //qinfoTd.Show();
                }
            }
        }
        public virtual void PlayerScoreChanged(Player player, double newScore)
        {
            if (scoreLimit > 0 && newScore >= scoreLimit)
            {
                List<Player> SortedList = GameMode.GetPlayers().OrderBy(item => -item.Score).ToList();
                string reason="";
                int i = 0;
                foreach(var pl in SortedList)
                {
                    i++;
                    reason += i.ToString()+". " + pl.Name + "~n~";
                }
                StopGamePlay(reason);
            }
        }
    }

    public class Spectator
    {
    }
}
