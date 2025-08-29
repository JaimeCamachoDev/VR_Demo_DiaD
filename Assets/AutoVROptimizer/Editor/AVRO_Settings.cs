/*
Copyright (c) 2025 Valem Studio

This asset is the intellectual property of Valem Studio and is distributed under the Unity Asset Store End User License Agreement (EULA).

Unauthorized reproduction, modification, or redistribution of any part of this asset outside the terms of the Unity Asset Store EULA is strictly prohibited.

For support or inquiries, please contact Valem Studio via social media or through the publisher profile on the Unity Asset Store.
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEditor.Build;
namespace AVRO
{
    #region Editor
    [CustomEditor(typeof(AVRO_Settings))]
    public class AVRO_SettingsEditor : Editor
    {
        AVRO_Settings script = null;
        SerializedProperty Tickets;
        SerializedProperty CustomTickets;
        bool showDescriptionEditor = false;
        AVRO_Settings settings;

        void OnEnable()
        {
            script = (AVRO_Settings)target;
            Tickets = serializedObject.FindProperty("Tickets");
            CustomTickets = serializedObject.FindProperty("CustomTickets");
            settings = AVRO_Settings.GetOrCreateSettings();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Documentation", AVRO_Styles.TicketGreyText))
            {
                Application.OpenURL("https://auto-vr-optimizer.gitbook.io/auto-vr-optimizer-docs/");
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            settings.MaxCharCount = EditorGUILayout.IntField("MaxCharCount", settings.MaxCharCount);
            settings.AlternateTicketColors = EditorGUILayout.Toggle("Alternate Ticket Colors", settings.AlternateTicketColors);
            settings.AlwaysOpenInfoWindowDebug = EditorGUILayout.Toggle("Open Info On Click", settings.AlwaysOpenInfoWindowDebug);
            settings.DefaultAudioQuality = EditorGUILayout.FloatField("Default Audio Quality", settings.DefaultAudioQuality);
            settings.DefaultAudioQuality = Mathf.Clamp(settings.DefaultAudioQuality, 0, 1);
            settings.TargetSceneTriangles = EditorGUILayout.IntField("Target Scene Triangles", settings.TargetSceneTriangles);
            settings.DisplayObjectsOfTrianglesOver = EditorGUILayout.IntField("Display Objects Of Triangles Over", settings.DisplayObjectsOfTrianglesOver);
            EditorGUILayout.Space();

            if (settings.ShowOptions = EditorGUILayout.Foldout(settings.ShowOptions, "Advanced Options"))
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(CustomTickets);
                EditorGUILayout.PropertyField(Tickets);
                settings.EnableDebug = EditorGUILayout.Toggle("EnableDebug", settings.EnableDebug);
                if (settings.ShowColors = EditorGUILayout.Foldout(settings.ShowColors, "Show Colors"))
                    SetupTagColorsDictionary();

                EditorGUILayout.Space();
                if (GUILayout.Button(new GUIContent("Reset All")))
                {
                    AVRO_Utilities.ResetAllTickets(script.Tickets);
                    settings.FirstTimeSetup = true;
                    AVRO_Settings.SaveSettings();
                }

                EditorGUILayout.Space();
                if (GUILayout.Button(new GUIContent("Write All Tickets In Console")))
                {
                    script.WriteTicketsInConsole();
                }

                EditorGUILayout.Space();
                if (showDescriptionEditor = EditorGUILayout.Foldout(showDescriptionEditor, "Batch Tickets Description Editor"))
                {
                    settings.SourceText = EditorGUILayout.TextArea(settings.SourceText, GUILayout.Height(100));
                    if (GUILayout.Button(new GUIContent("Update Descriptions")))
                    {
                        script.DescriptAllItems();
                    }
                }
                EditorGUI.indentLevel--;
            }

            if (GUI.changed)
                AVRO_Settings.SaveSettings();
            serializedObject.ApplyModifiedProperties();
        }

        void SetupTagColorsDictionary()
        {
            var _enumValues = (AVRO_Settings.TicketTags[])Enum.GetValues(typeof(AVRO_Settings.TicketTags));
            if (settings.TagColorPairs == null || settings.TagColorPairs.Count == 0)
            {
                settings.TagColorPairs = new List<AVRO_Settings.TagColorPair>();
                for (int i = 0; i < _enumValues.Length; i++)
                {
                    var color = i < settings.DefaultTagsColors.Length ? settings.DefaultTagsColors[i] : Color.white;
                    settings.TagColorPairs.Add(new AVRO_Settings.TagColorPair
                    {
                        tag = _enumValues[i],
                        color = color
                    });
                }
            }

            foreach (var _tag in _enumValues)
            {
                if (!settings.TagColorPairs.Any(_pair => _pair.tag == _tag))
                    settings.TagColorPairs.Add(new AVRO_Settings.TagColorPair { tag = _tag });
            }

            settings.TagColorPairs.RemoveAll(_pair => !_enumValues.Contains(_pair.tag));
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            for (int i = 0; i < settings.TagColorPairs.Count; i++)
            {
                var _pair = settings.TagColorPairs[i];
                _pair.color = EditorGUILayout.ColorField(_pair.tag.ToString(), _pair.color);
                settings.TagColorPairs[i] = _pair;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }
    #endregion

    public class AVRO_Settings : ScriptableObject
    {
        public const string CustomObjectPath = "Assets/AutoVROptimizer/ScriptablesObjects/AVRO_Settings.asset";
        #region Variables
        public static string Path
        {
            get => EditorPrefs.GetString("AVRO_Path", "Assets/AutoVROptimizer/ScriptablesObjects/AVRO_Settings.asset");
            set => EditorPrefs.SetString("AVRO_Path", value);
        }
        public bool FirstTimeSetup = true;
        public bool AlternateTicketColors = true;
        public int MaxCharCount = 65;
        public bool ShowOptions = true;
        public bool ShowColors = true;
        public int TodoCount = 0;
        public int DoneCount = 0;
        public int IgnoreCount = 0;
        public int OmittedCount = 0;
        public int BigTodoCount = 0;
        public int BigDoneCount = 0;
        public int BigIgnoreCount = 0;
        public int BigOmittedCount = 0;
        public int SavedCount = 0;
        public bool ShowToDo = true;
        public bool ShowDone = true;
        public bool ShowIgnore = true;
        public bool ShowOmitted = true;
        public bool EnableDebug = false;
        public bool AlwaysOpenInfoWindowDebug = true;
        public bool NeedsReordering = false;
        public bool SkipNextGUIFrame = false;
        public bool CanRestore = false;
        public double LastSortedTime = 0;
        [Range(0, 1)] public float DefaultAudioQuality = 0.9f;
        public int TargetSceneTriangles = 300000;
        public int DisplayObjectsOfTrianglesOver = 10000;
        public AVRO_CustomTickets CustomTickets;
        public List<AVRO_Ticket> Tickets = new List<AVRO_Ticket>();
        public List<AVRO_Ticket> AllTickets = new List<AVRO_Ticket>();
        public Texture2D Icon_warning;
        public Texture2D Icon_error;
        public Texture2D Icon_message;
        public Texture2D Icon_info;
        public Texture2D Icon_foldOpen;
        public Texture2D Icon_foldClosed;
        public Texture2D Icon_help;
        public Texture2D Icon_validated;
        public Texture2D Icon_toggleOn;
        public Texture2D Icon_toggleMixed;
        public TicketWindow InfoWindow;
        public PopupWindow PopupWindow;
        public TicketTags FilteredTags = (TicketTags)~0;
        public SortingOrders SortedBy = SortingOrders.Tags;
        public BuildTarget CurrentBuildTarget = BuildTarget.NoTarget;
        public BuildTargetGroup CurrentBuildTargetGroup = BuildTargetGroup.Standalone;
        public NamedBuildTarget CurrentNamedBuildTarget = NamedBuildTarget.Standalone;

        public enum TicketLevels
        {
            Optional = 0,
            Recommended = 1,
            Required = 2,
            Information = 3
        }
        [Flags]
        public enum TicketTags
        {
            None = 0,
            Project = 1 << 0,
            Player = 1 << 1,
            Graphics = 1 << 2,
            Quality = 1 << 3,
            Renderer = 1 << 4,
            Files = 1 << 5,
            Scene = 1 << 6,
            Meta = 1 << 7,
            Textures = 1 << 8,
            Materials = 1 << 9,
            Audios = 1 << 10,
            Meshes = 1 << 11,
            Lights = 1 << 12,
            Physics = 1 << 13,
            Conditional = 1 << 14
        }
        public List<TagColorPair> TagColorPairs = new List<TagColorPair>();
        public Color[] DefaultTagsColors = {
            new Color(0f, 0f, 0f),
            new Color(0f, 0.466f, 0.764f),
            new Color(0.490f, 1f, 1f),
            new Color(0.118f, 0.725f, 0f),
            new Color(0.239f, 0.933f, 0.024f),
            new Color(0.439f, 1f, 0.706f),
            new Color(0.8f, 0.482f, 0f),
            new Color(0.773f, 0.773f, 0.773f),
            new Color(0f, 0.745f, 1f),
            new Color(1f, 0.259f, 0f),
            new Color(1f, 0.420f, 0f),
            new Color(1f, 0.627f, 0f),
            new Color(0.906f, 0.804f, 0.553f),
            new Color(1f, 0.859f, 0f),
            new Color(0.827f, 0.694f, 1f),
            new Color(1f, 1f, 1f)
        };
        [Serializable]
        public struct TagColorPair
        {
            public TicketTags tag;
            public Color color;
        }
        public enum TicketStates
        {
            Todo = 0,
            Done = 1,
            Ignore = 2,
            Omitted = 3
        }
        public enum SortingOrders
        {
            Tags = 0,
            Priority = 1,
        }

        [Serializable]
        public class Ticket
        {
            public string name;
            [TextArea] public string description;
            public TicketStates state;
            public TicketLevels level;
            public TicketTags tags;
            public string functionName;
            [Space]
            public bool noFix;
            public bool noRestore;
            public bool hide;
            [HideInInspector] public List<ConcernedObjectData> concernedObjects;
            [HideInInspector] public int ticketValue;
            [HideInInspector] public bool savedInFixAll;
            [Header("Debug")]
            [Tooltip("If there are multiple items, those are not listed by their hierarchy orders but Unity Find system.")] public List<string> lastValues = new List<string>();
            [HideInInspector] public List<string> lastObjects = new List<string>();

            [System.Serializable]
            public class ConcernedObjectData
            {
                public string guid;
                public bool toggle;
                public int value;
            }
        }
        #endregion
        #region Methods
        public static AVRO_Settings GetOrCreateSettings()
        {
            AVRO_Settings _settings = AssetDatabase.LoadAssetAtPath<AVRO_Settings>(Path);
            if (_settings == null)
            {
                string[] _assets = AssetDatabase.FindAssets("AVRO_Settings t:ScriptableObject");
                foreach (var _asset in _assets)
                {
                    string _assetPath = AssetDatabase.GUIDToAssetPath(_asset);
                    if (_assetPath.EndsWith(".asset"))
                    {
                        Path = _assetPath; // Update path when found
                        _settings = AssetDatabase.LoadAssetAtPath<AVRO_Settings>(_assetPath);
                        break;
                    }
                }
                if (_settings == null)
                {
                    _settings = CreateInstance<AVRO_Settings>();
                    AssetDatabase.CreateAsset(_settings, Path);
                }
                AssetDatabase.SaveAssets();
            }
            return _settings;
        }

        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        public void WriteTicketsInConsole()
        {
            string _ticketNames = "";

            foreach (var _item in Tickets)
            {
                _ticketNames += _item.name + "\n";
            }

            Debug.Log(_ticketNames);
        }

        public static void SaveSettings()
        {
            EditorUtility.SetDirty(GetOrCreateSettings());
        }

        public static void GetTextures()
        {
            var _settingsInstance = GetOrCreateSettings();
            _settingsInstance.Icon_warning = EditorGUIUtility.FindTexture("console.warnicon.sml");
            _settingsInstance.Icon_error = EditorGUIUtility.FindTexture("d_console.erroricon.sml");
            _settingsInstance.Icon_message = EditorGUIUtility.FindTexture("d_console.infoicon.sml");
            _settingsInstance.Icon_info = EditorGUIUtility.FindTexture("d_console.infoicon.inactive.sml");
            _settingsInstance.Icon_foldOpen = EditorGUIUtility.FindTexture("d_icon dropdown");
            _settingsInstance.Icon_foldClosed = EditorGUIUtility.FindTexture("d_Profiler.NextFrame");
            _settingsInstance.Icon_help = EditorGUIUtility.FindTexture("d__Help");
            _settingsInstance.Icon_validated = EditorGUIUtility.FindTexture("TestPassed");
            _settingsInstance.Icon_toggleOn = EditorGUIUtility.FindTexture("d_FilterSelectedOnly");
            _settingsInstance.Icon_toggleMixed = EditorGUIUtility.FindTexture("d_Toolbar Minus");
        }

        public string SourceText;
        [ContextMenu("Descript All Items")]
        public void DescriptAllItems()
        {
            string _newText = "";
            // Remove //
            var _lines0 = SourceText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            var _filtered0 = _lines0.Where(_line => !_line.TrimStart().StartsWith("//"));
            _newText = string.Join("\n", _filtered0);
            // Remove URLs
            var _lines1 = _newText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            var _filtered1 = _lines1.Where(_line => !_line.TrimStart().StartsWith("https:"));
            _newText = string.Join("\n", _filtered1);
            // Reformat Text
            _newText = _newText.Replace("What It Does:", "<b>• What It Does:</b>").Replace("Our Modification: ", "\n<b>• Our Modification: </b>");
            // CompareTicketsAndText
            CompareLinesAndFillTickets(_newText);
        }

        public static void CompareLinesAndFillTickets(string _fullText)
        {
            var _lines = _fullText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            var _settingsInstance = GetOrCreateSettings();
            for (int i = 0; i < _lines.Length; i++)
            {
                string _currentLine = _lines[i].Trim();
                foreach (var _ticket in _settingsInstance.Tickets)
                {
                    if (_currentLine == _ticket.name.Trim())
                    {
                        List<string> _collectedLines = new List<string>();
                        int j = i + 1;
                        while (j < _lines.Length)
                        {
                            string _nextLine = _lines[j].Trim();
                            bool _isNextTicket = _settingsInstance.Tickets.Any(t => t.name.Trim() == _nextLine);
                            if (_isNextTicket)
                                break;
                            _collectedLines.Add(_lines[j]);
                            j++;
                        }

                        _ticket.data.description = string.Join("\n", _collectedLines).Trim();
                        break;
                    }
                }
            }
        }

        public static void GetCurrentBuildTargets()
        {
            var _settingsInstance = GetOrCreateSettings();
            _settingsInstance.CurrentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            _settingsInstance.CurrentBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            _settingsInstance.CurrentNamedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(_settingsInstance.CurrentBuildTargetGroup);
        }

        public static T ReturnBuildType<T>()
        {
            var _settingsInstance = GetOrCreateSettings();
            if (typeof(T) == typeof(BuildTarget))
                return (T)(object)_settingsInstance.CurrentBuildTarget;
            if (typeof(T) == typeof(BuildTargetGroup))
                return (T)(object)_settingsInstance.CurrentBuildTargetGroup;
            if (typeof(T) == typeof(NamedBuildTarget))
                return (T)(object)_settingsInstance.CurrentNamedBuildTarget;

            throw new InvalidOperationException($"Cannot return build of type {typeof(T).Name}.");
        }
    }
    #endregion
}