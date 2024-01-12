using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bone_King
{
    class BoneKing
    {
        enum AnimState
        {
            Standing,
            Throwing,
            BeatChest,
            SpecialBone
        }

        AnimState animState;

        Texture2D standing, throwing, beatChest, special;
        Vector2 position;
        Rectangle source;
        public Rectangle collision;

        float frameTimer;
        int angryCounter;
        bool wasAngry, firstBone;
        public bool boneDrop, specialBoneDrop;

        const int ANIMATIONSPEED = 30;

        public BoneKing(Texture2D standing, Texture2D throwing, Texture2D angery, Texture2D special, int x, int y)
        {
            animState = AnimState.Standing;

            this.standing = standing;
            this.throwing = throwing;
            beatChest = angery;
            this.special = special;
            position = new Vector2(x, y);
            source = new Rectangle(0, 0, 95, 77);
            collision = new Rectangle(x, y, 95, 70);

            frameTimer = ANIMATIONSPEED * 2;
        }

        public void Update(GameTime gt, Random RNG, GameValues level)
        {
            switch (animState)
            {
                case AnimState.Standing:
                    if (frameTimer <= 0)
                    {
                        int randomAngry = RNG.Next(0, 4);
                        if (randomAngry == 0 && wasAngry == false && level.level < 5)
                        {
                            animState = AnimState.BeatChest;
                            frameTimer = 30;
                        }
                        else
                        {
                            int randomSpecial = RNG.Next(0, 20);
                            if (randomSpecial == 0 || firstBone == false)
                            {
                                animState = AnimState.SpecialBone;
                                frameTimer = (ANIMATIONSPEED * 2) / level.multiplier;
                            }
                            else
                            {
                                animState = AnimState.Throwing;
                                frameTimer = (ANIMATIONSPEED * 2) / level.multiplier;
                                wasAngry = false;
                            }
                        }
                    }
                    else
                    {
                        frameTimer -= 1;
                    }
                    break;
                case AnimState.Throwing:
                    if (frameTimer <= 0)
                    {
                      source.X += source.Width;
                      if (source.X >= throwing.Width)
                      {
                            source.X = 0;
                            animState = AnimState.Standing;
                            boneDrop = true;
                      }
                      frameTimer = (ANIMATIONSPEED * 2) / level.multiplier;
                    }
                    else
                    {
                        frameTimer -= 1;
                    }
                    break;
                case AnimState.BeatChest:
                    if (frameTimer <= 0)
                    {
                        source.X += source.Width;
                        if (source.X >= beatChest.Width)
                        {
                            angryCounter += 1;
                            source.X = 0;
                        }
                        frameTimer = ANIMATIONSPEED / level.multiplier;
                        if (angryCounter == 2)
                        {
                            source.X = 0;
                            animState = AnimState.Standing;
                            frameTimer = (ANIMATIONSPEED * 2) / level.multiplier;
                            angryCounter = 0;
                            wasAngry = true;
                        }
                    }
                    else
                    {
                        frameTimer -= 1;
                    }
                    break;
                case AnimState.SpecialBone:
                    if (frameTimer <= 0)
                    {
                        source.X += source.Width;
                        if (source.X >= 184)
                        {
                            source.X = 0;
                            animState = AnimState.Standing;
                            specialBoneDrop = true;
                            firstBone = true;
                        }
                        frameTimer = (ANIMATIONSPEED * 2) / level.multiplier;
                    }
                    else
                    {
                        frameTimer -= 1;
                    }
                    break;
            }

            if (animState == AnimState.BeatChest)
            {
                source.Width = 92;
                source.Height = 70;
            }
            else
            {
                source.Width = 95;
                source.Height = 77;
            }
        }

        public void Reset(GameValues vals)
        {
            if (vals.level < 5)
            {
                animState = AnimState.Standing;
            }
            else
            {
                animState = AnimState.SpecialBone;
            }

            source = new Rectangle(0, 0, 95, 77);

            frameTimer = (ANIMATIONSPEED * 2) / vals.multiplier;

            firstBone = false;
        }

        public void Draw(SpriteBatch sb)
        {
            switch (animState)
            {
                case AnimState.Standing:
                    sb.Draw(standing, new Vector2((int)position.X, (int)position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case AnimState.Throwing:
                    sb.Draw(throwing, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case AnimState.BeatChest:
                    sb.Draw(beatChest, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case AnimState.SpecialBone:
                    sb.Draw(special, new Vector2((int)position.X, (int)position.Y), source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
            }
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D texture)
        {
            sb.Draw(texture, collision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
