//---------------------------------------------------------------------------------
// <copyright file="XmlConfigReader.cs" company="BHGame Inc.">
//     Copyright (c) 2011 CMGE Inc. All rights reserved.
// </copyright>
// <author></author>
// <description>
//             
//  </description>
// <history created="2014/06/09">
//    <modified date ="2014/06/09"> </modified>
// </history>
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System;

public class XmlConfigReader
{
    #region Members
    string XmlPath;

    protected Dictionary<string, Dictionary<string, string>> xmlTable = new Dictionary<string, Dictionary<string, string>>();
    #endregion
    public XmlConfigReader(string file)
    {
        XmlPath = file;
        byte[] databytes = readFromFile(XmlPath);

        if (databytes != null)
        {
            string DecodeString = System.Text.Encoding.UTF8.GetString(databytes);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(DecodeString);

            InitDic(xml);
        }
    }

    void InitDic(XmlDocument xmldoc)
    {
        try
        {
            XmlNode node = xmldoc.ChildNodes.Item(0);

            int index = 0;

            foreach (XmlNode nodeChild in node.ChildNodes)
            {
                Dictionary<string, string> vals = new Dictionary<string, string>();


                string mainkey = index.ToString();

                bool valid = false;

                foreach (XmlAttribute valChild in nodeChild.Attributes)
                {
                    string key = valChild.Name;
                    string value = valChild.Value;
                    vals[key] = value;

                    if (!string.IsNullOrEmpty(value))
                    {
                        valid = true;
                    }
                }

                if(valid)
                {
                    xmlTable.Add(mainkey, vals);


                }
                index++;
            }
        }
        catch(Exception e)
        {
            ATrace.LogError("Load Xml Failed : " + xmldoc.BaseURI);
        }
      
    }
    public T String2T<T>(string s)
    {
        object v = null;
        if (typeof(T) == typeof(int))
        {
            s = s.Replace("٫", ".");
            s = s.Replace(",", ".");
            if(s == "" || s == null || string.IsNullOrEmpty(s))
            {
                v = int.MaxValue;
            }
            else
            {
                v = Convert.ToInt32(s);

            }
        }
        else if (typeof(T) == typeof(float))
        {
            s = s.Replace("٫", ".");
            s = s.Replace(",", ".");
            v = Convert.ToSingle(s);
        }
        else if (typeof(T) == typeof(string))
        {
            v = s;
        }
        return (T)v;
    }

    public T GetValue<T>(string mainkey, string key)
    {
        if (xmlTable.ContainsKey(mainkey))
        {
            Dictionary<string, string> dic = xmlTable[mainkey];
            if (dic.ContainsKey(key))
            {
                string v = dic[key];
                return String2T<T>(v);
            }
        }
        return default(T);
    }

    public List<string> GetMainKeys()
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, Dictionary<string, string>> kv in xmlTable)
        {
            list.Add(kv.Key);
        }
        return list;
    }

    public List<string> GetSubKeys()
    {
        List<string> list = new List<string>();
        if (xmlTable.Count > 0)
        {
            string mainkey = "";
            foreach (KeyValuePair<string, Dictionary<string, string>> kv in xmlTable)
            {
                mainkey = kv.Key;
                break;
            }

            Dictionary<string, string> dic = xmlTable[mainkey];

            foreach (KeyValuePair<string, string> kv in dic)
            {
                list.Add(kv.Key);
            }


        }
        return list;
    }


    public T Find<T>(string key1, string v1, string key2, string v2, string keyrt)
    {
        foreach (KeyValuePair<string, Dictionary<string, string>> kv in xmlTable)
        {
            string key = kv.Key;
            Dictionary<string, string> dic = kv.Value;

            if (dic.ContainsKey(key1) && dic[key1] == v1)
            {
                if (dic.ContainsKey(key2) && dic[key2] == v2)
                {
                    if (dic.ContainsKey(keyrt))
                    {
                        string v = dic[keyrt];
                        return String2T<T>(v);
                    }
                }
            }

        }
        return default(T);
    }

    public static string ConnectPath(string s1,string s2)
    {
        return s1 + "_" + s2;
    }

    #region File
    byte[] readFromFile(string File_Name)
    {
        ATrace.Log("XmlFile:" + File_Name);
        TextAsset textContent = Resources.Load(File_Name) as TextAsset;
        if(textContent == null)
        {
            return null;
        }
        return textContent.bytes;
    }

    #endregion
}
