using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine.Managers
{
    public class WallManager
    {
        private static List<Wall> walls = new List<Wall>();

        private static bool InRange(Vector3D<int> a, Vector3D<int> b, out int Pos)
        {
            if (a.X - 1 == b.X) 
            {
                Pos = 0;
                return true;
            }

            if(a.X + 1 == b.X)
            {
                Pos = 1;
                return true;
            }

            if (a.Z - 1 == b.Z)
            {
                Pos = 2;
                return true;
            }

            if (a.Z + 1 == b.Z)
            {
                Pos = 3;
                return true;
            }

            Pos = -1;
            return false;
        }
        public static void AddWall(Vector3D<int> Position)
        {
            Wall wall = new Wall(Position, WallType.STRAIGHT);
            
            walls.Add(wall);
            walls.ForEach(x =>
            {
                if (InRange(wall.Position, x.Position, out int Pos))
                {
                    wall.Connect(x, Pos);
                    x.Connect(wall, Math.Abs(Pos - 4));
                }
            });
        }

        public static void DrawWalls()
        {
            walls.ForEach(x =>
            {
                //get vertices, indices, texcoords etc. and build
            });
        }
    }

    public class Wall
    {
        private Wall?[] connected = new Wall?[4]{ null,null,null,null };
        public Vector3D<int> Position { get; private set; }
        public WallType Type { get; private set; }

        public Wall(Vector3D<int> Position, WallType Type)
        {
            this.Position = Position;
            this.Type = Type;
        }

        public void Connect(Wall wall, int Position)
        {
            connected[Position] = wall;
        }
    }

    public enum WallType
    {
        STRAIGHT,
        CURVE
    }
}
