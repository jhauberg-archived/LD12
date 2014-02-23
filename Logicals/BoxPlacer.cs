using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mantra.Framework;
using Microsoft.Xna.Framework.Input;
using Mantra.XNA;
using Microsoft.Xna.Framework;
using Mantra.XNA.Graphics;
using Microsoft.Xna.Framework.Graphics;
using LD12.Drawables;

namespace LD12.Logicals
{
    class BoxPlacer : Behavior
    {
        MouseState msLast;

        const int MAX_CRATES = 5;

        int n = 4; // just start at 4..

        Transform currentBoxTransform;

        bool placing;

        public bool Placing { get { return placing; } }

        BoundingBox rooftopArea;

        // doesn't really belong in here.. meh, whatever
        public BoundingBox RooftopArea { get { return rooftopArea; } }

        public override void Initialize()
        {
            base.Initialize();

            Transform mainbuildingTransform = Repository.Get<Transform>(Groups.MainBuilding);
            Box mainbuilding = Repository.Get<Box>(Groups.MainBuilding);

            float w = mainbuilding.Width / 2;
            float h = mainbuilding.Height / 2;
            float d = mainbuilding.Depth / 2;

            Vector3 position = mainbuildingTransform.Position;

            position.Y += h * 2;

            rooftopArea = new BoundingBox(
                position + new Vector3(-w, -h, -d),
                position + new Vector3(w, h, d));
        }

        public override void Update(TimeSpan elapsed)
        {
            MouseState ms = Mouse.GetState();

            if (ms.RightButton == ButtonState.Released && msLast.RightButton == ButtonState.Pressed) {
                // place box, green transparent box

                if (placing) {
                    placing = false;

                    string group = String.Format("Crate #{0}", n++);

                    Repository.Delegater.Bind(group, new BoxCollider(false));
                    Repository.Delegater.Bind(group, new ReceiveWindGust());

                    currentBoxTransform = null;
                }
            }

            if (ms.RightButton == ButtonState.Pressed && msLast.RightButton == ButtonState.Released) {
                Vector3 position = WindStroker.MouseToWorld(ms, Cameras.Current.View, Cameras.Current.Projection, Matrix.CreateTranslation(0, 0, 0));

                if (rooftopArea.Contains(position) == ContainmentType.Contains || rooftopArea.Contains(position) == ContainmentType.Intersects) {
                    var results = Repository.Behaviors.Where
                        (
                            x => x is BoxCollider && x.Group.IndexOf("Crate #") != -1
                        );

                    if (results.Count() < MAX_CRATES) {
                        // start box placing
                        placing = true;

                        Vector3 initialPosition = Repository.Get<Transform>(Groups.MainBuildingRoof).Position;

                        float size = 1.75f;

                        string group = String.Format("Crate #{0}", n);

                        Repository.Delegater.Bind(group, new Transform()
                        {
                            Position = position
                        });
                        Repository.Delegater.Bind(group, new Box(size, size, size)
                        {
                            Color = Color.Beige
                        });

                        Repository.Delegater.Bind(group, new ScaleToDistanceFromFloor());

                        currentBoxTransform = Repository.Get<Transform>(group);
                    }
                }
            }

            if (currentBoxTransform != null) {
                string group = String.Format("Crate #{0}", n);
                Vector3 mouseWorld = WindStroker.MouseToWorld(ms, Cameras.Current.View, Cameras.Current.Projection, Matrix.CreateTranslation(0, 0, 0));
                currentBoxTransform.Position = WindStroker.MouseToWorld(ms, Cameras.Current.View, Cameras.Current.Projection, Matrix.CreateTranslation(0, 0, 0));
            }

            msLast = ms;
        }
    }
}
