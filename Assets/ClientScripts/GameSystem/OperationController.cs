using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationController : MonoBehaviour
{
    [HideInInspector]
    public MainController _ParentController;
    #region Buttons

    public Toggle _ShowHideToggle;
    public Toggle _LightToggle;
    public Toggle _CaputreModeToggle;
    public Toggle _WifiToggle;
    public Button _Capture;
    public Button _StartRecord;
    public Button _StopRecord;
    #endregion


    void OnEnable()
    {


        _ShowHideToggle.onValueChanged.AddListener(OnShowHide);
        _LightToggle.onValueChanged.AddListener(OnLight);
        _CaputreModeToggle.onValueChanged.AddListener(OnCaptureMode);
        _WifiToggle.onValueChanged.AddListener(OnWifi);

        _Capture.onClick.AddListener(OnCapture);
        _StartRecord.onClick.AddListener(OnStartRecord);
        _StopRecord.onClick.AddListener(OnStopRecord);
    }
    void OnDisable()
    {
        _ShowHideToggle.onValueChanged.RemoveListener(OnShowHide);
        _LightToggle.onValueChanged.RemoveListener(OnLight);
        _CaputreModeToggle.onValueChanged.RemoveListener(OnCaptureMode);
        _WifiToggle.onValueChanged.RemoveListener(OnWifi);


        _Capture.onClick.RemoveListener(OnCapture);
        _StartRecord.onClick.RemoveListener(OnStartRecord);
        _StopRecord.onClick.RemoveListener(OnStopRecord);
    }


    void OnShowHide(bool b)
    {
        _ParentController.ShowHideMeters(b);
    }
    void OnWifi(bool b)
    {

    }
    void OnLight(bool b)
    {

    }
    void OnCaptureMode(bool b)
    {
        _Capture.gameObject.SetActive(b);
        _StartRecord.gameObject.SetActive(!b);
        _StopRecord.gameObject.SetActive(false);
    }

    void OnCapture()
    {

    }
    void OnStartRecord()
    {
        _StartRecord.gameObject.SetActive(false);
        _StopRecord.gameObject.SetActive(true);
    }
    void OnStopRecord()
    {

        _StartRecord.gameObject.SetActive(true);
        _StopRecord.gameObject.SetActive(false);
    }
}
