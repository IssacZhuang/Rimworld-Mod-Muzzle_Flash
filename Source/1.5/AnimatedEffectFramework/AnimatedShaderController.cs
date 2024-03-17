using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AEF
{
    public class AnimatedShaderController
    {
        private Material material;
        private float cachedStage = 0f;

        public Material Material
        {
            get { return material; }
            set { material = value; }
        }

        public float Stage
        {
            get
            {
                return cachedStage;
            }
            set
            {
                cachedStage = value;
                this.material?.SetFloat(ShaderPool.ID_FRAME, cachedStage);
            }
        }

        public AnimatedShaderController()
        {

        }

        public AnimatedShaderController(Material material)
        {
            this.material = material ?? throw new UnityException("Class AnimatedShaderController must loaded with the material");
            SetSplits(new Vector2(4,4));

        }

        public AnimatedShaderController(Material material, Vector2 splits)
        {
            this.material = material ?? throw new UnityException("Class AnimatedShaderController must loaded with the material");
            SetSplits(splits);
        }

        public void SetSplits(Vector2 splits)
        {
            if (this.material == null) return;
            this.material.SetVector(ShaderPool.ID_SPLITS, splits);
            
        }

        public void SetLightIntensity(float value)
        {
            this.material.SetFloat(ShaderPool.ID_LIGHT_INTENSITY, value);
        }
    }

}
