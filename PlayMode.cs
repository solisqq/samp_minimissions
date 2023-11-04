using partymode.Widgets;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;

namespace partymode
{
    public abstract class PlayMode
    {
        public readonly String cmd;
        public TDialog rulesDialog;
        
        TLabel nameLabel, rulesLabel;
        public enum PlayModeState
        {
            STARTED=0,
            BEGAN=1,
            FINISHED=2
        }
        public PlayModeState currentState { get; protected set; } = PlayModeState.STARTED;
        protected List<SampSharp.GameMode.Vector3> SpawnPositions;
        protected List<GlobalObject> staticObjects = new List<GlobalObject>();
        protected List<BaseVehicle> staticVehicles = new List<BaseVehicle>();
        protected List<PMAttribute> pmAttributes = new List<PMAttribute>();
        protected Abilities.AbilitiesRandomSpawner vAbilitiesSpawner = new Abilities.AbilitiesRandomSpawner();
        protected Abilities.AbilitiesRandomSpawner pAbilitiesSpawner = new Abilities.AbilitiesRandomSpawner();
        public SampSharp.GameMode.Vector3 startRotation { get; private set; }
        public int autoBegin = 0;
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
        public EventHandler PlayModeFinished;

        public PlayMode(String command)
        {
            GameMode.playModes.Add(command, this);
            cmd = command;
            beginCountDown = new System.Timers.Timer(1000);
            beginCountDown.AutoReset = false;
            beginCountDown.Elapsed += BeginCountDown_Elapsed;

            rulesDialog = new TDialog(
                new IGlobalTD(),
                new SampSharp.GameMode.Vector2(320, 240), 
                TDialog.VerticalAlignment.Center, 
                TDialog.HorizontalAlignment.Center, 
                TLabel.DefaultColors.Background);

            nameLabel = new TLabel(
                new IGlobalTD(),
                TLabel.DefaultTextStyles.PlayMode,
                new TLabel.ContentStyle(TextDrawAlignment.Left, 32, true),
                new Tuple<int,int,int,int>(4,0,0,0),
                "");

            rulesLabel = new TLabel(
                new IGlobalTD(),
                TLabel.DefaultTextStyles.DefaultText,
                new TLabel.ContentStyle(TextDrawAlignment.Left, 48, true),
                new Tuple<int, int, int, int>(4, 0, 2, 0),
                "");
            rulesDialog.addChild(nameLabel);
            rulesDialog.addChild(rulesLabel);
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
        protected void addAttribute(PMAttribute attribute)
        {
            pmAttributes.Add(attribute);
        }
        
        private void BeginCountDown_Elapsed(object sender = null, ElapsedEventArgs e=null)
        {
            if (beginCountDownCounter > 0)
            {
                BasePlayer.GameTextForAll("~r~"+ beginCountDownCounter.ToString(), 999, 6);
                beginCountDownCounter--;
            } else onBegin();
            
        }
        public void onBegin()
        {
            BasePlayer.GameTextForAll("~g~Start", 3000, 6);
            beginCountDown.AutoReset = false;
            beginCountDown.Stop();
            currentState = PlayModeState.BEGAN;
            foreach (var player in GameMode.GetPlayers())
                rulesDialog.hide(player);

            Begin(GameMode.GetPlayers());
            foreach (var att in pmAttributes) att.onBegin(GameMode.GetPlayers());
            pAbilitiesSpawner.StartSpawning();
            vAbilitiesSpawner.StartSpawning();
        }
        public GlobalObject CreateObject(int id, double x, double y, double z, double rotx, double roty, double rotz)
        {
            var obj = new GlobalObject(id, new SampSharp.GameMode.Vector3(x, y, z), new SampSharp.GameMode.Vector3(rotx, roty, rotz));
            staticObjects.Add(obj);
            return obj;
        }
        public GlobalObject DestroyObject(GlobalObject obj)
        {
            staticObjects.Remove(obj);
            obj.Dispose();
            return obj;
        }
        private void SetStartPosition(List<SampSharp.GameMode.Vector3> startPositions, double rotX, double rotY, int randomize = 0)
        {
            SpawnPositions = startPositions;
            startRotation = new SampSharp.GameMode.Vector3(rotX, rotY, 0);
        }
        public List<GlobalObject> getObjects()
        {
            return staticObjects;
        }
        public List<BaseVehicle> getVehicles()
        {
            return staticVehicles;
        }
        public BaseVehicle CreateVehicle(int id, SampSharp.GameMode.Vector3 vect, double rotation, bool nonStatic = false)
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
            return CreateVehicle(id, new SampSharp.GameMode.Vector3(x, y, z), rotation, nonStatic);
        }
        public BaseVehicle CreateVehicle(List<int> randomId, double x, double y, double z, double rotation, bool nonStatic = false)
        {
            int toSpawn = 409;
            try
            {
                Random rand = new Random();
                toSpawn = randomId[rand.Next(0, randomId.Count)];
            }
            catch { }
            return CreateVehicle(toSpawn, new SampSharp.GameMode.Vector3(x, y, z), rotation, nonStatic);
        }
        private string removeAccents(string input)
        {
            string decomposed = input.Normalize(NormalizationForm.FormD);
            char[] filtered = decomposed
                .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();
            return new String(filtered);
        }
        public string getFullName() { return nameLabel.text; }
        private void LoadFromDB()
        {
            Console.WriteLine(DateTime.Now.ToString("dd.MM hh:mm:ss") + ": Loading " + cmd + " playmode.");
            var playModeInfo = Database.instance.get(
                new List<string>() {
                    "fullname", "rules", "spawn_pos", "spectator", "abilities",
                    "vabilities", "abilities_set", "vehicles", "weapons",
                    "objects", "spawn_rotx", "spawn_roty", "randomize_spawn",
                }, "samp_playmode", "name", cmd);

            nameLabel.setText(playModeInfo["fullname"].ToString());
            rulesLabel.setText(removeAccents(playModeInfo["rules"].ToString()).Replace('ł','l'));
            var spectatorParams = playModeInfo["spectator"].ToString().Split(',');
            /*this.spectator = new CustomSpectator(
                new SampSharp.GameMode.Vector3(Convert.ToDouble(spectatorParams[0], CultureInfo.InvariantCulture), Convert.ToDouble(spectatorParams[1], CultureInfo.InvariantCulture), Convert.ToDouble(spectatorParams[2], CultureInfo.InvariantCulture)),
                Convert.ToInt32(spectatorParams[3], CultureInfo.InvariantCulture), Convert.ToInt32(spectatorParams[4], CultureInfo.InvariantCulture));
            */double spawn_rotx = 0.0;
            double spawn_roty = 0.0;
            var randomizeSpawn = 0;
            try { spawn_rotx = Convert.ToDouble(playModeInfo["spawn_rotx"].ToString()); } catch { }
            try { spawn_roty = Convert.ToDouble(playModeInfo["spawn_roty"].ToString()); } catch { }
            try { randomizeSpawn = Convert.ToInt32(playModeInfo["randomize_spawn"].ToString()); } catch { }
            var spawns = utils.flatToVectorList(playModeInfo["spawn_pos"].ToString());
            Console.WriteLine(DateTime.Now.ToString("dd.MM hh:mm:ss") + ": Spawn positions amount: " + spawns.Count.ToString() + ".");
            SetStartPosition(spawns, spawn_rotx, spawn_roty, randomizeSpawn);
            var vehicleData = utils.unpackData(playModeInfo["vehicles"].ToString(), 6);
            Console.WriteLine(DateTime.Now.ToString("dd.MM hh:mm:ss") + ": Vehicles amount: " + vehicleData.Count.ToString() + ".");
            foreach (var veh in vehicleData)
            {
                var stringid = veh[0].ToString();
                if (stringid.Contains('|')) stringid = utils.getRandomFromFlat(veh[0].ToString(), '|');
                CreateVehicle(
                    Convert.ToInt32(stringid),
                    Convert.ToDouble(veh[1], CultureInfo.InvariantCulture),
                    Convert.ToDouble(veh[2], CultureInfo.InvariantCulture),
                    Convert.ToDouble(veh[3], CultureInfo.InvariantCulture),
                    Convert.ToDouble(veh[4], CultureInfo.InvariantCulture),
                    Convert.ToBoolean(Convert.ToInt32(veh[5])));
            }
            var objectsData = utils.unpackData(playModeInfo["objects"].ToString(), 7);
            Console.WriteLine(DateTime.Now.ToString("dd.MM hh:mm:ss") + ": Objects amount: " + objectsData.Count.ToString() + ".");
            foreach (var obj in objectsData)
            {
                CreateObject(
                    Convert.ToInt32(obj[0]),
                    Convert.ToDouble(obj[1], CultureInfo.InvariantCulture),
                    Convert.ToDouble(obj[2], CultureInfo.InvariantCulture),
                    Convert.ToDouble(obj[3], CultureInfo.InvariantCulture),
                    Convert.ToDouble(obj[4], CultureInfo.InvariantCulture),
                    Convert.ToDouble(obj[5], CultureInfo.InvariantCulture),
                    Convert.ToDouble(obj[6], CultureInfo.InvariantCulture));
            }
            if (playModeInfo["abilities"].ToString().Length > 1)
            {
                var splittedPAbb = playModeInfo["abilities"].ToString().Split(":");
                if (splittedPAbb.Length == 2)
                {
                    var pabilitiesData = splittedPAbb[0].ToString().Split(",");
                    var pAbilitesArray = new List<Ability> { };
                    foreach (var ability in pabilitiesData)
                        foreach (var knownAbility in Abilities.abilities)
                            if (knownAbility != null && knownAbility.name == ability)
                                pAbilitesArray.Add(knownAbility);

                    var abilitiesVectors = utils.flatToVectorList(splittedPAbb[1]);
                    var settings = playModeInfo["abilities_set"].ToString().Split(',');
                    pAbilitiesSpawner.Setup(pAbilitesArray, abilitiesVectors, Convert.ToInt32(settings[0]), Convert.ToInt32(settings[1]));
                    Console.WriteLine(DateTime.Now.ToString("dd.MM hh:mm:ss") + ": Spawned " + pAbilitesArray.Count.ToString() + " player abilities in " + abilitiesVectors.Count.ToString() + " locations with settings: " + settings[0] + ", " + settings[1]);
                }
            }
            if (playModeInfo["vabilities"].ToString().Length > 1)
            {
                var splittedVAbb = playModeInfo["vabilities"].ToString().Split(":");
                if (splittedVAbb.Length == 2)
                {
                    var vabilitiesData = splittedVAbb[0].ToString().Split(",");
                    var vAbilitesArray = new List<Ability> { };
                    foreach (var ability in vabilitiesData)
                        foreach (var knownAbility in Abilities.abilities)
                            if (knownAbility != null && knownAbility.name == ability)
                                vAbilitesArray.Add(knownAbility);

                    var abilitiesVectors = utils.flatToVectorList(splittedVAbb[1]);
                    var settings = playModeInfo["abilities_set"].ToString().Split(',');
                    vAbilitiesSpawner.Setup(vAbilitesArray, abilitiesVectors, Convert.ToInt32(settings[0]), Convert.ToInt32(settings[1]));
                    Console.WriteLine(DateTime.Now.ToString("dd.MM hh:mm:ss") + ": Spawned " + vAbilitesArray.Count.ToString() + " vehicle abilities in " + abilitiesVectors.Count.ToString() + " locations with settings: " + settings[0] + ", " + settings[1]);
                }
            }
            if (playModeInfo["weapons"].ToString().Length > 0)
            {
                var weaponsData = utils.unpackData(playModeInfo["weapons"].ToString(), 4);
                // DONT FUCKIN TOUCH IT
                var test = WeaponItems.AK47;
                //WILL FAIL WITHOUT THIS
                foreach (var weapon in weaponsData)
                {
                    try
                    {
                        ItemWeapon.createdWeapons[utils.getRandomFromFlat(weapon[0], '|')].Spawn(
                            new SampSharp.GameMode.Vector3(
                                (float)Convert.ToDouble(weapon[1], CultureInfo.InvariantCulture),
                                (float)Convert.ToDouble(weapon[2], CultureInfo.InvariantCulture),
                                (float)Convert.ToDouble(weapon[3], CultureInfo.InvariantCulture)+0.72),
                            new SampSharp.GameMode.Vector3(0, 0, 0), -1);
                    }
                    catch (Exception ex) {

                    }
                }
                Console.WriteLine(DateTime.Now.ToString("dd.MM hh:mm:ss") + ": Spawned " + weaponsData.Count.ToString() + " weapons.");
            }
        }
        public void Start(List<Player> players)
        {
            currentState = PlayModeState.STARTED;
            LoadFromDB();
            InitializeStatics();
            foreach (var player in players)
            {
                player.ClearPlayer();
                player.dialogs = new Dictionary<string, TDialog>();
                InitializePlayer(player);
            }

            OnStart(players);
            pmAttributes.Add(new PlayerFinishedBehaviour(this));
            if (autoBegin > 0) addAttribute(new AutoBegin(autoBegin));
            foreach (var att in pmAttributes) att.onStart(GameMode.GetPlayers());
        }
        
