using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Mantra.XNA.Graphics;

namespace LD12.Visuals
{
    class DeathTollCounter : Drawable
    {
        SpriteBatch sprite;
        SpriteFont font;

        Dictionary<int, Vector3> crimescenes = new Dictionary<int, Vector3>();

        Dictionary<int, float> timeOnScreen = new Dictionary<int, float>();

        float maxTimeOnScreen = 1;

        public event EventHandler DeathOccured;

        void OnDeathOccured()
        {
            if (DeathOccured != null) {
                DeathOccured(this, EventArgs.Empty);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            sprite = new SpriteBatch(GameContainer.Graphics.GraphicsDevice);

            font = GameContainer.ContentManager.Load<SpriteFont>("nectar\\ui\\font_small");
        }

        public override void Update(TimeSpan elapsed)
        {
            for (int i = 0; i < crimescenes.Keys.Count; i++) {
                int key = crimescenes.Keys.ElementAt(i);

                crimescenes[key] += new Vector3(0, 1, 0) * (float)elapsed.TotalSeconds;

                timeOnScreen[key] += 1 * (float)elapsed.TotalSeconds;

                if (timeOnScreen[key] > maxTimeOnScreen) {
                    timeOnScreen.Remove(key);
                    crimescenes.Remove(key);
                }
            }
        }

        public override void Draw()
        {
            sprite.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            foreach (KeyValuePair<int, Vector3> crime in crimescenes) {
                string txt = String.Format("+{0}", crime.Key);

                Vector3 screenPos = GameContainer.Graphics.GraphicsDevice.Viewport.Project(crime.Value, Cameras.Current.Projection, Cameras.Current.View, Matrix.Identity);
                Vector2 pos = new Vector2(screenPos.X, screenPos.Y);

                sprite.DrawString(font, txt, pos + Vector2.One, Color.Black);
                sprite.DrawString(font, txt, pos, Color.White);
            }

            sprite.End();
        }

        public void IncreaseDeathToll(Vector3 at, int toll)
        {
            crimescenes.Add(toll, at);
            timeOnScreen.Add(toll, 0);

            OnDeathOccured();
        }
    }
}
