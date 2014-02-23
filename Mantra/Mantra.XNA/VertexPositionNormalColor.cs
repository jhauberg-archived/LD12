using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace Mantra.XNA
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalColor
    {
        public Vector3 Position;
        public Vector3 Normal;

        public Color Color;

        public static readonly VertexElement[] VertexElements;

        static VertexPositionNormalColor()
        {
            VertexElements = new VertexElement[] 
            {
                new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                new VertexElement(0, sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
                new VertexElement(0, sizeof(float) * 6, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0)
            };
        }

        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color color)
        {
            this.Position = position;
            this.Normal = normal;
            this.Color = color;
        }

        public static bool operator != (VertexPositionNormalColor left, VertexPositionNormalColor right)
        {
            return left.GetHashCode() != right.GetHashCode();
        }

        public static bool operator ==(VertexPositionNormalColor left, VertexPositionNormalColor right)
        {
            return left.GetHashCode() == right.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) {
                return false;
            }

            if (obj.GetType() != base.GetType()) {
                return false;
            }

            return (this == (VertexPositionNormalColor)obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() | Normal.GetHashCode() | Color.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{{Position:{0} Normal:{1} Color:{2}}}", Position, Normal, Color);
        }

        public static int SizeInBytes
        {
            get
            {
                return Marshal.SizeOf(typeof(VertexPositionNormalColor));
            }
        }
    }
}
