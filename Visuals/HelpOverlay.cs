using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mantra.XNA.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LD12.Visuals
{
    class HelpOverlay : Drawable
    {
        SpriteBatch sprite;
        SpriteFont font;

        const string PressF1 = "[F1] for help";
        const string RMB = "[RMB] for new crate";
        const string Reset = "[R] for reset";

        public override void Initialize()
        {
            base.Initialize();

            sprite = new SpriteBatch(GameContainer.Graphics.GraphicsDevice);

            font = GameContainer.ContentManager.Load<SpriteFont>("nectar\\ui\\font_small");
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            sprite.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            sprite.DrawString(font, PressF1, new Vector2(10, 10) + Vector2.One, Color.Black);
            sprite.DrawString(font, PressF1, new Vector2(10, 10), Color.White);

            sprite.DrawString(font, RMB, new Vector2(10, 40) + Vector2.One, Color.Black);
            sprite.DrawString(font, RMB, new Vector2(10, 40), Color.White);

            sprite.DrawString(font, Reset, new Vector2(10, 70) + Vector2.One, Color.Black);
            sprite.DrawString(font, Reset, new Vector2(10, 70), Color.White);
            
            sprite.End();
        }
    }
}
