using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Bone_King
{
    class AnimatedSprite : Sprite
    {
        List<Texture2D> sheets;
        List<Vector2> sourceSizes;
        Vector2 range; //X = min, Y = max
        Rectangle source;
        SpriteEffects spriteEffects;

        int frameTimer, animationSpeed, sheet;

        //Flags when a frame of animation is finished
        public bool FrameFlag
        {
            get 
            {
                if (frameTimer == 0)
                {
                    frameTimer--;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int YLayer
        {
            set { source.Y = source.Height * value; }
        }

        public AnimatedSprite(Vector2 position, int animationSpeed, float layer, Vector2 sourceSize) : base(position, layer)
        {
            sourceSizes = new List<Vector2>{sourceSize};
            source = new Rectangle(0, 0, (int)sourceSizes[0].X, (int)sourceSizes[0].Y);

            frameTimer = animationSpeed;
            this.animationSpeed = animationSpeed;
            spriteEffects = SpriteEffects.None;
        }

        public AnimatedSprite (Vector2 position, int animationSpeed, float layer, List<Vector2> sourceSizes) : base(position, layer)
        {
            this.sourceSizes = sourceSizes;
            source = new Rectangle(0, 0, (int)sourceSizes[0].X, (int)sourceSizes[0].Y);

            frameTimer = animationSpeed;
            this.animationSpeed = animationSpeed;
            spriteEffects = SpriteEffects.None;
        }

        public override void Load (Texture2D spritesheet)
        {
            sheets = new List<Texture2D>{spritesheet};
        }

        public void Load (List<Texture2D> spriteSheets)
        {
            sheets = spriteSheets;
        }

        public void ChangeSheet(int newSheet, int min = 0, int max = -1, int animationSpeed = -1, SpriteEffects spriteEffects = SpriteEffects.None, bool reset = true)
        {
            sheet = newSheet;
            this.spriteEffects = spriteEffects;

            range = new Vector2(min, max);

            if (animationSpeed >= 0)
                this.animationSpeed = animationSpeed;

            if (reset)
            {
                frameTimer = this.animationSpeed;
                source = new Rectangle((int)sourceSizes[newSheet].X * min, 0, (int)sourceSizes[newSheet].X, (int)sourceSizes[newSheet].Y);
            }
            else if (frameTimer > this.animationSpeed)
            {
                frameTimer = this.animationSpeed;
            }
        }

        public override void Draw (SpriteBatch spriteBatch)
        {
            if (frameTimer > 0)
            {
                frameTimer -= 1;
            }
            else
            {
                source.X += source.Width;
                if (source.X == sourceSizes[sheet].X * range.Y || source.X >= sheets[sheet].Width)
                    source.X = (int)(sourceSizes[sheet].X * range.X);

                frameTimer = animationSpeed;
            }
            
            spriteBatch.Draw(sheets[sheet], position, source, Color.White, 0, Vector2.Zero, 1, spriteEffects, layer);
        }
    }
}