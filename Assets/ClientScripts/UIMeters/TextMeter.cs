using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMeter : MeterBase<object[]>
{
    public Text[] _TextArr;

    int _ControlLength;

    protected override void Start()
    {
        base.Start();

        _ControlLength = _TextArr.Length;
    }

    protected override void UpdateValue()
    {
        if(_CurrentValue == null)
        {
            return;
        }
        
        for(int i = 0; i< _ControlLength;i++)
        {
            if(_CurrentValue.Length > i)
            {
                _TextArr[i].text = _CurrentValue[i].ToString();

            }
        }
    }

    public void SetCurrentOneValue<T>(T s)
    {
        if(_CurrentValue == null)
        {
            _CurrentValue = new string[1];
        }
        _CurrentValue[0] = _PreStuff + s.ToString() + _PostStuff;
    }

}
