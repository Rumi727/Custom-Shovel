using GameNetcodeStuff;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rumi.CustomShovel
{
    public static class CustomShovelDrawHitbox
    {
        public static PlayerControllerB? player => _player;
        static PlayerControllerB? _player;

        public static void Init() => RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        private static void PlayerControllerB_Update_Postfix(PlayerControllerB __instance)
        {
            if (__instance.IsOwner && (!__instance.IsServer || __instance.isHostPlayerObject) && __instance.isPlayerControlled && !__instance.isPlayerDead)
                _player = __instance;
        }

        static void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, Camera arg2)
        {
            if
            (
                !(CustomShovel.config?.showShovelHitbox ?? CustomShovelConfig.dShowShovelHitbox) ||
                CustomShovel.emptyTransform == null ||
                player == null ||
                !player.isHoldingObject ||
                !(player.currentlyHeldObjectServer is Shovel)
            )
                return;

            float radius = CustomShovel.config?.radius ?? CustomShovelConfig.dRadius;
            float maxDistance = CustomShovel.config?.maxDistance ?? CustomShovelConfig.dMaxDistance;
            Vector3 cameraPos = player.gameplayCamera.transform.position;
            Vector3 normal = player.gameplayCamera.transform.forward;

            RaycastHit[] hits = Physics.SphereCastAll(cameraPos, radius, normal, maxDistance, 11012424, QueryTriggerInteraction.Collide);
            Vector3 pos;
            RaycastHit hit = hits.Aggregate((x, y) => x.distance > y.distance ? x : y);

            if (hits.Length > 0)
            {
                pos = cameraPos + ((hit.distance - radius) * normal);

                GL.PushMatrix();
                GL.MultMatrix(CustomShovel.emptyTransform.localToWorldMatrix);

                if (CustomShovel.coloredMaterial.SetPass(0))
                {
                    Circle3D(pos, radius, Vector3.right);
                    Circle3D(pos, radius, Vector3.forward);
                    Circle3D(pos, radius, Vector3.up);
                }

                for (int i = 0; i < hits.Length; i++)
                    Circle3D(hits[i].point, 0.1f, normal);

                GL.PopMatrix();
            }
        }

        /// <summary>
        /// Draws a circle in world space
        /// </summary>
        public static void Circle3D(Vector3 center, float radius, Vector3 normal)
        {
            GL.Begin(GL.LINES);
            GL.Color(Color.red);

            normal = normal.normalized;
            Vector3 forward = normal == Vector3.up ?
                Vector3.ProjectOnPlane(Vector3.forward, normal).normalized :
                Vector3.ProjectOnPlane(Vector3.up, normal);
            Vector3 right = Vector3.Cross(normal, forward);

            for (float theta = 0.0f; theta < (2 * Mathf.PI); theta += 0.25f)
            {
                Vector3 ci = center + forward * Mathf.Cos(theta) * radius + right * Mathf.Sin(theta) * radius;
                GL.Vertex(ci);

                if (theta != 0)
                    GL.Vertex(ci);
            }

            GL.End();
        }
    }
}
