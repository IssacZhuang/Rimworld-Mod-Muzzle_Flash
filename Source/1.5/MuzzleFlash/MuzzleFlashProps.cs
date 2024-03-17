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
    public class MuzzleFlashProps : DefModExtension
    {
        public AnimatedEffectDef def = MuzzleDefOf.MF_StandardMuzzleFalsh;
        public Vector2 drawSize = new Vector2(0.8f, 0.8f);
        public List<Vector2> offsets;
        public bool isAlternately;
    }
}
