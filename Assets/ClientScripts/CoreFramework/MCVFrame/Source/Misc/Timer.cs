using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>计时器核心类</summary>
public class Timer
{
    #region Fields

    /// <summary>是否在计时中</summary>
    private bool _isTicking;

    /// <summary>当前时间</summary>
    private float _currentTime;

    /// <summary>结束时间</summary>
    private float _endTime;

    #endregion

    #region 委托事件

    public delegate void TimerEventHandler(params object[] paramarr);
    public event TimerEventHandler tickEvent;

    #endregion

    #region Constructed Function

    public Timer()
    {

    }

    public Timer(float second)
    {
        _currentTime = 0;
        _endTime = second;
    }

    #endregion

    #region Method

    /// <summary>开始倒计时</summary>
    public void StartTimer()
    {
        _isTicking = true;
    }

    /// <summary>
    /// 更新中
    /// </summary>
    /// <param name="deltaTime">更新的数值</param>
    public void UpdateTimer(float deltaTime)
    {
        if (_isTicking)
        {
            _currentTime += deltaTime;
            if (_currentTime > _endTime)
            {
                _isTicking = false;
                //tickEvent();
                OnTickEvent();
            }
        }
    }

    /// <summary>停止倒计时</summary>
    public void StopTimer()
    {
        _isTicking = false;
    }

    /// <summary>持续倒计时</summary>
    public void ContinueTimer()
    {
        _isTicking = true;
    }

    /// <summary>重新倒计时</summary>
    public void RestartTimer()
    {
        _isTicking = true;
        _currentTime = 0;
    }

    /// <summary>
    /// 重新倒计时
    /// </summary>
    /// <param name="second"></param>
    public void ResetEndTimer(float second)
    {
        _endTime = second;
    }

    #endregion

    #region 调用事件

    /// <summary>
    /// 调用无返有参事件
    /// </summary>
    public void OnTickEvent()
    {
        if (tickEvent != null)
        {
            Debug.Log("CallBack tickEvent（）");
            tickEvent();
        }
    }

    #endregion
}
