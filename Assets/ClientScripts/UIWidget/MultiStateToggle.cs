using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Toggle))]
public class MultiStateToggle : MonoBehaviour
{

    private Toggle _Toggle;
    public List<Sprite> _StateSprites;
    public Sprite _DiableSprite;
    public int _DefaultState = -1;
    public int _CurrentState = 0;


    public ToggleStateEvent onValueChanged = new ToggleStateEvent();

    void Awake()
    {
        //if (onValueChanged == null)
        //    onValueChanged = new ToggleStateEvent();


        _Toggle = GetComponent<Toggle>();
        _Toggle.onValueChanged.AddListener(OnToggle);

        SetState(_DefaultState);

    }
    private void OnDestroy()
    {
        _Toggle.onValueChanged.RemoveAllListeners();

    }

    public void OnToggle(bool isOn)
    {
        if (isOn)
        {
            _CurrentState = ++_CurrentState % (_StateSprites.Count);
            _Toggle.image.sprite = _StateSprites[_CurrentState];

            if (onValueChanged != null)
            {
                onValueChanged.Invoke(_CurrentState);
            }
        }
        else
        {
            _Toggle.image.sprite = _DiableSprite;
            if (onValueChanged != null)
            {
                onValueChanged.Invoke(-1);
            }

        }
    }


    public void SetState(int nstate)
    {
        if (nstate < 0)
        {

            _Toggle.isOn = false;
            _CurrentState = nstate;
            _Toggle.image.sprite = _DiableSprite;
        }
        else
        {
            _Toggle.isOn = true;
            _CurrentState = nstate;

            int index = (_CurrentState % _StateSprites.Count);
            _Toggle.image.sprite = _StateSprites[index];

        }
    }
}

public class ToggleStateEvent : UnityEvent<int>
{
}
