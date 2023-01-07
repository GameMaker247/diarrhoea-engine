using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class WorldManager
    {
        public List<Entity> entities = new List<Entity>(); //Includes players, NPCs and world objects (Doesn't include Tiles)

        public void SpawnEntity(Entity entity)
        {
            entities.Add(entity);
        }

        public void Render()
        {
            //tiles.ForEach(x => x.Draw());
            entities.ForEach(x => x.Draw());
        }
    }
}
