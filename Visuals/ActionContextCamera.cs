using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mantra.Framework;
using Mantra.XNA;
using Microsoft.Xna.Framework;
using Mantra.XNA.Graphics;
using Microsoft.Xna.Framework.Graphics;
using LD12.Logicals;

namespace LD12.Visuals
{
    public class ActionContextCamera : Camera
    {
        [Dependency]
        Transform transform = null;

        public ActionContextCamera(GraphicsDevice device)
            : base(device) { }

        public Transform Target { get; set; }

        Vector3 tempTarget;
        Vector3 actualLookAt;

        public Vector3 ActualTarget { get { return actualLookAt; } }

        public override void Update(TimeSpan elapsed)
        {
            tempTarget = Target.Position;

            var results = Repository.Behaviors.Where
                (
                    x => x.Group.IndexOf("Crate #") != -1 && x is Transform
                );

            Transform furthestBox = null;

            float previousDistance = 0;

            foreach (Transform t in results) {
                float d = Vector3.Distance(t.Position, Target.Position);

                if (d > previousDistance) {
                    previousDistance = d;

                    furthestBox = t;
                }
            }

            if (furthestBox != null && !Repository.Get<BoxPlacer>("box placer").Placing) {
                tempTarget = furthestBox.Position;
            }

            float dist = Vector3.Distance(tempTarget, actualLookAt);

            if (dist > 0.1f) {
                actualLookAt += (Vector3.Normalize(tempTarget - actualLookAt) * dist) * (float)elapsed.TotalSeconds;
            }
        }

        public override Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(
                    transform.Position,
                    actualLookAt,
                    Vector3.Up);
            }
        }
    }
}
