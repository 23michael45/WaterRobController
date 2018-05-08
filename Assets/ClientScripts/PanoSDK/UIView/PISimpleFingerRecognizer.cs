using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PISimpleFingerRecognizer : MonoBehaviour {


	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            gameObject.SendMessage("OnSimpleFingerDown", Input.mousePosition, SendMessageOptions.DontRequireReceiver);
        }
        if(Input.GetMouseButtonUp(0))
        {

            gameObject.SendMessage("OnSimpleFingerUp", Input.mousePosition, SendMessageOptions.DontRequireReceiver);
        }
	}
}
