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
    class GameScene : Scene
    {
        public static Point GameArea => new Point(384 * 2, 448 * 2);

        private Texture2D background;
        private RasterizerState rasterizerState = new RasterizerState() { ScissorTestEnable = true };

        public readonly Player player;

        public readonly Enemy testEnemy;

        public GameScene()
        {
            this.Bounds = new Rectangle(16, 0, GameArea.X, GameArea.Y);
            this.player = new Player(this);

            this.testEnemy = new Enemy(this);
        }

        public void Reset()
        {
            this.LoadContent();

            this.player.Reset();

            this.testEnemy.Reset();
        }

        protected override void InternalUpdate(GameTime gameTime)
        {
            this.player.Update(gameTime);
            
            this.testEnemy.Update(gameTime);
           
            if (this.testEnemy.IsDamaging(this.player))
            {
                Console.WriteLine("ded");
            }
        }

        protected override void InternalDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: this.rasterizerState);
            spriteBatch.GraphicsDevice.ScissorRectangle = this.Bounds;

            spriteBatch.Draw(this.background, this.Bounds, Color.White);

            this.player.Draw(spriteBatch);

            this.testEnemy.Draw(spriteBatch);

            spriteBatch.End();
        }

        public override void LoadContent()
        {
            this.background = Root.LoadContent<Texture2D>("background");
            this.player.LoadContent();

            this.testEnemy.LoadContent();
            this.testEnemy.LoadFromFile("testenemy");
        }

        public override void UnloadContent()
        {
            this.background.Dispose();
        }
    }
}
