using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using UnityEngine;
using System.IO;

namespace AEF
{
    [StaticConstructorOnStartup]
    public class AnimatedEffectMod : Mod
    {
        static AnimatedEffectMod()
        {
            
        }

        public AnimatedEffectMod(ModContentPack content) : base(content)
        {
            LongEventHandler.QueueLongEvent(() =>
            {
                foreach(AssetBundle asset in content.assetBundles.loadedAssetBundles)
                {
                    ShaderPool.LoadShader(asset);
                }
                
            }, "MF_LoadingShaders", false, null, true);
            
        }

    }
}
