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
    public class MapComponent_AnimatedEffectManager : MapComponent
    {
        private readonly LinkedList<AnimatedEffect> animatedEffects = new LinkedList<AnimatedEffect>();
        //public Queue<AnimatedEffect> AnimatedEffects => animatedEffects;

        public MapComponent_AnimatedEffectManager(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            var pointer = animatedEffects.First;
            while (pointer != null)
            {
                if (pointer.Value.LifeTick <= 0)
                {
                    var tmp = pointer.Next;
                    animatedEffects.Remove(pointer);
                    pointer = tmp;
                }
                if (pointer != null)
                {
                    pointer.Value.TickFixed();
                    pointer = pointer.Next;
                }
            }

           
            //Log.Message(animatedEffects.Count.ToString());
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            for (int i = 0; i < animatedEffects.Count; i++)
            {
                animatedEffects.ElementAt(i).Update();
            }
        }

        public void RegisterEffect(AnimatedEffect effect)
        {
            effect.Start();
            animatedEffects.AddLast(effect);
        }   
    }
}
