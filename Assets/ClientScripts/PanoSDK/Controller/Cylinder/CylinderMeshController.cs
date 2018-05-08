using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CylinderMeshController : PanoControllerBase
{
    public GameObject _RigidMesh;
    public float _MinSpeed = .5f;

    public float _YDragSpeed = 5.0f;
    public float _RotForce = 5.0f;

    public float _PitchMax = 90;
    public float _PitchMin = -90;

    Quaternion _OrgRotation;
    private void Awake()
    {
        _OrgRotation = transform.localRotation;
    }
    private void Start()
    {
    }

    void Update()
    {

    }



    public override void OnDrag(DragGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            RotateUp(gesture.DeltaMove.x);
            RotateRight(-gesture.DeltaMove.y);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {

            AddForceY(gesture.TotalMove.x / gesture.ElapsedTime);
        }
    }



    public override void OnSimpleFingerDown(object v)
    {
        StopRot();
    }


    public void StopMove()
    {

        _RigidMesh.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void FixSpeed()
    {
        float x = gameObject.GetComponent<Rigidbody>().velocity.x;
        if (x != 0 && Mathf.Abs(x) < _MinSpeed)
        {
            _RigidMesh.GetComponent<Rigidbody>().velocity = new Vector3(_MinSpeed * x / Mathf.Abs(x), 0, 0);
        }
    }
    public void StopRot()
    {
        _RigidMesh.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
    public void AddForceY(float v)
    {
        _RigidMesh.GetComponent<Rigidbody>().AddTorque(_RigidMesh.transform.up * -v * _RotForce);
    }
    public void RotateUp(float v)
    {
        _RigidMesh.transform.Rotate(_RigidMesh.transform.up, -v * _YDragSpeed, Space.World);
    }
    public void RotateRight(float v)
    {
        //rotx = Mathf.Clamp(rotx + v, _PitchMin, _PitchMax);

        //transform.localRotation = _OrgRotation;
        transform.Rotate(transform.right, -v * _YDragSpeed, Space.World);


        Vector3 euler = transform.localRotation.eulerAngles;

        if(euler.x > 180)
        {
            euler.x -= 360;
        }
        euler = new Vector3(Mathf.Clamp(euler.x, _PitchMin, _PitchMax),euler.y,euler.z);
        transform.eulerAngles = euler;

        Debug.Log(euler);
    }
}