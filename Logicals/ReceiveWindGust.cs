using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mantra.Framework;
using Mantra.XNA;
using Microsoft.Xna.Framework;

namespace LD12.Logicals
{
    class ReceiveWindGust : Behavior
    {
        [Dependency]
        Transform transform = null;

        [Dependency(Group = Groups.Wind)]
        WindStroker stroker = null;

        [Dependency]
        BoxCollider collider = null;

        public int AmountOfUpwardsStrokes
        {
            get
            {
                return amountOfUpwardsStrokes;
            }
        }

        int amountOfUpwardsStrokes = 0;

        Vector3 a, b;

        float maxAngle = 15;

        public override void Initialize()
        {
            base.Initialize();

            a = Vector3.Transform(Vector3.Up, Matrix.CreateRotationZ(MathHelper.ToRadians(-maxAngle)));
            b = Vector3.Transform(Vector3.Up, Matrix.CreateRotationZ(MathHelper.ToRadians(maxAngle)));
        }

        public override void Update(TimeSpan elapsed)
        {
            float radius = 3; // hack, crates are 2.5f

            if (stroker.activeStroke != null) {
                foreach (WindSegment segment in stroker.activeStroke.segments) {
                    float d = Vector3.Distance(transform.Position, segment.Position);

                    if (d <= segment.Radius + radius) {
                        Vector3 force = segment.Direction * (segment.Radius * 0.1f);

                        collider.Geom.applyImpulse(transform.Position, force);

                        BoundingBox rooftop = Repository.Get<BoxPlacer>("box placer").RooftopArea;

                        if (!(rooftop.Contains(collider.Geom.centerPosition) == ContainmentType.Contains || 
                            rooftop.Contains(collider.Geom.centerPosition) == ContainmentType.Intersects)) {
                            float distA = Vector3.Distance(segment.Direction, a);
                            float distB = Vector3.Distance(segment.Direction, b);

                            if (distA < 0.5f || distB < 0.5f) {
                                amountOfUpwardsStrokes++;
                            }
                        }
                    }
                }
            }
        }
    }
}
