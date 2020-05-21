using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Warfare.Unit
{
    // [CanEditMultipleObjects]
    // [CustomEditor (typeof (KocmocraftDatabase))]
    public class DatabaseEditor : EditorWindow
    {
        private Unit.Data source;
        private Vector2 scrollPos;
        private Editor editor;
        private Database database;

        [MenuItem ("Warfare/Warfare Database #F7")]
        public static void ShowDatabaseWindow ()
        {
            var window = EditorWindow.GetWindow<DatabaseEditor> (false, "Warfare Database", true);
            window.database = UnityEditor.AssetDatabase.LoadAssetAtPath<Database> ("Assets/_iLYuSha_Mod/Base/Warfare/Database.asset");
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
            GUILayout.Label ("Warfare Database");

            GUILayout.Space (5);
            GUILayout.BeginHorizontal ();
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.fontSize = 18;
            GUILayout.Label ("Total: ", GUILayout.Width (57));

            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.contentColor = Color.yellow;
            GUILayout.Label (database.units.Count.ToString (), GUILayout.Width (27));

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
            GUI.skin.label.fontSize = 16;
            GUI.contentColor = Color.green;
            GUILayout.BeginHorizontal ();
            GUILayout.Label ("Warfare Unit", GUILayout.Width (200));
            GUILayout.Label ("N", GUILayout.Width (20));
            GUILayout.Space (5);
            GUILayout.Label ("Hr", GUILayout.Width (37));
            GUILayout.Space (5);
            GUILayout.Label ("Price", GUILayout.Width (63));
            GUILayout.Space (5);
            GUILayout.Label ("Total", GUILayout.Width (52));
            GUILayout.Space (5);
            GUILayout.Label ("HP", GUILayout.Width (50));
            GUILayout.Space (10);
            GUILayout.Label ("Sur.", GUILayout.Width (60));
            GUILayout.Space (10);
            GUI.contentColor = Color.white;
            GUILayout.Label ("Data", GUILayout.Width (100));
            GUILayout.EndHorizontal ();

            GUILayout.Space (5);
            foreach (KeyValuePair<Unit.Type, Unit.Data> unit in database.units.ToList ())
            {
                GUILayout.BeginHorizontal ();

                GUI.skin.label.fontSize = 12;
                GUI.skin.label.fontStyle = FontStyle.Bold;
                GUI.contentColor = Color.yellow;
                GUILayout.Label (((int) unit.Key).ToString (), GUILayout.Width (37));

                GUI.skin.label.fontSize = 14;
                GUI.contentColor = Color.white;
                GUILayout.Label (unit.Key.ToString (), GUILayout.Width (163));

                GUI.skin.label.fontSize = 12;
                GUI.skin.label.fontStyle = FontStyle.Normal;
                GUI.contentColor = Color.white;
                GUI.backgroundColor = Color.white;
                EditorGUI.BeginChangeCheck ();
                GUILayout.Label (unit.Value.m_formation.Length.ToString (), GUILayout.Width (20));
                GUILayout.Space (5);
                unit.Value.m_Hour = EditorGUILayout.IntField (unit.Value.m_Hour, GUILayout.Width (37));
                GUILayout.Space (5);
                unit.Value.m_price = EditorGUILayout.IntField (unit.Value.m_price, GUILayout.Width (63));
                GUILayout.Space (5);
                GUILayout.Label ((unit.Value.m_price * unit.Value.m_formation.Length).ToString (), GUILayout.Width (52));
                GUILayout.Space (5);
                unit.Value.m_hp = EditorGUILayout.IntField (unit.Value.m_hp, GUILayout.Width (50));
                GUILayout.Space (10);
                GUILayout.Label ((unit.Value.m_hp * unit.Value.m_formation.Length).ToString (), GUILayout.Width (35));
                if (EditorGUI.EndChangeCheck ())
                {
                    Undo.RecordObject (unit.Value, "Modify Types");
                    EditorUtility.SetDirty (unit.Value);
                }

                GUILayout.Space (10);
                source = EditorGUILayout.ObjectField (unit.Value, typeof (Unit.Data), true, GUILayout.Width (100)) as Unit.Data;

                GUI.backgroundColor = Color.red;
                EditorGUI.BeginChangeCheck ();
                if (GUILayout.Button ("Remove", GUILayout.Width (66)))
                {
                    Debug.Log ("<color=yellow>" + unit.Key.ToString () + "</color> has been <color=#fdb4ca>removed.</color>");
                    database.DeleteKey (unit.Key);
                }
                if (EditorGUI.EndChangeCheck ())
                {
                    Undo.RecordObject (database, "Modify Types");
                    EditorUtility.SetDirty (database);
                }
                GUILayout.EndHorizontal ();
            }
            GUILayout.Space (15);
        }

        // void DrawKocmoraft(int index)
        // {
        //     if (index < 0 || index >= database.kocmocraft.Count)
        //         return;
        //     // BeginChangeCheck() 用來檢查在 BeginChangeCheck() 和 EndChangeCheck() 之間是否有 Inspector 變數改變
        //     EditorGUI.BeginChangeCheck();
        //     GUILayout.BeginHorizontal();
        //     {
        //         GUILayout.Label(database.kocmocraft[index].design.code.ToString(), GUILayout.Width(163));
        //         database.kocmocraft[index].type = (Kocmoca.Type)EditorGUILayout.EnumPopup(database.kocmocraft[index].type, GUILayout.Width(163));
        //         database.kocmocraft[index].turretOption = (TurretOption)EditorGUILayout.EnumPopup(database.kocmocraft[index].turretOption, GUILayout.Width(163));
        //         database.kocmocraft[index].type = (Kocmoca.Type)EditorGUILayout.EnumPopup(database.kocmocraft[index].type, GUILayout.Width(163));
        //         database.kocmocraft[index].type = (Kocmoca.Type)EditorGUILayout.EnumPopup(database.kocmocraft[index].type, GUILayout.Width(163));

        //         // 如果 Inspector 變數有改變，EndChangeCheck() 會回傳 True，才有必要去做變數存取
        //         if (EditorGUI.EndChangeCheck())
        //         {
        //             // 在修改之前建立 Undo/Redo 記錄步驟
        //             Undo.RecordObject(database, "Modify Types");

        //             // database.Types[index].Name = newName;
        //             // database.Types[index].HitColor = newColor;

        //             // 每當直接修改 Inspector 變數，而不是使用 serializedObject 修改時，必須要告訴 Unity 這個 Compoent 已經修改過了
        //             // 在下一次存檔時，必須要儲存這個變數
        //             EditorUtility.SetDirty(database);
        //         }
        //         // if (GUILayout.Button ("Remove"))
        //         // {

        //         // }
        //     }
        //     GUILayout.EndHorizontal();
        // }

        // void DrawTypesInspector ()
        // {
        //     GUILayout.Space (5);
        //     GUILayout.Label ("State", EditorStyles.boldLabel);

        //     for (int i = 0; i < database.Types.Count; i++)
        //     {
        //         DrawType (i);
        //     }

        //     DrawAddTypeButton ();
        // }

        // void DrawType (int index)
        // {
        //     if (index < 0 || index >= database.Types.Count)
        //         return;

        //     GUILayout.BeginHorizontal ();
        //     {
        //         GUILayout.Label ("Name", EditorStyles.label, GUILayout.Width (50));

        //         // BeginChangeCheck() 用來檢查在 BeginChangeCheck() 和 EndChangeCheck() 之間是否有 Inspector 變數改變
        //         EditorGUI.BeginChangeCheck ();
        //         string newName = GUILayout.TextField (database.Types[index].Name, GUILayout.Width (120));
        //         Color newColor = EditorGUILayout.ColorField (database.Types[index].HitColor);

        //         database.Types[index].Name = newName;
        //         database.Types[index].HitColor = newColor;

        //         // 如果 Inspector 變數有改變，EndChangeCheck() 會回傳 True，才有必要去做變數存取
        //         if (EditorGUI.EndChangeCheck ())
        //         {
        //             // 在修改之前建立 Undo/Redo 記錄步驟
        //             Undo.RecordObject (database, "Modify Types");

        //             database.Types[index].Name = newName;
        //             database.Types[index].HitColor = newColor;

        //             // 每當直接修改 Inspector 變數，而不是使用 serializedObject 修改時，必須要告訴 Unity 這個 Compoent 已經修改過了
        //             // 在下一次存檔時，必須要儲存這個變數
        //             EditorUtility.SetDirty (database);
        //         }

        //         if (GUILayout.Button ("Remove"))
        //         {
        //             // 系統會 "登" 一聲
        //             EditorApplication.Beep ();

        //             // 顯示對話框功能(帶有 OK 和 Cancel 兩個按鈕)
        //             if (EditorUtility.DisplayDialog ("Really?", "Do you really want to remove the state '" + database.Types[index].Name + "'?", "Yes", "No") == true)
        //             {
        //                 database.Types.RemoveAt (index);
        //                 EditorUtility.SetDirty (database);
        //             }

        //         }
        //     }
        //     GUILayout.EndHorizontal ();
        // }

        // void DrawAddTypeButton ()
        // {
        //     if (GUILayout.Button ("Add new State", GUILayout.Height (30)))
        //     {
        //         Undo.RecordObject (database, "Add new Type");

        //         database.Types.Add (new BrickType { Name = "New State" });
        //         EditorUtility.SetDirty (database);
        //     }
        // }

    }
}