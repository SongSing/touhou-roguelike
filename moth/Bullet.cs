using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Xml;

namespace moth
{
    abstract class Bullet : Sprite, IAttributeOwner
    {
        public override Point SpriteSize => new Point(16, 16);
        protected bool cullWhenOffscreen = true;
        public abstract string[] AttributeNames { get; }

        public bool IsFriendly = false;
        public float Damage = 0.0f;
        public float Duration = -1;

        protected IAttributeOwner attrParent;

        // [ attr, delta, min, max ] //
        public readonly Dictionary<string, AttributeInfo> Attributes;

        public Bullet() : base()
        {
            this.AddHitBox("hitbox", new CircleHitbox(this, Vector2.Zero, 0));
            this.Attributes = new Dictionary<string, AttributeInfo>();
        }

        public virtual void Initialize(Scene parentScene, IAttributeOwner attrParent)
        {
            this.ParentScene = parentScene;
            this.attrParent = attrParent;

            foreach (string attrName in this.AttributeNames)
            {
                this.Attributes[attrName] = this.GetAttribute(attrName, this).Clone();
            }
        }

        protected void Initialize()
        {
            this.Initialize(this.ParentScene, this.attrParent);
        }

        protected float attr(string name)
        {
            return this.Attributes[name].Value;
        }

        public virtual AttributeInfo GetAttribute(string name, Bullet requester)
        {
            return this.attrParent.GetAttribute(name, requester);
        }

        public virtual void ShootFrom(Vector2 position)
        {
            this.Position = position;
            this.Initialize();
        }

        protected override void InternalUpdate(GameTime gameTime)
        {
            base.InternalUpdate(gameTime);

            if (this.cullWhenOffscreen && !this.Bounds.Intersects(this.ParentScene.Bounds))
            {
                this.IsActive = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (this.ShouldUpdate)
            {
                this.InternalUpdate(gameTime);

                foreach (KeyValuePair<string, AttributeInfo> kv in this.Attributes.ToList())
                {
                    if (!kv.Value.OnUpdate)
                    {
                        continue;
                    }

                    kv.Value.UpdateDeltas((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            this.texture = Root.LoadContent<Texture2D>("bullets");

            this.AddAnimation("default", new FrameAnimation(this.TextureBounds, new Point(16, 16), 0, AnimationBehavior.Static));
            this.SetCurrentAnimation("default");
        }
    }
}