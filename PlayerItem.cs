using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using SampSharp.GameMode.Definitions;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Drawing.Printing;

namespace partymode
{
    public class PlayerItem
    {
        public class PickableInstances
        {
            public GlobalObject go;
            public float amount;
            public PickableInstances(GlobalObject g, float a)
            {
                go = g;
                amount = a;
            }
        };
        protected int objectID;
        protected string name;
        protected CustomPickupFactory pfactory;
        protected Dictionary<Pickup, PickableInstances> pickupObjects = new Dictionary<Pickup, PickableInstances>();
        public static List<PlayerItem> Instances = new List<PlayerItem>();

        protected PlayerItem(int objectID, string uniqueName)
        {
            this.objectID = objectID;
            name = uniqueName;
            instantiate();
        }
        void instantiate()
        {
            pfactory = new CustomPickupFactory(
                CustomPickupFactory.PickupModel.Checkpoint,
                CustomPickupFactory.PickupType.PlayerPickup,
                new EventHandler<PickUpPickupEventArgs>(delegate (Object o, PickUpPickupEventArgs a)
                {
                    if (pickupObjects.ContainsKey(a.Pickup))
                    {
                        GameMode.currentPlayMode.DestroyObject(pickupObjects[a.Pickup].go);
                        GiveToPlayer((Player)a.Player, pickupObjects[a.Pickup].amount);
                        Instances.Remove(this);
                    }
                }));

            Instances.Add(this);
        }

        public Pickup Spawn(Vector3 pos, Vector3 rot, float amount)
        {
            var pickup = pfactory.Create(new Vector3(pos.X, pos.Y, pos.Z - 1.6));
            pickupObjects.Add(
                pickup,
                new PickableInstances(
                    GameMode.currentPlayMode.CreateObject(objectID, pos.X, pos.Y, pos.Z-0.8, rot.X+90, rot.Y, new Random().Next(180)),
                    amount)
                );
            return pickup;
        }

        public virtual void GiveToPlayer(Player player, float amount){}
    }

    public class ItemWeapon : PlayerItem
    {
        public static Dictionary<string, ItemWeapon> createdWeapons = new Dictionary<string, ItemWeapon>();
        protected Weapon weaponId { get; private set; }
        int defaultPickCount;
        public ItemWeapon(int weaponObjectID, SampSharp.GameMode.Definitions.Weapon weapon, string name, int defaultPickCount) : 
            base(weaponObjectID, weapon.ToString()) 
        {
            weaponId = weapon;
            createdWeapons.Add(name, this);
            this.defaultPickCount = defaultPickCount;
        }

        public override void GiveToPlayer(Player player, float amount=-1) {
            if (amount < 0) amount = defaultPickCount;
            player.GiveWeapon(weaponId, (int)amount);
        }
    }
    public class WeaponTier
    {
        Dictionary<ItemWeapon, int> weapons;
        SampSharp.GameMode.Vector3 position;
        System.Timers.Timer respawnTimer;
        public WeaponTier(Dictionary<ItemWeapon, int> weapons, SampSharp.GameMode.Vector3 position)
        {
            this.weapons = weapons;
            this.position = position;
        }
        public void Spawn(int respawnTimeMS = 0) {
            if(this.weapons ==null || this.weapons.Count == 0) { return; }
            Random random = new Random();
            int id = random.Next(0, weapons.Count);
            var weapon = weapons.Keys.ToList()[id];
            var ammo = weapons.Values.ToList()[id];
            var pickup = weapon.Spawn(position, new Vector3(0, 0, 0), ammo);
            if (respawnTimeMS > 0)
            {
                pickup.PickUp += new EventHandler<PickUpPickupEventArgs>(delegate (Object o, PickUpPickupEventArgs a)
                {
                    respawnTimer = new System.Timers.Timer();
                    respawnTimer.Interval = respawnTimeMS;
                    respawnTimer.AutoReset = false;
                    respawnTimer.Start();
                    respawnTimer.Elapsed += new System.Timers.ElapsedEventHandler(delegate (object o, System.Timers.ElapsedEventArgs args)
                    {
                        Spawn(respawnTimeMS);
                    });
                });
            }
        }
    }

    public static class WeaponItems
    {
        public static ItemWeapon AK47 = new ItemWeapon(355, Weapon.AK47, "AK47", 30);
        public static ItemWeapon DEagle = new ItemWeapon(348, Weapon.Deagle, "Deagle", 14);
        public static ItemWeapon CombatShotgun = new ItemWeapon(351, Weapon.CombatShotgun, "CombatShotgun", 7);
        public static ItemWeapon MP5 = new ItemWeapon(353, Weapon.MP5, "MP5", 30);
        public static ItemWeapon Granade = new ItemWeapon(342, Weapon.Grenade, "Grenade",2);
        public static ItemWeapon M4 = new ItemWeapon(356, Weapon.M4, "M4", 30);
        public static ItemWeapon Minigun = new ItemWeapon(362, Weapon.Minigun, "Minigun", 100);
        public static ItemWeapon Moltov = new ItemWeapon(344, Weapon.Moltov, "Moltov", 2);
        public static ItemWeapon Parachute = new ItemWeapon(371, Weapon.Parachute, "Parachute",1);
        public static ItemWeapon Rifle = new ItemWeapon(357, Weapon.Rifle, "Rifle", 10);
        public static ItemWeapon RocketLauncher = new ItemWeapon(359, Weapon.RocketLauncher, "RocketLauncher", 2);
        public static ItemWeapon Shotgun = new ItemWeapon(349, Weapon.Shotgun, "Shotgun", 8);
        public static ItemWeapon Sawedoff = new ItemWeapon(350, Weapon.Sawedoff, "Sawedoff", 8);
        public static ItemWeapon Silenced = new ItemWeapon(347, Weapon.Silenced, "Silenced",24);
        public static ItemWeapon Sniper = new ItemWeapon(2036, Weapon.Sniper, "Sniper", 8);
        public static ItemWeapon Teargas = new ItemWeapon(343, Weapon.Teargas, "Teargas", 2);
        public static ItemWeapon Tec9 = new ItemWeapon(372, Weapon.Tec9, "Tec9", 40);
        public static ItemWeapon Uzi = new ItemWeapon(352, Weapon.Uzi, "Uzi", 40);
        public static ItemWeapon Colt45 = new ItemWeapon(346, Weapon.Colt45, "Colt45", 24);
    }
}
