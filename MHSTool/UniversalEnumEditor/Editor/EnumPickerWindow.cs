using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
        // --- THIS IS THE FIX ---
        // Instead of using the non-standard 'toolbarFoldout', we use the default foldout on a toolbar background.
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
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (EnumScriptDatabase.IsScanning) { EditorGUILayout.HelpBox("Scanning project for enums...", MessageType.Info); }
        else if (filteredScriptInfos.Count == 0) { EditorGUILayout.HelpBox("No matching scripts found.", MessageType.Info); }
        else
        {
            Event currentEvent = Event.current;
            foreach (var scriptInfo in filteredScriptInfos)
            {
                Rect entryRect = EditorGUILayout.BeginVertical();
                EditorGUILayout.Space(2);
                if (entryRect.Contains(currentEvent.mousePosition)) { EditorGUI.DrawRect(entryRect, hoverColor); }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.Label(EditorGUIUtility.IconContent("cs Script Icon"), GUILayout.Width(20), GUILayout.Height(20));
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(Path.GetFileNameWithoutExtension(scriptInfo.Path), EditorStyles.boldLabel);
                EditorGUILayout.LabelField(scriptInfo.Path, EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(2);
                EditorGUILayout.EndVertical();

                if (currentEvent.type == EventType.MouseDown && entryRect.Contains(currentEvent.mousePosition))
                {
                    OnScriptSelected?.Invoke(scriptInfo.Path);
                    this.Close();
                    currentEvent.Use();
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawFooter()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.FlexibleSpace();
        string summary = EnumScriptDatabase.IsScanning
            ? "Scanning..."
            : $"Showing {filteredScriptInfos.Count} of {allScriptInfos.Count} scripts.";
        EditorGUILayout.LabelField(summary, EditorStyles.miniLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    public void RefreshLists()
    {
        allScriptInfos = EnumScriptDatabase.GetEnumScriptInfos();
        availableAssemblies = EnumScriptDatabase.GetAssemblyNames();
        assemblyMask = -1;
        FilterScripts();
        Repaint();
    }

    private void FilterScripts()
    {
        IEnumerable<ScriptInfo> currentFilter = allScriptInfos;

        if (availableAssemblies != null && availableAssemblies.Count > 0)
        {
            var selectedAssemblies = new List<string>();
            for (int i = 0; i < availableAssemblies.Count; i++)
            {
                if ((assemblyMask & (1 << i)) != 0)
                {
                    selectedAssemblies.Add(availableAssemblies[i]);
                }
            }

            if (selectedAssemblies.Count > 0 && selectedAssemblies.Count < availableAssemblies.Count)
            {
                currentFilter = currentFilter.Where(s => selectedAssemblies.Contains(s.AssemblyName));
            }
        }

        if (!string.IsNullOrEmpty(pathSearchQuery))
        {
            currentFilter = currentFilter.Where(s => s.Path.IndexOf(pathSearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        if (!string.IsNullOrEmpty(nameSearchQuery))
        {
            currentFilter = currentFilter.Where(s => Path.GetFileName(s.Path).IndexOf(nameSearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        filteredScriptInfos = currentFilter.ToList();
    }
}