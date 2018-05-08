using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenMeshHalfInCameraController : ScreenMeshHalfCameraController
{
   
    protected override void MakeISM()
    {
        mISM = new ISMStateMachine<ScreenMeshHalfCameraController>(this);
        
        mISM.CreateAndAdd<SMHBounceState>("Bounce", this);
        mISM.CreateAndAdd<SMHBounceDragState>("BounceDrag", this);

        
        mISM.Push("Bounce");

    }
    

}