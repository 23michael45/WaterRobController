// on OpenGL ES there is no way to query texture extents from native texture id
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
#define UNITY_GLES_RENDERER
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using CoreFramework;
using System;
using Aubergine;

public class PostProcessManager : MonoSingleton<PostProcessManager>
{
    /// <summary>
    /// 后处理模式
    /// </summary>
    public enum EPOSTPROCESSMODE
    {
        PP_NONE = -1,

        PP_Bleach = 0,
        PP_Charcoal,
        PP_Contours,
        PP_Desaturate,
        PP_Emboss,
        PP_Godrays,
        PP_Pixelated,
        PP_Posterize,
        PP_Vintage,
        PP_Blueberry,
        PP_Amethyst,

        PP_4Bit,
        PP_Amnesia,
        PP_BlackAndWhite,
        PP_BlurH,
        PP_BlurV,
        PP_Crosshatch,
        PP_Displacement,
        PP_DreamBlur,
        PP_DreamBlurColor,
        PP_FakeHDR,
        PP_FakeSSAO,
        PP_Frost,
        PP_HeightFog,
        PP_Holywood,
        PP_LensCircle,
        PP_Lightshafts,
        PP_LightWave,
        PP_LineArt,
        PP_Negative,
        PP_NightVision,
        PP_NightVisionV2,
        PP_Noise,
        PP_Pulse,
        PP_RadialBlur,
        PP_RadialUndistortion,
        PP_SecurityCamera,
        PP_SimpleBloom,
        PP_SinCity,
        PP_SobelEdge,
        PP_Spherical,
        PP_ThermalVision,
        PP_ThermalVisionV2,
        PP_Thicken,
        PP_Tonemap,
        PP_Vignette,
        PP_Waves,
        PP_Wiggle,
        PP_HSV,
        PP_BravoX,

        MAX,
    }

    Dictionary<EPOSTPROCESSMODE, PostProcessBase> mPostProcessDic = new Dictionary<EPOSTPROCESSMODE, PostProcessBase>();
    EPOSTPROCESSMODE _CurPostProcessMode = EPOSTPROCESSMODE.MAX;

    #region Public Function

    public void Start()
    {
        GameObject root = GameObject.Find("SDK");

        GameObject mCamera = root.transform.Find("AppMain/PanoManager/Group/Camera").gameObject;

        foreach (int epp in Enum.GetValues(typeof(EPOSTPROCESSMODE)))
        {
            string eppname = Enum.GetName(typeof(EPOSTPROCESSMODE), epp);//获取名称

            if (eppname != "MAX" && eppname != "PP_NONE")
            {
                Type t = Type.GetType("Aubergine." + eppname);

                PostProcessBase ppb = mCamera.AddComponent(t) as PostProcessBase;
                ppb.enabled = false;

                mPostProcessDic.Add((EPOSTPROCESSMODE)epp, ppb);

                //Debug.LogError("eppname: " + t);
            }
        }
        //mPostProcessDic[EPOSTPROCESSMODE.PP_4Bit].enabled = true;
    }

    public void SetPostProcessMode(EPOSTPROCESSMODE emode)
    {
        foreach (KeyValuePair<EPOSTPROCESSMODE, PostProcessBase> kv in mPostProcessDic)
        {
            if (kv.Key == emode)
            {
                _CurPostProcessMode = emode;
                kv.Value.enabled = true;
            }
            else
            {
                kv.Value.enabled = false;
            }
        }
    }

    #endregion
}