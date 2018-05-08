using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class SwitchMeter : MeterBase<bool> {

    public Image _SwitchOn;
    public Image _SwitchOff;
    // Use this for initialization

    public override void SetCurrentValue(bool b)
    {
        if (_SwitchOn)
        {

            _SwitchOn.gameObject.SetActive(b);
        }
        if (_SwitchOff)
        {

            _SwitchOff.gameObject.SetActive(!b);
        }
    }
}
