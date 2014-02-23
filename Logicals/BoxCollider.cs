using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LD12.Drawables;
using Mantra.XNA;

using Physics = BEPUphysics;
using Mantra.Framework;
using Microsoft.Xna.Framework;

namespace LD12.Logicals
{
    class BoxCollider : Behavior
    {
        [Dependency]
        Transform transform = null;

        [Dependency]
        Box box = null;

        Physics.Box geom;

        public Physics.Box Geom
        {
            get
            {
                return geom;
            }
        }

        bool isStatic;

        public BoxCollider(bool isStatic)
        {
            this.isStatic = isStatic;
        }

        public override void Initialize()
        {
            base.Initialize();

            geom = new BEPUphysics.Box(transform.Position, box.Width, box.Height, box.Depth);
            geom.makePhysical(1);

            geom.isAffectedByGravity = !isStatic;
            geom.isPhysicallySimulated = !isStatic;

            GameContainer.Space.add(geom);
        }

        public override void Update(TimeSpan elapsed)
        {
            transform.Position = geom.centerPosition;
            transform.Rotation = Quaternion.CreateFromRotationMatrix(geom.rotationMatrix);
        }
    }
}
