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
    public class MuzzleFlash : AnimatedEffect
    {
        public MuzzleFlash(Material material)
        {
            shaderController = new AnimatedShaderController(material);
        }

        public override void Update()
        {
            base.Update();
        }


    }
}
