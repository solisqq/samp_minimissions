using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using System.Text;

namespace partymode
{
    public class Team
    {
        List<Player> members;
        public Team(List<Player> members) 
        {
            this.members = members;
        }
        public void SetPos(Vector3 pos)
        {
            foreach (var member in members)
                member.Position = pos;
        }
        public void AddHealth(float amount)
        {
            foreach (var member in members)
                member.Health += amount;
        }
        public bool IsMember(Player player)
        {
            return members.Contains(player);
        }
    }
}
