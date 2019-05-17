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
    class Enemy : Sprite
    {
        public override Point SpriteSize => new Point(96, 96);
        private bool hasSpawned = false;
        private float spawnTime = 0;

        private string contentUrl;

        private List<BulletPool>[] bulletPoolMatrix;
        protected int currentPhase = 0;

        public Enemy(Scene parentScene) : base(parentScene)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            this.texture = Root.LoadContent<Texture2D>("enemies");
            this.AddAnimation("default", new FrameAnimation(this.TextureBounds, this.SpriteSize, 0, AnimationBehavior.Static));
            this.SetCurrentAnimation("default");
        }

        /*private IEnumerable<BulletPool> bulletPools
        {
            get
            {
                for (int i = 0; i < this.bulletPoolMatrix.Length; i++)
                {
                    foreach (BulletPool pool in this.bulletPoolMatrix[i])
                    {
                        yield return pool;
                    }
                }
            }
        }*/

        private IEnumerable<BulletPool> activePools
        {
            get
            {
                foreach (BulletPool pool in this.bulletPoolMatrix[this.currentPhase])
                {
                    yield return pool;
                }
            }
        }

        protected override void InternalUpdate(GameTime gameTime)
        {
            if (this.spawnTime > 0)
            {
                this.spawnTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                this.hasSpawned = true;
            }

            if (this.hasSpawned)
            {
                base.InternalUpdate(gameTime);
                
                foreach (BulletPool pool in this.activePools)
                {
                    pool.Update(gameTime);
                }

                // move around etc
            }
        }

        protected override void InternalDraw(SpriteBatch spriteBatch)
        {
            if (this.hasSpawned)
            {
                base.InternalDraw(spriteBatch);

                foreach (BulletPool pool in this.activePools)
                {
                    pool.Draw(spriteBatch);
                }
            }
        }

        public void Reset()
        {
            this.LoadContent();

            if (this.contentUrl != null)
            {
                this.LoadFromFile(this.contentUrl);
            }

            this.currentPhase = 0;
        }

        public bool IsDamaging(Sprite sprite)
        {
            // test touching here

            foreach (BulletPool pool in this.activePools)
            {
                foreach (Bullet bullet in pool.ActiveSprites)
                {
                    if (bullet.Intersects("hitbox", sprite, "hitbox"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void LoadFromFile(string filename)
        {
            this.contentUrl = filename;

            // load xml file //
            XmlDocument xml = new XmlDocument();
            xml.Load("./content/" + filename + ".xml");
            XmlNode parentNode = xml.SelectSingleNode("//enemy");

            // load enemy attributes //
            int spriteIndex = int.Parse(parentNode.Attributes["sprite"].Value);
            this.CurrentAnimation.CurrentFrame = spriteIndex;
            this.Position = Utils.ParseToVector(parentNode.Attributes["position"].Value);
            this.spawnTime = int.Parse(parentNode.Attributes["spawnTime"].Value);

            // load bullet nodes //
            XmlNodeList bulletNodes = xml.SelectNodes("enemy/bullet");
            int numPhases = 0;

            foreach (XmlNode bulletNode in bulletNodes)
            {
                if (bulletNode.Attributes["phase"] == null)
                {
                    throw new Exception("hey.. um, it needs a PHASE..");
                }

                int phase = int.Parse(bulletNode.Attributes["phase"].Value);
                if (phase > numPhases)
                {
                    numPhases = phase;
                }
            }

            if (numPhases == 0)
            {
                return;
            }

            this.bulletPoolMatrix = new List<BulletPool>[numPhases];

            for (int i = 0; i < this.bulletPoolMatrix.Length; i++)
            {
                this.bulletPoolMatrix[i] = new List<BulletPool>();
            }

            foreach (XmlNode bulletNode in bulletNodes)
            {
                // create bullet pool //
                int phase = int.Parse(bulletNode.Attributes["phase"].Value);
                int poolSize = int.Parse(bulletNode.Attributes["poolSize"].Value);
                TimedBulletPool pool = new TimedBulletPool(this, poolSize);
                this.bulletPoolMatrix[phase - 1].Add(pool);

                // get bullet info //
                string bulletType = bulletNode.Attributes["type"].Value;
                float bulletOffset = float.Parse(bulletNode.Attributes["offset"].Value);
                float bulletRepeat = float.Parse(bulletNode.Attributes["repeat"].Value);
                int bulletSpriteIndex = int.Parse(bulletNode.Attributes["sprite"].Value);
                float bulletRadius = float.Parse(bulletNode.Attributes["hitboxRadius"].Value);
                Vector2 bulletHitboxPosition = Utils.ParseToVector(bulletNode.Attributes["hitboxPosition"].Value);
                Vector2 bulletSpawnPosition = Utils.ParseToVector(bulletNode.Attributes["spawnPosition"].Value);
                string bulletSpawnPositionType = bulletNode.Attributes["spawnPositionType"].Value;
                string bulletDuration = bulletNode.Attributes["duration"].Value;
                               
                // initialize bullets //
                pool.OffsetTime = bulletOffset;
                pool.RepeatTime = bulletRepeat;
                pool.Position = bulletSpawnPosition;

                switch (bulletSpawnPositionType.ToLower())
                {
                    case "relative":
                        pool.PositionType = PositionType.Relative;
                        break;
                    case "absolute":
                        pool.PositionType = PositionType.Absolute;
                        break;
                }

                for (int i = 0; i < poolSize; i++)
                {
                    Bullet bullet = (Bullet)Activator.CreateInstance(Type.GetType("moth.Bullet" + bulletType.Capitlized()));

                    if (i == 0)
                    {
                        // set bulletpool attrs //

                        // default each to 0 //
                        foreach (string attrName in bullet.AttributeNames)
                        {
                            pool.CreateAttribute(attrName);
                        }

                        // set the ones we have //
                        foreach (XmlNode childNode in bulletNode)
                        {
                            pool.SetAttribute(
                                childNode.Name,
                                float.Parse(childNode.InnerText),
                                childNode.Attributes.GetValue<float>("delta", 0.0f),
                                childNode.Attributes.GetValue<float>("min", 0.0f),
                                childNode.Attributes.GetValue<float>("max", 0.0f),
                                childNode.Attributes.GetValue("behavior", "wrap"),
                                childNode.Attributes.GetValue("onUpdate", "false") == "true"
                            );
                        }
                    }

                    bullet.LoadContent();

                    bullet.CurrentAnimation.CurrentFrame = bulletSpriteIndex;
                    bullet.GetHitbox<CircleHitbox>("hitbox").Radius = bulletRadius;
                    bullet.GetHitbox<CircleHitbox>("hitbox").LocalPosition = bulletHitboxPosition;

                    if (bulletDuration == "infinity")
                    {
                        bullet.Duration = -1;
                    }
                    else
                    {
                        bullet.Duration = float.Parse(bulletDuration);
                    }

                    bullet.IsActive = false;
                    bullet.Initialize(this.ParentScene, pool);

                    pool.Add(bullet);
                }
            }
        }
    }
}
