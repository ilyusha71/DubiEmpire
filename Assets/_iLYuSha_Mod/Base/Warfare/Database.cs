using System.Collections.Generic;
using UnityEngine;

namespace Warfare
{
    [CreateAssetMenu (fileName = "Warfare Unit Database", menuName = "Warfare/Unit/Create Warfare Unit Database")]
    public class Database : ScriptableObject
    {
        public Dictionary<Unit.Type,Unit.Data> units = new Dictionary<Unit.Type, Unit.Data>();
        //public AutoLevelSetting autoLevel;
        // public List<BrickType> Types = new List<BrickType> ();
        public string typeName;
        public LayerMask whatIsPlayer;

        private BoxCollider2D m_boxCollider2D;

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