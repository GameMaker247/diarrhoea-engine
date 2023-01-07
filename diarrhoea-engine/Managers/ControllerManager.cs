using Silk.NET.Input;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class ControllerManager
    {
        public List<Key> keysPressed = new List<Key>();
        public bool IsKeyPressed(Key key) { return keysPressed.Contains(key); }
        
        private Vector2 lastPosition = new Vector2(0, 0);

        public ControllerManager(IInputContext ctx) 
        {
            ctx.Mice[0].Cursor.CursorMode = CursorMode.Raw;
            ctx.Mice[0].Click += OnMouseClick;
            ctx.Mice[0].MouseMove += OnMouseMove;
            ctx.Mice[0].Scroll += OnMouseScroll;
            ctx.Keyboards[0].KeyDown += (_, key, _) => keysPressed?.Add(key);
            ctx.Keyboards[0].KeyUp += (_, key, _) => keysPressed?.Remove(key);
        }

        private void OnMouseScroll(IMouse arg1, ScrollWheel arg2)
        {
            ZoomCam(-arg2.Y);
        }

        private void OnMouseMove(IMouse mouse, Vector2 current)
        {
            Vector2 delta = lastPosition - current;
            RotateCam(-delta.X);
            lastPosition = current;
        }

        private void OnMouseClick(IMouse mouse, MouseButton btn, System.Numerics.Vector2 arg3)
        {
            Console.WriteLine($"CLICKED THE {btn} MOUSE BUTTON");
        }

        private void RotateCam(float delta)
        {
            Program.camera.rotation += (delta) / (float)Program.window.FramesPerSecond;
        }

        private void ZoomCam(float delta)
        {
            Program.camera.FOV += delta*30.0f/(float)Program.window.FramesPerSecond;
            if (Program.camera.FOV < 30.0f) Program.camera.FOV = 30.0f;
            else if (Program.camera.FOV > 130.0f) Program.camera.FOV = 130.0f;

            Console.WriteLine(Program.camera.FOV);
        }

        public void Update()
        {
            keysPressed.ForEach(x =>
            {
                switch (x)
                {
                    case Key.W:
                        {
                            Program.camera.position += new Vector3D<float>(0, 0, 1.0f / (float)Program.window.FramesPerSecond);
                        };
                        break;
                    case Key.A:
                        {
                            Program.camera.position += new Vector3D<float>(1.0f / (float)Program.window.FramesPerSecond, 0, 0);
                        };
                        break;
                    case Key.S:
                        {
                            Program.camera.position += new Vector3D<float>(0, 0, -1.0f / (float)Program.window.FramesPerSecond);
                        };
                        break;
                    case Key.D:
                        {
                            Program.camera.position += new Vector3D<float>(-1.0f / (float)Program.window.FramesPerSecond, 0, 0);
                        };
                        break;
                    case Key.Q:
                        {
                            Program.camera.rotation -= 45.0f / (float)Program.window.FramesPerSecond;
                            if (Program.camera.rotation <= 0.0f) Program.camera.rotation = 360.0f;
                        }
                        break;
                    case Key.E:
                        {
                            Program.camera.rotation += 45.0f / (float)Program.window.FramesPerSecond;
                            if (Program.camera.rotation >= 360.0f) Program.camera.rotation = 0.0f;
                        }
                        break;
                    case Key.Escape:
                        {
                            Program.window.Close();
                        }
                        break;
                }
            });
        }
    }
}
