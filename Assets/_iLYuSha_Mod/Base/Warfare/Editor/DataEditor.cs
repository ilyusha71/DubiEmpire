using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Warfare.Unit
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
                    script.JoinDatabase ();
                    Debug.Log ("<color=lime>" + script.name + "</color> has been Joined.");
                    EditorUtility.SetDirty (script);
                    AssetDatabase.SaveAssets ();
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
            DrawDefaultInspector ();
        }
    }
}