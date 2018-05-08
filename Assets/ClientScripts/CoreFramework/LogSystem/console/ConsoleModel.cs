// Copyright Nicholas Ventimiglia
// Nick@AvariceOnline.com
// http://www.AvariceOnline.com/Home/Api
// 19/08/2013
// 
// Part of the GUIConsole system for unity3d
// 
// ConsoleModel.cs

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#if !UNITY_WEBPLAYER
using System.IO;
#endif

namespace GUIConsole
{

    /// <summary>
    /// Method signiture for and Console commands.
    /// Console commands may be invoked as a button or text input (with arguments)
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object ConsoleCommand(params string[] args);

    /// <summary>
    /// This Enum holds the message types used to easily control the formatting and display of a message.
    /// </summary>
    public enum MessageType
    {
        Text,
        Warning,
        Error,
        Success,
        Output,
        Input,
    }

    /// <summary>
    /// A console line item
    /// </summary>
    public struct ConsoleItem
    {
        readonly string _text;
        public string Text
        {
            get { return _text; }
        }

        readonly string _formatted;
        public string Formatted
        {
            get { return _formatted; }
        }

        readonly MessageType _type;
        public MessageType Type
        {
            get { return _type; }
        }

        readonly Color _color;
        public Color Color
        {
            get { return _color; }
        }

        public ConsoleItem(MessageType type, string text)
        {
            _text = text;
            _type = type;
            switch (_type)
            {
                case MessageType.Warning:
                    _formatted = string.Format("<< {0}", text);
                    _color = ConsoleContext.Instance.WarningColor;
                    break;
                case MessageType.Error:
                    _formatted = string.Format("<< {0}", text);
                    _color = ConsoleContext.Instance.ErrorColor;
                    break;
                case MessageType.Success:
                    _formatted = string.Format("<< {0}", text);
                    _color = ConsoleContext.Instance.SuccessColor;
                    break;
                case MessageType.Output:
                    _formatted = string.Format("<< {0}", text);
                    _color = ConsoleContext.Instance.OutputColor;
                    break;
                case MessageType.Input:
                    _formatted = string.Format(">> {0}", text);
                    _color = ConsoleContext.Instance.InputColor;
                    break;
                default:
                    _formatted = text;
                    _color = ConsoleContext.Instance.TextColor;
                    break;
            }
        }
    }
    
    /// <summary>
    /// The console context is the centerpiece of the console system.
    /// It is the datamodel to which views may observer
    /// </summary>
    public class ConsoleContext
    // If MvvM
    //: ObservableObject
    {
        #region static
        public static readonly ConsoleContext Instance = new ConsoleContext();
        #endregion

        #region settings

        // Default color of the standard display text.

        public Color TextColor = Color.white;
        public Color WarningColor = Color.yellow;
        public Color ErrorColor = Color.red;
        public Color SuccessColor = Color.green;
        public Color InputColor = Color.green;
        public Color OutputColor = Color.cyan;

        #endregion

        #region props

        // If MvvM
        //public readonly ObservableList<ConsoleItem> Items = new ObservableList<ConsoleItem>();
        public readonly List<ConsoleItem> Items = new List<ConsoleItem>();
        public readonly List<ConsoleItem> AppendItems = new List<ConsoleItem>();

        public readonly Dictionary<string, ConsoleCommand> Listeners = new Dictionary<string, ConsoleCommand>();

        // If MvvM
        //public readonly ObservableDictionary<string, ConsoleCommand> Items = new ObservableDictionary<string, ConsoleCommand>()
        public readonly Dictionary<string, ConsoleCommand> CommandMenu = new Dictionary<string, ConsoleCommand>();

        #endregion

        #region private methods

        /// <summary>
        ///  log file name
        /// </summary>
        public string logFile = "";

        ConsoleContext()
        {
            this.logFile = string.Format("{0}.txt", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));

            RegisterCommand("clear", true, DoClear);
            RegisterCommand("sys", true, CMDSystemInfo);
#if !UNITY_WEBPLAYER
            RegisterCommand("save", true, DoSave);
#endif
            RegisterCommand("/?", true, CMDHelp);
        }

        object DoClear(string[] p)
        {
            Clear();

            return string.Empty;
        }
        
#if !UNITY_WEBPLAYER
        object DoSave(string[] p)
        {
            if (AppendItems.Count == 0)
                return this.logFile ;

            var sb = new StringBuilder();

            foreach (var message in AppendItems)
            {
                sb.AppendLine(message.Formatted);
            }

            var path = Application.persistentDataPath + "/Console/";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //File.WriteAllText(path + n, sb.ToString());
            File.AppendAllText(path + this.logFile, sb.ToString());
            AppendItems.Clear();

            return this.logFile;
        }
#endif

        object CMDHelp(string[] p)
        {
            var output = new StringBuilder();

            output.AppendLine(":: Command List ::");

            foreach (var key in Listeners)
            {
                output.AppendLine(key.Key);
            }

            return output.ToString();

        }

        object CMDSystemInfo(params string[] args)
        {
            var info = new StringBuilder();

