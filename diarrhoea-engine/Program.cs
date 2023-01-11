using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Input;
using System.Drawing;

namespace DiarrhoeaEngine
{
    public static class Program
    {
        public static Camera camera;
        public static Entity player;

        public static ControllerManager controls { get; private set; }
        public static GL GL;

        public static IWindow window;
        public static Vector2D<int> GetWindowSize()
        {
            return window.Size;
        }

        public static Matrix4X4<float> _view;
        public static Matrix4X4<float> _projection;
        private static Random rand = new Random();

        public static Renderer obj;
        public static Renderer example;
        public static Renderer player_renderer;

        private static unsafe void Main(string[] args)
        {
            WindowOptions options = WindowOptions.Default;
            options.Title = "Diarrhoea Engine -- V 0.0.1";
            options.Size = new Vector2D<int>(1280, 720);
            options.FramesPerSecond = 60;
            
            window = Window.Create(options);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Resize += OnResize;

            window.Run();
        }
        private static void OnLoad()
        {
            controls = new ControllerManager(window.CreateInput());
            //camera = new RTSCamera(GetWindowSize().X / 32, GetWindowSize().Y / 32, Vector3D<float>.Zero);
            camera = new FPSCamera(80.0f, GetWindowSize().X / GetWindowSize().Y, new Vector3D<float>(0.0f, 4.0f, 0.0f));

            GL = window.CreateOpenGL();

            GL.ClearColor(Color.Aqua);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            //GL.Enable(EnableCap.CullFace); Helps in FPS but not RTS
            GL.Enable(EnableCap.DepthClamp);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _view = camera.GetViewMatrix();
            _projection = camera.GetProjectionMatrix();

            int gridSize = 10;

            //Model.LoadModel("../../../Models/retard.obj");

            obj = new Renderer(Model.LoadModel("../../../Models/house.obj"), shader: "default", new string[] { "../../../Models/house.png" });
            example = new Renderer(Model.Square, shader: "fade", textures: new string[] { "../../../Images/retard.png", "../../../Images/bean.png" });
            player_renderer = new Renderer(Model.Square, textures: new string[] { "../../../Images/bean.png" });

            for (int x = -gridSize; x < gridSize; x++)
            {
                for (int z = -gridSize; z < gridSize; z++)
                {
                    //if (rand.Next(0, 2) == 1)
                        WorldManager.SpawnEntity(new Entity($"NPC ({x + z})", ref example, Position: new Vector3D<float>(x, -0.1f, z), Rotation: new Vector3D<float>(0.0f, 90.0f, 90.0f)));//new Entity("Player", Model.Square, textures: new string[]{ "../../../Images/retard.png", "../../../Images/bean.png" }, shader: "fade", position: new Vector3D<float>(x, z, 0), rotation: new Vector3D<float>(-90.0f, 45.0f, 180.0f), scale: 1.0f));
                }
            }

           for (int i = 0; i < 20; i++)
           {
                WorldManager.SpawnEntity(new Entity("Stall", ref obj, Position: new Vector3D<float>(i*12.0f, 0.0f, 0.0f), Rotation: new Vector3D<float>(0.0f, 90.0f, 0.0f), scale: 1.0f));
           }

            
            //WorldManager.SpawnEntity(player); 
            WorldManager.SpawnEntity(new Entity("Mr. Bean", ref player_renderer, Position: new Vector3D<float>(0.0f, 12.0f, 36.0f), Rotation: new Vector3D<float>(-25.0f, 45.0f, 0.0f), scale: 25.0f));
            WorldManager.SpawnEntity(new Entity("Mr. Bean2", ref player_renderer, Position: new Vector3D<float>(0.0f, -20.0f, 0.0f), Rotation: new Vector3D<float>(0.0f, 90.0f, 90.0f), scale: 25.0f));
        }

        private static void OnRender(double obj)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _view = camera.GetViewMatrix();
            _projection = camera.GetProjectionMatrix();

            WorldManager.Render();
        }


        public static float loop = 0.0f;
        private static bool up = true;
        private static float loopSpeed = 6.0f;
        private static void OnUpdate(double obj)
        {
            controls.Update();
            
            WorldManager.Update();
            WorldManager.FindEntity("Mr. Bean").Rotation = new Vector3D<float>(loop*360.0f, loop*360.0f, 0.0f);
            if (up)
            {
                loop += 1.0f / loopSpeed / (float)window.FramesPerSecond;
                if (loop >= 1.0f) up = false;
            }
            else
            {
                loop -= 1.0f / loopSpeed / (float)window.FramesPerSecond;
                if (loop <= 0.0f) up = true;
            }
            
        }
        private static void OnResize(Vector2D<int> obj)
        {

        }
    }
}
