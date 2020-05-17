using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kocmoca
{
    public class Prototype : MonoBehaviour
    {
      
        [Header ("Model")]
        public Mesh mesh;
        public Vector3 centre;
        public Vector3 size;
        [Header ("Painting")]
        public GameObject[] painting;
        [SerializeField]
        private int countPainting;
        [SerializeField]
        private int nowPainting = 1;
        [SerializeField]
        private int lastPainting = 1;
        private readonly int mainPainting = 2;
        [Header ("FreeLook")]
        public CinemachineFreeLook cmFreeLook;

        public void Reset ()
        {
            // 若模型有子物件，生成一個合併的Mesh，並用於後續計算使用
            Transform model = transform.GetChild (2); // 2 = Painting I
            if (model.childCount > 0)
                mesh = CombineMesh (model.gameObject);
            // else if (model.childCount == 1)
            //     mesh = model.GetComponentInChildren<MeshFilter> ().sharedMesh;
            else
                mesh = model.GetComponent<MeshFilter> ().sharedMesh;
            centre = mesh.bounds.center;
            size = mesh.bounds.size;

            CheckPainting ();
            Debug.Log ("<color=lime>" + name + " data has been preset.</color>");
        }

        /* 網格合併 */
        public Mesh CombineMesh (GameObject obj)
        {
            string MESH_PATH = "Assets/___iLYuSha_Mod/Wakaka Kocmocraft/Meshes/";
            if (obj.GetComponent<MeshRenderer> () == null)
            {
                obj.AddComponent<MeshRenderer> ();
            }
            if (obj.GetComponent<MeshFilter> () == null)
            {
                obj.AddComponent<MeshFilter> ();
            }

            List<Material> material = new List<Material> ();
            Matrix4x4 matrix = obj.transform.worldToLocalMatrix;
            MeshFilter[] filters = obj.GetComponentsInChildren<MeshFilter> ();
            int filterLength = filters.Length;
            CombineInstance[] combine = new CombineInstance[filterLength];
            for (int i = 0; i < filterLength; i++)
            {
                MeshFilter filter = filters[i];
                MeshRenderer render = filter.GetComponent<MeshRenderer> ();
                if (render == null)
                {
                    continue;
                }
                if (render.sharedMaterial != null && !material.Contains (render.sharedMaterial))
                {
                    material.Add (render.sharedMaterial);
                }
                combine[i].mesh = filter.sharedMesh;
                //对坐标系施加变换的方法是 当前对象和子对象在世界空间中的矩阵 左乘 当前对象从世界空间转换为本地空间的变换矩阵
                //得到当前对象和子对象在本地空间的矩阵。
                combine[i].transform = matrix * filter.transform.localToWorldMatrix;
                // render.enabled = false;
            }

            MeshFilter meshFilter = obj.GetComponent<MeshFilter> ();
            Mesh mesh = new Mesh ();
            mesh.name = "Combine";
            //合并Mesh
            mesh.CombineMeshes (combine);
            meshFilter.sharedMesh = mesh;
            //合并第二套UV
            Unwrapping.GenerateSecondaryUVSet (meshFilter.sharedMesh);
            MeshRenderer renderer = obj.GetComponent<MeshRenderer> ();
            renderer.sharedMaterials = material.ToArray ();
            renderer.enabled = true;

            MeshCollider collider = new MeshCollider ();
            if (collider != null)
            {
                collider.sharedMesh = mesh;
            }
            string tempPath = MESH_PATH + obj.name + " _mesh.asset";
            AssetDatabase.CreateAsset (meshFilter.sharedMesh, tempPath);
            //PrefabUtility.DisconnectPrefabInstance(obj);
            Mesh target = meshFilter.sharedMesh;
            DestroyImmediate (obj.GetComponent<MeshFilter> ());
            DestroyImmediate (obj.GetComponent<MeshRenderer> ());
            return target;
        }
        /* 對齊質心 */
        public void AlignCentre ()
        {
            for (int i = 0; i < countPainting; i++)
            {
                painting[i].transform.localPosition = -centre;
            }
        }
        /* 對齊底座 */
        public void AlignBase ()
        {
            for (int i = 0; i < countPainting; i++)
            {
                painting[i].transform.localPosition = -centre + new Vector3 (0, 0.5f * size.y, 0);
            }
        }

        #region Painting
        void CheckPainting ()
        {
            countPainting = transform.childCount - 1; // 扣除 Free Look Camera
            painting = new GameObject[countPainting];
            for (int i = 0; i < countPainting; i++)
            {
                painting[i] = transform.GetChild (i).gameObject;
                painting[i].SetActive (true);
                Transform[] objects = painting[i].GetComponentsInChildren<Transform> ();
                for (int j = 0; j < objects.Length; j++)
                {
                    objects[j].gameObject.layer = 9;
                }
                if (i != 0 && i != mainPainting)
                    painting[i].SetActive (false);
            }
            nowPainting = mainPainting;
            gameObject.layer = 9;
        }
        public void LoadPainting (int order)
        {
            nowPainting = order == 0 ? mainPainting : order; // painting[2] = Painting I
            for (int i = 1; i < countPainting; i++) // painting[0] 為 Collider
            {
                painting[i].SetActive (false);
            }
            painting[nowPainting].SetActive (true);
        }
        public int ChangePainting ()
        {
            nowPainting = (int) Mathf.Repeat (++nowPainting, painting.Length) == 0 ? mainPainting : nowPainting;
            for (int i = 1; i < countPainting; i++) // painting[0] 為 Collider
            {
                painting[i].SetActive (false);
            }
            painting[nowPainting].SetActive (true);
            return nowPainting;
        }
        public void SwitchWireframe ()
        {
            if (nowPainting == 1)
                nowPainting = lastPainting;
            else
            {
                lastPainting = nowPainting;
                nowPainting = 1;
            }
            for (int i = 1; i < countPainting; i++) // painting[0] 為 Collider
            {
                painting[i].SetActive (false);
            }
            painting[nowPainting].SetActive (true);
        }
        public int GetRandomPaintingIndex ()
        {
            return Random.Range (mainPainting, countPainting);
        }
        public void RandomPainting ()
        {
            nowPainting = Random.Range (mainPainting, countPainting);
            for (int i = 1; i < countPainting; i++) // painting[0] 為 Collider
            {
                painting[i].SetActive (false);
            }
            painting[nowPainting].SetActive (true);
        }
        #endregion

        #region Free Look
        public void CreatePrototypeDatabase ()
        {
            Vector3 size = GetComponent<BoxCollider> ().size;
            float wingspan = size.x;
            float length = size.z;
            float height = size.y;
            float max = wingspan > length ? (wingspan > height ? wingspan : height) : (length > height ? length : height);
            float wingspanScale = wingspan / max;
            float lengthScale = length / max;
            float heightScale = height / max;
            float orthoSize = max * 0.5f;
            float near = orthoSize + 2.7f;
            float far = orthoSize + 19.3f;
            cmFreeLook = GetComponentInChildren<CinemachineFreeLook> ();
            cmFreeLook.enabled = true;
            cmFreeLook.Follow = transform;
            cmFreeLook.LookAt = transform;
            cmFreeLook.m_Lens.FieldOfView = 60;
            cmFreeLook.m_BindingMode = CinemachineTransposer.BindingMode.LockToTarget;
            cmFreeLook.m_Orbits[0].m_Height = orthoSize + 3; //sizeView.Height * 0.5f + 5;
            cmFreeLook.m_Orbits[1].m_Height = 0;
            cmFreeLook.m_Orbits[2].m_Height = -orthoSize - 3; //-sizeView.Height;
            cmFreeLook.m_Orbits[0].m_Radius = 11; //sizeView.NearView + 2;
            cmFreeLook.m_Orbits[1].m_Radius = 11; //sizeView.HalfSize + 15;
            cmFreeLook.m_Orbits[2].m_Radius = 11; //sizeView.NearView + 1;
            cmFreeLook.m_Heading.m_Bias = Random.Range (-180, 180);
            cmFreeLook.m_YAxis.Value = 1.0f;
            cmFreeLook.m_XAxis.m_InputAxisName = string.Empty;
            cmFreeLook.m_YAxis.m_InputAxisName = string.Empty;
            cmFreeLook.m_XAxis.m_InvertInput = false;
            cmFreeLook.m_YAxis.m_InvertInput = true;
            cmFreeLook.enabled = false;

            // Saving
#if UNITY_EDITOR
            int type = int.Parse (name.Split (new char[2] { '(', ')' }) [1]);
            KocmocraftDatabase index = UnityEditor.AssetDatabase.LoadAssetAtPath<KocmocraftDatabase> ("Assets/_iLYuSha Wakaka Setting/ScriptableObject/Kocmocraft Database.asset");
            KocmocraftModule module = index.kocmocraft[type];
            module.type = (Type) type;
            module.design.size.wingspan = wingspan;
            module.design.size.length = length;
            module.design.size.height = height;
            module.design.size.wingspanScale = wingspanScale;
            module.design.size.lengthScale = lengthScale;
            module.design.size.heightScale = heightScale;
            module.design.size.weight = Mathf.RoundToInt (30 * (wingspan * length + length * height + height * wingspan) + wingspan * length * height);
            module.design.view.orthoSize = orthoSize;
            module.design.view.near = near;
            module.design.view.far = far;
            module.name = string.Format ("{0:00}", type);
            module.Calculate ();
            UnityEditor.AssetDatabase.SaveAssets ();
#endif
        }
        #endregion
    }

    #if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor (typeof (Prototype))]
    public class PrototypeEdiTor : Editor
    {
        public override void OnInspectorGUI ()
        {
            DrawDefaultInspector ();

            var scripts = targets.OfType<Prototype> ();
            if (GUILayout.Button ("Align Centre"))
                foreach (var script in scripts)
                    script.AlignCentre ();
            if (GUILayout.Button ("Align Base"))
                foreach (var script in scripts)
                    script.AlignBase ();
            if (GUILayout.Button ("Change Painting"))
                foreach (var script in scripts)
                    script.ChangePainting ();
            if (GUILayout.Button ("Random Painting"))
                foreach (var script in scripts)
                    script.RandomPainting ();
            if (GUILayout.Button ("Switch Wireframe"))
                foreach (var script in scripts)
                    script.SwitchWireframe ();
            // UnityEditor.AssetDatabase.SaveAssets ();
        }
    }
#endif
}