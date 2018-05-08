


using CoreFramework;
using System.Collections;
using UnityEngine;

public class SMHBaseState : ISMState<ScreenMeshHalfCameraController>
{

    public SMHBaseState(string name, ScreenMeshHalfCameraController entity, ISMStateMachine<ScreenMeshHalfCameraController> parentISM, int priority)
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

public class SMHYDragRotState : SMHBaseState
{
    public SMHYDragRotState(string name, ScreenMeshHalfCameraController entity, ISMStateMachine<ScreenMeshHalfCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }
    public override bool Enter()
    {
        return base.Enter();
    }

    public override void OnDrag(DragGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            mEntity.SetVelocityCameraUp(0);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
           
                mEntity.RotateCameraUp(gesture.LastDelta.x);
          
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {

            mEntity.AddForceCameraY(gesture.TotalMove.x / gesture.ElapsedTime);

        }
    }
}

public class SMHIdleState : SMHYDragRotState
{
    bool _IsDrag = false;

    public SMHIdleState(string name, ScreenMeshHalfCameraController entity, ISMStateMachine<ScreenMeshHalfCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }
    
    public override bool Enter()
    {
        return base.Enter();
    }

    public override void Execute()
    {


        mEntity.KeepMinSpeed();

        if (_IsDrag == false)
        {
            mEntity.MaxToMid();
        }

        base.Execute();
    }
    public override void Exit()
    {
        base.Exit();
    }


    public override void OnDrag(DragGesture gesture)
    {
        //不用BASE因为要区分手指位置转动
        //base.OnDrag(gesture);
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            mEntity.SetVelocityCameraUp(0);
            _IsDrag = true;
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            

            if (gesture.Position.y > Screen.height / 2)
            {

                mEntity.RotateCameraUp(gesture.LastDelta.x);
            }
            else
            {

                mEntity.RotateCameraUp(-gesture.LastDelta.x);
            }
            mEntity.DoDrag(Mathf.Abs(gesture.LastDelta.y));
            mEntity.CalculateYPos();


        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            _IsDrag = false;
            if (gesture.Position.y > Screen.height / 2)
            {
                mEntity.AddForceCameraY(gesture.TotalMove.x / gesture.ElapsedTime);
            }
            else
            {
                mEntity.AddForceCameraY(-gesture.TotalMove.x / gesture.ElapsedTime);
            }
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

        }
    }
    public override void OnTap(TapGesture gesture)
    {
    }

    public override void OnSimpleFingerDown(object v)
    {
        mEntity.SetVelocityCameraUp(0);
    }

}


public class SMHDragState : SMHBaseState
{
    protected float _LastPinchDelta = 0;
    public SMHDragState(string name, ScreenMeshHalfCameraController entity, ISMStateMachine<ScreenMeshHalfCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        mEntity.StartDrag();
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
                int v = mEntity.DoPinch(-gesture.Delta);
                if (v == 0)
                {
                    mEntity.CalculateYPos();

                    mEntity.CalculateFOV();
                    mEntity.CalculateEulerAng();
                }
                else if (v == 1)
                {
                    mEntity.mISM.Push("Idle");

                }
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

public class SMHDragNoIdleState : SMHDragState
{
    public SMHDragNoIdleState(string name, ScreenMeshHalfCameraController entity, ISMStateMachine<ScreenMeshHalfCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
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
                int v = mEntity.DoPinch(-gesture.Delta);
                if (v == 0)
                {
                    mEntity.CalculateYPos();

                    mEntity.CalculateFOV();
                    mEntity.CalculateEulerAng();
                }
                else if (v == 1)
                {

                }
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



public class SMHZoomOutState : SMHBaseState
{

    public SMHZoomOutState(string name, ScreenMeshHalfCameraController entity, ISMStateMachine<ScreenMeshHalfCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        mEntity.StartZoomOut();
        return base.Enter();
    }

    public override void Execute()
    {

        mEntity.MinToMid();
        if (mEntity.CheckZoomOutEnd())
        {

            mEntity.mISM.Push("Idle");
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
            //mEntity.SetVelocityCameraUp(0);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {

            mEntity.AddForceCameraY(gesture.LastDelta.x);

        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            mEntity.AddForceCameraY(gesture.LastDelta.x);
        }
    }
}

public class SMHZoomInState : SMHBaseState
{

    public SMHZoomInState(string name, ScreenMeshHalfCameraController entity, ISMStateMachine<ScreenMeshHalfCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {

        mEntity.AddZoomRotForce();
        return base.Enter();


    }

    public override void Execute()
    {

        mEntity.MidToMin();

        if(mEntity.CheckZoomInEnd())
        {
            mEntity.StartBounce(0);
            mEntity.mISM.Push("Bounce");
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
            mEntity.SetVelocityCameraUp(0);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            mEntity.RotateCameraUp(gesture.LastDelta.x);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            mEntity.AddForceCameraY(gesture.LastDelta.x);
        }
    }
}

public class SMHBounceState : SMHYDragRotState
{
    public SMHBounceState(string name, ScreenMeshHalfCameraController entity, ISMStateMachine<ScreenMeshHalfCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        mEntity._CurrentValue = 0;
        mEntity.CalculateYPos();
        mEntity.CalculateEulerAng();
        return base.Enter();
    }

    public override void Execute()
    {
        if (mEntity.DoBounce())
        {
            mEntity.mISM.Push("BounceDrag");
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
            mEntity.mISM.Push("BounceDrag");
        }
    }
}


public class SMHBounceDragState : SMHYDragRotState
{
    CrudeElapsedTimer _NoOperationTimer;
  
    float _LastY;
    bool _Pinching = false;
    public SMHBounceDragState(string name, ScreenMeshHalfCameraController entity, ISMStateMachine<ScreenMeshHalfCameraController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
        _NoOperationTimer = new CrudeElapsedTimer(1.0f);
    }

    public override bool Enter()
    {
        mEntity._CurrentValue = 0;
        mEntity.CalculateYPos();
        //mEntity.CalculateEulerAng();

        _NoOperationTimer.Reset();
        return base.Enter();
    }

    public override void Execute()
    {
        if (_Pinching)
        {
            return;
        }

        _NoOperationTimer.Advance(Time.deltaTime);
        if(_NoOperationTimer.TimeOutCount > 0)
        {
            mEntity.GoToBounce(0);
        }


        mEntity.KeepMidFov();
        mEntity.KeepMinSpeed();
        base.Execute();
    }
    public override void Exit()
    {
        _Pinching = false;
        base.Exit();
    }

    public override void OnDrag(DragGesture gesture)
    {
        _NoOperationTimer.Reset();
        if (_Pinching)
        {
            return;
        }

        base.OnDrag(gesture);
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            if (gesture.LastDelta.y != 0)
            {
                _LastY = gesture.LastDelta.y;

            }
            mEntity.AddEulerAng(gesture.LastDelta.y);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            mEntity.GoToBounce(_LastY);

        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        _NoOperationTimer.Reset();
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
            mEntity.GoToBounce(_LastY);
        }
    }
    public override void OnSimpleFingerDown(object v)
    {

        _NoOperationTimer.Reset();
        mEntity.SetVelocityCameraUp(0);
    }
    public override void OnSimpleFingerUp(object v)
    {

        _NoOperationTimer.Reset();
        mEntity.GoToBounce(0);
    }


}
