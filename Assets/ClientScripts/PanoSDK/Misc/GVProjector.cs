using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GVProjector : MonoBehaviour {

    CameraControllerBase _CurrentController;
    Projector _Projector;
    private void Awake()
    {
        _Projector = gameObject.GetComponent<Projector>();
    }
    private void Start()
    {

    }
    private void Update()
    {
        UpdateFOV();
    }

    public void SetController(CameraControllerBase controller)
    {
        _CurrentController = controller;

        if(_CurrentController)
        {
            transform.parent = _CurrentController.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        UpdateFOV();


    }

    void UpdateFOV()
    {
        if (_Projector && _CurrentController && _CurrentController._ControlCamera)
        {
            _Projector.fieldOfView = _CurrentController._ControlCamera.fieldOfView;
            _Projector.farClipPlane = _CurrentController._ControlCamera.farClipPlane;
            _Projector.nearClipPlane = _CurrentController._ControlCamera.nearClipPlane;
            _Projector.orthographic = _CurrentController._ControlCamera.orthographic;
            _Projector.orthographicSize = _CurrentController._ControlCamera.orthographicSize;
            _Projector.aspectRatio = _CurrentController._ControlCamera.aspect;

        }

    }
}
