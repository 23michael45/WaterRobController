using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoItem : MonoBehaviour {

    static bool _Loading = false;

    GalleryController _ParentController;
    string _PhotoUrl;
    RawImage _PhotoImage;
    Button _Btn;

    private void Awake()
    {
        _PhotoImage = gameObject.GetComponentInChildren<RawImage>();
        _Btn = gameObject.GetComponent<Button>();
        _Btn.onClick.AddListener(OnClick);
    }
    private void OnDestroy()
    {
        _Btn.onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        _ParentController.SetPhoto(_PhotoUrl);
    }

    public void LoadPhoto(GalleryController parentController, string url)
    {
        _ParentController = parentController;
        _PhotoUrl = url;
        StartCoroutine(IEOpenPhoto(_PhotoUrl));
    }

    IEnumerator IEOpenPhoto(string url)
    {
        while(_Loading)
        {

            yield return 0;
        }

        _Loading = true;

        string wwwurl = "file:///" + url;
        WWW www = new WWW(wwwurl);
        while (www != null && !www.isDone)
        {
            yield return 0;
        }
        if (www == null)
        {
            Debug.LogWarning("Photo Load WWW is Release!");


            _Loading = false;
            yield break;
        }

        if (string.IsNullOrEmpty(www.error))
        {
            Texture2D tex = new Texture2D(4, 4, TextureFormat.RGB24, false);
            www.LoadImageIntoTexture(tex);
            if (tex.width > SystemInfo.maxTextureSize)
            {
                tex = new Texture2D(4, 4, TextureFormat.RGB24, true);
                www.LoadImageIntoTexture(tex);
            }

            _PhotoImage.texture = tex;
        }
        else
        {
            Debug.LogWarning("Photo Load WWW is Failed:" + url);
        }


        _Loading = false;
    }

}
