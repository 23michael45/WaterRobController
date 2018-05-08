using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ScrollRectEx : ScrollRect
{

    private bool routeToParent = false;
    ScrollSnap snap;


    public class ScrollEndEvent : UnityEvent
    {
    }

    public ScrollEndEvent onScrollTop = new ScrollEndEvent();
    public ScrollEndEvent onScrollBottom = new ScrollEndEvent();
    public ScrollEndEvent onScrollLeft = new ScrollEndEvent();
    public ScrollEndEvent onScrollRight = new ScrollEndEvent();

    public ScrollEndEvent onScrollStart = new ScrollEndEvent();
    public ScrollEndEvent onScrollEnd = new ScrollEndEvent();

    Vector2 _DragDelta;

    protected override void Awake()
    {
        base.Awake();
        snap = gameObject.GetComponent<ScrollSnap>();
    }

    /// <summary>
    /// Do action for all parents
    /// </summary>
    private void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            foreach (var component in parent.GetComponents<Component>())
            {
                if (component is T)
                    action((T)(IEventSystemHandler)component);
            }
            parent = parent.parent;
        }
    }

    /// <summary>
    /// Always route initialize potential drag event to parents
    /// </summary>
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
        base.OnInitializePotentialDrag(eventData);
    }

    /// <summary>
    /// Drag event
    /// </summary>
    public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {

        _DragDelta += eventData.delta;
        if (routeToParent)
            DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
        else
            base.OnDrag(eventData);


        snap.OnDrag(_DragDelta);

       
    }

    /// <summary>
    /// Begin drag event
    /// </summary>
    public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        _DragDelta = Vector2.zero;

        if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
            routeToParent = true;
        else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
            routeToParent = true;
        else
            routeToParent = false;

        if (routeToParent)
            DoForParents<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
        else
            base.OnBeginDrag(eventData);

        if (onScrollStart != null)
        {

            onScrollStart.Invoke();
        }
    }

    /// <summary>
    /// End drag event
    /// </summary>
    public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (routeToParent)
            DoForParents<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
        else
            base.OnEndDrag(eventData);
        routeToParent = false;


        snap.DragEnd(_DragDelta);

        if (onScrollEnd != null)
        {

            onScrollEnd.Invoke();
        }
    }


    public virtual void Update()
    {
        if(normalizedPosition.y > 1.0f)
        {
            if(onScrollTop != null)
            {
                onScrollTop.Invoke();

            }
        }


        if (normalizedPosition.y < 0.0f)
        {
            if (onScrollBottom != null)
            {
                onScrollBottom.Invoke();

            }
        }

        if (normalizedPosition.x > 1.0f)
        {
            if (onScrollLeft != null)
            {
                onScrollLeft.Invoke();

            }
        }

        if (normalizedPosition.x < 0.0f)
        {
            if (onScrollRight != null)
            {
                onScrollRight.Invoke();

            }
        }
    }
}