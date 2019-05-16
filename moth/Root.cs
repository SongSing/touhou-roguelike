using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace moth
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Root : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameScene gameScene;
        SpriteFont spriteFont;
        SmartFramerate smartFramerate;

        private static ContentManager GContent;
        private static Dictionary<string, object> resources = new Dictionary<string, object>();

        public static T LoadContent<T>(string name, bool invalidateCache = false) where T : class
        {
            return GContent.Load<T>(name) as T;
        }

        public static List<string> DebugInfo = new List<string>();

        public static class KeyDefs
        {
            public static Keys Shoot = Keys.Z;
            public static Keys Bomb = Keys.X;
            public static Keys Focus = Keys.LeftShift;
            public static Keys Up = Keys.Up;
            public static Keys Down = Keys.Down;
            public static Keys Right = Keys.Right;
            public static Keys Left = Keys.Left;
        }

        public static Random Random = new Random();

        public Root()
        {
            graphics = new GraphicsDeviceManager(this);

            this.gameScene = new GameScene();

            const int padding = 16;

            this.gameScene.Bounds.X = padding;
            this.gameScene.Bounds.Y = padding;

            this.graphics.PreferredBackBufferWidth = (this.gameScene.Bounds.Height + padding * 2) * 1920 / 1080;
            this.graphics.PreferredBackBufferHeight = this.gameScene.Bounds.Height + padding * 2;

            this.smartFramerate = new SmartFramerate(5);

            /*this.graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 60);
            this.graphics.SynchronizeWithVerticalRetrace = true;*/

            this.Content.RootDirectory = "Content";
            GContent = this.Content;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            AKS.WatchKey(Keys.RightAlt);
            AKS.WatchKey(Keys.Enter);
            AKS.WatchKey(Keys.R);

            AKS.WatchKey(KeyDefs.Up);
            AKS.WatchKey(KeyDefs.Down);
            AKS.WatchKey(KeyDefs.Left);
            AKS.WatchKey(KeyDefs.Right);
            AKS.WatchKey(KeyDefs.Shoot);
            AKS.WatchKey(KeyDefs.Bomb);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.gameScene.LoadContent();

            this.spriteFont = Content.Load<SpriteFont>("fonty");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.gameScene.UnloadContent();
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            AKS.Update();

            if (AKS.IsKeyDown(Keys.RightAlt) && AKS.WasJustPressed(Keys.Enter))
            {
                this.graphics.ToggleFullScreen();
            }

            if (AKS.WasJustPressed(Keys.R))
            {
                this.gameScene.Reset();
            }

            DebugInfo.Clear();
            double frameRate = this.smartFramerate.Framerate;
            if (double.IsInfinity(frameRate))
            {
                frameRate = 0;
            }

            DebugInfo.Add("FPS: " + Math.Round(frameRate).ToString());

            DebugInfo.Add("Memory: " + GC.GetTotalMemory(false) / 1024);

            this.gameScene.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.DeepSkyBlue);

            this.gameScene.Draw(this.spriteBatch);

            base.Draw(gameTime);

            smartFramerate.Update(gameTime.ElapsedGameTime.TotalSeconds);

            string debugString = "";

            foreach (string s in DebugInfo)
            {
                debugString += s + " \n";
            }

            this.spriteBatch.Begin();
            this.spriteBatch.DrawString(this.spriteFont, debugString, this.gameScene.Position + new Vector2(16), Color.White);
            this.spriteBatch.End();
        }

        public void Reset()
        {
            this.gameScene.Reset();
        }
    }
}
