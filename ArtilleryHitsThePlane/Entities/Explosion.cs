using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtilleryHitsThePlane.Entities
{
    internal class Explosion
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int ExplosionFrame { get; set; }
        public bool IsActive { get; set; }
        public int ExplosionFrameCount { get; set; }

        public Explosion(int x, int y, int explosionFrameCount)
        {
            X = x;
            Y = y;
            ExplosionFrame = 0;
            IsActive = true;
            ExplosionFrameCount = explosionFrameCount;
        }
}
}
