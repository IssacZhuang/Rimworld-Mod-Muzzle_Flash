using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace MuzzleFlash
{
    /// <summary>
    /// RimWorld 1.7: entries are weakly keyed by Thing, so dead equipment drops out of the
    /// cache with the GC and no periodic clearing (and no GameComponent lifetime) is needed.
    /// </summary>
    public static class WeaponPositionCache
    {
        private static readonly ConditionalWeakTable<Thing, StrongBox<Vector3>> _drawPos = new ConditionalWeakTable<Thing, StrongBox<Vector3>>();

        public static void SetDrawPos(Thing eq, Vector3 pos)
        {
            // GetOrCreateValue updates the box in place - zero allocation after the first insert
            _drawPos.GetOrCreateValue(eq).Value = pos;
        }

        public static Vector3 GetDrawPos(Thing eq)
        {
            return _drawPos.TryGetValue(eq, out StrongBox<Vector3> box) ? box.Value : default;
        }
    }
}
