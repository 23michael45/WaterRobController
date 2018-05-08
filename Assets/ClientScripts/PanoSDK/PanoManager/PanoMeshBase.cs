using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class PanoMeshBase : MonoBehaviour
{

    //Vector2 _TextureFlipScale = Vector2.one;


    public virtual PanoManager.EPANOMODE _PanoShowMode
    {
        get { return PanoManager.EPANOMODE.PANO21; }
    }

    protected Renderer[] _Renderers;

    protected virtual void Awake()
    {
        _Renderers = gameObject.GetComponentsInChildren<Renderer>(true);
    }
    
  



    public bool ReSetUV(string text)
    {
        bool ret = true;
        int index = 0;
        foreach(Renderer r in _Renderers)
        {
            MeshFilter mf = r.gameObject.GetComponent<MeshFilter>();
            ret = GetComponent<MeshUVHelper>().SetMeshUV(index, text, ref mf) && ret;
            index++;
        }
        return ret;
    }


    public virtual void SetMaterial(PanoManager.EPANOTEXTUREMODE texMode, Vector2 mediaSize,Vector2 contentSize,params PanoManager.PanoTextureForOneDevice[] texDeviceArr)
    {
       
        //SetFlipScale();
    }


    protected void SetOneMaterial(Material mat, int index, int sum, Rect rc, PanoManager.EPANOTEXTUREMODE texMode, PanoManager.PanoTextureForOneDevice texDevice)
    {
        ATrace.Log(string.Format("SetOneMaterial: index:{0} sum:{1} rc:{2}", index, sum, rc.ToString()));
        PanoManager.PanoTextureForOneDevice dv = texDevice;

        if (texMode == PanoManager.EPANOTEXTUREMODE.EPM_YUV)
        {
            if (dv._TextureArr.Length > 0)
            {
                SetOneMaterialOneParam(mat, index, sum, rc, dv._TextureArr[0], "_MainTex");
            }
            if (dv._TextureArr.Length > 1)
            {
                SetOneMaterialOneParam(mat, index, sum, rc, dv._TextureArr[1], "_MainTexU");

            }
            if (dv._TextureArr.Length > 2)
            {
                SetOneMaterialOneParam(mat, index, sum, rc, dv._TextureArr[2], "_MainTexV");

            }
        }
        else if (texMode == PanoManager.EPANOTEXTUREMODE.EMP_RGB)
        {
            if (dv._TextureArr.Length > 0)
            {
                SetOneMaterialOneParam(mat, index, sum, rc, dv._TextureArr[0]);
            }
        }
        else if (texMode == PanoManager.EPANOTEXTUREMODE.EPM_SurfaceToTexture)
        {
            if (dv._TextureArr.Length > 0)
            {
                SetOneMaterialOneParam(mat, index, sum, rc, dv._TextureArr[0]);
            }
        }


    }

    protected void SetOneMaterialOneParam(Material mat,int index,int sum ,Rect rc,Texture tex = null, string texname = "_MainTex")
    {
        ATrace.Log("SetOneMaterialOneParam:" + mat);
        if(mat && mat.HasProperty(texname))
        {
            
            ATrace.Log("SetOneMaterialOneParam Texture Scale Offset:" + mat + " index:" + index + "sum:" + sum);
            mat.SetTexture(texname, tex);
            mat.SetTextureScale(texname, GetTexScale(index,sum, rc));
            mat.SetTextureOffset(texname, GetTexOffset(index, sum, rc));
        }

    }



    protected virtual Vector2 GetTexScale(int index,int sum,Rect rc)
    {
        float offsetX = index * (1f / sum) + rc.x / sum;
        return new Vector2(rc.width / sum, rc.height);

    }

    protected virtual Vector2 GetTexOffset(int index, int sum, Rect rc)
    {
        float offsetX = index * (1f / sum) + rc.x / sum;
        return new Vector2(offsetX, rc.y);
    }


    protected void SetFlipScale()
    {
        //foreach (Renderer r in _Renderers)
        //{
        //    Material mat = r.sharedMaterial;
        //    if (mat && mat.HasProperty("_MainTex"))
        //    {
        //        mat.SetTextureScale("_MainTex", _TextureFlipScale);

        //        if (mat.GetTexture("_MainTex") != null)
        //        {
        //            mat.GetTexture("_MainTex").wrapMode = TextureWrapMode.Repeat;
        //        }
        //    }
        //}
    }

    public virtual void SetXFlip(bool b)
    {
    }

    public virtual void SetYFlip(bool b)
    {
      
    }
    
    public virtual void SetMaterialMatrix(Matrix4x4 matrix, string matrixname)
    {
        foreach (Renderer r in _Renderers)
        {
            Material mat = r.sharedMaterial;
            if (mat != null && mat.HasProperty(matrixname))
            {
                mat.SetMatrix(matrixname, matrix);
            }
        }

    }
    protected Rect GetContentRect(Vector2 imageSize,Vector2 contentSize)
    {
        float offsetx = (imageSize.x - contentSize.x) / 2 / imageSize.x;
        float tilingx = contentSize.x / imageSize.x;

        float offsety = (imageSize.y - contentSize.y) / 2 / imageSize.y;
        float tilingy = contentSize.y / imageSize.y;

        Rect rc = new Rect(offsetx, offsety, tilingx, tilingy);
        return rc;
    }

    public virtual Shader GetCurrentShader()
    {
        foreach (Renderer r in _Renderers)
        {
            Material mat = r.sharedMaterial;
            if(mat != null)
            {
                return mat.shader;
            }
        }
        return null;
    }

    public virtual Material GetCurrentMaterial()
    {
        foreach (Renderer r in _Renderers)
        {
            Material mat = r.sharedMaterial;

            return mat;

        }
        return null;
    }

    public void Log()
    {
        foreach (Renderer r in _Renderers)
        {
            Material mat = r.sharedMaterial;

            ATrace.Log(string.Format("Current Mat:{0} Shader: {0}", mat, ToString(), mat.shader.ToString()));

        }
    }
}
