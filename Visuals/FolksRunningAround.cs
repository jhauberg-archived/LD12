using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Mantra.Framework;
using Mantra.XNA;
using Mantra.XNA.Graphics;

namespace LD12.Visuals
{
    class FolksRunningAround : Drawable
    {
        int population = 100;

        [Dependency(Group = "Death Counter")]
        DeathTollCounter tollCounter = null;

        [Dependency]
        Transform transform = null;

        Effect fx;
        EffectParameter fxWVP;

        VertexPointSpriteColor[] vertices;
        VertexDeclaration vertexDeclaration;

        Texture2D particle;

        Random r = new Random();

        float width = 35;
        float depth = 35;

        Vector3[] directions;
        float[] scale;

        float normalScale = 10;

        // should really put this, and the logic it involves into another component but gah.. CLOCK IS TICKING !!1
        int deathtoll = 0;
        DateTime from;
        float interval = 2; // amount of time that can pass before toll resets
        List<int> alreadyHit = new List<int>(); // list of indices (the index represents the vertex that was collided with)

        public FolksRunningAround() { }

        public FolksRunningAround(int population, int normalScale)
        {
            this.population = population;
            this.normalScale = normalScale;
        }

        public override void Initialize()
        {
            base.Initialize();

            particle = GameContainer.ContentManager.Load<Texture2D>("nectar\\texturing\\cloud");

            fx = GameContainer.ContentManager.Load<Effect>("nectar\\shading\\point_sprite_color");
            fx.CurrentTechnique = fx.Techniques["Main"];
            fx.Parameters["ColorMap"].SetValue(particle);

            fxWVP = fx.Parameters["WVP"];

            vertexDeclaration = new VertexDeclaration(GameContainer.Graphics.GraphicsDevice, VertexPointSpriteColor.VertexElements);
            vertices = new VertexPointSpriteColor[population];

            directions = new Vector3[population];

            scale = new float[population];

            int w = (int)width / 2;
            int d = (int)depth / 2;

            for (int i = 0; i < vertices.Length; i++) {
                Vector3 offset = new Vector3(
                    (float)r.Next(-w, w),
                    0,
                    (float)r.Next(-d, d));

                vertices[i].Position = transform.Position + offset;
                vertices[i].PointSize = normalScale;

                vertices[i].Color = Color.Black;

                scale[i] = normalScale;

                float x = (float)r.NextDouble();
                float z = (float)r.NextDouble();

                directions[i] = new Vector3(
                    r.Next(0, 2) > 0 ? x : -x,
                    0,
                    r.Next(0, 2) > 0 ? z : -z);
            }
        }

        public override void Update(TimeSpan elapsed)
        {
            if (deathtoll > 0) {
                TimeSpan since = DateTime.Now - from;

                if (since.TotalSeconds > interval) {
                    deathtoll = 0;
                    alreadyHit.Clear();
                }
            }

            var results = Repository.Behaviors.Where
                (
                    x =>
                        x.Group.IndexOf("Crate #") != -1 &&
                        x is Transform
                );

            float dotRadius = 0.5f;
            float boxRadius = 1.5f;

            for (int i = 0; i < population; i++) {
                bool noCollision = true;
                foreach (Transform boxTransform in results) {
                    float d = Vector3.Distance(vertices[i].Position, boxTransform.Position);

                    if (d <= dotRadius + boxRadius) {
                        vertices[i].Color = Color.Crimson;

                        scale[i] = 16;

                        noCollision = false;

                        if (!alreadyHit.Contains(i)) {
                            deathtoll++;

                            from = DateTime.Now;

                            alreadyHit.Add(i);

                            tollCounter.IncreaseDeathToll(vertices[i].Position, deathtoll);
                        }
                    }
                }

                if (noCollision) {
                    vertices[i].Color = Color.Black;
                    scale[i] = normalScale;
                }

                vertices[i].Position += directions[i] * (float)elapsed.TotalSeconds;
                vertices[i].PointSize = scale[i];

                if (vertices[i].Position.X > transform.Position.X + width / 2 ||
                    vertices[i].Position.X < transform.Position.X - width / 2) {
                    directions[i].X = -directions[i].X;
                }

                if (vertices[i].Position.Z > transform.Position.Z + depth / 2 ||
                    vertices[i].Position.Z < transform.Position.Z - depth / 2) {
                    directions[i].Z = -directions[i].Z;
                }
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

            fx.Begin();
            foreach (EffectPass pass in fx.CurrentTechnique.Passes) {
                pass.Begin();

                device.VertexDeclaration = vertexDeclaration;
                device.DrawUserPrimitives(PrimitiveType.PointList, vertices, 0, vertices.Length);

                pass.End();
            }
            fx.End();

            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.PointSpriteEnable = false;
            device.RenderState.DepthBufferWriteEnable = true;
            device.RenderState.SourceBlend = oldSourceBlend;
            device.RenderState.DestinationBlend = oldDestinationBlend;
        }
    }
}
