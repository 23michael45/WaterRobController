using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMode : PanoModeBase
{

    CameraFovController _FovController;
    SphereController _SphereController;

    protected override void Awake()
    {
        _FovController = gameObject.GetComponentsInChildren<CameraFovController>(true)[0];
        _SphereController = gameObject.GetComponentsInChildren<SphereController>(true)[0];

    }


    #region Finger Gesture

    public override void OnDrag(DragGesture gesture)
    {
        if(_FovController._IsPinching == false)
        {
            if (_SphereController)
            {
                _SphereController.OnDrag(gesture);
            }

        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        if (_FovController)
        {
            _FovController.OnPinch(gesture);
        }

    }
    public override void OnDoubleTap(TapGesture gesture)
    {
        if (_FovController)
        {
            _FovController.OnDoubleTap(gesture);
        }
    }

    public override void OnSimpleFingerDown(object v)
    {
        if (_SphereController)
        {
            _SphereController.OnSimpleFingerDown(v);
        }
    }
    #endregion
}
