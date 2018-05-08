using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollPlaneMeshController : PanoControllerBase
{

    public float _DragSensitive = .2f;   // Drag Add Value速率
    public float _ForceSensitive = 3;   // Drag Add Value速率
    public float _MinSpeed = 6f;
    public Camera _PlaneCamera;
    Transform _Clone;

    public float _MeshWidthHalf = 200f;

    float _CameraWidth;

    private void Awake()
    {
        _Clone = gameObject.GetComponentsInChildren<Transform>()[1];
    }
    private void Start()
    {
        float width = _PlaneCamera.rect.width;
        float height = _PlaneCamera.rect.height;
        _CameraWidth = _PlaneCamera.orthographicSize / (Screen.height * height) * (Screen.width * width);
    }

    void Update()
    {
        FixPosition();
        FixSpeed();
    }



    public override void OnDrag(DragGesture gesture)
    {
        base.OnDrag(gesture);
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            transform.Translate(new Vector3(gesture.LastDelta.x * _DragSensitive, 0,0));

        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            AddForceX(gesture.TotalMove.x / gesture.ElapsedTime);
        }
    }
   
    
    public override void OnSimpleFingerDown(object v)
    {
        StopMove();
    }
    public override void OnSimpleFingerUp(object v)
    {
      
    }

    public void AddForceX(float v)
    {
        
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right * v * _ForceSensitive);
    }

    public bool CheckRaycast(Vector2 pos)
    {
        Ray ray = _PlaneCamera.ScreenPointToRay(pos);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if(hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }
        return false;
    }
    void StopMove()
    {

        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    void FixSpeed()
    {
        float x = gameObject.GetComponent<Rigidbody>().velocity.x;
        if ( x != 0 && Mathf.Abs(x) < _MinSpeed)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(_MinSpeed * x / Mathf.Abs(x), 0, 0);
        }
    }
    void FixPosition()
    {
        var v = transform.localPosition;
        var s = transform.localScale;
        //v.y = 0;


        var max = _MeshWidthHalf / 2 - _CameraWidth;
        max = Mathf.Max(max, 0);
        if (v.x > max)
        {
            v.x = v.x % _MeshWidthHalf;


            _Clone.localPosition = new Vector3(-_MeshWidthHalf / s.x, _Clone.localPosition.y, _Clone.localPosition.z);

        }
        else if (v.x < -max)
        {
            v.x = -(-v.x % _MeshWidthHalf);

            _Clone.localPosition = new Vector3(_MeshWidthHalf / s.x, _Clone.localPosition.y, _Clone.localPosition.z);
        }


        transform.localPosition = v;
    }
}