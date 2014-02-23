using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Mantra.XNA.Graphics;
using Mantra.Framework;

namespace LD12.Achievements
{
    // inspired by Geometry Wars 2 :)))
    class AchievementProgress : Drawable
    {
        [Dependency]
        DisplayAchievementGain gains = null;

        List<AchievementMetaData> achievements = new List<AchievementMetaData>();

        SpriteBatch sprite;
        SpriteFont font;

        Vector2 origin = new Vector2(38, 350);
        Vector2 current;

        public override void Initialize()
        {
            base.Initialize();

            sprite = new SpriteBatch(GameContainer.Graphics.GraphicsDevice);
            font = GameContainer.ContentManager.Load<SpriteFont>("nectar\\ui\\font_small");

            origin.Y = GameContainer.Graphics.GraphicsDevice.Viewport.Height - 30;
        }

        public override void Update(TimeSpan elapsed)
        {
            current = origin;

            for (int i = 0; i < achievements.Count; i++) {
                AchievementMetaData meta = achievements[i];

                meta.Update();

                if (meta.Progress >= 1) {
                    achievements.Remove(meta);
                    gains.Add(meta);
                }
            }
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            if (achievements.Count > 0) {
                sprite.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

                foreach (AchievementMetaData meta in achievements) {
                    if (meta.IsActive) {
                        sprite.Draw(meta.Icon, current - new Vector2((meta.Icon.Width / 2) + 5, 6), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);

                        string percent = String.Format("{0:0}%", meta.Progress * 100);

                        sprite.DrawString(font, percent, current + Vector2.One, Color.Black);
                        sprite.DrawString(font, percent, current, Color.White);

                        sprite.DrawString(font, meta.Title, current + new Vector2(60, 0) + Vector2.One, Color.Black);
                        sprite.DrawString(font, meta.Title, current + new Vector2(60, 0), Color.White);

                        current += new Vector2(0, -30);
                    }
                }

                sprite.End();
            }
        }

        public void Add(AchievementMetaData meta)
        {
            achievements.Add(meta);
        }
    }
}
