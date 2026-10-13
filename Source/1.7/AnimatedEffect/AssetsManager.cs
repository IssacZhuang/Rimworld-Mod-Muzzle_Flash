using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MuzzleFlash
{
    /// <summary>
    /// RimWorld 1.7 (Waylith): shaders are loaded through ShaderDef (Defs/ShaderDefs +
    /// Shaders/*.glsl); Unity AssetBundles and per-platform precompiled shaders are gone.
    /// </summary>
    public class AssetsManager
    {
        private const string ShaderDefName = "MuzzleFlashAnimatedAdditive";

        private static AssetsManager _instance;
        public static AssetsManager Default
        {
            get
            {
                if (_instance == null) _instance = new AssetsManager();
                return _instance;
            }
        }

        private readonly Dictionary<string, Shader> _shaders = new Dictionary<string, Shader>();
        private readonly List<Material> _materials = new List<Material>();

        private Shader _animatedInstanced;
        private bool _initialized = false;

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            ShaderDef def = DefDatabase<ShaderDef>.GetNamedSilentFail(ShaderDefName);
            if (def != null)
            {
                _animatedInstanced = def.Shader;
                _shaders.Add(ShaderDefName, _animatedInstanced);
                Log.Message($"[Muzzle Flash] Shader '{ShaderDefName}' loaded from ShaderDef");
            }
            else
            {
                Log.Error($"[Muzzle Flash] ShaderDef '{ShaderDefName}' not found. Check 1.7/Defs/ShaderDefs and 1.7/Shaders.");
            }
        }

        public Shader GetShader(string key)
        {
            if (!_initialized) Initialize();
            if (!_shaders.TryGetValue(key, out Shader shader))
            {
                return null;
            }
            return shader;
        }

        public IEnumerable<string> GetShaderNames()
        {
            foreach (var shader in _shaders)
            {
                yield return shader.Value.name;
            }
        }

        public Material GetMaterial(Shader shader, Texture2D texture, Vector4 splits, float lightIntensity = 1f)
        {
            if (shader == null) throw ExceptionRendering.ShaderNotFound(" Null shader");

            for (int i = 0; i < _materials.Count; i++)
            {
                if (_materials[i].shader != shader) continue;
                if (_materials[i].mainTexture != texture) continue;
                if (_materials[i].GetVector(ShaderPropertyID.splits) != splits) continue;
                if (_materials[i].GetFloat(ShaderPropertyID.lightIntensity) != lightIntensity) continue;
                return _materials[i];
            }

            Material result = UtilsMaterial.CreateMaterial(shader);
            result.mainTexture = texture;
            result.SetVector(ShaderPropertyID.splits, splits);
            result.SetFloat(ShaderPropertyID.lightIntensity, lightIntensity);
            _materials.Add(result);
            return result;
        }


        public Shader ShaderAnimatedAdditiveInstanced
        {
            get
            {
                if (_animatedInstanced == null) Initialize();
                if (_animatedInstanced == null) ExceptionRendering.ShaderNotFound(ShaderDefName);
                return _animatedInstanced;
            }
        }
    }
}
