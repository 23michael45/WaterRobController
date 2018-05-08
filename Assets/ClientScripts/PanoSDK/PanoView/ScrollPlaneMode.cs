using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollPlaneMode : PanoModeBase {

    ScrollPlaneMeshController[] _ControllerArr;
    ScrollPlaneMeshController _Controller;

    [HideInInspector]
    public Camera _Camera;

    protected override void Awake()
    {
        _ControllerArr = gameObject.GetComponentsInChildren<ScrollPlaneMeshController>(true);
        _Camera = gameObject.GetComponentsInChildren<Camera>(true)[0];
    }
    public override void FlipY(bool b)
    {
        base.FlipY(b);
        if (b)
        {
            if (_ControllerArr != null)
            {
                foreach (ScrollPlaneMeshController c in _ControllerArr)
                {
                    c.gameObject.transform.localScale = new Vector3(1, -1, 1);
                }
            }
        }
        else
        {
            if (_ControllerArr != null)
            {
                foreach (ScrollPlaneMeshController c in _ControllerArr)
                {
                    c.gameObject.transform.localScale = new Vector3(1, 1, 1);
                }
            }
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
    
    public override void OnSimpleFingerDown(object v)
    {
        Vector3 v3 = (Vector3)v;

        foreach(ScrollPlaneMeshController c in _ControllerArr)
        {
            if(c.CheckRaycast(new Vector2(v3.x,v3.y)))
            {
                _Controller = c;
                break;
            }
        }

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
