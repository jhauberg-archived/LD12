using Mantra.Framework;
using Mantra.XNA;
using Mantra.XNA.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD12.Drawables
{
    class Box : Drawable
    {
        [Dependency]
        Transform transform = null;

        VertexDeclaration vertexDeclaration;
        VertexPositionNormalColor[] vertices;

        BasicEffect effect;

        Color color = Color.LightGray;

        float width, height, depth;

        public Box(float width, float height, float depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            effect = new BasicEffect(GameContainer.Graphics.GraphicsDevice, null);
            effect.EnableDefaultLighting();
            effect.VertexColorEnabled = true;

            vertexDeclaration = new VertexDeclaration(GameContainer.Graphics.GraphicsDevice, VertexPositionNormalColor.VertexElements);

            BuildVertices();
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            Camera cam = Cameras.Current;
 
            CullMode previousCullMode = device.RenderState.CullMode;

            device.RenderState.CullMode = CullMode.CullClockwiseFace;

            effect.View = cam.View;
            effect.Projection = cam.Projection;
            effect.World = transform.World;

            effect.DiffuseColor = color.ToVector3();

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Begin();

                device.VertexDeclaration = vertexDeclaration;
                device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);

                pass.End();
            }
            effect.End();

            device.RenderState.CullMode = previousCullMode;
        }

        void BuildVertices()
        {
            float w = width / 2;
            float h = height / 2;
            float d = depth / 2;

            Vector3 backUpperLeft = new Vector3(-w, h, -d);
            Vector3 backUpperRight = new Vector3(w, h, -d);
            Vector3 backLowerLeft = new Vector3(-w, -h, -d);
            Vector3 backLowerRight = new Vector3(w, -h, -d);

            Vector3 frontUpperLeft = new Vector3(-w, h, d);
            Vector3 frontUpperRight = new Vector3(w, h, d);
            Vector3 frontLowerLeft = new Vector3(-w, -h, d);
            Vector3 frontLowerRight = new Vector3(w, -h, d);

            vertices = new VertexPositionNormalColor[]
            {
                // front
                new VertexPositionNormalColor(frontUpperRight, Vector3.Forward, color),
                new VertexPositionNormalColor(frontUpperLeft, Vector3.Forward, color),
                new VertexPositionNormalColor(frontLowerLeft, Vector3.Forward, color),

                new VertexPositionNormalColor(frontUpperRight, Vector3.Forward, color),
                new VertexPositionNormalColor(frontLowerLeft, Vector3.Forward, color),
                new VertexPositionNormalColor(frontLowerRight, Vector3.Forward, color),

                // bottom
                new VertexPositionNormalColor(backLowerRight, Vector3.Down, color),
                new VertexPositionNormalColor(frontLowerLeft, Vector3.Down, color),
                new VertexPositionNormalColor(backLowerLeft, Vector3.Down, color),

                new VertexPositionNormalColor(frontLowerRight, Vector3.Down, color),
                new VertexPositionNormalColor(frontLowerLeft, Vector3.Down, color),
                new VertexPositionNormalColor(backLowerRight, Vector3.Down, color),

                // right
                new VertexPositionNormalColor(backUpperRight, Vector3.Right, color),
                new VertexPositionNormalColor(frontLowerRight, Vector3.Right, color),
                new VertexPositionNormalColor(backLowerRight, Vector3.Right, color),

                new VertexPositionNormalColor(frontLowerRight, Vector3.Right, color),
                new VertexPositionNormalColor(backUpperRight, Vector3.Right, color),
                new VertexPositionNormalColor(frontUpperRight, Vector3.Right, color),

                // top
                new VertexPositionNormalColor(frontUpperRight, Vector3.Up, color),
                new VertexPositionNormalColor(backUpperRight, Vector3.Up, color),
                new VertexPositionNormalColor(backUpperLeft, Vector3.Up, color),

                new VertexPositionNormalColor(frontUpperRight, Vector3.Up, color),
                new VertexPositionNormalColor(backUpperLeft, Vector3.Up, color),
                new VertexPositionNormalColor(frontUpperLeft, Vector3.Up, color),

                // left
                new VertexPositionNormalColor(frontUpperLeft, Vector3.Left, color),
                new VertexPositionNormalColor(backUpperLeft, Vector3.Left, color),
                new VertexPositionNormalColor(backLowerLeft, Vector3.Left, color),

                new VertexPositionNormalColor(frontLowerLeft, Vector3.Left, color),
                new VertexPositionNormalColor(frontUpperLeft, Vector3.Left, color),
                new VertexPositionNormalColor(backLowerLeft, Vector3.Left, color),

                // back
                new VertexPositionNormalColor(backUpperLeft, Vector3.Backward, color),
                new VertexPositionNormalColor(backUpperRight, Vector3.Backward, color),
                new VertexPositionNormalColor(backLowerLeft, Vector3.Backward, color),

                new VertexPositionNormalColor(backUpperRight, Vector3.Backward, color),
                new VertexPositionNormalColor(backLowerRight, Vector3.Backward, color),
                new VertexPositionNormalColor(backLowerLeft, Vector3.Backward, color)
            };
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public float Depth
        {
            get
            {
                return depth;
            }
            set
            {
                depth = value;
            }
        }
    }
}
