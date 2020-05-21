using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Warfare.Legion
{
    public class DatabaseEditor : EditorWindow
    {
        private Legion.Data source;
        private Vector2 scrollPos;
        private Editor editor;
        private Database database;

        [MenuItem ("Warfare/Legion Database #F6")]
        public static void ShowDatabaseWindow ()
        {
            var window = EditorWindow.GetWindow<DatabaseEditor> (false, "Legion Database", true);
            window.database = UnityEditor.AssetDatabase.LoadAssetAtPath<Database> ("Assets/_iLYuSha_Mod/Base/Warfare/Legion/Database.asset");
            window.editor = Editor.CreateEditor (window.database);
        }
        public void OnGUI ()
        {
            if (!editor)
                ShowDatabaseWindow ();
            EditorGUILayout.BeginVertical (GUILayout.MinHeight (position.height));
            scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

            // this.editor.OnInspectorGUI();
            DrawDatabaseInspector ();

            EditorGUILayout.EndScrollView ();
            EditorGUILayout.EndVertical ();
        }

        void DrawDatabaseInspector ()
        {
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.fontSize = 21;
            GUILayout.Label ("Warfare Legion Database");

            GUILayout.Space (5);
            GUILayout.BeginHorizontal ();
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.fontSize = 18;
            GUILayout.Label ("Total: ", GUILayout.Width (57));

            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.contentColor = Color.yellow;
            GUILayout.Label (database.legions.Count.ToString (), GUILayout.Width (27));

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            EditorGUI.BeginChangeCheck ();
            if (GUILayout.Button ("Sort", GUILayout.Width (66)))
                database.Sort ();
            if (EditorGUI.EndChangeCheck ())
            {
                Undo.RecordObject (database, "Modify Types");
                EditorUtility.SetDirty (database);
            }

            GUILayout.EndHorizontal ();

            GUILayout.Space (5);
            foreach (KeyValuePair<int, Legion.Data> legion in database.legions.ToList ())
            {
                // Faction Title
                if (legion.Key % 100 == 0)
                {
                    GUILayout.Space (20);
                    GUILayout.BeginHorizontal ();
                    GUI.skin.label.fontStyle = FontStyle.Bold;
                    GUI.skin.label.fontSize = 21;
                    GUILayout.Label (legion.Value.m_faction.ToString ());
                    GUILayout.EndHorizontal ();
                    GUILayout.Space (5);
                }
                else
                {
                    GUI.skin.label.fontSize = 16;
                    GUI.skin.label.fontStyle = FontStyle.Bold;
                    GUI.contentColor = Color.green;
                    switch (legion.Key % 10)
                    {
                        case 1:
                            GUILayout.Label ((legion.Value.m_legion).ToString () + "st Legion", GUILayout.Width (200));
                            break;
                        case 2:
                            GUILayout.Label ((legion.Value.m_legion).ToString () + "nd Legion", GUILayout.Width (200));
                            break;
                        case 3:
                            GUILayout.Label ((legion.Value.m_legion).ToString () + "rd Legion", GUILayout.Width (200));
                            break;
                        default:
                            GUILayout.Label ((legion.Value.m_legion).ToString () + "th Legion", GUILayout.Width (200));
                            break;
                    }
                    EditorGUI.BeginChangeCheck ();
                    GUI.contentColor = Color.white;
                    GUILayout.BeginHorizontal ();
                    GUILayout.FlexibleSpace ();
                    legion.Value.m_squadron[0].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[0].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    legion.Value.m_squadron[1].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[1].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    legion.Value.m_squadron[2].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[2].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    GUILayout.FlexibleSpace ();
                    GUILayout.EndHorizontal ();
                    GUILayout.BeginHorizontal ();
                    GUILayout.FlexibleSpace ();
                    legion.Value.m_squadron[9].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[9].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    legion.Value.m_squadron[10].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[10].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    GUILayout.Space (30);
                    legion.Value.m_squadron[3].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[3].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    legion.Value.m_squadron[4].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[4].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    legion.Value.m_squadron[5].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[5].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    GUILayout.Space (30);
                    legion.Value.m_squadron[11].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[11].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    legion.Value.m_squadron[12].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[12].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    GUILayout.FlexibleSpace ();
                    GUILayout.EndHorizontal ();
                    GUILayout.BeginHorizontal ();
                    GUILayout.FlexibleSpace ();
                    legion.Value.m_squadron[6].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[6].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    legion.Value.m_squadron[7].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[7].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    legion.Value.m_squadron[8].m_texture = EditorGUILayout.ObjectField (legion.Value.m_squadron[8].m_texture, typeof (Texture), true, GUILayout.Height (100), GUILayout.Width (100)) as Texture;
                    GUILayout.FlexibleSpace ();
                    GUILayout.EndHorizontal ();

                    // for (int i = 0; i < 13; i++)
                    // {
                    //     legion.Value.m_squadron[i].SetType ();
                    // }
                    if (EditorGUI.EndChangeCheck ())
                    {
                        Undo.RecordObject (database, "Modify Types");
                        EditorUtility.SetDirty (legion.Value);
                    }
                }
            }
        }
    }
}