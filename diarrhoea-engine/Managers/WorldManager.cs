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

        public void SpawnEntity(string name, Model model)
        {
            entities.Add(new Entity(name, model, "../../../Images/retard.png"));
        }

        public void Render()
        {
            //tiles.ForEach(x => x.Draw());
            entities.ForEach(x => x.Draw());
        }
    }
}
