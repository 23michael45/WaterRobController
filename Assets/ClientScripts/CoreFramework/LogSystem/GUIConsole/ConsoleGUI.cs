// Copyright Nicholas Ventimiglia
// Nick@AvariceOnline.com
// http://www.AvariceOnline.com/Home/Api
// 19/08/2013
// 
// Part of the GUIConsole system for unity3d
// 
// ConsoleGUI.cs

using System;
using System.Linq;
using UnityEngine;

namespace GUIConsole
{
    /// <summary>
    /// renders the ConsoleContext using OnGUI
    /// </summary>
    [AddComponentMenu("Avarice/Console/ConsoleGUI")]
    public class ConsoleGUI : MonoSingleton<ConsoleGUI>
    {
        /// <summary>
        /// skin to use
        /// </summary>
        public GUISkin MySkin;

        /// <summary>
        /// Items
        /// </summary>
        protected bool ReverseSort = false;

        public float MarginTop = 32;
        public float MarginBottom = 200;
        public float MarginLeft = 32;
        public float MarginRight = 32;

        public float Padding = 4;

        public float CommandWidth = 180;

        // submit
        string _inputValue = string.Empty;
        // items
        Vector2 _scrollItems = Vector2.zero;
        // command menu
        protected Vector2 _scrollCmdMenu = Vector2.zero;
        //items
        ConsoleItem[] _items;

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool bVisible = true;


        private int _current_Page = 0;

        void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
            this.bVisible = false;

            Update();            
        }

        private  float _fFlushTimeout = 1.0f;
        void Update()
        {
#if    UNITY_EDITOR

            if (Input.GetKeyDown(KeyCode.F10))
            {
                this.bVisible = !this.bVisible;
            }

            if (this.bVisible)
            {
                _items = ReverseSort
                                ? ConsoleContext.Instance.Items.AsEnumerable().Reverse().ToArray()
                                : ConsoleContext.Instance.Items.ToArray();

                _fFlushTimeout -= Time.deltaTime;

                if (_fFlushTimeout < 0)
                {
                    _fFlushTimeout = 1.0f;
                }


                if (Input.GetKeyDown(KeyCode.F11))
                {
                    ConsoleContext.Instance.Items.Clear();
                }
            }
#endif
        }

        void OnDestroy()
        {
            ConsoleContext.Instance.Flush();
        }

        void OnGUI()
        {
            if (!this.bVisible)
                return;

            GUI.skin = MySkin;

            GUILayout.BeginHorizontal();
            GUILayout.Space(MarginLeft);

            // left side
            GUILayout.BeginVertical(
                GUILayout.MaxWidth(Screen.width - (MarginRight + CommandWidth + Padding)),
                GUILayout.MaxHeight(Screen.height - (MarginTop +  Padding))
                );
            GUILayout.Space(MarginTop);

            // top menu
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("PrePage"))
            {
                DoMovePrePage();
            }

            if (GUILayout.Button("NextPage"))
            {
                DoMoveNextPage();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(Padding);

            // end menu

            _scrollItems = GUILayout.BeginScrollView(_scrollItems,
                true,
                true,
                GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true));

            int idx = 0;
            int count = _items.Length;
            foreach (var item in _items)
            {
                if ((count - idx) < (_current_Page + 1) * 50 && (count - idx) >= _current_Page * 50)
                {
                    GUI.color = item.Color;
                    GUILayout.Label(item.Formatted, GUILayout.ExpandHeight(false));            
                }

                idx++;
            }
            GUI.color = GUI.contentColor;

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.Space(Padding);
            //end left side

            // menu
            GUILayout.BeginVertical(
                GUILayout.Width(CommandWidth + MarginRight));
            GUILayout.Space(MarginTop);

            GUILayout.Space(MarginBottom);
            GUILayout.EndVertical();

            //end menu

            GUILayout.EndHorizontal();
        }

        public void DoSend()
        {
            _inputValue = _inputValue.Replace(Environment.NewLine, "");

            if (string.IsNullOrEmpty(_inputValue))
                return;

            ConsoleContext.Instance.Submit(_inputValue);

            _inputValue = string.Empty;
        }

        public void DoMovePrePage()
        { 
            _current_Page++;

            if (_current_Page > this._items.Count() / 50 + 1)
            {
                _current_Page = this._items.Count() / 50 + 1;
            }
        }

        public void DoMoveNextPage()
        {
            _current_Page--;
            _current_Page = _current_Page >= 0 ? _current_Page : 0;
        }
    }
}