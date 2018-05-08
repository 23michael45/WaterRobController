using CoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class RobotControllerDataInterface : HardwareInterfaceBase
{

    public float _Battery;
    public float _Volt;
    public bool _PropellerState;
    public int _WifiState;
    public float _LaunchTime;


    public float _Compass;
    public float _Temperature;
    public Vector3 _Accelerate;

    public float _ElectricCurrent;
    public float[] _MotorSpeed;
    public float _Speed;

    public Vector3 _Attitude;

    public float _Deep;
    public float _Pressure;
    public float _DampnessB;
    public float _DampnessE;

    #region Hardware Export Function

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public const string dllName = "Windows";
#elif UNITY_ANDROID || UNITY_STANDALONE_LINUX
      public const string dllName = "librov";
#elif UNITY_IPHONE && !UNITY_EDITOR
      public const string dllName = "__Internal";
#endif

    //[DllImport(dllName, EntryPoint = "getPressure")]
    //public static extern int getPressure();

    //[DllImport(dllName, EntryPoint = "getHygro1")]
    //public static extern int getHygro1();

    //[DllImport(dllName, EntryPoint = "getHygro2")]
    //public static extern int getHygro2();
    #endregion





    #region Interface

    public override float GetBattery()
    {
        return _Battery;
    }
    public override float GetVolt()
    {
        return _Volt;
    }
    public override bool GetPropellerState()
    {
        return _PropellerState;
    }

    public override int GetWifiState()
    {
        return _WifiState;
    }
    public override string GetCurrentTime()
    {
        return DateTime.Now.ToShortTimeString();
    }
    public override string GetLaunchTime()
    {
        int h = (int)_LaunchTime / 3600;
        int m = (int)(_LaunchTime - 60 * h) / 60;
        int s = (int)_LaunchTime % 3600;
        string str = string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
        return str;
    }

    public override float GetCompass()
    {
        return _Compass;
    }

    public override float GetTemperature()
    {
        return _Temperature;
    }

    public override Vector3 GetAccelerate()
    {
        return _Accelerate;
    }


    public override float GetElectricCurrent()
    {
        return _ElectricCurrent;
    }

    public override float GetMotorSpeed(int index)
    {
        return _MotorSpeed[index];
    }

    public override float GetSpeed()
    {
        return _Speed;
    }

    public override Vector3 GetAttitude()
    {
        return _Attitude;
    }
    public override float GetDeep()
    {
        return _Deep;
    }

    public override float GetPressure()
    {
        return _Pressure;
    }
    public override float GetDampnessE()
    {
        return _DampnessE;
    }
    public override float GetDampnessB()
    {
        return _DampnessB;
    }
    #endregion

    #region Set Value
    void SetBattery(float v)
    {
        _Battery = v;
        Message.Send<BatteryChangeMessage>(new BatteryChangeMessage());
    }
    void SetVolt(float v)
    {
        _Volt = v;
        Message.Send<VoltChangeMessage>(new VoltChangeMessage());
    }
    void SetPropellerState(bool v)
    {
        _PropellerState = v;
        Message.Send<PropellerStateChangeMessage>(new PropellerStateChangeMessage());
    }

    void SetWifiState(int v)
    {
        _WifiState = v;

        Message.Send<WifiStateChangeMessage>(new WifiStateChangeMessage());
    }
    void SetCurrentTime(string v)
    {
        Message.Send<CurrentTimeChangeMessage>(new CurrentTimeChangeMessage());

    }
    void SetLaunchTime(string v)
    {
        Message.Send<LaunchTimeChangeMessage>(new LaunchTimeChangeMessage());
    }

    void SetCompass(float v)
    {
        _Compass = v;

        Message.Send<CompassChangeMessage>(new CompassChangeMessage());
    }

    void SetTemperature(float v)
    {
        _Temperature = v;
        Message.Send<TemperatureChangeMessage>(new TemperatureChangeMessage());
    }

    void SetAccelerate(Vector3 v)
    {
        _Accelerate = v;
        Message.Send<AccelerateChangeMessage>(new AccelerateChangeMessage());
    }


    void SetElectricCurrent(float v)
    {
        _ElectricCurrent = v;
        Message.Send<ElectricCurrentChangeMessage>(new ElectricCurrentChangeMessage());
    }

    void SetMotorSpeed(float v, int index)
    {
        _MotorSpeed[index] = v;


        MotorSpeedChangeMessage msg = new MotorSpeedChangeMessage();
        msg._Index = index;
        Message.Send<MotorSpeedChangeMessage>(msg);
    }

    void SetSpeed(float v)
    {
        _Speed = v;

        Message.Send<SpeedChangeMessage>(new SpeedChangeMessage());
    }

    void SetAttitude(Vector3 v)
    {
        _Attitude = v;

        Message.Send<AttitudeChangeMessage>(new AttitudeChangeMessage());
    }
    void SetDeep(float v)
    {
        _Deep = v;
        Message.Send<DeepChangeMessage>(new DeepChangeMessage());
    }

    void SetPressure(float v)
    {
        _Pressure = v;

        Message.Send<PressureChangeMessage>(new PressureChangeMessage());
    }
    void SetDampnessE(float v)
    {
        _DampnessE = v;
        Message.Send<DampnessEChangeMessage>(new DampnessEChangeMessage());
    }
    void SetDampnessB(float v)
    {
        _DampnessB = v;
        Message.Send<DampnessBChangeMessage>(new DampnessBChangeMessage());
    }

    #endregion

    private void Start()
    {
        _MotorSpeed = new float[5];
        StartCoroutine(IntervalSetValue());

    }
    protected override void OnDestroy()
    {
        StopAllCoroutines();
    }
    private void Update()
    {
        _LaunchTime += Time.deltaTime;
    }

    IEnumerator IntervalSetValue()
    {
        yield return new WaitForSeconds(1.0f);
        
        SetBattery(BaseFunction.g_msBatteryVolts);

        SetVolt(BaseFunction.g_slvBatteryVolts);

        SetPropellerState(_Volt > 50);

        SetWifiState(0);
        SetCurrentTime("");
        SetLaunchTime(Time.time.ToString());

        SetCompass(BaseFunction.g_modifiedAngle[0]);
        SetTemperature(0);

        SetAccelerate(UnityEngine.Random.insideUnitSphere * 50);

        SetElectricCurrent(BaseFunction.g_current[0]);

        SetMotorSpeed(BaseFunction.g_rpm[0], 0);
        SetMotorSpeed(BaseFunction.g_rpm[1], 1);
        SetMotorSpeed(BaseFunction.g_rpm[2], 2);
        SetMotorSpeed(BaseFunction.g_rpm[3], 3);
        SetMotorSpeed(BaseFunction.g_rpm[4], 4);

        SetSpeed(UnityEngine.Random.Range(0, 200));

        SetAttitude(UnityEngine.Random.insideUnitSphere * 360);
        SetDeep(BaseFunction.g_depth);

        SetPressure(BaseFunction.g_pressure);
        SetDampnessB(BaseFunction.g_hygro1);
        SetDampnessE(BaseFunction.g_hygro2);

        StartCoroutine(IntervalSetValue());
    }
}
