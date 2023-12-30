using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtilleryHitsThePlane.Entities
{
    internal class Bullet
    {
        public int X { get; set; } = 0;// 炮弹的横坐标
        public int Y{ get; set; } = 0;// 炮弹的纵坐标 
        public int Width { get; set; } = 0;//炮弹宽度 
        public int Height { get; set; } = 0;//炮弹高度 

        public int Speed { get; set; } = 0;// 炮弹的速度  



        public int BulletPosition { get; set; } // 炮弹的位置  
        public bool IsAlive { get; set; } // 炮弹是否还存在  
        
        public int Direction { get; set; } // 炮弹的方向（0为向上，1为向右，2为向下，3为向左）  

        public Bullet(int x, int y, int speed)
        {
            X = x;
            Y= y;
            Speed = speed;
        }

        public Bullet(int x , int y , int width, int height, int speed)
        {
            X = x;
            Y= y;
            Width = width;
            Height = height;    
            Speed = speed;
        }

        public void Move() // 炮弹移动的方法  
        {
            switch (Direction)
            {
                case 0: // 向上  
                    BulletPosition -= Speed;
                    break;
                case 1: // 向右  
                    BulletPosition += Speed;
                    break;
                case 2: // 向下  
                    BulletPosition += Speed;
                    break;
                case 3: // 向左  
                    BulletPosition -= Speed;
                    break;
            }
        }
    }
}
