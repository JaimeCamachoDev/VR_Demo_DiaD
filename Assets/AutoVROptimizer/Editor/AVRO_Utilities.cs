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
using System.Reflection;
using UnityEngine.Rendering;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using System.Linq;
using System.Collections;
using System.Globalization;
namespace AVRO
{
    public class AVRO_Utilities
    {
        #region Utilities Methods
        public static Texture2D LoadTexture(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        public static Texture2D ResizeTexture(Texture2D source, int width, int height)
        {
            RenderTexture rt = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(source, rt);

            Texture2D result = new Texture2D(width, height);
            RenderTexture.active = rt;
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();

            RenderTexture.ReleaseTemporary(rt);
            return result;
        }

        public static void UpdateTicketCounts(SerializedProperty _ticketsProperty)
        {
            var settingsInstance = AVRO_Settings.GetOrCreateSettings();
            settingsInstance.TodoCount = 0;
            settingsInstance.DoneCount = 0;
            settingsInstance.IgnoreCount = 0;
            settingsInstance.OmittedCount = 0;
            settingsInstance.BigTodoCount = 0;
            settingsInstance.BigDoneCount = 0;
            settingsInstance.BigIgnoreCount = 0;
            settingsInstance.BigOmittedCount = 0;
            settingsInstance.SavedCount = 0;
            for (int i = 0; i < _ticketsProperty.arraySize; i++)
            {
                var _ticketProperty = _ticketsProperty.GetArrayElementAtIndex(i);
                AVRO_Ticket _ticketInstance = _ticketProperty.objectReferenceValue as AVRO_Ticket;
                bool _isFlagSelected = (_ticketInstance.data.tags & settingsInstance.FilteredTags) != 0;
                if (!_ticketInstance.data.hide && _isFlagSelected)
                    switch (_ticketInstance.data.state)
                    {
                        default:
                            break;
                        case AVRO_Settings.TicketStates.Todo:
                            settingsInstance.TodoCount++;
                            if (_ticketInstance.IsBigTicket)
                                settingsInstance.BigTodoCount++;
                            break;
                        case AVRO_Settings.TicketStates.Done:
                            settingsInstance.DoneCount++;
                            if (_ticketInstance.IsBigTicket)
                                settingsInstance.BigDoneCount++;
                            break;
                        case AVRO_Settings.TicketStates.Ignore:
                            settingsInstance.IgnoreCount++;
                            if (_ticketInstance.IsBigTicket)
                                settingsInstance.BigIgnoreCount++;
                            break;
                        case AVRO_Settings.TicketStates.Omitted:
                            settingsInstance.OmittedCount++;
                            if (_ticketInstance.IsBigTicket)
                                settingsInstance.BigOmittedCount++;
                            break;
                    }
                if (!_ticketInstance.data.noRestore && _ticketInstance.data.savedInFixAll)
                    settingsInstance.SavedCount++;
            }
            settingsInstance.CanRestore = settingsInstance.SavedCount > 0;
            AVRO_Settings.SaveSettings();
        }

        public static void AnalyzeProject(SerializedProperty _ticketsProperty)
        {
            var settingsInstance = AVRO_Settings.GetOrCreateSettings();
            if (_ticketsProperty == null || _ticketsProperty.arraySize == 0)
            {
                Debug.LogError("Tickets list is null or empty.", settingsInstance);
                return;
            }

            CheckForConditions();
            for (int i = 0; i < _ticketsProperty.arraySize; i++)
            {
                var _ticketProperty = _ticketsProperty.GetArrayElementAtIndex(i);
                AVRO_Ticket _ticketInstance = _ticketProperty.objectReferenceValue as AVRO_Ticket;
                _ticketInstance.data.savedInFixAll = false;
                if (!_ticketInstance.data.hide && _ticketInstance.data.state != AVRO_Settings.TicketStates.Ignore)
                {
                    AnalyzeTicket(_ticketInstance);
                    EditorUtility.SetDirty(_ticketInstance);
                }
                if (settingsInstance.InfoWindow && TicketWindow.ticket == _ticketInstance)
                    TicketWindow.ShowTicketWindow(_ticketInstance);
            }
            UpdateTicketCounts(_ticketsProperty);
            settingsInstance.FirstTimeSetup = false;
            settingsInstance.CanRestore = false;
            AVRO_Settings.SaveSettings();
        }

        public static void FixAllProject(SerializedProperty _ticketsProperty)
        {
            var settingsInstance = AVRO_Settings.GetOrCreateSettings();
            int _ExperimentalCount = 0;
            for (int i = 0; i < _ticketsProperty.arraySize; i++)
            {
                var _ticketProperty = _ticketsProperty.GetArrayElementAtIndex(i);
                AVRO_Ticket _ticketInstance = _ticketProperty.objectReferenceValue as AVRO_Ticket;
                if (!_ticketInstance.data.hide && !_ticketInstance.data.noFix && _ticketInstance.data.state == AVRO_Settings.TicketStates.Todo && (_ticketInstance.data.tags & AVRO_Settings.TicketTags.Conditional) == AVRO_Settings.TicketTags.Conditional)
                    _ExperimentalCount++;
                if (settingsInstance.InfoWindow && TicketWindow.ticket == _ticketInstance)
                    TicketWindow.ShowTicketWindow(_ticketInstance);
            }
            if (_ExperimentalCount <= 0)
            {
                for (int i = 0; i < _ticketsProperty.arraySize; i++)
                {
                    var _ticketProperty = _ticketsProperty.GetArrayElementAtIndex(i);
                    AVRO_Ticket _ticketInstance = _ticketProperty.objectReferenceValue as AVRO_Ticket;
                    if (!_ticketInstance.data.hide && !_ticketInstance.data.noFix && !_ticketInstance.data.noRestore && _ticketInstance.data.state == AVRO_Settings.TicketStates.Todo && FixTicket(_ticketInstance, true))
                    {
                        _ticketInstance.data.savedInFixAll = true;
                        _ticketInstance.data.state = AVRO_Settings.TicketStates.Done;
                    }
                    else if (_ticketInstance.data.state != AVRO_Settings.TicketStates.Todo)
                    {
                        _ticketInstance.data.savedInFixAll = false;
                    }
                    EditorUtility.SetDirty(_ticketInstance);
                    UpdateTicketCounts(_ticketsProperty);
                }
                AnalyzeProject(_ticketsProperty);
            }
            else
            {
                PopupWindow.ShowPopupConditionalWindow(null, _ExperimentalCount);
            }
            AVRO_Settings.SaveSettings();
        }

        public static void RestoreProject(SerializedProperty _ticketsProperty)
        {
            var settingsInstance = AVRO_Settings.GetOrCreateSettings();
            for (int i = 0; i < _ticketsProperty.arraySize; i++)
            {
                var _ticketProperty = _ticketsProperty.GetArrayElementAtIndex(i);
                AVRO_Ticket _ticketInstance = _ticketProperty.objectReferenceValue as AVRO_Ticket;
                if (!_ticketInstance.data.hide && !_ticketInstance.data.noFix && !_ticketInstance.data.noRestore && _ticketInstance.data.savedInFixAll && _ticketInstance.data.state != AVRO_Settings.TicketStates.Ignore)
                {
                    FixTicket(_ticketInstance, false, true);
                    AnalyzeTicket(_ticketInstance);
                    EditorUtility.SetDirty(_ticketInstance);
                }
            }
            settingsInstance.CanRestore = false;
            UpdateTicketCounts(_ticketsProperty);
            AVRO_Settings.SaveSettings();
        }

        /// <summary>
        /// Returns either the wanted or last saved value based on the _restore parameter.
        /// </summary>
        /// <param name="_restore">The passed _restore parameter from the parent method, determines which value to choose.</param>
        /// <param name="_target">The wanted value for the current ticket.</param>
        /// <param name="_savedValue">The current ticket.data.lastValues in case _restore is true.</param>
        /// <returns></returns>
        public static T CheckRestoreValue<T>(bool _restore, T _target, AVRO_Ticket _ticket = null, int _index = 0)
        {
            if (_ticket && _ticket.data.lastValues.Count <= _index)
                _ticket.data.lastValues.Add(null);
            if (_ticket && _ticket.data.concernedObjects.Count > _index && _ticket.data.lastObjects.Count <= _index)
                _ticket.data.lastObjects.Add(null);

            if (!_restore)
                return _target;

            if (typeof(T) == null)
                return (T)(object)null;

            if (typeof(T) == typeof(string))
                return (T)(object)_ticket.data.lastValues[_index];

            if (typeof(T) == typeof(bool))
            {
                bool _result = _restore ? !(bool)(object)_target : (bool)(object)_target;
                return (T)(object)_result;
            }

            if (typeof(T) == typeof(int) && int.TryParse(_ticket.data.lastValues[_index], out var _intVal))
                return (T)(object)_intVal;

            if (typeof(T) == typeof(float) && float.TryParse(_ticket.data.lastValues[_index], out var _floatVal))
                return (T)(object)_floatVal;

            if (typeof(T) == typeof(Vector3))
            {
                string _raw = _ticket.data.lastValues[_index].Replace("(", "").Replace(")", "");
                string[] _parts = _raw.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (_parts.Length == 3 &&
                    float.TryParse(_parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
                    float.TryParse(_parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y) &&
                    float.TryParse(_parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
                    return (T)(object)new Vector3(x, y, z);
            }

            if (typeof(T).IsEnum && Enum.TryParse(typeof(T), _ticket.data.lastValues[_index], out var _enumVal) && _enumVal is T _typedEnumVal)
                return _typedEnumVal;

            throw new InvalidOperationException($"Cannot restore value '{_ticket.data.lastValues[0]}' to type {typeof(T).Name}.");
        }

        public static void ResetAllTickets(List<AVRO_Ticket> _tickets)
        {
            var settingsInstance = AVRO_Settings.GetOrCreateSettings();
            foreach (var _ticket in _tickets)
            {
                _ticket.data.state = AVRO_Settings.TicketStates.Todo;
                _ticket.data.concernedObjects = new List<AVRO_Settings.Ticket.ConcernedObjectData>();
            }
            settingsInstance.SortedBy = AVRO_Settings.SortingOrders.Tags;
            settingsInstance.FilteredTags = (AVRO_Settings.TicketTags)~0;
            settingsInstance.ShowToDo = true;
            settingsInstance.ShowDone = false;
            settingsInstance.ShowIgnore = false;
            settingsInstance.ShowOmitted = false;
            SortOutTicketsBy(settingsInstance.SortedBy, _tickets);
        }

#pragma warning disable UDR0001 // Domain Reload Analyzer
        static List<AVRO_Settings.TicketLevels> levelOrder = new List<AVRO_Settings.TicketLevels>()
        {
            AVRO_Settings.TicketLevels.Required,
            AVRO_Settings.TicketLevels.Recommended,
            AVRO_Settings.TicketLevels.Optional,
            AVRO_Settings.TicketLevels.Information
        };
#pragma warning restore UDR0001 // Domain Reload Analyzer
        public static void SortOutTicketsBy(AVRO_Settings.SortingOrders _sortBy, List<AVRO_Ticket> _tickets)
        {
            _tickets.Sort((a, b) =>
            {
                if (_sortBy == AVRO_Settings.SortingOrders.Priority)
                {
                    // Level order
                    int _aLevelIndex = levelOrder.IndexOf(a.data.level);
                    int _bLevelIndex = levelOrder.IndexOf(b.data.level);
                    if (_aLevelIndex != _bLevelIndex) return _aLevelIndex.CompareTo(_bLevelIndex);
                    // Tag order
                    int _aTagValue = GetFirstTagValue(a.data.tags);
                    int _bTagValue = GetFirstTagValue(b.data.tags);
                    if (_aTagValue != _bTagValue) return _aTagValue.CompareTo(_bTagValue);
                }
                else
                {
                    // Tag order
                    int _aTagValue = GetFirstTagValue(a.data.tags);
                    int _bTagValue = GetFirstTagValue(b.data.tags);
                    if (_aTagValue != _bTagValue) return _aTagValue.CompareTo(_bTagValue);
                    // Level order
                    int _aLevelIndex = levelOrder.IndexOf(a.data.level);
                    int _bLevelIndex = levelOrder.IndexOf(b.data.level);
                    if (_aLevelIndex != _bLevelIndex) return _aLevelIndex.CompareTo(_bLevelIndex);
                }
                // Name order
                return string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
            });
        }

        private static int GetFirstTagValue(AVRO_Settings.TicketTags tags)
        {
            foreach (AVRO_Settings.TicketTags tag in Enum.GetValues(typeof(AVRO_Settings.TicketTags)))
            {
                if (tag == AVRO_Settings.TicketTags.None) continue;
                if (tags.HasFlag(tag)) return (int)tag;
            }
            return int.MaxValue;
        }
        #endregion

        #region Function Methods

#pragma warning disable UDR0001 // Domain Reload Analyzer
        public static bool IsUsingUnityURP = false;
        public static bool IsRendererSelected = false;
        public static bool IsXRManagementInstalled = false;
        public static bool IsMetaInstalled = false;
        public static bool IsOculusXRInstalled = false;
        public static bool IsOpenXRInstalled = false;
        public static bool IsAndroidModuleInstalled = false;
        public static bool IsVulkanUsed = false;
        public static object OculusSettings;
        public static object URPSettings;
        public static string OVRManagerGUID;
        public static object OpenXRSettingsDictionary;
        public static AVRO_Settings Settings;
        static Type[] functionTypes = {
        typeof(AVRO_Functions_Lite),
        typeof(AVRO_Functions_Pro),
        typeof(AVRO_Functions_Customize)
        };
        public static XRLoaderNames XRLoaderName = XRLoaderNames.None;
        public enum XRLoaderNames
        {
            None, Oculus, OpenXR, Other
        }
        static Dictionary<string, MethodInfo> analysisMethods = new Dictionary<string, MethodInfo>();
        static Dictionary<string, MethodInfo> fixMethods = new Dictionary<string, MethodInfo>();
#pragma warning restore UDR0001 // Domain Reload Analyzer
        public static void AnalyzeTicket(AVRO_Ticket _ticket)
        {
            _ticket.data.state = InvokeReturnBool("Get" + _ticket.data.functionName, _ticket);
        }
        static AVRO_Settings.TicketStates InvokeReturnBool(string _methodName, AVRO_Ticket _ticket)
        {
            if (Settings == null)
                Settings = AVRO_Settings.GetOrCreateSettings();
            if (!analysisMethods.TryGetValue(_methodName, out MethodInfo _method))
            {
                foreach (var _type in functionTypes)
                {
                    _method = _type.GetMethod(_methodName);
                    if (_method != null && _method.ReturnType == typeof(AVRO_Settings.TicketStates))
                    {
                        analysisMethods[_methodName] = _method;
                        break;
                    }
                }
            }
            if (_method != null)
                return (AVRO_Settings.TicketStates)_method.Invoke(null, new object[] { _ticket });
            if (Settings.EnableDebug)
                Debug.LogError($"Method '{_methodName}' not found in AVRO_Functions or does not return a bool.");
            return AVRO_Settings.TicketStates.Omitted;
        }
        public static bool FixTicket(AVRO_Ticket _ticket, bool _fromFixAll = false, bool _restore = false)
        {
            _ticket.data.savedInFixAll = _fromFixAll;
            return InvokeMethod("Set" + _ticket.data.functionName, _ticket, _restore);
        }
        static bool InvokeMethod(string _methodName, AVRO_Ticket _ticket, bool _restore = false)
        {
            if (Settings == null)
                Settings = AVRO_Settings.GetOrCreateSettings();
            if (!fixMethods.TryGetValue(_methodName, out MethodInfo _method))
            {
                foreach (var _type in functionTypes)
                {
                    _method = _type.GetMethod(_methodName);
                    if (_method != null)
                    {
                        fixMethods[_methodName] = _method;
                        break;
                    }
                }
            }
            if (_method != null)
            {
                _method.Invoke(null, new object[] { _ticket, _restore });
                return true;
            }
            if (Settings.EnableDebug)
                Debug.LogError($"Method '{_methodName}' not found in AVRO_Functions.");
            return false;
        }

        public static void CheckForConditions()
        {
            AssetDatabase.Refresh();
            IsRendererSelected = GraphicsSettings.currentRenderPipeline && GraphicsSettings.defaultRenderPipeline || QualitySettings.renderPipeline;
            IsMetaInstalled = IsPackageInstalled("com.meta.xr.sdk.all");
            IsOpenXRInstalled = IsPackageInstalled("com.unity.xr.openxr");
            IsOculusXRInstalled = IsPackageInstalled("com.unity.xr.oculus");
            IsAndroidModuleInstalled = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android);
#if UNITY_ANDROID
            IsVulkanUsed = AVRO_Functions_Lite.GetVulkanGraphicsAPI(null) == AVRO_Settings.TicketStates.Done;
#endif
            // Use Reflection to get OculusSettings in case it's not installed
            // TODO : Check versioning for either IsOculusXRInstalled or IsMetaInstalled
            Type oculusSettingsType = Type.GetType("Unity.XR.Oculus.OculusSettings, Unity.XR.Oculus");
            if (oculusSettingsType != null)
            {
                var _instances = Resources.FindObjectsOfTypeAll(oculusSettingsType);
                OculusSettings = _instances.Length > 0 ? _instances[0] : null;
            }
            else
            {
                OculusSettings = null;
            }
            Type ovrManagerType = Type.GetType("OVRManager, Oculus.VR");
            if (IsMetaInstalled && ovrManagerType != null)
            {
#if UNITY_2021_1_OR_NEWER
                var _obj = GameObject.FindFirstObjectByType(ovrManagerType);
#else
                var _obj = UnityEngine.Object.FindObjectOfType(ovrManagerType);
#endif
                OVRManagerGUID = GlobalObjectId.GetGlobalObjectIdSlow(_obj).ToString();
            }
            // Use Reflection to get URPSettings in case it's not installed
            Type URPSettingsType = Type.GetType("UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset, Unity.RenderPipelines.Universal.Runtime");
            if (URPSettingsType != null)
            {
                var _instances = Resources.FindObjectsOfTypeAll(URPSettingsType);
                URPSettings = _instances.Length > 0 ? _instances[0] : null;
            }
            else
            {
                URPSettings = null;
            }
            IsUsingUnityURP = URPSettingsType != null;
            // Get selected XR Loader
            XRLoaderName = GetActiveXRLoaderName();
            // Get OpenXR Settings
            GetOpenXRDictionary();
            // Get current build targets
            AVRO_Settings.GetCurrentBuildTargets();
        }
        private static bool IsPackageInstalled(string _packageName)
        {
            ListRequest _request = Client.List(true);
            bool _isInstalled = false;
            while (!_request.IsCompleted)
                _isInstalled = false;

            if (_request.Status == StatusCode.Success)
            {
                foreach (var _package in _request.Result)
                {
                    if (_package.name == _packageName)
                    {
                        _isInstalled = true;
                        break;
                    }
                }
            }
            return _isInstalled;
        }
        public static void SetDirtyReflectionOf(object _settings)
        {
            MethodInfo _setDirtyMethod = typeof(EditorUtility).GetMethod("SetDirty", new Type[] { typeof(UnityEngine.Object) });
            _setDirtyMethod?.Invoke(null, new object[] { _settings });
        }

        public static XRLoaderNames GetActiveXRLoaderName()
        {
            Type _xrSettingsPerBuildTargetType = Type.GetType("UnityEditor.XR.Management.XRGeneralSettingsPerBuildTarget, Unity.XR.Management.Editor");
            IsXRManagementInstalled = _xrSettingsPerBuildTargetType != null;
            if (!IsXRManagementInstalled) return XRLoaderNames.None;

            MethodInfo _getSettingsMethod = _xrSettingsPerBuildTargetType.GetMethod("XRGeneralSettingsForBuildTarget", BindingFlags.Public | BindingFlags.Static);
            if (_getSettingsMethod == null) return XRLoaderNames.None;
            object _settings = _getSettingsMethod.Invoke(null, new object[] { BuildTargetGroup.Android });
            if (_settings == null) return XRLoaderNames.None;

            PropertyInfo _managerProp = _settings.GetType().GetProperty("Manager", BindingFlags.Public | BindingFlags.Instance);
            object _manager = _managerProp?.GetValue(_settings);
            if (_manager == null) return XRLoaderNames.None;

            PropertyInfo _loadersProp = _manager.GetType().GetProperty("activeLoaders", BindingFlags.Public | BindingFlags.Instance);
            var _loaders = _loadersProp?.GetValue(_manager) as System.Collections.IEnumerable;
            if (_loaders == null) return XRLoaderNames.None;

            bool _anyUsed = false;
            foreach (var _loader in _loaders)
            {
                if (_loader == null) continue;
                _anyUsed = true;
                string _name = _loader.GetType().Name;
                if (_name.Contains("Oculus"))
                    return XRLoaderNames.Oculus;
                else if (_name.Contains("OpenXR"))
                    return XRLoaderNames.OpenXR;
            }
            return _anyUsed ? XRLoaderNames.Other : XRLoaderNames.None;
        }

        static void GetOpenXRDictionary()
        {
            var _openXRSettingsType = Type.GetType("UnityEngine.XR.OpenXR.OpenXRSettings, Unity.XR.OpenXR");
            if (_openXRSettingsType == null) return;

            // Use AssetDatabase.FindAssets to get all OpenXRSettings assets
            string[] _guids = AssetDatabase.FindAssets("t:" + _openXRSettingsType.Name);
            UnityEngine.Object _packageSettingsAsset = null;
            foreach (var _guid in _guids)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_guid);
                var _asset = AssetDatabase.LoadMainAssetAtPath(_path);
                if (_asset == null) continue;

                if (_asset.GetType().FullName.Contains("OpenXRPackageSettings"))
                {
                    _packageSettingsAsset = _asset;
                    break;
                }
            }
            if (_packageSettingsAsset == null) return;

            // Use SerializedObject to access the internal "Settings" dictionary - May be "m_Settings" in older versions
            var _pkgSettingsType = _packageSettingsAsset.GetType();
            var _mSettingsField = _pkgSettingsType.GetField("Settings", BindingFlags.NonPublic | BindingFlags.Instance);

            if (_mSettingsField == null) return;
            OpenXRSettingsDictionary = _mSettingsField.GetValue(_packageSettingsAsset);
        }

        public static int SetOpenXRDictionaryValue(string _keyName, bool _set = false, bool _value = true)
        {
            var _dictType = OpenXRSettingsDictionary.GetType();
            var _keysProp = _dictType.GetProperty("Keys");
            var _getItemMethod = _dictType.GetMethod("get_Item");
            foreach (var _key in (IEnumerable)_keysProp.GetValue(OpenXRSettingsDictionary))
            {
                // 7 = Android in UnityEditor.BuildTargetGroup
                if ((int)(object)_key == 7)
                {
                    var _androidSettings = _getItemMethod.Invoke(OpenXRSettingsDictionary, new object[] { _key });
                    if (_androidSettings != null)
                    {
                        var _openXRSettingsType = Type.GetType("UnityEngine.XR.OpenXR.OpenXRSettings, Unity.XR.OpenXR");
                        var _featuresField = _openXRSettingsType.GetField("features", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (_featuresField == null) return -1;

                        var _featuresList = _featuresField.GetValue(_androidSettings) as IEnumerable;
                        if (_featuresList == null) return -1;

                        foreach (var _feature in _featuresList)
                        {
                            var _type = _feature.GetType();
                            string _name = _type.FullName;
                            string _key1 = _keyName.Split('@').First();
                            string _key2 = _keyName.Split('@').Last();
                            if (!string.IsNullOrEmpty(_name) && _name.Contains(_key1))
                            {
                                if (_key1 != "MetaQuestFeature")
                                {
                                    PropertyInfo _enabledProp = _type.GetProperty("enabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                    if (_enabledProp == null) return -1;
                                    if (_set)
                                    {
                                        _enabledProp.SetValue(_feature, _value);
                                        SetDirtyReflectionOf(_androidSettings);
                                        AssetDatabase.SaveAssets();
                                    }
                                    return (bool)_enabledProp.GetValue(_feature) ? 1 : 0;
                                }
                                else if (_key1 != _key2)
                                {
                                    if (_key2 != "targetDevices")
                                    {
                                        var _field = _type.GetField(_key2, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                                        if (_field == null) return -1;
                                        if (_set)
                                        {
                                            _field.SetValue(_feature, _value);
                                            SetDirtyReflectionOf(_androidSettings);
                                            AssetDatabase.SaveAssets();
                                        }
                                        return (bool)_field.GetValue(_feature) ? 1 : 0;
                                    }
                                    else
                                    {
                                        var _targetDevicesField = _type.GetField("targetDevices", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                                        if (_targetDevicesField != null)
                                        {
                                            var _targetDevices = _targetDevicesField.GetValue(_feature) as IEnumerable;
                                            if (_targetDevices != null)
                                            {
                                                bool _atLeastOneEnabled = false;
                                                var _newList = (IList)Activator.CreateInstance(_targetDevices.GetType());
                                                foreach (var device in _targetDevices)
                                                {
                                                    var _devType = device.GetType();
                                                    var _nameField = _devType.GetField("visibleName");
                                                    var _enabledField = _devType.GetField("enabled");
                                                    // Create a copy of the array and paste all fields from the original
                                                    var _newDevice = Activator.CreateInstance(_devType);
                                                    foreach (var _f in _devType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                                                        _f.SetValue(_newDevice, _f.GetValue(device));
                                                    if (_set && _nameField?.GetValue(device)?.ToString() != "Quest")
                                                        _enabledField.SetValue(_newDevice, _value);
                                                    else if ((bool)_enabledField?.GetValue(device))
                                                        _atLeastOneEnabled = true;
                                                    _newList.Add(_newDevice);
                                                }
                                                if (_set)
                                                {
                                                    _targetDevicesField.SetValue(_feature, _newList);
                                                    EditorUtility.SetDirty((UnityEngine.Object)_feature);
                                                    AssetDatabase.SaveAssets();
                                                }
                                                else
                                                    return _atLeastOneEnabled ? 1 : 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }

        [InitializeOnLoadMethod]
        static void ClearCacheOnDomainReload()
        {
            analysisMethods.Clear();
            fixMethods.Clear();
        }
        #endregion
    }
}