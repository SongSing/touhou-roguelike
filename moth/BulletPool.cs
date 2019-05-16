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
    public enum PositionType
    {
        Relative,
        Absolute
    };

    public enum DeltaBehavior
    {
        Wrap,
        Bounce,
        Stop
    };

    class BulletPool : SpriteCollection<Bullet>, IAttributeOwner
    {
        protected Sprite parent;
        public PositionType PositionType = PositionType.Relative;
        public Vector2 Position = Vector2.Zero;

        // [ attr, delta, min, max ] //
        public readonly Dictionary<string, AttributeInfo> Attributes;

        public BulletPool(Sprite parent, int maxSize) : base(maxSize)
        {
            this.parent = parent;
            this.Attributes = new Dictionary<string, AttributeInfo>();
        }

        public void CreateAttribute(string attr)
        {
            AttributeInfo a = new AttributeInfo();

            this.Attributes[attr] = a;
        }

        public void SetAttribute(string attr, float value, float delta, float min, float max, string behavior, bool onUpdate)
        {
            AttributeInfo b = new AttributeInfo()
            {
                Value = value,
                Delta = delta,
                Min = min,
                Max = max,
                OnUpdate = onUpdate
            };

            switch (behavior)
            {
                default:
                case "wrap":
                    b.Behavior = DeltaBehavior.Wrap;
                    break;
                case "bounce":
                    b.Behavior = DeltaBehavior.Bounce;
                    break;
                case "stop":
                    b.Behavior = DeltaBehavior.Bounce;
                    break;
            }

            this.Attributes[attr] = b;
        }

        public AttributeInfo GetAttribute(string name, Bullet requester)
        {
            return this.Attributes[name];
        }

        public void Shoot()
        {
            Bullet shot = this.GetInactive();
            shot.IsActive = true;

            if (this.PositionType == PositionType.Relative)
            {
                shot.ShootFrom(this.Position + this.parent.Position);
            }
            else
            {
                shot.ShootFrom(this.Position);
            }

            foreach (KeyValuePair<string, AttributeInfo> kv in this.Attributes.ToList())
            {
                if (kv.Value.OnUpdate)
                {
                    continue;
                }

                kv.Value.UpdateDeltas();
            }
        }

        protected override void InternalUpdate(GameTime gameTime)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.sprites[i].Update(gameTime);
            }
        }
    }
}
