// on OpenGL ES there is no way to query texture extents from native texture id
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
#define UNITY_GLES_RENDERER
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using CoreFramework;
using Aubergine;
using System;

public class PanoManager : MonoSingleton<PanoManager>
{
    /// <summary>
    /// 媒体拼接展示模式
    /// </summary>
    public enum EPANOMODE
    {
        OLD = -1, //保持上一个不变
        PANO21 = 0,// 2:1 拼接处理
        PANO11,    // 1:1 拼接处理
        RAW21,               // 原图为2:1
        RAW11,               // 原图为1:1
        ONEEYE,              // 单目相机展开
        MAX,

    }

    /// <summary>
    /// Material的贴图模式
    /// </summary>
    public enum EPANOTEXTUREMODE
    {
        EMP_OLD = -1, //保持上一个不变
        EMP_RGB = 0,
        EMP_ALPHARGB,
        EPM_YUV,
        EPM_SurfaceToTexture,
        MAX,
    }

    public class PanoTextureForOneDevice
    {
        public Texture[] _TextureArr;
        public PanoTextureForOneDevice(params Texture[]  arr)
        {
            _TextureArr = arr;
        }
    }
    

    [HideInInspector]
    public EPANOMODE _CurShowMode = EPANOMODE.PANO21;
    EPANOTEXTUREMODE _CurrentTextureMode = EPANOTEXTUREMODE.EMP_RGB;

    [HideInInspector]
    public Camera mCameraPreRender;

    [HideInInspector]
    public RenderTexture mPreRenderTexture;

    
    Dictionary<EPANOMODE, PanoMeshBase> mPanoMeshDic = new Dictionary<EPANOMODE, PanoMeshBase>();

    Vector2 mMediaSize = Vector2.one;
    Vector2 mContentSize = Vector2.one;


    [HideInInspector]
    public PanoTextureForOneDevice[] mPanoTextureDeviceArr;

    public static bool mGettingCurrentFrame = false;

    public RenderTexture GetRenderTexture()
    {
        return mPreRenderTexture;
    }
    public PanoTextureForOneDevice GetPanoTextureDeviceArr(int index)
    {
        if(mPanoTextureDeviceArr != null && index < mPanoTextureDeviceArr.Length)
        {
            return mPanoTextureDeviceArr[index];
        }
        return null;
    }
    #region Public Function

    public void Start()
    {
        Camera[] arr = gameObject.GetComponentsInChildren<Camera>(true);
        if (arr.Length > 0)
        {

            mCameraPreRender = arr[0];
            mPreRenderTexture = new RenderTexture(16, 16, 0, RenderTextureFormat.ARGB32);
            mPreRenderTexture.Release();
            mPreRenderTexture.wrapMode = TextureWrapMode.Repeat;
            mPreRenderTexture.anisoLevel = 0;

            mCameraPreRender.targetTexture = mPreRenderTexture;

            PanoMeshBase[] panomeshs = gameObject.GetComponentsInChildren<PanoMeshBase>(true);
            foreach (PanoMeshBase pmb in panomeshs)
            {
                mPanoMeshDic.Add(pmb._PanoShowMode, pmb);
            }

            ReSetUV(null);
        }



  
    }

    public void SetUpsideDown(bool b)
    {
        if (b)
        {
            mCameraPreRender.transform.localRotation = Quaternion.identity;
            mCameraPreRender.transform.Rotate(Vector3.up, 180);
            mCameraPreRender.transform.Rotate(Vector3.forward, 180);
        }
        else
        {

            mCameraPreRender.transform.localRotation = Quaternion.identity;
            mCameraPreRender.transform.Rotate(Vector3.up, 180);
        }
    }
    /// <summary>
    /// 获取当前展开图的帧
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public Texture2D GetCurrentFrame(int width, int height)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        RenderTexture currentActiveRT = RenderTexture.active;//Cache
        mCameraPreRender.targetTexture = renderTexture;

