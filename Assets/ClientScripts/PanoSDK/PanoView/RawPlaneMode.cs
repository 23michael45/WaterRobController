using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawPlaneMode : PanoModeBase {
    
    ScrollPlaneMeshController _Controller;

    [HideInInspector]
    public CameraFovController _CameraFovController;

    protected override void Awake()
    {
        _Controller = gameObject.GetComponentsInChildren<ScrollPlaneMeshController>(true)[0];
        _CameraFovController = gameObject.GetComponentsInChildren<CameraFovController>(true)[0];
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
        if (_CameraFovController)
        {
            _CameraFovController.OnPinch(gesture);
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
