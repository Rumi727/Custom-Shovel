using GameNetcodeStuff;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace Rumi.CustomShovel
{
    public static class CustomShovelPatches
    {
        public static void Init() => IL.Shovel.HitShovel += HitShovel;

        public static void ShovelPatchUpdate()
        {
            IL.Shovel.HitShovel -= HitShovel;
            IL.Shovel.HitShovel += HitShovel;
        }

        static void HitShovel(ILContext il)
        {
            //출처 : https://github.com/Rocksnotch/shovelFix/blob/main/Patches/ShovelHotfix.cs
            ILCursor c = new ILCursor(il);
            if 
            (
                c.TryGotoNext
                (
                    x => x.MatchLdarg(0),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<Shovel>("previousPlayerHeldBy"),
                    x => x.MatchLdfld<PlayerControllerB>("gameplayCamera"),
                    x => x.MatchCallvirt<Component>("get_transform"),
                    x => x.MatchCallvirt<Transform>("get_position")
                )
            )
            {
                CustomShovel.logger?.LogMessage("Try Goto Next Success!");
                c.Index++;
                c.RemoveRange(25);

                c.EmitDelegate<Action<Shovel>>((Shovel instance) => instance.objectsHitByShovel = Physics.SphereCastAll(instance.previousPlayerHeldBy.gameplayCamera.transform.position, CustomShovel.config?.radius ?? CustomShovelConfig.dRadius, instance.previousPlayerHeldBy.gameplayCamera.transform.forward, CustomShovel.config?.maxDistance ?? CustomShovelConfig.dMaxDistance, instance.shovelMask, QueryTriggerInteraction.Collide));
            }
            else
            {
                CustomShovel.logger?.LogError("Try Goto Next Failed! Bad IL Code!");
            }
        }
    }
}
