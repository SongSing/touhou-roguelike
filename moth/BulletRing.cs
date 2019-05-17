using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace moth
{
    class BulletRing : Bullet
    {
        public override string[] AttributeNames => new string[]
        {
            "numBullets",
            "diameter",
            "spinSpeed",
            "spinDirection",
            "angle",
            "speed",
            "growth",
            "spinProgress"
        };

        private Vector2 orbitPoint;
        private BulletStationary[] bullets;

        private bool isInitialized = false;
        private float spinProgress = 0.0f;

        public BulletRing() : base()
        {
            this.cullWhenOffscreen = false;
        }

        public override void Initialize(Scene parentScene, IAttributeOwner parentPool)
        {
            base.Initialize(parentScene, parentPool);
            this.spinProgress = this.attr("spinProgress");

            int len = (int)this.attr("numBullets");

            if (!this.isInitialized)
            {
                this.bullets = new BulletStationary[len];

                for (int i = 0; i < len; i++)
                {
                    this.bullets[i] = new BulletStationary();
                    this.bullets[i].LoadContent();
                    this.bullets[i].Initialize(parentScene, this);
                    this.bullets[i].CurrentAnimation.CurrentFrame = this.CurrentAnimation.CurrentFrame;
                    this.bullets[i].SetHitbox("hitbox", this.GetHitbox("hitbox"));
                }

                this.isInitialized = true;
            }
        }

        public override void ShootFrom(Vector2 position)
        {
            base.ShootFrom(position);
            this.orbitPoint = position;
            int len = (int)this.attr("numBullets");

            for (int i = 0; i < len; i++)
            {
                this.SetPosition(i);
                this.bullets[i].IsActive = true;
            }
        }

        public void SetPositions()
        {
            for (int i = 0; i < this.bullets.Length; i++)
            {
                this.SetPosition(i);
            }
        }

        public void SetPosition(int i)
        {
            float angle = (this.spinProgress + (float)i / this.bullets.Length) * 2 * (float)Math.PI;
            this.bullets[i].Position = this.orbitPoint + Utils.RadToVector2(angle) * this.attr("diameter") / 2; ;
        }

        protected override void InternalUpdate(GameTime gameTime)
        {
            this.orbitPoint += Utils.DegToVector2(this.attr("angle") - 90) * this.attr("speed");
            this.spinProgress += Math.Sign(this.attr("spinDirection")) * (float)gameTime.ElapsedGameTime.TotalSeconds * this.attr("spinSpeed");
            if (this.spinProgress < 0)
            {
                this.spinProgress += -(float)Math.Ceiling(this.spinProgress) + 1;
            }
            this.spinProgress %= 1;

            bool shouldCull = true;
            int len = (int)this.attr("numBullets");

            for (int i = 0; i < len; i++)
            {
                this.SetPosition(i);

                this.bullets[i].Update(gameTime);

                if (this.bullets[i].IsActive)
                {
                    shouldCull = false;
                }
            }

            base.InternalUpdate(gameTime);

            if (shouldCull)
            {
                this.IsActive = false;
            }
        }

        protected override void InternalDraw(SpriteBatch spriteBatch)
        {
            int len = (int)this.attr("numBullets");

            for (int i = 0; i < len; i++)
            {
                this.bullets[i].Draw(spriteBatch);
            }

            //base.InternalDraw(spriteBatch);
        }
    }
}
