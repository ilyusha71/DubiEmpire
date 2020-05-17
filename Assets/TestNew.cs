using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNew : MonoBehaviour
{
    public Transform core;
    public GameObject[] crafts;
    public int index;
    // Start is called before the first frame update
    // public RenderTexture rt;

    void Center()
    {
        for (int i = 0; i < crafts.Length; i++)
        {
            crafts[i].transform.localPosition = Vector3.one * 999;
            if (i == index)
                crafts[i].transform.localPosition = Vector3.zero;
        }
        crafts[index].GetComponent<Kocmoca.Protodesign>().AlignCentre();
        GetComponent<Camera>().orthographicSize = 7.0f / crafts[index].GetComponent<Kocmoca.Protodesign>().GetScalePower();
    }

    void DDD()
    {
        RenderTexture rt = GetComponent<Camera>().targetTexture;
        //    = Selection.activeObject as RenderTexture;

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes;
        bytes = tex.EncodeToPNG();

        string path = "Assets/"+ crafts[index].transform.name + ".png";
        System.IO.File.WriteAllBytes(path, bytes);
        // AssetDatabase.ImportAsset(path);
        Debug.Log("Saved to " + path);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            DDD();

        if (Input.GetKeyDown(KeyCode.D))
        {
            index = (int)Mathf.Repeat(++index, crafts.Length);
            Center();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            index = (int)Mathf.Repeat(--index, crafts.Length);
            Center();
        }
    }
}
