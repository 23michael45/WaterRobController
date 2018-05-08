using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshUVFlip : MonoBehaviour {

    public Mesh _mesh;

    [ExecuteInEditMode]
    private void Awake()
    {
        if(_mesh == null)
        {
            _mesh = gameObject.GetComponent<MeshFilter>().mesh;
        }
    }
}
