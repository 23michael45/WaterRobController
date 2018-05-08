///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Camera Transitions.
//
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using CameraTransitions;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraTransitionManager : MonoSingleton<CameraTransitionManager>
{
    public CameraTransition cameraTransition;
    public CameraTransitionsAssistant assistant;
    public CameraTransitionEffects _CurTransitionMode = CameraTransitionEffects.None;

    public Camera mCameraA;

    private GUIStyle labelStyle;

    Dictionary<PIPanoView.EPANOMESHMODE, Camera[]> _CameraDic = new Dictionary<PIPanoView.EPANOMESHMODE, Camera[]>();

    PIPanoView.EPANOMESHMODE _NextMode = PIPanoView.EPANOMESHMODE.EMP_MAX;

    private void Awake()
    {

        _CameraDic = PIPanoView.Instance._CameraDic;

        //if (null == mCameraA)
        //{
        //    Debug.LogError("mCameraA Init");

        //    mCameraA = gameObject.GetComponentsInChildren<Camera>(true)[0];

        //    mCameraA.CopyFrom(PanoManager.Instance.mCameraPreRender);

        //    mCameraA.targetTexture = RenderTexture.active;

        //    assistant.cameraA = mCameraA;

        //    mCameraA.enabled = false;
        //}
    }

    private void OnEnable()
    {
        if (cameraTransition == null)
        {
            cameraTransition = GameObject.FindObjectOfType<CameraTransition>();
            if (cameraTransition == null)
            {
                Debug.LogError(@"No CameraTransition found.");
            }
        }

        if (assistant == null)
        {
            assistant = GameObject.FindObjectOfType<CameraTransitionsAssistant>();
            if (cameraTransition == null)
            {
                Debug.LogError(@"No CameraTransitionsAssistant found.");
            }
        }

        _CurTransitionMode = CameraTransitionEffects.None;
        assistant.transitionEffect = _CurTransitionMode;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            assistant.ExecuteTransition();
        }
    }

    private void OnGUI()
    {
        //if (labelStyle == null)
        //{
        //    labelStyle = new GUIStyle(GUI.skin.textArea);
        //    labelStyle.alignment = TextAnchor.MiddleCenter;
        //    labelStyle.fontSize = 22;
        //}

        //string label = string.Empty;
        //if (cameraTransition.IsRunning == true)
        //    label = cameraTransition.Transition.ToString();
        //else
        //    label = @"Press any number";

        //GUI.enabled = !cameraTransition.IsRunning;

        //GUILayout.BeginArea(new Rect(Screen.width * 0.5f - 150.0f, 20.0f, 300.0f, 30.0f), label, labelStyle);
        //GUILayout.EndArea();
    }

    #region Public Function

    public void SetTransitionMode(CameraTransitionEffects emode)
    {
        if (_CurTransitionMode == emode)
        {
            return;
        }

        if (emode >= CameraTransitionEffects.None && emode < CameraTransitionEffects.Max)
        {
            _CurTransitionMode = emode;
            assistant.transitionEffect = _CurTransitionMode;
        }
    }

    //根据当前模式和下一个模式，设置过渡动画相机
    public bool SetTransitionCameras(PIPanoView.EPANOMESHMODE emode)
    {
        Camera cameraA = null;
        Camera cameraB = null;

        if (emode == PIPanoView.Instance._CurrentMode)
        {
            return false;
        }

        //只对一个相机的模式做处理
        foreach (KeyValuePair<PIPanoView.EPANOMESHMODE, Camera[]> kv in _CameraDic)
        {
            if (kv.Key == PIPanoView.Instance._CurrentMode)
            {
                if (kv.Value.Length == 1)
                {
                    cameraA = kv.Value[0];
                }
                else
                {
                    //CameraTransitionManager.Instance.mCameraA.targetTexture = PanoManager.Instance.mPreRenderTexture;//PanoManager.Instance.mCameraPreRender.targetTexture;
                    //cameraA = CameraTransitionManager.Instance.mCameraA;

                    //cameraA = PanoManager.Instance.mCameraPreRender;
                }
            }

            if (kv.Key == emode)
            {
                if (kv.Value.Length == 1)
                {
                    cameraB = kv.Value[0];
                }
            }
        }

        //Debug.LogError("cameraA: " + cameraA);
        //Debug.LogError("cameraB: " + cameraB);

        if (cameraA == null || cameraB == null)
        {
            return false;
        }
        else
        {
            //设置目标相机的显示
            foreach (KeyValuePair<PIPanoView.EPANOMESHMODE, PanoModeBase> kv in PIPanoView.Instance._ModeDic)
            {
                if (kv.Key == emode)
                {
                    kv.Value.gameObject.SetActive(true);
                }
            }

            assistant.cameraA = cameraA;
            assistant.cameraB = cameraB;
            return true;
        }
    }

    public void CameraTransitionStart(PIPanoView.EPANOMESHMODE emode)
    {
        bool ret = false;

        //先判参数是否合法
        foreach (KeyValuePair<PIPanoView.EPANOMESHMODE, PanoModeBase> kv in PIPanoView.Instance._ModeDic)
        {
            if (kv.Key == emode)
            {
                ret = true;
                break;
            }
        }

        if (!ret)
        {
            Debug.LogError("EPANOMESHMODE invalid: " + emode);
            StartCoroutine(CmaeraTransitionEnd(emode));
            return;
        }

        ret = SetTransitionCameras(emode);
        if (!ret)
        {
            StartCoroutine(CmaeraTransitionEnd(emode));
        }
        else
        {
            _NextMode = emode;
            StartCoroutine(ExecuteTransition());
        }
    }

    public IEnumerator CmaeraTransitionEnd(PIPanoView.EPANOMESHMODE emode)
    {
        if (emode == PIPanoView.EPANOMESHMODE.EMP_MAX)
        {
            emode = _NextMode;
        }
        PIPanoView.Instance.EnablePanoMode(emode);

        assistant.cameraA = null;
        assistant.cameraB = null;

        yield return null;
    }

    public IEnumerator ExecuteTransition()
    {
        //yield return new WaitForSeconds(1.0f);
        assistant.ExecuteTransition();

        yield return null;
    }
    #endregion
}