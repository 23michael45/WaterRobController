using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MeshUVFlip))]
public class MeshUVFlipEditor : Editor {

    // Use this for initialization
    MeshUVFlip t;
    void OnEnable()
    {
        t = target as MeshUVFlip;
    }
    public override void OnInspectorGUI()
    {
        t._mesh = EditorGUILayout.ObjectField("Mesh", t._mesh ,typeof(Mesh)) as Mesh; 

        if (GUILayout.Button("DoFlipUVX"))
        {
            DoFlipUVX();
        }
        if (GUILayout.Button("DoFlipUVY"))
        {
            DoFlipUVY();
        }
        if (GUILayout.Button("DoFlipPosY"))
        {
            DoFlipPosY();
        }
        if (GUILayout.Button("DoTrans"))
        {
            DoTrans();
        }
        if (GUILayout.Button("DoSave"))
        {
            DoSave();
        }
        if (GUILayout.Button("GetMesh"))
        {
            GetMesh();
        }
    }
    void DoFlipUVX()
    {
       Mesh _mesh = t._mesh;
       Vector2[] uvs = new Vector2[_mesh.uv.Length];
       for(int i = 0; i< uvs.Length;i++)
        {

            Vector2 uv = _mesh.uv[i];
            uvs[i] = new Vector2(1 - uv.x, uv.y);
        }


        Mesh newmesh = new Mesh();
        newmesh.vertices = _mesh.vertices;
        newmesh.triangles = _mesh.triangles;

        newmesh.uv = uvs;

        AssetDatabase.CreateAsset(newmesh, "Assets/ArtRes/3D/Meshs/spherein_full_down.asset");
    }
    void DoFlipUVY()
    {
        Mesh _mesh = t._mesh;
        Vector2[] uvs = new Vector2[_mesh.uv.Length];
        for (int i = 0; i < uvs.Length; i++)
        {

            Vector2 uv = _mesh.uv[i];
            uvs[i] = new Vector2(uv.x, 1 - uv.y);
        }


        Mesh newmesh = new Mesh();
        newmesh.vertices = _mesh.vertices;
        newmesh.triangles = _mesh.triangles;

        newmesh.uv = uvs;

        AssetDatabase.CreateAsset(newmesh, "Assets/ArtRes/3D/Meshs/oneeyepano_single_center_flipy.asset");
    }
    void DoFlipPosY()
    {
        Mesh _mesh = t._mesh;
        Vector3[] vertices = new Vector3[_mesh.vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {

            Vector3 ver = _mesh.vertices[i];
            vertices[i] = new Vector3(ver.x,  -ver.y,ver.z);
        }


        Mesh newmesh = new Mesh();
        newmesh.vertices = vertices;
        newmesh.triangles = _mesh.triangles;

        newmesh.uv = _mesh.uv;

        AssetDatabase.CreateAsset(newmesh, "Assets/ArtRes/3D/Meshs/oneeyepano_single_center_flipy.asset");
    }

    void DoTrans()
    {
        Mesh _mesh = t._mesh;

        float maxX = float.MinValue;
        float maxY = float.MinValue;
        float minX = float.MaxValue;
        float minY = float.MaxValue;

        for (int i = 0; i < _mesh.vertices.Length; i++)
        {

            Vector3 ver = _mesh.vertices[i];

            maxX = Mathf.Max(maxX, ver.x);
            minX = Mathf.Min(minX, ver.x);
            maxY = Mathf.Max(maxY, ver.y);
            minY = Mathf.Min(minY, ver.y);
        }
        float distX = (maxX + minX) / 2;
        float distY = (maxY + minY) / 2;

        Vector3[] vertices = new Vector3[_mesh.vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 ver = _mesh.vertices[i];
            vertices[i] = new Vector3(ver.x - distX, ver.y - distY);
        }

        Mesh newmesh = new Mesh();
        newmesh.vertices = vertices;
        newmesh.triangles = _mesh.triangles;
        newmesh.uv = _mesh.uv;

        AssetDatabase.CreateAsset(newmesh, "Assets/ArtRes/3D/Meshs/oneeyepano_single_center.asset");
    }
    void DoSave()
    {
        Mesh _mesh = t._mesh;

       

        AssetDatabase.CreateAsset(_mesh, "Assets/ArtRes/3D/Meshs/newmesh.asset");
    }
    void GetMesh()
    {
        t._mesh = t.gameObject.GetComponent<MeshFilter>().mesh;
    }

}
