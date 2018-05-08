using CoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WideAngleCameraController : CameraControllerBase
{
    public GVCircleCutProjector _Projector;
    MatrixBlender _CameraMatrixBlender;

    public float _CircleRadius = 600f;//球半径
    public float _ClipEdgeSize = 200f;//切掉边缘大小
    public float _ImageAspect = 2.0f;//图像长宽比

    public float _MaxOrthoSize = 1500f; //最大正交尺寸
    public float _MidOrthoSize = 1000; //全屏正交尺寸 半球直径600 / screen.w * screen.h
    public float _MinOrthoSize = 300f;  //最小正交尺寸
    float _CurrentOrthoSize  = 0;
    public float _MaxToMidSpeed = 1.0f;//最大正交到全屏正交的动画速度

    public float _ZoomSpeed = 10.0f;//正交到透视动作速度

    public float _IdleDragSensitive = 3;   // Drag Add Value速率


    public float _MaxFOV = 60; //最大透视FOV
    public float _MinFOV = 30; //最小透视FOV
    public float _MidFOV = 50; //内球放大后，回到的FOV值
    public float _MinToMidFovSpeed = 20;//内球放大后,回到的FOV值动画速度

    public float _YDragSpeed = .2f; //Y Drag跟随移动的速度
    public float _XDragSpeed = .1f; //X Drag跟随移动的速度



    protected float _DragMaxXAng = 30;     //X轴最大值 
    protected float _DragMinXAng = -30; //X轴最小值 
    protected float _CurrentXAng = 0; //X轴当前角
    public float _BackToXCenterSpeed = 1.0f;//回X中心动画速率

    protected float _DragMaxYAng = 90; //Y轴最大值 
    protected float _DragMinYAng = -90; //Y轴最小值 
    protected float _CurrentYAng = 0; //Y轴当前角


    public float _DragSensitive = 3;   // Drag Add Value速率
    public float _PinchSensitive = 0.3f;   // Drag Add Value速率
    
    public float _EulerAngDragSpeed = .2f; //X Euler Ang移动的速度
    public float _FOVDragSpeed = 5f;   //球内视角放大速率
    

    
    public float _BounceAngAccelerate = 5000;   //弹力时回速度
    public float _BounceAngVelocityDrag = .4f;  //回弹时的能量消耗

    public float _PatrolSpeed = 1.0f;    //巡航速率      

    protected float _BounceAngVelocity = 0;    //当前弹力速率
    protected bool _BounceDirChange = false;   //弹力方向是否改变

    
    protected float _PullEnergy = 0.0f;  //当前拉力势能
    public float _PullSensitive = 1.0f; //拉力敏感度

    protected float _PatrolDir = 0;//巡逻方向 -1 0 1
    public float _MinPatrolSpeed = 0.5f;//最小速度达到这个值开始巡逻
    public float _MinPatrolDist = 5f;//最小Y角达到这个值，PULL ENERGY结束



    public ISMStateMachine<WideAngleCameraController> mISM;

    protected override void Awake()
    {
        base.Awake();

        _MidOrthoSize = _CircleRadius / Screen.width * Screen.height;

        _CameraMatrixBlender = gameObject.GetComponent<MatrixBlender>();
        _CameraMatrixBlender.CaluculateMatrix(_MidOrthoSize,_MaxFOV);

        UpdateParam();
        MakeISM();
        

    }
    private void OnDestroy()
    {
        
    }
    private void Start()
    {
       
    }

    void Update()
    {

        if (mISM != null)
        {
            mISM.Update();
        }


        WABaseState state = mISM.GetFromDic<WABaseState>(mISM.GetCurrentFirstState());
        //Debug.Log(state.mName);
    }
    protected virtual void MakeISM()
    {
        mISM = new ISMStateMachine<WideAngleCameraController>(this);

        mISM.CreateAndAdd<WAIdleState>("Idle", this);
        mISM.CreateAndAdd<WADragState>("Drag", this);
        mISM.CreateAndAdd<WAZoomOutState>("ZoomOut", this);
        mISM.CreateAndAdd<WAZoomInState>("ZoomIn", this);
        mISM.CreateAndAdd<WABounceState>("Bounce", this);
        mISM.CreateAndAdd<WAPullEnergyState>("PullEnergy", this);
        mISM.CreateAndAdd<WAPatrolDragState>("PatrolDrag", this);


        mISM.Push("Idle");

    }
    public void UpdateParam()
    {
        if (_Projector)
        {
            _Projector.SetParams(_ImageAspect, _ClipEdgeSize, _CircleRadius);
            _ControlCamera.transform.position = new Vector3(0, 0, _ClipEdgeSize);
        }

    }

  

    #region FingerGesture
    public override void OnDrag(DragGesture gesture)
    {
        base.OnDrag(gesture);
        WABaseState state = mISM.GetFromDic<WABaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnDrag(gesture);
        }

    }
    public override void OnPinch(PinchGesture gesture)
    {

        base.OnPinch(gesture);
        WABaseState state = mISM.GetFromDic<WABaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnPinch(gesture);
        }

    }
    public override void OnTap(TapGesture gesture)
    {

        WABaseState state = mISM.GetFromDic<WABaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnTap(gesture);
        }

    }

    public override void OnSimpleFingerDown(object v)
    {
        WABaseState state = mISM.GetFromDic<WABaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnSimpleFingerDown(v);
        }
    }
    public override void OnSimpleFingerUp(object v)
    {
        WABaseState state = mISM.GetFromDic<WABaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnSimpleFingerUp(v);
        }
    }
    #endregion




    #region Camera TRS
    float GetPitchAngByImageAspect()
    {
        float radius = _CircleRadius;

        //没有切边时的仰角
        float nocutAng = Mathf.Asin(1 / _ImageAspect);

        float dist = radius - Mathf.Cos(nocutAng) * radius;

        float tanAng = _CircleRadius / _ImageAspect /(_CircleRadius - _ClipEdgeSize - dist);

        float ang = Mathf.Atan(tanAng);

        return ang * Mathf.Rad2Deg;
    }

  

  

    float GetLimitPitch()
    {
        float pitch = GetPitchAngByImageAspect();
        float vang = _ControlCamera.fieldOfView / 2;
        return pitch - vang;
    }



    public void SetIdle()
    {
        _CurrentOrthoSize = _MidOrthoSize;
        _ControlCamera.orthographic = true;
        _ControlCamera.orthographicSize = _CurrentOrthoSize;
    }

    public void AddIdleOrthoSize(float delta)
    {
        _CurrentOrthoSize += delta * _IdleDragSensitive;
        _CurrentOrthoSize = Mathf.Clamp(_CurrentOrthoSize, _MidOrthoSize,_MaxOrthoSize);

        _ControlCamera.orthographicSize = _CurrentOrthoSize;
    }
    public void MaxToMidOrthoSize()
    {
        _CurrentOrthoSize = _CurrentOrthoSize - Time.deltaTime * (_MaxOrthoSize - _MidOrthoSize) * _MaxToMidSpeed;
        _CurrentOrthoSize = Mathf.Clamp(_CurrentOrthoSize, _MidOrthoSize, _MaxOrthoSize);
        _ControlCamera.orthographicSize = _CurrentOrthoSize;
    }

    public void StartOrtho()
    {
        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        _CurrentYAng = GetN180ToP180(eulerAng.y);
        _CurrentXAng = GetN180ToP180(eulerAng.x);
    }

    public float AddToPerspectiveValue()
    {
        return  _CameraMatrixBlender.AddValue(Time.deltaTime * _ZoomSpeed);
    }
    public float AddToOrthoValue()
    {
        float v = _CameraMatrixBlender.AddValue(-Time.deltaTime * _ZoomSpeed);
        SetCameraY(_CurrentYAng * v);
        SetCameraX(_CurrentXAng * v);
        return v;
    }
    public void SetCameraX(float ang)
    {

        ang = Mathf.Clamp(ang, _DragMinXAng, _DragMaxXAng);

        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        eulerAng.x = ang;
        _ControlCamera.transform.eulerAngles = eulerAng;
    }

    public void BackToXCenter()
    {

        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        float x = GetN180ToP180(eulerAng.x);

        if(x > 0)
        {

            x -= _BackToXCenterSpeed * Time.deltaTime;
            x = Mathf.Max(x, 0);
        }
        else if(x < 0)
        {
            x += _BackToXCenterSpeed * Time.deltaTime;
            x = Mathf.Min(x, 0);
        }
        SetCameraX(x);
    }

    public void SetCameraY(float ang)
    {
        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;

        Mathf.Clamp(ang, _DragMinYAng, _DragMaxYAng);

        eulerAng.y = ang;
        _ControlCamera.transform.eulerAngles = eulerAng;
    }
    public void RotateCameraRight(float v)
    {
        _ControlCamera.transform.Rotate(Vector3.right, v * _XDragSpeed, Space.Self);


        Vector3 euler = _ControlCamera.transform.rotation.eulerAngles;
        float x = GetN180ToP180(euler.x);
        x = Mathf.Clamp(x, _DragMinXAng, _DragMaxXAng);

        euler.x = x;
        _ControlCamera.transform.rotation = Quaternion.Euler(euler);
    }
    public void RotateCameraUp(float v)
    {
        _ControlCamera.transform.Rotate(Vector3.up, v * _YDragSpeed, Space.World);


        Vector3 euler = _ControlCamera.transform.rotation.eulerAngles;
        float y = GetN180ToP180(euler.y);
        y = Mathf.Clamp(y, _DragMinYAng, _DragMaxYAng);

        euler.y = y;
        _ControlCamera.transform.rotation = Quaternion.Euler(euler);
    }
    public void SetVelocityCameraUp(float v)
    {
        _ControlCamera.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.up * v;
    }

    public void StartPullEnergy()
    {
        _PullEnergy = 0;
    }
    public void AddPullEnergy(float v)
    {
        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        float y = GetN180ToP180(eulerAng.y);
        if (Mathf.Abs(y + 90) <= 0.01f)
        {
            _PullEnergy += Mathf.Abs(v) * _PullSensitive;
        }
        else if (Mathf.Abs(y - 90) <= 0.01f)
        {

            _PullEnergy += Mathf.Abs(v) * _PullSensitive;
        }
    }

    public bool DoPullEnergy()
    {
        Vector3 euler = _ControlCamera.transform.rotation.eulerAngles;
        float y = GetN180ToP180(euler.y);

        //y<0 加速度向右，y>0 加速度向左，所以加个负号
        float acc = -_PullEnergy * y / 1000;

        Rigidbody rigid = _ControlCamera.gameObject.GetComponent<Rigidbody>();

        rigid.AddTorque(new Vector3(0, acc, 0));
        
        if (Mathf.Abs(rigid.angularVelocity.y) < _MinPatrolSpeed && Mathf.Abs(y) < _MinPatrolDist)
        {
            return true;
        }

        return false;
    }
    public void DoPatrol()
    {
        float Ang = GetVAngByFov();
        Vector3 euler = _ControlCamera.transform.rotation.eulerAngles;
        float y = GetN180ToP180(euler.y);

        Rigidbody rigid = _ControlCamera.gameObject.GetComponent<Rigidbody>();
        if(rigid.angularVelocity.y > 0 && Mathf.Abs(rigid.angularVelocity.y) <= _MinPatrolSpeed)
        {
            _PatrolDir = 1;

            if(y >= 90 - Ang)
            {
                _PatrolDir = -1;
            }
        }
        else if (rigid.angularVelocity.y < 0 && Mathf.Abs(rigid.angularVelocity.y) <= _MinPatrolSpeed)
        {
            _PatrolDir = -1;
            if (y <= -90 + Ang)
            {
                _PatrolDir = 1;
            }
        }
        else
        {
            _PatrolDir = 0;
        }


        Vector3 v = rigid.angularVelocity;
        v.y = _PatrolDir * _MinPatrolSpeed;
        rigid.angularVelocity = v;
    }


    //开始向内部拖
    public void StartDrag(bool bCurrentPos)
    {

        //if (bCurrentPos)
        //{
        //    _CurrentMinAng = _ControlCamera.transform.eulerAngles.x;
        //}
        //else
        //{
        //    _CurrentMinAng = 0;
        //}
        //_CurrentMaxAng = 90;
        
    }

    public bool StartBounce()
    {
        float Ang = GetVAngByFov();
        float CurrentY = GetN180ToP180(_ControlCamera.transform.eulerAngles.y);
        
        bool needBounce = false;
        if(CurrentY < -90 + Ang)
        {
            needBounce = true;
        }
        else if(CurrentY > 90 - Ang)
        {

            needBounce = true;
        }
        else
        {
            needBounce = false;
        }

        _BounceAngVelocity = 0;
        _BounceDirChange = false;

        return needBounce;
    }
    public bool DoBounce()
    {
        bool bEnd = false;

        float Ang = GetVAngByFov();
        float CurrentY = GetN180ToP180(_ControlCamera.transform.eulerAngles.y);

        //左边
        if (CurrentY < 0)
        {
            //到无黑边边界
            if (CurrentY >= -90 + Ang)
            {
                _BounceDirChange = true;
                _BounceAngVelocity = -_BounceAngVelocity * _BounceAngVelocityDrag;

                if (Mathf.Abs(_BounceAngVelocity) <= 0.001f)
                {
                    bEnd = true;
                }
                else
                {
                    bEnd = false;
                }
                SetCameraY(-90 + Ang);
                RotateCameraUp(_BounceAngVelocity);
            }
            else if (CurrentY < -90 + Ang)
            {
                float preVelocity = _BounceAngVelocity;
                _BounceAngVelocity += Time.deltaTime * _BounceAngAccelerate / 1000;

                if (_BounceDirChange == true)
                {
                    //变方向后，一次速度就变正负号，说明足够小了
                    if (preVelocity < 0 && _BounceAngVelocity > 0)
                    {
                        _BounceAngVelocity = 0;
                        bEnd = true;
                    }
                    else
                    {
                        _BounceDirChange = false;
                    }

                }


                RotateCameraUp(_BounceAngVelocity);
            }

            

        }
        //右边
        else
        {
            //到无黑边边界
            if (CurrentY <= 90 - Ang)
            {
                _BounceDirChange = true;
                _BounceAngVelocity = -_BounceAngVelocity * _BounceAngVelocityDrag;

                if (Mathf.Abs(_BounceAngVelocity) <= 0.001f)
                {
                    bEnd = true;
                }
                else
                {
                    bEnd = false;
                }
                SetCameraY(90 - Ang);
                RotateCameraUp(_BounceAngVelocity);
            }
            else if (CurrentY > 90 - Ang)
            {
                float preVelocity = _BounceAngVelocity;
                _BounceAngVelocity -= Time.deltaTime * _BounceAngAccelerate / 1000;

                if (_BounceDirChange == true)
                {
                    //变方向后，一次速度就变正负号，说明足够小了
                    if (preVelocity > 0 && _BounceAngVelocity < 0)
                    {
                        _BounceAngVelocity = 0;
                        bEnd = true;
                    }
                    else
                    {
                        _BounceDirChange = false;
                    }

                }


                RotateCameraUp(_BounceAngVelocity);
            }
        }
        

        return bEnd;
    }

   
    public void AddEulerAng(float delta)
    {
        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        eulerAng.x += delta * _EulerAngDragSpeed;
        eulerAng.x = Mathf.Clamp(eulerAng.x, 0.01f, 89.99f);
        _ControlCamera.transform.eulerAngles = eulerAng;
    }
    public float AddFOV(float delta)
    {
        _ControlCamera.fieldOfView += -delta * _FOVDragSpeed;
        _ControlCamera.fieldOfView = Mathf.Clamp(_ControlCamera.fieldOfView, _MinFOV, _MaxFOV);
        RefreshLimitPitch();


        SetCameraX(_ControlCamera.transform.rotation.eulerAngles.x);
        return _ControlCamera.fieldOfView;
    }
    public void RefreshLimitPitch()
    {
        float limitx = GetLimitPitch();
        _DragMaxXAng = limitx;
        _DragMinXAng = -limitx;
    }

    public void KeepMidFov()
    {
        if (_ControlCamera.fieldOfView < _MidFOV)
        {
            _ControlCamera.fieldOfView += Time.deltaTime * _MinToMidFovSpeed;
            if (_ControlCamera.fieldOfView > _MidFOV)
            {
                _ControlCamera.fieldOfView = _MidFOV;
            }

            RefreshLimitPitch();
        }
    }
    public void KeepPatrolSpeed()
    {
        Vector3 velocity = _ControlCamera.gameObject.GetComponent<Rigidbody>().angularVelocity;
        float speedY = velocity.y;
        if (speedY < Mathf.Abs(_PatrolSpeed) && speedY > 0)
        {
            speedY = Mathf.Abs(_PatrolSpeed);
        }
        else if (speedY > -Mathf.Abs(_PatrolSpeed) && speedY < 0)
        {
            speedY = -Mathf.Abs(_PatrolSpeed);
        }
        velocity.y = speedY;
        _ControlCamera.gameObject.GetComponent<Rigidbody>().angularVelocity = velocity;
    }

    public void GoToPullOrBounce()
    {
        if(_PullEnergy > 0)
        {
            mISM.Push("PullEnergy");
        }
        else
        {
            if (StartBounce())
            {
                mISM.Push("Bounce");
            }

        }

       
    }


    public void SetAspect(float w,float h)
    {
        _ImageAspect = w / h;
        UpdateParam();
    }

    public void CutEdge(float f)
    {
        _ClipEdgeSize = f;
        UpdateParam();
    }

   

    #endregion

}