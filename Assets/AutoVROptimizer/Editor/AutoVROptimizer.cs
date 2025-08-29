/*
Copyright (c) 2025 Valem Studio

This asset is the intellectual property of Valem Studio and is distributed under the Unity Asset Store End User License Agreement (EULA).

Unauthorized reproduction, modification, or redistribution of any part of this asset outside the terms of the Unity Asset Store EULA is strictly prohibited.

For support or inquiries, please contact Valem Studio via social media or through the publisher profile on the Unity Asset Store.
*/

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace AVRO
{
    static class MyCustomSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            Vector2[] _scrollPos = new Vector2[4];
            var settingsInstance = AVRO_Settings.GetOrCreateSettings();
            var provider = new SettingsProvider("Project/MyCustomIMGUISettings", SettingsScope.Project)
            {
                label = "Auto VR Optimizer",
                guiHandler = (searchContext) =>
                {
                    // Skips a frame if a repaint is needed
                    if (settingsInstance.SkipNextGUIFrame)
                    {
                        settingsInstance.SkipNextGUIFrame = false;
                        return;
                    }
                    // Reorder tickets if needed
                    if (Event.current.type == EventType.Layout && settingsInstance.NeedsReordering)
                    {
                        AVRO_Utilities.SortOutTicketsBy(settingsInstance.SortedBy, settingsInstance.Tickets);
                        settingsInstance.NeedsReordering = false;
                        settingsInstance.SkipNextGUIFrame = true;
                    }

                    // Get & Update
                    var settings = AVRO_Settings.GetSerializedSettings();
                    AVRO_Settings.GetTextures();
                    settings.Update();
                    EditorGUI.BeginChangeCheck();
                    settingsInstance.AllTickets = new List<AVRO_Ticket>(settingsInstance.Tickets);
                    if (settingsInstance.CustomTickets)
                        settingsInstance.AllTickets.AddRange(settingsInstance.CustomTickets.CustomTickets);
                    var ticketsProperty = settings.FindProperty("AllTickets");

                    // First Time Setup
                    #region Content
                    if (settingsInstance.FirstTimeSetup)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        EditorGUILayout.Space(20);

                        GUIStyle _titleStyle = new GUIStyle(EditorStyles.boldLabel)
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 16
                        };
                        GUILayout.Label("Welcome to Auto VR Optimizer", _titleStyle);
                        EditorGUILayout.Space(10);

                        GUILayout.Label(
                            "Auto VR Optimizer helps you build your project for Virtual Reality.\n" +
                            "It analyzes your project and generates a checklist of optimization tickets.\n\n" +
                            "You can review each issue individually or use the 'Fix All' button to optimize everything in one click.\n" +
                            "Click on a ticket to get more context and suggested solutions.\n\n" +
                            "You'll find additional settings under 'AVRO_Settings' to tailor the tool to your needs.\n\n" +
                            "Let's get your VR project running smoothly!",
                            AVRO_Styles.CenteredText
                        );
                        EditorGUILayout.Space(15);

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Start Analysis"), GUILayout.Width(172), GUILayout.Height(40)))
                        {
                            AVRO_Utilities.SortOutTicketsBy(settingsInstance.SortedBy, settingsInstance.Tickets);
                            AVRO_Utilities.AnalyzeProject(ticketsProperty);
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        EditorGUILayout.Space(10);
                        GUILayout.Label("Need help? Reach out to us on social media.", EditorStyles.centeredGreyMiniLabel);
                        EditorGUILayout.Space(10);

                        EditorGUILayout.EndVertical();
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Developed by Valem Studio", AVRO_Styles.TicketGreyText))
                        {
                            Application.OpenURL("https://www.patreon.com/cw/ValemVR");
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }

                    // Header
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal(AVRO_Styles.Title);
                    EditorGUILayout.BeginHorizontal();
                    // FixAll
                    if (GUILayout.Button(new GUIContent("Fix All"), settingsInstance.TodoCount > 0 ? AVRO_Styles.TicketButton : AVRO_Styles.TicketButtonGrey))
                    {
                        if (settingsInstance.TodoCount > 0)
                            AVRO_Utilities.FixAllProject(ticketsProperty);
                    }
                    // Restore
                    GUILayout.Space(50);
                    if (GUILayout.Button(new GUIContent("Restore"), settingsInstance.CanRestore ? AVRO_Styles.TicketButton : AVRO_Styles.TicketButtonGrey))
                    {
                        if (settingsInstance.CanRestore)
                        {
                            string _savedTickets = "";
                            foreach (var _ticket in settingsInstance.Tickets)
                            {
                                if (!_ticket.data.hide && !_ticket.data.noFix && !_ticket.data.noRestore && _ticket.data.savedInFixAll && _ticket.data.state != AVRO_Settings.TicketStates.Ignore)
                                {
                                    _savedTickets += _ticket.name + "\n";
                                    AVRO_Utilities.AnalyzeTicket(_ticket);
                                }
                            }
                            PopupWindow.ShowPopupRestoreWindow(_savedTickets, settingsInstance.SavedCount);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();
                    if (GUILayout.Button(new GUIContent("Analyze"), AVRO_Styles.TicketButton))
                    {
                        AVRO_Utilities.AnalyzeProject(ticketsProperty);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(10);

                    // Filtering / Sorting
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    settingsInstance.SortedBy = (AVRO_Settings.SortingOrders)EditorGUILayout.EnumPopup(new GUIContent("Sort By"), settingsInstance.SortedBy);
                    if (EditorGUI.EndChangeCheck())
                    {
                        settingsInstance.NeedsReordering = true;
                    }
                    EditorGUILayout.Separator();
                    EditorGUI.BeginChangeCheck();
                    settingsInstance.FilteredTags = (AVRO_Settings.TicketTags)EditorGUILayout.EnumFlagsField(new GUIContent("Filter by Tags"), settingsInstance.FilteredTags);
                    if (EditorGUI.EndChangeCheck())
                    {
                        AVRO_Utilities.UpdateTicketCounts(ticketsProperty);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    // TODO
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if (GUILayout.Button(new GUIContent(" ToDo (" + settingsInstance.TodoCount + ")", settingsInstance.ShowToDo ? settingsInstance.Icon_foldOpen : settingsInstance.Icon_foldClosed), AVRO_Styles.Title))
                        settingsInstance.ShowToDo = !settingsInstance.ShowToDo;
                    if (settingsInstance.ShowToDo)
                    {
                        if (settingsInstance.TodoCount > 0)
                        {
                            EditorGUILayout.BeginVertical(AVRO_Styles.ScrollBackground);
                            float scrollHeight = settingsInstance.TodoCount > 14 ? 760 : settingsInstance.TodoCount * 54f + settingsInstance.BigTodoCount * 12f;
                            _scrollPos[0] = EditorGUILayout.BeginScrollView(_scrollPos[0], GUIStyle.none, GUILayout.ExpandHeight(false), GUILayout.Height(scrollHeight));
                            ShowTicketsOfState(ticketsProperty, AVRO_Settings.TicketStates.Todo);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndScrollView();
                        }
                    }
                    EditorGUILayout.EndVertical();

                    // DONE
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if (GUILayout.Button(new GUIContent(" Done (" + settingsInstance.DoneCount + ")", settingsInstance.ShowDone ? settingsInstance.Icon_foldOpen : settingsInstance.Icon_foldClosed), AVRO_Styles.Title))
                        settingsInstance.ShowDone = !settingsInstance.ShowDone;
                    if (settingsInstance.ShowDone)
                    {
                        if (settingsInstance.DoneCount > 0)
                        {
                            EditorGUILayout.BeginVertical(AVRO_Styles.ScrollBackground);
                            float scrollHeight = settingsInstance.DoneCount > 14 ? 760 : settingsInstance.DoneCount * 54f + settingsInstance.BigDoneCount * 12f;
                            _scrollPos[1] = EditorGUILayout.BeginScrollView(_scrollPos[1], GUIStyle.none, GUILayout.ExpandHeight(false), GUILayout.Height(scrollHeight));
                            ShowTicketsOfState(ticketsProperty, AVRO_Settings.TicketStates.Done);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndScrollView();
                        }
                    }
                    EditorGUILayout.EndVertical();

                    // IGNORE
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false));
                    if (GUILayout.Button(new GUIContent(" Ignore (" + settingsInstance.IgnoreCount + ")", settingsInstance.ShowIgnore ? settingsInstance.Icon_foldOpen : settingsInstance.Icon_foldClosed), AVRO_Styles.Title))
                        settingsInstance.ShowIgnore = !settingsInstance.ShowIgnore;
                    if (settingsInstance.ShowIgnore)
                    {
                        if (settingsInstance.IgnoreCount > 0)
                        {
                            EditorGUILayout.BeginVertical(AVRO_Styles.ScrollBackground);
                            float scrollHeight = settingsInstance.IgnoreCount > 14 ? 760 : settingsInstance.IgnoreCount * 54f + settingsInstance.BigIgnoreCount * 12f;
                            _scrollPos[2] = EditorGUILayout.BeginScrollView(_scrollPos[2], GUIStyle.none, GUILayout.ExpandHeight(false), GUILayout.Height(scrollHeight));
                            ShowTicketsOfState(ticketsProperty, AVRO_Settings.TicketStates.Ignore);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndScrollView();
                        }
                    }
                    EditorGUILayout.EndVertical();

                    // OMITTED
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false));
                    if (GUILayout.Button(new GUIContent(" Omitted (" + settingsInstance.OmittedCount + ")", settingsInstance.ShowOmitted ? settingsInstance.Icon_foldOpen : settingsInstance.Icon_foldClosed), AVRO_Styles.GreyTitle))
                        settingsInstance.ShowOmitted = !settingsInstance.ShowOmitted;
                    if (settingsInstance.ShowOmitted)
                    {
                        EditorGUILayout.BeginVertical(AVRO_Styles.CommentGrey);
                        EditorGUILayout.LabelField(new GUIContent(" Tickets shown here are not applicable to your project as some packages\n are missing or the versioning is different from what the tickets need.", settingsInstance.Icon_info), AVRO_Styles.CommentGrey);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();

                        EditorGUILayout.BeginVertical(AVRO_Styles.ScrollBackground);
                        float scrollHeight = settingsInstance.OmittedCount > 14 ? 760 : settingsInstance.OmittedCount * 54f + settingsInstance.BigOmittedCount * 12f;
                        _scrollPos[3] = EditorGUILayout.BeginScrollView(_scrollPos[3], GUIStyle.none, GUILayout.ExpandHeight(false), GUILayout.Height(scrollHeight));
                        ShowTicketsOfState(ticketsProperty, AVRO_Settings.TicketStates.Omitted);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();

                    // End
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Developed by Valem Studio", AVRO_Styles.TicketGreyText))
                    {
                        Application.OpenURL("https://www.patreon.com/cw/ValemVR");
                    }
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck())
                    {
                        AVRO_Utilities.UpdateTicketCounts(ticketsProperty);
                        settings.ApplyModifiedProperties();
                        AVRO_Settings.SaveSettings();
                    }
                    #endregion
                },

                keywords = new HashSet<string>(new[] { "VR", "Optimizer", "Auto", "Performance", "Auto VR", "AVRO" })
            };

            #region Tickets State
            void ShowTicketsOfState(SerializedProperty _ticketsProperty, AVRO_Settings.TicketStates _displayState)
            {
                Rect _lastTicketRect = default;
                int _currentCount = 0;
                for (int i = 0; i < _ticketsProperty.arraySize; i++)
                {
                    var _ticketProperty = _ticketsProperty.GetArrayElementAtIndex(i);
                    AVRO_Ticket _ticketInstance = _ticketProperty.objectReferenceValue as AVRO_Ticket;
                    AVRO_Settings.Ticket _ticket = _ticketInstance.data;
                    bool isFlagSelected = (_ticket.tags & settingsInstance.FilteredTags) != 0;

                    if (!_ticket.hide && isFlagSelected && _ticket.state == _displayState)
                    {
                        // Set text
                        string _newName = " " + _ticket.name;
                        bool _cutName = _newName.Length > settingsInstance.MaxCharCount || _newName.Contains(@"\n");
                        _ticketInstance.IsBigTicket = _cutName;
                        int _nameSplitIndex = _newName.IndexOf(@"\n");
                        if (_nameSplitIndex != -1)
                            _newName = _newName.Substring(0, _nameSplitIndex) + "\n " + _newName.Substring(_nameSplitIndex + 2);
                        else if (_newName.Length > settingsInstance.MaxCharCount)
                            _newName = _newName.Substring(0, settingsInstance.MaxCharCount) + "\n " +
                                (_newName.Substring(settingsInstance.MaxCharCount)[0] == ' ' ? _newName.Substring(settingsInstance.MaxCharCount + 1) : _newName.Substring(settingsInstance.MaxCharCount));
                        // Determine style based on text size
                        GUIStyle _ticketStyle = _cutName ? AVRO_Styles.BigTicket : AVRO_Styles.Ticket;
                        if (settingsInstance.AlternateTicketColors)
                            AVRO_Styles.SetBackgroundColor(AVRO_Styles.TicketStyle, new Color(1f, 1f, 1f, _currentCount % 2 == 0 ? 0.05f : 0.09f), new Color(1f, 1f, 1f, 0.2f));
                        GUIStyle _textStyle = _ticket.state == AVRO_Settings.TicketStates.Todo
                            ? (_cutName ? AVRO_Styles.BigTicketText : AVRO_Styles.TicketText)
                            : (_cutName ? AVRO_Styles.BigTicketGreyText : AVRO_Styles.TicketGreyText);

                        GUILayout.BeginVertical(_ticketStyle);
                        GUILayout.BeginHorizontal();
                        DisplayTags(_ticket);
                        GUILayout.EndHorizontal();
                        if (_ticketInstance.IsBigTicket)
                            GUILayout.Space(8);
                        GUILayout.BeginHorizontal();
                        // Label
                        EditorGUILayout.LabelField(new GUIContent(_newName,
                            _ticket.state == AVRO_Settings.TicketStates.Done ? settingsInstance.Icon_validated :
                            (_ticket.level == AVRO_Settings.TicketLevels.Optional ? settingsInstance.Icon_message :
                            _ticket.level == AVRO_Settings.TicketLevels.Recommended ? settingsInstance.Icon_warning :
                            _ticket.level == AVRO_Settings.TicketLevels.Information ? settingsInstance.Icon_info :
                            settingsInstance.Icon_error)), _textStyle);
                        // Buttons
                        // Fix
                        if (!_ticket.noFix && _ticket.state == AVRO_Settings.TicketStates.Todo && GUILayout.Button("Fix", AVRO_Styles.TicketButton))
                        {
                            AVRO_Utilities.CheckForConditions();
                            if ((_ticket.tags & AVRO_Settings.TicketTags.Conditional) == AVRO_Settings.TicketTags.Conditional)
                            {
                                PopupWindow.ShowPopupConditionalWindow(_ticketInstance);
                            }
                            else
                            {
                                AVRO_Utilities.FixTicket(_ticketInstance);
                                _ticket.state = AVRO_Settings.TicketStates.Done;
                                AVRO_Utilities.UpdateTicketCounts(_ticketsProperty);
                                AVRO_Utilities.AnalyzeTicket(_ticketInstance);
                            }
                            AVRO_Settings.SaveSettings();
                        }
                        // Ignore
                        if (_ticket.state == AVRO_Settings.TicketStates.Todo && GUILayout.Button("Ignore", AVRO_Styles.TicketButton))
                        {
                            _ticket.state = AVRO_Settings.TicketStates.Ignore;
                            AVRO_Utilities.UpdateTicketCounts(_ticketsProperty);
                            AVRO_Settings.SaveSettings();
                        }
                        // Restore
                        if (_ticket.state == AVRO_Settings.TicketStates.Ignore && GUILayout.Button("Restore", AVRO_Styles.TicketButton))
                        {
                            AVRO_Utilities.CheckForConditions();
                            _ticket.state = AVRO_Settings.TicketStates.Todo;
                            AVRO_Utilities.AnalyzeTicket(_ticketInstance);
                            AVRO_Utilities.UpdateTicketCounts(_ticketsProperty);
                            AVRO_Settings.SaveSettings();
                        }
                        // Info
                        if (GUILayout.Button(new GUIContent("", settingsInstance.Icon_help), AVRO_Styles.SmallTicketButton))
                        {
                            TicketWindow.ShowTicketWindow(_ticketInstance);
                        }
                        EditorGUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        _lastTicketRect = GUILayoutUtility.GetLastRect();
                        if (Event.current.type == EventType.MouseDown && _lastTicketRect.Contains(Event.current.mousePosition))
                        {
                            if (settingsInstance.AlwaysOpenInfoWindowDebug || settingsInstance.InfoWindow)
                                TicketWindow.ShowTicketWindow(_ticketInstance);
                            Event.current.Use();
                        }

                        _currentCount++;
                    }
                }
            }
            #endregion

            #region Tags
            void DisplayTags(AVRO_Settings.Ticket _ticket)
            {
                AVRO_Settings.TicketTags[] _allTags = (AVRO_Settings.TicketTags[])Enum.GetValues(typeof(AVRO_Settings.TicketTags));
                foreach (AVRO_Settings.TicketTags _tag in _allTags)
                {
                    if (_tag != AVRO_Settings.TicketTags.None && (_ticket.tags & _tag) == _tag)
                    {
                        if (settingsInstance == null || settingsInstance.TagColorPairs == null)
                        {
                            Debug.LogWarning("settingsInstance or TagColorPairs was null during DisplayTags.");
                            return;
                        }
                        int _index = Array.IndexOf(Enum.GetValues(typeof(AVRO_Settings.TicketTags)), _tag);
                        AVRO_Styles.SetBackgroundColor(AVRO_Styles.TagStyle, settingsInstance.TagColorPairs[_index].color);
                        GUILayout.BeginHorizontal(AVRO_Styles.Tag);
                        GUILayout.Label(_tag.ToString(), AVRO_Styles.TagText);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            #endregion
            return provider;
        }
    }

    #region Ticket Window
    public class TicketWindow : EditorWindow
    {
#pragma warning disable UDR0001 // Domain Reload Analyzer
        static AVRO_Settings settingsInstance;
        internal static AVRO_Ticket ticket;
#pragma warning restore UDR0001 // Domain Reload Analyzer
        List<Toggle> toggles = new List<Toggle>();
        int selectedToggles = 0;

        internal static void ShowTicketWindow(AVRO_Ticket _ticket)
        {
            settingsInstance = AVRO_Settings.GetOrCreateSettings();
            AVRO_Settings.GetTextures();
            if (!settingsInstance.InfoWindow)
                settingsInstance.InfoWindow = CreateInstance<TicketWindow>();

            settingsInstance.InfoWindow.Initialize(_ticket);
            settingsInstance.InfoWindow.titleContent = new GUIContent("Infos" + (_ticket.name.Split('-').Length > 1 ? " : " + _ticket.name.Split('-')[0].Trim() : ""));
            settingsInstance.InfoWindow.minSize = new Vector2(400, 200);
            settingsInstance.InfoWindow.maxSize = new Vector2(600, 400);
            settingsInstance.InfoWindow.Show();
            settingsInstance.InfoWindow.Focus();
        }

        void Initialize(AVRO_Ticket _ticket)
        {
            ticket = _ticket;
            rootVisualElement.Clear();
            CreateGUI();
            Repaint();
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();

            if (!ticket)
                return;

            // Tags
            VisualElement _tagsContainer = new VisualElement();
            _tagsContainer.style.fontSize = 10;
            _tagsContainer.style.flexDirection = FlexDirection.Row;
            _tagsContainer.style.alignSelf = Align.FlexEnd;
            _tagsContainer.style.marginTop = 5;
            _tagsContainer.style.marginBottom = 5;
            rootVisualElement.Add(_tagsContainer);
            foreach (AVRO_Settings.TicketTags _tag in Enum.GetValues(typeof(AVRO_Settings.TicketTags)))
            {
                if (_tag != AVRO_Settings.TicketTags.None && (ticket.data.tags & _tag) == _tag)
                {
                    int _index = Array.IndexOf(Enum.GetValues(typeof(AVRO_Settings.TicketTags)), _tag);
                    Label _tagLabel = CreateTagLabel(_tag.ToString(), AVRO_Settings.GetOrCreateSettings().TagColorPairs[_index].color);
                    _tagsContainer.Add(_tagLabel);
                }
            }

            string _newName = ticket.data.name;
            int _nameSplitIndex = _newName.IndexOf(@"\n");
            if (_nameSplitIndex != -1)
                _newName = _newName.Substring(0, _nameSplitIndex) + "\n" + _newName.Substring(_nameSplitIndex + 2);
            Label _titleLabel = CreatDefaultLabel(_newName);
            _titleLabel.style.fontSize = 14;
            _titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            rootVisualElement.Add(_titleLabel);

            string _description = ticket.data.description;
            _description = _description.Replace("@TargetSceneTriangles",
                "\nCurrent value : " + ticket.data.ticketValue + " triangles.\nMax value : " + AVRO_Settings.GetOrCreateSettings().TargetSceneTriangles.ToString() + " triangles.");
            Label _descriptionLabel = CreatDefaultLabel(_description);
            _descriptionLabel.style.marginBottom = 20;
            rootVisualElement.Add(_descriptionLabel);

            // Clickable Concerned Objects
            if (ticket.data.concernedObjects.Count <= 0 || ticket.data.state == AVRO_Settings.TicketStates.Ignore)
                return;

            List<UnityEngine.Object> _concernedList = new List<UnityEngine.Object>();
            for (int i = 0; i < ticket.data.concernedObjects.Count; i++)
            {
                if ((ticket.data.tags & AVRO_Settings.TicketTags.Scene) == AVRO_Settings.TicketTags.Scene)
                {
                    GlobalObjectId _id;
                    if (GlobalObjectId.TryParse(ticket.data.concernedObjects[i].guid, out _id))
                        _concernedList.Add(GlobalObjectId.GlobalObjectIdentifierToObjectSlow(_id));
                }
                else if ((ticket.data.tags & AVRO_Settings.TicketTags.Files) == AVRO_Settings.TicketTags.Files)
                {
                    _concernedList.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ticket.data.concernedObjects[i].guid));
                }
            }
            Label _concernedLabel;
            VisualElement _concernedContainer = new VisualElement();
            Image _icon = new Image();
            Label _count = new Label();
            if (_concernedList[0] == null)
            {
                _concernedLabel = CreatDefaultLabel("The last analyze was done in another scene.\nPlease make a new one to refresh the list.");
                _concernedLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            }
            else
            {
                _concernedContainer.style.flexDirection = FlexDirection.Row;

                _icon.scaleMode = ScaleMode.ScaleAndCrop;
                _icon.style.width = 16;
                _icon.style.height = 16;

                Button _selectAll = null;
                _selectAll = new Button(() => ToggleAll(ticket, _icon, _count));
                _selectAll.style.marginLeft = 7;
                _selectAll.style.marginRight = 9;
                _selectAll.style.width = 16;
                _selectAll.style.height = 16;
                _selectAll.style.paddingLeft = 0;
                _selectAll.style.paddingRight = 0;
                _selectAll.Add(_icon);

                _concernedContainer.Add(_selectAll);
                _concernedContainer.Add(_count);

                _concernedLabel = CreatDefaultLabel("/  " + ticket.data.concernedObjects.Count + " selected");
                _concernedLabel.style.alignSelf = Align.FlexStart;
            }
            _concernedContainer.Add(_concernedLabel);
            _concernedContainer.style.marginBottom = 5;
            rootVisualElement.Add(_concernedContainer);

            if (_concernedList[0] == null)
                return;
            CheckToggles(ticket, _icon, _count);
            ScrollView _scrollView = new ScrollView(ScrollViewMode.Vertical);
            _scrollView.style.width = 400;
            _scrollView.style.maxHeight = 355;
            _scrollView.style.backgroundColor = Color.black;
            toggles = new List<Toggle>();

            for (int i = 0; i < _concernedList.Count; i++)
            {
                int _index = i;
                UnityEngine.Object _obj = _concernedList[i];
                if (_obj == null)
                    continue;

                VisualElement _row = new VisualElement();
                _row.style.flexDirection = FlexDirection.Row;
                _row.style.marginLeft = 5;
                _row.style.alignItems = Align.Center;
                _row.style.height = 24;

                Toggle _toggle = new Toggle();
                _toggle.value = ticket.data.concernedObjects[i].toggle;
                _toggle.RegisterValueChangedCallback(evt => { ticket.data.concernedObjects[_index].toggle = evt.newValue; CheckToggles(ticket, _icon, _count); });
                _toggle.style.marginTop = 0;
                _toggle.style.marginBottom = 0;
                _toggle.style.marginRight = 5;
                _toggle.style.paddingLeft = 0;
                _toggle.style.paddingRight = 0;
                _toggle.style.height = 18;
                _toggle.style.alignSelf = Align.Center;
                toggles.Add(_toggle);

                Button _button = new Button(() => OnObjectClick(_obj));
                _button.style.width = 350;
                _button.style.unityTextAlign = TextAnchor.MiddleLeft;
                _button.style.height = 20;
                _button.style.alignSelf = Align.Center;

                VisualElement _buttonContainer = new VisualElement();
                _buttonContainer.style.flexDirection = FlexDirection.Row;
                _buttonContainer.style.justifyContent = Justify.SpaceBetween;
                _buttonContainer.style.alignItems = Align.FlexStart;
                _buttonContainer.style.width = Length.Percent(100);

                Label _name = new Label(_obj.name);
                _name.style.flexGrow = 1;

                Label _countLabel = new Label(ticket.data.concernedObjects[i].value > -1 ? ticket.data.concernedObjects[i].value.ToString() : "");
                _countLabel.style.unityTextAlign = TextAnchor.UpperRight;
                _countLabel.style.color = new Color(1, 1, 1, 0.5f);

                _buttonContainer.Add(_name);
                _buttonContainer.Add(_countLabel);
                _button.Add(_buttonContainer);

                _row.Add(toggles[i]);
                _row.Add(_button);
                _scrollView.Add(_row);
            }
            rootVisualElement.Add(_scrollView);

            Button _GetSelection;
            _GetSelection = CreateStyledButton("Get Selection", () => GetSelection(ticket), 100, 5);
            _GetSelection.style.alignSelf = Align.FlexStart;
            rootVisualElement.Add(_GetSelection);
        }

        Label CreatDefaultLabel(string _text)
        {
            Label _label = new Label(_text);
            _label.style.whiteSpace = WhiteSpace.Normal;
            _label.style.flexWrap = Wrap.Wrap;
            _label.style.paddingLeft = 5;
            _label.style.marginBottom = 10;
            return _label;
        }

        Label CreateTagLabel(string _text, Color _color)
        {
            Label _label = new Label(_text);
            _label.style.color = Color.black;
            _label.style.backgroundColor = _color;
            _label.style.unityTextAlign = TextAnchor.MiddleCenter;
            _label.style.marginRight = 5;
            _label.style.height = 16;
            _label.style.width = 68;
            _label.style.paddingRight = 5;
            return _label;
        }

        Button CreateStyledButton(string _text, Action _onClick, int _width = 100, int _topMargin = 0)
        {
            Button _button = new Button(_onClick);
            _button.text = _text;
            _button.style.width = _width;
            _button.style.height = 25;
            _button.style.alignSelf = Align.Center;
            _button.style.marginTop = _topMargin;
            _button.style.marginBottom = 5;
            return _button;
        }

        void OnObjectClick(UnityEngine.Object _obj)
        {
            Selection.activeObject = _obj;
            EditorGUIUtility.PingObject(_obj);
        }

        void GetSelection(AVRO_Ticket _ticket)
        {
            List<UnityEngine.Object> _selection = new List<UnityEngine.Object>();
            if ((ticket.data.tags & AVRO_Settings.TicketTags.Scene) == AVRO_Settings.TicketTags.Scene)
            {
                GlobalObjectId _id;
                for (int i = 0; i < _ticket.data.concernedObjects.Count; i++)
                {
                    if (_ticket.data.concernedObjects[i].toggle)
                        if (GlobalObjectId.TryParse(_ticket.data.concernedObjects[i].guid, out _id))
                            _selection.Add(GlobalObjectId.GlobalObjectIdentifierToObjectSlow(_id) as GameObject);
                }
            }
            else if ((ticket.data.tags & AVRO_Settings.TicketTags.Files) == AVRO_Settings.TicketTags.Files)
            {
                for (int i = 0; i < _ticket.data.concernedObjects.Count; i++)
                {
                    if (_ticket.data.concernedObjects[i].toggle)
                        _selection.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_ticket.data.concernedObjects[i].guid));
                }
            }
            Selection.objects = _selection.ToArray();
            // SetFocusOnUnityWindow("UnityEditor.SceneHierarchyWindow");
        }

        void ToggleAll(AVRO_Ticket _ticket, Image _image, Label _count)
        {
            bool _select = CheckToggles(_ticket, _image, _count) != 2;
            for (int i = 0; i < _ticket.data.concernedObjects.Count; i++)
            {
                _ticket.data.concernedObjects[i].toggle = _select;
                toggles[i].value = _select;
            }
        }

        int CheckToggles(AVRO_Ticket _ticket, Image _image, Label _count)
        {
            selectedToggles = 0;
            foreach (var _data in _ticket.data.concernedObjects)
            {
                if (_data.toggle)
                    selectedToggles++;
            }
            _image.image = selectedToggles == _ticket.data.concernedObjects.Count ? settingsInstance.Icon_toggleOn : selectedToggles == 0 ? null : settingsInstance.Icon_toggleMixed;
            _count.text = selectedToggles + "";
            return selectedToggles == 0 ? 0 : selectedToggles == _ticket.data.concernedObjects.Count ? 2 : 1;
        }

        void SetFocusOnUnityWindow(string _windowPath)
        {
            Type _type = typeof(Editor).Assembly.GetType(_windowPath);
            if (_type != null)
            {
                EditorWindow _window = EditorWindow.GetWindow(_type);
                _window.Focus();
            }
        }
    }
    #endregion

    #region Popup Window
    public class PopupWindow : EditorWindow
    {
#pragma warning disable UDR0001 // Domain Reload Analyzer
        static AVRO_Settings settingsInstance;
#pragma warning restore UDR0001 // Domain Reload Analyzer
        AVRO_Ticket ticket;
        string savedTickets;
        int count;

        internal static void ShowPopupConditionalWindow(AVRO_Ticket _ticket, int _count = 0)
        {
            settingsInstance = AVRO_Settings.GetOrCreateSettings();
            if (!settingsInstance.PopupWindow)
                settingsInstance.PopupWindow = CreateInstance<PopupWindow>();

            settingsInstance.PopupWindow.InitializeConditional(_ticket, _count);
            settingsInstance.PopupWindow.titleContent = new GUIContent("Warning" + (_ticket ?
                (_ticket.name.Split('-').Length > 1 ? " : " + _ticket.name.Split('-')[0].Trim() : "") :
                " : " + _count + " conditional tickets"));
            settingsInstance.PopupWindow.minSize = new Vector2(600, 150);
            settingsInstance.PopupWindow.maxSize = new Vector2(600, 150);
            settingsInstance.PopupWindow.ShowAuxWindow();
            settingsInstance.PopupWindow.Focus();
        }

        internal static void ShowPopupRestoreWindow(string _savedTickets, int _count)
        {
            settingsInstance = AVRO_Settings.GetOrCreateSettings();
            if (!settingsInstance.PopupWindow)
                settingsInstance.PopupWindow = CreateInstance<PopupWindow>();

            settingsInstance.PopupWindow.InitializeRestore(_savedTickets, _count);
            settingsInstance.PopupWindow.titleContent = new GUIContent($"Restore {_count} tickets to last Fix All State");
            settingsInstance.PopupWindow.minSize = new Vector2(600, 250);
            settingsInstance.PopupWindow.maxSize = new Vector2(600, 250);
            settingsInstance.PopupWindow.ShowAuxWindow();
            settingsInstance.PopupWindow.Focus();
        }

        void InitializeConditional(AVRO_Ticket _ticket, int _count)
        {
            ticket = _ticket;
            count = _count;
            rootVisualElement.Clear();
            CreateConditionalGUI();
            Repaint();
        }

        void InitializeRestore(string _savedTickets, int _count)
        {
            savedTickets = _savedTickets;
            count = _count;
            rootVisualElement.Clear();
            CreateRestoreGUI();
            Repaint();
        }

        public void CreateConditionalGUI()
        {
            rootVisualElement.Clear();

            Label _warningLabel;
            if (ticket)
            {
                Label _titleLabel = new Label(ticket.data.name);
                _titleLabel.style.fontSize = 10;
                _titleLabel.style.flexDirection = FlexDirection.Row;
                _titleLabel.style.alignSelf = Align.FlexEnd;
                _titleLabel.style.whiteSpace = WhiteSpace.Normal;
                _titleLabel.style.flexWrap = Wrap.Wrap;
                rootVisualElement.Add(_titleLabel);
                _warningLabel = CreateCenteredBoldLabel("Be careful, this ticket is conditional.\nYou should run some tests on your project to decide wether or not to use this feature.");
            }
            else
                _warningLabel = CreateCenteredBoldLabel($"Be careful, you have {count} conditional tickets.\nYou should run some tests on your project to decide wether or not to use those features.");
            rootVisualElement.Add(_warningLabel);

            // Buttons
            VisualElement _buttonContainer = new VisualElement();
            _buttonContainer.style.flexDirection = FlexDirection.Row;
            _buttonContainer.style.justifyContent = Justify.Center;
            Button _proceedButton = CreateStyledButton(ticket ? "Proceed" : "Fix All", ticket ? () => OnButtonFixProceed() : () => OnButtonFixAll());
            Button _halfProceeButton = CreateStyledButton("Fix Non Conditional", () => OnButtonFixAllButExperimentals(), 160);
            Button _cancelButton = CreateStyledButton("Cancel", () => OnButtonCancel());
            _proceedButton.style.marginRight = 10;
            _halfProceeButton.style.marginRight = 10;
            _buttonContainer.Add(_proceedButton);
            if (!ticket)
                _buttonContainer.Add(_halfProceeButton);
            _buttonContainer.Add(_cancelButton);
            rootVisualElement.Add(_buttonContainer);
        }

        public void CreateRestoreGUI()
        {
            Label _warningLabel = CreateCenteredBoldLabel("This will restore the project to the last state before the last 'Fix All' operation.");
            _warningLabel.style.marginBottom = 5;
            rootVisualElement.Add(_warningLabel);
            Label _currentLabel = new Label($"You currently have {count} tickets saved from last your 'Fix All' : ");
            _currentLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _currentLabel.style.marginBottom = 10;
            rootVisualElement.Add(_currentLabel);
            ScrollView _scrollView = new ScrollView(ScrollViewMode.Vertical);
            _scrollView.style.alignSelf = Align.Center;
            _scrollView.style.width = 500;
            _scrollView.style.maxHeight = 100;
            _scrollView.style.backgroundColor = Color.black;

            Label _ticketsLabel = CreateCenteredBoldLabel(savedTickets);
            _scrollView.Add(_ticketsLabel);
            rootVisualElement.Add(_scrollView);
            Label _proceedLabel = CreateCenteredBoldLabel("Are you sure you wish to proceed ?");
            rootVisualElement.Add(_proceedLabel);

            // Buttons
            VisualElement _buttonContainer = new VisualElement();
            _buttonContainer.style.flexDirection = FlexDirection.Row;
            _buttonContainer.style.justifyContent = Justify.Center;
            Button _proceedButton = CreateStyledButton("Restore", () => OnButtonRestore());
            Button _cancelButton = CreateStyledButton("Cancel", () => OnButtonCancel());
            _buttonContainer.Add(_proceedButton);
            _buttonContainer.Add(_cancelButton);
            rootVisualElement.Add(_buttonContainer);
        }

        Button CreateStyledButton(string text, Action onClick, int _width = 100)
        {
            Button _button = new Button(onClick);
            _button.text = text;
            _button.style.width = _width;
            _button.style.height = 30;
            _button.style.alignSelf = Align.Center;
            _button.style.marginTop = 10;
            return _button;
        }

        Label CreateCenteredBoldLabel(string _text)
        {
            Label _label = new Label(_text);
            _label.style.alignSelf = Align.Center;
            _label.style.unityTextAlign = TextAnchor.MiddleCenter;
            _label.style.unityFontStyleAndWeight = FontStyle.Bold;
            _label.style.whiteSpace = WhiteSpace.Normal;
            _label.style.flexWrap = Wrap.Wrap;
            _label.style.marginTop = 15;
            _label.style.marginBottom = 10;
            return _label;
        }

        void OnButtonFixProceed()
        {
            AVRO_Utilities.FixTicket(ticket);
            ticket.data.state = AVRO_Settings.TicketStates.Done;
            SerializedProperty ticketsProperty = AVRO_Settings.GetSerializedSettings().FindProperty("Tickets");
            AVRO_Utilities.UpdateTicketCounts(ticketsProperty);
            AVRO_Utilities.AnalyzeTicket(ticket);
            settingsInstance.PopupWindow.Close();
        }

        void OnButtonFixAll()
        {
            var _ticketsProperty = AVRO_Settings.GetSerializedSettings().FindProperty("Tickets");
            for (int i = 0; i < _ticketsProperty.arraySize; i++)
            {
                var _ticketProperty = _ticketsProperty.GetArrayElementAtIndex(i);
                AVRO_Ticket _ticketInstance = _ticketProperty.objectReferenceValue as AVRO_Ticket;
                if (!_ticketInstance.data.hide && !_ticketInstance.data.noFix && !_ticketInstance.data.noRestore && _ticketInstance.data.state == AVRO_Settings.TicketStates.Todo && AVRO_Utilities.FixTicket(_ticketInstance, true))
                {
                    _ticketInstance.data.savedInFixAll = true;
                    _ticketInstance.data.state = AVRO_Settings.TicketStates.Done;
                }
                else if (_ticketInstance.data.state != AVRO_Settings.TicketStates.Todo)
                {
                    _ticketInstance.data.savedInFixAll = false;
                }
                EditorUtility.SetDirty(_ticketInstance);
            }
            AVRO_Utilities.AnalyzeProject(_ticketsProperty);
            AVRO_Utilities.UpdateTicketCounts(_ticketsProperty);
            AVRO_Settings.SaveSettings();
            settingsInstance.PopupWindow.Close();
        }

        void OnButtonFixAllButExperimentals()
        {
            var _ticketsProperty = AVRO_Settings.GetSerializedSettings().FindProperty("Tickets");
            for (int i = 0; i < _ticketsProperty.arraySize; i++)
            {
                var _ticketProperty = _ticketsProperty.GetArrayElementAtIndex(i);
                AVRO_Ticket _ticketInstance = _ticketProperty.objectReferenceValue as AVRO_Ticket;
                if ((_ticketInstance.data.tags & AVRO_Settings.TicketTags.Conditional) != AVRO_Settings.TicketTags.Conditional &&
                    !_ticketInstance.data.hide && !_ticketInstance.data.noFix && !_ticketInstance.data.noRestore && _ticketInstance.data.state == AVRO_Settings.TicketStates.Todo && AVRO_Utilities.FixTicket(_ticketInstance))
                {
                    _ticketInstance.data.savedInFixAll = true;
                    _ticketInstance.data.state = AVRO_Settings.TicketStates.Done;
                }
                else if (_ticketInstance.data.state != AVRO_Settings.TicketStates.Todo)
                {
                    _ticketInstance.data.savedInFixAll = false;
                }
                EditorUtility.SetDirty(_ticketInstance);
            }
            AVRO_Utilities.AnalyzeProject(_ticketsProperty);
            AVRO_Utilities.UpdateTicketCounts(_ticketsProperty);
            AVRO_Settings.SaveSettings();
            settingsInstance.PopupWindow.Close();
        }

        void OnButtonRestore()
        {
            AVRO_Utilities.RestoreProject(AVRO_Settings.GetSerializedSettings().FindProperty("Tickets"));
            settingsInstance.PopupWindow.Close();
        }

        void OnButtonCancel()
        {
            settingsInstance.PopupWindow.Close();
        }
    }
    #endregion
}