        mGettingCurrentFrame = true;
        mCameraPreRender.Render();
        mGettingCurrentFrame = false;

        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = currentActiveRT;
        mCameraPreRender.targetTexture = mPreRenderTexture;
        return tex;
    }
    public EPANOTEXTUREMODE GetTextureMode()
    {
        return _CurrentTextureMode;
    }

    public void GetPreRenderTextureSize(out int w, out int h)
    {
        w = mPreRenderTexture.width;
        h = mPreRenderTexture.height;
    }
    
    public void SetPreRenderTextureSize(int width, int height)
    {
        Debug.LogFormat("---------------------------SetPreRenderTextureSize w:{0} h:{1}", width, height);
        if(mPreRenderTexture == null)
        {
            Debug.LogFormat("---------------------------SetPreRenderTextureSize error");
            return;
        }
        if ((width != mPreRenderTexture.width || height != mPreRenderTexture.height) ||
            mCameraPreRender.targetTexture == null)
        {
            //Debug.LogFormat("haha:{0} {1}", width, height);
            if (_CurShowMode == EPANOMODE.ONEEYE && width == height)//单目的也需要拉伸到2：1，但是传入的texture都是1：1的
                width *= 2;

            mPreRenderTexture.Release();
            mPreRenderTexture.width = width;
            mPreRenderTexture.height = height;
            mPreRenderTexture.wrapMode = TextureWrapMode.Repeat;
            mPreRenderTexture.anisoLevel = 0;
            mCameraPreRender.targetTexture = mPreRenderTexture;

            //修改RenderTexure后需要更新Camera的视野
            mCameraPreRender.enabled = false;
            mCameraPreRender.enabled = true;



            PanoSizeResetMsg msg = new PanoSizeResetMsg();
            msg._Width = width;
            msg._Height = height;
            PanoSizeResetMsg.Send<PanoSizeResetMsg>(msg);
        }
    }


    public delegate void GetPanoTextureAsyn(string url);
    public IEnumerator GetPanoTextureProcess(string url, GetPanoTextureAsyn cb)
    {
        string wwwurl = "file:///" + url;

        WWW www = new WWW(wwwurl);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.05f);
        }

        if (string.IsNullOrEmpty(www.error))
        {
            Texture2D tex = new Texture2D(4, 4, TextureFormat.RGB24, true);
            www.LoadImageIntoTexture(tex);

            int oldwidth = mPreRenderTexture.width;
            int oldheight = mPreRenderTexture.height;

            if (tex)
            {
                SetPreRenderTextureSize(tex.width, tex.height);
            }
            SetPanoMode(_CurShowMode,PanoManager.EPANOTEXTUREMODE.EMP_RGB, new PanoManager.PanoTextureForOneDevice(tex));
            Texture2D panotex = GetCurrentFrame(tex.width, tex.height);


            DestroyImmediate(tex);


            SetPreRenderTextureSize(oldwidth, oldheight);


            byte[] arr = panotex.EncodeToJPG();
            string savepath = url;


            File.WriteAllBytes(savepath, arr);//存储png图

            Destroy(panotex);

            if (cb != null)
            {
                cb(url);
            }
        }
    }
    
    #region ResetUV
    //N8
    static readonly string defaultParam = "PE000160PIPIPIPIA28A9682E8B7F0CB295570F1629C95BE3EA13EC50A5AF0A9341E42EA0F82DFD24E4A698F44DF546EF2FD9881C23FADA565F546736B1CB7D5C81517A5FCE354C54FCD6EB3350776D72ECA1A0E164B9C7FDD8F2ADA6E90DBD3F941AEF8D71A6FA7C95E59FA8FF1A90629E4ACC667DE23018AB26EBE8AA6BD99B59B87E8198E9949B7C44820D16E7013A28145E746451DC0EB44F850C0EABC55067A20EA074B519A3E2C7DE05563E5F6CC311B3307901797";

    //白色小相机用的
    //static readonly string defaultParam = "PE000160PIPIPIPI7124C10C7F7558295DA746C07A1854726A97FB2C53CDE12977B5D73F26F43720F4B91972C72708A0C644C7573C63F0C54B1AA4B77175817A4CBE515D0DE7830543D0CE7722F6C64DD8FAD2595FD80D1A4FA0E5430B3A30D0395F364C2D5CEB258BA935D8F9FCBA2CCDE867CF2A514368A8E5527BB13389D155EEAB01A1E7AC5D82182DA5C3C5B341A3CE92B44F819B9A13303AD7C65139D7FFE8C88A60073445EEC97CEC4B647F590DAC25ED47BF0945";

    //static readonly string defaultParam = "PE000160PIPIPIPIA5072E8226E0C9B6D67D41E85DC1375418C0495E0C26F100441E451992ECD4648B3CEAF5400E7011AABA402F08B95E9E6232470D92E759B5FB6603E59247352761552911EAF80E902BC4A5258D300895A5B2359770A997406888203FA85AE12C9ADEBF2F06FD3826DD4A88ECC261AA8B788419DF71C052730FD7D9AF6B968F70BE3D4BB05541C694D9277ECA0B320CA791BF577E122A6D8A3ABAC433E4D32A18099DE3293083ED43A4E21BBAAB0D4997";
    //string mEncryptString = "PE000100PIPIPIPI657E08409FE14D7D4E86A98C3006F6998BE0349C0EF508C5F42F21D523014CE52D445F543311E0D5C303833979FBAED791C8BDD5AF662619312BB0BC86CEF413E2A947832740EF077889136A7173E3A63C5B3830AB0E6F7355CD431AE2BA38CB59F339DDAF4176F26880366421756D0C639DF403AEA1E10D56DC1B2D137531D2";
    string mCurParam = defaultParam;

    public void ReSetUV(string text)
    {
        if (text == null)
            text = mCurParam;

        bool b = true;
        b = mPanoMeshDic[EPANOMODE.PANO11].ReSetUV(text) && b;
        b = mPanoMeshDic[EPANOMODE.PANO21].ReSetUV(text) && b;
        if (b)
        {
        
            mCurParam = text;//Save Newest Param
        }
        else
        {
            if (text == defaultParam)
            {
                Debug.LogError("Default Param Error!");
                return;
            }
            else
            {
                Debug.LogWarning("Camera Param Error! Fall Back !");
                ReSetUV(mCurParam);//Fall Back
            }
        }
    }

    /// <summary>
    /// 使用媒体文件中的拼接参数
    /// </summary>
    /// <param name="text"></param>
    public void ReSetUVMedia(string text)
    {
        bool b = false;
        b = mPanoMeshDic[EPANOMODE.PANO11].ReSetUV(text) && b;
        b = mPanoMeshDic[EPANOMODE.PANO21].ReSetUV(text) && b;
        if (b)
        {
        }
        else
        {
            Debug.LogWarning("Media Param Error! Fall Back !");
            ReSetUV(mCurParam);//Fall Back
        }
    }

    #endregion

    #endregion

    void SetPreRenderTextureSizeByShowMode(EPANOMODE showMode,int width,int height)
    {
        if(showMode == EPANOMODE.PANO11)
        {

            PanoManager.Instance.SetPreRenderTextureSize(width * 2, height);
        }
        else if (showMode == EPANOMODE.PANO21)
        {

            PanoManager.Instance.SetPreRenderTextureSize(width, height);
        }
        else if (showMode == EPANOMODE.RAW11)
        {
            PanoManager.Instance.SetPreRenderTextureSize(width, height);
        }
        else if (showMode == EPANOMODE.RAW21)
        {
            PanoManager.Instance.SetPreRenderTextureSize(width, height);
        }
        else if (showMode == EPANOMODE.ONEEYE)
        {

            PanoManager.Instance.SetPreRenderTextureSize(width * 2, height);
        }
    }

    public void SetPanoMode(EPANOMODE showMode = EPANOMODE.OLD, EPANOTEXTUREMODE texmode = EPANOTEXTUREMODE.EMP_OLD, params PanoTextureForOneDevice[] texDeviceArr) 
    {
        if (texDeviceArr.Length > 0)
        {
            mPanoTextureDeviceArr = texDeviceArr;
        }

        if (showMode != EPANOMODE.OLD)
        {
            _CurShowMode = showMode;
        }
        if (texmode != EPANOTEXTUREMODE.EMP_OLD)
        {
            _CurrentTextureMode = texmode;
        }


        ATrace.Log(string.Format("SetPanoMode: {0} : {1}", _CurShowMode, _CurrentTextureMode));

        foreach (KeyValuePair<EPANOMODE, PanoMeshBase> kv in mPanoMeshDic)
        {
            if (_CurShowMode == kv.Key)
            {
                kv.Value.gameObject.SetActive(true);
                if(mPanoTextureDeviceArr != null)
                {
                    kv.Value.SetMaterial(_CurrentTextureMode,mMediaSize,mContentSize, mPanoTextureDeviceArr);
                }
            }
            else
            {
                kv.Value.gameObject.SetActive(false);
            }
        }

        if(mPanoTextureDeviceArr != null && mPanoTextureDeviceArr.Length > 0)
        {
            PanoTextureForOneDevice dv = mPanoTextureDeviceArr[0];
            if (dv != null && dv._TextureArr.Length > 0)
            {
                Texture tex = dv._TextureArr[0];

                int width = (int)(tex.width * mContentSize.x / mMediaSize.x);
                int height = (int)(tex.height * mContentSize.y / mMediaSize.y);

                SetPreRenderTextureSizeByShowMode(_CurShowMode, width, height);
            }

        }

        PanoTextureResetMsg msg = new PanoTextureResetMsg();
        PanoTextureResetMsg.Send<PanoTextureResetMsg>(msg);
    }


    public void SetMaterialMatrix(Matrix4x4 matrix, string matrixname)
    {
        if(mPanoMeshDic.ContainsKey(_CurShowMode))
        {
            PanoMeshBase pmb = mPanoMeshDic[_CurShowMode];
            pmb.SetMaterialMatrix(matrix, matrixname);
        }

    }
    public void SetMediaContentSize(Vector2 mediaSize,Vector2 contentSize)
    {
        mMediaSize = mediaSize;
        mContentSize = contentSize;

    }
    public void FlipYCurrentRenderMeshMode(bool b)
    {
        foreach (KeyValuePair<EPANOMODE, PanoMeshBase> kv in mPanoMeshDic)
        {
            if (kv.Value != null)
            {
                kv.Value.SetYFlip(b);
            }
        }
    }
    public void FlipXCurrentRenderMeshMode(bool b)
    {
   
        foreach (KeyValuePair<EPANOMODE, PanoMeshBase> kv in mPanoMeshDic)
        {
            if (kv.Value != null)
            {
                kv.Value.SetXFlip(b);
            }
        }
    }


    public void FlipYCurrentPanoMode(bool b)
    {
        PIPanoView.Instance.FlipY(b);

    }
    public void FlipXCurrentPanoMode(bool b)
    {
        PIPanoView.Instance.FlipX(b);
    }
    public void RotateXCurrentPanoMode(float rot)
    {
        PIPanoView.Instance.SetXRotate(rot);
       
    }
    public void EnableGyroscope(bool b)
    {
        PIPanoView.Instance.EnableGyroscope(b);
    }
    public Shader GetShader(PanoManager.EPANOTEXTUREMODE texMode)
    {
        Shader shader = null;
        if (texMode == PanoManager.EPANOTEXTUREMODE.EMP_RGB)
        {
            shader = Shader.Find("Unlit/RGB");
        }
        else if (texMode == PanoManager.EPANOTEXTUREMODE.EMP_ALPHARGB)
        {
            shader = Shader.Find("Unlit/AlphaRGB");
        }
        else if (texMode == PanoManager.EPANOTEXTUREMODE.EPM_YUV)
        {

            shader = Shader.Find("Unlit/AlphaYUV");
        }
        else if (texMode == PanoManager.EPANOTEXTUREMODE.EPM_SurfaceToTexture)
        {
#if UNITY_5_6
                 shader = Shader.Find("Unlit/SurfaceToTexture");
#else
            shader = Shader.Find("Unlit/AlphaRGB");
#endif
        }

        return shader;
    }
    public Shader GetCurrentShader()
    {
        return GetShader(_CurrentTextureMode);
    }
    public PanoMeshBase GetCurrentPanoMesh()
    {
        if (mPanoMeshDic.ContainsKey(_CurShowMode))
        {
            return mPanoMeshDic[_CurShowMode];
        }
        return null;
    }
}

public class PanoTextureResetMsg : Message
{
}
public class PanoSizeResetMsg : Message
{
    public int _Width;
    public int _Height;
}
