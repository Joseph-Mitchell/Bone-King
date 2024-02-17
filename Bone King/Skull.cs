using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Skull : PhysicsObject
    {
        enum State
        {
            Ladder,
            Moving,
        }

        AnimatedSprite sprite;

        State state;

        bool ladderIntersect;
        public bool active;

        const int ANIMATIONSPEED = 15;
        const float MOVEMENTSPEED = 0.7f;

        public Skull (Vector2 position, Texture2D spriteSheet) : base(position, new Point(30, 32), Point.Zero)
        {
            sprite = new AnimatedSprite(position, ANIMATIONSPEED, 1, new Vector2(30, 32));

            facingRight = true;
            SetState(State.Moving);
            active = true;

            sprite.Load(spriteSheet);
        }

        public void Load(Texture2D spriteSheet)
        {
            sprite.Load(spriteSheet);
        }

        private void SetState(State newState)
        {
            state = newState;

            SpriteEffects spriteEffects = facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.ChangeSheet(0, spriteEffects: spriteEffects);
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();
            sprite.Update(position);
        }

        public void Update(Rectangle[] platforms, Ladder[] ladders, bool playerHoldingAxe)
        {
            UpdatePosition();

            CheckGrounded(platforms);

            if (state != State.Ladder)
                Gravity();

            //Check ladder intersect and starts climbing a ladder
            ladderIntersect = false;
            for (int i = 0; i < ladders.Length; i++)
            {
                if (!colliders[0].Area.Intersects(ladders[i].Body))
                    continue;

                ladderIntersect = true;

                if (state == State.Ladder)
                    break;

                if (ladders[i].Broken)
                    break;

                bool moreThan13rdWidth = colliders[0].Area.Center.X > ladders[i].Body.X + (ladders[i].Body.Width / 3);
                bool lessThan23rdWidth = colliders[0].Area.Center.X < ladders[i].Body.X + (ladders[i].Body.Width / 3 * 2);

                if (moreThan13rdWidth && lessThan23rdWidth)
                {
                    state = State.Ladder;
                    position.X = ladders[i].Body.Center.X - (colliders[0].Area.Width / 2);
                }

                break;
            }

            if (state == State.Ladder && !ladderIntersect)
            {
                if ((position.Y >= 359 || (position.Y >= 239 && position.Y < 299) || (position.Y >= 119 && position.Y < 179)))
                    facingRight = true;
                else if (((position.Y < 359 && position.Y >= 299) || (position.Y < 239 && position.Y >= 179) || (position.Y < 119)))
                    facingRight = false;

                SetState(State.Moving);
            }

            if (state == State.Moving)
            {
                if (facingRight)
                    velocity.X = MOVEMENTSPEED;
                else
                    velocity.X = -MOVEMENTSPEED;
            }
            else if (state == State.Ladder)
            {
                velocity.X = 0;
                velocity.Y = -MOVEMENTSPEED;
            }

            if (playerHoldingAxe)
                sprite.YLayer = 1;
            else
                sprite.YLayer = 0;

            UpdateColliders();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
#if DEBUG
        public void DebugDraw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, colliders[0].Area, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
#endif
    }
}
