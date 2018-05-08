using CoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDataInterface : HardwareInterfaceBase
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

        SetBattery(UnityEngine.Random.Range(0, 100));
        SetVolt(UnityEngine.Random.Range(0, 100));

        SetPropellerState(_Volt > 50);

        SetWifiState(UnityEngine.Random.Range(0, 100));
        SetCurrentTime("");
        SetLaunchTime("");

        SetCompass(UnityEngine.Random.Range(0, 360));
        SetTemperature(UnityEngine.Random.Range(-50, 50));
        SetAccelerate(UnityEngine.Random.insideUnitSphere * 50);

        SetElectricCurrent(UnityEngine.Random.Range(0, 20));

        SetMotorSpeed(UnityEngine.Random.Range(0, 2000), 0);
        SetMotorSpeed(UnityEngine.Random.Range(0, 2000), 1);
        SetMotorSpeed(UnityEngine.Random.Range(0, 2000), 2);
        SetMotorSpeed(UnityEngine.Random.Range(0, 2000), 3);
        SetMotorSpeed(UnityEngine.Random.Range(0, 2000), 4);

        SetSpeed(UnityEngine.Random.Range(0, 200));

        SetAttitude(UnityEngine.Random.insideUnitSphere * 360);
        SetDeep(UnityEngine.Random.Range(-100, 0));

        SetPressure(UnityEngine.Random.Range(0, 200));
        SetDampnessB(UnityEngine.Random.Range(0, 100));
        SetDampnessE(UnityEngine.Random.Range(0, 100));

        StartCoroutine(IntervalSetValue());
    }
}
