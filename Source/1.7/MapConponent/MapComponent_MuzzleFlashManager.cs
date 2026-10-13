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

        private int _debugRendererID = -1;
        private float _nextDiagLogTime;

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
                AnimatedRenderManager.Default.AddInstance(entity.def.RenderID, entity.position, entity.rotation, entity.size, Color.white, entity.frame);
            }

            if (Prefs.DevMode) DrawDebugOverlay();

            AnimatedRenderManager.Default.Draw();

            if (Time.realtimeSinceStartup > _nextDiagLogTime)
            {
                _nextDiagLogTime = Time.realtimeSinceStartup + 5f;
                Log.Message($"[MF diag] entities={_entities.Count}");
            }
        }

        /// <summary>
        /// DevMode 诊断: 在地图中心画 5 个测试四边形。
        /// 品红 = 绘制管线正常但 SSBO 数据未到达; 品红偏绿/发白 = SSBO 数据到达; 完全不可见 = draw 未进入渲染管线。
        /// </summary>
        private void DrawDebugOverlay()
        {
            if (_debugRendererID < 0)
            {
                Material mat = AssetsManager.Default.GetOrCreateDebugMaterial();
                if (mat == null) return;
                _debugRendererID = AnimatedRenderManager.Default.GetRendererID(MeshPool.plane10, mat);
            }

            Vector3 center = new Vector3(map.Size.x * 0.5f, AltitudeLayer.VisEffects.AltitudeFor(), map.Size.z * 0.5f);
            Vector3 scale = new Vector3(1.5f, 1f, 1.5f);
            AnimatedRenderManager.Default.AddInstance(_debugRendererID, center, Quaternion.identity, scale, Color.white, 0f);
            AnimatedRenderManager.Default.AddInstance(_debugRendererID, center + new Vector3( 2.5f, 0,  0), Quaternion.identity, scale, Color.white, 0f);
            AnimatedRenderManager.Default.AddInstance(_debugRendererID, center + new Vector3(-2.5f, 0,  0), Quaternion.identity, scale, Color.white, 0f);
            AnimatedRenderManager.Default.AddInstance(_debugRendererID, center + new Vector3( 0, 0,  2.5f), Quaternion.identity, scale, Color.white, 0f);
            AnimatedRenderManager.Default.AddInstance(_debugRendererID, center + new Vector3( 0, 0, -2.5f), Quaternion.identity, scale, Color.white, 0f);
        }

        public void RegisterEntity(MuzzleFlashEntity entity)
        {
            _entities.Add(entity);
        }
    }
}
