using Harmony12;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityModManagerNet;

namespace Sth4nothing.DynamicExecutor
{
    public class Settings : UnityModManager.ModSettings
    {
        public string msbuildPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MsBuild.exe";
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }

    public class Main
    {
        public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
        public static Settings Setting { get; private set; }
        public static bool Enabled { get; private set; }
        private static bool running = false;
        private static int count = 0;

        private static readonly string[] files =
        {
            "Execute.cs.template",
            "Execute.csproj.template",
            "AssemblyInfo.cs.template",
            "Execute.csproj.user.template",
        };

        private static string rootPath;

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (!value)
            {
                return false;
            }
            Enabled = true;
            return true;
        }

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            rootPath = modEntry.Path;
            Logger = modEntry.Logger;
            Setting = Settings.Load<Settings>(modEntry);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            HarmonyInstance.Create(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("msbuild·����");
            Setting.msbuildPath = GUILayout.TextField(Setting.msbuildPath);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("�򿪴���·��", GUILayout.Width(100)))
            {
                var p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "explorer.exe";
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WorkingDirectory = rootPath;
                p.StartInfo.Arguments = "/e,/select,\"" + Path.Combine(rootPath, "Execute.cs.template") + "\"";
                p.Start();
            }
            if (!running && DateFile.instance != null && DateFile.instance.actorsDate != null && DateFile.instance.actorsDate.ContainsKey(DateFile.instance.mianActorId))
            {
                if (GUILayout.Button("���д���", GUILayout.Width(100)))
                {
                    Execute();
                }
            }
            GUILayout.EndHorizontal();
        }

        public static void Execute()
        {
            foreach (var file in files)
            {
                if (!File.Exists(Path.Combine(rootPath, file)))
                {
                    Logger.Log($"�������ļ��� " + file);
                    return;
                }
            }

            running = true;

            try
            {
                var cs = File.ReadAllText(Path.Combine(rootPath, "Execute.cs.template"));
                File.WriteAllText(Path.Combine(rootPath, "Execute.cs"), cs.Replace("Execute", "Execute" + count));

                // TODO: �������mod������
                var csproj = File.ReadAllText(Path.Combine(rootPath, "Execute.csproj.template"));
                File.WriteAllText(Path.Combine(rootPath, "Execute.csproj"),
                    csproj.Replace("<AssemblyName>Execute</AssemblyName>", $"<AssemblyName>Execute{count}</AssemblyName>"));

                File.Copy(Path.Combine(rootPath, "AssemblyInfo.cs.template"),
                    Path.Combine(rootPath, "AssemblyInfo.cs"), true);

                var user = File.ReadAllText(Path.Combine(rootPath, "Execute.csproj.user.template"));
                File.WriteAllText(Path.Combine(rootPath, "Execute.csproj.user"),
                    user.Replace("%GAMEPATH%", Directory.GetParent(UnityModManager.modsPath).FullName));

                var p = new System.Diagnostics.Process();
                p.StartInfo.FileName = Setting.msbuildPath;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WorkingDirectory = rootPath;
                p.StartInfo.Arguments = "/m /p:Configuration=Release Execute.csproj";

                p.Start();
                p.WaitForExit();

                if (!File.Exists(Path.Combine(rootPath, $"Execute{count}.dll")))
                {
                    Logger.Error("����ʧ��");
                }
                else
                {
                    var ass = Assembly.LoadFile(Path.Combine(rootPath, $"Execute{count}.dll"));
                    var execute = ass.CreateInstance("Sth4nothing.Execute" + count);
                    var method = execute.GetType().GetMethod("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    var ans = method.Invoke(execute, null);
                    if (ans != null)
                    {
                        Logger.Log($"��������: {ans.GetType()}\n���ؽ��:\n{ans}");
                    }
                    else
                    {
                        Logger.Log("null");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"{e.Message}\n{e.StackTrace}\n{e.TargetSite}");
            }
            finally
            {
                running = false;
                count++;
            }
        }

        public static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Setting.Save(modEntry);
        }
    }
}