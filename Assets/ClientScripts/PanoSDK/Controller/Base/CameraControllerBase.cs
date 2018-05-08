using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

 public class CameraControllerBase : PanoControllerBase
{
    [HideInInspector]
    public Camera _ControlCamera;
    

    protected virtual void Awake()
    {
        _ControlCamera = gameObject.GetComponent<Camera>();     
    }

    protected virtual void Start()
    {
    }

    public virtual bool CheckInCameraScreenArea(Vector2 pos)
    {
        Rect viewrect = _ControlCamera.rect;
        Vector2 normalpos = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
        if(viewrect.Contains(normalpos))
        {
            return true;
        }
        return false;
    }



    //从FOV得到宽度上的视角大小
    protected float GetVAngByFov()
    {
        float tanxd2 = Mathf.Tan(_ControlCamera.fieldOfView * Mathf.Deg2Rad / 2) * _ControlCamera.aspect;
        float ang = Mathf.Atan(tanxd2);

        //ang * 2 / 2 所以不乘不除即可 
        return ang * Mathf.Rad2Deg;
    }
  

}