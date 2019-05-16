using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace moth
{
    class BulletOrbit : Bullet
    {
        public override string[] AttributeNames => new string[]
        {
            "diameter",
            "spinSpeed",
            "spinDirection",
            "angle",
            "speed",
            "spinProgress",
            "growth"
        };

        private Vector2 orbitPoint;
        private float spinProgress;
        private float diameter;

        public BulletOrbit() : base()
        {

        }

        public override void Initialize(Scene parentScene, IAttributeOwner attrParent)
        {
            base.Initialize(parentScene, attrParent);
            this.spinProgress = this.attr("spinProgress");
            if (this.attr("growth") > 0)
            {
                this.diameter = 0;
            }
            else
            {
                this.diameter = this.attr("diameter");
            }
        }

        public override void ShootFrom(Vector2 position)
        {
            base.ShootFrom(position);
            this.spinProgress = this.attr("spinProgress");

            this.orbitPoint = position;
            this.SetPosition();
        }

        private void SetPosition()
        {
            this.Position = this.orbitPoint + Utils.RadToVector2(this.spinProgress * 2 * (float)Math.PI) * this.diameter / 2;
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

            if (this.attr("growth") > 0 && this.diameter < this.attr("diameter"))
            {
                this.diameter += this.attr("growth") * (float)gameTime.ElapsedGameTime.TotalSeconds * 10;
            }
            else
            {
                this.diameter = this.attr("diameter");
            }

            //Root.DebugInfo.Add((Math.Sign(spinDirection) * gameTime.ElapsedGameTime.Seconds * spinSpeed).ToString());

            //Console.WriteLine(this.orbitPoint);
            this.SetPosition();

            base.InternalUpdate(gameTime);
        }
    }
}
