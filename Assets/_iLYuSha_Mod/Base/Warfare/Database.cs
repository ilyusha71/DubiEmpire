using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Warfare
{
    [CreateAssetMenu (fileName = "Warfare Unit Database", menuName = "Warfare/Unit/Create Warfare Unit Database")]
    public class Database : ScriptableObject, ISerializationCallbackReceiver
    {
        [HideInInspector]
        public List<Unit.Type> keyList = new List<Unit.Type> ();
        [HideInInspector]
        public List<Unit.Data> valueList = new List<Unit.Data> ();
        public Dictionary<Unit.Type, Unit.Data> units = new Dictionary<Unit.Type, Unit.Data> ();

        public void DeleteKey (Unit.Type key)
        {
            units.Remove (key);
        }
        public void Sort ()
        {
            Dictionary<Unit.Type, Unit.Data> dic1Asc = units.OrderBy (o => o.Key).ToDictionary (o => o.Key, p => p.Value);
            units = dic1Asc;

        }
        public void OnBeforeSerialize ()
        {
            keyList.Clear ();
            valueList.Clear ();

            foreach (var pair in units)
            {
                keyList.Add (pair.Key);
                valueList.Add (pair.Value);
            }
        }
        public void OnAfterDeserialize ()
        {
            units.Clear ();

            for (int i = 0; i < keyList.Count; ++i)
            {
                units[keyList[i]] = valueList[i];
            }
        }

#if UNITY_EDITOR
        public void SaveDatabase ()
        {
            //for (int i = 0; i < 20; i++)
            //{
            //    UnityEditor.AssetDatabase.RenameAsset("Assets/_iLYuSha Wakaka Setting/ScriptableObject/Kocmocraft Module " + i + ".asset", index.kocmocraft[i + 20].name + ".asset");
            //}
            Debug.Log ("<color=yellow>Database has been updated!</color>");
            UnityEditor.AssetDatabase.SaveAssets ();
        }
#endif
    }
}