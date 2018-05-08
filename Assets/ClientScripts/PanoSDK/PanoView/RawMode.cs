using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMode : PanoModeBase {
    
    [HideInInspector]
    public CameraFovController _CameraFovController;

    protected override void Awake()
    {
        _CameraFovController = gameObject.GetComponentsInChildren<CameraFovController>(true)[0];
    }



    #region Finger Gesture
    public override void OnPinch(PinchGesture gesture)
    {
        if (_CameraFovController)
        {
            _CameraFovController.OnPinch(gesture);
        }
    }
    #endregion
}
