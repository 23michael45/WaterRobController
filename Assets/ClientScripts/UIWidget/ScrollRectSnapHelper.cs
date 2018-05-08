using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectSnapHelper : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{

    public int NumberOfScreens = 3;
    [Tooltip("How fast does it tween to next page when Flick gesture is detected")]
    public float ThrowSpeed = 5;
    [Tooltip("Percent scroll needs to be moved to detect Flick gesture")]
    public float FlickPercentThreshold = .05f;
    [Tooltip("Maximum time length from drag begin to drag end to clasify as a Flick gesture")]
    public float FlickTimeThreshold = .2f;

    private float screenStep, desiredScreenPos, dragStartPos, flickStartTimeStamp;
    private bool canAnimate = false;
    private ScrollRect scrollRect;

    // Use this for initialization
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        screenStep = 1.0f / (NumberOfScreens - 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (canAnimate)
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, desiredScreenPos, Time.deltaTime * ThrowSpeed);
    }

    public void OnBeginDrag(PointerEventData data)
    {
        canAnimate = false;
        dragStartPos = scrollRect.horizontalNormalizedPosition;
        flickStartTimeStamp = Time.time;
    }

    public void OnEndDrag(PointerEventData data)
    {
        desiredScreenPos = Mathf.Round(scrollRect.horizontalNormalizedPosition / screenStep) * screenStep;

        if (Time.time - flickStartTimeStamp < FlickTimeThreshold &&
            Mathf.Abs(scrollRect.horizontalNormalizedPosition - dragStartPos) > FlickPercentThreshold)
        {
            desiredScreenPos = Mathf.Clamp01(desiredScreenPos + screenStep * Mathf.Sign(scrollRect.horizontalNormalizedPosition - dragStartPos));
        }

        canAnimate = true;
    }
}