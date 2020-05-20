using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Warfare.Unit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Data))]
    public class DataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var scripts = targets.OfType<Data>();
            if (GUILayout.Button("Join Database"))
            {
                foreach (var script in scripts)
                {
                    if (!script.m_instance)
                        return;
                    script.SetType();
                    script.SetFormation();
                    Database database = AssetDatabase.LoadAssetAtPath<Database>("Assets/_iLYuSha_Mod/Base/Warfare/Database.asset");
                    if (!database.units.ContainsKey(script.m_type))
                    {
                        database.units.Add(script.m_type, script);
                        Debug.Log("<color=yellow>" + script.m_type.ToString() + "</color> has been <color=lime>Joined</color>.");
                        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(script), script.m_instance.name);
                    }
                    else
                    {
                        database.units[script.m_type] = script;
                        Debug.Log("<color=yellow>" + script.m_type.ToString() + "</color> has been <color=cyan>Updated</color>.");
                    }
                    EditorUtility.SetDirty(script);
                    EditorUtility.SetDirty(database);
                    AssetDatabase.SaveAssets();
                }
            }
            /* 以下無法觸發多項目批量編輯 */
            //var myTarget = (KocmocraftModule)target;
            //if (GUILayout.Button("Save Database"))
            //{
            //    Debug.Log(myTarget.name);
            //    EditorUtility.SetDirty(myTarget);
            //    AssetDatabase.SaveAssets();
            //}
            DrawDefaultInspector();
        }
    }
}