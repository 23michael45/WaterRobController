using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollSnap : MonoBehaviour
{

    float[] points;
    [Tooltip("how many screens or pages are there within the content (steps)")]
    public int screens = 1;
    float stepSize;

    ScrollRect scroll;
    bool LerpH;
    float targetH;
    [Tooltip("Snap horizontally")]
    public bool snapInH = true;

    bool LerpV;
    float targetV;
    [Tooltip("Snap vertically")]
    public bool snapInV = true;

    public float speed = 100f;

    public Toggle[] _PageToggles;
    public Button _NextBtn;
    public Button _PreBtn;

    // Use this for initialization
    void Start()
    {
        scroll = gameObject.GetComponent<ScrollRect>();
        scroll.inertia = true;

        if(_NextBtn)
        {

            _NextBtn.onClick.AddListener(OnNextPage);
        }
        if(_PreBtn)
        {
            _PreBtn.onClick.AddListener(OnPrePage);

        }

        if (screens > 0)
        {
            points = new float[screens];
            stepSize = 1 / (float)(screens - 1);

            for (int i = 0; i < screens; i++)
            {
                points[i] = i * stepSize;
            }
        }
        else
        {
            points[0] = 0;
        }
    }
    private void OnDestroy()
    {
        if (_NextBtn)
        {
            _NextBtn.onClick.RemoveListener(OnNextPage);
        }

        if (_PreBtn)
        {
            _PreBtn.onClick.RemoveListener(OnPrePage);
        }

    }

    void Update()
    {
        if (LerpH)
        {
            scroll.horizontalNormalizedPosition = Mathf.Lerp(scroll.horizontalNormalizedPosition, targetH, speed * scroll.elasticity * Time.deltaTime);
            if (Mathf.Approximately(scroll.horizontalNormalizedPosition, targetH)) LerpH = false;
        }
        if (LerpV)
        {
            scroll.verticalNormalizedPosition = Mathf.Lerp(scroll.verticalNormalizedPosition, targetV, speed * scroll.elasticity * Time.deltaTime);
            if (Mathf.Approximately(scroll.verticalNormalizedPosition, targetV)) LerpV = false;
        }
    }

    public void DragEnd(Vector2 delta)
    {
        if (scroll.horizontal && snapInH)
        {
            int nearpoint = FindNearest(scroll.horizontalNormalizedPosition, points);


            if(delta.x > 0)
            {
                nearpoint = FindLeft(scroll.horizontalNormalizedPosition, points);
                targetH = points[nearpoint];
            }
            else if (delta.x < 0)
            {

                nearpoint = FindRight(scroll.horizontalNormalizedPosition, points);
                targetH = points[nearpoint];
            }


            SetPageToggle(nearpoint);
            LerpH = true;
        }
        if (scroll.vertical && snapInV)
        {
            targetH = points[FindNearest(scroll.verticalNormalizedPosition, points)];
            LerpH = true;
        }
    }

    public void OnDrag(Vector2 delta)
    {
        LerpH = false;
        LerpV = false;
    }
    int FindLeft( float f, float[] array)
    {
        for (int index = 0; index < array.Length - 1; index++)
        {
            if (array[index] <= f && f < array[index + 1])
            {
                return index;

            }
        }


        return 0;

    }
    int FindRight(float f, float[] array)
    {
        for (int index = 0; index < array.Length - 1; index++)
        {
            if (array[index] <= f && f < array[index + 1])
            {

                return index + 1;
            }
        }
        return array.Length - 1;
    }

    int FindNearest(float f, float[] array)
    {
        float distance = Mathf.Infinity;
        int output = 0;
        for (int index = 0; index < array.Length; index++)
        {
            if (Mathf.Abs(array[index] - f) < distance)
            {
                distance = Mathf.Abs(array[index] - f);
                output = index;
            }
        }
        return output;
    }

    void SetPageToggle(int page)
    {
        for(int i = 0; i< _PageToggles.Length;i++)
        {
            if(i == page)
            {
                _PageToggles[i].isOn = true;
            }
            else
            {
                _PageToggles[i].isOn = false;
            }
        }
    }

    void OnNextPage()
    {
        NextPage();
    }
    void OnPrePage()
    {
        PrePage();
    }

    public void NextPage()
    {
        scroll.horizontalNormalizedPosition -= 0.1f;
        DragEnd(new Vector2(1, 0));
    }
    public void PrePage()
    {

        scroll.horizontalNormalizedPosition += 0.1f;
        DragEnd(new Vector2(-1, 0));
    }
}