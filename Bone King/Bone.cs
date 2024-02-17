using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bone_King
{
    class Bone : PhysicsObject
    {
        enum State
        {
            Rolling,
            Falling,
            Ladder
        }

        AnimatedSprite sprite;

        State state;

        bool atLadderTop;
        public bool active;

        const int ANIMATIONSPEED = 4;
        const float BONESPEED = 1.5f, BONESPEEDLADDER = 1f;

        public Collider PlayerCollider
        {
            get { return colliders[1]; }
            protected set { colliders[1] = value; }
        }

        public Collider ScoreCollider
        {
            get { return colliders[2]; }
            protected set { colliders[2] = value; }
        }

        public Bone(Vector2 position, List<Texture2D> spriteSheets) : base(position, new Point(15, 15), Point.Zero)
        {
            velocity = Vector2.Zero;

            sprite = new AnimatedSprite(position, ANIMATIONSPEED, 0.9f, new List<Vector2>{new Vector2(33, 33), new Vector2(50, 33)});

            colliders.AddRange(new List<Collider>
            { 
                new Collider(new Point(15, 13), new Point(0, 2)), //[1] Player Collision
                new Collider(new Point(4, 20), new Point(5, -20))  //[2] Score Collision
            });
            colliders[1].Update(position);
            colliders[2].Update(position);

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

        protected override void UpdatePosition()
        {
            base.UpdatePosition();
            sprite.Update(position);
        }

        protected override void Gravity()
        {
            if (!grounded)
            {
                if (velocity.Y < MAXFALL)
                    velocity.Y += GRAVITY;

                if (state != State.Falling)
                    ChangeState(State.Falling, facingRight: facingRight);
            }
        }

        public void Update(Ladder[] ladders, Rectangle[] platforms, Random RNG, float levelMultiplier)
        {
            UpdatePosition();

            atLadderTop = false;
            for (int i = 0; i < ladders.Length; i++)
            {
                if (!GroundCollider.Area.Intersects(ladders[i].Top))
                    continue;

                atLadderTop = true;

                if (state == State.Rolling)
                {
                    int random = RNG.Next(0, 24);
                    if (random == 0)
                    {
                        ChangeState(State.Ladder, reset: true);
                        position.X = (ladders[i].Top.X + (ladders[i].Top.Width / 2)) - 7;
                        position.Y += 10;
                    }
                }
                break;
            }

            if (!(atLadderTop && state == State.Ladder))
                CheckGrounded(platforms);

            if (state != State.Ladder)           
                Gravity();

            //Changes state depending on bones position and collisions
            if (grounded && state != State.Rolling)
            {
                bool reset = state == State.Ladder ? true : false;

                if ((position.Y >= 148 && position.Y <= 220) || (position.Y >= 268 && position.Y <= 336) || position.Y >= 390)
                    ChangeState(State.Rolling, reset: reset, facingRight: facingRight = false);
                else
                    ChangeState(State.Rolling, reset: reset, facingRight: facingRight = true);
            }

            //Updates collisions and velocity
            PlayerCollider.SetArea(new Point(15, 13));
            PlayerCollider.Offset = new Point(0, 2);

            int sign = facingRight ? 1 : -1;
            switch (state)
            {
                case State.Rolling:
               
                    velocity.X = sign * BONESPEED * levelMultiplier;
                    velocity.Y = 0;

                    break;
                case State.Falling:

                    int overEdge = position.X < 56 || position.X > 456 ? 2 : 1;

                    velocity.X = sign * BONESPEED * levelMultiplier / overEdge;
                    velocity.Y += GRAVITY * levelMultiplier;

                    break;
                case State.Ladder:

                    velocity.X = 0;
                    velocity.Y = BONESPEEDLADDER * levelMultiplier;

                    PlayerCollider.SetArea(new Point(40, 15));
                    PlayerCollider.Offset = Point.Zero;

                    break;
            }
            UpdateColliders();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
#if DEBUG
        public void DebugDraw(SpriteBatch spriteBatch, Texture2D hitBoxTexture)
        {
            spriteBatch.Draw(hitBoxTexture, GroundCollider.Area, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(hitBoxTexture, PlayerCollider.Area, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 0.99f);
            spriteBatch.Draw(hitBoxTexture, ScoreCollider.Area, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
