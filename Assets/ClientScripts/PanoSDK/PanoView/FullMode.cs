
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullMode : PanoModeBase {
    
    ScreenMeshHalfCameraController _Controller;

    protected override void Awake()
    {
        _Controller = gameObject.GetComponentsInChildren<ScreenMeshHalfCameraController>(true)[0];

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
            _Controller.OnTap(gesture);
        }
    }
    public override void OnDoubleTap(TapGesture gesture)
    {
        if (_Controller)
        {
            _Controller.OnDoubleTap(gesture);
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
