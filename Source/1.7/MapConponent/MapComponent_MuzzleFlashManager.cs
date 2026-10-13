using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using UnityEngine;
using RimWorld.Planet;

namespace MuzzleFlash
{
    public class MapComponent_MuzzleFlashManager : MapComponent
    {
        private readonly List<MuzzleFlashEntity> _entities;

        public MapComponent_MuzzleFlashManager(Map map) : base(map)
        {
            _entities = new List<MuzzleFlashEntity>();
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            //remove dead entities,
            //just replace the dead entity with the last entity and remove the last entity
            int length = _entities.Count;
            int i = 0;
            while (i < length)
            {
                var entity = _entities[i];
                if (!entity.IsAlive)
                {
                    _entities[i] = _entities[length - 1];
                    _entities.RemoveAt(length - 1);
                    length--;
                }
                else
                {
                    entity.Tick();
                    _entities[i] = entity;//it a struct, so we need to replace it
                    i++;
                }
            }
        }

        // RimWorld 1.7 (Waylith): instanced draw 必须在 MapComponentDraw 阶段提交。
        // 参照本体 VacuumComponent.MapComponentDraw 的官方用法,
        // 在 MapComponentUpdate(逻辑帧)期间提交的 draw 不会进入渲染管线。
        public override void MapComponentDraw()
        {
            if (WorldRendererUtility.WorldRendered || Find.CurrentMap != this.map) return;

            for (int i = 0; i < _entities.Count; i++)
            {
                var entity = _entities[i];
                AnimatedRenderManager.Default.AddInstance(entity.def.RenderID, entity.position, entity.rotCos, entity.rotSin, entity.size, Color.white, entity.frame);
            }

            AnimatedRenderManager.Default.Draw();
        }

        public void RegisterEntity(MuzzleFlashEntity entity)
        {
            _entities.Add(entity);
        }
    }
}
