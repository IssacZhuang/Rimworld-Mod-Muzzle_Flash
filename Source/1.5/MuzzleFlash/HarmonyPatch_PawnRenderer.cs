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
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming")]
    internal class HarmonyPatch_PawnRenderer
    {
        public static void Postfix(Thing eq, Vector3 drawLoc, float aimAngle)
        {
            CacheWeaponsDrawPos.SetDrawPos(eq, drawLoc);
        }
    }
}
