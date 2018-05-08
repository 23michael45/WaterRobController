// Copyright Nicholas Ventimiglia
// Nick@AvariceOnline.com
// http://www.AvariceOnline.com/Home/Api
// 19/08/2013
// 
// Part of the GUIConsole system for unity3d
// 
// ConsoleSetup.cs

using UnityEngine;

namespace GUIConsole
{
    /// <summary>
    /// An optional setup script for the console system.
    /// Applies the color scheme to the context.
    /// </summary>
    [AddComponentMenu("Avarice/Console/ConsoleSetup")]
    public class ConsoleSetup : MonoBehaviour
    {
        public Color TextColor = Color.white;
        public Color WarningColor = Color.yellow;
        public Color ErrorColor = Color.red;
        public Color SuccessColor = Color.green;
        public Color InputColor = Color.green;
        public Color OutputColor = Color.cyan;

        public void Awake()
        {
            ConsoleContext.Instance.TextColor = TextColor;
            ConsoleContext.Instance.WarningColor = WarningColor;
            ConsoleContext.Instance.ErrorColor = ErrorColor;
            ConsoleContext.Instance.SuccessColor = SuccessColor;
            ConsoleContext.Instance.InputColor = InputColor;
            ConsoleContext.Instance.OutputColor = OutputColor;
        }
    }
}