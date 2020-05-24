﻿using System.Linq;
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
                    if (!script.m_instance && !script.m_sprite)
                        return;
                    script.SetType();
                    script.SetFormation();
                    if (script.m_instance)
                        script.m_sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_iLYuSha_Mod/Base/Warfare/Icon/" + script.m_instance.name + ".png");
                    else if (script.m_sprite)
                    {
                        string path = (int)script.m_type < 2000 ? "Assets/_iLYuSha_Mod/Wakaka Kocmocraft/Prefabs/Design/" : "Assets/_iLYuSha_Mod/Wakaka Character/Prefabs/Design/";
                        script.m_instance = AssetDatabase.LoadAssetAtPath<GameObject>(path + script.m_sprite.name + ".prefab");
                    }
                    Database database = AssetDatabase.LoadAssetAtPath<Database>("Assets/_iLYuSha_Mod/Base/Warfare/Unit/Database.asset");
                    if (!database.units.ContainsKey(script.m_type))
                    {
                        database.units.Add(script.m_type, script);
                        Debug.Log("<color=yellow>" + script.m_type.ToString() + "</color> has been <color=lime>Joined</color>.");
                        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(script), script.m_instance.name);
                    }
                    else
                    {
                        if (database.units[script.m_type].name != script.m_instance.name)
                        {
                            database.units[script.m_type] = script;
                            Debug.Log("<color=yellow>" + script.m_type.ToString() + "</color> has been <color=cyan>Updated</color>.");
                            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(script), script.m_instance.name);
                        }
                    }
                    EditorUtility.SetDirty(script);
                    EditorUtility.SetDirty(database);
                    AssetDatabase.SaveAssets();
                }
            }
            DrawDefaultInspector();
        }
    }
}