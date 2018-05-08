/********************************************************************
	created:	2016/12/19
	created:	19:12:2016   15:12
	file base:	ATrace
	file ext:	cs
	author:		michael lee
	
	purpose:	日志系统
*********************************************************************/
#define HAS_ATRACE
//#define DISABLE_ATRACE_DISPLAY

#if HAS_ATRACE

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using GUIConsole;
using System.Text.RegularExpressions;
using System.Threading;

/// <summary>
/// The ATrace class defines ...
/// </summary>
public class ATrace : MonoSingleton<ATrace>
{
    #region Fields

    /// <summary>
    /// The name of the trace.
    /// </summary>
    private const string TraceName = "ATrace";

    /// <summary>
    /// The capability of the number of the entries.
    /// </summary>
    private const int Capability = 32;

    /// <summary>
    /// The time length of an entry.
    /// </summary>
    private const float EntryTimeLength = 32f;

    /// <summary>
    /// The width of the entry.
    /// </summary>
    private const float EntryWidth = 1600f;

    /// <summary>
    /// The height of the entry.
    /// </summary>
    private const float EntryHeight = 24f;

    /// <summary>
    /// The entries.
    /// </summary>
    ////private Entry[] entries = new Entry[Capability];
    private IDictionary<string, Entry> entries = new Dictionary<string, Entry>();

    /// <summary>
    /// The maximum available index.
    /// </summary>
    private int maxIndex = 0;

    #endregion

    #region Constructors


    #endregion

    /// <summary>
    ///  日志显示控制
    /// </summary>
    static Dictionary<string, string> showDict = new Dictionary<string, string>();

    /// <summary>
    /// 日志buffer队列
    /// </summary>
    static Queue<string> LogQueue = new Queue<string>();

    /// <summary>
    /// 日志文件路径
    /// </summary>
    static string logPath;

    /// <summary>
    /// 日志buffer
    /// </summary>
    static string logBuff =  "" ;

    /// <summary>
    /// 日志进程临界区
    /// </summary>
    static System.Object thisLock = new System.Object();

    /// <summary>
    /// 日志文件写入流对象
    /// </summary>
    static StreamWriter streamWrite;

    /// <summary>
    /// 日志线程
    /// </summary>
    static Thread logThread;

    public void Start()
    {
        var fileAddress = System.IO.Path.Combine(Application.streamingAssetsPath, "debug.txt");
        FileInfo fInfo0 = new FileInfo(fileAddress);
        string s = "";
        if (fInfo0.Exists)
        {
            StreamReader r = new StreamReader(fileAddress);
            while (r.Peek() >= 0)
            {
                s = r.ReadLine();
                if (s.Contains("="))
                {
                    string[] sArry = s.Split('=');
                    string key = sArry[0].Trim();
                    string value = sArry[1].Trim();
                    Match mc = Regex.Match(key, @"(?<=\[)[^\[\]]+(?=\])");   // 匹配中括号里面的字符,只匹配一次
                    key = mc.ToString().Trim();
					showDict[key] = value;
                }
                else
                    continue;
            }
        }

        #if    UNITY_EDITOR
        var logFolder = System.IO.Path.Combine(Application.dataPath, "../log");
#else
        var logFolder = System.IO.Path.Combine(Application.persistentDataPath, "log");
#endif
        if (!Directory.Exists(logFolder))
        {
            System.IO.Directory.CreateDirectory(logFolder);
        }

        var ss = System.DateTime.Now;
        var log = ss.ToString("yyyy-MM-dd-HH-mm-ss") + ".log";
        logPath = System.IO.Path.Combine(logFolder, log);
        FileStream logInfo = File.Create(logPath);
        logInfo.Close();
        InitLogThread();
    }

    #region Enumerators
    #endregion


    #region Methods    

    /// <summary>
    /// Says something using the text as the key.
    /// </summary>
    /// <param name="text">The text to say. And it is also the key.</param>
    public static void Log(object text)
    {
        Debug.Log(text);
        string logTime = GetTimeNow();
        text = logTime + text.ToString();

        ConsoleContext.Instance.LogText(text);
        AddToBuff(text);
    }

    public static void Log(object text, object type)
    {
        Debug.Log(text);
        if (!CanShowLog(type as string))
            return;
        string logTime = GetTimeNow();
        string moduleName = "[" + type + "]";
        text = logTime + moduleName+ text.ToString();

        ConsoleContext.Instance.LogText(text);
        AddToBuff(text);
    }

    public static string GetTimeNow()
    {
        string logTime = "[" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute 
            + ":" + System.DateTime.Now.Second + "." 
            + System.DateTime.Now.Millisecond + "]";
        return logTime;
    }

