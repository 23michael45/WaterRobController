using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavRTMode : PanoModeBase
{

    NavCameraController _NavController;
    ScreenMeshHalfInCameraController _MeshHalfInController;

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
        CameraControllerBase controller = GetControllerByInputPos(gesture.LastPos);
        if (controller)
        {
            controller.OnDrag(gesture);
        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        CameraControllerBase controller = GetControllerByInputPos(gesture.Position);
        if (controller)
        {
            controller.OnPinch(gesture);
        }
    }
    public override void OnTap(TapGesture gesture)
    {
        CameraControllerBase controller = GetControllerByInputPos(gesture.Position);
        if (controller)
        {
            controller.OnTap(gesture);
        }
    }
    public override void OnDoubleTap(TapGesture gesture)
    {
        CameraControllerBase controller = GetControllerByInputPos(gesture.Position);
        if (controller)
        {
            controller.OnDoubleTap(gesture);
        }
    }

    public override void OnSimpleFingerDown(object v)
    {
        Vector3 v3 = (Vector3)v;
        CameraControllerBase controller = GetControllerByInputPos(v3);
        if (controller)
        {
            controller.OnSimpleFingerDown(v);
        }
    }
    public override void OnSimpleFingerUp(object v)
    {
        Vector3 v3 = (Vector3)v;
        CameraControllerBase controller = GetControllerByInputPos(v3);
        if (controller)
        {
            controller.OnSimpleFingerUp(v);
        }
    }
    #endregion

    CameraControllerBase GetControllerByInputPos(Vector2 pos)
    {
        if (_NavController.CheckInCameraScreenArea(pos))
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
