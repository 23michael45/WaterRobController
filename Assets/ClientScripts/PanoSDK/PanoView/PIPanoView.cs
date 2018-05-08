using CoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TapRecognizer))]
[RequireComponent(typeof(DragRecognizer))]
[RequireComponent(typeof(PinchRecognizer))]
[RequireComponent(typeof(PISimpleFingerRecognizer))]
public class PIPanoView : MonoSingleton<PIPanoView> {
    
    public Dictionary<EPANOMESHMODE, PanoModeBase> _ModeDic = new Dictionary<EPANOMESHMODE, PanoModeBase>();
    public Dictionary<EPANOMESHMODE, Camera[]> _CameraDic = new Dictionary<EPANOMESHMODE, Camera[]>();

    public EPANOMESHMODE _CurrentMode;
    public enum EPANOMESHMODE
    {
        EPM_NONE = -1,   //黑屏模式
        EPM_FOUR = 0,    //四分屏模式
        EPM_NAV,        //导航模式
        EPM_NAVRT,      //导航(UI渲染模式）
        EPM_NAVMIX,     //导航混合模式
        EPM_FULL,       //全屏半球模式
        EPM_CYLINDER,   //圆柱模式
        EPM_CYLINDERPLANE,   //平铺及圆柱模式（平铺用极坐标）
        EPM_WIDEANGLE,  //广角模式
        EPM_SCROLLPLANE,//双平面模式（极坐标）
        EPM_SPHEREIN,    //内圆沉浸模式
        EPM_SPHEREOUT,      //外圆模式
        EPM_SPHEREASTEROID, //小行星模式
        EPM_VR,         //VR模式
        EPM_RAWPLANE,   //原始平面模式 
        EPM_RAWPOLARPLANE,    //极坐标原始平面模式 
        EPM_HALFSPHEREUP,    //半球沉浸模式 上半球
        EPM_HALFSPHEREDOWN,    //半球沉浸模式 下半球
        EPM_RAW,   //原始模式 用于显示原始摄相头数据 
        EPM_CAR,   //车辆模式 
        EPM_HALFSPHEREUPVR,    //半球沉浸模式 上半球 VR
        EMP_MAX,
    }


    private void Awake()
    {
        PanoModeBase[]  _PanoModeArr = GetComponentsInChildren<PanoModeBase>(true);
        foreach (PanoModeBase mode in _PanoModeArr)
        {
            mode.gameObject.SetActive(false);
            EPANOMESHMODE emode = (EPANOMESHMODE)Enum.Parse(typeof(EPANOMESHMODE), mode.gameObject.name, true);
            _ModeDic.Add(emode, mode);
        }

        foreach (PanoModeBase mode in _PanoModeArr)
        {
            EPANOMESHMODE emode = (EPANOMESHMODE)Enum.Parse(typeof(EPANOMESHMODE), mode.gameObject.name, true);

            Camera[] mCamera = mode.gameObject.GetComponentsInChildren<Camera>(true);

            //Debug.LogError("emode: " + emode + " cameraNum = " + mCamera.Length);

            _CameraDic.Add(emode, mCamera);
        }

        EnablePanoMode(_CurrentMode);
    }
    // Use this for initialization
    void Start() {

    }
    private void OnEnable()
    {
        Message.AddListener<PanoTextureResetMsg>(OnPanoTextureReset);
    }
    private void OnDisable()
    {

        Message.RemoveListener<PanoTextureResetMsg>(OnPanoTextureReset);
    }

    // Update is called once per frame
    void Update() {

    }

  

    public void EnablePanoMode(EPANOMESHMODE emode)
    {
        foreach(KeyValuePair<EPANOMESHMODE,PanoModeBase> kv in _ModeDic)
        {
            if(kv.Key == emode)
            {
                _CurrentMode = emode;
                kv.Value.gameObject.SetActive(true);

                //每次切模式要重新SETMATERIAL
                PanoManager.Instance.SetPanoMode(PanoManager.EPANOMODE.OLD);
            }
            else
            {

                kv.Value.gameObject.SetActive(false);
            }
        }
    }
    public PanoModeBase GetCurrentMode()
    {
        if (_ModeDic.ContainsKey(_CurrentMode))
        {
            return _ModeDic[_CurrentMode];
        }
        return null; 
    }

#region Finger Gesture
    
    public void OnDrag(DragGesture gesture)
    {
        if (_ModeDic.ContainsKey(_CurrentMode))
        {
            _ModeDic[_CurrentMode].OnDrag(gesture);
        }

    }
    public void OnPinch(PinchGesture gesture)
    {
        if (_ModeDic.ContainsKey(_CurrentMode))
        {
            _ModeDic[_CurrentMode].OnPinch(gesture);
        }
    }
    public void OnTap(TapGesture gesture)
    {

        if (_ModeDic.ContainsKey(_CurrentMode))
        {
            _ModeDic[_CurrentMode].OnTap(gesture);
        }

    }
    public void OnDoubleTap(TapGesture gesture)
    {
        if (_ModeDic.ContainsKey(_CurrentMode))
        {
            _ModeDic[_CurrentMode].OnDoubleTap(gesture);
        }
    }

    public void OnSimpleFingerDown(object v)
    {
        if (_ModeDic.ContainsKey(_CurrentMode))
        {
            _ModeDic[_CurrentMode].OnSimpleFingerDown(v);
        }
    }
    public void OnSimpleFingerUp(object v)
    {
        if (_ModeDic.ContainsKey(_CurrentMode))
        {
            _ModeDic[_CurrentMode].OnSimpleFingerUp(v);
        }
    }
#endregion

    public void FlipY(bool b)
    {
        foreach(KeyValuePair<EPANOMESHMODE, PanoModeBase> kv in _ModeDic)
        {
            if(kv.Value != null)
            {
                kv.Value.FlipY(b);
            }
        }

    }
    public void FlipX(bool b)
    {

        foreach (KeyValuePair<EPANOMESHMODE, PanoModeBase> kv in _ModeDic)
        {
            if (kv.Value != null)
            {
                kv.Value.FlipX(b);
            }
        }

    }
    public void SetXRotate(float rot)
    {
        foreach (KeyValuePair<EPANOMESHMODE, PanoModeBase> kv in _ModeDic)
        {
            if (kv.Value != null)
            {
                kv.Value.SetXRotate(rot);
            }
        }
    }

    public void EnableGyroscope(bool b)
    {
        foreach (KeyValuePair<EPANOMESHMODE, PanoModeBase> kv in _ModeDic)
        {
            if (kv.Value != null)
            {
                kv.Value.EnableGyroscope(b);
            }
        }
    }


#region Message

    void OnPanoTextureReset(PanoTextureResetMsg msg)
    {
        if (_ModeDic.ContainsKey(_CurrentMode))
        {
            _ModeDic[_CurrentMode].ResetTexture();
        }
    }
#endregion
}
