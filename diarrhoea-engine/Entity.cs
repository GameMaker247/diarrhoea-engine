using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace DiarrhoeaEngine
{
    public class Entity
    {
        public string Name { get; set; }
        public Vector3D<float> Position { get; set; }
        public Vector3D<float> Rotation { get; set; }
        public float scale;

        private Renderer renderer;
        private List<Component> components = new List<Component>();

        public Action onUpdate;

        public Entity(string Name, ref Renderer renderer, List<Component> components = null, Vector3D<float>? Position = null, Vector3D<float>? Rotation = null, float scale = 1.0f)
        {
            this.Name= Name;

            this.renderer = new Renderer(renderer);
            this.renderer.SetEntity(this);
            this.renderer.Initialize();

            if (components != null)
            {
                this.components = components;
                this.components.ForEach(x => x.SetEntity(this));
                this.components.ForEach(x => x.Initialize());
            }

            if (Position == null) this.Position = Vector3D<float>.Zero;
            else this.Position = (Vector3D<float>)Position;


            if (Rotation == null) this.Rotation = Vector3D<float>.Zero;
            else this.Rotation = (Vector3D<float>)Rotation;

            this.scale= scale;
        }

        public void AddComponent(Component component)
        {
            if (component.multiple == false && components.Exists(x => x.GetType() == component.GetType())) return;

            component.SetEntity(this);
            component.Initialize();
            components.Add(component);
        }

        public Component GetComponent(Type type) { return components.Find(x => x.GetType() == type); }

        public void Draw()
        {
            renderer.Update();
        }

        public void Update()
        {
            components?.ForEach(x => x.Update());
            onUpdate?.Invoke();
            //if (MathHelper.Distance(Position, new Vector3D<float>(-Program.camera.Position.Z, Program.camera.Position.X, -Program.camera.Position.Y)) < 10.0f) Destroy();
        }

        public void Destroy()
        {
            WorldManager.RemoveEntity(this);
        }
    }
}
