using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtilleryHitsThePlane.Entities
{
  
    internal class Cannon
    {
        public bool OnFired { get; set; } = true;
        public int Angle { get; set; } = 90;

        public int X { get; set; } = 0;//大炮横坐标 
        public int Y { get; set; } = 0;//大炮纵坐标 

        public int Width { get; set; } = 0;//大炮宽度 
        public int Height { get; set; } = 0;//大炮高度 

        public Cannon(){}

        public Cannon(int cannonAngle) 
        {
            Angle = cannonAngle;
        }

        public Cannon(int x,int y)
        {
            X = x;
            Y = y;
        }
    }
}
