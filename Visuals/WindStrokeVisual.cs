using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mantra.Framework;
using Mantra.XNA.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Mantra.XNA;
using Microsoft.Xna.Framework;
using LD12.Logicals;

namespace LD12.Visuals
{
    class WindStrokerVisual : Drawable
    {
        [Dependency]
        WindStroker stroker = null;

        Effect fx;
        EffectParameter fxWVP;

        VertexPointSpriteColor[] vertices;
        VertexDeclaration vertexDeclaration;

        Texture2D particle;

        Random r = new Random();

        public override void Initialize()
        {
            base.Initialize();

            particle = GameContainer.ContentManager.Load<Texture2D>("nectar\\texturing\\cloud");

            fx = GameContainer.ContentManager.Load<Effect>("nectar\\shading\\point_sprite_color");
            fx.CurrentTechnique = fx.Techniques["Main"];
            fx.Parameters["ColorMap"].SetValue(particle);

            fxWVP = fx.Parameters["WVP"];

            vertexDeclaration = new VertexDeclaration(GameContainer.Graphics.GraphicsDevice, VertexPointSpriteColor.VertexElements);
            vertices = new VertexPointSpriteColor[0];
        }

        public override void Update(TimeSpan elapsed)
        {
            if (stroker.activeStroke != null) {
                List<WindSegment> segments = stroker.activeStroke.segments;

                List<VertexPointSpriteColor> tmpVertices = new List<VertexPointSpriteColor>();

                Color color = Color.WhiteSmoke;

                for (int i = 0; i < segments.Count; i++) {
                    tmpVertices.Add(new VertexPointSpriteColor(segments[i].Position, 96 * segments[i].Radius, color));

                    //tmpVertices.Add(new VertexPointSpriteColor(segments[i].Position + segments[i].Direction, 96 * segments[i].Radius, color));
                    //tmpVertices.Add(new VertexPointSpriteColor(segments[i].Position - segments[i].Direction, 64 * segments[i].Radius, color));

                    tmpVertices.Add(new VertexPointSpriteColor(segments[i].Position + segments[i].Direction * 0.5f /*+ new Vector3(0, (float)r.NextDouble() * 0.1f, 0)*/, 96 * segments[i].Radius, color));
                    tmpVertices.Add(new VertexPointSpriteColor(segments[i].Position - segments[i].Direction * 0.5f /*- new Vector3(0, (float)r.NextDouble() * 0.1f, 0)*/, 96 * segments[i].Radius, color));

                    tmpVertices.Add(new VertexPointSpriteColor(segments[i].Position + segments[i].Direction * 0.5f + new Vector3(0, 0, 0.25f), 96 * segments[i].Radius, color));
                    tmpVertices.Add(new VertexPointSpriteColor(segments[i].Position - segments[i].Direction * 0.5f - new Vector3(0, 0, 0.25f), 96 * segments[i].Radius, color));
                }

                vertices = tmpVertices.ToArray();
            }
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            Blend oldSourceBlend = device.RenderState.SourceBlend;
            Blend oldDestinationBlend = device.RenderState.DestinationBlend;

            device.RenderState.PointSpriteEnable = true;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            device.RenderState.DepthBufferWriteEnable = false;

            device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.AlphaFunction = CompareFunction.Greater;
            device.RenderState.ReferenceAlpha = 0;

            fxWVP.SetValue(Cameras.Current.View * Cameras.Current.Projection);

            if (vertices.Length > 0) {
                fx.Begin();
                foreach (EffectPass pass in fx.CurrentTechnique.Passes) {
                    pass.Begin();

                    device.VertexDeclaration = vertexDeclaration;
                    device.DrawUserPrimitives(PrimitiveType.PointList, vertices, 0, vertices.Length);

                    pass.End();
                }
                fx.End();
            }

            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.PointSpriteEnable = false;
            device.RenderState.DepthBufferWriteEnable = true;
            device.RenderState.SourceBlend = oldSourceBlend;
            device.RenderState.DestinationBlend = oldDestinationBlend;
        }
    }
}
