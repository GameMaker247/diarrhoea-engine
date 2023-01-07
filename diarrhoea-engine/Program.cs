﻿using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Input;
using System.Drawing;

namespace DiarrhoeaEngine
{
    public static class Program
    {
        public static ShaderManager shader = new ShaderManager();
        public static CameraManager camera = new CameraManager();
        public static WorldManager world = new WorldManager();
        
        public static ControllerManager controls;
        public static GL GL;

        private static IWindow window;
        public static Vector2D<int> GetWindowSize()
        {
            return window.Size;
        }

        public static Matrix4X4<float> _view;
        public static Matrix4X4<float> _projection;

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
            GL = window.CreateOpenGL();

            GL.ClearColor(Color.Aqua);
            
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend); 

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _view = Matrix4X4.CreateTranslation<float>(camera.position);
            _projection = Matrix4X4.CreatePerspectiveFieldOfView<float>(((float)Math.PI / 180) * 90.0f, Program.GetWindowSize().X / Program.GetWindowSize().Y, 0.001f, 1000.0f);

            world.SpawnEntity("Player", Model.Square);
        }

        private static void OnRender(double obj)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _view = Matrix4X4.CreateTranslation<float>(camera.position);
            _projection = Matrix4X4.CreatePerspectiveFieldOfView<float>(((float)Math.PI / 180) * 90.0f, Program.GetWindowSize().X / Program.GetWindowSize().Y, 0.001f, 1000.0f);

            camera.Render();
            world.Render();
            //UI.Render();
        }


        public static float loop = 0.0f;
        private static bool up = true;

        private static void OnUpdate(double obj)
        {
            //controls.Update();
            controls.keysPressed.ForEach(x =>
            {
                switch (x)
                {
                    case Key.W:
                        {
                            Program.camera.position += new Vector3D<float>(0, 0, 1.0f / (float)window.FramesPerSecond);
                        };
                        break;
                    case Key.A:
                        {
                            Program.camera.position += new Vector3D<float>(1.0f / (float)window.FramesPerSecond, 0, 0);
                        };
                        break;
                    case Key.S:
                        {
                            Program.camera.position += new Vector3D<float>(0, 0, -1.0f / (float)window.FramesPerSecond);
                        };
                        break;
                    case Key.D:
                        {
                            Program.camera.position += new Vector3D<float>(-1.0f / (float)window.FramesPerSecond, 0, 0);
                        };
                        break;
                }
            });
            if (up)
            {
                loop += 1.0f / (float)window.FramesPerSecond;
                if (loop >= 1.0f) up = false;
            }
            else
            {
                loop -= 1.0f / (float)window.FramesPerSecond;
                if (loop <= 0.0f) up = true;
            }
            
        }
        private static void OnResize(Vector2D<int> obj)
        {

        }
    }
}
