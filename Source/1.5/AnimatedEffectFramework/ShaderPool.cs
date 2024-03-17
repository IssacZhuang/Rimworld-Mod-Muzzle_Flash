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
    public static class ShaderPool
    {
        public readonly static int ID_FRAME = Shader.PropertyToID("_Frame");
        public readonly static int ID_SPLITS = Shader.PropertyToID("_Splits");
        public readonly static int ID_LIGHT_INTENSITY = Shader.PropertyToID("_LightIntensity");

        public static void LoadShader(AssetBundle assetBundle)
        {
            foreach (Shader shader in assetBundle.LoadAllAssets<Shader>())
            {
                ShaderPool.shaders.Add(shader.name, shader);
            }
        }

        public static Shader GetShader(string key)
        {
            return shaders.TryGetValue(key);
        }

        private readonly static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

        private static Shader animated;

        public static Shader Animated
        {
            get
            {
                if (animated == null) animated = GetShader("Unlit/Animated");
                if (animated == null) Log.Error("The shader Unlit/Animated not exist");
                return animated;
            }
        }
    }
}
