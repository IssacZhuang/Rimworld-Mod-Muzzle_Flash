using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using UnityEngine;

namespace AEF
{
    public static class MaterialPool
    {
        public static Material CreateMaterial(Shader shader,Texture2D mainTexture)
        {
            return new Material(shader)
            {
                mainTexture = mainTexture
            };
        }
    }
}
