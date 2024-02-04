using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bone_King
{
    class BoneKing
    {
        enum State
        {
            Standing,
            BeatChest,
            Throwing,
            SpecialBone
        }

        State state;

        AnimatedSprite sprite;
        public Rectangle collision;

        int counter;
        bool wasAngry, firstBone;
        public bool boneDrop, specialBoneDrop;

        const int ANIMATIONSPEED = 30;

        public BoneKing(int x, int y)
        {
            state = State.Standing;

            Vector2 source1 = new Vector2(92, 70);
            Vector2 source2 = new Vector2(95, 77);
            sprite = new AnimatedSprite(x, y, ANIMATIONSPEED, 0.9f, new List<Vector2>(){source1, source1, source2, source2});

            collision = new Rectangle(x, y, 95, 70);
        }

        public void Load(List<Texture2D> spriteSheets)
        {
            sprite.Load(spriteSheets);
        }

        public void Reset(GameValues vals)
        {
            if (vals.level < 5)
                ChangeState(State.Standing, (int)(ANIMATIONSPEED * 2 / vals.multiplier));
            else
                ChangeState(State.SpecialBone, (int)(ANIMATIONSPEED * 2 / vals.multiplier));

            firstBone = false;
            counter = 0;
        }

        private void ChangeState(State state, int animationSpeed)
        {
            this.state = state;
            sprite.ChangeSheet((int)state, animationSpeed: animationSpeed, reset: true);
        }

        public void Update(Random RNG, GameValues level)
        {
            if (!sprite.FrameFlag) 
                return;

            switch (state)
            {
            case State.Standing:

                int randomAngry = RNG.Next(0, 4);
                if (randomAngry == 0 && wasAngry == false && level.level < 5)
                {
                    ChangeState(State.BeatChest, (int)(ANIMATIONSPEED / level.multiplier));
                    break;
                }
                wasAngry = false;

                int randomSpecial = RNG.Next(0, 20);
                if (randomSpecial == 0 || firstBone == false)
                {
                    ChangeState(State.SpecialBone, (int)(ANIMATIONSPEED * 2 / level.multiplier));
                    break;
                }

                ChangeState(State.Throwing, (int)(ANIMATIONSPEED * 2 / level.multiplier));
                    
                break;
            case State.Throwing:

                counter++;
                if (counter == 3)
                {
                    ChangeState(State.Standing, (int)(ANIMATIONSPEED * 2 / level.multiplier));
                    boneDrop = true;

                    counter = 0;
                }

                break;
            case State.BeatChest:

                counter++;
                if (counter == 2)
                {
                    ChangeState(State.Standing, (int)(ANIMATIONSPEED * 2 / level.multiplier));
                    wasAngry = true;

                    counter = 0;
                }

                break;
            case State.SpecialBone:

                counter++;
                if (counter == 2)
                {
                    ChangeState(State.Standing, (int)(ANIMATIONSPEED * 2 / level.multiplier));
                    specialBoneDrop = true;
                    firstBone = true;

                    counter = 0;
                }

                break;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sprite.Draw(sb);
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D texture)
        {
            sb.Draw(texture, collision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
