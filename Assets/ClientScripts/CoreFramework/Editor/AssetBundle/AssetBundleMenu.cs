using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

public class AssetBundleMenu
{
    const string kSimulateAssetBundlesMenu = "AssetBundle/Simulate AssetBundles";
    const string kDistributeVersion = "AssetBundle/Distribute Version";

    static string FinalDistributionDef = "FINAL_DISTRIBUTION";


  

#region Define Symbol
    static void GetAllSymbolsFromPlayerSettings(BuildTargetGroup targetGroup, ref List<string> definedList)
    {

        definedList.Clear();

        string scriptingDeineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        char[] stringSeparators = new char[] { ';' };

        string[] symbolsStringArray = scriptingDeineSymbols.Split(stringSeparators, StringSplitOptions.None);

        for (int i = 0; i < symbolsStringArray.Length; i++)
        {

            if (symbolsStringArray[i] != string.Empty)

                definedList.Add(symbolsStringArray[i]);

        }

    }

    static void ApplyAllChangesToPlayerSettings(BuildTargetGroup targetGroup, List<string> definedList)
    {

        string defineSymbols = "";

        for (int i = 0; i < definedList.Count; i++)
        {

            defineSymbols += definedList[i] + ",";

        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbols);

    }


    public static void AddSymbol(string s)
    {
        List<string> ss = new List<string>();
        GetAllSymbolsFromPlayerSettings(BuildTargetGroup.Android,ref ss);
        if (!ss.Contains(s))
        {
            ss.Add(s);
        }
        ApplyAllChangesToPlayerSettings(BuildTargetGroup.Android, ss);

        ss = new List<string>();
        GetAllSymbolsFromPlayerSettings(BuildTargetGroup.iOS, ref ss);
        if (!ss.Contains(s))
        {
            ss.Add(s);
        }
        ApplyAllChangesToPlayerSettings(BuildTargetGroup.iOS, ss);
    }
    public static void RemoveAllSymbol()
    {
        List<string> ss = new List<string>();
        ApplyAllChangesToPlayerSettings(BuildTargetGroup.Android, ss);
        ApplyAllChangesToPlayerSettings(BuildTargetGroup.iOS, ss);
    }
    public static void RemoveSymbol(string s)
    {
        List<string> ss = new List<string>();
        GetAllSymbolsFromPlayerSettings(BuildTargetGroup.Android, ref ss);
        ss.Remove(s);
        ApplyAllChangesToPlayerSettings(BuildTargetGroup.Android, ss);

        ss = new List<string>();
        GetAllSymbolsFromPlayerSettings(BuildTargetGroup.iOS, ref ss);
        ss.Remove(s);
        ApplyAllChangesToPlayerSettings(BuildTargetGroup.iOS, ss);
    }

    static bool HasSymbol(string s)
    {
        List<string> ssa = new List<string>();
        List<string> ssi = new List<string>();
        GetAllSymbolsFromPlayerSettings(BuildTargetGroup.Android, ref ssa);
        GetAllSymbolsFromPlayerSettings(BuildTargetGroup.iOS, ref ssi);
        if (ssa.Contains(s) && ssi.Contains(s))
        {
            return true;
        }
        return false;
    }
#endregion
    [MenuItem(kDistributeVersion)]
    public static void ToggleDistributeVersion()
    {
        if (HasSymbol(FinalDistributionDef) == false)
        {
            AddSymbol(FinalDistributionDef);
        }
        else
        {
            RemoveSymbol(FinalDistributionDef);
        }
    }
    [MenuItem(kDistributeVersion, true)]
    public static bool ToggleDistributeVersionValidate()
    {
        Menu.SetChecked(kDistributeVersion, HasSymbol(FinalDistributionDef));
        return true;
    }
    [MenuItem(kSimulateAssetBundlesMenu)]
    public static void ToggleSimulateAssetBundle()
    {
        AssetBundleLoadManager.SimulateAssetBundleInEditor = !AssetBundleLoadManager.SimulateAssetBundleInEditor;
    }
    [MenuItem(kSimulateAssetBundlesMenu, true)]
    public static bool ToggleSimulateAssetBundleValidate()
    {
        Menu.SetChecked(kSimulateAssetBundlesMenu, AssetBundleLoadManager.SimulateAssetBundleInEditor);
        return true;
    }

    [MenuItem("AssetBundle/Generate Version File")]
    public static void GenerateVersionFile()
    {
        AssetBundleBuild.GenerateVersionFile();
    }

    [MenuItem("AssetBundle/Build AssetBundle")]
    public static void BuildAssetBundles()
    {
        AssetBundleBuild.BuildAssetBundles();
    }

    [MenuItem("AssetBundle/Build Player")]
    public static void BuildPlayer()
    {
        AssetBundleBuild.BuildPlayer();
    }




    [MenuItem("AssetBundle/List All AssetBundle")]
    public static void ListAllAssetBundle()
    {
        string[] bundles = AssetDatabase.GetAllAssetBundleNames();
        foreach (string bundle in bundles)
        {
            string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundle);
            foreach (string asset in assets)
            {
                Debug.Log("AssetBundle : " + bundle + " - " + "Asset :" + asset);
                if (!asset.Contains("."))
                {
                    Debug.LogError("AssetBundle : " + bundle + " - " + "Asset :" + asset + " Is a Folder");
                }
            }
        }
        
    }



    //here add some function to be call by jenkins, 
    //first we should build assetbundle 
    //and then run bat to copy
    //last we build player
    //////////////////////////////////////////////////////////////////////////
    [MenuItem("AssetBundle/Batch/Build Android AssetBundle")]
    public static void BuildAndroidAssetBundle()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        AssetBundleBuild.BuildAssetBundles();
    }
    [MenuItem("AssetBundle/Batch/Build IOS AssetBundle")]
    public static void BuildIOSAssetBundle()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
        AssetBundleBuild.BuildAssetBundles();
    }
     [MenuItem("AssetBundle/Batch/Build Windows PC AssetBundle")]
    public static void BuildWindowsPCAssetBundle()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
        AssetBundleBuild.BuildAssetBundles();
    }

    [MenuItem("AssetBundle/Batch/Build Android Player")]
    public static void BuildAndroidPlayer()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        AssetBundleBuild.BuildPlayer();
    }
    [MenuItem("AssetBundle/Batch/Build IOS Player")]
    public static void BuildIOSPlayer()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
        AssetBundleBuild.BuildPlayer();
    }
    [MenuItem("AssetBundle/Batch/Build WindowsPC Player")]
    public static void BuildWindowsPCPlayer()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
        AssetBundleBuild.BuildPlayer();
    }


    public static void BuildLight()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
       
    }
}
