using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavRTCameraController : NavCameraController
{
    public Image _BlackBG;
    public RawImage _RawImage;
    CanvasScaler _Scaler;

    RenderTexture _Rendertexture;

    protected override void Awake()
    {
        base.Awake();

        _Scaler = _RawImage.gameObject.GetComponentInParent<CanvasScaler>();
        

        float len = (float)_Scaler.referenceResolution.y / 3;

        _BlackBG.rectTransform.sizeDelta = new Vector2(len, len);
        _BlackBG.rectTransform.anchorMax = new Vector2(1, 0);
        _BlackBG.rectTransform.anchorMin = new Vector2(1, 0);
        _BlackBG.rectTransform.offsetMax = new Vector2(0, len);
        _BlackBG.rectTransform.offsetMin = new Vector2(-len, 0);

        _Rendertexture = new RenderTexture((int)len, (int)len, 0);
        _ControlCamera.targetTexture = _Rendertexture;
        _RawImage.texture = _Rendertexture;
    }
    protected override void Start()
    {
        _Camera.rect = new Rect(0, 0, 1, 1);
    }

}