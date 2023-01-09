using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class Component
    {
        public bool multiple = false;
        public Entity entity;

        public void SetEntity(Entity entity) { this.entity = entity; }
        public virtual void Initialize() { }
        public virtual void Update() { }
    }
}
