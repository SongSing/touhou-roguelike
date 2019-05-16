using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moth
{
    abstract class UnsealedEntity
    {
        public bool IsActive = true;
        public bool UpdateWhenInactive = false;
        public bool DrawWhenInactive = false;

        protected bool ShouldUpdate => this.IsActive || this.UpdateWhenInactive;
        protected bool ShouldDraw => this.IsActive || this.DrawWhenInactive;

        protected virtual void InternalUpdate(GameTime gameTime)
        {

        }

        protected virtual void InternalDraw(SpriteBatch spriteBatch)
        {

        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void LoadContent()
        {

        }
    }

    abstract class Entity : UnsealedEntity
    {
        public override void Update(GameTime gameTime)
        {
            if (this.ShouldUpdate)
            {
                this.InternalUpdate(gameTime);
            }
        }

        public sealed override void Draw(SpriteBatch spriteBatch)
        {
            if (this.ShouldDraw)
            {
                this.InternalDraw(spriteBatch);
            }
        }
    }
}
