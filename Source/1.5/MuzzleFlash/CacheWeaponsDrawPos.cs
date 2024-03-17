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
    public static class CacheWeaponsDrawPos
    {
        private static Dictionary<Thing, Vector3> drawPos = new Dictionary<Thing, Vector3>();

        public static void SetDrawPos(Thing eq, Vector3 pos)
        {
            drawPos[eq] = pos;
        }

        public static Vector3 GetDrawPos(Thing eq)
        {
            Vector3 result = drawPos.GetWithFallback(eq);
            //drawPos.Remove(eq);
            return result;
        }

        public static void Clear()
        {
            drawPos.Clear();
        }
    }
}
