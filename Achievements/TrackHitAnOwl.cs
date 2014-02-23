using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mantra.Framework;
using Microsoft.Xna.Framework;
using Mantra.XNA;
using Microsoft.Xna.Framework.Graphics;

namespace LD12.Achievements
{
    class TrackHitAnOwl : Behavior
    {
        [Dependency]
        AchievementProgress progress = null;

        AchievementMetaData data = new AchievementMetaData();

        public override void Initialize()
        {
            base.Initialize();

            data.Title = "Crashin' Owls";
            data.Description = "You knocked down an owl!";

            data.Icon = GameContainer.ContentManager.Load<Texture2D>("nectar\\texturing\\icon_owl");

            progress.Add(data);
        }

        public override void Update(TimeSpan elapsed)
        {
            var boxResults = Repository.Behaviors.Where
                (
                    x => x is Transform && x.Group.IndexOf("Crate #") != -1
                );

            var owlResults = Repository.Behaviors.Where
                (
                    x => x is Transform && x.Group.IndexOf("Owl LOL #") != -1
                );

            Transform collidingOwl = null;

            bool collision = false;

            float combinedRadii = 3;

            foreach (Transform boxTransform in boxResults) {
                if (!collision) {
                    foreach (Transform owlTransform in owlResults) {
                        float d = Vector3.Distance(boxTransform.Position, owlTransform.Position);

                        if (d <= combinedRadii) {
                            collision = true;

                            collidingOwl = owlTransform;

                            break;
                        }
                    }
                }
            }

            if (collision) {
                Repository.Get<OwlMovement>(collidingOwl.Group).Enabled = false;
                Repository.Get<OwlMovementCrash>(collidingOwl.Group).Enabled = true;

                data.Progress = 1;
            }
        }
    }
}
