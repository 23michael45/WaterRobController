using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardwareInterfaceBase : MonoSingleton<HardwareInterfaceBase> {




    public virtual float GetBattery()
    {
        return 0;
    }
    public virtual float GetVolt()
    {
        return 0;
    }
    public virtual bool GetPropellerState()
    {
        return false;
    }

    public virtual int GetWifiState()
    {
        return 0;
    }
    public virtual string GetCurrentTime()
    {
        return "00:00:00 AM";
    } 
    public virtual string GetLaunchTime()
    {
        return "00:00:00";
    }

    public virtual float GetCompass()
    {
        return 0;
    }

    public virtual float GetTemperature()
    {
        return 0;
    }

    public virtual Vector3 GetAccelerate()
    {
        return Vector3.zero;
    }


    public virtual float GetElectricCurrent()
    {
        return 0;
    }

    public virtual float GetMotorSpeed(int index)
    {
        return 0;
    }

    public virtual float GetSpeed()
    {
        return 0;
    }

    public virtual Vector3 GetAttitude()
    {
        return Vector3.zero;
    }
    public virtual float GetDeep()
    {
        return 0;
    }

    public virtual float GetPressure()
    {
        return 0;
    }
    public virtual float GetDampnessE()
    {
        return 0;
    }
    public virtual float GetDampnessB()
    {
        return 0;
    }
}

