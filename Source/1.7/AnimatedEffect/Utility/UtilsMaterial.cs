using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace MuzzleFlash
{
    public static class UtilsMaterial
    {
        // RimWorld 1.7 (Waylith): enableInstancing is a dead store in the Unity compat
        // layer - instancing happens whenever an instanced draw API is called.
        public static Material CreateMaterial(Shader shader)
        {
            return new Material(shader);
        }
    }
}
