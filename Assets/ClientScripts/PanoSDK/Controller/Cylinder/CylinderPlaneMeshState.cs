


using CoreFramework;
using System.Collections;
using UnityEngine;

public class CMBaseState : ISMState<CylinderPlaneMeshController>
{

    public CMBaseState(string name, CylinderPlaneMeshController entity, ISMStateMachine<CylinderPlaneMeshController> parentISM, int priority)
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

public class CMPlaneState : CMBaseState
{

    protected float _DeltaX;
    protected float _DragTime;
    public CMPlaneState(string name, CylinderPlaneMeshController entity, ISMStateMachine<CylinderPlaneMeshController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }
    public override bool Enter()
    {
        mEntity.SetPlaneState(true);
        return base.Enter();
    }
    public override void Execute()
    {

        mEntity.FixPosition();
        mEntity.FixSpeed();
        base.Execute();
    }
    public override void OnDrag(DragGesture gesture)
    {
        base.OnDrag(gesture);
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            mEntity.AddForceX(gesture.LastDelta.x);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {

        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        base.OnPinch(gesture);

        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            mEntity.mISM.Push("Pinch");
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {

        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {

        }
    }
    public override void OnSimpleFingerDown(object v)
    {
        mEntity.StopMove();
    }
}

public class CMCylinderState : CMBaseState
{

    public CMCylinderState(string name, CylinderPlaneMeshController entity, ISMStateMachine<CylinderPlaneMeshController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }
    
    public override bool Enter()
    {
        mEntity.EnableAnimator(false);
        return base.Enter();
    }

    public override void Execute()
    {
        base.Execute();
    }
    public override void Exit()
    {
        mEntity.EnableAnimator(true);
        base.Exit();
    }


    public override void OnDrag(DragGesture gesture)
    {
        base.OnDrag(gesture);
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            Debug.Log(gesture.DeltaMove);
            mEntity.RotateUp(gesture.DeltaMove.x);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            mEntity.AddForceY(gesture.TotalMove.x /  gesture.ElapsedTime);
        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        base.OnPinch(gesture);

        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            mEntity.mISM.Push("Pinch");
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
           
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {

        }
    }
    public override void OnTap(TapGesture gesture)
    {
    }

    public override void OnSimpleFingerDown(object v)
    {
        mEntity.StopRot();
    }

}

public class CMPinchState : CMBaseState
{
    float _LastDelta;
    public CMPinchState(string name, CylinderPlaneMeshController entity, ISMStateMachine<CylinderPlaneMeshController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        mEntity.SetPlaneState(false);
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
    
    public override void OnPinch(PinchGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            mEntity.PinchAnimation(gesture.Delta);
            if(gesture.Delta != 0)
            {
                _LastDelta = gesture.Delta;
            }
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            if(_LastDelta > 0)
            {

                mEntity.mISM.Push("ZoomIn");
            }
            else if (_LastDelta < 0)
            {

                mEntity.mISM.Push("ZoomOut");
            }
        }
    }
    public override void OnTap(TapGesture gesture)
    {
    }

    public override void OnSimpleFingerDown(object v)
    {
    }

}



public class CMZoomOutState : CMBaseState
{

    public CMZoomOutState(string name, CylinderPlaneMeshController entity, ISMStateMachine<CylinderPlaneMeshController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        return base.Enter();
    }

    public override void Execute()
    {
        mEntity.DoToPlane();

        if(mEntity.GetAnimationProgress() == 0)
        {

            mEntity.mISM.Push("Plane");
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
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            

        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
        }
    }
}

public class CMZoomInState : CMBaseState
{

    public CMZoomInState(string name, CylinderPlaneMeshController entity, ISMStateMachine<CylinderPlaneMeshController> parentISM, int priority)
        : base(name, entity, parentISM, priority)
    {
    }

    public override bool Enter()
    {
        
        return base.Enter();


    }

    public override void Execute()
    {

        mEntity.DoToCylinder();
        if (mEntity.GetAnimationProgress() == 0.999f)
        {

            mEntity.mISM.Push("Cylinder");
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
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
        }
    }
}

