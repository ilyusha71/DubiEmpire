using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;

namespace Warfare.Unit
{
    [CreateAssetMenu(fileName = "Warfare Unit Database", menuName = "Warfare/Unit/Create Warfare Unit Database")]
    public class Database : ScriptableObject, ISerializationCallbackReceiver
    {
        [HideInInspector]
        public List<Unit.Type> keyList = new List<Unit.Type>();
        [HideInInspector]
        public List<Unit.Data> valueList = new List<Unit.Data>();
        public Dictionary<Unit.Type, Unit.Data> units = new Dictionary<Unit.Type, Unit.Data>();

        public void DeleteKey(Unit.Type key)
        {
            units.Remove(key);
        }
        public void Sort()
        {
            for (int i = 0; i < valueList.Count; i++)
            {

                valueList[i].m_type = valueList[i].model.m_type;
                valueList[i].m_price = valueList[i].model.m_price;
                valueList[i].m_Hour = valueList[i].model.m_Hour;
                valueList[i].m_hp = valueList[i].model.m_hp;
                valueList[i].m_atk = valueList[i].model.m_atk;
                valueList[i].m_power = valueList[i].model.m_power;
                valueList[i].m_fire = valueList[i].model.m_fire;
                valueList[i].m_field = valueList[i].model.m_field;
                valueList[i].m_anti = valueList[i].model.m_anti;
                valueList[i].m_range = valueList[i].model.m_range;
                valueList[i].m_square = valueList[i].model.m_square;
                valueList[i].m_formation = new Vector3[valueList[i].model.m_formation.Length];
                int cc = valueList[i].m_formation.Length;
                for (int kk = 0; kk < cc; kk++)
                {
                    valueList[i].m_formation[kk] = (Vector3)valueList[i].model.m_formation[kk];
                }
            }




            Dictionary<Unit.Type, Unit.Data> dic1Asc = units.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            units = dic1Asc;
        }
        public void OnBeforeSerialize()
        {
            keyList.Clear();
            valueList.Clear();

            foreach (var pair in units)
            {
                keyList.Add(pair.Key);
                valueList.Add(pair.Value);
            }
        }
        public void OnAfterDeserialize()
        {
            units.Clear();

            for (int i = 0; i < keyList.Count; ++i)
            {
                units[keyList[i]] = valueList[i];
            }
        }

#if UNITY_EDITOR
        public void SaveDatabase()
        {
            Debug.Log("<color=yellow>Database has been updated!</color>");
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}