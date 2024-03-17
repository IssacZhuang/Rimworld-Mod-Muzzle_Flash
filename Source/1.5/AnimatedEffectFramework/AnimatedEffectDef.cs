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
    public class AnimatedEffectDef : Def
    {
        [NoTranslate]
        public string texture;
        public Vector2 defaultSize;

        public Vector2 splits = new Vector2(5, 5);
        public Vector2 drawOffsetMultiplier = Vector2.zero;

        public int duration = 30;
        public float framesPerAnimation = 3;
        public float lightIntensity = 1f;

        private Texture2D cacheTexture;

        public Texture2D Texture
        {
            get
            {
                if (cacheTexture == null) cacheTexture = ContentFinder<Texture2D>.Get(texture);
                return cacheTexture;
            }
        }

        public AnimatedEffect CreateAnimatedEffect()
        {
            Material material = MaterialPool.CreateMaterial(ShaderPool.Animated, this.Texture);
            AnimatedEffect effect = new AnimatedEffect(material)
            {
                def = this,
            };
            return effect;
        }

        public override void PostLoad()
        {
            base.PostLoad();
            //InitialCahce();
        }

        public override IEnumerable<string> ConfigErrors()
        { 
            return base.ConfigErrors();
        }
    }
}
