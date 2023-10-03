using SampSharp.Core.Natives.NativeObjects;
using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace partymode
{
    public class NativesDestroyPickup
    {
        [NativeMethod]
        public virtual void DestroyPickup(int id)
        {
            throw new NativeNotImplementedException();
        }
    }
    public class CustomPickupFactory
    {
        public enum PickupType
        {
            PlayerPickup = 19,
            VehiclePickup = 14
        }
        public enum PickupModel
        {
            AdrenalinePill = 1241,
            Armor = 1242,
            GreyBomb = 1252,
            SingleSkull = 1254,
            TwinSkull = 1313,
            WhiteArrow = 1318,
            Tiki = 1276,
            Checkpoint = 1317,
            GreenArrowUp = 19134,
            Cocaine = 1279,
            Kebab = 2769,
            Repair = 3096,
            BoxingGlove = 19555,
            Invisible = 18703
        }
        private PickupType type;
        private PickupModel model;
        private Timer respawnTimer;
        List<Pickup> pickups = new List<Pickup>();
        EventHandler<PickUpPickupEventArgs> action;
        public CustomPickupFactory(PickupModel pmodel, PickupType ptype, EventHandler<PickUpPickupEventArgs> actionToInvokeOnPickup)
        {
            type = ptype;
            model = pmodel;
            action = actionToInvokeOnPickup;
        }
        public Pickup Create(Vector3 pos, double respawnAfterMS = -1)
        {
            var pickup = Pickup.Create((int)model, (int)type, pos);
            pickups.Add(pickup);
            pickup.PickUp += action;

            if (respawnAfterMS > 0)
            {
                pickup.PickUp += new System.EventHandler<SampSharp.GameMode.Events.PickUpPickupEventArgs>(
                    delegate (object o, SampSharp.GameMode.Events.PickUpPickupEventArgs args)
                    {
                        respawnTimer.Interval = respawnAfterMS;
                        respawnTimer.AutoReset = false;
                        respawnTimer.Start();
                        respawnTimer.Elapsed += new System.Timers.ElapsedEventHandler(delegate (object o, System.Timers.ElapsedEventArgs args)
                        {
                            Create(pos, respawnAfterMS);
                        });
                    });
            } else Console.WriteLine("not Created");
            pickup.PickUp += Pickup_PickUp;
            return pickup;
        }

        private void Pickup_PickUp(object sender, PickUpPickupEventArgs e)
        {
            pickups.Remove(e.Pickup);
            e.Pickup.Dispose();
        }

        public void Reset()
        {
            foreach (var p in pickups)
            {
                p.Dispose();
            }
            pickups.Clear();
        }
    }
}
