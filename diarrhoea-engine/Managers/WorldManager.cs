using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public static class WorldManager
    {
        public static List<Entity> entities = new List<Entity>(); //Includes players, NPCs and world objects (Doesn't include Tiles)
        private static List<Entity> uninitialized = new List<Entity>();

        public static Entity SpawnEntity(Entity entity)
        {
            entities.Add(entity);
            uninitialized.Add(entity);
            return entity;
        }

        public static void RemoveEntity(Entity entity)
        { 
            entities.Remove(entity);
        }

        public static void Update()
        {
            foreach(Entity entity in entities.ToList()) 
            {
                entity.Update();
            }
        }

        public static Entity FindEntity(string name)
        {
            return entities.Find(x => x.Name == name);
        }

        public static void Initialize()
        {
            uninitialized.ForEach(x =>
            {
                x.Initialize();
            });

            uninitialized.Clear();
        }

        public static void Render()
        {
            IEnumerable<Entity> sortByPosition =
                from entity in entities
                orderby MathHelper.Distance(entity.Position, Program.camera.Position) descending
                select entity;


            //tiles.ForEach(x => x.Draw());
            foreach(Entity x in sortByPosition) 
            {
                x.Draw();
            }
        }
    }
}

/* IMPORTANT!!! TRY THIS FUNCTION WITH OTHER RETARD SHIT NOT WORKING */
//X = -Z
//Y = X
//Z = -Y (need to check Cameras Y position to invert if below 0)
/* ----------------------------------------------------------------- */
// I THINK I FIXED THIS
//                orderby Program.camera.Position.Y > 0.0f ? MathHelper.Distance(entity.Position, new Vector3D<float>(-Program.camera.Position.Z, Program.camera.Position.X, -Program.camera.Position.Y)) : MathHelper.Distance(entity.Position, new Vector3D<float>(-Program.camera.Position.Z, Program.camera.Position.X, Program.camera.Position.Y)) descending
