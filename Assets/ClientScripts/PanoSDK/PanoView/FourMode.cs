using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FourMode : PanoModeBase {

    ScreenMeshHalfInCameraController[] _ControllerArr;

    ScreenMeshHalfInCameraController _CurrentController;

    protected override void Awake() {

        _ControllerArr = gameObject.GetComponentsInChildren<ScreenMeshHalfInCameraController>(true);

    }

    void CheckControllerAera(Vector2 iPos)
    {
        if(_ControllerArr != null)
        {
            foreach (ScreenMeshHalfInCameraController controller in _ControllerArr)
            {
                if (controller.CheckInCameraScreenArea(iPos))
                {
                    _CurrentController = controller;
                    break;
                }
            }
        }
       
    }


    #region Finger Gesture

    public override void OnDrag(DragGesture gesture)
    {
        if(_CurrentController)
        {
            _CurrentController.OnDrag(gesture);
        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        if (_CurrentController)
        {
            _CurrentController.OnPinch(gesture);
        }

    }
    public override void OnTap(TapGesture gesture)
    {
        if (_CurrentController)
        {
            _CurrentController.OnTap(gesture);
        }
    }
    public override void OnDoubleTap(TapGesture gesture)
    {
        if (_CurrentController)
        {
            _CurrentController.OnDoubleTap(gesture);
        }
    }

    public override void OnSimpleFingerDown(object v)
    {
        Vector3 v3 = (Vector3)v;
        CheckControllerAera(new Vector2(v3.x, v3.y));


        if (_CurrentController)
        {
            _CurrentController.OnSimpleFingerDown(v);
        }
    }
    public override void OnSimpleFingerUp(object v)
    {
        if (_CurrentController)
        {
            _CurrentController.OnSimpleFingerUp(v);
        }
    }
    #endregion
}
