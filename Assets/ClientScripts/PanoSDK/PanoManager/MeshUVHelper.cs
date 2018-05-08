using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Collections.Generic;

public class MeshUVHelper : MonoBehaviour
{
#if UNITY_IPHONE && !UNITY_EDITOR
	public const string dllName = "__Internal";
#else
    public const string dllName = "mappano";
#endif

    public bool _IsResetMeshUV = true;


    [DllImport(dllName)]
    private static extern int GenerateMeshByEncryptString(int cameraIndex, string encryptStr, out int vCount, out int tCount);

    [DllImport(dllName)]
    private static extern void CopyMeshData(IntPtr vertices, IntPtr uvs, IntPtr colors, IntPtr triangles);

    public bool SetMeshUV(int index, string text, ref MeshFilter meshFilter)
    {
        if (!_IsResetMeshUV)
            return true;

        try
        {
            //需要先判断是否是有效数据
            if (!text.StartsWith("PE"))
            {
                Debug.LogWarning("Param Format Error:" + text);
                return false;
            }
            string encryptStr = text.Substring(16, text.Length - 16);

            meshFilter.mesh = new Mesh();


            int vCount;
            int tCount;
            int ret;

            if ((ret = GenerateMeshByEncryptString(index, encryptStr, out vCount, out tCount)) == 0)
            {
                int[] triangles = new int[tCount];
                Vector3[] vertices = new Vector3[vCount];
                Vector2[] uvs = new Vector2[vCount];
                Color32[] colors = new Color32[vCount];

                unsafe
                {
                    fixed (void* pVertices = &vertices[0], pUVs = &uvs[0], pColors = &colors[0], pTriangles = &triangles[0])
                    {
                        CopyMeshData((IntPtr)pVertices, (IntPtr)pUVs, (IntPtr)pColors, (IntPtr)pTriangles);
                    }
                }

                meshFilter.mesh.vertices = vertices;
                meshFilter.mesh.uv = uvs;
                meshFilter.mesh.colors32 = colors;
                meshFilter.mesh.triangles = triangles;

                //ATrace.Log(string.Format("EncryptString:len:{0} content:{1}", uvs.Length,encryptStr));
                //for (int i = 0; i < uvs.Length; i = i + 100)
                //{
                //    ATrace.Log(string.Format("UV:{0} :{1}", i, uvs[i]));
                //}
            }
            else
            {
                Debug.LogErrorFormat("GenerateMesh error " + index + "ret:{0}", ret);
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("GenerateMesh error " + e);
            return false;
        }

        return true;
    }



}
