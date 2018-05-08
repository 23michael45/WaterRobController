/********************************************************************
	created:	2016/12/19
	created:	19:12:2016   9:50
	file base:	Message
	file ext:	cs
	author:		michael lee
	
	purpose:	Message 消息类
*********************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CoreFramework {

    /// <summary>
    /// 全局消息类 负责全局交互 解耦合
    /// </summary>
    public class Message{

        /// <summary>
        /// 回调代理
        /// </summary>
        /// <param name="callerType">调用者类型</param>
        /// <param name="handlerType">处理者类型</param>
        /// <param name="messageType">消息子类类型</param>
        /// <param name="messageName">消息名称</param>
        /// <param name="handlerMethodName">处理方法名</param>
        public delegate void OnMessageHandleDelegate(Type callerType, Type handlerType, Type messageType, string messageName, string handlerMethodName);

        /// <summary>
        ///消息被发送并处理时调用. 只在Unity editor里有效.
        /// </summary>
        public static OnMessageHandleDelegate OnMessageHandle;

        private static Dictionary<string, List<Delegate>> handlers = new Dictionary<string, List<Delegate>>();

        /// <summary>
        /// 前缀名 区分内部无名称的消息类型
        /// </summary>
        private const string TypelessMessagePrefix = "typeless ";

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="messageName">消息名称</param>
        /// <param name="callback">回调函数</param>
        public static void AddListener(string messageName, Action callback) {
            RegisterListener(TypelessMessagePrefix + messageName, callback);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="callback">回调函数</param>
        public static void AddListener<T>(Action<T> callback) where T : Message {
            RegisterListener(typeof(T).ToString(), callback);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="messageName">消息名称</param>
        /// <param name="callback">回调函数</param>
        public static void AddListener<T>(string messageName, Action<T> callback) where T : Message {
            RegisterListener(typeof(T).ToString() + messageName, callback);
        }

        /// <summary>
        /// 删除监听 
        /// </summary>
        /// <param name="messageName">消息名称</param>
        /// <param name="callback">回调函数</param>
        public static void RemoveListener(string messageName, Action callback) {
            UnregisterListener(TypelessMessagePrefix + messageName, callback);
        }

        /// <summary>
        /// 删除监听 
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="callback">回调函数</param>
        public static void RemoveListener<T>(Action<T> callback) where T : Message {
            UnregisterListener(typeof(T).ToString(), callback);
        }

        /// <summary>
        /// 删除监听 
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="messageName">消息名称</param>
        /// <param name="callback">回调函数</param>
        public static void RemoveListener<T>(string messageName, Action<T> callback) where T : Message {
            UnregisterListener(typeof(T).ToString() + messageName, callback);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageName">消息名称</param>
        public static void Send(string messageName) {
            SendMessage<Message>(TypelessMessagePrefix + messageName, null);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="message">消息类实例</param>
        public static void Send<T>(T message) where T : Message {
            SendMessage<T>(typeof(T).ToString(), message);
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="messageName">消息名称</param>
        /// <param name="message">消息类实例</param>
        public static void Send<T>(string messageName, T message) where T : Message {
            SendMessage<T>(typeof(T).ToString() + messageName, message);
        }

        private static void RegisterListener(string messageName, Delegate callback) {
            if (callback == null) {
                UnityEngine.Debug.LogError("Failed to add Message Listener because the given callback is null!");
                return;
            }
            if (!handlers.ContainsKey(messageName)) {
                handlers.Add(messageName, new List<Delegate>());
            }
            List<Delegate> messageHandlers = handlers[messageName];
            messageHandlers.Add(callback);
        }

        private static void UnregisterListener(string messageName, Delegate callback) {
            if (!handlers.ContainsKey(messageName)) { return; }
            List<Delegate> messageHandlers = handlers[messageName];
            Delegate messageHandler = messageHandlers.Find(x => x.Method == callback.Method && x.Target == callback.Target);
            if (messageHandler == null) { return; }
            messageHandlers.Remove(messageHandler);
        }

        private static void SendMessage<T>(string messageName, T e) where T : Message {
            TimerManager.Instance.StartCoroutine(DealMessage<T>(messageName, e));

            ATrace.Log("Send Message:" + typeof(T).ToString());
            //UnityEngine.Debug.Log("Send Message:" + typeof(T).ToString());
        }

        static IEnumerator DealMessage<T>(string messageName, T e)
        {
            yield return new WaitForEndOfFrame();

            //UnityEngine.Debug.LogFormat("SendMessage -> messageName:{0} match Handler:{1}", messageName, handlers.ContainsKey(messageName));
            if (!handlers.ContainsKey(messageName)) { yield break; }

            Type callerType = null;
            if (Application.isEditor)
            {
                StackTrace stackTrace = new StackTrace();
                callerType = stackTrace.GetFrame(2).GetMethod().DeclaringType;
            }

            List<Delegate> messageHandlers = handlers[messageName];
            foreach (Delegate messageHandler in messageHandlers)
            {
                if (messageHandler.GetType() != typeof(Action<T>) && messageHandler.GetType() != typeof(Action)) { continue; }

                if (Application.isEditor && OnMessageHandle != null)
                {
                    string methodName = messageHandler.Method.Name;

                    messageName = messageName.Replace(TypelessMessagePrefix, "");

                    if (typeof(T) != typeof(Message))
                    {
                        messageName = messageName.Replace(typeof(T).ToString(), "");
                    }

                    OnMessageHandle(callerType, messageHandler.Target.GetType(), typeof(T), messageName, methodName);
                }

                if (typeof(T) == typeof(Message))
                {
                    Action action = (Action)messageHandler;
                    action();
                }
                else
                {
                    Action<T> action = (Action<T>)messageHandler;
                    action(e);
                }
                //UnityEngine.Debug.LogFormat("delegate:{0} target:{1} method:{2}", messageHandler, messageHandler.Target, messageHandler.Method.Name);
            }
        }
      
        protected Message() { }

    }

    public class Simple_Msg : Message
    {
        public virtual void SendMsg<T>() where T : Simple_Msg
        {

            ATrace.Log("Simple_Msg:" + typeof(T).ToString());
            Message.Send<T>((T)this);
        }

        public static void Send<T>() where T : Simple_Msg
        {
            T inst = Activator.CreateInstance<T>();
            inst.SendMsg<T>();
        }
    }

    public class Bool_Base_Msg : Message
    {
        public bool _b = false;
        public virtual void SendMsg<T>(bool b) where T : Bool_Base_Msg
        {

            ATrace.Log("Bool_Base_Msg:" + typeof(T).ToString() + " : " + b);

            _b = b;
            Message.Send<T>((T)this);
        }

        public static void Send<T>(bool b) where T : Bool_Base_Msg
        {
            T inst = Activator.CreateInstance<T>();
            inst.SendMsg<T>(b);
        }
    }

    public class State_Base_Msg : Message
    {
        public enum ESET_Result
        {
            ESR_Success,
            ESR_Failed,
        }
        public ESET_Result _Result = ESET_Result.ESR_Failed;
        public virtual void SendMsg<T>(ESET_Result r) where T : State_Base_Msg
        {

            ATrace.Log("State_Base_Msg:" + typeof(T).ToString() + " : " + r);

            _Result = r;
            Message.Send<T>((T)this);
        }

        public static void Send<T>(ESET_Result r) where T : State_Base_Msg
        {
            T inst = Activator.CreateInstance<T>();
            inst.SendMsg<T>(r);
        }
    }
}