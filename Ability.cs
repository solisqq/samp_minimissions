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
    public class TDID
    {
        private static int _id=1;
        public static int value { get { return _id++; } }
    }
    public abstract class Ability
    {
        public enum AbilityType
        {
            VehicleAbility=0,
            PlayerAbility=1
        };
        private int keyToActivate;
        private TextDraw td = new TextDraw();
        private CustomPickupFactory pickupFactory;
        private AbilityType abtype;
        public int WarmOffMS = 0;
        public string name { get; private set; }

        protected Ability(
            string name,
            string tdText, 
            AbilityType atype, 
            CustomPickupFactory.PickupModel model, 
            CustomPickupFactory.PickupType type, 
            int keyActivated = -1) 
        {
            this.name = name;
            keyToActivate = keyActivated;
            abtype = atype;
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
            td.Width = 160;
            if (keyActivated >= 0) td.Position = new Vector2(18.0, 91.0);
            else td.Position = new Vector2(18.0, 111.0);
            pickupFactory = new CustomPickupFactory(model, type,
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
                    if (WarmOffMS > 0)
                    {
                        if (WarmOffMS > 2)
                            player.countDown.Setup(WarmOffMS / 1000, "~r~0");
                        if (abtype == AbilityType.PlayerAbility) player.pabilityWarmedOff = false;
                        else player.vabilityWarmedOff = false;
                        player.addTask((Player p) =>
                        {
                            if (player != null && player.IsConnected && player.IsAlive) {
                                OnWarmOffEnd(player);
                                if (abtype == AbilityType.PlayerAbility) player.pabilityWarmedOff = true;
                                else player.vabilityWarmedOff = true;
                            }
                        }, WarmOffMS);
                    }
                    DetachFromPlayer(player);
                }
            }
        }
        public virtual void OverwriteOnUpdate(Player player){}
        protected abstract bool Activate(Player player);
        private void OnPickUp(Player player) {
            if(!player.pabilityWarmedOff || !player.vabilityWarmedOff)
            {
                player.SendClientMessage(Color.DarkRed, "Poczekaj az twoja aktualna umiejetnosc sie skonczy,");
                player.SendClientMessage(Color.White, "zanim sprobujesz podniesc nastepna.");
                return;
            }
            td.Show(player);
            if (keyToActivate<0)
                StaticTimer.RunAsync(new TimeSpan(0,0,5), ()=> { 
                    if(player!=null && player.IsConnected) 
                        DetachFromPlayer(player); });
            Initialize(player);
        }
        public void OnPlayerExitVehicle(PlayerVehicleEventArgs e)
        {
            if (abtype == AbilityType.VehicleAbility) td.Hide();
            else td.Show();
        }
        public void OnPlayerEnterVehicle(EnterVehicleEventArgs e)
        {
            if (abtype == AbilityType.VehicleAbility) td.Show();
            else td.Hide();
        }
        public void AttachToPlayer(Player player)
        {
            if (keyToActivate >= 0)
            {
                if (abtype == AbilityType.VehicleAbility && player.vability != null)
                    player.vability.DetachFromPlayer(player);
                else if (abtype == AbilityType.PlayerAbility && player.pability != null)
                    player.pability.DetachFromPlayer(player);
                if (abtype == AbilityType.VehicleAbility)
                    player.vability = this;
                else if (abtype == AbilityType.PlayerAbility)
                    player.pability = this;
            }
            else if (Activate(player) && WarmOffMS > 0)
            {
                StaticTimer.RunAsync(new TimeSpan(0, 0, 0, 0, WarmOffMS), () =>{
                    if (player != null && player.IsConnected && player.IsAlive) 
                        OnWarmOffEnd(player);});
            }
            
            OnPickUp(player);
        }
        public void DetachFromPlayer(Player player)
        {
            if (keyToActivate >= 0)
            {
                if (abtype == AbilityType.VehicleAbility) player.vability = null;
                else if (abtype == AbilityType.PlayerAbility) player.pability = null;
            }
            td.Hide(player);
            OnDetach(player);
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
        protected virtual void OnWarmOffEnd(Player player) { }
        protected virtual void Initialize(Player player) { }
        protected virtual void OnDetach(Player player) { }
    }
}
