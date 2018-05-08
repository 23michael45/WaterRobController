using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMode : PanoModeBase
{
    ScreenOrientation _OldScreenOrientation;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _OldScreenOrientation = Screen.orientation;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        StartCoroutine(EnableVR());
    }

    IEnumerator EnableVR()
    {
        yield return new WaitForSeconds(0.5f);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Screen.orientation = _OldScreenOrientation;
    }

    public override void SetXRotate(float rot)
    {
        base.SetXRotate(rot);
        if(_MeshRendererArr != null && _MeshRendererArr.Length > 0)
        {
            _MeshRendererArr[0].transform.localEulerAngles = new Vector3(rot, 0, 0);
        }
    }
    #region Finger Gesture

    public override void OnDrag(DragGesture gesture)
    {
      
    }
    public override void OnPinch(PinchGesture gesture)
    {
       

    }
    public override void OnDoubleTap(TapGesture gesture)
    {
      
    }

    public override void OnSimpleFingerDown(object v)
    {
      
    }
    #endregion
}
