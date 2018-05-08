using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WideAngleMode : PanoModeBase {

    WideAngleCameraController _CurrentController;
    GVCircleCutProjector _CutProjector;
    protected override void Awake() {

        _CurrentController = gameObject.GetComponentsInChildren<WideAngleCameraController>(true)[0];
        _CutProjector = gameObject.GetComponentsInChildren<GVCircleCutProjector>(true)[0];
    }

    public override void ResetTexture()
    {
        //投影器，不用PanoManager展开
        Texture tex = PanoManager.Instance.GetRenderTexture();
        _CutProjector.SetTexture(tex);
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
