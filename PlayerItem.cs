using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using SampSharp.GameMode.Definitions;

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
        protected readonly int objectID;
        protected readonly string name;
        protected CustomPickupFactory pfactory;
        protected Dictionary<Pickup, PickableInstances> pickupObjects = new Dictionary<Pickup, PickableInstances>();
        public static List<PlayerItem> Instances = new List<PlayerItem>();
        protected PlayerItem(int objectID, string uniqueName)
        {
            this.objectID = objectID;
            name = uniqueName;
            pfactory = new CustomPickupFactory(
                CustomPickupFactory.PickupModel.Checkpoint, 
                CustomPickupFactory.PickupType.PlayerPickup,
                new EventHandler<PickUpPickupEventArgs>(delegate (Object o, PickUpPickupEventArgs a)
                {

                    if (pickupObjects.ContainsKey(a.Pickup))
                    {
                        GameMode.currentPlayMode.DestroyObject(pickupObjects[a.Pickup].go);
                        GiveToPlayer((Player)a.Player, pickupObjects[a.Pickup].amount);
                    }
                }));
            Instances.Add(this);
        }
        
        public void Spawn(Vector3 pos, Vector3 rot, float amount)
        {
            pickupObjects.Add(
                pfactory.Create(new Vector3(pos.X, pos.Y, pos.Z-1.6)),
                new PickableInstances(
                    GameMode.currentPlayMode.CreateObject(objectID, pos.X, pos.Y, pos.Z-0.8, rot.X+90, rot.Y, new Random().Next(180)),
                    amount)
                );
        }

        public virtual void GiveToPlayer(Player player, float amount){}
    }
    public class ItemWeapon : PlayerItem
    {
        Weapon weaponId;
        public ItemWeapon(int weaponObjectID, SampSharp.GameMode.Definitions.Weapon weapon) : 
            base(weaponObjectID, weapon.ToString()) 
        {
            weaponId = weapon;
        }
        public override void GiveToPlayer(Player player, float amount) {
            player.GiveWeapon(weaponId, (int)amount);
        }
    }
    public class WeaponItems
    {
        public static ItemWeapon AK47 = new ItemWeapon(355, Weapon.AK47);
        public static ItemWeapon DEagle = new ItemWeapon(348, Weapon.Deagle);
    }
}
