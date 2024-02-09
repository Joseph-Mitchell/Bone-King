using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bone_King
{
    class Bone
    {
        enum State
        {
            Rolling,
            Falling,
            Ladder
        }

        AnimatedSprite sprite;
        Vector2 position, velocity;
        public Rectangle groundCollision, playerCollision, scoreCollision;

        State state;

        bool collisionCheck, atLadderTop, facingRight;
        public bool active;

        const int ANIMATIONSPEED = 4;
        const float GRAVITY = 0.1f, BONESPEED = 1.5f, BONESPEEDLADDER = 1f;

        public Bone(int x, int y, List<Texture2D> spriteSheets)
        {
            position = new Vector2(x, y);
            velocity = Vector2.Zero;

            sprite = new AnimatedSprite(position, ANIMATIONSPEED, 0.9f, new List<Vector2>{new Vector2(33, 33), new Vector2(50, 33)});

            groundCollision = new Rectangle(x - 7, y - 7, 15, 15);
            playerCollision = new Rectangle(x - 7, y - 7, 15, 15);
            scoreCollision = new Rectangle(x - 2, y - 17, 4, 10);

            ChangeState(State.Rolling, facingRight = true);

            active = true;

            sprite.Load(spriteSheets);
        }

        private void ChangeState(State state, bool reset = false, bool facingRight = true)
        {
            this.state = state;

            int sheet = state == State.Ladder ? 1 : 0;

            if (facingRight)
                sprite.ChangeSheet(sheet, reset: reset);
            else              
                sprite.ChangeSheet(sheet, spriteEffects: SpriteEffects.FlipHorizontally, reset: reset);
        }

        public void Update(Level background, Random RNG, float levelMultiplier)
        {
            position += velocity;
            collisionCheck = false;

            atLadderTop = false;
            for (int i = 0; i < background.ladders.Length; i++)
            {
                if (groundCollision.Intersects(background.ladders[i].Top))
                    atLadderTop = true;
                else
                    continue;

                if (state == State.Rolling)
                {
                    int random = RNG.Next(0, 24);
                    if (random == 0)
                    {
                        ChangeState(State.Ladder, reset: true);
                        position.X = background.ladders[i].Top.X + (background.ladders[i].Top.Width / 2);
                        position.Y += 10;
                    }
                }
                break;
            }

            if (!(atLadderTop && state == State.Ladder))
            {
                for (int i = 0; i < background.platformHitBoxes.Length; i++)
                {
                    if (!groundCollision.Intersects(background.platformHitBoxes[i]))
                        continue;

                    collisionCheck = true;
                    position.Y = background.platformHitBoxes[i].Y - 7;
                    break;
                }
            }

            //Updates collisions and velocity
            groundCollision.X = (int)position.X - 7;
            groundCollision.Y = (int)position.Y - 7;
            scoreCollision = new Rectangle((int)position.X - 2, (int)position.Y - 27, 4, 20);
            playerCollision = new Rectangle((int)position.X - 7, (int)position.Y - 5, 15, 13);
            sprite.position.X = position.X - 17;
            sprite.position.Y = position.Y - 17;
            switch (state)
            {
                case State.Rolling:

                    int sign = facingRight ? 1 : -1;
                    velocity.X = sign * BONESPEED * levelMultiplier;
                    velocity.Y = 0;

                    break;
                case State.Falling:

                    if (!facingRight)
                    {
                        if (position.X + (groundCollision.Width / 2) < 64)
                            velocity.X = -(BONESPEED * levelMultiplier) / 2;
                        else
                            velocity.X = -BONESPEED * levelMultiplier;
                    }
                    else
                    {
                        if (position.X - (groundCollision.Width / 2) > 448)
                            velocity.X = (BONESPEED * levelMultiplier) / 2;
                        else
                            velocity.X = BONESPEED * levelMultiplier;
                    }

                    velocity.Y += GRAVITY * levelMultiplier;

                    break;
                case State.Ladder:

                    velocity.X = 0;
                    velocity.Y = BONESPEEDLADDER * levelMultiplier;
                    sprite.position.X = position.X - 25;
                    playerCollision = new Rectangle((int)position.X - 20, (int)position.Y - 7, 40, 15);

                    break;
            }

            //Changes state depending on bones position and collisions
            if (collisionCheck)
            {
                if (state != State.Rolling)
                {
                    bool reset = state == State.Ladder ? true : false;

                    if ((position.Y >= 155 && position.Y <= 213) || (position.Y >= 275 && position.Y <= 329) || position.Y >= 397)
                        ChangeState(State.Rolling, reset: reset, facingRight: facingRight = false);
                    else
                        ChangeState(State.Rolling, reset: reset, facingRight: facingRight = true);
                }
            }
            else if (state != State.Ladder && state != State.Falling)
            {
                ChangeState(State.Falling, facingRight: facingRight);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
#if DEBUG
        public void DebugDraw(SpriteBatch spriteBatch, Texture2D hitBoxTexture)
        {
            spriteBatch.Draw(hitBoxTexture, groundCollision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(hitBoxTexture, playerCollision, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 0.99f);
            spriteBatch.Draw(hitBoxTexture, scoreCollision, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
