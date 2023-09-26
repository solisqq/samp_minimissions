using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode;

namespace partymode
{
    class FreeRoamPM : PlayMode
    {
        public FreeRoamPM() : 
            base(
                "freeroam",
                new CustomSpectator(new Vector3(1588.7, 1611.5, 10.8), 80, 5000),
                "Tryb rozgrywki: ~g~Dowolny ~w~~n~Zabijaj, zbieraj ukryta bron, wyprobuj pojazdy! Tryb dowolny oznacza, ze mozesz robic co chcesz.",
                new List<Vector3> { new Vector3(1588.7, 1611.5, 10.8) }, 0, 0, 3) {}

        public override void InitializeStatics()
        {
            CreateObject(985, 1678.7, 1639.5, 9.8, 0, 0, 0);
            CreateObject(987, 1704.6, 1602.3, 9.2, 0, 0, 76);
            CreateObject(3665, 1524.9, 1653.4, 11.8, 0, 0, 0);
            CreateObject(12956, 1519.2, 1753.1, 13.6, 0, 0, 86);
            CreateObject(13641, 1518, 1543, 11.6, 0, 0, 182);
            CreateObject(1632, 1537.3, 1586.4, 11.1, 0, 0, 0);
            CreateObject(1634, 1545.9, 1614.9, 11.1, 0, 0, 90);
            CreateObject(1271, 1599.6, 1613, 10.2, 0, 0, 0);
            CreateObject(1271, 1599.5, 1611.8, 10.2, 0, 0, 0);
            CreateObject(1271, 1599.4, 1612.7, 10.9, 0, 0, 0);
            CreateVehicle(483, 1589.3, 1637.6, 10.9, 0);
            CreateVehicle(483, 1591.9, 1637.5, 10.9, 0);
            CreateVehicle(478, 1595, 1637.4, 10.9, 0);
            CreateVehicle(403, 1578.3, 1648.3, 11.5, 0);
            CreateVehicle(403, 1582.7, 1647.5, 11.5, 0);
            CreateVehicle(450, 1581.4, 1665.2, 11.5, 178);
            CreateVehicle(406, 1600.9, 1637.9, 11.6, 359.5);
            CreateVehicle(408, 1566.1, 1653.8, 11.6, 0);
            CreateVehicle(443, 1555.1, 1673.1, 11.6, 0);
            CreateVehicle(486, 1561.1, 1654.2, 11.1, 0);
            CreateVehicle(514, 1573.7, 1649.3, 11.5, 0);
            CreateVehicle(525, 1569.8, 1648.3, 10.8, 0);
            CreateVehicle(530, 1604, 1611.6, 10.6, 94);
            CreateVehicle(530, 1604.2002, 1613.2998, 10.6, 93.999);
            CreateVehicle(530, 1604, 1610, 10.6, 93.999);
            CreateVehicle(530, 1604, 1608.4, 10.6, 93.999);
            CreateVehicle(407, 1622.3, 1617.6, 11.2, 270);
            CreateVehicle(407, 1623.1, 1621.6, 11.2, 270);
            CreateVehicle(407, 1623.1, 1625.5, 11.2, 270);
            CreateVehicle(432, 1303, 1221.8, 10.9, 0);
            CreateVehicle(495, 1260.6, 1312.9, 11.4, 0);
            CreateVehicle(444, 1269, 1618.9, 10.9, 0);
            CreateVehicle(532, 1269.3, 1639.7, 11.9, 0);
            CreateVehicle(556, 1268.80005, 1606.19995, 10.8, 0);
            CreateVehicle(402, 1615.5, 1569.3, 10.8, 0);
            CreateVehicle(402, 1611.8, 1569.3, 10.8, 0);
            CreateVehicle(411, 1608.1, 1568.9, 10.6, 0);
            CreateVehicle(411, 1604.4, 1568.9, 10.6, 0);
            CreateVehicle(415, 1596.8, 1568.9, 10.7, 0);
            CreateVehicle(415, 1600.2002, 1568.9004, 10.7, 0);
            CreateVehicle(429, 1593.6, 1569, 10.6, 0);
            CreateVehicle(429, 1590.7, 1568.9, 10.6, 0);
            CreateVehicle(451, 1587.6, 1568.9, 10.6, 0);
            CreateVehicle(451, 1584.9, 1568.8, 10.6, 0);
            CreateVehicle(477, 1577, 1568.6, 10.7, 0);
            CreateVehicle(477, 1580.7002, 1568.9004, 10.7, 0);
            CreateVehicle(506, 1570.6, 1568.6, 10.6, 0);
            CreateVehicle(506, 1573.7002, 1568.7002, 10.6, 0);
            CreateVehicle(541, 1621.1, 1577.4, 10.5, 90);
            CreateVehicle(541, 1621.0996, 1574.7998, 10.5, 90);
            CreateVehicle(555, 1621.5, 1580.3, 10.6, 90);
            CreateVehicle(555, 1621.4, 1582.9, 10.6, 90);
            CreateVehicle(559, 1621.4, 1586, 10.6, 90);
            CreateVehicle(559, 1621.2, 1589.1, 10.6, 90);
            CreateVehicle(560, 1621.4, 1592.3, 10.6, 88);
            CreateVehicle(560, 1621.5, 1597.9, 10.6, 87.995);
            CreateVehicle(560, 1621.4, 1595, 10.6, 87.995);
            CreateVehicle(409, 1595.9, 1557.3, 10.7, 0);
            CreateVehicle(409, 1600.7, 1557.8, 10.7, 0);
            CreateVehicle(431, 1598.2, 1590.7, 11.1, 0);
            CreateVehicle(431, 1593.8, 1590.7, 11.1, 0);
            CreateVehicle(448, 1639.6, 1613.6, 10.5, 0);
            CreateVehicle(461, 1638.3, 1613.5, 10.5, 0);
            CreateVehicle(462, 1637.1, 1613.6, 10.5, 0);
            CreateVehicle(463, 1635.8, 1613.8, 10.4, 0);
            CreateVehicle(468, 1634.8, 1614, 10.6, 0);
            CreateVehicle(471, 1633.3, 1613.8, 10.4, 0);
            CreateVehicle(521, 1631.8, 1613.9, 10.5, 0);
            CreateVehicle(522, 1630.3, 1614, 10.5, 0);
            CreateVehicle(523, 1642, 1609.8, 10.5, 0);
            CreateVehicle(581, 1643.5, 1610.1, 10.5, 0);
            CreateVehicle(586, 1645.2, 1610, 10.4, 0);
            CreateVehicle(509, 1648, 1610.2, 10.4, 0);
            CreateVehicle(509, 1646.9004, 1610.2002, 10.4, 0);
            CreateVehicle(481, 1649.2, 1610.3, 10.4, 0);
            CreateVehicle(481, 1650.4, 1610.3, 10.4, 0);
            CreateVehicle(510, 1644.1, 1614.3, 14.5, 0);
            CreateVehicle(510, 1642.9004, 1614, 14.5, 0);
            CreateVehicle(510, 1645.5, 1614.2, 14.5, 0);

            CreateVehicle((int)SampSharp.GameMode.Definitions.VehicleModelType.Infernus, 1582.5277, 1607.2704, 10.8203125, 0);

            /*Abilities.speedUpVAbility.CreatePickup(new Vector3(1528.1932, 1653.2258, 10.822912), 10000);
            Abilities.jumpVAbility.CreatePickup(new Vector3(1518.5077, 1653.8824, 10.823072), 10000);
            Abilities.speedUpVAbility.CreatePickup(new Vector3(1560.6125, 1685.6108, 10.8203125), 10000);
            Abilities.jumpVAbility.CreatePickup(new Vector3(1640.615, 1671.2825, 10.8203125), 10000);
            Abilities.speedUpVAbility.CreatePickup(new Vector3(1660.1886, 1633.8268, 10.8203125), 10000);
            Abilities.jumpVAbility.CreatePickup(new Vector3(1712.7196, 1555.5225, 10.755176), 10000);
            Abilities.speedUpVAbility.CreatePickup(new Vector3(1703.2284, 1655.2985, 10.582462), 10000);*/
            abilitiesSpawner.Setup(new[] {
                    //(Ability)Abilities.jumpVAbility,
                    //(Ability)Abilities.plantMineVAbility,
                    //(Ability)Abilities.repairVAbility,
                    //(Ability)Abilities.speedUpVAbility,
                    (Ability)Abilities.invulnerablePAbility,
                    (Ability)Abilities.superExplodePAbility,
                    (Ability)Abilities.invisibilePAbility,
                    (Ability)Abilities.superPunchPAbility,
                    (Ability)Abilities.restorHealthPAbility,
                    //(Ability)Abilities.jumpPAbility,
                }, new[] {
                    new Vector3(1528.1932, 1653.2258, 10.822912),
                    new Vector3(1518.5077, 1653.8824, 10.823072),
                    new Vector3(1560.6125, 1685.6108, 10.8203125),
                    new Vector3(1640.615, 1671.2825, 10.8203125),
                    new Vector3(1660.1886, 1633.8268, 10.8203125),
                    new Vector3(1712.7196, 1555.5225, 10.755176),
                    new Vector3(1326.1936, 1531.8811, 10.527753),
                    new Vector3(1596.0714, 1476.4114, 10.530005),
                    new Vector3(1503.8698, 1542.1672, 17.539557),//Below is for debug
                    new Vector3(1600.8907, 1594.5449, 10.8203125),
                    new Vector3(1601.3232, 1592.0791, 10.8203125),
                    new Vector3(1601.229,  1588.9583, 10.8203125),
                    new Vector3(1601.0889, 1585.8091, 10.8203125),
                    new Vector3(1604.0168, 1585.5177, 10.8203125),
                    new Vector3(1604.2289, 1588.3243, 10.8203125),
                    new Vector3(1604.0085, 1591.8567, 10.8203125)
                },
                800);
            autoBegin = true;
            WeaponItems.AK47.Spawn(new Vector3(1540.5671, 1615.7604, 10.8203125), new Vector3(0,0,0), 30);
        }

        protected override void OnEnd(List<Player> players)
        {
        
        }

        protected override void OnStart(List<Player> players)
        {
            /*foreach (Player player in players)
            {
                player.Position = 
            }*/
        }
    }
}
