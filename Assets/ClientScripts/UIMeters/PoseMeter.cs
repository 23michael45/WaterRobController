using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseMeter : MeterBase<Vector3> {


    public OneAxisOffsetMeter _PicthMeter;
    public RotationAngleMeter _RollMeter;
    public RotationAngleMeter _YawMeter;


    public override void SetCurrentValue(Vector3 v)
    {
        _CurrentValue = v;
        _PicthMeter.SetCurrentValue(_CurrentValue.x);
        _RollMeter.SetCurrentValue(_CurrentValue.z);
        _YawMeter.SetCurrentValue(_CurrentValue.y);
    }

    public override void UpdateTextControl()
    {
        if (_TextControlArr != null && _TextControlArr.Length >= 3)
        {
            string s = ((int)_CurrentValue.x).ToString();
            if (_TextControlArr[0].text != s)
            {
                _TextControlArr[0].text = s;
            }
            s = ((int)_CurrentValue.y).ToString();
            if (_TextControlArr[1].text != s)
            {
                _TextControlArr[1].text = s;
            }

            s = ((int)_CurrentValue.z).ToString();
            if (_TextControlArr[2].text !=s )
            {
                _TextControlArr[2].text = s;
            }
        }
    }
}
