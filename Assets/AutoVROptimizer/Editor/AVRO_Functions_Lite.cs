/*
Copyright (c) 2025 Valem Studio

This asset is the intellectual property of Valem Studio and is distributed under the Unity Asset Store End User License Agreement (EULA).

Unauthorized reproduction, modification, or redistribution of any part of this asset outside the terms of the Unity Asset Store EULA is strictly prohibited.

For support or inquiries, please contact Valem Studio via social media or through the publisher profile on the Unity Asset Store.
*/

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Reflection;
using UnityEditor.Build;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.Rendering.Universal;
namespace AVRO
{
    public class AVRO_Functions_Lite
    {
        #region Project Settings
        //----------------------------------------
        //  PROJECT SETTINGS
        //----------------------------------------
        // Project 00 - Check if Unity6 (or recent) is used
        // NO FIX
        public static AVRO_Settings.TicketStates GetUsingUnity6OrAbove(AVRO_Ticket _ticket)
        {
#if UNITY_2023_1_OR_NEWER
            return AVRO_Settings.TicketStates.Done;
#else
            return AVRO_Settings.TicketStates.Todo;
#endif
        }

        // Project 01 - Check if URP is in use
        // NO FIX
        public static AVRO_Settings.TicketStates GetUsingURP(AVRO_Ticket _ticket)
        {
            return AVRO_Utilities.IsUsingUnityURP ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }

        // Project 02 - Donwload Android Module
        // NO FIX
        public static AVRO_Settings.TicketStates GetAndroidModule(AVRO_Ticket _ticket)
        {
            return AVRO_Utilities.IsAndroidModuleInstalled ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }

