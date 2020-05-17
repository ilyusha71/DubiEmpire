using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kocmoca
{
    public class Protodesign : MonoBehaviour
    {

        [Header("Model")]
        public Mesh mesh;
        public Vector3 centre;
        public Vector3 size;

        public void Reset()
        {
            // 若模型有子物件，生成一個合併的Mesh，並用於後續計算使用
            Transform model = transform.GetChild(0); // 2 = Painting I
            if (model.childCount > 0)
                mesh = CombineMesh(model.gameObject);
            // else if (model.childCount == 1)
            //     mesh = model.GetComponentInChildren<MeshFilter> ().sharedMesh;
            else
                mesh = model.GetComponent<MeshFilter>().sharedMesh;
            centre = mesh.bounds.center;
            size = mesh.bounds.size;
            AlignBase();
            Debug.Log("<color=lime>" + name + " data has been preset.</color>");
        }

        /* 網格合併 */
        public Mesh CombineMesh(GameObject obj)
        {
            string MESH_PATH = "Assets/_iLYuSha_Mod/Wakaka Kocmocraft/Meshes/";
            if (obj.GetComponent<MeshRenderer>() == null)
            {
                obj.AddComponent<MeshRenderer>();
            }
            if (obj.GetComponent<MeshFilter>() == null)
            {
                obj.AddComponent<MeshFilter>();
            }

            List<Material> material = new List<Material>();
            Matrix4x4 matrix = obj.transform.worldToLocalMatrix;
            MeshFilter[] filters = obj.GetComponentsInChildren<MeshFilter>();
            int filterLength = filters.Length;
            CombineInstance[] combine = new CombineInstance[filterLength];
            for (int i = 0; i < filterLength; i++)
            {
                MeshFilter filter = filters[i];
                MeshRenderer render = filter.GetComponent<MeshRenderer>();
                if (render == null)
                {
                    continue;
                }
                if (render.sharedMaterial != null && !material.Contains(render.sharedMaterial))
                {
                    material.Add(render.sharedMaterial);
                }
                combine[i].mesh = filter.sharedMesh;
                //对坐标系施加变换的方法是 当前对象和子对象在世界空间中的矩阵 左乘 当前对象从世界空间转换为本地空间的变换矩阵
                //得到当前对象和子对象在本地空间的矩阵。
                combine[i].transform = matrix * filter.transform.localToWorldMatrix;
                // render.enabled = false;
            }

            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.name = "Combine";
            //合并Mesh
            mesh.CombineMeshes(combine);
            meshFilter.sharedMesh = mesh;
            //合并第二套UV
            Unwrapping.GenerateSecondaryUVSet(meshFilter.sharedMesh);
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            renderer.sharedMaterials = material.ToArray();
            renderer.enabled = true;

            MeshCollider collider = new MeshCollider();
            if (collider != null)
            {
                collider.sharedMesh = mesh;
            }
            string tempPath = MESH_PATH + obj.name + "_mesh.asset";
            AssetDatabase.CreateAsset(meshFilter.sharedMesh, tempPath);
            //PrefabUtility.DisconnectPrefabInstance(obj);
            Mesh target = meshFilter.sharedMesh;
            DestroyImmediate(obj.GetComponent<MeshFilter>());
            DestroyImmediate(obj.GetComponent<MeshRenderer>());
            return target;
        }
        /* 對齊質心 */
        public void AlignCentre()
        {
            transform.GetChild(0).localPosition = -centre;
        }
        /* 對齊底座 */
        public void AlignBase()
        {
            transform.GetChild(0).localPosition = -centre + new Vector3(0, 0.5f * size.y, 0);
        }
        public float GetScalePower()
        {
            // float max = Mathf.Max(0, size.x);
            // max = Mathf.Max(max, size.y);
            // max = Mathf.Max(max, size.z);
            float max = Mathf.Sqrt(size.x * size.z);
            Debug.Log(max);
            return 12.0f / max;
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Protodesign))]
    public class ProtodesignEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var scripts = targets.OfType<Protodesign>();
            if (GUILayout.Button("Align Centre"))
                foreach (var script in scripts)
                    script.AlignCentre();
            if (GUILayout.Button("Align Base"))
                foreach (var script in scripts)
                    script.AlignBase();
        }
    }
#endif
}