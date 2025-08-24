using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
namespace MHSTools.UniversalEnumEditor
{
    public class UniversalEnumEditor : EditorWindow
    {
        private TextAsset enumScript;
        private List<string> foundEnumNames = new List<string>();
        private int selectedEnumIndex = -1;
        private string selectedEnumName = "";
        private List<string> editableEnums = new List<string>();
        private List<string> originalEnums = new List<string>();
        private List<string> filteredEnums = new List<string>();
        private string newEnumValue = "";
        private string searchQuery = "";
        private Vector2 scrollPosition;
        private bool sortAlphabetically = false;

        private const string EnumScriptKey = "UniversalEnumEditor_ScriptPath";
        private GUIStyle headerStyle, boxStyle, searchFieldStyle, searchCancelButtonStyle, dropZoneStyle, assignedScriptBoxStyle;

        [MenuItem("Tools/Universal Enum Editor")]
        public static void ShowWindow()
        {
            GetWindow<UniversalEnumEditor>("Enum Editor");
        }

        private void OnEnable()
        {
            EnumPickerWindow.OnScriptSelected += HandleScriptSelectedFromPicker;
            string path = EditorPrefs.GetString(EnumScriptKey, "");
            if (!string.IsNullOrEmpty(path))
            {
                if (AssetDatabase.LoadAssetAtPath<TextAsset>(path) is TextAsset script)
                {
                    AssignNewScript(script);
                }
            }
        }

        private void OnDisable()
        {
            EnumPickerWindow.OnScriptSelected -= HandleScriptSelectedFromPicker;
        }

        private void HandleScriptSelectedFromPicker(string assetPath)
        {
            TextAsset script = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            if (script != null)
            {
                AssignNewScript(script);
                Repaint();
            }
        }

