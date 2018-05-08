using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CylinderPlaneMeshController : PanoControllerBase
{

    public float _DragSensitive = 0.3f;   // Drag Add Value速率

    public float _PinchSensitive = 0.01f;   // Pinch Add Value速率
    public float _MinSpeed = .5f;
    public float _AnimationSpeed = 5.0f;

    public float _YDragSpeed = 5.0f;
    public float _RotForce = 5.0f;
    CylinderPlaneMode _ParentMode;

    public Transform _PlaneClone;



    public Animator _AnimatorTRS;



    public ISMStateMachine<CylinderPlaneMeshController> mISM;


    public Mesh _RawPlaneMesh;
    public MeshFilter _BendMeshFilter;
    private void Awake()
    {
        _ParentMode = gameObject.GetComponentInParent<CylinderPlaneMode>();



        _BendMeshFilter.mesh = _RawPlaneMesh;
        MegaModifyObject mob = _BendMeshFilter.gameObject.GetComponent<MegaModifyObject>();
        mob.Reset();




        MakeISM();
    }
    private void Start()
    {
        _AnimatorTRS.speed = 0;
    }
    protected virtual void MakeISM()
    {
        mISM = new ISMStateMachine<CylinderPlaneMeshController>(this);

        mISM.CreateAndAdd<CMPlaneState>("Plane", this);
        mISM.CreateAndAdd<CMPinchState>("Pinch", this);
        mISM.CreateAndAdd<CMCylinderState>("Cylinder", this);
        mISM.CreateAndAdd<CMZoomOutState>("ZoomOut", this);
        mISM.CreateAndAdd<CMZoomInState>("ZoomIn", this);

        mISM.Push("Plane");

    }
    void Update()
    {
        if (mISM != null)
        {
            mISM.Update();
        }
    }



    public override void OnDrag(DragGesture gesture)
    {
        base.OnDrag(gesture);
        CMBaseState state = mISM.GetFromDic<CMBaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnDrag(gesture);
        }
      
    }
    public override void OnPinch(PinchGesture gesture)
    {
        base.OnPinch(gesture);
        CMBaseState state = mISM.GetFromDic<CMBaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnPinch(gesture);
        }

    }


    public override void OnSimpleFingerDown(object v)
    {
        CMBaseState state = mISM.GetFromDic<CMBaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnSimpleFingerDown(v);
        }
    }
    public override void OnSimpleFingerUp(object v)
    {
        CMBaseState state = mISM.GetFromDic<CMBaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnSimpleFingerUp(v);
        }

    }

    public void AddForceX(float v)
    {

        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right * v * _DragSensitive);
    }

    public void PinchAnimation(float v)
    {

        AddAnimationProgress(v * _PinchSensitive);
    }

    public void AddAnimationProgress(float v)
    {
        float f = v;

        float time = _AnimatorTRS.GetCurrentAnimatorStateInfo(0).normalizedTime;
        time = Mathf.Clamp(time + f, 0, 0.999f);
        _AnimatorTRS.Play("CylinderTRS", 0, time);
    }

    public float GetAnimationProgress()
    {
        float time = _AnimatorTRS.GetCurrentAnimatorStateInfo(0).normalizedTime;
        return time;
    }
    public void StopMove()
    {

        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    public void FixPosition()
    {
        var width = 200;
        var v = transform.localPosition;
        //v.y = 0;

        var cameraWidth = _ParentMode._Camera.orthographicSize / Screen.height * Screen.width;
        var objWidth = width / 2;
        var max = objWidth - cameraWidth;
        max = Mathf.Max(max, 0);
        if (v.x > max)
        {
            v.x = v.x % width;


            _PlaneClone.localPosition = new Vector3(-width, _PlaneClone.localPosition.y, _PlaneClone.localPosition.z);

        }
        else if (v.x < -max)
        {
            v.x = -(-v.x % width);

            _PlaneClone.localPosition = new Vector3(width, _PlaneClone.localPosition.y, _PlaneClone.localPosition.z);
        }


        transform.localPosition = v;
    }
    public void FixSpeed()
    {
        float x = gameObject.GetComponent<Rigidbody>().velocity.x;
        if (x != 0 && Mathf.Abs(x) < _MinSpeed)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(_MinSpeed * x / Mathf.Abs(x), 0, 0);
        }
    }
    public void StopRot()
    {
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
    public void AddForceY(float v)
    {
        gameObject.GetComponent<Rigidbody>().AddTorque(transform.up * -v * _RotForce);
    }
    public void RotateUp(float v)
    {
        transform.Rotate(transform.up, -v * _YDragSpeed, Space.World);
    }
    public void SetPlaneState(bool b)
    {
        StopRot();
        StopMove();
        transform.localRotation = Quaternion.Euler(0,0,0); 
        transform.localPosition = Vector3.zero;

        _PlaneClone.gameObject.SetActive(b);
    }
    public void EnableAnimator(bool b)
    {
        _AnimatorTRS.enabled = b;
    }

    public void DoToCylinder()
    {
        AddAnimationProgress(_AnimationSpeed * Time.deltaTime);
    }
    public void DoToPlane()
    {

        AddAnimationProgress(-_AnimationSpeed * Time.deltaTime);
    }
}