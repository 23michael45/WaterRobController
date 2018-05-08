using CoreFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class MainController : MonoBehaviour {

    public MeterController _MetersContainer;
    public OperationController _OperationContainer;

    
    private void Start()
    {
        _OperationContainer._ParentController = this; 
    }

    public void ShowHideMeters(bool b)
    {
        _MetersContainer.gameObject.SetActive(b);

        Camera.main.gameObject.GetComponent<Bloom>().enabled = b;
    }

    
}
