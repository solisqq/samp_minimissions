using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Display;
using System.Timers;
using SampSharp.GameMode.SAMP;

namespace partymode
{
    class TDID
    {
        private static int _id;
        public static int value { get { return _id++; } }
    }
    public abstract class Ability
    {
        private int keyToActivate;
        private TextDraw td = TextDraw.Create(TDID.value);
        private System.Timers.Timer hideTDTimer = null;
        private CustomPickupFactory pickupFactory;
        protected Ability(string tdText, CustomPickupFactory.PickupModel model, CustomPickupFactory.PickupType type, int keyActivated = -1) {

            keyToActivate = keyActivated;
            td.Hide();
            td.Alignment = SampSharp.GameMode.Definitions.TextDrawAlignment.Left;
            td.BackColor = Color.Black;
            td.Font = SampSharp.GameMode.Definitions.TextDrawFont.Normal;
            td.LetterSize = new Vector2(0.3, 1.2);
            td.ForeColor = Color.White;
            td.BoxColor = Color.Black;
            td.UseBox = true;
            td.Text = tdText;
            td.Position = new Vector2(18.0, 91.0);
            td.Width = 140;
            if (keyActivated < 0) td.Position = new Vector2(18.0, 91.0);
            else td.Position = new Vector2(25.0, 91.0);
            pickupFactory = new CustomPickupFactory(model,type,
                new EventHandler<PickUpPickupEventArgs>(delegate (Object o, PickUpPickupEventArgs a)
                {
                    AttachToPlayer((Player)a.Player);
                }));
        }
        public virtual void OverwriteKeyStateChangeEvent(KeyStateChangedEventArgs e, Player player) {
            if ((((int)e.NewKeys) & keyToActivate) > 0 && (((int)e.OldKeys) & keyToActivate) == 0)
            {
                if (Activate(player))
                {
                    DetachFromPlayer(player);
                }
            }
        }
        public virtual void OverwriteOnUpdate(Player player){}
        protected abstract bool Activate(Player player);
        private void OnPickUp(Player player) {
            td.Show(player);
            if (keyToActivate<0)
            {
                hideTDTimer = new System.Timers.Timer(5000);
                hideTDTimer.AutoReset = false;
                hideTDTimer.Start();
                hideTDTimer.Elapsed += new System.Timers.ElapsedEventHandler(delegate (Object o, System.Timers.ElapsedEventArgs a)
                {
                    DetachFromPlayer(player);
                });
            }
        }
        public void AttachToPlayer(Player player)
        {
            if (keyToActivate >= 0)
            {
                if (player.ability != null) player.ability.DetachFromPlayer(player);
                player.ability = this;
            }
            else
            {
                Activate(player);
            }
            OnPickUp(player);
        }
        public void DetachFromPlayer(Player player)
        {
            if (keyToActivate >= 0) player.ability = null;
            td.Hide(player);
        }
        public Pickup CreatePickup(Vector3 pos, double respawnAfterMS = -1)
        {
            return pickupFactory.Create(pos, respawnAfterMS);
        }
        public void CleanUp()
        {
            pickupFactory.Reset();
            _CleanUp();
        }
        protected virtual void _CleanUp() { }
        //public virtual void OnUpdate(Player player) {}
    }
    public class SpeedUpVAbility : Ability
    {
        public SpeedUpVAbility() : base(
            "Wcisnij ALT by przyspieszyc!",
            CustomPickupFactory.PickupModel.AdrenalinePill,
            CustomPickupFactory.PickupType.VehiclePickup,
            (int)SampSharp.GameMode.Definitions.Keys.Fire){}

        protected override bool Activate(Player player)
        {
            if ( player.InAnyVehicle )
            {
                var playerVeh = player.Vehicle;
                var velocity = new Vector3(playerVeh.Velocity.X * 2, playerVeh.Velocity.Y * 2, playerVeh.Velocity.Z * 2);
                playerVeh.Velocity = velocity;
                return true;
            }
            return false;
        }
    }
    public class JumpVAbility : Ability
    {
        public JumpVAbility() : base(
            "Wcisnij ALT by podskoczyc!",
            CustomPickupFactory.PickupModel.Tiki,
            CustomPickupFactory.PickupType.VehiclePickup,
            (int)SampSharp.GameMode.Definitions.Keys.Fire) { }

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
            "Pancerz pojazdu odnowiony!",
            CustomPickupFactory.PickupModel.Armor,
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
            public Mine(Player Owner)
            {
                this.Owner = Owner;
                Position = Owner.Position;
                //Owner.
            }

            public bool CheckActivate(Player player)
            {
                if (player != Owner)
                {
                    //player.SendClientMessage("test1");
                    if(player.InArea(Position, 5))
                    {
                        player.SendClientMessage("test2");
                        BasePlayer.CreateExplosionForAll(Position, SampSharp.GameMode.Definitions.ExplosionType.LargeVisibleDamage2, 6);
                        return true;
                    }
                }
                return false;
            }
            ~Mine() { }
        }

        private List<Mine> mines = new List<Mine>();
        public PlantMineVAbility() : base(
            "ALT by podlozyc mine!",
            CustomPickupFactory.PickupModel.GreyBomb,
            CustomPickupFactory.PickupType.VehiclePickup,
            (int)SampSharp.GameMode.Definitions.Keys.Fire)
        {}
        public override void OverwriteOnUpdate(Player player) {
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
}
