using System;
using System.Collections.Generic;

using Mantra.Framework;
using Mantra.XNA;
using Mantra.XNA.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Physics = BEPUphysics;

namespace LD12
{
    partial class GameContainer : Game
    {
        public static GraphicsDeviceManager Graphics;
        public static ContentManager ContentManager;
        
        public static Physics.Space Space;

        public static FMOD.System SoundSystem;
        public static FMOD.Channel SoundChannel;

        List<Mantra.Framework.IUpdateable> modules = new List<Mantra.Framework.IUpdateable>();

        Repository repo;

        Cameras cameras;
        Drawing drawing;
        Timing timing;

        public GameContainer()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferMultiSampling = false;
            Graphics.PreferredBackBufferWidth = 800;
            Graphics.PreferredBackBufferHeight = 600;

            Graphics.MinimumPixelShaderProfile = Microsoft.Xna.Framework.Graphics.ShaderProfile.PS_1_1;
            Graphics.MinimumVertexShaderProfile = Microsoft.Xna.Framework.Graphics.ShaderProfile.VS_1_1;

            ContentManager = Content;

            this.Window.AllowUserResizing = false;
            this.IsMouseVisible = true;

            this.Window.Title = "Pfff";
        }

        protected override void Initialize()
        {
            Space = new Physics.Space(new Physics.PersistentUniformGrid(10));
            Space.simulationSettings.gravity = new Vector3(0, -9.81f, 0);

            FMOD.Factory.System_Create(ref SoundSystem);
            SoundSystem.init(32, FMOD.INITFLAG.NORMAL, (IntPtr)null);

            InitializeModules();
            InitializeBehaviors();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            timing.Elapsed = gameTime.ElapsedGameTime;

            Space.update(gameTime);

            for (int i = 0; i < modules.Count; i++) {
                if (modules[i].Enabled) {
                    modules[i].Update();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            drawing.Update();

            base.Draw(gameTime);
        }
    }
}
