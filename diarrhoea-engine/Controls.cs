using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class Controls
    {
        private static List<Key> keysPressed = new List<Key>();
        public bool IsKeyPressed(Key key) { return keysPressed.Contains(key); }

        public Controls(IInputContext ctx) 
        {
            ctx.Mice[0].Click += OnMouseClick;
            ctx.Keyboards[0].KeyDown += (_, key, _) => keysPressed?.Add(key);
            ctx.Keyboards[0].KeyUp += (_, key, _) => keysPressed?.Remove(key);
        }

        private void OnMouseClick(IMouse mouse, MouseButton btn, System.Numerics.Vector2 arg3)
        {
            Console.WriteLine($"CLICKED THE {btn} MOUSE BUTTON");
        }

        public void Update()
        {

        }
    }
}
