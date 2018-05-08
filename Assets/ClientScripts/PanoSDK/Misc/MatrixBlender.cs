using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class MatrixBlender : MonoBehaviour
{
    Camera _Camera;

    private Matrix4x4 _OrthoMatrix;
    private Matrix4x4 _PerspectiveMatrix;


    private float _Aspect;
    public float _Fov = 60f;
    public float _Near = .3f;
    public float _Far = 1000f;
    public float _OrthographicSize = 50f;
    
    
    public AnimationCurve _Curve;
    float _CurrentValue = 0;

    private void Awake()
    {
        _Camera = gameObject.GetComponent<Camera>();
   
    }
    public void CaluculateMatrix(float orthosize,float fov)
    {
        _OrthographicSize = orthosize;
        _Fov = fov;

        _Aspect = (float)Screen.width / (float)Screen.height;
        _OrthoMatrix = Matrix4x4.Ortho(-_OrthographicSize * _Aspect, _OrthographicSize * _Aspect, -_OrthographicSize, _OrthographicSize, _Near, _Far);
        _PerspectiveMatrix = Matrix4x4.Perspective(_Fov, _Aspect, _Near, _Far);
    }


    public Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time,AnimationCurve curve = null)
    {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
        {
            if(curve == null)
            {
                curve = AnimationCurve.Linear(0,0,1,1);
            }
            ret[i] = CurveAdd(from[i], to[i], time, curve);
        }
        return ret;
    }
    float CurveAdd(float from, float to, float time,AnimationCurve curve)
    {
        float rt = from + (to - from) * curve.Evaluate(time);
        return rt;
    }

    private void UpdateFromTo()
    {

        if(_CurrentValue == 0)
        {
            _Camera.orthographic = true;
            _Camera.ResetProjectionMatrix();
            _Camera.nearClipPlane = _Near;
            _Camera.farClipPlane = _Far;

            _Camera.aspect = _Aspect;
            _Camera.orthographicSize = _OrthographicSize;
            
        }
        else if(_CurrentValue == 1)
        {
            _Camera.orthographic = false;
            _Camera.ResetProjectionMatrix();
            _Camera.nearClipPlane = _Near;
            _Camera.farClipPlane = _Far;
            _Camera.fieldOfView = _Fov;
            
        }
        else
        {
            _Camera.projectionMatrix = MatrixLerp(_OrthoMatrix, _PerspectiveMatrix, _CurrentValue, _Curve);
        }
        
    }

    public float AddValue(float v)
    {
        _CurrentValue += v;
        _CurrentValue = Mathf.Clamp01(_CurrentValue);
        UpdateFromTo();

        return _CurrentValue;
    }
    
}
