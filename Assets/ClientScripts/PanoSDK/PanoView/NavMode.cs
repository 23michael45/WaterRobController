using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMode : PanoModeBase {

    NavCameraController _NavController;
    ScreenMeshHalfInCameraController _MeshHalfInController;

    CameraControllerBase _CurrentController;

    protected override void Awake()
    {
        _MeshHalfInController = gameObject.GetComponentsInChildren<ScreenMeshHalfInCameraController>(true)[0];
        _NavController = gameObject.GetComponentsInChildren<NavCameraController>(true)[0];

    }
    protected override void Start()
    {
        base.Start();
        CalculateRect();
    }
    void CalculateRect()
    {
        float width = 0.5f;
        float height = width * Screen.width / Screen.height;
        Rect rc = new Rect(0.5f, 0f, width, height);
        _NavController._ControlCamera.rect = rc;
    }

    #region Finger Gesture

    public override void OnDrag(DragGesture gesture)
    {
        if (_CurrentController)
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

    public override void OnSimpleFingerDown(object v)
    {
        Vector3 v3 = (Vector3)v;
        _CurrentController = GetControllerByInputPos(v3);
        if (_CurrentController)
        {
            _CurrentController.OnSimpleFingerDown(v);
        }
    }
    public override void OnSimpleFingerUp(object v)
    {
        Vector3 v3 = (Vector3)v;
        if (_CurrentController)
        {
            _CurrentController.OnSimpleFingerUp(v);
        }
    }
    #endregion

    CameraControllerBase GetControllerByInputPos(Vector2 pos)
    {
        if(_NavController.CheckInCameraScreenArea(pos))
        {
            return _NavController;
        }
        else if (_MeshHalfInController.CheckInCameraScreenArea(pos))
        {
            return _MeshHalfInController;
        }
        return null;
    }
}
