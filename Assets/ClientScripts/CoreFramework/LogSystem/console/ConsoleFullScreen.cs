using System.Collections;
using UnityEngine;

namespace GUIConsole
{
    /// <summary>
    /// Extends the console with 'Full Screen' command
    /// </summary>
    public class ConsoleFullScreen : MonoBehaviour
    {

        void Awake()
        {
            StartCoroutine(FullScreen(false));
        }

        IEnumerator FullScreen(bool on)
        {
            Screen.fullScreen = on;

            yield return 1;
            ConsoleContext.Instance.UnregisterCommand("FullScreen");
            ConsoleContext.Instance.UnregisterCommand("ShrinkScreen");
            yield return 1;
            
            if (on)
            {
                ConsoleContext.Instance.RegisterCommand("ShrinkScreen", true, args =>
                {
                    StartCoroutine(FullScreen(false));
                    return string.Empty;
                });
            }
            else
            {
                ConsoleContext.Instance.RegisterCommand("FullScreen", true, args =>
                {
                    StartCoroutine(FullScreen(true));
                    return string.Empty;
                });
            }
        }

    }
}