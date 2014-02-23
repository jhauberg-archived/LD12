using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mantra.Framework;
using LD12.Logicals;
using Microsoft.Xna.Framework.Graphics;

namespace LD12.Achievements
{
    class TrackAirTime : Behavior
    {
        [Dependency]
        AchievementProgress progress = null;

        AchievementMetaData data = new AchievementMetaData();

        int requiredStrokes = 2000;

        public override void Initialize()
        {
            base.Initialize();

            data.Title = "Juggler";
            data.Description = "Kept a crate in the air for quite a while!";

            data.Icon = GameContainer.ContentManager.Load<Texture2D>("nectar\\texturing\\icon_juggler");

            progress.Add(data);
        }

        public override void Update(TimeSpan elapsed)
        {
            var boxResults = Repository.Behaviors.Where
                (
                    x => x is ReceiveWindGust && x.Group.IndexOf("Crate #") != -1
                );

            int bestAmount = 0;

            foreach (ReceiveWindGust receiver in boxResults) {
                if (receiver.AmountOfUpwardsStrokes > bestAmount) {
                    bestAmount = receiver.AmountOfUpwardsStrokes;
                }
            }

            if (bestAmount < requiredStrokes) {
                data.Progress = (float)bestAmount / requiredStrokes;
            } else {
                data.Progress = 1; // eh
            }
        }
    }
}
