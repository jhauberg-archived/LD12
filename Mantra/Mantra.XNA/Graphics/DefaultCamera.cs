using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mantra.Framework;

namespace Mantra.XNA.Graphics
{
    /// <summary>
    /// Represents a camera that looks at a target, with a perspective projection.
    /// </summary>
    public class DefaultCamera : Camera
    {
        [Dependency]
        Transform transform = null;

        public DefaultCamera(GraphicsDevice device)
            : base(device) { }

        public Transform Target { get; set; }

        public override Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(
                    transform.Position, 
                    Target == null ? Vector3.Zero : Target.Position, 
                    Vector3.Up);
            }
        }
    }
}