        // Project 03 - Switch To Android
        public static AVRO_Settings.TicketStates GetAndroidPlatform(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsAndroidModuleInstalled)
                return AVRO_Settings.TicketStates.Omitted;
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetAndroidPlatform(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, BuildTarget.Android, _ticket);
            _ticket.data.lastValues[0] = !_restore ? EditorUserBuildSettings.activeBuildTarget.ToString() : null;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(_targetValue), _targetValue);
        }

        // Project 04 - OpenXR Package
        // NO FIX
        public static AVRO_Settings.TicketStates GetOpenXRPackage(AVRO_Ticket _ticket)
        {
            return AVRO_Utilities.IsOpenXRInstalled ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }

        // Project 06 - XRManagement Package
        // NO FIX
        public static AVRO_Settings.TicketStates GetXRManagementPackage(AVRO_Ticket _ticket)
        {
            return AVRO_Utilities.IsXRManagementInstalled ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        #endregion

        #region Player Settings
        //----------------------------------------
        //  PLAYER SETTINGS
        //----------------------------------------
        // Player 01 - Linear Color Space
        public static AVRO_Settings.TicketStates GetColorSpaceLinear(AVRO_Ticket _ticket)
        {
            return PlayerSettings.colorSpace == ColorSpace.Linear ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetColorSpaceLinear(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, ColorSpace.Linear, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.colorSpace.ToString() : null;
            PlayerSettings.colorSpace = _targetValue;
        }

        // Player 02 - MSAA Downgrade
        // A bit hacky, we overwrite the project settings file's text itself because it can't be accessed.
        public static AVRO_Settings.TicketStates GetMSAAFallbackDowngrade(AVRO_Ticket _ticket)
        {
            string _path = "ProjectSettings/ProjectSettings.asset";
            if (!File.Exists(_path))
                return AVRO_Settings.TicketStates.Omitted;
            string _content = File.ReadAllText(_path);
            if (!_content.Contains("unsupportedMSAAFallback"))
                return AVRO_Settings.TicketStates.Omitted;
            return Regex.IsMatch(_content, @"unsupportedMSAAFallback:\s*0") ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetMSAAFallbackDowngrade(AVRO_Ticket _ticket, bool _restore = false)
        {
            string _path = "ProjectSettings/ProjectSettings.asset";
            string _content = File.ReadAllText(_path);
            if (!File.Exists(_path)) return;
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, 0, _ticket);
            _ticket.data.lastValues[0] = !_restore ? Regex.Match(_content, @"unsupportedMSAAFallback:\s*(\S+)").Groups[1].Value : null;
            string _newContent = Regex.Replace(_content, @"(unsupportedMSAAFallback:\s*)\S+", "${1}" + _targetValue);
            File.WriteAllText(_path, _newContent);
            AssetDatabase.Refresh();
        }

        // Player 04 - GraphicsJobs (experimental)
        public static AVRO_Settings.TicketStates GetGraphicsJobsEnabled(AVRO_Ticket _ticket)
        {
            return PlayerSettings.graphicsJobs ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetGraphicsJobsEnabled(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.graphicsJobs.ToString() : null;
            PlayerSettings.graphicsJobs = _targetValue;
        }

        // Player 11 - PrebakeCollisionMeshes
        public static AVRO_Settings.TicketStates GetPrebakeCollisionMeshes(AVRO_Ticket _ticket)
        {
            return PlayerSettings.bakeCollisionMeshes ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetPrebakeCollisionMeshes(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.graphicsJobs.ToString() : null;
            PlayerSettings.bakeCollisionMeshes = _targetValue;
        }

        // Player 12 - Optimize Mesh data
        public static AVRO_Settings.TicketStates GetOptimizeMeshData(AVRO_Ticket _ticket)
        {
            return PlayerSettings.stripUnusedMeshComponents ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetOptimizeMeshData(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.stripUnusedMeshComponents.ToString() : null;
            PlayerSettings.stripUnusedMeshComponents = _targetValue;
        }

        // Player 13 - MipmapStripping
        public static AVRO_Settings.TicketStates GetMipmapStripping(AVRO_Ticket _ticket)
        {
            return PlayerSettings.mipStripping ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetMipmapStripping(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.mipStripping.ToString() : null;
            PlayerSettings.mipStripping = _targetValue;
        }

        // Player 16 - Managed Stripping Level
        public static AVRO_Settings.TicketStates GetStrippingLevel(AVRO_Ticket _ticket)
        {
            return PlayerSettings.GetManagedStrippingLevel(AVRO_Settings.ReturnBuildType<NamedBuildTarget>()) == ManagedStrippingLevel.Medium ||
            PlayerSettings.GetManagedStrippingLevel(AVRO_Settings.ReturnBuildType<NamedBuildTarget>()) == ManagedStrippingLevel.High ?
            AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetStrippingLevel(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, ManagedStrippingLevel.Medium, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.GetManagedStrippingLevel(AVRO_Settings.ReturnBuildType<NamedBuildTarget>()).ToString() : null;
            PlayerSettings.SetManagedStrippingLevel(AVRO_Settings.ReturnBuildType<NamedBuildTarget>(), _targetValue);
        }
        #region.  Android
#if UNITY_ANDROID
        // Player 03 - GraphicsAPI_Vulkan (AutoGraphicsAPI disabled + Vulkan chosen)
        public static AVRO_Settings.TicketStates GetDisableAutoGraphicsAPI(AVRO_Ticket _ticket)
        {
            bool _usingDefault = PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android);
            return !_usingDefault ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetDisableAutoGraphicsAPI(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, false, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android).ToString() : null;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, _targetValue);
        }

        // Player 07 - Multithreaded Rendering
        public static AVRO_Settings.TicketStates GetMultiThreadedRendering(AVRO_Ticket _ticket)
        {
            return PlayerSettings.GetMobileMTRendering(NamedBuildTarget.Android) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetMultiThreadedRendering(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.GetMobileMTRendering(NamedBuildTarget.Android).ToString() : null;
            PlayerSettings.SetMobileMTRendering(NamedBuildTarget.Android, _targetValue);
        }

        // Player 17 - TextureCompression_ASTC
        public static AVRO_Settings.TicketStates GetTextureCompressionASTC(AVRO_Ticket _ticket)
        {
            return EditorUserBuildSettings.androidBuildSubtarget == MobileTextureSubtarget.ASTC ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetTextureCompressionASTC(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, MobileTextureSubtarget.ASTC, _ticket);
            _ticket.data.lastValues[0] = !_restore ? EditorUserBuildSettings.androidBuildSubtarget.ToString() : null;
            EditorUserBuildSettings.androidBuildSubtarget = _targetValue;
        }

        // Player 20 - Use Vulkan Graphics API
        public static AVRO_Settings.TicketStates GetVulkanGraphicsAPI(AVRO_Ticket _ticket)
        {
            bool _usingDefault = PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android);
            if (_usingDefault) return AVRO_Settings.TicketStates.Omitted;
            GraphicsDeviceType[] _apiList = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
            return (_apiList != null && _apiList.Length > 0 && _apiList[0] == GraphicsDeviceType.Vulkan) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetVulkanGraphicsAPI(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, GraphicsDeviceType.Vulkan, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)[0].ToString() : null;
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { _targetValue });
        }
#endif
        #endregion
        #region.  Unity2023+
#if UNITY_2023_1_OR_NEWER
        // Player 08 - Static Batching
        public static AVRO_Settings.TicketStates GetStaticBatching(AVRO_Ticket _ticket)
        {
            return PlayerSettings.GetStaticBatchingForPlatform(AVRO_Settings.ReturnBuildType<BuildTarget>()) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetStaticBatching(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.GetStaticBatchingForPlatform(AVRO_Settings.ReturnBuildType<BuildTarget>()).ToString() : null;
            PlayerSettings.SetStaticBatchingForPlatform(AVRO_Settings.ReturnBuildType<BuildTarget>(), _targetValue);
        }

        // Player 14 - Minimum API Level 32
        public static AVRO_Settings.TicketStates GetMinAPILevel32(AVRO_Ticket _ticket)
        {
            return (int)PlayerSettings.Android.minSdkVersion >= 32 ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetMinAPILevel32(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, AndroidSdkVersions.AndroidApiLevel32, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.Android.minSdkVersion.ToString() : null;
            PlayerSettings.Android.minSdkVersion = _targetValue;
        }

        // Player 15 - TargetAPI 32
        public static AVRO_Settings.TicketStates GetTargetAPILevel32(AVRO_Ticket _ticket)
        {
            return ((int)PlayerSettings.Android.targetSdkVersion >= 32 ||
            (int)PlayerSettings.Android.minSdkVersion >= 32 && PlayerSettings.Android.targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto) ?
            AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetTargetAPILevel32(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, AndroidSdkVersions.AndroidApiLevel32, _ticket);
            _ticket.data.lastValues[0] = !_restore ? PlayerSettings.Android.targetSdkVersion.ToString() : null;
            PlayerSettings.Android.targetSdkVersion = _targetValue;
        }
#endif
        #endregion
        #endregion

        #region Quality Settings
        //----------------------------------------
        //  QUALITY SETTINGS
        //----------------------------------------
        // Quality 01 - RealtimeReflectionProbes
        public static AVRO_Settings.TicketStates GetRealtimeReflectionProbesDisabled(AVRO_Ticket _ticket)
        {
            return !QualitySettings.realtimeReflectionProbes ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetRealtimeReflectionProbesDisabled(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, false, _ticket);
            _ticket.data.lastValues[0] = !_restore ? QualitySettings.realtimeReflectionProbes.ToString() : null;
            QualitySettings.realtimeReflectionProbes = _targetValue;
        }

        // Quality 02 - Disable VSync
        public static AVRO_Settings.TicketStates GetVSyncDisabled(AVRO_Ticket _ticket)
        {
            return QualitySettings.vSyncCount == 0 ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetVSyncDisabled(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, 0, _ticket);
            _ticket.data.lastValues[0] = !_restore ? QualitySettings.vSyncCount.ToString() : null;
            QualitySettings.vSyncCount = _targetValue;
        }

        // Quality 03 - Full Resolution Mip Map
#if UNITY_2022_1_OR_NEWER
        public static AVRO_Settings.TicketStates GetFullResolutionMipMap(AVRO_Ticket _ticket)
        {
            return QualitySettings.globalTextureMipmapLimit == 0 ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetFullResolutionMipMap(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, 0, _ticket);
            _ticket.data.lastValues[0] = !_restore ? QualitySettings.globalTextureMipmapLimit.ToString() : null;
            QualitySettings.globalTextureMipmapLimit = _targetValue;
        }
#endif

        // Quality 04 - AnisotropicTextures
        public static AVRO_Settings.TicketStates GetAnisotropicTexturesPerTexture(AVRO_Ticket _ticket)
        {
            return QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetAnisotropicTexturesPerTexture(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, AnisotropicFiltering.Enable, _ticket);
            _ticket.data.lastValues[0] = !_restore ? QualitySettings.anisotropicFiltering.ToString() : null;
            QualitySettings.anisotropicFiltering = _targetValue;
        }

        // Quality 05 - MipmapStreaming
        public static AVRO_Settings.TicketStates GetMipMapStreaming(AVRO_Ticket _ticket)
        {
            return QualitySettings.streamingMipmapsActive ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetMipMapStreaming(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
            _ticket.data.lastValues[0] = !_restore ? QualitySettings.streamingMipmapsActive.ToString() : null;
            QualitySettings.streamingMipmapsActive = _targetValue;
        }

        // Quality 06 - ShadowMaskMode
        public static AVRO_Settings.TicketStates GetShadowmaskMode(AVRO_Ticket _ticket)
        {
            return QualitySettings.shadowmaskMode == ShadowmaskMode.Shadowmask ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetShadowmaskMode(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, ShadowmaskMode.Shadowmask, _ticket);
            _ticket.data.lastValues[0] = !_restore ? QualitySettings.shadowmaskMode.ToString() : null;
            QualitySettings.shadowmaskMode = _targetValue;
        }

        // Quality 07 - SkinWeightsTwoBones
        public static AVRO_Settings.TicketStates GetSkinWeightsTwoBones(AVRO_Ticket _ticket)
        {
            return QualitySettings.skinWeights == SkinWeights.TwoBones ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetSkinWeightsTwoBones(AVRO_Ticket _ticket, bool _restore = false)
        {
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, SkinWeights.TwoBones, _ticket);
            _ticket.data.lastValues[0] = !_restore ? QualitySettings.skinWeights.ToString() : null;
            QualitySettings.skinWeights = _targetValue;
        }
        #endregion

        #region Graphics Settings
        //----------------------------------------
        //  GRAPHICS SETTINGS
        //----------------------------------------
        // Graphics 01 - BatchRendererGroup KeepAll
        // NO FIX - CAN'T BE READ
        public static AVRO_Settings.TicketStates GetBatchRendererGroupKeepAll(AVRO_Ticket _ticket)
        {
            string _path = "ProjectSettings/GraphicsSettings.asset";
            if (!File.Exists(_path))
                return AVRO_Settings.TicketStates.Omitted;
            string _content = File.ReadAllText(_path);
            if (!_content.Contains("m_BrgStripping"))
                return AVRO_Settings.TicketStates.Omitted;
            return Regex.IsMatch(_content, @"m_BrgStripping:\s*2") ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetBatchRendererGroupKeepAll(AVRO_Ticket _ticket, bool _restore = false)
        {
            string _path = "ProjectSettings/GraphicsSettings.asset";
            if (!File.Exists(_path)) return;
            string _content = File.ReadAllText(_path);
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, 2, _ticket);
            _ticket.data.lastValues[0] = !_restore ? Regex.Match(_content, @"m_BrgStripping:\s*(\S+)").Groups[1].Value : null;
            string _newContent = Regex.Replace(_content, @"(m_BrgStripping:\s*)\S+", "${1}" + _targetValue);
            File.WriteAllText(_path, _newContent);
            AssetDatabase.Refresh();
        }
        #endregion

        #region Renderer Settings
        //----------------------------------------
        //  RENDERER SETTINGS (URP-Focused)
        //----------------------------------------
        // Renderer 00 - ActiveRenderPipelineAsset
        // NO FIX
        public static AVRO_Settings.TicketStates GetActiveRenderPipelineAsset(AVRO_Ticket _ticket)
        {
            return AVRO_Utilities.IsUsingUnityURP && AVRO_Utilities.IsRendererSelected ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }

        // Renderer 01 - DepthTexture
        public static AVRO_Settings.TicketStates GetDepthTextureDisabled(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("supportsCameraDepthTexture");
            if (_property == null || _property.PropertyType != typeof(bool))
                return AVRO_Settings.TicketStates.Omitted;
            return !(bool)_property.GetValue(AVRO_Utilities.URPSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetDepthTextureDisabled(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected) return;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("supportsCameraDepthTexture");
            if (_property != null && _property.CanWrite && _property.PropertyType == typeof(bool))
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, false, _ticket);
                _ticket.data.lastValues[0] = !_restore ? _property.GetValue(AVRO_Utilities.URPSettings).ToString() : null;
                _property.SetValue(AVRO_Utilities.URPSettings, _targetValue);
                EditorUtility.SetDirty((UnityEngine.Object)AVRO_Utilities.URPSettings);
            }
        }

        // Renderer 02 - OpaqueTexture
        public static AVRO_Settings.TicketStates GetOpaqueTextureDisabled(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("supportsCameraOpaqueTexture");
            if (_property == null || _property.PropertyType != typeof(bool))
                return AVRO_Settings.TicketStates.Omitted;
            return !(bool)_property.GetValue(AVRO_Utilities.URPSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetOpaqueTextureDisabled(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected) return;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("supportsCameraOpaqueTexture");
            if (_property != null && _property.CanWrite && _property.PropertyType == typeof(bool))
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, false, _ticket);
                _ticket.data.lastValues[0] = !_restore ? _property.GetValue(AVRO_Utilities.URPSettings).ToString() : null;
                _property.SetValue(AVRO_Utilities.URPSettings, _targetValue);
                EditorUtility.SetDirty((UnityEngine.Object)AVRO_Utilities.URPSettings);
            }
        }

        // Renderer 03 - Disable Terrain Holes
        // NO FIX - READ ONLY
        public static AVRO_Settings.TicketStates GetTerrainHolesDisabled(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("supportsTerrainHoles");
            if (_property == null || _property.PropertyType != typeof(bool))
                return AVRO_Settings.TicketStates.Omitted;
            return !(bool)_property.GetValue(AVRO_Utilities.URPSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }

        // Renderer 06 - Disable HDR
        public static AVRO_Settings.TicketStates GetHDRDisabled(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("supportsHDR");
            if (_property == null || _property.PropertyType != typeof(bool))
                return AVRO_Settings.TicketStates.Omitted;
            return !(bool)_property.GetValue(AVRO_Utilities.URPSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetHDRDisabled(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected) return;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("supportsHDR");
            if (_property != null && _property.CanWrite && _property.PropertyType == typeof(bool))
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, false, _ticket);
                _ticket.data.lastValues[0] = !_restore ? _property.GetValue(AVRO_Utilities.URPSettings).ToString() : null;
                _property.SetValue(AVRO_Utilities.URPSettings, _targetValue);
                EditorUtility.SetDirty((UnityEngine.Object)AVRO_Utilities.URPSettings);
            }
        }

        // Renderer 07 - MSAA4x
        public static AVRO_Settings.TicketStates GetMSAA4x(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("msaaSampleCount");
            if (_property == null || _property.PropertyType != typeof(int) || !_property.CanRead)
                return AVRO_Settings.TicketStates.Omitted;
            return (int)_property.GetValue(AVRO_Utilities.URPSettings) == 4 ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetMSAA4x(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected) return;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("msaaSampleCount");
            if (_property != null && _property.PropertyType == typeof(int) && _property.CanWrite)
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, 4, _ticket);
                _ticket.data.lastValues[0] = !_restore ? _property.GetValue(AVRO_Utilities.URPSettings).ToString() : null;
                _property.SetValue(AVRO_Utilities.URPSettings, _targetValue);
                EditorUtility.SetDirty((UnityEngine.Object)AVRO_Utilities.URPSettings);
            }
        }

        // Renderer 08 - Render Scale
        public static AVRO_Settings.TicketStates GetRenderScaleOne(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("renderScale");
            if (_property == null || _property.PropertyType != typeof(float))
                return AVRO_Settings.TicketStates.Omitted;
            float _value = (float)_property.GetValue(AVRO_Utilities.URPSettings);
            return _value >= 0.85f && _value <= 1.2f ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetRenderScaleOne(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected) return;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("renderScale");
            if (_property != null && _property.PropertyType == typeof(float) && _property.CanWrite)
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, 1.0f, _ticket);
                _ticket.data.lastValues[0] = !_restore ? _property.GetValue(AVRO_Utilities.URPSettings).ToString() : null;
                _property.SetValue(AVRO_Utilities.URPSettings, _targetValue);
                EditorUtility.SetDirty((UnityEngine.Object)AVRO_Utilities.URPSettings);
            }
        }

        // Renderer 10 - Lod Cross Fade
        // NO FIX - READ ONLY
        public static AVRO_Settings.TicketStates GetLODCrossFadeDisabled(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _property = AVRO_Utilities.URPSettings.GetType().GetProperty("enableLODCrossFade");
            if (_property == null || _property.PropertyType != typeof(bool))
                return AVRO_Settings.TicketStates.Omitted;
            return !(bool)_property.GetValue(AVRO_Utilities.URPSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }

        // Renderer 12 - No Additional Light
        // Unique Restore Value
        public static AVRO_Settings.TicketStates GetNoAdditionalLights(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _maxAdditionalLightsCountProp = AVRO_Utilities.URPSettings.GetType().GetProperty("maxAdditionalLightsCount");
            var _additionalLightsRenderingModeProp = AVRO_Utilities.URPSettings.GetType().GetProperty("additionalLightsRenderingMode");
            if (_maxAdditionalLightsCountProp == null || _additionalLightsRenderingModeProp == null)
                return AVRO_Settings.TicketStates.Omitted;
            int _maxAdditionalLightsCount = (int)_maxAdditionalLightsCountProp.GetValue(AVRO_Utilities.URPSettings);
            var _additionalLightsRenderingMode = _additionalLightsRenderingModeProp.GetValue(AVRO_Utilities.URPSettings);
            return (_maxAdditionalLightsCount == 0 || (int)_additionalLightsRenderingMode == 0) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetNoAdditionalLights(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected) return;
            var _maxAdditionalLightsCountProp = AVRO_Utilities.URPSettings.GetType().GetProperty("maxAdditionalLightsCount");
            var _additionalLightsRenderingModeProp = AVRO_Utilities.URPSettings.GetType().GetProperty("additionalLightsRenderingMode");
            if (_maxAdditionalLightsCountProp != null && _additionalLightsRenderingModeProp != null)
            {
                var _targetValueMode = AVRO_Utilities.CheckRestoreValue(_restore, 0, _ticket, 0);
                var _targetValueCount = AVRO_Utilities.CheckRestoreValue(_restore, 0, _ticket, 1);
                _ticket.data.lastValues[0] = !_restore ? Convert.ToInt32(_additionalLightsRenderingModeProp.GetValue(AVRO_Utilities.URPSettings)).ToString() : null;
                _ticket.data.lastValues[1] = !_restore ? _maxAdditionalLightsCountProp.GetValue(AVRO_Utilities.URPSettings).ToString() : null;
                _additionalLightsRenderingModeProp.SetValue(AVRO_Utilities.URPSettings, _targetValueMode);
                _maxAdditionalLightsCountProp.SetValue(AVRO_Utilities.URPSettings, _targetValueCount);
                EditorUtility.SetDirty((UnityEngine.Object)AVRO_Utilities.URPSettings);
            }
        }

        // Renderer 13 - Disable Main Light 
        public static AVRO_Settings.TicketStates GetDisableMainLight(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _type = AVRO_Utilities.URPSettings.GetType();
            var _mainLightRenderingModeProp = _type.GetProperty("mainLightRenderingMode");
            if (_mainLightRenderingModeProp == null)
                return AVRO_Settings.TicketStates.Omitted;
            return (int)_mainLightRenderingModeProp.GetValue(AVRO_Utilities.URPSettings) == 0 ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetDisableMainLight(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected) return;
            var _type = AVRO_Utilities.URPSettings.GetType();
            var _mainLightRenderingModeProp = _type.GetProperty("mainLightRenderingMode");
            if (_mainLightRenderingModeProp != null && _mainLightRenderingModeProp.CanWrite)
            {
                var _enumType = _mainLightRenderingModeProp.PropertyType;
                var _currentValue = _mainLightRenderingModeProp.GetValue(AVRO_Utilities.URPSettings);
                var _disabledValue = Enum.ToObject(_enumType, 0);
                var _method = typeof(AVRO_Utilities).GetMethod("CheckRestoreValue").MakeGenericMethod(_enumType);
                var _targetValue = _method.Invoke(null, new object[] { _restore, _disabledValue, _ticket, 0 });
                _ticket.data.lastValues[0] = !_restore ? _currentValue.ToString() : null;
                _mainLightRenderingModeProp.SetValue(AVRO_Utilities.URPSettings, _targetValue);
                EditorUtility.SetDirty((UnityEngine.Object)AVRO_Utilities.URPSettings);
            }
        }

        // Renderer 14 - Disable Post Processing
        // Unique Restore Value
        public static AVRO_Settings.TicketStates GetPostProcessingDisabled(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _urpType = AVRO_Utilities.URPSettings.GetType();
            var _rendererDataListField = _urpType.GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
            var _renderers = _rendererDataListField?.GetValue(AVRO_Utilities.URPSettings) as object[];
            if (_renderers == null || _renderers.Length == 0 || _renderers[0] == null)
                return AVRO_Settings.TicketStates.Omitted;
            var _rendererData = _renderers[0];
            var _postProcessField = _rendererData.GetType().GetField("postProcessData", BindingFlags.Public | BindingFlags.Instance);
            if (_postProcessField == null)
                return AVRO_Settings.TicketStates.Omitted;
            return _postProcessField.GetValue(_rendererData) == null ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetPostProcessingDisabled(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return;
            var _urpType = AVRO_Utilities.URPSettings.GetType();
            var _rendererDataListField = _urpType.GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
            var _renderers = _rendererDataListField?.GetValue(AVRO_Utilities.URPSettings) as object[];
            if (_renderers == null || _renderers.Length == 0 || _renderers[0] == null)
                return;
            var _rendererData = _renderers[0];
            var _postProcessField = _rendererData.GetType().GetField("postProcessData", BindingFlags.Public | BindingFlags.Instance);
            if (_postProcessField == null)
                return;
            if (_ticket.data.lastValues.Count == 0)
                _ticket.data.lastValues.Add(null);
            var _ppData = AssetDatabase.FindAssets($"{_ticket.data.lastValues[0]} t:PostProcessData")
                .Select(guid => AssetDatabase.LoadAssetAtPath<PostProcessData>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault(p => p != null && p.name == _ticket.data.lastValues[0]);
            var _targetValue = !_restore ? null : _ppData;
            _ticket.data.lastValues[0] = !_restore ? _postProcessField.GetValue(_rendererData).ToString().Split('(')[0].Trim() : null;
            _postProcessField.SetValue(_rendererData, _targetValue);
        }

        // Renderer 15 - RenderingPathForwardPlus (sometimes called Forward+ or ForwardPlus)
        // Unique Restore Value
        public static AVRO_Settings.TicketStates GetRenderingPathForwardPlus(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _urpType = AVRO_Utilities.URPSettings.GetType();
            var _rendererDataListField = _urpType.GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
            var _renderers = _rendererDataListField?.GetValue(AVRO_Utilities.URPSettings) as object[];
            if (_renderers == null || _renderers.Length == 0 || _renderers[0] == null)
                return AVRO_Settings.TicketStates.Omitted;
            var _rendererData = _renderers[0];
            var _rendererType = _rendererData.GetType();
            var _renderingModeProperty = _rendererType.GetProperty("renderingMode", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var _renderingModeValue = _renderingModeProperty?.GetValue(_rendererData);
            return (_renderingModeValue?.ToString() == "ForwardPlus" || _renderingModeValue?.ToString() == "Forward") ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetRenderingPathForwardPlus(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return;
            var _urpType = AVRO_Utilities.URPSettings.GetType();
            var _rendererDataListField = _urpType.GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
            var _renderers = _rendererDataListField?.GetValue(AVRO_Utilities.URPSettings) as object[];
            if (_renderers == null || _renderers.Length == 0 || _renderers[0] == null)
                return;
            var _rendererData = _renderers[0];
            var _rendererType = _rendererData.GetType();
            var _renderingModeField = _rendererType.GetField("renderingMode", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Type _enumType = null;
            if (_renderingModeField != null)
            {
                _enumType = _renderingModeField.FieldType;
                var _method = typeof(AVRO_Utilities).GetMethod("CheckRestoreValue").MakeGenericMethod(_enumType);
                var _forwardPlusValue = Enum.Parse(_enumType, "Forward");
                var _currentValue = _renderingModeField.GetValue(AVRO_Utilities.URPSettings);
                if (_ticket.data.lastValues.Count == 0)
                    _ticket.data.lastValues.Add(null);
                var _targetValue = _method.Invoke(null, new object[] { _restore, _forwardPlusValue, _ticket, 0 });
                _ticket.data.lastValues[0] = !_restore ? _currentValue.ToString() : null;
                _renderingModeField.SetValue(_rendererData, _targetValue);
            }
            else
            {
                var _renderingModeProperty = _rendererType.GetProperty("renderingMode", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (_renderingModeProperty == null || !_renderingModeProperty.CanWrite)
                    return;
                _enumType = _renderingModeProperty.PropertyType;
                var _method = typeof(AVRO_Utilities).GetMethod("CheckRestoreValue").MakeGenericMethod(_enumType);
                var _forwardPlusValue = Enum.Parse(_enumType, "Forward");
                var _currentValue = _renderingModeProperty.GetValue(_rendererData);
                if (_ticket.data.lastValues.Count == 0)
                    _ticket.data.lastValues.Add(null);
                var _targetValue = _method.Invoke(null, new object[] { _restore, _forwardPlusValue, _ticket, 0 });
                _ticket.data.lastValues[0] = !_restore ? _currentValue.ToString() : null;
                _renderingModeProperty.SetValue(_rendererData, _targetValue);
            }
            EditorUtility.SetDirty((UnityEngine.Object)AVRO_Utilities.URPSettings);
        }

        #region.  Unity2023+
#if UNITY_2023_1_OR_NEWER// && IS_RENDERER_SELECTED
        // Renderer 04 - GPUResidentDrawer (experimental)
        // Unique Restore Value
        public static AVRO_Settings.TicketStates GetGPUResidentDrawerEnabled(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _field = AVRO_Utilities.URPSettings.GetType().GetField("m_GPUResidentDrawerMode", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_field == null)
                return AVRO_Settings.TicketStates.Omitted;
            return _field.GetValue(AVRO_Utilities.URPSettings).ToString() == "Disabled" ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetGPUResidentDrawerEnabled(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return;
            var _field = AVRO_Utilities.URPSettings.GetType().GetField("m_GPUResidentDrawerMode", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_field != null)
            {
                if (_ticket.data.lastValues.Count == 0)
                    _ticket.data.lastValues.Add(null);
                var _targetValue = Enum.ToObject(_field.FieldType, !_restore ? 0 : 1);
                _ticket.data.lastValues[0] = !_restore ? _field.GetValue(AVRO_Utilities.URPSettings).ToString() : null;
                _field.SetValue(AVRO_Utilities.URPSettings, _targetValue);
                EditorUtility.SetDirty((UnityEngine.Object)AVRO_Utilities.URPSettings);
            }
        }

        // Renderer 05 - GPU Occlusion Culling
        public static AVRO_Settings.TicketStates GetGPUOcclusionCullingEnabled(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return AVRO_Settings.TicketStates.Omitted;
            var _field = AVRO_Utilities.URPSettings.GetType().GetField("m_GPUResidentDrawerEnableOcclusionCullingInCameras", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_field == null)
                return AVRO_Settings.TicketStates.Omitted;
            return (bool)_field.GetValue(AVRO_Utilities.URPSettings) ? AVRO_Settings.TicketStates.Todo : AVRO_Settings.TicketStates.Done;
        }
        public static void SetGPUOcclusionCullingEnabled(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsUsingUnityURP || !AVRO_Utilities.IsRendererSelected)
                return;
            var _field = AVRO_Utilities.URPSettings.GetType().GetField("m_GPUResidentDrawerEnableOcclusionCullingInCameras", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_field != null)
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, false, _ticket);
                _ticket.data.lastValues[0] = !_restore ? _field.GetValue(AVRO_Utilities.URPSettings).ToString() : null;
                _field.SetValue(AVRO_Utilities.URPSettings, _targetValue);
                EditorUtility.SetDirty((UnityEngine.Object)AVRO_Utilities.URPSettings);
            }
        }
#endif
        #endregion
        #endregion

        #region Meta Settings
        //----------------------------------------
        //  META / OCULUS SETTINGS
        //----------------------------------------
        // Meta 00 - Add Oculus Package
        public static AVRO_Settings.TicketStates GetOculusPackage(AVRO_Ticket _ticket)
        {
            return AVRO_Utilities.IsOculusXRInstalled ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }

        // Meta 01 - Add Target Device
        // 
        public static AVRO_Settings.TicketStates GetOculusTargetDevice(AVRO_Ticket _ticket)
        {
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                Type _settingsType = AVRO_Utilities.OculusSettings.GetType();
                string[] _propertyNames = { "TargetQuest2", "TargetQuest3", "TargetQuest3S", "TargetQuestPro" };
                foreach (string _propName in _propertyNames)
                {
                    FieldInfo _property = _settingsType.GetField(_propName);
                    if (_property != null && _property.FieldType == typeof(bool))
                    {
                        if ((bool)_property.GetValue(AVRO_Utilities.OculusSettings))
                            return AVRO_Settings.TicketStates.Done;
                    }
                }
                return AVRO_Settings.TicketStates.Todo;
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                int _result = AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@targetDevices");
                return _result == 1 ? AVRO_Settings.TicketStates.Done : _result == -1 ? AVRO_Settings.TicketStates.Omitted : AVRO_Settings.TicketStates.Todo;
            }
            return AVRO_Settings.TicketStates.Omitted;
        }
        public static void SetOculusTargetDevice(AVRO_Ticket _ticket, bool _restore = false)
        {
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                string[] _propertyNames = { "TargetQuest2", "TargetQuest3", "TargetQuest3S", "TargetQuestPro" };
                foreach (string _propName in _propertyNames)
                {
                    FieldInfo _property = AVRO_Utilities.OculusSettings.GetType().GetField(_propName);
                    if (_property != null && _property.FieldType == typeof(bool))
                    {
                        _property.SetValue(AVRO_Utilities.OculusSettings, !_restore);
                    }
                }
                AVRO_Utilities.SetDirtyReflectionOf(AVRO_Utilities.OculusSettings);
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
                _ticket.data.lastValues[0] = !_restore ? (AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@targetDevices") == 1).ToString() : null;
                AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@targetDevices", true, _targetValue);
            }
        }

        // Meta 02 - Multiview
        public static AVRO_Settings.TicketStates GetMultiviewEnabled(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsOculusXRInstalled)
                return AVRO_Settings.TicketStates.Omitted;
            FieldInfo _m_StereoRenderingModeAndroid = AVRO_Utilities.OculusSettings.GetType().GetField("m_StereoRenderingModeAndroid");
            if (_m_StereoRenderingModeAndroid == null)
                return AVRO_Settings.TicketStates.Omitted;
            object _stereoRenderingModeValue = _m_StereoRenderingModeAndroid.GetValue(AVRO_Utilities.OculusSettings);
            Type _stereoRenderingModeEnumType = _m_StereoRenderingModeAndroid.FieldType;
            object _multiviewEnumValue = Enum.Parse(_stereoRenderingModeEnumType, "Multiview");
            return _stereoRenderingModeValue.Equals(_multiviewEnumValue) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetMultiviewEnabled(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsOculusXRInstalled) return;
            FieldInfo _m_StereoRenderingModeAndroid = AVRO_Utilities.OculusSettings.GetType().GetField("m_StereoRenderingModeAndroid");
            if (_m_StereoRenderingModeAndroid == null) return;
            Type _enumType = _m_StereoRenderingModeAndroid.FieldType;
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, "Multiview", _ticket);
            _ticket.data.lastValues[0] = !_restore ? _m_StereoRenderingModeAndroid.GetValue(AVRO_Utilities.OculusSettings).ToString() : null;
            object _multiviewValue = Enum.Parse(_enumType, _targetValue);
            _m_StereoRenderingModeAndroid.SetValue(AVRO_Utilities.OculusSettings, _multiviewValue);
            AVRO_Utilities.SetDirtyReflectionOf(AVRO_Utilities.OculusSettings);
        }

        // Meta 03 - Enable Optimize Buffer Discards
        public static AVRO_Settings.TicketStates GetOptimizeBufferDiscards(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsVulkanUsed)
                return AVRO_Settings.TicketStates.Omitted;
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                FieldInfo _optimizeBufferDiscards = AVRO_Utilities.OculusSettings.GetType().GetField("OptimizeBufferDiscards");
                return (bool)_optimizeBufferDiscards.GetValue(AVRO_Utilities.OculusSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                int _result = AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@optimizeBufferDiscards");
                return _result == 1 ? AVRO_Settings.TicketStates.Done : _result == -1 ? AVRO_Settings.TicketStates.Omitted : AVRO_Settings.TicketStates.Todo;
            }
            return AVRO_Settings.TicketStates.Omitted;
        }
        public static void SetOptimizeBufferDiscards(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsVulkanUsed) return;
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                FieldInfo _optimizeBufferDiscards = AVRO_Utilities.OculusSettings.GetType().GetField("OptimizeBufferDiscards");
                if (_optimizeBufferDiscards == null) return;
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
                _ticket.data.lastValues[0] = !_restore ? _optimizeBufferDiscards.GetValue(AVRO_Utilities.OculusSettings).ToString() : null;
                _optimizeBufferDiscards.SetValue(AVRO_Utilities.OculusSettings, _targetValue);
                AVRO_Utilities.SetDirtyReflectionOf(AVRO_Utilities.OculusSettings);
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
                _ticket.data.lastValues[0] = !_restore ? (AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@optimizeBufferDiscards") == 1).ToString() : null;
                AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@optimizeBufferDiscards", true, _targetValue);
            }
        }

        // Meta 04 - Enable Symmetric Projection
        public static AVRO_Settings.TicketStates GetSymmetricProjection(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsVulkanUsed)
                return AVRO_Settings.TicketStates.Omitted;
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                FieldInfo _symmetricProjection = AVRO_Utilities.OculusSettings.GetType().GetField("SymmetricProjection");
                return (bool)_symmetricProjection.GetValue(AVRO_Utilities.OculusSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                int _result = AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@symmetricProjection");
                return _result == 1 ? AVRO_Settings.TicketStates.Done : _result == -1 ? AVRO_Settings.TicketStates.Omitted : AVRO_Settings.TicketStates.Todo;
            }
            return AVRO_Settings.TicketStates.Omitted;
        }
        public static void SetSymmetricProjection(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsVulkanUsed) return;
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                FieldInfo _symmetricProjection = AVRO_Utilities.OculusSettings.GetType().GetField("SymmetricProjection");
                if (_symmetricProjection == null) return;
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
                _ticket.data.lastValues[0] = !_restore ? _symmetricProjection.GetValue(AVRO_Utilities.OculusSettings).ToString() : null;
                _symmetricProjection.SetValue(AVRO_Utilities.OculusSettings, _targetValue);
                AVRO_Utilities.SetDirtyReflectionOf(AVRO_Utilities.OculusSettings);
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
                _ticket.data.lastValues[0] = !_restore ? (AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@symmetricProjection") == 1).ToString() : null;
                AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@symmetricProjection", true, _targetValue);
            }
        }

        // Meta 05 - Subsample Layout
        public static AVRO_Settings.TicketStates GetSubsampleLayout(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsVulkanUsed)
                return AVRO_Settings.TicketStates.Omitted;
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                FieldInfo _subsampledLayout = AVRO_Utilities.OculusSettings.GetType().GetField("SubsampledLayout");
                return (bool)_subsampledLayout.GetValue(AVRO_Utilities.OculusSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                int _result = AVRO_Utilities.SetOpenXRDictionaryValue("MetaXRSubsampledLayout");
                return _result == 1 ? AVRO_Settings.TicketStates.Done : _result == -1 ? AVRO_Settings.TicketStates.Omitted : AVRO_Settings.TicketStates.Todo;
            }
            return AVRO_Settings.TicketStates.Omitted;
        }
        public static void SetSubsampleLayout(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsVulkanUsed) return;
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                FieldInfo _subsampledLayout = AVRO_Utilities.OculusSettings.GetType().GetField("SubsampledLayout");
                if (_subsampledLayout == null) return;
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
                _ticket.data.lastValues[0] = !_restore ? _subsampledLayout.GetValue(AVRO_Utilities.OculusSettings).ToString() : null;
                _subsampledLayout.SetValue(AVRO_Utilities.OculusSettings, _targetValue);
                AVRO_Utilities.SetDirtyReflectionOf(AVRO_Utilities.OculusSettings);
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
                _ticket.data.lastValues[0] = !_restore ? (AVRO_Utilities.SetOpenXRDictionaryValue("MetaXRSubsampledLayout") == 1).ToString() : null;
                AVRO_Utilities.SetOpenXRDictionaryValue("MetaXRSubsampledLayout", true, true);
            }
        }

        // Meta 06 - Vulkan Depth OculusDepthSubmission
        public static AVRO_Settings.TicketStates GetVulkanDepthSubmission(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsOculusXRInstalled || !AVRO_Utilities.IsVulkanUsed)
                return AVRO_Settings.TicketStates.Omitted;
            FieldInfo _depthSubmission = AVRO_Utilities.OculusSettings.GetType().GetField("DepthSubmission");
            return (bool)_depthSubmission.GetValue(AVRO_Utilities.OculusSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetVulkanDepthSubmission(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsOculusXRInstalled || !AVRO_Utilities.IsVulkanUsed) return;
            FieldInfo _depthSubmission = AVRO_Utilities.OculusSettings.GetType().GetField("DepthSubmission");
            if (_depthSubmission == null) return;
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
            _ticket.data.lastValues[0] = !_restore ? _depthSubmission.GetValue(AVRO_Utilities.OculusSettings).ToString() : null;
            _depthSubmission.SetValue(AVRO_Utilities.OculusSettings, _targetValue);
            AVRO_Utilities.SetDirtyReflectionOf(AVRO_Utilities.OculusSettings);
        }

        // Meta 07 - Vulkan Late Latching
        public static AVRO_Settings.TicketStates GetVulkanLateLatching(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsVulkanUsed)
                return AVRO_Settings.TicketStates.Omitted;
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                FieldInfo _lateLatching = AVRO_Utilities.OculusSettings.GetType().GetField("LateLatching");
                return (bool)_lateLatching.GetValue(AVRO_Utilities.OculusSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                int _result = AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@lateLatchingMode");
                return _result == 1 ? AVRO_Settings.TicketStates.Done : _result == -1 ? AVRO_Settings.TicketStates.Omitted : AVRO_Settings.TicketStates.Todo;
            }
            return AVRO_Settings.TicketStates.Omitted;
        }
        public static void SetVulkanLateLatching(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsVulkanUsed) return;
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                FieldInfo _lateLatching = AVRO_Utilities.OculusSettings.GetType().GetField("LateLatching");
                if (_lateLatching == null) return;
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
                _ticket.data.lastValues[0] = !_restore ? _lateLatching.GetValue(AVRO_Utilities.OculusSettings).ToString() : null;
                _lateLatching.SetValue(AVRO_Utilities.OculusSettings, _targetValue);
                AVRO_Utilities.SetDirtyReflectionOf(AVRO_Utilities.OculusSettings);
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
                _ticket.data.lastValues[0] = !_restore ? (AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@lateLatchingMode") == 1).ToString() : null;
                AVRO_Utilities.SetOpenXRDictionaryValue("MetaQuestFeature@lateLatchingMode", true, _targetValue);
            }
        }

        // Meta 08 - OpenGLES Subsample Layout
        public static AVRO_Settings.TicketStates GetOpenGLESLowOverhead(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsOculusXRInstalled || AVRO_Utilities.IsVulkanUsed)
                return AVRO_Settings.TicketStates.Omitted;
            FieldInfo _lowOverheadModeProperty = AVRO_Utilities.OculusSettings.GetType().GetField("LowOverheadMode");
            return (bool)_lowOverheadModeProperty.GetValue(AVRO_Utilities.OculusSettings) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetOpenGLESLowOverhead(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsOculusXRInstalled || AVRO_Utilities.IsVulkanUsed) return;
            FieldInfo _lowOverheadModeProperty = AVRO_Utilities.OculusSettings.GetType().GetField("LowOverheadMode");
            if (_lowOverheadModeProperty == null) return;
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
            _ticket.data.lastValues[0] = !_restore ? _lowOverheadModeProperty.GetValue(AVRO_Utilities.OculusSettings).ToString() : null;
            _lowOverheadModeProperty.SetValue(AVRO_Utilities.OculusSettings, _targetValue);
            AVRO_Utilities.SetDirtyReflectionOf(AVRO_Utilities.OculusSettings);
        }

        // Meta 09 - Fixed Foveated Rendering
        public static AVRO_Settings.TicketStates GetFixedFoveatedRendering(AVRO_Ticket _ticket)
        {
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                FieldInfo _foveatedRenderingMethod = AVRO_Utilities.OculusSettings.GetType().GetField("FoveatedRenderingMethod");
                if (_foveatedRenderingMethod == null)
                    return AVRO_Settings.TicketStates.Omitted;
                object _stereoRenderingModeValue = _foveatedRenderingMethod.GetValue(AVRO_Utilities.OculusSettings);
                Type _stereoRenderingModeEnumType = _foveatedRenderingMethod.FieldType;
                object _multiviewEnumValue = Enum.Parse(_stereoRenderingModeEnumType, "FixedFoveatedRendering");
                return _stereoRenderingModeValue.Equals(_multiviewEnumValue) ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                int _result = AVRO_Utilities.SetOpenXRDictionaryValue("MetaXRFoveationFeature");
                return _result == 1 ? AVRO_Settings.TicketStates.Done : _result == -1 ? AVRO_Settings.TicketStates.Omitted : AVRO_Settings.TicketStates.Todo;
            }
            return AVRO_Settings.TicketStates.Omitted;
        }
        public static void SetFixedFoveatedRendering(AVRO_Ticket _ticket, bool _restore = false)
        {
            // Oculus
            if (AVRO_Utilities.IsOculusXRInstalled && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.Oculus)
            {
                FieldInfo _foveatedRenderingMethod = AVRO_Utilities.OculusSettings.GetType().GetField("FoveatedRenderingMethod");
                if (_foveatedRenderingMethod == null) return;
                Type _enumType = _foveatedRenderingMethod.FieldType;
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, "FixedFoveatedRendering", _ticket);
                _ticket.data.lastValues[0] = !_restore ? _foveatedRenderingMethod.GetValue(AVRO_Utilities.OculusSettings).ToString() : null;
                object _multiviewValue = Enum.Parse(_enumType, _targetValue);
                _foveatedRenderingMethod.SetValue(AVRO_Utilities.OculusSettings, _multiviewValue);
                AVRO_Utilities.SetDirtyReflectionOf(AVRO_Utilities.OculusSettings);
            }
            // OpenXR
            else if (AVRO_Utilities.IsOpenXRInstalled && AVRO_Utilities.OpenXRSettingsDictionary != null && AVRO_Utilities.XRLoaderName == AVRO_Utilities.XRLoaderNames.OpenXR)
            {
                var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
                _ticket.data.lastValues[0] = !_restore ? (AVRO_Utilities.SetOpenXRDictionaryValue("MetaXRFoveationFeature") == 1).ToString() : null;
                AVRO_Utilities.SetOpenXRDictionaryValue("MetaXRFoveationFeature", true, _targetValue);
            }
        }

        // Meta 10 - OpenXR Foveated Rendering
        public static AVRO_Settings.TicketStates GetOpenXRFoveatedRendering(AVRO_Ticket _ticket)
        {
            if (!AVRO_Utilities.IsOpenXRInstalled || AVRO_Utilities.OpenXRSettingsDictionary == null || AVRO_Utilities.XRLoaderName != AVRO_Utilities.XRLoaderNames.OpenXR)
                return AVRO_Settings.TicketStates.Omitted;
            int _result = AVRO_Utilities.SetOpenXRDictionaryValue("FoveatedRendering");
            return _result == 1 ? AVRO_Settings.TicketStates.Done : _result == -1 ? AVRO_Settings.TicketStates.Omitted : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetOpenXRFoveatedRendering(AVRO_Ticket _ticket, bool _restore = false)
        {
            if (!AVRO_Utilities.IsOpenXRInstalled || AVRO_Utilities.OpenXRSettingsDictionary == null || AVRO_Utilities.XRLoaderName != AVRO_Utilities.XRLoaderNames.OpenXR) return;
            var _targetValue = AVRO_Utilities.CheckRestoreValue(_restore, true, _ticket);
            _ticket.data.lastValues[0] = !_restore ? (AVRO_Utilities.SetOpenXRDictionaryValue("FoveatedRendering") == 1).ToString() : null;
            AVRO_Utilities.SetOpenXRDictionaryValue("FoveatedRendering", true, _targetValue);
        }
        #endregion
    }
}