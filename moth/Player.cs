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
    class Player : Sprite
    {
        private readonly float speed = 0.5f;
        private readonly float focusMod = 0.5f;

        public readonly BulletPool BulletPool;
        private readonly float shootCooldown = 70.0f;
        private float shootCounter = 0;
        private bool canShoot = true;

        private readonly Vector2 hitboxPosition = new Vector2(64, 76);
        private readonly float hitboxR = 5.0f;

        public override Point SpriteSize => new Point(128, 128);

        public bool IsFocused => AKS.IsKeyDown(Root.KeyDefs.Focus);

        private Vector2 initialPosition;

        public Player(Scene parentScene) : base(parentScene)
        {
            //this.Origin = new Vector2(this.SpriteWidth * this.Scale / 2 - 0.5f, this.SpriteHeight * this.Scale / 2 - 0.5f);
            this.AddHitBox("hitbox", new CircleHitbox(this, this.hitboxPosition, hitboxR));

            this.BulletPool = new BulletPool(this, 5000);

            this.Scale = 0.5f;

            this.initialPosition = new Vector2(this.ParentScene.Bounds.Width / 2, this.ParentScene.Bounds.Height * 3 / 4);
            this.Position = this.initialPosition;
        }

        public void Reset()
        {
            this.BulletPool.Reset();
            this.Position = this.initialPosition;
        }

        public override void LoadContent()
        {
            this.texture = Root.LoadContent<Texture2D>("moth_back");
            this.BulletPool.LoadContent();

            this.MakeAnimations();
        }

        // only to be called once, by LoadTexture //
        private void MakeAnimations()
        {
            this.AddAnimation("idle", new FrameAnimation(this.TextureBounds, this.SpriteSize, 1, AnimationBehavior.Static));
            this.SetCurrentAnimation("idle");
        }

        private void Shoot(float angleDegTopAligned)
        {
            /*float angle = (angleDegTopAligned - 90) / 180.0f * (float)Math.PI;
            LappyBullet bullet = this.BulletPool.GetInactive<LappyBullet>();

            if (bullet != null)
            {
                bullet.Position.X = this.Position.X;
                bullet.Position.Y = this.Position.Y - this.ScaledSpriteHeight / 5;
                bullet.Direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                bullet.IsActive = true;
            }*/
        }

        protected override void InternalUpdate(GameTime gameTime)
        {
            base.InternalUpdate(gameTime);

            // update position //

            float mod = this.IsFocused ? this.focusMod : 1;

            Vector2 translation = new Vector2(AKS.Axis(Root.KeyDefs.Left, Root.KeyDefs.Right), AKS.Axis(Root.KeyDefs.Up, Root.KeyDefs.Down));
            translation *= this.speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds * mod;

            this.Position += translation;

            this.Position = this.Position.Clamped(new Vector2(hitboxR), GameScene.GameArea.ToVector2() - new Vector2(hitboxR));

            if (this.Position.X >= GameScene.GameArea.X / 2)
            {
                this.SpriteEffects = SpriteEffects.None;
            }
            else
            {
                this.SpriteEffects = SpriteEffects.FlipHorizontally;
            }

            /*if (translation.X < 0)
            {
                if (translation.Y <= 0)
                {
                    this.SpriteEffects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    this.SpriteEffects = SpriteEffects.None;
                }
            }
            else if (translation.X > 0)
            {
                if (translation.Y > 0)
                {
                    this.SpriteEffects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    this.SpriteEffects = SpriteEffects.None;
                }
            }*/
            //Log.Print(this.AbsoluteRoundedPosition.ToString() + "\n");

            // update shooting stuff //

            float shootMod = AKS.IsKeyDown(Root.KeyDefs.Focus) ? 0.1f : 1;

            if (this.canShoot)
            {
                if (AKS.IsKeyDown(Root.KeyDefs.Shoot))
                {
                    this.Shoot(0 * shootMod);
                    this.Shoot(-20 * shootMod);
                    this.Shoot(20 * shootMod);
                    this.Shoot(-40 * shootMod);
                    this.Shoot(40 * shootMod);
                    this.canShoot = false;
                    this.shootCounter = 0;
                }
            }
            else
            {
                this.shootCounter += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (this.shootCounter >= this.shootCooldown)
                {
                    this.canShoot = true;
                    this.shootCounter = 0;
                }
            }

            this.BulletPool.Update(gameTime);
        }

        protected override void InternalDraw(SpriteBatch spriteBatch)
        {
            this.BulletPool.Draw(spriteBatch);
            base.InternalDraw(spriteBatch);
        }
    }
}
