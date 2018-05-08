using CoreFramework;
using System.Collections;
using UnityEngine;

public class WABaseState : ISMState<WideAngleCameraController>
{

    public WABaseState(string name, WideAngleCameraController entity, ISMStateMachine<WideAngleCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority, "Exclusive")
    {
    }

    public override bool Enter()
    {
        return base.Enter();
    }

    public override void Execute()
    {


        base.Execute();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public virtual void OnDrag(DragGesture gesture)
    {

    }
    public virtual void OnPinch(PinchGesture gesture)
    {

    }
    public virtual void OnTap(TapGesture gesture)
    {

    }
    public virtual void OnSimpleFingerDown(object v)
    {

    }
    public virtual void OnSimpleFingerUp(object v)
    {
    }
}


public class WAIdleState : WABaseState
{
    bool _Draging = false;
    public WAIdleState(string name, WideAngleCameraController entity, ISMStateMachine<WideAngleCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        mEntity.SetIdle();
        return base.Enter();
    }

    public override void Execute()
    {
        if(_Draging == false)
        {

            mEntity.MaxToMidOrthoSize();
        }
        base.Execute();
    }
    public override void Exit()
    {
        base.Exit();
    }


    public override void OnDrag(DragGesture gesture)
    {
        base.OnDrag(gesture);
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            _Draging = true;
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            mEntity.AddIdleOrthoSize(Mathf.Abs(gesture.LastDelta.y)); 

        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            _Draging = false;
        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        if (gesture.Delta > 0)
        {
            mEntity.mISM.Push("Drag");
        }
        else
        {
            mEntity.AddIdleOrthoSize(Mathf.Abs(gesture.Delta));

        }
    }
  

}


public class WADragState : WABaseState
{
    protected float _LastPinchDelta = 0;
    public WADragState(string name, WideAngleCameraController entity, ISMStateMachine<WideAngleCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        mEntity.StartDrag(false);
        return base.Enter();
    }

    public override void Execute()
    {

        base.Execute();
    }
    public override void Exit()
    {
        base.Exit();
    }



    public override void OnDrag(DragGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {


        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {

        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            if (gesture.Delta != 0)
            {
                _LastPinchDelta = gesture.Delta;

            }

            if (gesture.Delta > 0)
            {

            }
            else
            {

            }
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            if (_LastPinchDelta > 0)
            {
                mEntity.mISM.Push("ZoomIn");
            }
            else if (_LastPinchDelta < 0)
            {
                mEntity.mISM.Push("ZoomOut");

            }
        }
    }
}


public class WAZoomOutState : WABaseState
{

    public WAZoomOutState(string name, WideAngleCameraController entity, ISMStateMachine<WideAngleCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        mEntity.StartOrtho();
        return base.Enter();
    }

    public override void Execute()
    {
        ;
        if (mEntity.AddToOrthoValue() == 0)
        {
            mEntity.mISM.Push("Idle");
        }
        base.Execute();
    }
    public override void Exit()
    {
        base.Exit();
    }


}

public class WAZoomInState : WABaseState
{

    public WAZoomInState(string name, WideAngleCameraController entity, ISMStateMachine<WideAngleCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        return base.Enter();
    }

    public override void Execute()
    {
        if (mEntity.AddToPerspectiveValue() == 1)
        {

            mEntity.mISM.Push("PatrolDrag");
        }

        base.Execute();
    }
    public override void Exit()
    {
        base.Exit();
    }
}

public class WABounceState : WABaseState
{
    public WABounceState(string name, WideAngleCameraController entity, ISMStateMachine<WideAngleCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        mEntity.SetVelocityCameraUp(0);
        return base.Enter();
    }

    public override void Execute()
    {
        mEntity.BackToXCenter();
        if (mEntity.DoBounce())
        {
            mEntity.mISM.Push("PatrolDrag");
        }

        base.Execute();
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void OnDrag(DragGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            mEntity.mISM.Push("PatrolDrag");
        }
    }
}

public class WAPullEnergyState : WABaseState
{
    public WAPullEnergyState(string name, WideAngleCameraController entity, ISMStateMachine<WideAngleCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        return base.Enter();
    }

    public override void Execute()
    {
        mEntity.BackToXCenter();
        if (mEntity.DoPullEnergy())
        {
            mEntity.mISM.Push("PatrolDrag");
        }

        base.Execute();
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void OnDrag(DragGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            mEntity.StartPullEnergy();
            mEntity.mISM.Push("PatrolDrag");
        }
    }
}
public class WAPatrolDragState : WABaseState
{
    float _LastY;
    bool _Pinching = false;
    bool _Draging = false;
    public WAPatrolDragState(string name, WideAngleCameraController entity, ISMStateMachine<WideAngleCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        mEntity.SetVelocityCameraUp(0);
        mEntity.RefreshLimitPitch();
        return base.Enter();
    }

    public override void Execute()
    {
        if (_Pinching)
        {
            return;
        }
        if (_Draging)
        {
            return;
        }
        

        mEntity.BackToXCenter();
        mEntity.KeepMidFov();

        mEntity.DoPatrol();
        base.Execute();
    }
    public override void Exit()
    {
        _Draging = false;
        _Pinching = false;
        base.Exit();
    }

    public override void OnDrag(DragGesture gesture)
    {
        if (_Pinching)
        {
            return;
        }
       

        base.OnDrag(gesture);
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            mEntity.SetVelocityCameraUp(0);
            _Draging = true;
            mEntity.StartPullEnergy();
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            _Draging = true;
            if (gesture.LastDelta.y != 0)
            {
                _LastY = gesture.LastDelta.y;

            }
            mEntity.RotateCameraRight(gesture.LastDelta.y);
            mEntity.RotateCameraUp(-gesture.LastDelta.x);

            mEntity.AddPullEnergy(gesture.LastDelta.x);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            mEntity.GoToPullOrBounce();

            _Draging = false;
        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            _Pinching = true;
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {

            if (mEntity.AddFOV(gesture.Delta) == mEntity._MaxFOV)
            {
                mEntity.mISM.Push("ZoomOut");
            }
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            _Pinching = false;
        }
    }
    public override void OnSimpleFingerDown(object v)
    {

        mEntity.SetVelocityCameraUp(0);
    }


}

