using GameNetcodeStuff;
using HarmonyLib;
using ModelReplacement;
using ModelReplacement.AvatarBodyUpdater;
using System;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rumi.CustomShovel
{
    public static class CustomShovelSkinFix
    {
        static bool refresh = false;

        [HarmonyPatch(typeof(ModelReplacementAPI), "SetPlayerModelReplacement")]
        [HarmonyPrefix]
        public static void ModelReplacementAPI_SetPlayerModelReplacement(ref PlayerControllerB player, ref Type type)
        {
            if (player.gameObject.TryGetComponent(out BodyReplacementBase component))
            {
                int currentSuitID = player.currentSuitID;
                string unlockableName = StartOfRound.Instance.unlockablesList.unlockables[currentSuitID].unlockableName;

                if (component.GetType() == type && component.suitName == unlockableName)
                    return;
            }

            CustomShovel.logger?.LogInfo("SetPlayerModelReplacement call detected");
            refresh = true;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyPostfix]
        static void PlayerControllerB_LateUpdate_Postfix()
        {
            if (!refresh)
                return;

            refresh = false;

            int defaultLayer = LayerMask.NameToLayer("Default");
            int layer = LayerMask.NameToLayer("Ignore Raycast");

            OffsetBuilder[] offsetBuilders = Object.FindObjectsByType<OffsetBuilder>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < offsetBuilders.Length; i++)
            {
                OffsetBuilder offsetBuilder = offsetBuilders[i];
                Transform[] transforms = offsetBuilder.GetComponentsInChildren<Transform>(true);

                for (int j = 0; j < transforms.Length; j++)
                {
                    Transform transform = transforms[j];
                    if (transform.gameObject.layer == defaultLayer)
                        transform.gameObject.layer = layer;
                }
            }

            RotationOffset[] rotationOffsets = Object.FindObjectsByType<RotationOffset>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < offsetBuilders.Length; i++)
            {
                RotationOffset rotationOffset = rotationOffsets[i];
                Transform[] transforms = rotationOffset.GetComponentsInChildren<Transform>(true);
                
                for (int j = 0; j < transforms.Length; j++)
                {
                    Transform transform = transforms[j];
                    if (transform.gameObject.layer == defaultLayer)
                        transform.gameObject.layer = layer;
                }
            }
        }
    }
}
