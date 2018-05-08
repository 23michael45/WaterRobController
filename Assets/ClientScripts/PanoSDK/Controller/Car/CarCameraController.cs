using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCameraController : CameraControllerBase
{

    public float _MinFov = 0;
    public float _MaxFov = 90f;
    public float _Sensitive = 1.0f;

    public override void OnPinch(PinchGesture gesture)
    {
        base.OnPinch(gesture);
        if (_ControlCamera)
        {
            _ControlCamera.fieldOfView -= gesture.Delta * _Sensitive;
            _ControlCamera.fieldOfView = Mathf.Clamp(_ControlCamera.fieldOfView, _MinFov, _MaxFov);
        }
    }

}
