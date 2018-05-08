using CoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenMeshHalfCameraController : CameraControllerBase
{
    public bool _NeedRenderTexture = true;

    public enum ECAMERADIRECTION
    {
        ECD_UP = -1,
        ECD_DOWN = 1,
    }
    public ECAMERADIRECTION _CameraDirection = ECAMERADIRECTION.ECD_DOWN;
    [HideInInspector]
    public RenderTexture _RenderTexture;


    public float _MaxMidSpeed = 1f; //Min Mid间自动动画速率
    public float _MinMidSpeed = .6f; //Min Mid间自动动画速率
    public float _MidMinRotSpeed = 500f; //Min Mid间旋转动画速率
    public float _DragSensitive = 3;   // Drag Add Value速率
    public float _PinchSensitive = 0.3f;   // Drag Add Value速率
    public float _RotForce = 5f;   //Y转动的力3

    public float _YDragSpeed = .2f; //Y Drag跟随移动的速度
    public float _EulerAngDragSpeed = .2f; //X Euler Ang移动的速度
    public float _FOVDragSpeed = 5f;   //球内视角放大速率



    public float _MaxY = 25000;
    public float _MidY = 12000;
    public float _MinY = 0;

    public float _MaxFOV = 90; //内球进入时FOV正常值
    public float _MinFOV = 30; //最小FOV，内球放大，后球全屏时用这个值 
    public float _MidFov = 40; //内球放大后，回到的FOV值
    public float _MinToMidFovSpeed = 20f; //内球放大后,回到的FOV值动画速度

    protected float _CurrentMaxAng = 90;   //相机x轴角 最大值
    protected float _CurrentMinAng = 0;//相机x轴角 最小值
    float _BounceAng = 0;//彈時的x轴角
    protected float _ZoomOutStartAng = 0; //Zoom Out时x 轴角起始值 

    public float _BounceDragSpeed = 100f;
    public float _BounceAngAccelerate = 5000;
    public float _BounceAngVelocityDrag = .5f;

    public float _MinSpeed = 1.0f;

    protected float _BounceAngVelocity = 0;
    protected bool _BounceDirChange = false;

    public AnimationCurve _YHighCurve;
    public AnimationCurve _YLowCurve;
    public AnimationCurve _FOVCurve;
    public AnimationCurve _AngCurve;

    

    protected bool _CanManipulate = false;

    //全局的Value计算相机姿态
    [HideInInspector]
    public float _CurrentValue = 0;
    


    public ISMStateMachine<ScreenMeshHalfCameraController> mISM;

    protected override void Awake()
    {
        base.Awake();
        MakeISM();

        if(_NeedRenderTexture)
        {
            _RenderTexture = new RenderTexture(Screen.width / 2, Screen.height / 2, 0);
            _ControlCamera.targetTexture = _RenderTexture;

        }
        

    }
 
    private void Start()
    {
        AddForceCameraY(10);
    }

    void Update()
    {
       
        if (mISM != null)
        {
            mISM.Update();
        }
        SMHBaseState state = mISM.GetFromDic<SMHBaseState>(mISM.GetCurrentFirstState());
        //Debug.Log(state.mName);
    }
    protected virtual void MakeISM()
    {
        mISM = new ISMStateMachine<ScreenMeshHalfCameraController>(this);

        mISM.CreateAndAdd<SMHIdleState>("Idle", this);
        mISM.CreateAndAdd<SMHDragState>("Drag", this);
        mISM.CreateAndAdd<SMHZoomOutState>("ZoomOut", this);
        mISM.CreateAndAdd<SMHZoomInState>("ZoomIn", this);
        mISM.CreateAndAdd<SMHBounceState>("Bounce", this);
        mISM.CreateAndAdd<SMHBounceDragState>("BounceDrag", this);

        
        mISM.Push("Idle");

    }



    public override void OnDrag(DragGesture gesture)
    {
        base.OnDrag(gesture);
        SMHBaseState state = mISM.GetFromDic<SMHBaseState>(mISM.GetCurrentFirstState());
        if(state != null)
        {
            state.OnDrag(gesture);
        }

    }
    public override void OnPinch(PinchGesture gesture)
    {
       
        SMHBaseState state = mISM.GetFromDic<SMHBaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnPinch(gesture);
        }

    }
    public override void OnTap(TapGesture gesture)
    {
      
        SMHBaseState state = mISM.GetFromDic<SMHBaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnTap(gesture);
        }

    }

    public override void OnSimpleFingerDown(object v)
    {
        SMHBaseState state = mISM.GetFromDic<SMHBaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnSimpleFingerDown(v);
        }
    }
    public override void OnSimpleFingerUp(object v)
    {
        SMHBaseState state = mISM.GetFromDic<SMHBaseState>(mISM.GetCurrentFirstState());
        if (state != null)
        {
            state.OnSimpleFingerUp(v);
        }
    }
    

    #region Camera TRS
    void AddValue(float delta)
    {
        _CurrentValue += delta / 1000f;
        _CurrentValue = Mathf.Clamp(_CurrentValue, 0, 2);
    }
    public void CalculateYPos()
    {
        if(_CurrentValue > 1 && _CurrentValue <=2)
        {
            float y = _MidY + _YHighCurve.Evaluate(_CurrentValue - 1) * (_MaxY - _MidY);
            _ControlCamera.transform.position = new Vector3(0, y * (int)_CameraDirection, 0);

        }
        else if(_CurrentValue >= 0 && _CurrentValue <= 1)
        {
            float y = _MinY + _YLowCurve.Evaluate(_CurrentValue) * (_MidY - _MinY);
            _ControlCamera.transform.position = new Vector3(0, y * (int)_CameraDirection, 0);

        }

    }
    public void CalculateFOV()
    {
        if (_CurrentValue > 1 && _CurrentValue <= 2)
        {
            _ControlCamera.fieldOfView = _MinFOV;
        }
        else if (_CurrentValue >= 0 && _CurrentValue <= 1)
        {

            float fieldOfView = _MinFOV + _FOVCurve.Evaluate(_CurrentValue) * (_MaxFOV - _MinFOV);
            _ControlCamera.fieldOfView = Mathf.Clamp(fieldOfView, _MinFOV, _MaxFOV);
      
        }

    }
    public void CalculateEulerAng()
    {
        if (_CurrentValue > 1 && _CurrentValue <= 2)
        {
            Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
            eulerAng.x = _CurrentMaxAng * (int)_CameraDirection;
            _ControlCamera.transform.eulerAngles = eulerAng;

        }
        else if (_CurrentValue >= 0 && _CurrentValue <= 1)
        {
            Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
            eulerAng.x = (_CurrentMinAng +  _AngCurve.Evaluate(_CurrentValue) * (_CurrentMaxAng - _CurrentMinAng)) *(int)_CameraDirection;
            _ControlCamera.transform.eulerAngles = eulerAng ;
        }


    }
    public void SetCameraY(float ang)
    {
        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        eulerAng.y = ang * (int)_CameraDirection;
        _ControlCamera.transform.eulerAngles = eulerAng;
    }
    public void SetCameraX(float ang)
    {
        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        eulerAng.x = (_CurrentMaxAng - (_CurrentMaxAng - _CurrentMinAng) * ang * 2) * (int)_CameraDirection;
        _ControlCamera.transform.eulerAngles = eulerAng;
    }


    public void RotateCameraUp(float v)
    {
        _ControlCamera.transform.Rotate(Vector3.up, -v * _YDragSpeed, Space.World);
    }
    public void SetVelocityCameraUp(float v)
    {
        _ControlCamera.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.up * v;
    }
    public void AddForceCameraY(float v)
    {
        if(!v.IsNull() && v != 0)
        {
            _ControlCamera.gameObject.GetComponent<Rigidbody>().AddTorque(Vector3.up * -v * _RotForce);

        }
    }
    public void AddZoomRotForce()
    {
        AddForceCameraY(_MidMinRotSpeed);
    }


    public void MidToMin()
    {
        _CurrentValue -= Time.deltaTime * _MinMidSpeed;
        _CurrentValue = Mathf.Clamp(_CurrentValue,0,1);


        CalculateYPos();
        CalculateFOV();
        CalculateEulerAng();
    }

    public void StartZoomOut()
    {
        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        _ZoomOutStartAng = eulerAng.x;
    }
    public void MinToMid()
    {

        float y = _MidY + _YHighCurve.Evaluate(_CurrentValue - 1) * (_MaxY - _MidY);


        _CurrentValue += Time.deltaTime * _MinMidSpeed;
        _CurrentValue = Mathf.Clamp(_CurrentValue, 0, 1);
        CalculateYPos();
        CalculateFOV();

        //这里不按这种方法计算Euler Ang 因为在ZOOM OUT时，起始的Euler Ang不定
        //CalculateEulerAng();


        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        eulerAng.x = (_ZoomOutStartAng + _AngCurve.Evaluate(_CurrentValue) * (_CurrentMaxAng - _ZoomOutStartAng)) * (int)_CameraDirection;
        _ControlCamera.transform.eulerAngles = eulerAng;
    }
    public void MaxToMid()
    {

        float y = _MidY + _YHighCurve.Evaluate(_CurrentValue - 1) * (_MaxY - _MidY);


        _CurrentValue -= Time.deltaTime * _MaxMidSpeed;
        _CurrentValue = Mathf.Clamp(_CurrentValue, 1, 2);
        CalculateYPos();
        CalculateFOV();
        CalculateEulerAng();

    }



    //开始向内部拖
    public void StartDrag()
    {
        _CurrentValue = Mathf.Clamp01(_CurrentValue);

        _CurrentMinAng = 0;
        _CurrentMaxAng = 90;
        
    }
    public int DoPinch(float delta)
    {
        AddValue(delta * _PinchSensitive);
        return (int)_CurrentValue;
    }
    public int DoDrag(float delta)
    {
        AddValue(delta * _DragSensitive);
        return (int)_CurrentValue;
    }

    public void StartBounce(float angVelocity)
    {
        _CurrentMaxAng = 90;
        if (_CameraDirection == ECAMERADIRECTION.ECD_UP)
        {
            _BounceAng = (_ControlCamera.transform.rotation.eulerAngles.x - 360)%360;
        }
        else if (_CameraDirection == ECAMERADIRECTION.ECD_DOWN)
        {
            _BounceAng = _ControlCamera.transform.rotation.eulerAngles.x;
        }

            _BounceDirChange = false;
        //拖出黑边，为向下拖，是Y为负，黑边为0，无边为 fieldofview/2， 下拖为黑边变多，即角变小，那么角速也为负
        _BounceAngVelocity = angVelocity * _BounceDragSpeed / 10000 * (int)_CameraDirection;
    }
    public bool DoBounce()
    {
        bool bEnd = false;

        if (Mathf.Abs(_BounceAng) >= _ControlCamera.fieldOfView/2)
        {
            _BounceDirChange = true;
            _BounceAngVelocity = -_BounceAngVelocity * _BounceAngVelocityDrag;

            if (Mathf.Abs(_BounceAngVelocity) <= 0.001f)
            {
                bEnd = true;
                goto End;
            }
            else
            {
                bEnd = false;
            }
            _BounceAng += _BounceAngVelocity;
        }
        else
        {

            //到无黑边状态，角要变大，所以速度要+
           

            float preVelocity = _BounceAngVelocity;
            _BounceAngVelocity += Time.deltaTime * _BounceAngAccelerate / 1000 * (int)_CameraDirection;

            if (_BounceDirChange == true)
            {
                if (_CameraDirection == ECAMERADIRECTION.ECD_UP)
                {
                    //变方向后，一次速度就变正负号，说明足够小了
                    if (preVelocity > 0 && _BounceAngVelocity < 0)
                    {
                        _BounceAngVelocity = 0;
                        _CurrentMinAng = _ControlCamera.fieldOfView / 2;
                        bEnd = true;
                        goto End;
                    }
                    else
                    {
                        _BounceDirChange = false;
                    }
                }
                else if (_CameraDirection == ECAMERADIRECTION.ECD_DOWN)
                { 
                    //变方向后，一次速度就变正负号，说明足够小了
                    if (preVelocity < 0 && _BounceAngVelocity > 0)
                    {
                        _BounceAngVelocity = 0;
                        _CurrentMinAng = _ControlCamera.fieldOfView / 2;
                        bEnd = true;
                        goto End;
                    }
                    else
                    {
                        _BounceDirChange = false;
                    }
                }
                   

            }



            _BounceAng += _BounceAngVelocity;
        }

        End:
        if (_CameraDirection == ECAMERADIRECTION.ECD_UP)
        {

            _BounceAng = Mathf.Clamp(_BounceAng,  -_ControlCamera.fieldOfView / 2,0);
        }
        else if(_CameraDirection == ECAMERADIRECTION.ECD_DOWN)
        {

            _BounceAng = Mathf.Clamp(_BounceAng, 0, _ControlCamera.fieldOfView / 2);
        }
        
        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        eulerAng.x = _BounceAng ;
        _ControlCamera.transform.eulerAngles = eulerAng;

        return bEnd;
    }

    public bool CheckZoomInEnd()
    {
        if(_CurrentValue < 0.001f)
        {
            return true;
        }
        return false;
    }
    public bool CheckZoomOutEnd()
    {
        if (_CurrentValue > 0.999f && _CurrentValue < 1.001f)
        {
            return true;
        }
        return false;
    }
    public void AddEulerAng(float delta)
    {
        Vector3 eulerAng = _ControlCamera.transform.eulerAngles;
        eulerAng.x += delta * _EulerAngDragSpeed;

        if(_CameraDirection == ECAMERADIRECTION.ECD_DOWN)
        {

            eulerAng.x = Mathf.Clamp(eulerAng.x, 0.01f , 89.99f );
        }
        else if(_CameraDirection == ECAMERADIRECTION.ECD_UP)
        {
            eulerAng.x -= 360;

            eulerAng.x = Mathf.Clamp(eulerAng.x, -89.99f,-0.01f);
        }

        _ControlCamera.transform.eulerAngles = eulerAng;
    }
    public float AddFOV(float delta)
    {
        _ControlCamera.fieldOfView += -delta * _FOVDragSpeed;
        _ControlCamera.fieldOfView = Mathf.Clamp(_ControlCamera.fieldOfView, _MinFOV, _MaxFOV);
        return _ControlCamera.fieldOfView;
    }
    public void KeepMidFov()
    {
        if(_ControlCamera.fieldOfView < _MidFov)
        {
            _ControlCamera.fieldOfView += Time.deltaTime * _MinToMidFovSpeed;
            if(_ControlCamera.fieldOfView > _MidFov)
            {
                _ControlCamera.fieldOfView = _MidFov;
            }
        }
    }

    public void KeepMinSpeed()
    {
        Vector3 velocity = _ControlCamera.gameObject.GetComponent<Rigidbody>().angularVelocity;
        float speedY = velocity.y;
        if (speedY < Mathf.Abs(_MinSpeed)  && speedY > 0)
        {
            speedY = Mathf.Abs(_MinSpeed);
        }
        else if (speedY > -Mathf.Abs(_MinSpeed) && speedY < 0)
        {
            speedY = -Mathf.Abs(_MinSpeed);
        }
        velocity.y = speedY;
        _ControlCamera.gameObject.GetComponent<Rigidbody>().angularVelocity = velocity;
    }
    public void GoToBounce(float y)
    {
        if(_CameraDirection == ECAMERADIRECTION.ECD_DOWN)
        {
            if (_ControlCamera.fieldOfView / 2 > _ControlCamera.transform.rotation.eulerAngles.x)
            {
                StartBounce(y);
                mISM.Push("Bounce");

            }
        }
        else if(_CameraDirection == ECAMERADIRECTION.ECD_UP)
        {
            if (-_ControlCamera.fieldOfView / 2 < (_ControlCamera.transform.rotation.eulerAngles.x - 360)%360)
            {
                StartBounce(y);
                mISM.Push("Bounce");

            }
        }
      
    }
    //public void Log()
    //{
    //    Debug.Log(string.Format("V:{0} Y:{1} Fov:{2} Euler:{3}", 
    //        _CurrentValue,
    //        _ControlCamera.transform.position.y,
    //        _ControlCamera.fieldOfView,
    //         _ControlCamera.transform.rotation.eulerAngles));
    //}
    
    #endregion

}