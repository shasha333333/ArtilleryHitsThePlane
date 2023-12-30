using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtilleryHitsThePlane.Entities
{
    internal class Plane
    {
        public int X { get; set; } //飞机的横坐标
        public int Y { get; set; } //飞机的横坐标
        public int Width { get; set; } = 0; //飞机宽度 
        public int Height { get; set; } = 0;    //飞机高度 
        public bool IsAlive { get; set; }   // 飞机是否还存在  
        public int Speed {  get; set; } //飞机速度
        public int DownSpeed { get; set; } //飞机速度

        public Plane()
        {
            IsAlive = true;
        }

        public Plane(int x,int y, bool isAlive)
        {
            X = x;
            Y = y;
            IsAlive = isAlive;
        }

      
    }
}
