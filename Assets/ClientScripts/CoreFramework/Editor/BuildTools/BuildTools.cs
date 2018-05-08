using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class BuildTools
{
    #region android Sign
    public static string keystoreName
    {
        get
        {
            string s = PlayerPrefs.GetString("keystoreName");
            return s;
        }
        set
        {
            PlayerPrefs.SetString("keystoreName", value);
            PlayerPrefs.Save();
        }
    }
    public static string keystorePass
    {
        get
        {
            string s = PlayerPrefs.GetString("keystorePass");
            return s;
        }
        set
        {
            PlayerPrefs.SetString("keystorePass", value);
            PlayerPrefs.Save();
        }
    }
    public static string keyaliasName
    {
        get
        {
            string s = PlayerPrefs.GetString("keyaliasName");
            return s;
        }
        set
        {
            PlayerPrefs.SetString("keyaliasName", value);
            PlayerPrefs.Save();
        }
    }
    public static string keyaliasPass
    {
        get
        {
            string s = PlayerPrefs.GetString("keyaliasPass");
            return s;
        }
        set
        {
            PlayerPrefs.SetString("keyaliasPass", value);
            PlayerPrefs.Save();
        }
    }
    static void SetProjectName(string projName)
    {
        PlayerPrefs.SetString("ProjName", projName);
        PlayerPrefs.Save();
    }
    public static string GetProjectName()
    {
        return PlayerPrefs.GetString("ProjName", "");
    }

    public static void SetSign(string keystore = "", string keystorepw = "", string keyalias = "", string keyaliaspw = "")
    {
        if (keystore.IsNullOrEmpty())
        {
            keystore = Application.dataPath + "/../public.keystore";
        }
        if (keystorepw.IsNullOrEmpty())
        {
            keystorepw = "12345678";
        }
        if (keyalias.IsNullOrEmpty())
        {
            keyalias = "pisoftware";
        }
        if (keyaliaspw.IsNullOrEmpty())
        {
            keyaliaspw = "12345678";
        }

        keystoreName = keystore;
        keystorePass = keystorepw;
        keyaliasName = keyalias;
        keyaliasPass = keyaliaspw;

        PlayerSettings.Android.keystoreName = BuildTools.keystoreName;
        PlayerSettings.Android.keystorePass = BuildTools.keystorePass;
        PlayerSettings.Android.keyaliasName = BuildTools.keyaliasName;
        PlayerSettings.Android.keyaliasPass = BuildTools.keyaliasPass;
    }
    #endregion

    [MenuItem("BuildTools/DEMO")]
    public static void DEMO()
    {
        SetProjectName("Demo");
        AssetBundleMenu.RemoveAllSymbol();
        AssetBundleMenu.AddSymbol("PISDKDEMO");

        List<EditorBuildSettingsScene> scenelist = new List<EditorBuildSettingsScene>();
        scenelist.Add(new EditorBuildSettingsScene("Assets/Scenes/AppMain_SDKDEMO.unity", true));
        EditorBuildSettings.scenes = scenelist.ToArray();

        var outputPath = AssetBundlePlatformPathManager.GetAppOutputPath();
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        if (outputPath.Length == 0)
            return;


        BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;



        PlayerSettings.Android.keystoreName = BuildTools.keystoreName;
        PlayerSettings.Android.keystorePass = BuildTools.keystorePass;
        PlayerSettings.Android.keyaliasName = BuildTools.keyaliasName;
        PlayerSettings.Android.keyaliasPass = BuildTools.keyaliasPass;

        PlayerSettings.iOS.appleEnableAutomaticSigning = false;

        option = BuildOptions.None;

        string outputFile = outputPath + "SDKDemo.apk";

        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputFile, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
    }
    [MenuItem("BuildTools/GV")]
    public static void GV()
    {
        SetProjectName("GV");
        List<EditorBuildSettingsScene> scenelist = new List<EditorBuildSettingsScene>();
        scenelist.Add(new EditorBuildSettingsScene("Assets/Scenes/AppMain_GVSDK.unity", true));
        EditorBuildSettings.scenes = scenelist.ToArray();


        SDKGEN();
    }
    [MenuItem("BuildTools/BL")]
    public static void BL()
    {
        SetProjectName("BL");
        List<EditorBuildSettingsScene> scenelist = new List<EditorBuildSettingsScene>();
        scenelist.Add(new EditorBuildSettingsScene("Assets/Scenes/AppMain_BLSDK.unity", true));
        EditorBuildSettings.scenes = scenelist.ToArray();


        SDKGEN();
    }
    [MenuItem("BuildTools/GX")]
    public static void GX()
    {
        SetProjectName("GX");
        List<EditorBuildSettingsScene> scenelist = new List<EditorBuildSettingsScene>();
        scenelist.Add(new EditorBuildSettingsScene("Assets/Scenes/AppMain_GXSDK.unity", true));
        EditorBuildSettings.scenes = scenelist.ToArray();


        SDKGEN();
    }
    [MenuItem("BuildTools/Level1")]
    public static void Level1()
    {
        SetProjectName("Level1");
        List<EditorBuildSettingsScene> scenelist = new List<EditorBuildSettingsScene>();
        scenelist.Add(new EditorBuildSettingsScene("Assets/Scenes/AppMain_SDK.unity", true));
        EditorBuildSettings.scenes = scenelist.ToArray();

        

        SDKGEN();
        
        AssetBundleMenu.AddSymbol("SDKLEVEL_1");
    }
  
    [MenuItem("BuildTools/Level2")]
    public static void Level2()
    {
        SetProjectName("Level2");
        List<EditorBuildSettingsScene> scenelist = new List<EditorBuildSettingsScene>();
        scenelist.Add(new EditorBuildSettingsScene("Assets/Scenes/AppMain_SDK.unity", true));
        EditorBuildSettings.scenes = scenelist.ToArray();



        SDKGEN();

        AssetBundleMenu.AddSymbol("SDKLEVEL_2");
    }

    [MenuItem("BuildTools/LevelMax")]
    public static void LevelMax()
    {
        SetProjectName("LevelMax");
        List<EditorBuildSettingsScene> scenelist = new List<EditorBuildSettingsScene>();
        scenelist.Add(new EditorBuildSettingsScene("Assets/Scenes/AppMain_SDK.unity", true));
        EditorBuildSettings.scenes = scenelist.ToArray();



        SDKGEN();

        AssetBundleMenu.AddSymbol("SDKLEVEL_MAX");
    }

    [MenuItem("BuildTools/Acer")]
    public static void Acer()
    {
        SetProjectName("Acer");
        List<EditorBuildSettingsScene> scenelist = new List<EditorBuildSettingsScene>();
        scenelist.Add(new EditorBuildSettingsScene("Assets/Scenes/AppMain_AcerSDK.unity", true));
        EditorBuildSettings.scenes = scenelist.ToArray();

        SDKGEN();
    }

    [MenuItem("BuildTools/SetTargetAndroid")]
    public static void SetTargetAndroid()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
    }
    [MenuItem("BuildTools/SetTargetiOS")]
    public static void SetTargetiOS()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
    }

    public static void SDKGEN()
    {

        AssetBundleMenu.RemoveAllSymbol();
        AssetBundleMenu.AddSymbol("PISDK");

        string dv = "SDK";
        string bundleid = "com.pi.unitysdk";
        
#if UNITY_5_6
        PlayerSettings.applicationIdentifier = bundleid;
#else
        PlayerSettings.bundleIdentifier = bundleid;
#endif
        PlayerSettings.productName = "SDK";

        SetSign("", "", bundleid, "");

        
        var outputPath = AssetBundlePlatformPathManager.GetAppOutputPath();
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        if (outputPath.Length == 0)
            return;

 
        BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;



        PlayerSettings.Android.keystoreName = BuildTools.keystoreName;
        PlayerSettings.Android.keystorePass = BuildTools.keystorePass;
        PlayerSettings.Android.keyaliasName = BuildTools.keyaliasName;
        PlayerSettings.Android.keyaliasPass = BuildTools.keyaliasPass;

        PlayerSettings.iOS.appleEnableAutomaticSigning = false;

        option = BuildOptions.None;

        string outputFile = outputPath + "SDK";
        
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputFile, EditorUserBuildSettings.activeBuildTarget, BuildOptions.AcceptExternalModificationsToPlayer);
    }
   
  

