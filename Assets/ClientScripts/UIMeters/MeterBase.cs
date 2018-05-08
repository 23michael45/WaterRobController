using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterBase<T> : MonoBehaviour {

    public T _CurrentValue;
    public Text[] _TextControlArr;

    public string _PreStuff;
    public string _PostStuff;
    // Use this for initialization
    protected virtual void Start () {
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {

        UpdateValue();

        UpdateTextControl();
    }
    protected virtual void UpdateValue()
    {
    }

    public virtual void SetCurrentValue(T v)
    {
        _CurrentValue = v;
    }

    public virtual void UpdateTextControl()
    {
        if(_TextControlArr != null &&_TextControlArr.Length > 0)
        {
            if (_TextControlArr[0].text != _CurrentValue.ToString())
            {
                _TextControlArr[0].text = _PreStuff + _CurrentValue.ToString() + _PostStuff;
            }

        }
    }
}
