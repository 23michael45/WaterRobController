using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoRaw21Mesh : PanoMeshBase
{

    public override PanoManager.EPANOMODE _PanoShowMode
    {
        get { return PanoManager.EPANOMODE.RAW21; }
    }


    public override void SetMaterial(PanoManager.EPANOTEXTUREMODE texMode, Vector2 mediaSize, Vector2 contentSize, params PanoManager.PanoTextureForOneDevice[] texDeviceArr)
    {
        int i = 0;
        foreach (Renderer r in _Renderers)
        {
            Material mat = new Material(PanoManager.Instance.GetShader(texMode));
            r.material = mat;

            if (texDeviceArr.Length > i)
            {
                //两个镜头的两张贴图，所以TexArr取不同index
                SetOneMaterial(mat, 0, 1, GetContentRect(mediaSize, contentSize), texMode, texDeviceArr[0]);

            }

            i++;
        }


        base.SetMaterial(texMode, mediaSize, contentSize, texDeviceArr);
    }

    public override void SetXFlip(bool b)
    {
        if (b)
        {
            foreach (Renderer r in _Renderers)
            {
                r.transform.localScale = new Vector3(-1, r.transform.localScale.y, 1);
                r.transform.localPosition = new Vector3(-100, r.transform.localPosition.y, 0);
            }
        }
        else
        {
            foreach (Renderer r in _Renderers)
            {
                r.transform.localScale = new Vector3(1, r.transform.localScale.y, 1);
                r.transform.localPosition = new Vector3(0, r.transform.localPosition.y, 0);
            }


        }
    }

    public override void SetYFlip(bool b)
    {
        if (b)
        {
            foreach (Renderer r in _Renderers)
            {
                r.transform.localScale = new Vector3(r.transform.localScale.x, -1, 1);
                r.transform.localPosition = new Vector3(r.transform.localPosition.x, 50, 0);
            }
        }
        else
        {
            foreach (Renderer r in _Renderers)
            {
                r.transform.localScale = new Vector3(r.transform.localScale.x, 1, 1);
                r.transform.localPosition = new Vector3(r.transform.localPosition.x, 0, 0);
            }


        }
    }
}
