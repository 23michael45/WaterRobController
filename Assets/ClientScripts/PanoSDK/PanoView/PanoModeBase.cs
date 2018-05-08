using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PanoModeBase : MonoBehaviour {

    [NonSerialized]
    [HideInInspector]
    public Renderer[] _MeshRendererArr;


    protected virtual void Awake()
    {
    

    }
    protected virtual void Start()
    {
        InitRenderer();
        ResetTexture();
    }

    protected void InitRenderer()
    {
        if (_MeshRendererArr == null || _MeshRendererArr.Length == 0)
        {


            _MeshRendererArr = GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in _MeshRendererArr)
            {
                if (r)
                {
                    r.sharedMaterial = new Material(r.sharedMaterial);
                }

            }
        }
    }

    protected virtual void OnEnable()
    {
        Texture tex = PanoManager.Instance.GetRenderTexture();
        ResetTexture();

    }
    protected virtual void OnDisable()
    {

    }


    protected virtual void Update()
    {

    }
    protected virtual void OnDestroy()
    {

    }

    public virtual void ResetTexture()
    {
        Texture tex = PanoManager.Instance.GetRenderTexture();
        if(_MeshRendererArr != null )
        {
            foreach (Renderer r in _MeshRendererArr)
            {
                if (r)
                {
                    r.sharedMaterial.SetTexture("_MainTex", tex);
                }
            }

        }  
    }
    public virtual void FlipX(bool b)
    {

        InitRenderer();
    }
    public virtual void FlipY(bool b)
    {
        InitRenderer();

    }
    public virtual void SetXRotate(float rot)
    {
        InitRenderer();
    }
    public virtual void EnableGyroscope(bool b)
    {

    }

    #region Finger Gesture

    public virtual void OnDrag(DragGesture gesture)
    {
        
    }
    public virtual void OnPinch(PinchGesture gesture)
    {
        

    }
    public virtual void OnTap(TapGesture gesture)
    {
        
    }
    public virtual void OnDoubleTap(TapGesture gesture)
    {
    }

    public virtual void OnSimpleFingerDown(object v)
    {
    }
    public virtual void OnSimpleFingerUp(object v)
    {
    }
    #endregion
}
