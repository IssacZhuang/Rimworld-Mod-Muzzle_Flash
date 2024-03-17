using AEF;
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
    public static class MuzzleFlashUtility

    {
        public static MuzzleFlash CreateMuzzleFromDef(AnimatedEffectDef def)
        {
            Material material = AEF.MaterialPool.CreateMaterial(ShaderPool.Animated, def.Texture);
            MuzzleFlash effect = new MuzzleFlash(material)
            {
                def = def,
                drawSize = new Vector3(def.defaultSize.x, 1f, def.defaultSize.y),
            };
            return effect;
        }

        public static void SpawnMuzzleFlash(Map map, AnimatedEffectDef def, Vector3 drawLoc, Vector3 offset, Vector3 direction, Vector2 drawSize)
        {
            MuzzleFlash effect = MuzzleFlashUtility.CreateMuzzleFromDef(def);
            float angle = direction.AngleFlat();

            Vector3 drawPos = drawLoc + direction * (offset.x + drawSize.x * def.drawOffsetMultiplier.x) + Vector3.Cross(direction, Vector3.up).normalized * offset.y * MuzzleFlashUtility.GetFlipped(angle);

            effect.drawPos = drawPos;
            effect.drawPos.y += Altitudes.AltInc;
            effect.drawSize = new Vector3(drawSize.x, 0, drawSize.y);
            effect.angle = angle;
            map.GetComponent<MapComponent_AnimatedEffectManager>().RegisterEffect(effect);
        }

        public static float GetFlipped(float aimAngle)
        {
            if (aimAngle > 200f && aimAngle < 340f)
            {
                return -1f;
            }
            return 1f;
        }
    }
}
