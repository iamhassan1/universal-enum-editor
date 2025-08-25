using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;

public struct ScriptInfo
{
    public string Path;
    public string AssemblyName;
}

public static class EnumScriptDatabase
{
    private static List<ScriptInfo> cachedScriptInfos;
    public static bool IsReady => cachedScriptInfos != null;
    public static bool IsScanning { get; private set; }

    private static int totalScripts;
    private static int processedScripts;

    public static List<ScriptInfo> GetEnumScriptInfos()
    {
        return IsReady ? cachedScriptInfos : new List<ScriptInfo>();
    }

    public static List<string> GetAssemblyNames()
    {
        if (!IsReady) return new List<string>();
        return cachedScriptInfos.Select(s => s.AssemblyName).Distinct().OrderBy(name => name).ToList();
    }

    public static void StartScan()
    {
        if (IsScanning) return;

        IsScanning = true;
        cachedScriptInfos = new List<ScriptInfo>();
        processedScripts = 0;

        // --- THIS IS THE FIX ---
        // We now tell AssetDatabase to search ONLY in the "Assets" folder, ignoring all packages.
        string[] allScriptGUIDs = AssetDatabase.FindAssets("t:Script", new[] { "Assets" });
        totalScripts = allScriptGUIDs.Length;

        // The rest of the logic can remain the same, as it now operates on a much smaller list of scripts.
        Task.Run(() =>
        {
            var foundInfos = new List<ScriptInfo>();
            var enumDefinitionRegex = new Regex(@"(?:\[.*?\]\s*)*(?:public|internal|private)?\s*\benum\s+[a-zA-Z_][a-zA-Z0-9_]*\s*{");

            // This temporary list will be populated on the main thread
            var scriptDetails = new List<(string guid, string path)>();

            // We still need to get paths on the main thread
            UnityEditor.EditorApplication.delayCall += () => {
                for (int i = 0; i < allScriptGUIDs.Length; i++)
                {
                    scriptDetails.Add((allScriptGUIDs[i], AssetDatabase.GUIDToAssetPath(allScriptGUIDs[i])));
                }
            };

            // The background task now processes this pre-fetched list
            Task.Run(async () => {
                // Wait until the main thread has fetched all the paths
                while (scriptDetails.Count < totalScripts) await Task.Delay(100);

                for (int i = 0; i < totalScripts; i++)
                {
                    string path = scriptDetails[i].path;
                    try
                    {
                        string content = File.ReadAllText(path);
                        if (enumDefinitionRegex.IsMatch(content))
                        {
                            // We need to jump back to the main thread to get assembly info
                            UnityEditor.EditorApplication.delayCall += () => {
                                MonoScript scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                                if (scriptAsset != null)
                                {
                                    Type classType = scriptAsset.GetClass();
                                    string assemblyName = classType != null ? classType.Assembly.GetName().Name : "Assembly-CSharp";
                                    lock (foundInfos)
                                    {
                                        foundInfos.Add(new ScriptInfo { Path = path, AssemblyName = assemblyName });
                                    }
                                }
                            };
                        }
                    }
                    catch (Exception) { /* Ignore */ }
                    finally { processedScripts++; }
                }

                // Wait until all main-thread calls for assembly info are likely done
                while (processedScripts < totalScripts) await Task.Delay(100);

                // Final delay to ensure last few delayCalls execute
                await Task.Delay(200);

                FinalizeScan(foundInfos);
            });
        });

        EditorApplication.update += UpdateScanProgress;
    }

    private static void FinalizeScan(List<ScriptInfo> foundInfos)
    {
        foundInfos.Sort((a, b) => a.Path.CompareTo(b.Path));
        cachedScriptInfos = foundInfos;
        IsScanning = false;
    }

    private static void UpdateScanProgress()
    {
        if (IsScanning)
        {
            float progress = totalScripts > 0 ? (float)processedScripts / totalScripts : 1f;
            EditorUtility.DisplayProgressBar("Scanning Scripts", $"Discovering script details {processedScripts}/{totalScripts}...", progress);
        }
        else
        {
            EditorUtility.ClearProgressBar();
            EditorApplication.update -= UpdateScanProgress;
            if (EditorWindow.HasOpenInstances<EnumPickerWindow>())
            {
                EditorWindow.GetWindow<EnumPickerWindow>().RefreshLists();
            }
        }
    }
}