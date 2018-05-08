using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneAxisOffsetMeter : MeterBase<float> {
    
    public float _MinValue = -100;
    public float _MaxValue = 100;

    public float _AnimationSpeed = 1.0f;

    public Transform _MaxPosition;
    public Transform _MinPosition;

    float _CurrentAnimationValue;
    Vector3 _PerValueDist;

    public Transform _ControlledTransform;

    protected override void Start()
    {
        base.Start();


        _PerValueDist = (_MaxPosition.localPosition - _MinPosition.localPosition) / (_MaxValue - _MinValue);

    }

    protected override void UpdateValue()
    {
        _CurrentAnimationValue = Mathf.Lerp(_CurrentAnimationValue, _CurrentValue, Time.deltaTime * _AnimationSpeed);
        _CurrentAnimationValue = Mathf.Clamp(_CurrentAnimationValue, _MinValue, _MaxValue);


        Vector3 pos = _MinPosition.localPosition + _PerValueDist * (_CurrentAnimationValue - _MinValue);
        _ControlledTransform.localPosition = pos;

    }
}
