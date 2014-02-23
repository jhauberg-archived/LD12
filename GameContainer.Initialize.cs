using System;
using System.Linq;
using System.Collections.Generic;
using Mantra.Framework;
using Mantra.Framework.Extensions;
using Mantra.XNA;
using Mantra.XNA.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LD12.Drawables;

using Physics = BEPUphysics;
using LD12.Visuals;
using LD12.Logicals;
using LD12.Achievements;
//using Microsoft.Xna.Framework.Audio;

namespace LD12
{
    // todo: name debug rendering classes like:
    // DebugWindStroke
    // DebugWindDirections ... whatever

    /*
    class TestMousePick : Behavior
    {
        public override void Update(TimeSpan elapsed)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
                Repository.Get<Transform>("Crate #1").Position = 
                    MouseToWorld(Mouse.GetState(), Cameras.Current.View, Cameras.Current.Projection, Matrix.CreateTranslation(0, 0, 0));
            }
        }

        Vector3 MouseToWorld(MouseState ms, Matrix view, Matrix projection, Matrix world)
        {
            Vector3 nearsource = new Vector3((float)ms.X, (float)ms.Y, 0f);
            Vector3 farsource = new Vector3((float)ms.X, (float)ms.Y, 1f);

            Vector3 nearPoint = GameContainer.Graphics.GraphicsDevice.Viewport.Unproject(nearsource, projection, view, world);
            Vector3 farPoint = GameContainer.Graphics.GraphicsDevice.Viewport.Unproject(farsource, projection, view, world);

            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);

            Ray pickRay = new Ray(nearPoint, direction);

            float? distance = pickRay.Intersects(new Plane(Vector3.Backward, -4));

            if (distance.HasValue) {
                return pickRay.Position + (pickRay.Direction * distance.Value);
            } else {
                return Vector3.Zero;
            }
        }
    }*/

    class WindSegment
    {
        public float Radius;

        public Vector3 Position;
        public Vector3 Direction;

        // debug stuff
        public VertexPositionColor[] Vertices;
        public VertexPositionColor[] DirectionVertices;

        // hackz with the z
        public void CreateVertices(float z)
        {
            int segments = 32;
            float angleStep = 2.0f * (float)Math.PI / (float)segments;

            float r = Radius;
            float x = Position.X;
            float y = Position.Y;

            Vertices = new VertexPositionColor[segments + 1];

            for (int i = 0; i < segments; i++) {
                float angle = i * angleStep;

                float bx = x + r * (float)Math.Cos(angle);
                float by = y + r * (float)Math.Sin(angle);

                Vertices[i].Position = new Vector3(bx, by, z);
                Vertices[i].Color = Color.Orange;
            }

            Vertices[segments].Position = Vertices[0].Position;
            Vertices[segments].Color = Color.Orange;

            DirectionVertices = new VertexPositionColor[2]
            {
                new VertexPositionColor(Position, Color.LightGreen),
                new VertexPositionColor(Position + (Direction * 2), Color.Green)
            };
        }

        public void Colorize(Color color)
        {
            for (int i = 0; i < Vertices.Length; i++) {
                Vertices[i].Color = color;
            }
        }
    }

    class WindStroke
    {
        public List<Vector3> controlPoints = new List<Vector3>();
        public List<WindSegment> segments = new List<WindSegment>();

        float decayTimer, decayingTimer;
        float decaySpeed = 0.2f;
        float decayInterval = 0.1f;

        int maxLength = 12;

        bool decaying;

        public void Move(Vector3 to)
        {
            if (controlPoints.Count > maxLength) {
                return;
            }

            controlPoints.Add(to);

            if (controlPoints.Count > 1) {
                WindSegment segment = new WindSegment();

                Vector3 a = controlPoints[controlPoints.Count - 2];
                Vector3 b = controlPoints[controlPoints.Count - 1]; // or "to".. :)

                segment.Position = (a + b) / 2;
                segment.Radius = Vector3.Distance(a, b) / 2;
                segment.Direction = Vector3.Normalize(b - a);

                // debug
                segment.CreateVertices(b.Z);

                segments.Add(segment);
            }
        }