        private void OnGUI()
        {
            InitializeStyles();
            if (enumScript == null)
            {
                HandleDragAndDrop(new Rect(0, 0, position.width, position.height));
            }

            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 10, 10, 10) });
            EditorGUILayout.LabelField("Enum C# Script", headerStyle);
            DrawEnumScriptSelector();
            EditorGUILayout.Space(10);

            if (enumScript == null)
            {
                EditorGUILayout.HelpBox("Select, drag-and-drop, or find a C# script to begin.", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            DrawEnumSelectionUI();
            if (!string.IsNullOrEmpty(selectedEnumName))
            {
                EditorGUILayout.LabelField($"Editing Enum: '{selectedEnumName}'", headerStyle);
                EditorGUILayout.BeginVertical(boxStyle);
                DrawSearchAndSortControls();
                DrawEnumList();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField("Add New Value", headerStyle);
                EditorGUILayout.BeginVertical(boxStyle);
                DrawAddNewEnumControls();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField("Import / Export", headerStyle);
                EditorGUILayout.BeginVertical(boxStyle);
                DrawToolsControls();
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();
                DrawValidationMessages();
                DrawUpdateAndCancelButtons();
            }
            else if (foundEnumNames.Count == 0 && enumScript != null)
            {
                EditorGUILayout.HelpBox("No public enums found in the selected script.", MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
        }

        private string[] GetCurrentEnumValues(TextAsset script, string enumName)
        {
            string scriptContent = script.text;

            string pattern = @"\benum\s+" + enumName + @"\s*\{([\s\S]*?)\}";
            Match match = Regex.Match(scriptContent, pattern);

            if (!match.Success || match.Groups.Count < 2)
            {
                Debug.LogError($"Could not find enum block for '{enumName}' in script. The parsing logic failed.");
                return new string[0];
            }

            string enumContent = match.Groups[1].Value;
            return enumContent.Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v) && !v.StartsWith("//"))
                .Select(v => v.Split('=')[0].Trim())
                .Where(v => IsValidEnumName(v))
                .ToArray();
        }

        private void UpdateEnumFile(TextAsset script, string enumName, string[] newValues)
        {
            string path = AssetDatabase.GetAssetPath(script);
            string originalContent = File.ReadAllText(path);

            // Find the enum block again.
            string pattern = @"(\benum\s+" + enumName + @"\s*\{)([\s\S]*?)(\})";
            Match match = Regex.Match(originalContent, pattern);

            if (!match.Success)
            {
                EditorUtility.DisplayDialog("Error", "Could not find the enum block to update. The file might have changed.", "OK");
                return;
            }

            var newEnumContent = new System.Text.StringBuilder();
            newEnumContent.AppendLine();
            foreach (var value in newValues)
            {
                newEnumContent.Append("        ").AppendLine($"{value},");
            }
            newEnumContent.Append("    ");

            string newFileContent = originalContent.Replace(match.Value, $"{match.Groups[1].Value}{newEnumContent}{match.Groups[3].Value}");

            File.WriteAllText(path, newFileContent);
        }

        #region Unchanged and Minor Methods
        private void DrawEnumScriptSelector() { if (enumScript == null) { Rect dropArea = GUILayoutUtility.GetRect(0.0f, 60.0f, GUILayout.ExpandWidth(true)); var buttonContent = new GUIContent("Click to Find Script or Drag Anywhere", EditorGUIUtility.IconContent("d_Search Icon").image); if (GUI.Button(dropArea, buttonContent, EditorStyles.helpBox)) { EnumPickerWindow.ShowWindow(); } } else { EditorGUILayout.BeginVertical(assignedScriptBoxStyle); EditorGUILayout.BeginHorizontal(); GUILayout.Label(EditorGUIUtility.IconContent("cs Script Icon"), GUILayout.Width(40), GUILayout.Height(40)); EditorGUILayout.BeginVertical(); EditorGUILayout.LabelField(new GUIContent(enumScript.name, "Click to select a different script"), EditorStyles.boldLabel); EditorGUILayout.LabelField(new GUIContent(AssetDatabase.GetAssetPath(enumScript)), EditorStyles.miniLabel); EditorGUILayout.EndVertical(); Rect clickableArea = GUILayoutUtility.GetLastRect(); if (Event.current.type == EventType.MouseDown && clickableArea.Contains(Event.current.mousePosition)) { EditorGUIUtility.ShowObjectPicker<TextAsset>(enumScript, false, "", GUIUtility.GetControlID(FocusType.Passive)); Event.current.Use(); } if (Event.current.commandName == "ObjectSelectorUpdated") { if (EditorGUIUtility.GetObjectPickerObject() is TextAsset selected) AssignNewScript(selected); Event.current.Use(); } GUILayout.FlexibleSpace(); if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_ViewToolZoom").image, "Ping Script"), GUILayout.Width(30), GUILayout.Height(30))) { EditorGUIUtility.PingObject(enumScript); } if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_Toolbar Minus").image, "Clear"), GUILayout.Width(30), GUILayout.Height(30))) { AssignNewScript(null); } EditorGUILayout.EndHorizontal(); EditorGUILayout.EndVertical(); } }
        private void HandleDragAndDrop(Rect dropArea) { Event currentEvent = Event.current; if (dropArea.Contains(currentEvent.mousePosition)) { if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform) { if (DragAndDrop.objectReferences.Any(obj => obj is TextAsset)) { DragAndDrop.visualMode = DragAndDropVisualMode.Copy; } else { DragAndDrop.visualMode = DragAndDropVisualMode.Rejected; } if (currentEvent.type == EventType.DragPerform) { DragAndDrop.AcceptDrag(); TextAsset droppedAsset = DragAndDrop.objectReferences.OfType<TextAsset>().FirstOrDefault(); if (droppedAsset != null) { AssignNewScript(droppedAsset); } } currentEvent.Use(); } } }
        private void InitializeStyles() { if (headerStyle == null) { headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 14, alignment = TextAnchor.MiddleLeft, padding = new RectOffset(5, 0, 0, 0) }; boxStyle = new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(10, 10, 10, 10) }; searchFieldStyle = new GUIStyle("SearchTextField"); searchCancelButtonStyle = new GUIStyle("SearchCancelButton"); dropZoneStyle = new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Italic, fontSize = 12, normal = { textColor = EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.4f, 0.4f, 0.4f) } }; assignedScriptBoxStyle = new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(10, 10, 10, 10), margin = new RectOffset(0, 0, 0, 0) }; Color bgColor = EditorGUIUtility.isProSkin ? new Color(0.18f, 0.18f, 0.18f) : new Color(0.8f, 0.8f, 0.8f); assignedScriptBoxStyle.normal.background = MakeTex(2, 2, bgColor); } }
        private Texture2D MakeTex(int width, int height, Color col) { var pix = new Color[width * height]; for (int i = 0; i < pix.Length; ++i) { pix[i] = col; } var result = new Texture2D(width, height); result.SetPixels(pix); result.Apply(); return result; }
        private void DrawEnumSelectionUI() { if (foundEnumNames.Count > 1) { EditorGUI.BeginChangeCheck(); int newIndex = EditorGUILayout.Popup("Select Enum to Edit", selectedEnumIndex, foundEnumNames.ToArray()); if (EditorGUI.EndChangeCheck()) { SelectEnum(newIndex); } EditorGUILayout.Space(5); } }
        private void AssignNewScript(TextAsset newScript) { if (enumScript == newScript && enumScript != null) return; enumScript = newScript; editableEnums.Clear(); originalEnums.Clear(); foundEnumNames.Clear(); selectedEnumIndex = -1; selectedEnumName = ""; if (enumScript != null) { EditorPrefs.SetString(EnumScriptKey, AssetDatabase.GetAssetPath(enumScript)); foundEnumNames = FindAllEnumsInScript(enumScript); if (foundEnumNames.Count == 1) { SelectEnum(0); } } else { EditorPrefs.DeleteKey(EnumScriptKey); } }
        private void SelectEnum(int index) { selectedEnumIndex = index; selectedEnumName = foundEnumNames[index]; LoadEnumsAndState(); }
        private List<string> FindAllEnumsInScript(TextAsset script) { var names = new List<string>(); if (script == null) return names; var regex = new Regex(@"(?:\[.*?\]\s*)*(?:public|internal|private)?\s*\benum\s+([a-zA-Z_][a-zA-Z0-9_]*)"); MatchCollection matches = regex.Matches(script.text); foreach (Match match in matches) { if (match.Groups.Count > 1) { names.Add(match.Groups[1].Value); } } return names; }
        private void LoadEnumsAndState() { if (enumScript == null || string.IsNullOrEmpty(selectedEnumName)) return; string[] currentEnums = GetCurrentEnumValues(enumScript, selectedEnumName); editableEnums = new List<string>(currentEnums); originalEnums = new List<string>(currentEnums); FilterEnums(); }
        private void DrawUpdateAndCancelButtons() { bool hasDuplicates = editableEnums.GroupBy(x => x).Any(g => g.Count() > 1); bool hasInvalidNames = editableEnums.Any(name => !IsValidEnumName(name)); bool hasChanged = !originalEnums.SequenceEqual(editableEnums); EditorGUILayout.BeginHorizontal(); GUILayout.FlexibleSpace(); if (GUILayout.Button("Cancel/Reload", GUILayout.Height(30), GUILayout.Width(120))) { LoadEnumsAndState(); } GUILayout.Space(10); using (new EditorGUI.DisabledScope(hasDuplicates || hasInvalidNames || !hasChanged)) { if (GUILayout.Button("Update Enums", GUILayout.Height(30), GUILayout.Width(120))) { UpdateEnumFile(enumScript, selectedEnumName, editableEnums.ToArray()); AssetDatabase.Refresh(); LoadEnumsAndState(); EditorUtility.DisplayDialog("Success", $"Enum '{selectedEnumName}' has been updated.", "OK"); } } GUILayout.FlexibleSpace(); EditorGUILayout.EndHorizontal(); }
        private void DrawToolsControls() { EditorGUILayout.BeginHorizontal(); if (GUILayout.Button(new GUIContent(" Import from .txt", EditorGUIUtility.IconContent("d_Import").image))) { string path = EditorUtility.OpenFilePanel("Import Enums", "", "txt"); if (!string.IsNullOrEmpty(path)) { editableEnums = File.ReadAllLines(path).Where(IsValidEnumName).ToList(); FilterEnums(); } } if (GUILayout.Button(new GUIContent(" Export to .txt", EditorGUIUtility.IconContent("d_SaveAs").image))) { string path = EditorUtility.SaveFilePanel("Export Enums", "", "MissionEnums", "txt"); if (!string.IsNullOrEmpty(path)) { File.WriteAllLines(path, editableEnums); } } EditorGUILayout.EndHorizontal(); }
        private void DrawEnumList() { int? indexToRemove = null; scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(Mathf.Min(filteredEnums.Count * 26, 250))); for (int i = 0; i < filteredEnums.Count; i++) { string enumValue = filteredEnums[i]; int originalIndex = editableEnums.IndexOf(enumValue); if (originalIndex == -1) continue; EditorGUILayout.BeginHorizontal(); string newValue = EditorGUILayout.TextField(enumValue); if (newValue != editableEnums[originalIndex]) { editableEnums[originalIndex] = newValue; FilterEnums(); } GUI.backgroundColor = new Color(1f, 0.6f, 0.6f); if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus"), GUILayout.Width(30))) { indexToRemove = originalIndex; } GUI.backgroundColor = Color.white; EditorGUILayout.EndHorizontal(); } EditorGUILayout.EndScrollView(); if (indexToRemove.HasValue) { editableEnums.RemoveAt(indexToRemove.Value); FilterEnums(); GUI.FocusControl(null); } }
        private void DrawSearchAndSortControls() { EditorGUILayout.BeginHorizontal(); searchQuery = EditorGUILayout.TextField(searchQuery, searchFieldStyle, GUILayout.ExpandWidth(true)); if (GUILayout.Button("", searchCancelButtonStyle)) { searchQuery = ""; GUI.FocusControl(null); } sortAlphabetically = GUILayout.Toggle(sortAlphabetically, new GUIContent(" Sort A-Z", EditorGUIUtility.IconContent("AlphabeticalSorting").image), EditorStyles.miniButton, GUILayout.Width(80)); EditorGUILayout.Space(5); }
        private void DrawAddNewEnumControls() { EditorGUILayout.BeginHorizontal(); newEnumValue = EditorGUILayout.TextField(newEnumValue); bool isValidNew = !string.IsNullOrEmpty(newEnumValue) && IsValidEnumName(newEnumValue) && !editableEnums.Contains(newEnumValue); using (new EditorGUI.DisabledScope(!isValidNew)) { if (GUILayout.Button(new GUIContent(" Add", EditorGUIUtility.IconContent("d_Toolbar Plus").image), GUILayout.Width(70))) { editableEnums.Add(newEnumValue); newEnumValue = ""; FilterEnums(); GUI.FocusControl(null); } } EditorGUILayout.EndHorizontal(); if (!isValidNew && !string.IsNullOrEmpty(newEnumValue)) { string error = ""; if (editableEnums.Contains(newEnumValue)) error = "This value name already exists."; else if (!IsValidEnumName(newEnumValue)) error = "Invalid name. Must be alphanumeric and start with a letter."; EditorGUILayout.HelpBox(error, MessageType.Error); } }
        private void DrawValidationMessages() { bool hasDuplicates = editableEnums.GroupBy(x => x).Any(g => g.Count() > 1); bool hasInvalidNames = editableEnums.Any(name => !IsValidEnumName(name)); if (hasDuplicates || hasInvalidNames) { string error = ""; if (hasDuplicates) error += "Error: Duplicate value names found.\n"; if (hasInvalidNames) error += "Error: One or more value names are invalid."; EditorGUILayout.HelpBox(error.Trim(), MessageType.Error); } }
        private void FilterEnums() { IEnumerable<string> result = editableEnums; if (!string.IsNullOrEmpty(searchQuery)) { result = result.Where(e => e.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0); } if (sortAlphabetically) { result = result.OrderBy(e => e); } filteredEnums = result.ToList(); }
        private bool IsValidEnumName(string name) { if (string.IsNullOrWhiteSpace(name)) return false; if (!char.IsLetter(name[0]) && name[0] != '_') return false; return name.All(c => char.IsLetterOrDigit(c) || c == '_'); }
        #endregion
    }
}
