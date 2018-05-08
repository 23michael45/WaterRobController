
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfSphereMode : PanoModeBase {
    
    ScreenMeshHalfInCameraController _Controller;
    GyroscopeCameraController _VRController;

    protected override void Awake()
    {
        _Controller = gameObject.GetComponentsInChildren<ScreenMeshHalfInCameraController>(true)[0];
        _VRController = gameObject.GetComponentsInChildren<GyroscopeCameraController>(true)[0];
        EnableGyroscope(false);
    }
    public override void EnableGyroscope(bool b)
    {
        if(b)
        { 
            _Controller.gameObject.SetActive(false);
            _VRController.gameObject.SetActive(true);
        }
        else
        {
            _Controller.gameObject.SetActive(true);
            _VRController.gameObject.SetActive(false);
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
