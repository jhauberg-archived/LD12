using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mantra.XNA
{
    public struct VertexPointSprite
    {
        public Vector3 Position;
        public float PointSize;

        public VertexPointSprite(Vector3 position, float pointSize)
        {
            this.Position = position;
            this.PointSize = pointSize;
        }

        public static int SizeInBytes
        {
            get
            {
                return sizeof(float) * 4;
            }
        }

        public static VertexElement[] VertexElements =
            {
                new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                new VertexElement(0, sizeof(float) * 3, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.PointSize, 0),
            };
    }

    public struct VertexPointSpriteColor
    {
        public Vector3 Position;
        public float PointSize;
        public Color Color;

        public VertexPointSpriteColor(Vector3 position, float pointSize, Color color)
        {
            this.Position = position;
            this.PointSize = pointSize;
            this.Color = color;
        }

        public static int SizeInBytes
        {
            get
            {
                return sizeof(float) * 5;
            }
        }

        public static VertexElement[] VertexElements =
            {
                new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                new VertexElement(0, sizeof(float) * 3, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.PointSize, 0),
                new VertexElement(0, sizeof(float) * 4, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0)
            };
    }
}
