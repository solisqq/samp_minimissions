using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using System.Text;

namespace partymode
{
    public class Abilities
    {
        public class AbilitiesRandomSpawner
        {
            List<Ability> abilitiesToSpawn;
            List<Vector3> positions;
            List<int> availablePositions = new List<int>();
            System.Timers.Timer timer;
            public AbilitiesRandomSpawner(IEnumerable<Ability> abilities, IEnumerable<Vector3> positions, int respawnTimeInMS)
            {
                timer = new System.Timers.Timer(respawnTimeInMS);
                timer.Elapsed += SpawnPickup;
                timer.Stop();
                this.abilitiesToSpawn = new List<Ability>(abilities);
                this.positions = new List<Vector3>(positions);
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
        public static List<Ability> abilities = new List<Ability>{
                speedUpVAbility,
                jumpVAbility,
                repairVAbility,
                plantMineVAbility
            };
        public static void CleanUp()
        {
            foreach (var ability in abilities) ability.CleanUp();
        }
    }
}
