using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : PanoControllerBase
{
    public float mSpeed = .6f;
    public float mMinY = -90;
    public float mMaxY = 90;
    public float mSpeedDrag = 0.6f;
    public float mSpeedForce = 0.2f;


    [HideInInspector]
    public Quaternion mOrgQuaternion; //默认旋转值

    [HideInInspector]
    public Quaternion mFlipOrgQuaternion; //上下颠倒后的旋转值

    Vector2 MeshRotate;

    bool mIsFlipByForwardAxis;

    [HideInInspector]
    public Transform _ControlMesh;
    

    
    Vector2 _CurrentSpeed;
    Vector2 _LastNonZeroDelta;

    protected virtual void Awake()
    {
        _ControlMesh = transform;

        //计算翻转值。为了避免欧拉角的问题，使用Rotate函数。
        mOrgQuaternion = transform.localRotation;
        transform.Rotate(mIsFlipByForwardAxis ? Vector3.forward : Vector3.right, 180, Space.Self);
        mFlipOrgQuaternion = transform.localRotation;
        transform.Rotate(mIsFlipByForwardAxis ? Vector3.forward : Vector3.right, 180, Space.Self);
    }

    private void Update()
    {
        if(_IsDraging ==false)
        {
            _CurrentSpeed *= mSpeedDrag;
        }


        MeshRotate = Rotate(MeshRotate, _CurrentSpeed);
       
    }

    public virtual Vector2 Rotate(Vector2 angle, Vector2 delta)
    {
        angle.x += delta.x;
        angle.y = Mathf.Clamp(angle.y - delta.y, mMinY, mMaxY);

        transform.localRotation = mOrgQuaternion;
        transform.Rotate(Vector3.up, angle.x, Space.Self);
        transform.Rotate(Vector3.right, angle.y, Space.World);
        return angle;
    }

    public override void OnDrag(DragGesture gesture)
    {
        base.OnDrag(gesture);
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            _CurrentSpeed = gesture.LastDelta * mSpeed;
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            _CurrentSpeed = gesture.TotalMove / gesture.ElapsedTime * mSpeedForce;
        }
    }

    public override void OnSimpleFingerDown(object v)
    {
    }
}
