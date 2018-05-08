using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Toggle))]
public class UISwitchSpriteToggle : MonoBehaviour
{

    private Toggle _Toggle;

    public Sprite spriteOn;
    public Sprite spriteOff;

    void Awake()
    {
        _Toggle = GetComponent<Toggle>();
        _Toggle.onValueChanged.AddListener(OnToggle);

        SetState(_Toggle.isOn);//初始化UI
    }
    private void OnDestroy()
    {
        _Toggle.onValueChanged.RemoveAllListeners();

    }

    public void OnToggle(bool isOn)
    {
        SetSprite(isOn);
    }

    public void SetSprite(bool isOn)
    {
        if (isOn)
        {
            _Toggle.targetGraphic.GetComponent<Image>().sprite = spriteOn;
        }
        else
        {
            _Toggle.targetGraphic.GetComponent<Image>().sprite = spriteOff;
        }
    }

    public void SetState(bool isOn)
    {
        _Toggle.isOn = isOn;
        SetSprite(isOn);
    }
}
