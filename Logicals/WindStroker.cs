using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Mantra.XNA.Graphics;
using Mantra.Framework;

namespace LD12.Logicals
{
    class WindStroker : Behavior
    {
        const int MAX_ACTIVE_STROKES = 3;

        MouseState msLast;

        Vector3 previously;

        public WindStroke activeStroke;

        float distBetweenWind = 1;

        FMOD.Sound windGust1, windGust2, windGust3;

        Random r = new Random();

        public override void Initialize()
        {
            base.Initialize();

            GameContainer.SoundSystem.createSound("nectar\\sounds\\wind_med.ogg", FMOD.MODE.HARDWARE, ref windGust1);
            GameContainer.SoundSystem.createSound("nectar\\sounds\\wind_quiet.ogg", FMOD.MODE.HARDWARE, ref windGust2);
            GameContainer.SoundSystem.createSound("nectar\\sounds\\wind_quiet2.ogg", FMOD.MODE.HARDWARE, ref windGust3);

            windGust1.setMode(FMOD.MODE.DEFAULT);
            windGust2.setMode(FMOD.MODE.DEFAULT);
            windGust3.setMode(FMOD.MODE.DEFAULT);
        }

        public override void Update(TimeSpan elapsed)
        {
            MouseState ms = Mouse.GetState();

            if (activeStroke != null) {
                activeStroke.Update(elapsed);
            }
            if (ms.LeftButton == ButtonState.Pressed && msLast.LeftButton == ButtonState.Released) {
                activeStroke = new WindStroke();

                int n = r.Next(0, 3);

                switch (n) {
                    case 0: GameContainer.SoundSystem.playSound(FMOD.CHANNELINDEX.FREE, windGust1, false, ref GameContainer.SoundChannel); break;
                    case 1: GameContainer.SoundSystem.playSound(FMOD.CHANNELINDEX.FREE, windGust2, false, ref GameContainer.SoundChannel); break;
                    case 2: GameContainer.SoundSystem.playSound(FMOD.CHANNELINDEX.FREE, windGust3, false, ref GameContainer.SoundChannel); break;
                }

                GameContainer.SoundChannel.setVolume(1);
            } else if (ms.LeftButton == ButtonState.Pressed) {
                Vector3 now = MouseToWorld(ms, Cameras.Current.View, Cameras.Current.Projection, Matrix.CreateTranslation(0, 0, 0));

                if (Vector3.Distance(now, previously) > distBetweenWind) {
                    activeStroke.Move(now);

                    previously = now;
                }
            }

            msLast = ms;
        }

        public static Vector3 MouseToWorld(MouseState ms, Matrix view, Matrix projection, Matrix world)
        {
            Vector3 nearsource = new Vector3((float)ms.X, (float)ms.Y, 0f);
            Vector3 farsource = new Vector3((float)ms.X, (float)ms.Y, 1f);

            Vector3 nearPoint = GameContainer.Graphics.GraphicsDevice.Viewport.Unproject(nearsource, projection, view, world);
            Vector3 farPoint = GameContainer.Graphics.GraphicsDevice.Viewport.Unproject(farsource, projection, view, world);

            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);

            Ray pickRay = new Ray(nearPoint, direction);

            float? distance = pickRay.Intersects(new Plane(Vector3.Backward, -4)); // hack

            if (distance.HasValue) {
                return pickRay.Position + (pickRay.Direction * distance.Value);
            } else {
                return Vector3.Zero;
            }
        }
    }
}
