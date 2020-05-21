using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Warfare.Legion
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof (Data))]
    public class DataEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            var scripts = targets.OfType<Data> ();
            if (GUILayout.Button ("Join Database"))
            {
                foreach (var script in scripts)
                {
                    script.SetIndex ();
                    Database database = AssetDatabase.LoadAssetAtPath<Database> ("Assets/_iLYuSha_Mod/Base/Warfare/Legion/Database.asset");
                    if (!database.legions.ContainsKey (script.m_index))
                    {
                        database.legions.Add (script.m_index, script);
                        Debug.Log ("<color=yellow>" + script.m_index.ToString () + "</color> has been <color=lime>Joined</color>.");
                        AssetDatabase.RenameAsset (AssetDatabase.GetAssetPath (script), script.m_index.ToString());
                    }
                    else
                    {
                        database.legions[script.m_index] = script;
                        Debug.Log ("<color=yellow>" + script.m_index.ToString () + "</color> has been <color=cyan>Updated</color>.");
                    }
                    EditorUtility.SetDirty (script);
                    EditorUtility.SetDirty (database);
                    AssetDatabase.SaveAssets ();
                }
            }
            DrawDefaultInspector ();
        }
    }
}