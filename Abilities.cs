using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace partymode
{
    public class Abilities
    {
        public class AbilitiesRandomSpawner
        {
            public List<Ability> abilitiesToSpawn = new List<Ability>();
            public List<Vector3> positions = new List<Vector3>();
            List<int> availablePositions = new List<int>();
            System.Timers.Timer timer;
            public AbilitiesRandomSpawner()
            {
                timer = new System.Timers.Timer();
                timer.Interval = 10000;
                timer.Elapsed += SpawnPickup;
                timer.Stop();
            }
            public void Setup(IEnumerable<Ability> abilities, IEnumerable<Vector3> positions, int respawnInMS)
            {
                abilitiesToSpawn = new List<Ability>(abilities);
                this.positions = new List<Vector3>(positions);
                timer.Interval = respawnInMS;
                int i = 0;
                foreach (var pos in positions)
                {
                    availablePositions.Add(i++);
                }

            }
            public void StartSpawning()
            {
                timer.Start();
            }
            public void StopSpawning()
            {
                timer.Stop();
            }

            private void SpawnPickup(object sender, System.Timers.ElapsedEventArgs e)
            {
                if (availablePositions.Count == 0) return;
                Random r = new Random();
                var randomAbility = r.Next(abilitiesToSpawn.Count);
                var randomPosition = r.Next(availablePositions.Count);
                var newPosition = availablePositions[randomPosition];
                var pickup = abilitiesToSpawn[randomAbility].CreatePickup(positions[newPosition]);
                pickup.PickUp += new EventHandler<SampSharp.GameMode.Events.PickUpPickupEventArgs>(
                    delegate (object o, SampSharp.GameMode.Events.PickUpPickupEventArgs args) {
                        if (!availablePositions.Contains(newPosition))
                        {
                            StaticTimer.RunAsync(new TimeSpan(0, 0, 5), ()=>availablePositions.Add(newPosition));
                            //availablePositions.Add(newPosition);
                        }
                            
                    });
                availablePositions.Remove(newPosition);
            }
        }
        public static void GlobalUpdate(Player player)
        {
            foreach(var ability in abilities)
            {
                ability.OverwriteOnUpdate(player);
            }
        }
        public static SpeedUpVAbility speedUpVAbility = new SpeedUpVAbility();
        public static JumpVAbility jumpVAbility = new JumpVAbility();
        public static RepairVAbility repairVAbility = new RepairVAbility();
        public static PlantMineVAbility plantMineVAbility = new PlantMineVAbility();
        public static JumpPAbility jumpPAbility = new JumpPAbility();
        public static InvisibilePAbility invisibilePAbility = new InvisibilePAbility();
        public static InvulnerablePAbility invulnerablePAbility = new InvulnerablePAbility();
        public static SuperExplodePAbility superExplodePAbility = new SuperExplodePAbility();
        public static SuperPunchPAbility superPunchPAbility = new SuperPunchPAbility();
        public static RestorHealthPAbility restorHealthPAbility = new RestorHealthPAbility();

        public static List<Ability> abilities = new List<Ability>{
                speedUpVAbility,
                jumpVAbility,
                repairVAbility,
                plantMineVAbility,
                jumpPAbility,
                invisibilePAbility,
                invulnerablePAbility,
                superExplodePAbility,
                superPunchPAbility
            };
        public static void CleanUp()
        {
            foreach (var ability in abilities) ability.CleanUp();
        }
        public class SpeedUpVAbility : Ability
        {
            public SpeedUpVAbility() : base(
                "~g~ALT~w~ PRZYSPIESZENIE",
                AbilityType.VehicleAbility,
                CustomPickupFactory.PickupModel.AdrenalinePill,
                CustomPickupFactory.PickupType.VehiclePickup,
                (int)SampSharp.GameMode.Definitions.Keys.Fire)
            { }

            protected override bool Activate(Player player)
            {
                if (player.InAnyVehicle)
                {
                    var playerVeh = player.Vehicle;
                    var velocity = new Vector3(
                        playerVeh.Velocity.X * 1.9,
                        playerVeh.Velocity.Y * 1.9,
                        playerVeh.Velocity.Z * 1.9);
                    playerVeh.Velocity = velocity;
                    return true;
                }
                return false;
            }
        }
        public class JumpVAbility : Ability
        {
            public JumpVAbility() : base(
                "~g~ALT~w~ PODSKOK",
                AbilityType.VehicleAbility,
                CustomPickupFactory.PickupModel.GreenArrowUp,
                CustomPickupFactory.PickupType.VehiclePickup,
                (int)SampSharp.GameMode.Definitions.Keys.Fire)
            { }

            protected override bool Activate(Player player)
            {
                if (player.InAnyVehicle)
                {
                    var playerVeh = player.Vehicle;
                    var velocity = new Vector3(playerVeh.Velocity.X, playerVeh.Velocity.Y, playerVeh.Velocity.Z + 0.29);
                    playerVeh.Velocity = velocity;
                    return true;
                }
                return false;
            }
        }
        public class RepairVAbility : Ability
        {
            public RepairVAbility() : base(
                "Pancerz pojazdu ~g~odnowiony~w~!",
                AbilityType.VehicleAbility,
                CustomPickupFactory.PickupModel.Repair,
                CustomPickupFactory.PickupType.VehiclePickup)
            { }

            protected override bool Activate(Player player)
            {
                if (player.InAnyVehicle)
                {
                    player.Vehicle.Repair();
                    return true;
                }
                return false;
            }
        }
        public class PlantMineVAbility : Ability
        {
            class Mine
            {
                public Player Owner;
                public Vector3 Position;
                private DateTime plantTime;
                private GlobalObject globalObject;
                public Mine(Player Owner)
                {
                    this.Owner = Owner;
                    Position = Owner.Position;
                    plantTime = DateTime.Now;
                    globalObject = GameMode.currentPlayMode.CreateObject(
                        1252, Position.X, Position.Y, Position.Z - 0.6, 90, 0, new Random().Next(180));
                }

                public bool CheckActivate(Player player)
                {
                    if (player != Owner)
                    {
                        if (player.InArea(Position, 5) && DateTime.Now > plantTime.AddMilliseconds(3000))
                        {
                            BasePlayer.CreateExplosionForAll(Position, SampSharp.GameMode.Definitions.ExplosionType.LargeVisibleDamage2, 6);
                            GameMode.currentPlayMode.DestroyObject(globalObject);
                            return true;
                        }
                    }
                    return false;
                }
                ~Mine()
                {
                    globalObject.Dispose();
                }
            }

            private List<Mine> mines = new List<Mine>();
            public PlantMineVAbility() : base(
                "~g~ALT~w~ PODLOZ MINE",
                AbilityType.VehicleAbility,
                CustomPickupFactory.PickupModel.GreyBomb,
                CustomPickupFactory.PickupType.VehiclePickup,
                (int)SampSharp.GameMode.Definitions.Keys.Fire)
            { }
            public override void OverwriteOnUpdate(Player player)
            {
                for (int i = mines.Count - 1; i >= 0; i--)
                    if (mines[i].CheckActivate(player))
                        mines.RemoveAt(i);
            }
            protected override bool Activate(Player player)
            {
                if (player.InAnyVehicle)
                {
                    var mine = new Mine(player);
                    mines.Add(mine);
                    return true;
                }
                return false;
            }
            protected override void _CleanUp()
            {
                mines.Clear();
            }
        }
        public class JumpPAbility : Ability
        {
            public JumpPAbility() : base(
                "~g~ALT~w~ WYSOKI SKOK",
                AbilityType.PlayerAbility,
                CustomPickupFactory.PickupModel.Cocaine,
                CustomPickupFactory.PickupType.PlayerPickup,
                (int)SampSharp.GameMode.Definitions.Keys.Walk)
            { WarmOffMS = 100; }

            protected override bool Activate(Player player)
            {
                if (!player.InAnyVehicle)
                {
                    GameMode.Gravity = -0.2f;
                    player.Velocity = new Vector3(player.Velocity.X * 2, player.Velocity.Y * 2, player.Velocity.Z + 2);
                    return true;
                }
                return false;
            }
            protected override void OnWarmOffEnd(Player player)
            {
                GameMode.Gravity = 0.008f;
            }
        }
        public class InvisibilePAbility : Ability
        {
            public InvisibilePAbility() : base(
                "~g~ALT~w~ NIEWIDZIALNOSC~n~(4 sekundy)",
                AbilityType.PlayerAbility,
                CustomPickupFactory.PickupModel.Tiki,
                CustomPickupFactory.PickupType.PlayerPickup,
                (int)SampSharp.GameMode.Definitions.Keys.Walk)
            { WarmOffMS = 4000; }

            protected override bool Activate(Player player)
            {
                if (!player.InAnyVehicle)
                {
                    player.VirtualWorld = 1;//285
                    var pSkin = player.Skin;
                    player.Skin = 285;
                    StaticTimer.RunAsync(new TimeSpan(0, 0, 0, 0, WarmOffMS + 50), () => {
                        if (player != null && player.IsConnected && player.IsAlive)
                            player.Skin = pSkin;
                    });
                    return true;
                }
                return false;
            }
            protected override void OnWarmOffEnd(Player player)
            {
                player.VirtualWorld = 0;
            }
        }
        public class InvulnerablePAbility : Ability
        {
            public InvulnerablePAbility() : base(
                "~g~ALT~w~ NIESMIERTELNOSC~n~(4 sekundy)",
                AbilityType.PlayerAbility,
                CustomPickupFactory.PickupModel.Armor,
                CustomPickupFactory.PickupType.PlayerPickup,
                (int)SampSharp.GameMode.Definitions.Keys.Walk)
            { WarmOffMS = 4000; }

            protected override bool Activate(Player player)
            {
                if (!player.InAnyVehicle)
                {
                    var pArmor = player.Armour;
                    var pHealth = player.Health;
                    var pSkin = player.Skin;
                    player.Armour = 100;
                    player.Skin = 167;
                    player.TakeDamage += Player_TakeDamage;
                    StaticTimer.RunAsync(new TimeSpan(0, 0, 0, 0, WarmOffMS + 50), () => {
                        if (player != null && player.IsConnected && player.IsAlive)
                        {
                            player.Health = pHealth;
                            player.Armour = pArmor;
                            player.Skin = pSkin;
                        }
                    });
                    return true;
                }
                return false;
            }

            private void Player_TakeDamage(object sender, DamageEventArgs e)
            {
                ((Player)sender).Health = 100;
                ((Player)sender).Armour = 100;
            }

            protected override void OnWarmOffEnd(Player player)
            {
                player.TakeDamage -= Player_TakeDamage;
            }
        }
        public class SuperExplodePAbility : Ability
        {
            public SuperExplodePAbility() : base(
                "~g~ALT~w~ KAMIKAZE~n~(wybuchasz po 3s)",
                AbilityType.PlayerAbility,
                CustomPickupFactory.PickupModel.SingleSkull,
                CustomPickupFactory.PickupType.PlayerPickup,
                (int)SampSharp.GameMode.Definitions.Keys.Walk)
            { WarmOffMS = 3000; }

            protected override bool Activate(Player player)
            {
                if (!player.InAnyVehicle)
                {
                    var pSkin = player.Skin;
                    player.Skin = 264;
                    StaticTimer.RunAsync(new TimeSpan(0, 0, 0, 0, WarmOffMS + 50), () => {
                        if (player != null && player.IsConnected && player.IsAlive)
                            player.Skin = pSkin;
                    });
                    return true;
                }
                return false;
            }

            protected override void OnWarmOffEnd(Player player)
            {
                BasePlayer.CreateExplosionForAll(
                    player.Position, SampSharp.GameMode.Definitions.ExplosionType.HugeVisibleDamage, 10.0f);
            }
        }
        public class SuperPunchPAbility : Ability
        {
            public SuperPunchPAbility() : base(
                "~r~POTEZNY ~b~SIERPOWY~n~~w~(nastepny cios)",
                AbilityType.PlayerAbility,
                CustomPickupFactory.PickupModel.BoxingGlove,
                CustomPickupFactory.PickupType.PlayerPickup,
                (int)SampSharp.GameMode.Definitions.Keys.Fire){}

            protected override void Initialize(Player player)
            {
                player.GiveDamage += Player_GiveDamage;
                player.SetAttachedObject(
                    0, (int)CustomPickupFactory.PickupModel.BoxingGlove,
                    SampSharp.GameMode.Definitions.Bone.RightHand, new Vector3(0, 0, 0),
                    new Vector3(250, 250, 90), new Vector3(2.5, 2.5, 2.5), SampSharp.GameMode.SAMP.Color.Red,
                    SampSharp.GameMode.SAMP.Color.Black);
                player.SetAttachedObject(
                    1, (int)CustomPickupFactory.PickupModel.BoxingGlove,
                    SampSharp.GameMode.Definitions.Bone.LeftHand, new Vector3(0, 0, 0),
                    new Vector3(250, 250, 90), new Vector3(2.5, 2.5, 2.5), SampSharp.GameMode.SAMP.Color.Red,
                    SampSharp.GameMode.SAMP.Color.Black);
            }
            protected override bool Activate(Player player){return false;}
            private void Player_GiveDamage(object sender, DamageEventArgs e)
            {
                if (e.Weapon != SampSharp.GameMode.Definitions.Weapon.None) return;
                var player = ((Player)sender);
                var pArmor = player.Armour;
                player.SendClientMessage("Ale jebles "+ e.Amount.ToString());
                player.Armour = 100;
                BasePlayer.CreateExplosionForAll(
                    player.Position, SampSharp.GameMode.Definitions.ExplosionType.NormalVisibleDamageFlash2, 3.0f);
                StaticTimer.RunAsync(new TimeSpan(0, 0, 0, 0, 100), () => {
                    if (player != null && player.IsConnected && player.IsAlive) player.Armour = 0; 
                });
                DetachFromPlayer(player);
            }
            protected override void OnDetach(Player player)
            {
                player.GiveDamage -= Player_GiveDamage;
                player.RemoveAttachedObject(0);
                player.RemoveAttachedObject(1);
            }
        }
        public class RestorHealthPAbility : Ability
        {
            public RestorHealthPAbility() : base(
                "~w~ODNOWIONO ~r~ZYCIE~n~~w~TYMCZASOWY ~g~PANCERZ ~w~(5s)",
                AbilityType.PlayerAbility,
                CustomPickupFactory.PickupModel.Kebab,
                CustomPickupFactory.PickupType.PlayerPickup)
            {
                WarmOffMS = 5000;
            }

            protected override bool Activate(Player player) {
                player.Health = 100;
                player.Armour = 100;
                return true; 
            }
            protected override void OnWarmOffEnd(Player player)
            {
                player.Armour = 0;
            }
        }
    }
}