            info.AppendFormat("Unity Ver: {0}\n", Application.unityVersion);
            info.AppendFormat("Platform: {0} Language: {1}\n", Application.platform, Application.systemLanguage);
            info.AppendFormat("Screen:({0},{1}) DPI:{2} Target:{3}fps\n", Screen.width, Screen.height, Screen.dpi, Application.targetFrameRate);
            info.AppendFormat("LevelContext: {0} ({1} of {2})\n", Application.loadedLevelName, Application.loadedLevel, Application.levelCount);
            info.AppendFormat("Quality: {0}\n", QualitySettings.names[QualitySettings.GetQualityLevel()]);
            info.AppendLine();
            info.AppendFormat("Data Path: {0}\n", Application.dataPath);
            info.AppendFormat("Cache Path: {0}\n", Application.temporaryCachePath);
            info.AppendFormat("Persistent Path: {0}\n", Application.persistentDataPath);
            info.AppendFormat("Streaming Path: {0}\n", Application.streamingAssetsPath);
#if UNITY_WEBPLAYER
    info.AppendLine();
    info.AppendFormat("URL: {0}\n", Application.absoluteURL);
    info.AppendFormat("srcValue: {0}\n", Application.srcValue);
    info.AppendFormat("security URL: {0}\n", Application.webSecurityHostUrl);
#endif
#if MOBILE
    info.AppendLine();
    info.AppendFormat("Net Reachability: {0}\n", Application.internetReachability);
    info.AppendFormat("Multitouch: {0}\n", Input.multiTouchEnabled);
#endif
#if UNITY_EDITOR
            info.AppendLine();
            info.AppendFormat("editorApp: {0}\n", UnityEditor.EditorApplication.applicationPath);
            info.AppendFormat("editorAppContents: {0}\n", UnityEditor.EditorApplication.applicationContentsPath);
            info.AppendFormat("scene: {0}\n", UnityEditor.EditorApplication.currentScene);
#endif
            info.AppendLine();
            var devices = WebCamTexture.devices;
            if (devices.Length > 0)
            {
                info.AppendLine("Cameras: ");

                foreach (var device in devices)
                {
                    info.AppendFormat("  {0} front:{1}\n", device.name, device.isFrontFacing);
                }
            }

            return info.ToString();
        }
        #endregion

        /// <summary>
        /// write only
        /// </summary>
        public void Log(object message, MessageType type)
        {
            Items.Add(new ConsoleItem(type, message.ToString()));
            AppendItems.Add(new ConsoleItem(type, message.ToString()));
        }

        bool bUseLogText = true;
        /// <summary>
        /// write only
        /// </summary>
        public void LogText(object message)
        {
            if (!bUseLogText)
                return;

            Items.Add(new ConsoleItem(MessageType.Text, message.ToString()));
            AppendItems.Add(new ConsoleItem(MessageType.Text, message.ToString()));
        }

        /// <summary>
        /// write only
        /// </summary>
        public void LogError(object message)
        {
            Items.Add(new ConsoleItem(MessageType.Error, message.ToString()));
            AppendItems.Add(new ConsoleItem(MessageType.Error, message.ToString()));
        }

        /// <summary>
        /// write only
        /// </summary>
        public void LogWarning(object message)
        {
            Items.Add(new ConsoleItem(MessageType.Warning, message.ToString()));
            AppendItems.Add(new ConsoleItem(MessageType.Warning, message.ToString()));
        }

        /// <summary>
        /// write only
        /// </summary>
        public void LogSuccess(object message)
        {
            Items.Add(new ConsoleItem(MessageType.Success, message.ToString()));
            AppendItems.Add(new ConsoleItem(MessageType.Success, message.ToString()));
        }

        /// <summary>
        /// write only
        /// </summary>
        public void LogInput(object message)
        {
            Items.Add(new ConsoleItem(MessageType.Input, message.ToString()));
            AppendItems.Add(new ConsoleItem(MessageType.Input, message.ToString()));
        }

        /// <summary>
        /// write only
        /// </summary>
        public void LogOutput(object message)
        {
            Items.Add(new ConsoleItem(MessageType.Output, message.ToString()));
            AppendItems.Add(new ConsoleItem(MessageType.Output, message.ToString()));
        }

        /// <summary>
        /// Input for a command
        /// </summary>
        /// <param name="message"></param>
        public void Submit(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                LogInput(string.Empty);
                return;
            }

            message = message.Trim();
            Items.Add(new ConsoleItem(MessageType.Input, message));

            var input = new List<string>(
                message.Split(new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries));

            input = input.ConvertAll(low => low.ToLower());

            var cmd = input[0].ToLower();

            if (Listeners.ContainsKey(cmd))
            {
                var r = Listeners[cmd].Invoke(input.ToArray());

                if (r != null)
                    Items.Add(new ConsoleItem(MessageType.Output, r.ToString()));
            }
            else
            {
                LogError(string.Format("Unknown Command: {0} ", cmd));
            }
        }

        /// <summary>
        /// clear writes
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// registers a command  listener
        /// </summary>
        /// <param name="key"></param>
        /// <param name="addToMenu"></param>
        /// <param name="cmd"></param>
        public void RegisterCommand(string key, bool addToMenu, ConsoleCommand cmd)
        {
            key = key.ToLower();

            if (Listeners.ContainsKey(key))
            {
                LogError(string.Format("Command In Use: {0} ", cmd));
            }
            else
            {
                Listeners.Add(key, cmd);
            }

            if (addToMenu)
            {
                if (CommandMenu.ContainsKey(key))
                {
                    LogError(string.Format("Menu Command In Use: {0} ", cmd));
                }
                else
                {
                    CommandMenu.Add(key, cmd);
                }

            }
        }

        /// <summary>
        /// removes a command listener
        /// </summary>
        /// <param name="key"></param>
        public void UnregisterCommand(string key)
        {
            Listeners.Remove(key.ToLower());
            CommandMenu.Remove(key.ToLower());
        }

        /// <summary>
        /// flush all the log to file
        /// </summary>
        public void Flush()
        {
            this.DoSave(null);
        }
    }
}