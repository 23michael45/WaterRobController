using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoControllerBase : MonoBehaviour {

    [HideInInspector]
    public bool _IsDraging = false;
    [HideInInspector]
    public bool _IsPinching = false;

    public virtual void OnDrag(DragGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            _IsDraging = true;
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            _IsDraging = false;

        }
    }
    public virtual void OnPinch(PinchGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            _IsDraging = true;
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            _IsPinching = false;
        }
    }
    public virtual void OnTap(TapGesture gesture)
    {
    }
    public virtual void OnDoubleTap(TapGesture gesture)
    {

    }

    public virtual void OnSimpleFingerDown(object v)
    {
    }
    public virtual void OnSimpleFingerUp(object v)
    {
    }


    //取角为-180到180之间
    protected float GetN180ToP180(float ang)
    {
        if (ang > 180)
        {
            ang = ang % 360 - 360;
        }

        return ang;
    }
}
