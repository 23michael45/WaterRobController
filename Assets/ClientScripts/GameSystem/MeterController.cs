using CoreFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterController : MonoBehaviour {

    #region LeftTop
    public RotationAngleMeter _Compass;
    public FillMeter _Temperature;
    public TextMeter _AccelerateXYZ;

    #endregion

    #region Left
    public XYPlotMeter _ElectricCurrent;
    #endregion

    #region LeftBottom

    public RotationAngleMeter[] _MotorSpeedArr;
    public TextMeter _Speed;
    #endregion

    #region RightTop

    public FillMeter _Pressure;
    public RotationAngleMeter _PressurePoint;

    public FillMeter _DampnessE;
    public FillMeter _DampnessB;
    #endregion

    #region RightBottm

    #endregion

    #region Right

    public OneAxisOffsetMeter _Deep;

    #endregion

    #region Center

    public PoseMeter _Attidude;
    #endregion


    void OnEnable()
    {
        Message.AddListener<CompassChangeMessage>(OnCompassChange);
        Message.AddListener<TemperatureChangeMessage>(OnTemperatureChange);
        Message.AddListener<AccelerateChangeMessage>(OnAcclerateChange);
        Message.AddListener<ElectricCurrentChangeMessage>(OnElectricCurrentChange);
        Message.AddListener<MotorSpeedChangeMessage>(OnMotorSpeedChange);
        Message.AddListener<SpeedChangeMessage>(OnSpeedChange);
        Message.AddListener<PressureChangeMessage>(OnPressureChange);
        Message.AddListener<DampnessEChangeMessage>(OnDampnessEChange);
        Message.AddListener<DampnessBChangeMessage>(OnDampnessBChange);
        Message.AddListener<AttitudeChangeMessage>(OnAttitudeChange);
        Message.AddListener<DeepChangeMessage>(OnDeepChange);
    }
    void OnDisable()
    {
        Message.RemoveListener<CompassChangeMessage>(OnCompassChange);
        Message.RemoveListener<TemperatureChangeMessage>(OnTemperatureChange);
        Message.RemoveListener<AccelerateChangeMessage>(OnAcclerateChange);
        Message.RemoveListener<ElectricCurrentChangeMessage>(OnElectricCurrentChange);
        Message.RemoveListener<MotorSpeedChangeMessage>(OnMotorSpeedChange);
        Message.RemoveListener<SpeedChangeMessage>(OnSpeedChange);
        Message.RemoveListener<PressureChangeMessage>(OnPressureChange);
        Message.RemoveListener<DampnessEChangeMessage>(OnDampnessEChange);
        Message.RemoveListener<DampnessBChangeMessage>(OnDampnessBChange);
        Message.RemoveListener<AttitudeChangeMessage>(OnAttitudeChange);
        Message.RemoveListener<DeepChangeMessage>(OnDeepChange);

    }

    #region Interface Event Listener
    void OnCompassChange(CompassChangeMessage msg)
    {
        _Compass.SetCurrentValue(HardwareInterfaceBase.Instance.GetCompass());
    }
    void OnTemperatureChange(TemperatureChangeMessage msg)
    {

        _Temperature.SetCurrentValue(HardwareInterfaceBase.Instance.GetTemperature());
    }
    void OnAcclerateChange(AccelerateChangeMessage msg)
    {
        Vector3 v = HardwareInterfaceBase.Instance.GetAccelerate();
        object[] arr = new object[3];
        arr[0] = (int)v.x;
        arr[1] = (int)v.y;
        arr[2] = (int)v.z;

        _AccelerateXYZ.SetCurrentValue(arr);
    }
    void OnElectricCurrentChange(ElectricCurrentChangeMessage msg)
    {
        Vector2 v = new Vector2(0, HardwareInterfaceBase.Instance.GetElectricCurrent());
        _ElectricCurrent.SetCurrentValue(v);

    }
    void OnMotorSpeedChange(MotorSpeedChangeMessage msg)
    {
        _MotorSpeedArr[msg._Index].SetCurrentValue(HardwareInterfaceBase.Instance.GetMotorSpeed(msg._Index));
    }
    void OnSpeedChange(SpeedChangeMessage msg)
    {
        _Speed.SetCurrentOneValue(HardwareInterfaceBase.Instance.GetSpeed());
    }
    void OnPressureChange(PressureChangeMessage msg)
    {
        _Pressure.SetCurrentValue(HardwareInterfaceBase.Instance.GetPressure());
        _PressurePoint.SetCurrentValue(HardwareInterfaceBase.Instance.GetPressure());
    }
    void OnDampnessEChange(DampnessEChangeMessage msg)
    {
        _DampnessE.SetCurrentValue(HardwareInterfaceBase.Instance.GetDampnessE());

    }
    void OnDampnessBChange(DampnessBChangeMessage msg)
    {
        _DampnessB.SetCurrentValue(HardwareInterfaceBase.Instance.GetDampnessB());
    }

    void OnAttitudeChange(AttitudeChangeMessage msg)
    {
        _Attidude.SetCurrentValue(HardwareInterfaceBase.Instance.GetAttitude());

    }
    void OnDeepChange(DeepChangeMessage msg)
    {
        float v = (int)HardwareInterfaceBase.Instance.GetDeep();
        _Deep.SetCurrentValue(v);

    }
    #endregion
}