        public void InitializePlayer(Player player)
        {
            player.Position = SpawnPositions[newRandomSpawnPosition];
            player.Rotation = startRotation;
            player.raceCheckpointId = 0;
            player.SetSpawnInfo(player.Team, player.Skin, player.Position, player.Rotation.Z);
            
            foreach (var att in pmAttributes) att.onInitialize(player);
            if (currentState==PlayModeState.STARTED)
            {
                rulesDialog.hide(player);
                rulesDialog.show(player);
            }
            if (!player.initializedAfterConnection) {
                player.initializedAfterConnection = true;
                player.ClearPlayer();
                OnJoin(player);
                OverwriteJoinBehaviour(currentState==PlayModeState.BEGAN, player);
                foreach(var att in pmAttributes)
                {
                    att.onJoin(player);
                }
            }
        }
        public void OnJoin(Player player)
        {
            GameMode.currentPlayMode.rulesDialog.show(player);
            if(currentState==PlayModeState.BEGAN)
                player.addTask((p) => GameMode.currentPlayMode.rulesDialog.hide(player), 5000);
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
            PlayerItem.clear();
            vAbilitiesSpawner.StopSpawning();
            pAbilitiesSpawner.StopSpawning();
            currentState = PlayModeState.FINISHED;
            /*spectator.Disable();*/

            foreach (var veh in staticVehicles)
            {
                veh.Dispose();
            }
            foreach (var obj in staticObjects)
            {
                obj.Dispose();
            }
            foreach (var att in pmAttributes)
                att.onFinish(GameMode.GetPlayers());
            staticVehicles.Clear();
            staticObjects.Clear();
            foreach (var player in players)
            {
                rulesDialog.hide(player);
            }
            Abilities.CleanUp();
            pmAttributes.Clear();
        }
        public abstract void InitializeStatics();
        protected abstract void OnStart(List<Player> players);
        protected abstract void OnEnd(List<Player> players);
        public virtual void OverwriteJoinBehaviour(bool begin, Player player) { }
        public virtual void OverwriteDeathBehaviour(Player player) {
            foreach(var att in pmAttributes)
            {
                att.onDeath(player);
            }
        }
        public virtual bool OverwriteSpawnBehaviour(Player player) {
            foreach (var att in pmAttributes) att.onSpawn(player);
            InitializePlayer(player);
            return false; 
        }
        public virtual void OverwriteEnterRaceCheckpoint(Player player) {
            foreach(var att in pmAttributes)
                att.onEnterRaceCheckpoint(player);
        }
        public virtual void OverwriteEnterCheckpoint(Player player) { }
        public void SetupPlayerAfterPlayModeStop(Player player)
        {
            SetupPlayerAfterEliminated(player);
            rulesDialog.hide(player);
        }
        public void SetupPlayerAfterEliminated(Player player)
        {
            player.ToggleControllable(false);
            var spawnPos = SpawnPositions[newRandomSpawnPosition];
            player.CameraPosition = new SampSharp.GameMode.Vector3(spawnPos.X, spawnPos.Y, spawnPos.Z+50);
            player.SetCameraLookAt(spawnPos);
        }
        public virtual void OverwriteKillBehaviour(Player killed, BasePlayer killer) { }
        public virtual void OverwriteUpdateBehaviour(Player player) { }
        public virtual void OverwriteDisconnectedBehaviour(Player player) {
            foreach(var att in pmAttributes)
            {
                att.onLeave(player);
            }
        }
        public virtual void StopGamePlay()
        {
            currentState = PlayModeState.FINISHED;
            var players = GameMode.GetPlayers();
            OnGamePlayFinish();
            foreach(var att in pmAttributes)
            {
                att.OnGamePlayFinish(players);
            }
            PlayModeFinished.Invoke(this, EventArgs.Empty);
        } 
        protected virtual void OnGamePlayFinish(){}
        public virtual void PlayerScoreChanged(Player player, double newScore)
        {
            player.infoDialog.updateScore((int)newScore);
            foreach(var att in pmAttributes)
            {
                att.onScoreChanged(player);
            }
        }
        public virtual void onPlayerFinished(Player player) {
            foreach(var att in pmAttributes)
            {
                att.onPlayerFinished(player);
            }
        }
        public abstract bool isAbleToStart();
    }

    public class Spectator
    {
    }
}
