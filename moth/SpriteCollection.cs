using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace moth
{
    class SpriteCollection<T> : Entity where T : Sprite
    {
        protected T[] sprites;
        public readonly int MaxSize;
        public int Count { get; protected set; } = 0;

        public SpriteCollection(int maxSize)
        {
            this.MaxSize = maxSize;
            this.sprites = new T[maxSize];
        }

        public override void LoadContent()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.sprites[i].LoadContent();
            }
        }

        protected override void InternalUpdate(GameTime gameTime)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.sprites[i].Update(gameTime);
            }
        }

        protected override void InternalDraw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.sprites[i].Draw(spriteBatch);
            }
        }

        public void Reset()
        {
            foreach (T sprite in this.ActiveSprites)
            {
                sprite.IsActive = false;
            }
        }

        public void Add(T sprite)
        {
            if (this.Count >= this.MaxSize)
            {
                throw new Exception("too many dumb ass");
            }
            else
            {
                //sprite.IsActive = false;
                this.sprites[this.Count] = sprite;
                this.Count++;
            }
        }

        public TT GetInactive<TT>() where TT : T
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this.sprites[i] != null && typeof(TT) == this.sprites[i].GetType() && !this.sprites[i].IsActive)
                {
                    return this.sprites[i] as TT;
                }
            }

            return null;
        }

        public T GetInactive()
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this.sprites[i] != null &&!this.sprites[i].IsActive)
                {
                    return this.sprites[i];
                }
            }

            return null;
        }

        public IEnumerable<T> ActiveSprites
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this.sprites[i].IsActive)
                    {
                        yield return this.sprites[i];
                    }
                }
            }
        }

        public virtual void Clear()
        {
            this.sprites = new T[this.MaxSize];
            this.Count = 0;
        }

        public bool Contains(T sprite)
        {
            return this.sprites.Contains(sprite);
        }
    }
}
