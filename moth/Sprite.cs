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
    abstract class Sprite : Entity
    {
        protected Texture2D texture;
        public Vector2 Position = Vector2.Zero;

        public float Rotation = 0.0f;
        public Vector2 Origin = Vector2.Zero;
        public SpriteEffects SpriteEffects = SpriteEffects.None;
        public int LayerDepth = 0;
        public float Scale = 1.0f;
        public float Opacity = 1.0f;
        public Color Tint = Color.White;

        private readonly Dictionary<string, Hitbox> hitboxes = new Dictionary<string, Hitbox>();

        private readonly Dictionary<string, FrameAnimation> animations = new Dictionary<string, FrameAnimation>();
        public FrameAnimation CurrentAnimation { get; private set; } = null;

        public Scene ParentScene = null;

        public abstract Point SpriteSize { get; }
        public Vector2 ScaledSpriteSize => this.SpriteSize.ToVector2() * this.Scale;

        public Vector2 LocalOrigin => this.SpriteSize.ToVector2() * this.Origin;
        public Vector2 ScaledLocalOrigin => this.SpriteSize.ToVector2() * this.Scale * this.Origin;

        public Rectangle TextureBounds => this.texture.Bounds;

        public Point RoundedPosition
        {
            get
            {
                return this.Position.Rounded();
            }
            set
            {
                this.Position = value.ToVector2();
            }
        }

        public Vector2 AbsolutePosition
        {
            get
            {
                return this.ParentScene == null ? this.Position : this.Position + this.ParentScene.Position;
            }
            set
            {
                if (this.ParentScene == null)
                {
                    this.Position = value;
                }
                else
                {
                    this.Position = value - this.ParentScene.Position;
                }
            }
        }

        public Point AbsoluteRoundedPosition
        {
            get
            {
                return this.AbsolutePosition.Rounded();
            }
            set
            {
                this.AbsolutePosition = value.ToVector2();
            }
        }

        public Vector2 TopLeftPosition
        {
            get
            {
                return this.Position - this.ScaledLocalOrigin;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(this.RoundedPosition, this.SpriteSize);
            }
        }

        protected Sprite()
        {
            this.Origin = new Vector2(0.5f);
        }

        protected Sprite(Scene parentScene)
        {
            this.Origin = new Vector2(0.5f);
            this.ParentScene = parentScene;
        }

        public float AngleTo(Sprite other)
        {
            return (float)Math.Atan2(other.Position.Y - this.Position.Y, other.Position.X - this.Position.X);
        }

        protected override void InternalUpdate(GameTime gameTime)
        {
            base.InternalUpdate(gameTime);

            if (this.CurrentAnimation != null)
            {
                this.CurrentAnimation.Update(gameTime);
            }
        }

        protected override void InternalDraw(SpriteBatch spriteBatch)
        {
            base.InternalDraw(spriteBatch);

            spriteBatch.Draw(
                this.texture,
                this.AbsoluteRoundedPosition.ToVector2(),
                this.CurrentAnimation == null ? this.texture.Bounds : this.CurrentAnimation.CurrentSourceRectangle,
                this.Tint * this.Opacity,
                this.Rotation,
                this.LocalOrigin,
                this.Scale,
                this.SpriteEffects,
                this.LayerDepth
            );
        }

        public void AddAnimation(string name, FrameAnimation animation)
        {
            this.animations[name] = animation;
        }

        public void SetCurrentAnimation(string name)
        {
            if (!this.animations.ContainsKey(name))
            {
                throw new Exception("Animation " + name + " doesn't exist sorry");
            }

            this.CurrentAnimation = this.animations[name];
        }

        public void UnsetCurrentAnimation()
        {
            this.CurrentAnimation = null;
        }

        public void AddHitBox(string name, Hitbox hitbox)
        {
            this.hitboxes.Add(name, hitbox);
        }

        public Hitbox GetHitbox(string name)
        {
            if (this.hitboxes.ContainsKey(name))
            {
                return this.hitboxes[name];
            }
            else
            {
                throw new Exception("Hitbox doesn't exist: " + name);
            }
        }

        public T GetHitbox<T>(string name) where T : Hitbox
        {
            if (this.hitboxes.ContainsKey(name))
            {
                return (T)this.hitboxes[name];
            }
            else
            {
                throw new Exception("Hitbox doesn't exist: " + name);
            }
        }

        public void SetHitbox(string name, Hitbox hitbox)
        {
            this.hitboxes[name] = hitbox;
        }

        public IEnumerable<Hitbox> Hitboxes
        {
            get
            {
                foreach (Hitbox hitbox in this.hitboxes.Values)
                {
                    yield return hitbox;
                }
            }
        }

        public bool Intersects(Hitbox myHitbox, Hitbox otherHitbox)
        {
            return myHitbox.Intersects(otherHitbox);
        }

        public bool Intersects(string myHitboxName, Sprite otherSprite, string otherHitboxName)
        {
            return this.Intersects(this.GetHitbox<Hitbox>(myHitboxName), otherSprite.GetHitbox<Hitbox>(otherHitboxName));
        }

        public bool Intersects(string myHitboxName, Sprite otherSprite)
        {
            Hitbox myHitbox = this.GetHitbox<Hitbox>(myHitboxName);
            foreach (Hitbox otherHitbox in otherSprite.Hitboxes)
            {
                if (myHitbox.Intersects(otherHitbox))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Intersects(Sprite otherSprite)
        {
            foreach (Hitbox myHitbox in this.Hitboxes)
            {
                foreach (Hitbox otherHitbox in otherSprite.Hitboxes)
                {
                    if (myHitbox.Intersects(otherHitbox))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Intersects(Sprite otherSprite, string otherHitboxName)
        {
            Hitbox otherHitbox = otherSprite.GetHitbox<Hitbox>(otherHitboxName);

            foreach (Hitbox myHitbox in this.Hitboxes)
            {
                if (myHitbox.Intersects(otherHitbox))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
