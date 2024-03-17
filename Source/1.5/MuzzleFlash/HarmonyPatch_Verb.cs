using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace MuzzleFlash
{
    [HarmonyPatch(typeof(Verb), "TryCastNextBurstShot")]
    internal class HarmonyPatch_Verb
    {
        public static void Postfix(Verb __instance, int ___burstShotsLeft)
        {
            Thing caster = __instance.Caster;
            if (caster == null) return;
            if (!(__instance.DirectOwner is ThingComp comp)) return;

            Thing equipment = __instance.EquipmentSource;

            if (equipment == null) return;

            MuzzleFlashProps muzzleProps = equipment?.def?.GetModExtension<MuzzleFlashProps>();
            if (muzzleProps == null || muzzleProps.offsets == null || muzzleProps.offsets.NullOrEmpty()) return;

            Vector3 targetPosition = __instance.CurrentTarget.CenterVector3;
            Vector3 sourcePostion = caster.DrawPos;
            Vector3 direction = (targetPosition - sourcePostion);
            Vector3 drawPos = default;

            if (__instance.CasterIsPawn)
            {
                drawPos = CacheWeaponsDrawPos.GetDrawPos(equipment);
            }
            else
            {
                drawPos = sourcePostion;
            }
            direction.y = 0;
            direction.Normalize();

            if (muzzleProps.isAlternately)
            {
                MuzzleFlashUtility.SpawnMuzzleFlash(caster.MapHeld, muzzleProps.def, drawPos, muzzleProps.offsets[___burstShotsLeft % muzzleProps.offsets.Count], direction, muzzleProps.drawSize);
            }
            else
            {
                for (int i = 0; i < muzzleProps.offsets.Count; i++)
                {
                    MuzzleFlashUtility.SpawnMuzzleFlash(caster.MapHeld, muzzleProps.def, drawPos, muzzleProps.offsets[i], direction, muzzleProps.drawSize);
                }
            }
        }
    }
}
