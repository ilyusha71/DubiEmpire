using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace Warfare
{
    [CreateAssetMenu (fileName = "Warfare Manager", menuName = "Warfare/Create Warfare Manager")]
    public class WarfareManager : ScriptableObject
    {
        public Legion.Database legionDB;
        public Unit.Database unitDB;
        public PlayerData playerData;
        public Dictionary<int, Unit.MasterModel> units = new Dictionary<int, Unit.MasterModel> ();

        public void MasterModelCollector ()
        {
            units.Clear ();
            int count = unitDB.units.Count;
            for (int i = 0; i < count; i++)
            {
                Unit.MasterModel model = new Unit.MasterModel (unitDB.valueList[i]);
                units.Add ((int) unitDB.keyList[i], model);
            }
            Debug.Log ("<color=yellow>" + units.Count + " MasterModel</color> has been <color=lime>Updated</color>.");
        }
        public void SynchronizeLegionsToPlayerData ()
        {
            Dictionary<int, Legion.Data> legions = legionDB.legions;
            List<int> keys = legions.Keys.ToList ();
            int dataCount = keys.Count;
            for (int index = 0; index < dataCount; index++)
            {
                int id = keys[index];
                Legion.DataModel legion = new Legion.DataModel (id);
                if (playerData.legions.ContainsKey (id))
                    playerData.legions[id] = legion;
                else
                    playerData.legions.Add (id, legion);

                for (int order = 0; order < legions[id].m_squadron.Length; order++)
                {
                    int type = (int) legions[id].m_squadron[order].type;
                    if (type == 0)
                        continue;
                    Unit.DataModel unit = new Unit.DataModel ();
                    unit.Type = type;
                    unit.HP = legions[id].m_squadron[order].HP;
                    // unit.Level = legions[id].m_squadron[order].HP;
                    // unit.Exp = legions[id].m_squadron[order].HP;
                    legion.squadron.Add (order, unit);
                }
            }
        }
        public void SynchronizeLegionToPlayerData (int index)
        {
            Dictionary<int, Legion.Data> legions = legionDB.legions;
            int id = index;
            Legion.DataModel legion = new Legion.DataModel (id);
            if (playerData.legions.ContainsKey (id))
                playerData.legions[id] = legion;
            else
                playerData.legions.Add (id, legion);

            for (int order = 0; order < legions[id].m_squadron.Length; order++)
            {
                int type = (int) legions[id].m_squadron[order].type;
                if (type == 0)
                    continue;
                Unit.DataModel unit = new Unit.DataModel ();
                unit.Type = type;
                unit.HP = legions[id].m_squadron[order].HP;
                // unit.Level = legions[id].m_squadron[order].HP;
                // unit.Exp = legions[id].m_squadron[order].HP;
                legion.squadron.Add (order, unit);
            }
        }
        public void SynchronizeLegionSquadronToPlayerData (int index, int order)
        {
            Dictionary<int, Legion.Data> legions = legionDB.legions;
            int id = index;
            Legion.DataModel legion;
            if (playerData.legions.ContainsKey (id))
            {
                legion = playerData.legions[id];
                legion.squadron.Remove (order);
            }
            else
            {
                legion = new Legion.DataModel (id);
                playerData.legions.Add (index, legion);
            }
            int type = (int) legions[id].m_squadron[order].type;
            Unit.DataModel unit = new Unit.DataModel ();
            unit.Type = type;
            unit.HP = legions[id].m_squadron[order].HP;
            // unit.Level = legions[id].m_squadron[order].HP;
            // unit.Exp = legions[id].m_squadron[order].HP;
            legion.squadron.Add (order, unit);
        }
        public void SynchronizeUnitsToPlayerData ()
        {
            Dictionary<int, Legion.Data> legions = legionDB.legions;
            List<int> keys = legions.Keys.ToList ();
            int dataCount = keys.Count;
            for (int index = 0; index < dataCount; index++)
            {
                int id = keys[index];
                if (id < 9900) continue;

                for (int order = 0; order < legions[id].m_squadron.Length; order++)
                {
                    int type = (int) legions[id].m_squadron[order].type;
                    if (type == 0)
                        continue;
                    Unit.DataModel unit = new Unit.DataModel ();
                    unit.Type = type;
                    unit.HP = legions[id].m_squadron[order].HP;
                    // unit.Level = legions[id].m_squadron[order].HP;
                    // unit.Exp = legions[id].m_squadron[order].HP;
                    playerData.units.Add (unit);
                }
            }
        }
        public bool Save (int index)
        {
            BinaryFormatter bf = new BinaryFormatter ();
            Stream s = File.Open (Application.dataPath + "/Save" + index + ".wak", FileMode.Create);
            bf.Serialize (s, playerData);
            s.Close ();
            Debug.Log ("Save");
            return true;
        }
        public bool Load (int index)
        {
            if (!System.IO.File.Exists (Application.dataPath + "/Save" + index + ".wak"))
            {
                Debug.LogWarning ("Bug");
                System.IO.File.Create (Application.dataPath + "/Save" + index + ".wak").Dispose ();
            }
            BinaryFormatter bf = new BinaryFormatter ();
            Stream s = File.Open (Application.dataPath + "/Save" + index + ".wak", FileMode.Open);
            playerData = (PlayerData) bf.Deserialize (s);
            s.Close ();
            Debug.Log ("Load");
            return true;
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public Dictionary<int, Legion.DataModel> legions = new Dictionary<int, Legion.DataModel> ();
        public List<Unit.DataModel> units = new List<Unit.DataModel> ();
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

    public static class Property
    {
        public static string Type (int type)
        {
            switch (type)
            {
                case 1001:
                    return "神偷机兵";
                case 1002:
                    return "邪恶机兵";
                case 1011:
                    return "红牛能量";
                case 1012:
                    return "魔爪能量";
                case 1021:
                    return "普鲸";
                case 1022:
                    return "凯尔鲸";
                case 1031:
                    return "纸飞机";
                case 1041:
                    return "咕咕鸡";
                case 1042:
                    return "紫燕";
                case 1051:
                    return "炮弹比尔";
                case 1052:
                    return "苏打炮弹";
                case 1061:
                    return "时光机MK.I";
                case 1062:
                    return "时光机MK.II";
                case 1071:
                    return "王牌狗屋";
                case 1081:
                    return "卡比之星";
                case 1091:
                    return "血之蝎";
                case 1092:
                    return "苍之蝎";
                case 1101:
                    return "恩威迪亚";
                case 1111:
                    return "快餐侠";
                case 1121:
                    return "圣诞驯鹿";
                case 1122:
                    return "极地驯鹿";
                case 1131:
                    return "北极星特快";
                case 1132:
                    return "养乐多快线";
                case 1141:
                    return "虚空飞鱼";
                case 1142:
                    return "化骨鱼";
                case 1151:
                    return "玩具独角兽";
                case 1161:
                    return "幽灵南瓜";
                case 1171:
                    return "赏金猎人";
                case 1181:
                    return "鹰纽特";
                case 1191:
                    return "新葡鲸";
                case 1192:
                    return "粉红葡鲸";
                case 1193:
                    return "朱红普鲸";
                case 1201:
                    return "捣蛋财神";
                case 2001:
                    return "小小兵戴夫";
                case 2002:
                    return "邪恶小小兵";
                case 2011:
                    return "维特尔";
                case 2012:
                    return "王尼玛";
                case 2021:
                    return "大毛";
                case 2022:
                    return "萌总";
                case 2031:
                    return "阿楞";
                case 2061:
                    return "哆啦啦";
                case 2062:
                    return "迷你哆啦啦";
                case 2091:
                    return "无面人";
                case 2101:
                    return "熊猫人张学友";
                case 2102:
                    return "熊猫人金馆长";
                case 2111:
                    return "阿痞";
                case 2112:
                    return "Awesom-O 4000";
                case 2131:
                    return "滑稽";
                case 2141:
                    return "海绵宝宝";
                case 2142:
                    return "粉红海绵宝宝";
                case 2151:
                    return "公主阿尼";
                case 2171:
                    return "凯子";
                case 2181:
                    return "印第安屎蛋";
                case 2182:
                    return "屎蛋";
                case 2191:
                    return "安格瑞";
            }
            return "";
        }
        public static string Range (Unit.Range range)
        {
            switch (range)
            {
                case Unit.Range.Near:
                    return "近程";
                case Unit.Range.Medium:
                    return "中程";
                case Unit.Range.Far:
                    return "远程";
            }
            return "";
        }
    }
}