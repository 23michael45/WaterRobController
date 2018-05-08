using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLightMeter : MeterBase<float> {

    public int _TotalCount =5;
    public float _MinValue = 0;
    public float _MaxValue = 100;

    public float _AnimationSpeed = 1.0f;

    float _ValuePerCount;
    float _CurrentCount;
    float _CurrentAnimationValue;

    Transform[] _ControlledTransformArr;
    

    protected override void Start()
    {
        base.Start();
        _ControlledTransformArr = new Transform[_TotalCount];
        for (int i = 0;i< gameObject.transform.childCount; i++)
        {
            _ControlledTransformArr[i] = gameObject.transform.GetChild(i);
        }


        _ValuePerCount = _TotalCount / (_MaxValue - _MinValue);

      
    }

    protected override void UpdateValue()
    {
        _CurrentAnimationValue = Mathf.Lerp(_CurrentAnimationValue, _CurrentValue, Time.deltaTime * _AnimationSpeed);
        _CurrentAnimationValue = Mathf.Clamp(_CurrentAnimationValue, 0, _MaxValue);
        _CurrentCount = _ValuePerCount * (_CurrentAnimationValue - _MinValue);

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Image img = _ControlledTransformArr[i].gameObject.GetComponent<Image>();
            if (Mathf.FloorToInt(_CurrentCount )== i)
            {
                img.gameObject.SetActive(true);

            }
            else
            {
                img.gameObject.SetActive(false);

            }
        }
    }
}
