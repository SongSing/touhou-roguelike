using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace moth
{
    class TimedBulletPool : BulletPool
    {
        public float OffsetTime = 0.0f;
        public float RepeatTime = 0.0f;

        private float counter = 0.0f;
        private bool hasOffset = false;

        public TimedBulletPool(Sprite parent, int maxSize) : base(parent, maxSize)
        {

        }

        protected override void InternalUpdate(GameTime gameTime)
        {
            base.InternalUpdate(gameTime);

            this.counter += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            if (!this.hasOffset)
            {
                if (this.counter >= this.OffsetTime)
                {
                    this.hasOffset = true;
                    this.counter %= this.OffsetTime == 0 ? 1 : this.OffsetTime;
                    this.Shoot();
                }
            }

            if (this.hasOffset)
            {
                if (this.counter >= this.RepeatTime)
                {
                    this.counter %= this.RepeatTime;
                    this.Shoot();
                }
            }
        }
    }
}
