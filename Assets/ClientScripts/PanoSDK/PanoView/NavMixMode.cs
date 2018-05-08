using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMixMode : PanoModeBase
{
    NavCameraController _NavController;
    ScreenMeshHalfInCameraController[] _MeshHalfInControllerArr;
    ScrollPlaneMeshController _PlaneMeshController;
    public CameraControllerBase _PlaneCameraController;

    CameraControllerBase _CurrentControlCamera;

    public Renderer _HalfSphereMesh;
    public Renderer _PlaneMesh;
    public Renderer _PlaneMeshClone;


    public RectTransform _FrameNav;
    public RectTransform[] _FramesFull;
    public RectTransform _FramePlane;

    protected override void Awake()
    {
        _NavController = gameObject.GetComponentsInChildren<NavCameraController>(true)[0];
        _MeshHalfInControllerArr = gameObject.GetComponentsInChildren<ScreenMeshHalfInCameraController>(true);
        _PlaneMeshController = gameObject.GetComponentsInChildren<ScrollPlaneMeshController>(true)[0];

        CalculateRect();
    }
    protected override void Start()
    {
        base.Start();
        SwitchCamera(_MeshHalfInControllerArr[0]);
    }

    void CalculateRect()
    {
        float navWidthPortion = (float)2 / 3;
        float navHeight = (float)Screen.width * navWidthPortion / Screen.height;

        float planeHeight = navHeight / 2;

        float yStart = 0.5f - (navHeight + planeHeight) / 2;

        _NavController.gameObject.GetComponent<Camera>().rect = new Rect(0, yStart + planeHeight, navWidthPortion, navHeight);
        _MeshHalfInControllerArr[0].gameObject.GetComponent<Camera>().rect = new Rect(navWidthPortion, yStart + planeHeight, 1 - navWidthPortion, navHeight / 2);
        _MeshHalfInControllerArr[1].gameObject.GetComponent<Camera>().rect = new Rect(navWidthPortion, yStart + planeHeight + planeHeight, 1 - navWidthPortion, navHeight / 2);
        _PlaneCameraController.gameObject.GetComponent<Camera>().rect = new Rect(0, yStart, 1, planeHeight);


        _FrameNav.anchorMax = new Vector2(navWidthPortion, yStart + planeHeight + navHeight);
        _FrameNav.anchorMin = new Vector2(0, yStart + planeHeight);
        _FrameNav.offsetMax = Vector2.zero;
        _FrameNav.offsetMin = Vector2.zero;

        _FramesFull[0].anchorMax = new Vector2(1, yStart + planeHeight + navHeight / 2);
        _FramesFull[0].anchorMin = new Vector2(navWidthPortion, yStart + planeHeight);
        _FramesFull[0].offsetMax = Vector2.zero;
        _FramesFull[0].offsetMin = Vector2.zero;

        _FramesFull[1].anchorMax = new Vector2(1, yStart + planeHeight + navHeight);
        _FramesFull[1].anchorMin = new Vector2(navWidthPortion, yStart + planeHeight + navHeight / 2);
        _FramesFull[1].offsetMax = Vector2.zero;
        _FramesFull[1].offsetMin = Vector2.zero;



        _FramePlane.anchorMax = new Vector2(1, yStart + planeHeight);
        _FramePlane.anchorMin = new Vector2(0, yStart);
        _FramePlane.offsetMax = Vector2.zero;
        _FramePlane.offsetMin = Vector2.zero;
    }

    #region Finger Gesture

    public override void OnDrag(DragGesture gesture)
    {
        if (_CurrentControlCamera)
        {
            if(_CurrentControlCamera == _PlaneCameraController)
            {
                _PlaneMeshController.OnDrag(gesture);
            }
            else
            {

                _CurrentControlCamera.OnDrag(gesture);
            }
        }
    }
    public override void OnPinch(PinchGesture gesture)
    {
        if (_CurrentControlCamera)
        {
            _CurrentControlCamera.OnPinch(gesture);
        }
    }

    public override void OnSimpleFingerDown(object v)
    {
        Vector3 v3 = (Vector3)v;
        _CurrentControlCamera = GetControllerByInputPos(v3);
        if (_CurrentControlCamera)
        {
            if (_CurrentControlCamera == _PlaneCameraController)
            {
                _PlaneMeshController.OnSimpleFingerDown(v);
            }
            else
            {

                _CurrentControlCamera.OnSimpleFingerDown(v);
            }
            SwitchCamera(_CurrentControlCamera);
        }
    }
    public override void OnSimpleFingerUp(object v)
    {
        Vector3 v3 = (Vector3)v;
        if (_CurrentControlCamera)
        {
            _CurrentControlCamera.OnSimpleFingerUp(v);
        }
    }
    #endregion

    CameraControllerBase GetControllerByInputPos(Vector2 pos)
    {
        if(_PlaneCameraController.CheckInCameraScreenArea(pos))
        {
            return _PlaneCameraController;
        }
        if (_NavController.CheckInCameraScreenArea(pos))
        {
            return _NavController;
        }
        foreach(ScreenMeshHalfInCameraController controller in _MeshHalfInControllerArr)
        {
            if (controller.CheckInCameraScreenArea(pos))
            {
                return controller;
            }
        }
        
        return null;
    }
    void SwitchCamera(CameraControllerBase basecam)
    {
        _CurrentControlCamera = basecam;
        if (_CurrentControlCamera == _NavController)
        {
        }
        else if (_CurrentControlCamera == _PlaneCameraController)
        {
            foreach (ScreenMeshHalfInCameraController smic in _MeshHalfInControllerArr)
            {
                smic.gameObject.GetComponentsInChildren<GVProjector>(true)[0].gameObject.SetActive(false);
            }
            _NavController.SetControlledCamera(null);


            _FrameNav.gameObject.SetActive(false);
            _FramesFull[0].gameObject.SetActive(false);
            _FramesFull[1].gameObject.SetActive(false);
            _FramePlane.gameObject.SetActive(true);
        }
        else
        {
            ScreenMeshHalfInCameraController smc = _CurrentControlCamera as ScreenMeshHalfInCameraController;
            _NavController.SetControlledCamera(smc);

            int i = 0;
            foreach (ScreenMeshHalfInCameraController smic in _MeshHalfInControllerArr)
            {
                if (smic == smc)
                {
                    smic.gameObject.GetComponentsInChildren<GVProjector>(true)[0].gameObject.SetActive(true);
                }
                else
                {

                    smic.gameObject.GetComponentsInChildren<GVProjector>(true)[0].gameObject.SetActive(false);
                }

                _FramesFull[i].gameObject.SetActive(smic == smc);
                i++;
            }

            _FrameNav.gameObject.SetActive(false);
            _FramePlane.gameObject.SetActive(false);
        }
    }
    public override void ResetTexture()
    {
        Texture rtex = PanoManager.Instance.GetRenderTexture();
        PanoManager.PanoTextureForOneDevice dv = PanoManager.Instance.GetPanoTextureDeviceArr(0);
        Texture[] texarr = dv._TextureArr;

        _HalfSphereMesh.sharedMaterial.SetTexture("_MainTex", rtex);

        if(texarr.Length > 0)
        {
            //Shader shader = PanoManager.Instance.GetCurrentShader();

            Material mat = new Material(_PlaneMesh.sharedMaterial.shader);
            _PlaneMesh.sharedMaterial = mat;
            _PlaneMeshClone.sharedMaterial = mat;

            PanoManager.EPANOTEXTUREMODE texmode = PanoManager.Instance.GetTextureMode();

            SetPlaneMeshMaterial(texmode, texarr, mat);
        }
    }


    void SetPlaneMeshMaterial(PanoManager.EPANOTEXTUREMODE texMode, Texture[] texarr,Material mat)
    {
        if (texMode == PanoManager.EPANOTEXTUREMODE.EPM_YUV)
        {
            if (texarr.Length > 0)
            {

                mat.SetTexture("_MainTex", texarr[0]);
            }
            if (texarr.Length > 1)
            {
                mat.SetTexture("_MainTexU", texarr[1]);

            }
            if (texarr.Length > 2)
            {
                mat.SetTexture("_MainTexV", texarr[2]);

            }
        }
        else if (texMode == PanoManager.EPANOTEXTUREMODE.EMP_RGB)
        {
            if (texarr.Length > 0)
            {
                mat.SetTexture("_MainTex", texarr[0]);
            }
        }
        else if (texMode == PanoManager.EPANOTEXTUREMODE.EPM_SurfaceToTexture)
        {
            if (texarr.Length > 0)
            {
                mat.SetTexture("_MainTex", texarr[0]);
            }
        }
    }
}
