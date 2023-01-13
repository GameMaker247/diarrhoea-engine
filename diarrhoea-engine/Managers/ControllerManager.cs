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
        private List<Key> keysPressed = new List<Key>();
        private List<Key> keysClicked = new List<Key>();

        public Action<Key> onKeyPressed;
        public Action<Key> onKeyClicked;
        public Action<Key> onKeyRelease;

        public bool IsKeyPressed(Key key) { return keysPressed.Contains(key); }
        
        private Vector2 lastPosition = new Vector2(0, 0);

        public ControllerManager(IInputContext ctx) 
        {
            ctx.Mice[0].Cursor.CursorMode = CursorMode.Raw;
            ctx.Mice[0].Click += OnMouseClick;
            ctx.Mice[0].MouseMove += OnMouseMove;
            ctx.Mice[0].Scroll += OnMouseScroll;
            ctx.Keyboards[0].KeyDown += (_, key, _) => { keysPressed?.Add(key); onKeyClicked?.Invoke(key); };
            ctx.Keyboards[0].KeyUp += (_, key, _) => { keysPressed?.Remove(key); onKeyRelease?.Invoke(key); };
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

        private void OnMouseClick(IMouse mouse, MouseButton btn, System.Numerics.Vector2 pos)
        { 

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

        public void Update()
        {
            keysPressed.ForEach(x =>
            {
                onKeyPressed?.Invoke(x);
            });
        }
    }
}
