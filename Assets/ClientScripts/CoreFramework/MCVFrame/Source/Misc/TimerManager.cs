using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class TimerManager : MonoSingleton<TimerManager> {

    public delegate void TimeoutEventDelegate(params object[] arr);

    public class Timer
    {
        CrudeElapsedTimer _TimerEntity;
        TimeoutEventDelegate _Callback;
        object[] _Params;


        public Timer(float timesec, TimeoutEventDelegate cb, params object[] arr)
        {
            _Params = arr;
            _Callback = cb;
            _TimerEntity = new CrudeElapsedTimer(timesec);
        }

        public bool Update()
        {
            if (_TimerEntity != null)
            {
                _TimerEntity.Advance(Time.deltaTime);

                if (_TimerEntity.TimeOutCount > 0)
                {
                    if (_Callback != null)
                    {
                        _Callback(_Params);
                    }
                    return true;
                }
            }
            return false;
        }
    }


    Dictionary<Type, Timer> _CallbackDic = new Dictionary<Type, Timer>();
    public TimerUpdateEvent _TimerUpdateEvent = new TimerUpdateEvent();

    List<Type> _WillRemove = new List<Type>();
    // Update is called once per frame
    void Update() {
        ClearWillRemove();

        foreach (KeyValuePair<Type, Timer> kv in _CallbackDic)
        {
            bool b = kv.Value.Update();
            if (b)
            {
                _WillRemove.Add(kv.Key);
            }
        }
        ClearWillRemove();

        if (_TimerUpdateEvent != null)
        {
            _TimerUpdateEvent.Invoke();
        }
    }

    void ClearWillRemove()
    {
        foreach (Type t in _WillRemove)
        {
            if (_CallbackDic.ContainsKey(t))
            {
                _CallbackDic.Remove(t);
            }
        }
        _WillRemove.Clear();
    }


    public void AddTimer<T>(float timesec, TimeoutEventDelegate cb, params object[] arr)
    {
        StartCoroutine(AddTimerCoroutine<T>(timesec, cb, arr));
    }
    IEnumerator AddTimerCoroutine<T>(float timesec, TimeoutEventDelegate cb, params object[] arr)
    {

        yield return 0;
        if (!_CallbackDic.ContainsKey(typeof(T)))
        {
            _CallbackDic.Add(typeof(T), new Timer(timesec, cb, arr));

        }
    }

    public void CancelTimer<T>()
    {
        StartCoroutine(CancelTimerCoroutine<T>());
    }
    IEnumerator CancelTimerCoroutine<T>()
    {
        yield return 0;

        if (!_WillRemove.Contains(typeof(T)))
        {
            _WillRemove.Add(typeof(T));
        }
    }


    #region calculate elapsed time tool

    private TimeSpan startTime;
    public void StartTime()
    {
        startTime = DateTime.Now.TimeOfDay;
    }

    /// <summary>
    /// 开始时间
    /// </summary>
    /// <returns>开始时间</returns>
    public TimeSpan StartTimeFlag()
    {
        TimeSpan stf = DateTime.Now.TimeOfDay;
        return stf;
    }

    /// <summary>
    /// 输出经过时间
    /// </summary>
    /// <param name="st">相对应的开始时间</param>
    /// <param name="timeFlag">需要计算的时间标志</param>
    public void LogElapsedTimeFlag(TimeSpan st,string timeFlag = "")
    {
        string te = (DateTime.Now.TimeOfDay - st).ToString();
        Debug.LogFormat("{0} elapse: {1}", timeFlag, te);
    }

    public TimeSpan EndTime(string entFlag = "")
    {
        TimeSpan endTime = DateTime.Now.TimeOfDay;
        return endTime;        
    }
    
    public void LogTimeElapsed(string tag = "")
    {
        string timeElapsed = (DateTime.Now.TimeOfDay - startTime).ToString();
        Debug.LogFormat("{0} !!!!!elapse: {1}", tag, timeElapsed);
    }
    
    #endregion


    #region Call Example

    //void OnTimeout(params object[] arr)
    //{

    //    //do something
    //}
    //TimerManager.Instance.AddTimer<T>(5.0f, OnTimeout);
    //TimerManager.Instance.CancelTimer<T>();


    #endregion
}
public class TimerUpdateEvent : UnityEvent
{
}