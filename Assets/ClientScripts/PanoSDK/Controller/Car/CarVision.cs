using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarVision : MonoBehaviour {

    public Transform _CarTransform;
    public Transform _CameraTransform;


    public void SetTransform(Transform srcCar,Transform _srcCamera)
    {
        srcCar.position = _CarTransform.position;
        srcCar.rotation = _CarTransform.rotation;
        srcCar.localScale = _CarTransform.localScale;


        _srcCamera.position = _CameraTransform.position;
        _srcCamera.rotation = _CameraTransform.rotation;
        _srcCamera.localScale = _CameraTransform.localScale;
    }
}
