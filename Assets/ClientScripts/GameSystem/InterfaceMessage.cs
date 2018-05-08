using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreFramework;

public class InterfaceMessage<T> : Message {
    T _Value;
    
}

public class BatteryChangeMessage : InterfaceMessage<float>
{
    public float _Value;
}
public class VoltChangeMessage : InterfaceMessage<float>
{
    public float _Value;
}
public class PropellerStateChangeMessage : InterfaceMessage<bool>
{
    public bool _Value;
}
public class WifiStateChangeMessage : InterfaceMessage<int>
{
    public int _Value;
}
public class CurrentTimeChangeMessage : InterfaceMessage<string>
{
    public string _Value;
}
public class LaunchTimeChangeMessage : InterfaceMessage<string>
{
    public string _Value;
}
public class CompassChangeMessage : InterfaceMessage<float>
{
    public float _Value;
}
public class TemperatureChangeMessage : InterfaceMessage<float>
{
    public float _Value;
}
public class AccelerateChangeMessage : InterfaceMessage<float>
{
    public Vector3 _Value;
}
public class ElectricCurrentChangeMessage : InterfaceMessage<float>
{
    public float _Value;
}
public class MotorSpeedChangeMessage : InterfaceMessage<float>
{
    public int _Index;
    public float _Value;
}
public class SpeedChangeMessage : InterfaceMessage<float>
{
    public float _Value;
}
public class AttitudeChangeMessage : InterfaceMessage<Vector3>
{
    public Vector3 _Value;
}
public class DeepChangeMessage : InterfaceMessage<float>
{
    public float _Value;
}
public class PressureChangeMessage : InterfaceMessage<float>
{
    public float _Value;
}
public class DampnessBChangeMessage : InterfaceMessage<float>
{
    public float _Value;
}
public class DampnessEChangeMessage : InterfaceMessage<float>
{
    public float _Value;
}
