using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mantra.Framework;
using Mantra.XNA.Graphics;
using Mantra.XNA;
using Mantra.Framework.Extensions;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace LD12.Logicals
{
    class RemoveDeactivatedBoxes : Behavior
    {
        KeyboardState ksLast;

        BoundingBox rooftop;

        public override void Initialize()
        {
            base.Initialize();

            rooftop = Repository.Get<BoxPlacer>("box placer").RooftopArea;
        }

        public override void Update(TimeSpan elapsed)
        {
            KeyboardState ks = Keyboard.GetState();

            var results = Repository.Behaviors.Where
                (
                    x => x is BoxCollider && x.Group.IndexOf("Crate #") != -1
                );

            for (int i = 0; i < results.Count(); i++) {
                BoxCollider result = results.ElementAt(i) as BoxCollider;

                bool onRoof = 
                    rooftop.Contains(result.Geom.centerPosition) == ContainmentType.Intersects || 
                    rooftop.Contains(result.Geom.centerPosition) == ContainmentType.Contains;

                if (!onRoof &&
                    result.Geom.isPhysicallySimulated &&
                    result.Geom.isAffectedByGravity &&
                    !result.Geom.isActive) {

                    Repository.Delegater.UnbindAll(result.Group);

                    GameContainer.Space.remove(result.Geom);
                }
            }

            if (ks.IsKeyUp(Keys.R) && ksLast.IsKeyDown(Keys.R)) {
                // reset
                for (int i = 0; i < results.Count(); i++) {
                    BoxCollider result = results.ElementAt(i) as BoxCollider;

                    bool onRoof =
                        rooftop.Contains(result.Geom.centerPosition) == ContainmentType.Intersects ||
                        rooftop.Contains(result.Geom.centerPosition) == ContainmentType.Contains;

                    if (!onRoof) {
                        result.Geom.deactivate();
                    }
                }
            }

            ksLast = ks;
        }
    }
}
