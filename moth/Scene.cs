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
    abstract class Scene : Entity
    {
        public Rectangle Bounds = Rectangle.Empty;
        public virtual void UnloadContent() { }

        public Vector2 Position
        {
            get
            {
                return this.Bounds.Location.ToVector2();
            }
            set
            {
                this.Bounds.Location = value.ToPoint();
            }
        }
    }
}
