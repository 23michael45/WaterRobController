using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryController : MonoBehaviour {

    public Transform _Container;
    public RawImage _PhotoImage;

    public GameObject _ItemPrefab;

    private void Start()
    {
        CreateItems();

        string url = Application.streamingAssetsPath + "/Photo/1.jpg";
        StartCoroutine(IEOpenPhoto(url));
    }


    void CreateItem(int index)
    {
        GameObject gonew = GameObject.Instantiate(_ItemPrefab);
        gonew.transform.parent = _Container;
        gonew.transform.localScale = Vector3.one;
        PhotoItem item = gonew.GetComponent<PhotoItem>();
        item.LoadPhoto(this,Application.streamingAssetsPath + "/Photo/" + (index % 10 + 1).ToString() + ".jpg");
        
    }

    public void SetPhoto(string url)
    {

        StartCoroutine(IEOpenPhoto(url));
    }

    void CreateItems()
    {
        for(int i = 0;i< 20;i++)
        {
            CreateItem(i);
        }
    }

    IEnumerator IEOpenPhoto(string url)
    {

        string wwwurl = "file:///" + url;
        WWW www = new WWW(wwwurl);
        while (www != null && !www.isDone)
        {
            yield return 0;
        }
        if (www == null)
        {
            Debug.LogWarning("Photo Load WWW is Release!");
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
    }

}
