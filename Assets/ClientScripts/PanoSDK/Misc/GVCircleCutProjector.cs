using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GVCircleCutProjector : MonoBehaviour {

    public bool _Ortho = true;
    public float _TargetCircleD = 600f;
    public float _CutEdgeSize = 0f;//切边大小
    float _SrcImagAspect = 2f;
    
    Projector _Projector;
    Material _ProjectorMaterial;
    private void Awake()
    {
        _Projector = gameObject.GetComponent<Projector>();
        if(_Projector)
        {
            _ProjectorMaterial = _Projector.material;
            _ProjectorMaterial.SetVector("_planeNorm", Vector3.forward);
        }
    }
    private void Start()
    {
        UpdateParams();

    }
    private void Update()
    {
#if UNITY_EDITOR
        UpdateParams();
#endif
    }

    public void SetParams(float aspect,float edge,float d)
    {
        _TargetCircleD = d;
        _SrcImagAspect = aspect;
        _CutEdgeSize = edge;
        UpdateParams();
    }

    void UpdateParams()
    {
        if (_Projector )
        {
            _Projector.orthographic = _Ortho;
            _Projector.orthographicSize = _TargetCircleD / _SrcImagAspect;
            _Projector.aspectRatio = _SrcImagAspect;

            _ProjectorMaterial.SetVector("_planePos", new Vector3(0, 0, _CutEdgeSize));
        }

    }

    public void SetTexture(Texture tex)
    {
        if(_ProjectorMaterial)
        {
            _ProjectorMaterial.SetTexture("_ShadowTex",tex);

        }
    }
}