        public void Update(TimeSpan elapsed)
        {
            if (controlPoints.Count > 0) {
                if (decaying) {
                    decayingTimer += 1 * (float)elapsed.TotalSeconds;

                    if (decayingTimer > decayInterval) {
                        if (segments.Count > 0) {
                            segments.RemoveAt(0);
                        }

                        controlPoints.RemoveAt(0);

                        decayingTimer = 0;

                        if (controlPoints.Count == 0) {
                            decaying = false;
                        }
                    }
                }

                decayTimer += 1 * (float)elapsed.TotalSeconds;

                if (decayTimer > decaySpeed) {
                    decaying = true;

                    decayTimer = 0;
                }
            }
        }
    }

    class DebugWindStroke : Drawable
    {
        [Dependency]
        WindStroker stroker = null;

        BasicEffect fx;

        VertexDeclaration vertexDecl;

        VertexPositionColor[] vertices;

        public override void Initialize()
        {
            base.Initialize();

            fx = new BasicEffect(GameContainer.Graphics.GraphicsDevice, null);

            fx.LightingEnabled = false;
            fx.VertexColorEnabled = true;

            vertexDecl = new VertexDeclaration(GameContainer.Graphics.GraphicsDevice, VertexPositionColor.VertexElements);

            vertices = new VertexPositionColor[0];
        }

