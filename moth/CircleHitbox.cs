using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace moth
{
    class CircleHitbox : Hitbox
    {
        public float Radius;
        public Vector2 LocalPosition;
        private Sprite followSprite = null;

        public CircleHitbox(Sprite follow, Vector2 localPosition, float radius)
        {
            this.followSprite = follow;
            this.Radius = radius;
            this.LocalPosition = localPosition;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public Vector2 Position
        {
            get
            {
                return this.LocalPosition + this.followSprite.TopLeftPosition;
            }
            set
            {
                this.LocalPosition = value - this.followSprite.TopLeftPosition;
            }
        }

        public override bool Intersects(Hitbox otherChecky)
        {
            if (otherChecky is CircleHitbox)
            {
                CircleHitbox other = otherChecky as CircleHitbox;
                Vector2 diff = this.Position - other.Position;
                return diff.LengthSquared() < Math.Pow(this.Radius + other.Radius, 2);
            }
            else
            {
                throw new NotImplementedException("idk that kind of hit box sorry !");
            }
        }
    }
}
