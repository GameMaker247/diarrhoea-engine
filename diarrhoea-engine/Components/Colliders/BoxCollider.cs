using DiarrhoeaEngine.Colliders;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class BoxCollider : Collider
    {
        private Vector3D<float> boundaries;

        public BoxCollider(Vector3D<float> boundaries, bool isTrigger = false) : base(isTrigger)
        {
            this.boundaries = boundaries;
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
