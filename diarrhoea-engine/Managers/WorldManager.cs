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
            /* IMPORTANT!!! TRY THIS FUNCTION WITH OTHER RETARD SHIT NOT WORKING */
            //X = -Z
            //Y = X
            //Z = -Y
            /* ----------------------------------------------------------------- */

            IEnumerable<Entity> sortByPosition =
                from entity in entities
                orderby MathHelper.Distance(entity.position, new Vector3D<float>(-Program.camera.Position.Z, Program.camera.Position.X, -Program.camera.Position.Y)) descending
                select entity;


            //tiles.ForEach(x => x.Draw());
            foreach(Entity x in sortByPosition) 
            {
                x.Draw();
            }
        }
    }
}
