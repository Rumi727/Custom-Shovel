using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace Rumi.CustomShovel
{
    [BepInPlugin(modGuid, modName, modVersion)]
    public class CustomShovel : BaseUnityPlugin
    {
        public const string modGuid = "Rumi.CustomShovel";
        public const string modName = "CustomShovel";
        public const string modVersion = "1.0.1";

        public static Assembly currentAssembly => _currentAssembly ??= Assembly.GetExecutingAssembly();
        static Assembly? _currentAssembly;

        public static string assemblyName => _assemblyName ??= currentAssembly.FullName.Split(',')[0];
        static string? _assemblyName;

        public static ManualLogSource? logger { get; private set; }
        public static CustomShovelConfig? config { get; private set; }

        public static Harmony harmony { get; } = new Harmony(modGuid);

        public static Transform emptyTransform
        {
            get
            {
                if (_emptyTransform == null)
                {
                    _emptyTransform = new GameObject("Easy Shovel Empty Transform").transform;
                    DontDestroyOnLoad(_emptyTransform);
                }

                return _emptyTransform;
            }
        }
        static Transform? _emptyTransform;

        public static Material coloredMaterial
        {
            get
            {
                if (_coloredMaterial == null)
                {
                    Shader shader = Shader.Find("Hidden/Internal-Colored");
                    _coloredMaterial = new Material(shader);
                    _coloredMaterial.hideFlags = HideFlags.HideAndDontSave;

                    _coloredMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    _coloredMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    _coloredMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                    _coloredMaterial.SetInt("_ZWrite", 0);
                }

                return _coloredMaterial;
            }
        }
        static Material? _coloredMaterial;

        void Awake()
        {
            logger = Logger;
            _ = emptyTransform;

            logger?.LogInfo("Start loading plugin...");

            logger?.LogInfo("Config Loading...");

            try
            {
                config = new CustomShovelConfig(Config);
            }
            catch (Exception e)
            {
                config = null;
                
                logger?.LogError(e);
                logger?.LogWarning($"Failed to load config file\nSettings will be loaded with defaults!");
            }

            logger?.LogInfo("HitShovel IL Patch...");
            CustomShovelPatches.Init();

            logger?.LogInfo("PlayerControllerB And Camera Patch...");
            harmony.PatchAll(typeof(CustomShovelDrawHitbox));
            CustomShovelDrawHitbox.Init();

            logger?.LogInfo("ModelReplacementAPI And PlayerControllerB Patch...");

            try
            {
                harmony.PatchAll(typeof(CustomShovelSkinFix));
            }
            catch (TypeLoadException e)
            {
                logger?.LogError(e);
                logger?.LogWarning("ModelReplacementAPI Patch Fail! (This is not a bug and occurs when ModelReplacementAPI is not present)");
            }
            catch (Exception e)
            {
                logger?.LogError(e);
                logger?.LogError("ModelReplacementAPI Patch Fail!");
            }

            logger?.LogInfo($"Plugin {modName} is loaded!");
        }
    }
}