#region Helper Functions
    

    static void DeleteFile(string file)
    {
        //去除文件夹和子文件的只读属性
        //去除文件夹的只读属性
        System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
        fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
        //去除文件的只读属性
        System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
        //判断文件夹是否还存在
        if (Directory.Exists(file))
        {
            foreach (string f in Directory.GetFileSystemEntries(file))
            {

                if (!f.Contains(".svn") && !f.Contains(".meta"))
                {
                    if (File.Exists(f))
                    {
                        //如果有子文件删除文件
                        File.Delete(f);
                    }
                    else
                    {
                        //循环递归删除子文件夹 
                        DeleteFile(f);
                    }

                }


            }
            //删除空文件夹 
            try
            {

            }
            catch(Exception e)
            {
                Directory.Delete(file);

            }
        }
    }
    static void CopyPictures(string dv)
    {
        string from = Application.dataPath + "/../../Design/Library/Hardware/Series/" + dv + "/UI";
        string to = Application.dataPath + "/ArtRes/2D/PIUI/UI";
        if (!from.Contains(".svn") && !to.Contains(".svn") && !to.Contains(".meta"))
        {
            if (Directory.Exists(to))
            {
                DeleteFile(to);
            }
            try
            {

                CopyDirectory(from, to, true);

            }
            catch (Exception ex)
            {
                Debug.LogError(ex);

            }
        }


        AssetDatabase.Refresh();
    }

    private static bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting)
    {
        bool ret = false;
        try
        {
            SourcePath = SourcePath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? SourcePath : SourcePath + Path.DirectorySeparatorChar;
            DestinationPath = DestinationPath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? DestinationPath : DestinationPath + Path.DirectorySeparatorChar;

            if (Directory.Exists(SourcePath))
            {
                if (Directory.Exists(DestinationPath) == false)
                    Directory.CreateDirectory(DestinationPath);

                foreach (string fls in Directory.GetFiles(SourcePath))
                {
                    FileInfo flinfo = new FileInfo(fls);

                    if(!flinfo.FullName.Contains(".svn"))
                    {
                        flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                    }
                    else
                    {
                        Debug.Log("SVN Files");
                    }
                }
                foreach (string drs in Directory.GetDirectories(SourcePath))
                {
                    DirectoryInfo drinfo = new DirectoryInfo(drs);
                    if(!drinfo.FullName.Contains(".svn"))
                    {
                        if (CopyDirectory(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;

                    }
                    else
                    {
                        Debug.Log("SVN Files");
                    }

                }
            }
            ret = true;
        }
        catch (Exception ex)
        {
            ret = false;
        }
        return ret;
    }
    private static void EmptyFolder(string SourcePath)
    {
        DirectoryInfo dir = new DirectoryInfo(SourcePath);
        EmptyFolder(dir);
    }
    private static void EmptyFolder(DirectoryInfo directory)

    {

        foreach (FileInfo file in directory.GetFiles())

        {
            if(file.FullName.Contains(".svn"))
            {

            }
            else
            {
                try
                {
                    file.Delete();

                }
                catch(Exception e)
                {

                }

            }

        }

        foreach (DirectoryInfo subdirectory in directory.GetDirectories())

        {

            EmptyFolder(subdirectory);
            if (subdirectory.FullName.Contains(".svn"))
            {

            }
            else
            {
                try
                {
                    subdirectory.Delete();
                }
                catch (Exception e)
                {

                }
            }

        }

    }
#endregion
}
