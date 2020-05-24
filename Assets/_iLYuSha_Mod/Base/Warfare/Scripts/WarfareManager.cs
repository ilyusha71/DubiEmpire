using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Warfare
{
    [CreateAssetMenu(fileName = "Warfare Manager", menuName = "Warfare/Create Warfare Manager")]
    public class WarfareManager : ScriptableObject
    {
        public Legion.Database legionDB;
        public Unit.Database unitDB;
        public PlayerData playerData;
    }

    [System.Serializable]
    public class PlayerData
    {
        // 各軍團小隊單位
        public Dictionary<int, Warfare.Unit.Model> squadrons = new Dictionary<int, Warfare.Unit.Model>();
        // 尚未編制單位
        public List<Unit.Model> units = new List<Unit.Model>();
        public int[] battlefield = new int[2];
    }

    public enum Faction
    {
        Experimental = 0,
        Wakaka = 10,
        NO1 = 11,
        NO2 = 12,
        NO3 = 13,
        NO4 = 14,
        NO5 = 15,
        Reserve = 99,
    }
}