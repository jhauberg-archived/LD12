using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mantra.Framework;
using LD12.Visuals;
using Microsoft.Xna.Framework.Graphics;

namespace LD12.Achievements
{
    class TrackTotalKillInOneSession : Behavior
    {
        [Dependency]
        AchievementProgress progress = null;

        [Dependency(Group = "Death Counter")]
        DeathTollCounter deaths = null;

        AchievementMetaData data = new AchievementMetaData();

        int totalKills;

        int killsToGet = 50;

        public override void Initialize()
        {
            base.Initialize();

            data.Title = "Mash";
            data.Description = "Mashed 50 people in one session!";

            data.Icon = GameContainer.ContentManager.Load<Texture2D>("nectar\\texturing\\icon_mash");

            progress.Add(data);

            deaths.DeathOccured += new EventHandler(deaths_DeathOccured);
        }

        void deaths_DeathOccured(object sender, EventArgs e)
        {
            if (totalKills < killsToGet) {
                totalKills++;
            }

            data.Progress = (float)totalKills / killsToGet;
        }
    }

    class TrackTotalKillInOneSessionSequel : Behavior
    {
        [Dependency]
        AchievementProgress progress = null;

        [Dependency(Group = "Death Counter")]
        DeathTollCounter deaths = null;

        AchievementMetaData data = new AchievementMetaData();

        int totalKills;

        int killsToGet = 100;

        public override void Initialize()
        {
            base.Initialize();

            data.Title = "Mash Extreme";
            data.Description = "Mashed 100 people in one session!";

            data.Icon = GameContainer.ContentManager.Load<Texture2D>("nectar\\texturing\\icon_mash_2");

            progress.Add(data);

            deaths.DeathOccured += new EventHandler(deaths_DeathOccured);
        }

        void deaths_DeathOccured(object sender, EventArgs e)
        {
            if (totalKills < killsToGet) {
                totalKills++;
            }

            data.Progress = (float)totalKills / killsToGet;
        }
    }
}