    /// <summary>
    /// 是否显示相应的日志
    /// </summary>
    public static bool CanShowLog(string module)
    {
        if (showDict.ContainsKey(module) && showDict[module].Contains("0"))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 初始化日志线程
    /// </summary>
    public void InitLogThread()
    {
        streamWrite = new StreamWriter(logPath, true);
        logThread = new Thread(WriteInLog);
        logThread.Priority = System.Threading.ThreadPriority.Lowest;
        logThread.Name = "LogThread";
        logThread.Start(logThread);
    }

    /// <summary>
    /// 写入到文本缓冲区
    /// </summary>
    /// <param name="text"></param>
    public static void AddToBuff(object text)
    {
        lock (thisLock)
        {
            LogQueue.Enqueue(text as string);
        }
    }

    /// <summary>
    /// 从文本缓冲区日志读取到日志缓冲区
    /// </summary>
    public static void ReadFromBuff()
    {
        lock (thisLock)
        {
            var boundary = Mathf.Min(5,LogQueue.Count);

            for (int i = 0; i < boundary; i++)
            {
                logBuff += LogQueue.Dequeue() + "\n";
            }
        }
    }

    /// <summary>
    /// 把日志缓冲区内容写入日志
    /// </summary>
    public static void WriteInLog()
    {
        while (true)
        {
            ReadFromBuff();
            if (!logBuff.Equals(""))
            {
                streamWrite.Write(logBuff);
                streamWrite.Flush();
                logBuff = "";
            }
            
            Thread.Sleep(100);
        }
    }

    public static void SaveLog()
    {
        if (streamWrite != null)
        {
            streamWrite.Flush();
            streamWrite.Close();
        }

        streamWrite = new StreamWriter(logPath, true);
    }

    /// <summary>
    /// Says something using the text as the key.
    /// </summary>
    /// <param name="text">The text to say. And it is also the key.</param>
    public static void LogError(object text)
    {
        string logTime = GetTimeNow();
        text = logTime + text.ToString();

        ConsoleContext.Instance.LogError(text);
        AddToBuff(text);
    }

    public static void Say(string text, string key = "global")
    {
#if UNITY_EDITOR
        string logTime = GetTimeNow();

        text = logTime + text;
        Instance.SetText(key, text);
#endif
    }


    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    protected void Update()
    {
        foreach (var entry in this.entries.Values)
        {
            if (entry != null && entry.HasText)
            {
                entry.Advance(Time.deltaTime);
            }
        }
    }

    new void OnDestroy()
    {
        if (streamWrite != null)
        {
            streamWrite.Flush();
            streamWrite.Close();
        }
        logThread.Abort();
    }

    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// </summary>
    /// 
#if !DISABLE_ATRACE_DISPLAY
    protected void OnGUI()
    {
        if (this.entries.Values.Count > 0)
        {
            GUILayout.BeginArea(new Rect(10, 20, EntryWidth, Capability * EntryHeight));

            foreach (var entry in this.entries.Values)
            {
                if (entry != null && entry.HasText)
                {
                    this.DrawEntry(entry);
                }
            }

            GUILayout.EndArea();
        }        
    }

#endif

    /// <summary>
    /// Gets the entry by key.
    /// </summary>
    /// <param name="key">The key of the entry.</param>
    /// <returns>The entry.</returns>
    private Entry GetEntry(string key)
    {
        ////if (index >= Capability)
        ////{
        ////    Debug.LogError("ATrace.Entry: Index out of range: index=" + index + " capability=" + Capability);
        ////}

        Entry entry = null;
        if (!this.entries.TryGetValue(key, out entry))
        {
            int index = this.AllocIndex();
            if (index >= 0)
            {
                entry = new Entry(index);
                this.entries[key] = entry;
            }
        }

        return entry;
    }

    /// <summary>
    /// Draws the entry.
    /// </summary>
    /// <param name="entry">The entry.</param>
    private void DrawEntry(Entry entry)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = entry.Color;
        ////style.fontSize = 16;
        ////style.fontStyle = FontStyle.Bold;
        ////GUI.color = entry.Color;
        GUI.Label(new Rect(0, entry.Index * EntryHeight, EntryWidth, EntryHeight), "[" + entry.Count + "]" + entry.Text, style);
    }

    /// <summary>
    /// Sets test on the specified entry.
    /// </summary>
    /// <param name="key">The key of the entry.</param>
    /// <param name="text">The text to say.</param>
    private void SetText(string key, string text)
    {
        var entry = this.GetEntry(key);
        if (entry != null)
        {
            entry.Reset(text);
        }
    }

    /// <summary>
    /// Allocates an index. The index value -1 indicates no index is available.
    /// </summary>
    /// <returns>The index.</returns>
    private int AllocIndex()
    {
        if (this.maxIndex >= Capability)
        {
            return -1;
        }

        return this.maxIndex++;
    }

    #endregion

    /// <summary>
    /// The Entry class defines ...
    /// </summary>
    private class Entry
    {
        #region Fields

        /// <summary>
        /// The red value.
        /// </summary>
        private float r = 1f;

        /// <summary>
        /// The green value.
        /// </summary>
        private float g = 1f;

        /// <summary>
        /// The blue value.
        /// </summary>
        private float b = 1f;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Entry class.
        /// </summary>
        /// <param name="index">The index of the entry.</param>
        public Entry(int index)
        {
            this.Count = 0;
            this.Timer = new CrudeElapsedTimer(EntryTimeLength);
            this.Text = null;
            this.Index = index;
        }

        #endregion

        #region Enumerators
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the index of the entry.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the text of the entry.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets the alpha.
        /// </summary>
        public float Alpha
        {
            get { return 1f - this.Timer.SaturatedElapsedRate; }
        }

        /// <summary>
        /// Gets a value indicating whether 
        /// </summary>
        public bool HasText
        {
            get { return this.Text != null; }
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        public Color Color
        {
            get { return new Color(this.r, this.g, this.b, this.Alpha); }
        }

        /// <summary>
        /// Gets or sets the timer of the entry.
        /// </summary>
        private CrudeElapsedTimer Timer { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Advances the timer.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        public void Advance(float deltaTime)
        {
            this.Timer.Advance(deltaTime);
        }

        /// <summary>
        /// Resets the entry.
        /// </summary>
        /// <param name="text">The test to reset.</param>
        public void Reset(string text)
        {
            this.Text = text;
            this.Timer.Reset();
            ++this.Count;
        }

        #endregion
    }
}

#endif
