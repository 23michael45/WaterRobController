using CoreFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TopController : MonoSingleton<TopController> {


    #region Top



    public NumericLightMeter _Battery;
    public TextMeter _CurrentTime;
    public TextMeter _LaunchTime;
    public TextMeter _Volt;
    public LevelLightMeter _Wifi;
    public SwitchMeter _Propeller;

    public Toggle _HomeBtn;
    public Toggle _GalleryBtn;
    public Toggle _SettingBtn;


    public Button _OpenBtn;
    public Button _CloseBtn;

    Animator _Animator;
    #endregion
    // Use this for initialization
    void Start () {
        _Animator = gameObject.GetComponentInChildren<Animator>();

        _OpenBtn.onClick.AddListener(OnOpen);
        _CloseBtn.onClick.AddListener(OnClose);


        Message.AddListener<BatteryChangeMessage>(OnBatteryChange);
        Message.AddListener<CurrentTimeChangeMessage>(OnCurrentTimeChange);
        Message.AddListener<LaunchTimeChangeMessage>(OnLaunchTimeChange);
        Message.AddListener<VoltChangeMessage>(OnVoltChange);
        Message.AddListener<PropellerStateChangeMessage>(OnPropellerStateChange);
        Message.AddListener<WifiStateChangeMessage>(OnWifiStateChange);


        _HomeBtn.onValueChanged.AddListener(OnHome);
        _GalleryBtn.onValueChanged.AddListener(OnGallery);
        _SettingBtn.onValueChanged.AddListener(OnSetting);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _OpenBtn.onClick.RemoveListener(OnOpen);
        _CloseBtn.onClick.RemoveListener(OnClose);


        Message.RemoveListener<BatteryChangeMessage>(OnBatteryChange);
        Message.RemoveListener<CurrentTimeChangeMessage>(OnCurrentTimeChange);
        Message.RemoveListener<LaunchTimeChangeMessage>(OnLaunchTimeChange);
        Message.RemoveListener<VoltChangeMessage>(OnVoltChange);
        Message.RemoveListener<PropellerStateChangeMessage>(OnPropellerStateChange);
        Message.RemoveListener<WifiStateChangeMessage>(OnWifiStateChange);


        _HomeBtn.onValueChanged.RemoveListener(OnHome);
        _GalleryBtn.onValueChanged.RemoveListener(OnGallery);
        _SettingBtn.onValueChanged.RemoveListener(OnSetting);
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnOpen()
    {
        _Animator.Play("Open");
    }
    void OnClose()
    {
        _Animator.Play("Close");

    }


    void OnBatteryChange(BatteryChangeMessage msg)
    {
        _Battery.SetCurrentValue(HardwareInterfaceBase.Instance.GetBattery());
    }
    void OnCurrentTimeChange(CurrentTimeChangeMessage msg)
    {
        string str = HardwareInterfaceBase.Instance.GetCurrentTime();

        _CurrentTime.SetCurrentOneValue(str);
    }
    void OnLaunchTimeChange(LaunchTimeChangeMessage msg)
    {
        string str = HardwareInterfaceBase.Instance.GetLaunchTime();

        _LaunchTime.SetCurrentOneValue(str);
    }
    void OnVoltChange(VoltChangeMessage msg)
    {
        _Volt.SetCurrentOneValue(HardwareInterfaceBase.Instance.GetVolt());
    }
    void OnWifiStateChange(WifiStateChangeMessage msg)
    {
        _Wifi.SetCurrentValue(HardwareInterfaceBase.Instance.GetWifiState());
    }

    void OnPropellerStateChange(PropellerStateChangeMessage msg)
    {
        _Propeller.SetCurrentValue(HardwareInterfaceBase.Instance.GetPropellerState());
    }



    void OnHome(bool b)
    {
        if(b)
        {
            SceneManager.LoadScene("Main");

        }
    }
    void OnGallery(bool b)
    {
        if (b)
        {
            SceneManager.LoadScene("Gallery");
        }
    }
    void OnSetting(bool b)
    {
        if (b)
        {
            SceneManager.LoadScene("Setting");
        }
    }
}
