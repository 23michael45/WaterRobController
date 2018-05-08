using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderPlaneMode : PanoModeBase {
    
    CylinderPlaneMeshController _Controller;

    [HideInInspector]
    public Camera _Camera;
    protected override void Awake()
    {
        _Controller = gameObject.GetComponentsInChildren<CylinderPlaneMeshController>(true)[0];
        _Camera = gameObject.GetComponentsInChildren<Camera>(true)[0];
    }
    public override void FlipY(bool b)
    {
        base.FlipY(b);
        if (b)
        {
            if (_Controller)
            {
                _Controller.gameObject.transform.localScale = new Vector3(1, -1, 1);
            }
        }
        else
        {
            if (_Controller)
            {
                _Controller.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    #region Finger Gesture

    public override void OnDrag(DragGesture gesture)
    {
        if (_Controller)
        {
            _Controller.OnDrag(gesture);
        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        if (_Controller)
        {
           _Controller.OnPinch(gesture);
        }

    }
    public override void OnTap(TapGesture gesture)
    {
        if (_Controller)
        {
           //_Controller.OnTap(gesture);
        }
    }
    public override void OnDoubleTap(TapGesture gesture)
    {
        if (_Controller)
        {
           // _Controller.OnDoubleTap(gesture);
        }
    }

    public override void OnSimpleFingerDown(object v)
    {

        if (_Controller)
        {
            _Controller.OnSimpleFingerDown(v);
        }
    }
    public override void OnSimpleFingerUp(object v)
    {
        if (_Controller)
        {
            _Controller.OnSimpleFingerUp(v);
        }
    }
    #endregion
}
