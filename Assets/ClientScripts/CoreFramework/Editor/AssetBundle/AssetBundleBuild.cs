using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System;
using System.Xml;

public class AssetBundleBuild
{
   

    public static void BuildAssetBundles()
    {
        string fullpath = AssetBundlePlatformPathManager.GetAssetBundleStorePath();

        if (!Directory.Exists(fullpath))
            Directory.CreateDirectory(fullpath);

        BuildPipeline.BuildAssetBundles(fullpath, 0, EditorUserBuildSettings.activeBuildTarget);
    }

    public static void BuildPlayer()
    {
        //var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
        var outputPath = AssetBundlePlatformPathManager.GetAppOutputPath();
         if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        if (outputPath.Length == 0)
            return;

        string[] levels = GetLevelsFromBuildSettings();
        if (levels.Length == 0)
        {
            Debug.Log("Nothing to build.");
            return;
        }

        string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
        if (targetName == null)
            return;

        // Build and copy AssetBundles.
        //BuildAssetBundles();
        //CopyAssetBundlesTo(Application.streamingAssetsPath + AssetBundlePlatformPathManager.kAssetBundlesPath);

        //RunBat(System.Environment.CurrentDirectory + "/../Make_Design.bat");

        BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;

        //Android签名
        //PlayerSettings.Android.keystoreName = Application.dataPath + "/../public.keystore";
        //PlayerSettings.Android.keystorePass = "12345678";
        //PlayerSettings.Android.keyaliasPass = "12345678";

        //if (PlayerSettings.productName == "Aoni Cam")
        //{
        //    PlayerSettings.Android.keyaliasName = "com.pi.aonican";
        //}

        //if (PlayerSettings.productName == "RZ")
        //{
        //    PlayerSettings.Android.keyaliasName = "com.pi.rz";
        //}

        //if (PlayerSettings.productName == "USB360")
        //{
        //    PlayerSettings.Android.keyaliasName = "com.pi.usb";
        //}

        //if (PlayerSettings.productName == "UVR360")
        //{
        //    PlayerSettings.Android.keyaliasName = "com.pi.urbetter";
        //}

        PlayerSettings.Android.keystoreName = BuildTools.keystoreName;
        PlayerSettings.Android.keystorePass = BuildTools.keystorePass;
        PlayerSettings.Android.keyaliasName = BuildTools.keyaliasName;
        PlayerSettings.Android.keyaliasPass = BuildTools.keyaliasPass;

        PlayerSettings.iOS.appleEnableAutomaticSigning = false;

        option = BuildOptions.None;
        //生成版本号文件
        GenerateVersionFile();

        string outputFile = outputPath + targetName;
        BuildPipeline.BuildPlayer(levels, outputFile, EditorUserBuildSettings.activeBuildTarget, option);
    }

    public static string GetBuildTargetName(BuildTarget target)
    {
        DateTime dt = DateTime.Now;
        var outputFile = dt.ToString("yyyy-MM-dd-HH-mm-ss");
        outputFile = "/App_" + outputFile;
        switch (target)
        {
            case BuildTarget.Android:
                return outputFile+".apk";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return outputFile+".exe";
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                return outputFile+".app";
            case BuildTarget.WebPlayer:
            case BuildTarget.WebPlayerStreamed:
                return "/XCodeProj";
            case BuildTarget.iOS:
                return "/XCodeProj";
            // Add more build targets for your own.
            default:
                Debug.Log("Target not implemented.");
                return null;
        }
    }

    static void CopyAssetBundlesTo(string outputPath)
    {
        // Clear streaming assets folder.
        FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
        Directory.CreateDirectory(outputPath);

        string outputFolder = AssetBundlePlatformPathManager.GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget);

        // Setup the source folder for assetbundles.
        var source = AssetBundlePlatformPathManager.GetAssetBundleStorePath();
      
        if (!System.IO.Directory.Exists(source))
            Debug.Log("No assetBundle output folder, try to build the assetBundles first.");

        // Setup the destination folder for assetbundles.
        var destination =  outputPath + outputFolder;
        if (System.IO.Directory.Exists(destination))
            FileUtil.DeleteFileOrDirectory(destination);

        FileUtil.CopyFileOrDirectory(source, destination);
    }

    static string[] GetLevelsFromBuildSettings()
    {
        List<string> levels = new List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
        {
            if (EditorBuildSettings.scenes[i].enabled)
                levels.Add(EditorBuildSettings.scenes[i].path);
        }

        return levels.ToArray();
    }

  

    //每次编译时生成一个版本号
    public static void GenerateVersionFile()
    {
        DateTime dt = DateTime.Now;
        var timestring = dt.Ticks.ToString();// ToString("yyyy-MM-dd-HH-mm-ss");

        string versionfilepath = Application.streamingAssetsPath + "/version.bytes";

        if (File.Exists(versionfilepath))
        {
            File.Delete(versionfilepath);
        }
        FileStream fs = new FileStream(versionfilepath,FileMode.CreateNew);

        System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();
        byte[] inputBytes = converter.GetBytes(timestring);


        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(inputBytes);
        bw.Close();
        fs.Close();



    }







#region Shell
    private static void RunBat(string batPath)
    {
        Process pro = new Process();

        FileInfo file = new FileInfo(batPath);
        pro.StartInfo.WorkingDirectory = file.Directory.FullName;
        pro.StartInfo.FileName = batPath;
        pro.StartInfo.CreateNoWindow = false;
        pro.Start();
        pro.WaitForExit();
    }
#endregion
}
