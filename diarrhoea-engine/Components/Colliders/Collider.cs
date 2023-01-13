using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine.Colliders
{
    public class Collider : Component
    {
        public bool isTrigger = false;

        public Collider(bool isTrigger)
        {
            this.isTrigger = isTrigger;
        }

        public virtual bool IsColliding(Collider other) { return false; }
    }
}