        public override void Update(TimeSpan elapsed)
        {
            if (stroker.activeStroke != null) {
                vertices = new VertexPositionColor[stroker.activeStroke.controlPoints.Count];

                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i].Position = stroker.activeStroke.controlPoints[i];
                    vertices[i].Color = Color.Red;
                }
            }
        }

        public override void Draw()
        {
            if (vertices.Length > 1) {
                fx.View = Cameras.Current.View;
                fx.Projection = Cameras.Current.Projection;

                fx.Begin();
                foreach (EffectPass pass in fx.CurrentTechnique.Passes) {
                    pass.Begin();

                    GameContainer.Graphics.GraphicsDevice.VertexDeclaration = vertexDecl;

                    //GameContainer.Graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);

                    foreach (WindSegment segment in stroker.activeStroke.segments) {
                        GameContainer.Graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, segment.Vertices, 0, segment.Vertices.Length - 1);
                        GameContainer.Graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, segment.DirectionVertices, 0, 1);
                    }

                    pass.End();
                }
                fx.End();
            }
        }
    }

    class ScaleToDistanceFromFloor : Behavior
    {
        [Dependency]
        Transform transform = null;

        public override void Update(TimeSpan elapsed)
        {
            float scale = transform.Position.Y * 0.5f;

            if (scale > 1)
                scale = 1;

            transform.Scale = new Vector3(scale);
        }
    }

    class OwlMovement : Behavior
    {
        [Dependency]
        Transform transform = null;

        int width, depth, height;

        Vector3 nextTarget;

        float speed = 10;

        Random r = new Random();

        Vector3 origin;

        public OwlMovement(int width, int height, int depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public override void Initialize()
        {
            base.Initialize();

            origin = transform.Position;

            Next();
        }

        public override void Update(TimeSpan elapsed)
        {
            if (Vector3.Distance(nextTarget, transform.Position) < 1) {
                Next();
            }

            Vector3 direction = Vector3.Normalize(nextTarget - transform.Position);

            Matrix rotation = Matrix.Identity;

            rotation.Forward = direction;
            rotation.Up = Vector3.Up;
            rotation.Left = Vector3.Cross(Vector3.Up, direction);

            transform.Rotation = Quaternion.CreateFromRotationMatrix(rotation);

            transform.Position += (direction * speed) * (float)elapsed.TotalSeconds;
        }

        void Next()
        {
            nextTarget = origin + new Vector3(
                (float)r.Next(-width, width),
                (float)r.Next(25, height),
                (float)r.Next(-depth, depth));
        }
    }

    class OwlCreature : Drawable
    {
        [Dependency]
        Transform transform = null;

        VertexDeclaration vertexDeclaration;
        VertexPositionNormalColor[] vertices;

        BasicEffect effect;

        Color color = Color.Beige;

        float alpha = 0.65f;

        float a;

        public override void Initialize()
        {
            base.Initialize();

            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            effect = new BasicEffect(device, null);
            effect.EnableDefaultLighting();
            effect.VertexColorEnabled = true;

            vertexDeclaration = new VertexDeclaration(device, VertexPositionNormalColor.VertexElements);

            vertices = new VertexPositionNormalColor[]
            {
                new VertexPositionNormalColor(new Vector3(0, 0, 0), Vector3.Up, color),
                new VertexPositionNormalColor(new Vector3(-2, 0, -2.5f), Vector3.Up, color),
                new VertexPositionNormalColor(new Vector3(-2, 0, 2.5f), Vector3.Up, color),

                new VertexPositionNormalColor(new Vector3(0, 0, 0), Vector3.Up, color),
                new VertexPositionNormalColor(new Vector3(2, 0, -2.5f), Vector3.Up, color),
                new VertexPositionNormalColor(new Vector3(2, 0, 2.5f), Vector3.Up, color),
            };
        }

        public override void Update(TimeSpan elapsed)
        {
            a += 6 * (float)elapsed.TotalSeconds;

            float y = (float)Math.Sin(a) * 2;

            vertices[1].Position.Y = y;
            vertices[2].Position.Y = y;

            vertices[4].Position.Y = y;
            vertices[5].Position.Y = y;
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            CullMode previousCullMode = device.RenderState.CullMode;

            device.RenderState.CullMode = CullMode.None;

            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            effect.View = Cameras.Current.View;
            effect.Projection = Cameras.Current.Projection;
            effect.World = transform.World;

            effect.DiffuseColor = color.ToVector3();

            effect.Alpha = alpha;

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
    }

    // lol ffs, silly trick to avoid the spawning of 2 identical owl paths
    class OwlDispenser : Behavior
    {
        DateTime from;

        float interval = 5;

        int maxAmount = 2;
        int dispensed = 0;

        public override void Update(TimeSpan elapsed)
        {
            if (dispensed < maxAmount) {
                TimeSpan since = DateTime.Now - from;

                if (since.TotalSeconds > interval) {
                    from = DateTime.Now;

                    string group = String.Format("Owl LOL #{0}", ++dispensed);

                    Repository.Delegater.Bind(group, new Transform() { Position = new Vector3(-50, 0, 0) });
                    Repository.Delegater.Bind(group, new OwlCreature());
                    Repository.Delegater.Bind(group, new OwlMovement(25, 50, 50));
                    Repository.Delegater.Bind(group, new OwlMovementCrash());
                }
            }
        }
    }

    class OwlMovementCrash : Behavior
    {
        [Dependency]
        Transform transform = null;

        [Dependency]
        OwlMovement regularMovement = null;

        public override void Update(TimeSpan elapsed)
        {
            Matrix rotation = Matrix.CreateFromQuaternion(transform.Rotation);

            transform.Position += ((Vector3.Down * 20) + (rotation.Forward * 1.5f)) * (float)elapsed.TotalSeconds;

            if (transform.Position.Y < -25) {
                Enabled = false;
                regularMovement.Enabled = true;
            }
        }
    }

    class DisplayAchievementGain : Drawable
    {
        List<AchievementMetaData> wins = new List<AchievementMetaData>();

        SpriteBatch sprite;
        SpriteFont font;

        DateTime from;

        float displayTime = 3.5f;

        AchievementMetaData currentDisplay;

        float currentScale;

        bool movingToCorner, up;

        Vector2 tmpPosition;

        Vector2 origin;
        Vector2 current;

        public override void Initialize()
        {
            base.Initialize();

            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            sprite = new SpriteBatch(device);

            font = GameContainer.ContentManager.Load<SpriteFont>("nectar\\ui\\font");

            origin = new Vector2(device.Viewport.Width - 8, 5);
        }

        public override void Update(TimeSpan elapsed)
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            current = origin;

            if (currentDisplay != null) {
                if (!movingToCorner) {
                    if (up) {
                        currentScale += 2.1f * (float)elapsed.TotalSeconds;
                    } else {
                        currentScale -= 1.75f * (float)elapsed.TotalSeconds;
                    }

                    if (currentScale > 1.25f) {
                        up = false;
                    }

                    if (currentScale < 0.75f) {
                        up = true;
                    }

                    tmpPosition = new Vector2(
                        (device.Viewport.Width / 2) - ((currentDisplay.Icon.Width * currentScale) / 2),
                        (device.Viewport.Height / 2) - ((currentDisplay.Icon.Height * currentScale) / 2));
                } else {
                    tmpPosition += Vector2.Normalize(new Vector2(1, 0.75f)) * (float)elapsed.TotalSeconds;

                    currentScale -= 0.75f * (float)elapsed.TotalSeconds;

                    if (currentScale < 0.25f) {
                        currentDisplay = null;
                    }
                }

                TimeSpan since = DateTime.Now - from;

                if (since.TotalSeconds > displayTime) {
                    movingToCorner = true;
                }
            }
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            sprite.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            if (currentDisplay != null) {
                sprite.Draw(currentDisplay.Icon, tmpPosition, null, Color.White, 0, Vector2.Zero, currentScale, SpriteEffects.None, 0);

                string txt = String.Format("Achieved: {0}!", currentDisplay.Title);

                Vector2 txtSize = font.MeasureString(txt);
                Vector2 descriptionSize = font.MeasureString(currentDisplay.Description);

                Vector2 txtPos = new Vector2(
                    (device.Viewport.Width / 2) - (txtSize.X / 2), 
                    (device.Viewport.Height / 2) - (txtSize.Y / 2) + 64);

                Vector2 descriptionPos = new Vector2(
                    (device.Viewport.Width / 2) - (descriptionSize.X / 2),
                    (device.Viewport.Height / 2) - (descriptionSize.Y / 2) + 128);

                sprite.DrawString(font, txt, txtPos + Vector2.One, Color.Black);
                sprite.DrawString(font, txt, txtPos, Color.White);

                sprite.DrawString(font, currentDisplay.Description, descriptionPos + Vector2.One, Color.Black);
                sprite.DrawString(font, currentDisplay.Description, descriptionPos, Color.White);
            }

            foreach (AchievementMetaData meta in wins) {
                if (meta != currentDisplay) {
                    sprite.Draw(meta.Icon, current - new Vector2((meta.Icon.Width), 0), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                    current -= new Vector2(meta.Icon.Width - 8, 0);
                }
            }

            sprite.End();
        }

        public void Add(AchievementMetaData meta)
        {
            if (!wins.Contains(meta)) {
                wins.Add(meta);

                currentDisplay = meta;
                currentScale = 1;

                up = true;

                movingToCorner = false;

                from = DateTime.Now;
            }
        }
    }

    partial class GameContainer
    {
        void InitializeModules()
        {
            repo = new Repository();

            if (modules.Count > 0) {
                modules.Clear();
            }

            drawing = new Drawing();
            drawing.Enabled = false; // manual updating (so it only happens when XNA wants to draw)
            drawing.Subscribe(repo.Delegater);

            timing = new Timing();
            timing.Subscribe(repo.Delegater);

            cameras = new Cameras();
            cameras.Subscribe(repo.Delegater);

            modules.Add(drawing);
            modules.Add(timing);
            modules.Add(cameras);
        }

        void InitializeBehaviors()
        {
            Delegater d = repo.Delegater;

            #region Camera 1
            
            d.Bind(
                Groups.DefaultCamera,
                new Transform() 
                { 
                    Position = new Vector3(0, 35, 28) 
                });

            d.Bind(
                Groups.DefaultCamera,
                new ActionContextCamera(GraphicsDevice)
                {
                    ClearColor = Color.SteelBlue//Color.Beige
                });

            #endregion

            d.Bind("Floor", new Transform()
            {
                Position = new Vector3(-25, -1, 0)
            });
            d.Bind("Floor", new Box(250, 1, 250)
            {
                Color = Color.LightGray
            });
            d.Bind("Floor", new BoxCollider(true));

            d.Bind(Groups.MainBuilding, new Transform());
            d.Bind(Groups.MainBuilding, new Box(20, 30, 15));
            
            repo.Get<Transform>(Groups.MainBuilding).Position.Y = repo.Get<Box>(Groups.MainBuilding).Height / 2;

            d.Bind(Groups.MainBuilding, new BoxCollider(true));

            Vector3 offset = new Vector3(-(repo.Get<Box>(Groups.MainBuilding).Width / 5), repo.Get<Box>(Groups.MainBuilding).Height / 2, 0);

            d.Bind(Groups.MainBuildingRoof, new Transform()
            {
                Position = repo.Get<Transform>(Groups.MainBuilding).Position + offset
            });

            d.Bind(Groups.Wind, new WindStroker());
            //d.Bind(Groups.Wind, new DebugWindStroke());
            d.Bind(Groups.Wind, new WindStrokerVisual() { DrawOrder = 2 });

            // crates
            d.Bind("box placer", new BoxPlacer());
            
            Vector3 initialPosition = repo.Get<Transform>(Groups.MainBuildingRoof).Position;

            float size = 1.75f;

            d.Bind("Crate #1", new Transform()
            {
                Position = initialPosition + new Vector3(size / 2) + new Vector3(-3, 0, 4)
            });
            d.Bind("Crate #1", new Box(size, size, size)
            {
                Color = Color.Beige
            });

            d.Bind("Crate #1", new BoxCollider(false));
            d.Bind("Crate #1", new ReceiveWindGust());
            d.Bind("Crate #1", new ScaleToDistanceFromFloor());

            //////////

            d.Bind("Crate #2", new Transform()
            {
                Position = initialPosition + new Vector3(size / 2) + new Vector3(-1, 0, 4)
            });
            d.Bind("Crate #2", new Box(size, size, size)
            {
                Color = Color.Beige
            });

            d.Bind("Crate #2", new BoxCollider(false));
            d.Bind("Crate #2", new ReceiveWindGust());
            d.Bind("Crate #2", new ScaleToDistanceFromFloor());

            //////////

            d.Bind("Crate #3", new Transform()
            {
                Position = initialPosition + new Vector3(size / 2) + new Vector3(-2.25f, 1.8f, 4)
            });
            d.Bind("Crate #3", new Box(size, size, size)
            {
                Color = Color.Beige
            });

            d.Bind("Crate #3", new BoxCollider(false));
            d.Bind("Crate #3", new ReceiveWindGust());
            d.Bind("Crate #3", new ScaleToDistanceFromFloor());

            d.Bind("Death Counter", new DeathTollCounter() { DrawOrder = 4 });

            d.Bind("people", new Transform() 
            { 
                Position = new Vector3(-35, 0.25f, 0) 
            });
            d.Bind("people", new FolksRunningAround() { DrawOrder = 2 });

            d.Bind("grassy field", new Transform()
            {
                Position = new Vector3(-35, -0.75f, 0)
            });
            d.Bind("grassy field", new Box(37, 1, 37)
            {
                Color = Color.Green
            });

            d.Bind("Another tower", new Transform()
            {
                Position = new Vector3(-100, 20, -10)
            });
            d.Bind("Another tower", new Box(35, 50, 30)
            {
                Color = Color.Bisque
            });
            d.Bind("Another tower", new BoxCollider(true));

            d.Bind("Another tower 2", new Transform()
            {
                Position = new Vector3(-100, 10, -45)
            });
            d.Bind("Another tower 2", new Box(35, 30, 30)
            {
                Color = Color.BlanchedAlmond
            });
            d.Bind("Another tower 2", new BoxCollider(true));

            d.Bind("road", new Transform()
            {
                Position = new Vector3(-65, -0.75f, 0)
            });
            d.Bind("road", new Box(20, 1, 250)
            {
                Color = Color.DarkGray
            });

            d.Bind("road separator", new Transform()
            {
                Position = new Vector3(-65, -0.5f, 0)
            });
            d.Bind("road separator", new Box(1, 1, 250)
            {
                Color = Color.DarkGray
            });

            d.Bind("another grassy field", new Transform()
            {
                Position = new Vector3(-98, -0.75f, 35)
            });
            d.Bind("another grassy field", new Box(37, 1, 37)
            {
                Color = Color.Green
            });

            d.Bind("people 2", new Transform()
            {
                Position = new Vector3(-98, 0.25f, 35)
            });
            d.Bind("people 2", new FolksRunningAround(50, 6) { DrawOrder = 2 }); // hacky with hardcoding scale, but cba with making it based on cam distance or whatever

            d.Bind("small path", new Transform()
            {
                Position = new Vector3(-115, -0.75f, 10)
            });
            d.Bind("small path", new Box(80, 1, 5)
            {
                Color = Color.DarkGray
            });


            d.Bind("deactivator", new RemoveDeactivatedBoxes());

            d.Bind("How to play", new HowToOverlay() { DrawOrder = 3 });
            d.Bind("How to play", new HelpOverlay() { DrawOrder = 3 });

            d.Bind("Owl Dispenser :D", new OwlDispenser());

            repo.Get<ActionContextCamera>(Groups.DefaultCamera).Target =
                //repo.Get<Transform>("Owl LOL");
                repo.Get<Transform>(Groups.MainBuildingRoof);


            ///// achievements woo!
            d.Bind("Achievement Progress", new DisplayAchievementGain() { DrawOrder = 3 });
            d.Bind("Achievement Progress", new AchievementProgress() { DrawOrder = 3 });

            // trackers!
            d.Bind("Achievement Progress", new TrackTotalKillInOneSession());
            d.Bind("Achievement Progress", new TrackTotalKillInOneSessionSequel());
            d.Bind("Achievement Progress", new TrackHitAnOwl());
            d.Bind("Achievement Progress", new TrackAirTime());
        }
    }
}
