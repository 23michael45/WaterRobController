using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFovController : CameraControllerBase
{

    public float _MinFov = 0;
    public float _MaxFov = 90f;
    public float _Sensitive = 1.0f;

    public override void OnPinch(PinchGesture gesture)
    {
        base.OnPinch(gesture);
        if (_ControlCamera)
        {
            if(_ControlCamera.orthographic)
            {
                _ControlCamera.orthographicSize -= gesture.Delta * _Sensitive;
                _ControlCamera.orthographicSize = Mathf.Clamp(_ControlCamera.orthographicSize, _MinFov, _MaxFov);

            }
            else
            {

                _ControlCamera.fieldOfView -= gesture.Delta * _Sensitive;
                _ControlCamera.fieldOfView = Mathf.Clamp(_ControlCamera.fieldOfView, _MinFov, _MaxFov);
            }
        }
    }

}
