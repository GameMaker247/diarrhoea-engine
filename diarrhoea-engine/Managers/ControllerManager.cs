using Silk.NET.Core.Contexts;
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
            Vector2 delta = current - lastPosition;
            YawCam(delta.X);
            PitchCam(-delta.Y);
            lastPosition = current;
        }

        private void OnMouseClick(IMouse mouse, MouseButton btn, System.Numerics.Vector2 arg3)
        {
            //Console.WriteLine($"CLICKED THE {btn} MOUSE BUTTON");
        }

        private void YawCam(float delta)
        {
            Program.camera.Yaw += (delta) / (float)Program.window.FramesPerSecond;
        }

        private void PitchCam(float delta)
        {
            Program.camera.Pitch += (delta) / (float)Program.window.FramesPerSecond;
            //if (Program.camera.pitch < -60.0f) Program.camera.pitch = -60.0f;
            //else if (Program.camera.pitch > 60.0f) Program.camera.pitch = 60.0f;
        }

        private void ZoomCam(float delta)
        {
            Program.camera.Zoom(delta * 30.0f / (float)Program.window.FramesPerSecond);
            //Program.camera.FOV += delta*30.0f/(float)Program.window.FramesPerSecond;
            //if (Program.camera.FOV < 30.0f) Program.camera.FOV = 30.0f;
            //else if (Program.camera.FOV > 130.0f) Program.camera.FOV = 130.0f;

            //Console.WriteLine(Program.camera.FOV);
        }

        private float speed = 6.0f;

        public void Update()
        {
            keysPressed.ForEach(x =>
            {
                switch (x)
                {
                    case Key.W:
                        {
                            Program.camera.Position += Program.camera.GetType() == typeof(FPSCamera) ? Program.camera.Forward * speed / (float)Program.window.FramesPerSecond : Program.camera.Up * speed / (float)Program.window.FramesPerSecond;
                        };
                        break;
                    case Key.A:
                        {
                            Program.camera.Position -= Vector3D.Normalize<float>(Vector3D.Cross<float>(Program.camera.Forward, Program.camera.Up)) * speed / (float)Program.window.FramesPerSecond;
                        };
                        break;
                    case Key.S:
                        {
                            Program.camera.Position -= Program.camera.GetType() == typeof(FPSCamera) ? Program.camera.Forward * speed / (float)Program.window.FramesPerSecond : Program.camera.Up * speed / (float)Program.window.FramesPerSecond;
                        };
                        break;
                    case Key.D:
                        {
                            Program.camera.Position += Vector3D.Normalize<float>(Vector3D.Cross<float>(Program.camera.Forward, Program.camera.Up)) * speed / (float)Program.window.FramesPerSecond;
                        };
                        break;
                    case Key.Escape:
                        {
                            Program.window.Close();
                        }
                        break;
                    case Key.ShiftLeft:
                        {
                            if (speed == 6.0f) speed = 12.0f;
                            else speed = 6.0f;
                        }
                        break;
                    case Key.B: 
                        { 
                            Program.world.SpawnEntity(new Entity("Mr. Bean", Model.Square, textures: new string[]{ "../../../Images/bean.png" }, position: Program.camera.Position, rotation: Program.camera.Forward, scale: 25.0f));     
                        }
                        break;
                    case Key.X:
                        {
                            Program.player.position += new Vector3D<float>(1.0f, 0, 0);
                        }
                        break;
                    case Key.Z:
                        {
                            Program.player.position += new Vector3D<float>(0, 0, 1.0f);
                        }
                        break;
                    case Key.Y:
                        {
                            Program.player.position += new Vector3D<float>(0, 1.0f, 0);
                        }
                        break;
                }
            });
        }
    }
}
