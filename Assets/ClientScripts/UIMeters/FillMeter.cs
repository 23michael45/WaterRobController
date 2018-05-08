using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillMeter : MeterBase<float>
{

    public Image.FillMethod _FillType = Image.FillMethod.Horizontal;

    public float _StartAmount = 0;
    public float _EndAmount = 1;

    public float _StartValue = 0;
    public float _EndValue = 360;
    public float _AnimationSpeed = 1.0f;

    public Image _ControlledBar;

    float _ValuePerUnit;
    float _CurrentDegree;
    float _CurrentAnimationValue;
    Quaternion _OrgRotation;
    protected override void Start()
    {
        base.Start();

        _ValuePerUnit = (_EndAmount - _StartAmount) / (_EndValue - _StartValue);

        if (_ControlledBar == null)
        {
            _ControlledBar = gameObject.GetComponentsInChildren<Image>(true)[0];
            _ControlledBar.fillMethod = _FillType;
        }

       
    }

    protected override void UpdateValue()
    {
        _CurrentAnimationValue = Mathf.Lerp(_CurrentAnimationValue, _CurrentValue, Time.deltaTime * _AnimationSpeed);

        if (_StartValue >= _EndValue)
        {

            _CurrentAnimationValue = Mathf.Clamp(_CurrentAnimationValue, _EndValue, _StartValue);
        }
        else
        {
            _CurrentAnimationValue = Mathf.Clamp(_CurrentAnimationValue, _StartValue, _EndValue);

        }
        _CurrentDegree = _StartAmount + _ValuePerUnit * (_CurrentAnimationValue - _StartValue);

        if (_ControlledBar)
        {
            _ControlledBar.fillAmount = _CurrentDegree;
        }
        
    }
}
