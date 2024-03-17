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
    public class AnimatedEffect
    {
        public AnimatedShaderController shaderController;
        public Vector3 drawSize = new Vector3(0.2f, 1f, 0.2f);
        public AnimatedEffectDef def;

        public Vector3 drawPos;
        public float angle;

        private int lifeTicks = 60;

        public int LifeTick => lifeTicks;

        public AnimatedEffect()
        {

        }

        public AnimatedEffect(Material material)
        {
            shaderController = new AnimatedShaderController(material);
        }

        public void TickFixed()
        {
            Tick();
            lifeTicks--;
        }

        public void Start()
        {
            if (def != null) lifeTicks = def.duration;
            shaderController.SetSplits(def.splits);
            shaderController.Stage = Mathf.Floor(Rand.Range(0, def.splits.y)) * (def.framesPerAnimation);
            shaderController.SetLightIntensity(def.lightIntensity);
        }

        protected void Draw()
        {
            
            float num = (angle - 90f) % 360f;
            Matrix4x4 matrix = default;
            Quaternion quat = Quaternion.AngleAxis(num, Vector3.up);
            matrix.SetTRS(drawPos, quat, drawSize);
            Graphics.DrawMesh(MeshPool.plane10, matrix, shaderController.Material, 0);

            //animatedShaderController.Material.SetPass(0);
            //BaseContent.BadMat.SetPass(0);
            
            //Graphics.DrawMeshNow(MeshPool.plane10, matrix);
        }

        
        public virtual void Tick() 
        {
            if (shaderController == null) return;
            shaderController.Stage += (def.framesPerAnimation-1) / def.duration;
        }
        public virtual void Update()
        {
            Draw();
        }
    }
}
