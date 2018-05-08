using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;
 
public class AndroidPackage : Editor {
 
	[MenuItem("AssetBundle/Build Google Project")]
	public static void BuildForAndroid()
	{

        //生成版本号文件
        AssetBundleBuild.GenerateVersionFile();


        UnityEngine.Debug.Log ("Begin build");
		string TopFolder = Application.dataPath;
		TopFolder = TopFolder.Replace ("Assets", "");
		//string path = TopFolder +"../../AndroidProj/";
        string path = "d:/AndroidProj";


        if (Directory.Exists(path))
		{
			DeleteFolder (path);
        }
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.ADT;


        string rt = BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
        UnityEngine.Debug.Log(rt);

		string projPath = path +"/" + Application.productName + "/";
        string resPath = path + "/unity-android-resources/";
 
  
		Directory.Move (projPath, path + "/unity");
		projPath = path + "/unity/";
		///检查环境
		//CheckBuildFileExist (projPath);
		//CheckBuildFileExist (resPath);

		Directory.CreateDirectory(resPath+"src/");  //要添加此目录不然会报错

		if(!File.Exists(projPath+"ant.properties"))
		{
			File.Copy(TopFolder+"ant.properties", projPath+"ant.properties");
		}
		if (!File.Exists (projPath + "public.keystore")) {
			File.Copy (TopFolder + "public.keystore", projPath + "public.keystore");
		}
 		
		CopyDirectory (TopFolder + "Assets/StreamingAssets", projPath + "assets");
	}

 




	static void CheckBuildFileExist(string path)
	{
		if (!File.Exists (path + "build.xml")) {
		
			string argument = "update project --path "+path;
			UnityEngine.Debug.Log("argument:"+argument);
			processCommand("android",argument);
		}

	}
		

	static string[] GetBuildScenes()
	{
		List<string> names = new List<string>();
		foreach(EditorBuildSettingsScene e in EditorBuildSettings.scenes)
		{
			if(e==null)
				continue;
            
#if FINAL_DISTRIBUTION
          if (e.enabled && e.path.IndexOf("SwitchNet") >= 0)
                continue;
           if(e.enabled)
				names.Add(e.path);
#else

            if (e.enabled)
                names.Add(e.path);
#endif 
		}
		return names.ToArray();
	}

	static void processCommand(string command, string argument){
		ProcessStartInfo start = new ProcessStartInfo(command);
		start.Arguments = argument;
		start.CreateNoWindow = false;
		start.ErrorDialog = true;
		start.UseShellExecute = true;
		
		if(start.UseShellExecute){
			start.RedirectStandardOutput = false;
			start.RedirectStandardError = false;
			start.RedirectStandardInput = false;
		} else{
			start.RedirectStandardOutput = true;
			start.RedirectStandardError = true;
			start.RedirectStandardInput = true;
			start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
			start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
		}
		
		Process p = Process.Start(start);
		
		if (!start.UseShellExecute) {
			printOutPut (p.StandardOutput);
			printOutPut (p.StandardError);
		} 
	 
		p.WaitForExit();
		p.Close();
	}

	static void printOutPut(StreamReader reader)
	{
		UnityEngine.Debug.Log (reader.ReadToEnd ());
	}


	
	public static void DeleteFolder(string dir)
	{
		foreach (string d in Directory.GetFileSystemEntries(dir))
		{
			if (File.Exists(d))
			{
				FileInfo fi = new FileInfo(d);
				if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
					fi.Attributes = FileAttributes.Normal;
				File.Delete(d);
			}
			else
			{
				DirectoryInfo d1 = new DirectoryInfo(d);

				if (d1.GetFiles().Length != 0 || d1.GetDirectories().Length!=0)
				{
					DeleteFolder(d1.FullName);////递归删除子文件夹
				}
				Directory.Delete(d);
			}
		}
	}
	
	public static void CopyDirectory(string sourcePath, string destinationPath)
	{
		DirectoryInfo info = new DirectoryInfo(sourcePath);
		Directory.CreateDirectory(destinationPath);
		foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
		{
			string destName = Path.Combine(destinationPath, fsi.Name);
			if (fsi is System.IO.FileInfo)
            {
                if(!File.Exists(destName))
                {

                    File.Copy(fsi.FullName, destName);
                }

            }    
			else                     
			{
                if (!Directory.Exists(destName))
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                }
			}
		}
	}
}
