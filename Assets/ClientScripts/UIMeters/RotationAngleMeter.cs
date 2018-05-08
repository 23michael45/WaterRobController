using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAngleMeter : MeterBase<float> {

    public float _StartAngle = 0;
    public float _EndAngle = 360;

    public float _StartValue = 0;
    public float _EndValue = 360;
    public float _AnimationSpeed = 1.0f;

    public Transform _ControlledTrans;

    float _ValuePerDegree;
    float _CurrentDegree;
    float _CurrentAnimationValue;
    Quaternion _OrgRotation;
    protected override void Start()
    {
        base.Start();

        _ValuePerDegree =  (_EndAngle - _StartAngle)/ (_EndValue - _StartValue);

        if(_ControlledTrans == null)
        {
            _ControlledTrans = transform;
        }

        if (_ControlledTrans)
        {
            _OrgRotation = _ControlledTrans.localRotation;
        }
    }

    protected override void UpdateValue()
    {
        _CurrentAnimationValue = Mathf.Lerp(_CurrentAnimationValue, _CurrentValue, Time.deltaTime * _AnimationSpeed);

        if(_StartValue >= _EndValue)
        {

            _CurrentAnimationValue = Mathf.Clamp(_CurrentAnimationValue, _EndValue, _StartValue);
        }
        else
        {
            _CurrentAnimationValue = Mathf.Clamp(_CurrentAnimationValue, _StartValue, _EndValue);

        }

        _CurrentDegree = _StartAngle + _ValuePerDegree * (_CurrentAnimationValue - _StartValue);

        if(_ControlledTrans)
        {
            _ControlledTrans.localRotation = Quaternion.Euler(0, 0, _CurrentDegree) * _OrgRotation;
        }
    }
    
}
