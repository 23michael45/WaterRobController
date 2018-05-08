using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavCameraController : CameraControllerBase
{
    public ScreenMeshHalfInCameraController _ControlledCamera;
    GVProjector _Projector;
    protected Camera _Camera;

    protected override void Awake()
    {
        base.Awake();

      

        _Camera = gameObject.GetComponentsInChildren<Camera>(true)[0];

        SetControlledCamera(_ControlledCamera);
    }
    public void SetControlledCamera(ScreenMeshHalfInCameraController smc)
    {
        _ControlledCamera = smc;
        if (_ControlledCamera)
        {
            _Projector = _ControlledCamera.gameObject.GetComponentsInChildren<GVProjector>(true)[0];
            _Projector.SetController(_ControlledCamera);
        }
    }

    public override void OnDrag(DragGesture gesture)
    {
        base.OnDrag(gesture);
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {

            if(_ControlledCamera)
            {
                Vector2 center = GetCenterPos();
                float ang = GetAng(center, gesture.LastPos);
                float radius = GetRadius(center, gesture.LastPos);
                radius = Mathf.Clamp01(radius);

                _ControlledCamera.SetCameraX(radius);
                _ControlledCamera.SetCameraY(ang);
            }


            Debug.Log(gesture.Position);

        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {

        }
    }
    public override void OnSimpleFingerUp(object v)
    {
        if(_ControlledCamera)
        {
            _ControlledCamera.GoToBounce(0);

        }
    }


    Vector2 GetCenterPos()
    {
        Rect viewrect = _ControlCamera.rect;
        Vector2 normalpos = new Vector2((viewrect.x + viewrect.width / 2) * Screen.width, (viewrect.y + viewrect.height / 2) * Screen.height);
        return normalpos;
    }
    float GetAng(Vector2 center, Vector2 pos)
    {
        Vector2 dir = pos - center;
        Quaternion rot = Quaternion.FromToRotation(dir, Vector2.up);
        return rot.eulerAngles.z;
    }
    float GetRadius(Vector2 center,Vector2 pos)
    {
        float dist = Vector3.Distance(center, pos);
        return dist /  (Screen.width * _ControlCamera.rect.width / 2) ;
    }
    
}