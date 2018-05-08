using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMode : PanoModeBase {

    CarVision[] _VisionArr;

    public Camera _Camera;
    public Transform _Car;

    int _CurrentVisionIndex = 0;
    protected override void Start()
    {
        _VisionArr = gameObject.GetComponentsInChildren<CarVision>(true);
        if(_VisionArr.Length > 0)
        {

            SetVision(_CurrentVisionIndex);
        }

    }

    public void SetVision(int index)
    {
        if(index < _VisionArr.Length)
        {
            _VisionArr[index].SetTransform(_Car, _Camera.transform);
        }
    }

    public void SetNextVison()
    {

        _CurrentVisionIndex++;
        if(_CurrentVisionIndex >= _VisionArr.Length)
        {
            _CurrentVisionIndex = 0;
        }
        SetVision(_CurrentVisionIndex);
    }
}
