using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine.Colliders
{
    public class MeshCollider : Collider
    {
        private Model model;

        public MeshCollider(Model model, bool isTrigger = false) : base(isTrigger)
        {
            this.model = model;
        }

        public override void Update()
        {

        }

        public override bool IsColliding(Collider other)
        {
            return false;
        }
    }
}
