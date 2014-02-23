using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mantra.XNA.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD12.Visuals
{
    class HowToOverlay : Drawable
    {
        const string HoldDrag = "HOLD [LMB] and DRAG";

        SpriteBatch sprite;
        SpriteFont font;

        Texture2D help;

        float alpha;
        float speed = 150;

        bool initialShowDone;

        public override void Initialize()
        {
            base.Initialize();

            sprite = new SpriteBatch(GameContainer.Graphics.GraphicsDevice);

            font = GameContainer.ContentManager.Load<SpriteFont>("nectar\\ui\\font");

            help = GameContainer.ContentManager.Load<Texture2D>("nectar\\texturing\\how_to_play");
        }

        public override void Update(TimeSpan elapsed)
        {
            if (alpha < 255) {
                alpha += speed * (float)elapsed.TotalSeconds;

                if (alpha > 255) {
                    alpha = 255;

                    if (!initialShowDone) {
                        initialShowDone = true;
                    }
                }
            }

            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            if (Visible && initialShowDone) {
                if (ms.LeftButton == ButtonState.Pressed || ms.MiddleButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed) {
                    Visible = false;
                }
            } else {
                if (ks.IsKeyDown(Keys.F1)) {
                    alpha = 0;
                    Visible = true;
                }
            }
        }

        public override void Draw()
        {
            GraphicsDevice device = GameContainer.Graphics.GraphicsDevice;

            Color color = new Color(
                255, 255, 255, (byte)alpha);

            sprite.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.SaveState);

            Vector2 textSize = font.MeasureString(HoldDrag);

            sprite.DrawString(font, HoldDrag, new Vector2((device.Viewport.Width / 2) - (textSize.X / 2), (device.Viewport.Height / 2) + 210), color);
            sprite.Draw(help, new Vector2((device.Viewport.Width / 2) - (help.Width / 2), (device.Viewport.Height / 2) - (help.Height / 2)), color);

            sprite.End();
        }
    }
}
