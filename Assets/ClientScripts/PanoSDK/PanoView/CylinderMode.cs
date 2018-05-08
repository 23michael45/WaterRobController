using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMode : PanoModeBase {
    
    CylinderMeshController _Controller;

    [HideInInspector]
    public CameraFovController _CameraFovController;
    protected override void Awake()
    {
        _Controller = gameObject.GetComponentsInChildren<CylinderMeshController>(true)[0];
        _CameraFovController = gameObject.GetComponentsInChildren<CameraFovController>(true)[0];
    }
    public override void FlipY(bool b)
    {
        base.FlipY(b);
        if(b)
        {
            if(_Controller)
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
    #endregion
}
