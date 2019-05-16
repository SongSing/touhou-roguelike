using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace moth
{
    class BulletLinear : Bullet
    {
        public override string[] AttributeNames => new string[]
        {
            "angle",
            "speed"
        };

        public BulletLinear() : base()
        {

        }

        protected override void InternalUpdate(GameTime gameTime)
        {
            this.Position += Utils.DegToVector2(this.attr("angle") - 90) * this.attr("speed");
            base.InternalUpdate(gameTime);
        }
    }
}
