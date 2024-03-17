using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MuzzleFlash
{
    public class GameComponent_MFGarbageCollections : GameComponent
    {
        public GameComponent_MFGarbageCollections(Game _)
        {

        }

        private int timer = 0;
        private const int collectionInterval = 10800;
        public override void GameComponentTick()
        {
            timer++;
            if (timer >= collectionInterval)
            {
                CacheWeaponsDrawPos.Clear();
                timer = 0;
            }
        }
    }
}
