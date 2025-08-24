using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MHSTools.UniversalEnumEditor
{
    public class EnumPickerWindow : EditorWindow
    {
        public static Action<string> OnScriptSelected;

        private List<ScriptInfo> allScriptInfos;
        private List<ScriptInfo> filteredScriptInfos;

        private string nameSearchQuery = "";
        private string pathSearchQuery = "";

        private Vector2 scrollPosition;
        private Color hoverColor;

        private bool showFilters = true;
        private List<string> availableAssemblies;
        private int assemblyMask = -1;

        public static void ShowWindow()
        {
            EnumPickerWindow window = GetWindow<EnumPickerWindow>(true, "Select Script with Enum", true);
            window.minSize = new Vector2(400, 450);
        }

        private void OnEnable()
        {
            if (!EnumScriptDatabase.IsReady) { EnumScriptDatabase.StartScan(); }
            RefreshLists();
            hoverColor = EditorGUIUtility.isProSkin
                ? new Color(0.3f, 0.3f, 0.3f, 1f)
                : new Color(0.7f, 0.7f, 0.7f, 1f);
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawFilters();
            DrawScriptList();
            DrawFooter();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(5, 5, 5, 5) });
            EditorGUILayout.LabelField("Find Script", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            nameSearchQuery = EditorGUILayout.TextField(nameSearchQuery, GUI.skin.FindStyle("SearchTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("SearchCancelButton"))) { nameSearchQuery = ""; GUI.FocusControl(null); }
            if (EditorGUI.EndChangeCheck()) { FilterScripts(); }

            using (new EditorGUI.DisabledScope(EnumScriptDatabase.IsScanning))
            {
                if (GUILayout.Button(new GUIContent("Refresh", EditorGUIUtility.IconContent("d_Refresh").image), GUILayout.Width(90)))
                {
                    EnumScriptDatabase.StartScan();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawFilters()
        {
        
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            showFilters = EditorGUILayout.Foldout(showFilters, "Filters", true);
            EditorGUILayout.EndHorizontal();

            if (showFilters)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Filter by Path Contains:", EditorStyles.miniBoldLabel);
                pathSearchQuery = EditorGUILayout.TextField(pathSearchQuery);

                if (availableAssemblies != null && availableAssemblies.Count > 0)
                {
                    assemblyMask = EditorGUILayout.MaskField(
                        new GUIContent("Filter by Assembly", "Select which assemblies to include in the search."),
                        assemblyMask,
                        availableAssemblies.ToArray());
                }

                if (GUI.changed) { FilterScripts(); }

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawScriptList()
        {
            if (filteredScriptInfos == null || filteredScriptInfos.Count == 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (EnumScriptDatabase.IsScanning)
                {
                    EditorGUILayout.LabelField("Scanning scripts...", EditorStyles.centeredGreyMiniLabel);
                }
                else
                {
                    EditorGUILayout.LabelField("No scripts found matching your criteria.", EditorStyles.centeredGreyMiniLabel);
                }
                EditorGUILayout.EndVertical();
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < filteredScriptInfos.Count; i++)
            {
                ScriptInfo scriptInfo = filteredScriptInfos[i];
                DrawScriptItem(scriptInfo, i);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawScriptItem(ScriptInfo scriptInfo, int index)
        {
            Rect itemRect = GUILayoutUtility.GetRect(0, 60, GUILayout.ExpandWidth(true));
            bool isHovered = itemRect.Contains(Event.current.mousePosition);

            if (Event.current.type == EventType.Repaint)
            {
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = isHovered ? hoverColor : Color.clear;
                EditorGUI.DrawRect(itemRect, GUI.backgroundColor);
                GUI.backgroundColor = originalColor;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(EditorGUIUtility.IconContent("cs Script Icon"), GUILayout.Width(40), GUILayout.Height(40));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(scriptInfo.scriptName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(scriptInfo.assetPath, EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Enums: {string.Join(", ", scriptInfo.enumNames)}", EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Select", GUILayout.Width(60), GUILayout.Height(30)))
            {
                OnScriptSelected?.Invoke(scriptInfo.assetPath);
                Close();
            }
            EditorGUILayout.EndHorizontal();

            if (index < filteredScriptInfos.Count - 1)
            {
                EditorGUILayout.Space(2);
            }
        }

        private void DrawFooter()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", EditorStyles.toolbarButton))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void RefreshLists()
        {
            allScriptInfos = EnumScriptDatabase.GetAllScriptInfos();
            availableAssemblies = EnumScriptDatabase.GetAvailableAssemblies();
            assemblyMask = -1; // Show all assemblies by default
            FilterScripts();
        }

        private void FilterScripts()
        {
            if (allScriptInfos == null) return;

            filteredScriptInfos = allScriptInfos.Where(scriptInfo =>
            {
                bool nameMatch = string.IsNullOrEmpty(nameSearchQuery) ||
                                scriptInfo.scriptName.IndexOf(nameSearchQuery, StringComparison.OrdinalIgnoreCase) >= 0;

                bool pathMatch = string.IsNullOrEmpty(pathSearchQuery) ||
                                scriptInfo.assetPath.IndexOf(pathSearchQuery, StringComparison.OrdinalIgnoreCase) >= 0;

                bool assemblyMatch = assemblyMask == -1 || 
                                   (scriptInfo.assemblyName != null && 
                                    (assemblyMask & (1 << availableAssemblies.IndexOf(scriptInfo.assemblyName))) != 0);

                return nameMatch && pathMatch && assemblyMatch;
            }).ToList();
        }
    }

    [System.Serializable]
    public class ScriptInfo
    {
        public string scriptName;
        public string assetPath;
        public List<string> enumNames;
        public string assemblyName;

        public ScriptInfo(string name, string path, List<string> enums, string assembly)
        {
            scriptName = name;
            assetPath = path;
            enumNames = enums ?? new List<string>();
            assemblyName = assembly;
        }
    }
}
