using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace moth
{
    public enum AnimationBehavior
    {
        Loop,
        Static,
        OneShot,
        PingPong
    }

    class FrameAnimation
    {

        private readonly List<Rectangle> sourceRectangles;
        private readonly int fps;
        private int currentFrame = 0;
        private float fpsCounter = 0;
        private readonly float fpsCheck;
        private readonly int xCells, yCells;
        private int frameVelocity = 1;

        public AnimationBehavior Behavior;

        public int Count
        {
            get { return this.sourceRectangles.Count; }
        }

        public int CurrentFrame
        {
            get { return this.currentFrame; }
            set
            {
                this.currentFrame = value % this.sourceRectangles.Count;
            }
        }

        public FrameAnimation(Rectangle textureBounds, Point cellSize, int fps, AnimationBehavior behavior = AnimationBehavior.Loop)
        {
            this.fps = fps;
            this.fpsCheck = fps <= 0 ? -1 : 1000.0f / fps;

            this.xCells = textureBounds.Width / cellSize.X;
            this.yCells = textureBounds.Height / cellSize.Y;

            this.sourceRectangles = new List<Rectangle>();

            for (int y = 0; y < yCells; y++)
            {
                for (int x = 0; x < xCells; x++)
                {
                    this.sourceRectangles.Add(new Rectangle(x * cellSize.X, y * cellSize.Y, cellSize.X, cellSize.Y));
                }
            }

            this.Behavior = behavior;
        }

        public FrameAnimation(List<Rectangle> sourceRectangles, int numXCells, int numYCells, int fps)
        {
            this.fps = fps;
            this.fpsCheck = fps == 0 ? -1 : 1000.0f / fps;

            this.xCells = numXCells;
            this.yCells = numYCells;

            this.sourceRectangles = new List<Rectangle>(sourceRectangles);
        }

        public void Update(GameTime gameTime)
        {
            bool oneShotDone = this.Behavior == AnimationBehavior.OneShot && this.CurrentFrame == this.Count - 1;
            if (!oneShotDone && this.fpsCheck >= 0 && this.Behavior != AnimationBehavior.Static)
            {
                this.fpsCounter += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (this.fpsCounter >= this.fpsCheck)
                {
                    this.fpsCounter %= this.fpsCheck;

                    this.currentFrame += this.frameVelocity;
                    if (this.currentFrame >= this.Count)
                    {
                        if (this.Behavior == AnimationBehavior.Loop || this.Count == 1)
                        {
                            this.currentFrame = 0;
                        }
                        else if (this.Behavior == AnimationBehavior.PingPong)
                        {
                            this.currentFrame = this.Count - 2;
                            this.frameVelocity = -1;
                        }
                    }
                    else if (this.currentFrame < 0)
                    {
                        this.currentFrame = 1;
                        this.frameVelocity = 1;
                    }
                }
            }
        }

        public Rectangle CurrentSourceRectangle
        {
            get { return this.sourceRectangles.ElementAt(this.CurrentFrame); }
        }

        public FrameAnimation GetColumn(int zeroBasedColumn)
        {
            List<Rectangle> rects = new List<Rectangle>();

            for (int i = zeroBasedColumn; i < this.sourceRectangles.Count; i += this.xCells)
            {
                rects.Add(this.sourceRectangles.ElementAt(i));
            }

            return new FrameAnimation(rects, 1, this.yCells, this.fps);
        }

        public FrameAnimation GetRow(int zeroBasedRow)
        {
            return new FrameAnimation(this.sourceRectangles.GetRange(zeroBasedRow * this.xCells, this.xCells), this.xCells, 1, this.fps);
        }
    }
}
