using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MHSTools.UniversalEnumEditor
{
    public static class EnumScriptDatabase
    {
        private static List<ScriptInfo> allScriptInfos = new List<ScriptInfo>();
        private static List<string> availableAssemblies = new List<string>();
        private static bool isScanning = false;
        private static bool isReady = false;

        public static bool IsReady => isReady;
        public static bool IsScanning => isScanning;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.update += OnEditorUpdate;
            StartScan();
        }

        private static void OnEditorUpdate()
        {
            if (isScanning)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }

        public static void StartScan()
        {
            if (isScanning) return;

            isScanning = true;
            isReady = false;
            allScriptInfos.Clear();
            availableAssemblies.Clear();

            EditorApplication.update += ScanUpdate;
        }

        private static void ScanUpdate()
        {
            try
            {
                ScanScripts();
                isScanning = false;
                isReady = true;
                EditorApplication.update -= ScanUpdate;
                EditorUtility.SetDirty(EditorWindow.GetWindow<UniversalEnumEditor>());
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during script scanning: {e.Message}");
                isScanning = false;
                EditorApplication.update -= ScanUpdate;
            }
        }

        private static void ScanScripts()
        {
            string[] scriptGuids = AssetDatabase.FindAssets("t:TextAsset");
            HashSet<string> assemblySet = new HashSet<string>();

            foreach (string guid in scriptGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!assetPath.EndsWith(".cs")) continue;

                try
                {
                    TextAsset scriptAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                    if (scriptAsset == null) continue;

                    List<string> enumNames = FindEnumsInScript(scriptAsset.text);
                    if (enumNames.Count > 0)
                    {
                        string assemblyName = GetAssemblyName(assetPath);
                        assemblySet.Add(assemblyName);

                        ScriptInfo scriptInfo = new ScriptInfo(
                            scriptAsset.name,
                            assetPath,
                            enumNames,
                            assemblyName
                        );

                        allScriptInfos.Add(scriptInfo);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Error scanning script {assetPath}: {e.Message}");
                }
            }

            availableAssemblies = assemblySet.OrderBy(x => x).ToList();
        }

        private static List<string> FindEnumsInScript(string scriptContent)
        {
            List<string> enumNames = new List<string>();
            
            // Pattern to find enum declarations
            string pattern = @"(?:\[.*?\]\s*)*(?:public|internal|private)?\s*\benum\s+([a-zA-Z_][a-zA-Z0-9_]*)";
            
            MatchCollection matches = Regex.Matches(scriptContent, pattern);
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    string enumName = match.Groups[1].Value;
                    if (!enumNames.Contains(enumName))
                    {
                        enumNames.Add(enumName);
                    }
                }
            }

            return enumNames;
        }

        private static string GetAssemblyName(string assetPath)
        {
            try
            {
                // Get the assembly definition file if it exists
                string directory = Path.GetDirectoryName(assetPath);
                while (!string.IsNullOrEmpty(directory) && directory != "Assets")
                {
                    string[] asmdefFiles = Directory.GetFiles(directory, "*.asmdef");
                    if (asmdefFiles.Length > 0)
                    {
                        string asmdefPath = asmdefFiles[0];
                        string asmdefContent = File.ReadAllText(asmdefPath);
                        
                        // Simple JSON parsing for assembly name
                        Match nameMatch = Regex.Match(asmdefContent, @"""name""\s*:\s*""([^""]+)""");
                        if (nameMatch.Success)
                        {
                            return nameMatch.Groups[1].Value;
                        }
                    }
                    
                    directory = Path.GetDirectoryName(directory);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error getting assembly name for {assetPath}: {e.Message}");
            }

            return "Assembly-CSharp";
        }

        public static List<ScriptInfo> GetAllScriptInfos()
        {
            return new List<ScriptInfo>(allScriptInfos);
        }

        public static List<string> GetAvailableAssemblies()
        {
            return new List<string>(availableAssemblies);
        }

        public static void Refresh()
        {
            StartScan();
        }
    }
}
