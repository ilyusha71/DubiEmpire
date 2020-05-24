using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Warfare.Unit
{
    public class DatabaseEditor : EditorWindow
    {
        private Unit.Data source;
        private Vector2 scrollPos;
        private Editor editor;
        private Database database;

        [MenuItem("Warfare/Unit Database #F7")]
        public static void ShowDatabaseWindow()
        {
            var window = EditorWindow.GetWindow<DatabaseEditor>(false, "Unit Database", true);
            window.database = UnityEditor.AssetDatabase.LoadAssetAtPath<Database>("Assets/_iLYuSha_Mod/Base/Warfare/Unit/Database.asset");
            window.editor = Editor.CreateEditor(window.database);
        }
        public void OnGUI()
        {
            if (!editor)
                ShowDatabaseWindow();
            EditorGUILayout.BeginVertical(GUILayout.MinHeight(position.height));
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            // this.editor.OnInspectorGUI();
            DrawDatabaseInspector();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void DrawDatabaseInspector()
        {
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.fontSize = 21;
            GUILayout.Label("Warfare Unit Database");

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            {
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                GUI.skin.label.fontStyle = FontStyle.Normal;
                GUI.skin.label.fontSize = 18;
                GUILayout.Label("Total: ", GUILayout.Width(55));

                GUI.skin.label.fontStyle = FontStyle.Bold;
                GUI.contentColor = Color.cyan;
                GUILayout.Label(database.units.Count.ToString(), GUILayout.Width(50));

                GUI.backgroundColor = Color.red;
                GUI.contentColor = Color.white;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(database, "Modify Types");
                if (GUILayout.Button("Sort", GUILayout.Height(25), GUILayout.Width(66)))
                    database.Sort();
                GUI.backgroundColor = Color.white;
                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(database);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                GUI.skin.label.fontSize = 16;
                GUI.contentColor = Color.green;
                GUILayout.Label("Warfare Unit", GUILayout.Width(210));
                GUILayout.Label("N", GUILayout.Width(20));
                GUILayout.Space(5);
                GUI.contentColor = Color.white;
                GUILayout.Label("Hr", GUILayout.Width(37));
                GUILayout.Space(5);
                GUILayout.Label("Price", GUILayout.Width(63));
                GUILayout.Space(5);
                GUI.contentColor = Color.green;
                GUILayout.Label("Total", GUILayout.Width(52));
                GUILayout.Space(5);
                GUILayout.Label("HP", GUILayout.Width(50));
                GUILayout.Space(10);
                GUILayout.Label("Sur.", GUILayout.Width(35));
                GUILayout.Space(10);
                GUI.contentColor = Color.white;
                GUILayout.Label("Data", GUILayout.Width(100));
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            foreach (KeyValuePair<Unit.Type, Unit.Data> unit in database.units.ToList())
            {
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(unit.Value, "Modify Types");

                GUILayout.BeginHorizontal();
                {
                    GUI.skin.label.fontSize = 12;
                    GUI.skin.label.fontStyle = FontStyle.Bold;
                    GUI.contentColor = Color.yellow;
                    GUILayout.Label(((int)unit.Key).ToString(), GUILayout.Width(47));

                    GUI.skin.label.fontSize = 14;
                    GUI.contentColor = Color.white;
                    GUILayout.Label(unit.Key.ToString(), GUILayout.Width(163));

                    GUI.skin.label.fontSize = 12;
                    GUI.skin.label.fontStyle = FontStyle.Normal;
                    GUI.contentColor = Color.white;

                    GUILayout.Label(unit.Value.m_formation.Length.ToString(), GUILayout.Width(20));
                    GUILayout.Space(5);
                    unit.Value.m_Hour = EditorGUILayout.IntField(unit.Value.m_Hour, GUILayout.Width(37));
                    GUILayout.Space(5);
                    unit.Value.m_price = EditorGUILayout.IntField(unit.Value.m_price, GUILayout.Width(63));
                    GUILayout.Space(5);
                    GUILayout.Label((unit.Value.m_price * unit.Value.m_formation.Length).ToString(), GUILayout.Width(52));
                    GUILayout.Space(5);
                    unit.Value.m_hp = EditorGUILayout.IntField(unit.Value.m_hp, GUILayout.Width(50));
                    GUILayout.Space(10);
                    GUILayout.Label((unit.Value.m_hp * unit.Value.m_formation.Length).ToString(), GUILayout.Width(35));
                    GUILayout.Space(10);
                    source = EditorGUILayout.ObjectField(unit.Value, typeof(Unit.Data), true, GUILayout.Width(100)) as Unit.Data;

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Remove", GUILayout.Width(66)))
                    {
                        Debug.Log("<color=yellow>" + unit.Key.ToString() + "</color> has been <color=#fdb4ca>removed.</color>");
                        Undo.RecordObject(database, "Modify Types");
                        database.DeleteKey(unit.Key);
                    }
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(unit.Value);
                    EditorUtility.SetDirty(database);
                }
            }
            GUILayout.Space(15);
        }
    }
}