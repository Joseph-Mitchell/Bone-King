using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bone_King
{
    class Barry
    {
        public enum AnimState
        {
            StandingRight,
            StandingLeft,
            RunningRight,
            RunningLeft,
            JumpingRight,
            JumpingLeft,
            Climbing,
            ClimbingOver,
            Death
        }

        public AnimState animState;

        Texture2D m_running, m_jumping, m_axe, m_climbing, m_climbingOver, m_death;
        Vector2 m_position, m_velocity;
        Rectangle m_source, m_feetRectangle, m_ladderRectangle;
        public Rectangle collision, axeCollision;

        int m_frameTimer, m_frame, m_deathTimer, m_deathStage, m_axeSwingTimer, m_axeTotalTimer;
        bool m_feetCollisionCheck, m_facingRight, m_ladderIntersect, m_atLadderTop;
        public bool reset, holdingAxe, axeDown;

        const int ANIMATIONSPEED = 3, LADDERANIMATIONSPEED = 8, DEATHANIMATIONSPEED = 8, AXESPEED = 15, AXETIME = 400;
        const float GRAVITY = 0.1f, RUNSPEED = 1.5f, JUMPHEIGHT = -2.0f, JUMPSPEED = 1.5f;

        public Barry(Texture2D running, Texture2D axe, Texture2D jumping, Texture2D climbing, Texture2D climbingover, Texture2D death, int x, int y)
        {

            animState = AnimState.StandingRight;

            m_running = running;
            m_axe = axe;
            m_jumping = jumping;
            m_climbing = climbing;
            m_climbingOver = climbingover;
            m_death = death;
            m_position = new Vector2(x, y);
            m_velocity = Vector2.Zero;
            m_source = new Rectangle(0, 0, 32, 32);
            m_feetRectangle = new Rectangle(x + 4, y + 29, 26, 3);
            m_ladderRectangle = new Rectangle(x + 4, y + 14, 26, 3);
            collision = new Rectangle(x + 4, y, 26, 32);
            axeCollision = new Rectangle(x - 16, y, 20, 32);

            m_frameTimer = ANIMATIONSPEED;
            m_frame = 0;
            m_deathTimer = 30;
            m_deathStage = 0;
            m_axeSwingTimer = AXESPEED;
            m_axeTotalTimer = AXETIME;

            m_facingRight = true;
        }

        public void Update(Input currentInput, Input oldInput, Background background, GameValues gameValues, int maxX, Game1 game)
        {
            if (animState != AnimState.Death)
            {
                m_feetCollisionCheck = false;

                //Gravity
                if (m_velocity.Y < GRAVITY * 30 && animState != AnimState.Climbing && animState != AnimState.ClimbingOver)
                {
                    m_velocity.Y += GRAVITY;
                }

                //Checks if Barry is touching the ground
                for (int i = 0; i < background.platformHitBoxes.Length; i++)
                {
                    if (background.platformHitBoxes[i].Intersects(m_feetRectangle))
                    {
                        m_feetCollisionCheck = true;
                        if (m_velocity.Y >= 0)
                        {
                            m_velocity.Y = 0;
                            if (animState != AnimState.Climbing && animState != AnimState.ClimbingOver)
                            {
                                m_position.Y = background.platformHitBoxes[i].Top - collision.Height + 1;
                            }
                        }
                    }
                }

                //Jumping
                if (currentInput.action && m_feetCollisionCheck && animState != AnimState.ClimbingOver && holdingAxe == false)
                {
                    if (currentInput.left)
                    {
                        animState = AnimState.JumpingLeft;
                        m_velocity.Y = JUMPHEIGHT;
                        m_velocity.X = -JUMPSPEED;
                        m_facingRight = false;
                    }
                    else if (currentInput.right)
                    {
                        animState = AnimState.JumpingRight;
                        m_velocity.Y = JUMPHEIGHT;
                        m_velocity.X = JUMPSPEED;
                        m_facingRight = true;
                    }
                    else if (m_facingRight)
                    {
                        animState = AnimState.JumpingRight;
                        m_velocity.Y = JUMPHEIGHT;
                        m_velocity.X = 0;
                    }
                    else
                    {
                        animState = AnimState.JumpingLeft;
                        m_velocity.Y = JUMPHEIGHT;
                        m_velocity.X = 0;
                    }
                }

                #region Horizontal Movement
                //Run Left
                if (currentInput.left && m_feetCollisionCheck && currentInput.action == false && animState != AnimState.Climbing && animState != AnimState.ClimbingOver)
                {
                    m_velocity.X = -RUNSPEED;
                    animState = AnimState.RunningLeft;
                    m_facingRight = false;
                }
                //Run Right
                else if (currentInput.right && m_feetCollisionCheck && currentInput.action == false && animState != AnimState.Climbing && animState != AnimState.ClimbingOver)
                {
                    m_velocity.X = RUNSPEED;
                    animState = AnimState.RunningRight;
                    m_facingRight = true;
                }
                //Stand Right
                else if (m_feetCollisionCheck && currentInput.action == false && m_facingRight && animState != AnimState.Climbing && animState != AnimState.ClimbingOver)
                {
                    m_velocity.X = 0;
                    animState = AnimState.StandingRight;
                }
                //Stand Left
                else if (m_feetCollisionCheck && currentInput.action == false && m_facingRight == false && animState != AnimState.Climbing && animState != AnimState.ClimbingOver)
                {
                    m_velocity.X = 0;
                    animState = AnimState.StandingLeft;
                }
                #endregion

                #region Climb Ladder
                //Climb ladder up if infront of ladder and pressing correct button
                for (int i = 0; i < background.ladderHitBoxes.Length; i++)
                {
                    if (currentInput.up && m_feetCollisionCheck && collision.Intersects(background.ladderHitBoxes[i]) && collision.X + (collision.Width / 2) > background.ladderHitBoxes[i].X && collision.X + (collision.Width / 2) < background.ladderHitBoxes[i].X + background.ladderHitBoxes[i].Width && animState != AnimState.ClimbingOver && animState != AnimState.JumpingLeft && animState != AnimState.JumpingRight && holdingAxe == false)
                    {
                        m_frameTimer = 0;
                        m_feetCollisionCheck = false;
                        animState = AnimState.Climbing;
                        m_position.X = background.ladderHitBoxes[i].Center.X - (m_source.Width / 2);
                        m_position.Y -= 1;
                        m_velocity.X = 0;
                    }
                }
                //Climb ladder down if above ladder and pressing correct button
                for (int i = 0; i < background.ladderTops.Length; i++)
                {
                    if (currentInput.down && m_feetCollisionCheck && m_feetRectangle.Intersects(background.ladderTops[i]) && !background.brokenLadder[i] && collision.X + (collision.Width / 2) > background.ladderTops[i].X + (background.ladderTops[i].Width / 3) && collision.X + (collision.Width / 2) < background.ladderTops[i].X + ((background.ladderTops[i].Width / 3) * 2) && animState != AnimState.ClimbingOver && animState != AnimState.JumpingLeft && animState != AnimState.JumpingRight && holdingAxe == false)
                    {
                        m_frameTimer = 0;
                        m_feetCollisionCheck = false;
                        animState = AnimState.Climbing;
                        m_position.X = background.ladderHitBoxes[i].Center.X - (m_source.Width / 2);
                        m_position.Y += 17;
                        m_velocity.X = 0;
                    }
                }
                #endregion

                //Moves Barry on the ladder
                if (animState == AnimState.Climbing)
                {
                    if (currentInput.up)
                    {
                        m_position.Y -= RUNSPEED / 2;
                        for (int i = 0; i < background.ladderHitBoxes.Length; i++)
                        {
                            if (collision.Intersects(background.ladderHitBoxes[i]))
                            {
                                if (m_frameTimer <= 0)
                                {
                                    if (m_facingRight)
                                    {
                                        m_facingRight = false;
                                    }
                                    else
                                    {
                                        m_facingRight = true;
                                    }

                                    m_frameTimer = ANIMATIONSPEED * 3;
                                }
                                else
                                {
                                    m_frameTimer -= 1;
                                }
                            }
                            else
                            {
                                m_frame = 0;
                            }
                        }
                    }
                    if (currentInput.down)
                    {
                        m_position.Y += RUNSPEED / 2;
                        if (m_frameTimer <= 0)
                        {
                            if (m_facingRight)
                            {
                                m_facingRight = false;
                            }
                            else
                            {
                                m_facingRight = true;
                            }

                            m_frameTimer = ANIMATIONSPEED * 3;
                        }
                        else
                        {
                            m_frameTimer -= 1;
                        }
                    }

                    //Moves Barry down if he starts to go above the climbable section of any of the broken ladders
                    m_ladderIntersect = false;
                    for (int i = 0; i < background.ladderHitBoxes.Length; i++)
                    {
                        if (collision.Intersects(background.ladderHitBoxes[i]))
                        {
                            m_ladderIntersect = true;
                        }
                    }
                    if (m_ladderIntersect == false)
                    {
                        m_position.Y += RUNSPEED / 2;
                    }

                    //Makes Barry "Climb Over" the platform when he reaches the top of the ladder
                    m_atLadderTop = false;
                    for (int i = 0; i < background.platformHitBoxes.Length; i++)
                    {
                        if (m_ladderRectangle.Intersects(background.platformHitBoxes[i]))
                        {
                            m_atLadderTop = true;
                        }
                    }
                    for (int i = 0; i < background.ladderHitBoxes.Length; i++)
                    {
                        if (m_feetRectangle.Intersects(background.ladderHitBoxes[i]))
                        {
                            if (m_atLadderTop && m_feetRectangle.Y <= (background.ladderHitBoxes[i].Y + background.ladderHitBoxes[i].Height / 2) && currentInput.up)
                            {
                                //m_position.Y += 16;
                                animState = AnimState.ClimbingOver;
                                m_frame = 0;
                                m_frameTimer = LADDERANIMATIONSPEED;
                            }
                            else if (m_feetCollisionCheck)
                            {
                                animState = AnimState.StandingLeft;
                            }
                        }
                    }
                }

                //Times the axe swing
                if (m_axeSwingTimer <= 0)
                {
                    if (axeDown)
                        axeDown = false;
                    else
                        axeDown = true;

                    m_axeSwingTimer = AXESPEED;
                }
                else
                {
                    m_axeSwingTimer -= 1;
                }

                //Stops holding axe after a certain amount of time
                if (holdingAxe)
                {
                    if (m_axeTotalTimer <= 0)
                    {
                        holdingAxe = false;
                        m_axeTotalTimer = AXETIME;
                    }
                    else
                    {
                        m_axeTotalTimer -= 1;
                    }
                }

                //Stops Barry from going off the screen
                if (collision.X + collision.Width > maxX)
                {
                    m_position.X = maxX - collision.Width;
                }
                if (collision.X < 0)
                {
                    m_position.X = 0;
                }

                //Resets and increases level by 1 each time Barry reaches the goal
                if (collision.Intersects(background.goal) && animState != AnimState.Climbing && animState != AnimState.ClimbingOver && animState != AnimState.JumpingLeft && animState != AnimState.JumpingRight)
                {
                    gameValues.level += 1;
                    gameValues.score += (int)game.timer;
                    if (gameValues.level % 5 == 0)
                    {
                        game.lives += 1;
                    }
                    reset = true;
                }

                m_position += m_velocity;

                #region Sets size/position of collision rectangles depending on the Animation State
                if (animState == AnimState.StandingRight || animState == AnimState.StandingLeft)
                {
                    collision = new Rectangle((int)m_position.X + 4, (int)m_position.Y, 24, 32);
                    m_feetRectangle.X = (int)m_position.X + 4;
                    m_feetRectangle.Y = (int)m_position.Y + 29;
                    m_feetRectangle.Width = 24;
                    m_ladderRectangle.X = (int)m_position.X + 4;
                    m_ladderRectangle.Y = (int)m_position.Y + 14;
                    m_ladderRectangle.Width = 24;
                    m_source.X = 0;
                    if (m_facingRight)
                    {
                        axeCollision.X = (int)m_position.X + collision.Width + 4;
                    }
                    else
                    {
                        axeCollision.X = (int)m_position.X - 16;
                    }
                    axeCollision.Y = (int)m_position.Y;
                    if (holdingAxe)
                    {
                        if (m_source.X % 48 != 0)
                        {
                            m_source.X = 0;
                        }
                    }
                    else
                    {
                        m_source.Y = 0;
                        if (m_source.X % 32 != 0)
                        {
                            m_source.X = 0;
                        }
                    }
                    if (axeDown && holdingAxe)
                    {
                        m_source.Y = 48;
                    }
                    else
                    {
                        m_source.Y = 0;
                    }
                    m_source.Width = 32;
                    m_source.Height = 32;
                    if (holdingAxe)
                    {
                        m_source.Width = 48;
                        m_source.Height = 48;
                    }
                }
                if (animState == AnimState.RunningRight || animState == AnimState.RunningLeft)
                {
                    collision.X = (int)m_position.X + 1;
                    collision.Y = (int)m_position.Y;
                    collision.Width = 30;
                    collision.Height = 32;
                    m_feetRectangle.X = (int)m_position.X + 1;
                    m_feetRectangle.Y = (int)m_position.Y + 29;
                    m_feetRectangle.Width = 30;
                    m_ladderRectangle.X = (int)m_position.X + 1;
                    m_ladderRectangle.Y = (int)m_position.Y + 14;
                    m_ladderRectangle.Width = 30;
                    if (m_facingRight)
                    {
                        axeCollision.X = (int)m_position.X + collision.Width + 1;
                    }
                    else
                    {
                        axeCollision.X = (int)m_position.X - 19;
                    }
                    axeCollision.Y = (int)m_position.Y;
                    if (holdingAxe)
                    {
                        if (m_source.X % 48 != 0)
                        {
                            m_source.X = 0;
                        }
                    }
                    else
                    {
                        m_source.Y = 0;
                        if (m_source.X % 32 != 0)
                        {
                            m_source.X = 0;
                        }
                    }
                    if (axeDown && holdingAxe)
                    {
                        m_source.Y = 48;
                    }
                    else
                    {
                        m_source.Y = 0;
                    }
                    m_source.Width = 32;
                    m_source.Height = 32;
                    if (holdingAxe)
                    {
                        m_source.Width = 48;
                        m_source.Height = 48;
                    }
                }
                if (animState == AnimState.JumpingRight || animState == AnimState.JumpingLeft)
                {
                    collision.X = (int)m_position.X + 2;
                    collision.Y = (int)m_position.Y;
                    collision.Width = 28;
                    collision.Height = 30;
                    m_feetRectangle.X = (int)m_position.X;
                    m_feetRectangle.Y = (int)m_position.Y + 29;
                    m_feetRectangle.Width = 32;
                    m_ladderRectangle.X = (int)m_position.X;
                    m_ladderRectangle.Y = (int)m_position.Y + 14;
                    m_ladderRectangle.Width = 32;
                    if (m_facingRight)
                    {
                        axeCollision.X = (int)m_position.X + collision.Width + 1;
                    }
                    else
                    {
                        axeCollision.X = (int)m_position.X - 19;
                    }
                    axeCollision.Y = (int)m_position.Y;
                    if (m_source.X % 32 != 0)
                    {
                        m_source.X = 0;
                    }
                    m_source.X = 0;
                    m_source.Y = 0;
                    m_source.Width = 32;
                    m_source.Height = 32;
                }
                if (animState == AnimState.Climbing)
                {
                    collision.X = (int)m_position.X + 3;
                    collision.Y = (int)m_position.Y - 2;
                    collision.Width = 26;
                    collision.Height = 30;
                    m_feetRectangle.X = (int)m_position.X + 3;
                    m_feetRectangle.Y = (int)m_position.Y + 29;
                    m_feetRectangle.Width = 26;
                    m_ladderRectangle.X = (int)m_position.X + 3;
                    m_ladderRectangle.Y = (int)m_position.Y + 14;
                    m_ladderRectangle.Width = 26;
                    if (m_facingRight)
                    {
                        axeCollision.X = (int)m_position.X + collision.Width + 1;
                    }
                    else
                    {
                        axeCollision.X = (int)m_position.X - 19;
                    }
                    axeCollision.Y = (int)m_position.Y;
                    if (m_source.X % 32 != 0)
                    {
                        m_source.X = 0;
                    }
                    m_source.X = 0;
                    m_source.Y = 0;
                    m_source.Width = 32;
                    m_source.Height = 32;
                }
                if (animState == AnimState.ClimbingOver)
                {
                    collision.X = (int)m_position.X;
                    collision.Y = (int)m_position.Y;
                    collision.Width = 32;
                    collision.Height = 32;
                    m_feetRectangle.X = (int)m_position.X;
                    m_feetRectangle.Y = (int)m_position.Y + 29;
                    m_feetRectangle.Width = 32;
                    m_ladderRectangle.X = (int)m_position.X;
                    m_ladderRectangle.Y = (int)m_position.Y + 14;
                    m_ladderRectangle.Width = 32;
                    if (m_facingRight)
                    {
                        axeCollision.X = (int)m_position.X + collision.Width + 1;
                    }
                    else
                    {
                        axeCollision.X = (int)m_position.X - 19;
                    }
                    axeCollision.Y = (int)m_position.Y;
                    if (m_source.X % 32 != 0)
                    {
                        m_source.X = 0;
                    }
                    m_source.Y = 0;
                    m_source.Width = 32;
                    m_source.Height = 32;
                }
                #endregion
            }

            #region OldInputs
            oldInput.up = currentInput.up;
            oldInput.down = currentInput.down;
            oldInput.left = currentInput.left;
            oldInput.right = currentInput.right;
            oldInput.action = currentInput.action;
            oldInput.pause = currentInput.pause;
#if DEBUG
            oldInput.debug = currentInput.debug;
#endif
            #endregion
        }

        public void Reset(int x, int y)
        {
            animState = AnimState.StandingRight;

            m_position = new Vector2(x, y);
            m_velocity = Vector2.Zero;
            m_source = new Rectangle(0, 0, 32, 32);
            m_feetRectangle = new Rectangle(x + 4, y + 29, 26, 3);
            m_ladderRectangle = new Rectangle(x + 4, y + 14, 26, 3);
            collision = new Rectangle(x + 4, y, 26, 32);

            m_frameTimer = ANIMATIONSPEED;
            m_frame = 0;
            m_deathTimer = 30;
            m_deathStage = 0;
            m_axeSwingTimer = AXESPEED;
            m_axeTotalTimer = AXETIME;

            m_facingRight = true;
            reset = false;
            holdingAxe = false;
        }

        public void DeathUpdate(Game1 game)
        {
            if (game.paused == false)
            {
                if (animState == AnimState.Death)
                {
                    if (m_deathStage == 0)
                    {
                        if (m_deathTimer <= 0)
                        {
                            m_deathStage = 1;
                            m_deathTimer = 150;
                            m_source.X = 32;
                            if (game.deathSongInstance == null)
                            {
                                game.deathSongInstance = game.deathSong.CreateInstance();
                                game.deathSongInstance.Play();
                            }
                        }
                        else
                        {
                            m_deathTimer -= 1;
                        }
                    }
                    if (m_deathStage == 1)
                    {
                        if (m_deathTimer <= 0)
                        {
                            m_deathStage = 2;
                            m_deathTimer = 150;
                        }
                        else
                        {
                            m_deathTimer -= 1;
                        }
                    }
                    if (m_deathStage == 2)
                    {
                        if (m_deathTimer <= 0)
                        {
                            reset = true;
                        }
                        else
                        {
                            m_deathTimer -= 1;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb, Game1 game)
        {
            #region Draws based on animState
            switch (animState)
            {
                case AnimState.StandingRight:
                    if (holdingAxe)
                    {
                        sb.Draw(m_axe, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, new Vector2(0, 16), 1, SpriteEffects.FlipHorizontally, 0.9f);
                    }
                    else
                    {
                        sb.Draw(m_running, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0.9f);
                    }
                    break;
                case AnimState.StandingLeft:
                    if (holdingAxe)
                    {
                        sb.Draw(m_axe, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, new Vector2(20, 16), 1, SpriteEffects.None, 0.9f);
                    }
                    else
                    {
                        sb.Draw(m_running, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    break;
                case AnimState.RunningRight:
                    if (holdingAxe)
                    {
                        sb.Draw(m_axe, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, new Vector2(0, 16), 1, SpriteEffects.FlipHorizontally, 0.9f);
                    }
                    else
                    {
                        sb.Draw(m_running, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0.9f);
                    }
                    if (game.paused == false)
                    {
                        if (m_frameTimer <= 0)
                        {
                            m_source.X += m_source.Width;
                            if (m_source.X >= m_running.Width)
                            {
                                m_source.X = 0;
                            }
                            m_frameTimer = ANIMATIONSPEED;
                        }
                        else
                        {
                            m_frameTimer -= 1;
                        }
                    }
                    break;
                case AnimState.RunningLeft:
                    if (holdingAxe)
                    {
                        sb.Draw(m_axe, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, new Vector2(20, 16), 1, SpriteEffects.None, 0.9f);
                    }
                    else
                    {
                        sb.Draw(m_running, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    if (game.paused == false)
                    {
                        if (m_frameTimer <= 0)
                        {
                            m_source.X += m_source.Width;
                            if (m_source.X >= m_running.Width)
                            {
                                m_source.X = 0;
                            }
                            m_frameTimer = ANIMATIONSPEED;
                        }
                        else
                        {
                            m_frameTimer -= 1;
                        }
                    }
                    break;
                case AnimState.JumpingRight:
                    sb.Draw(m_jumping, new Vector2((int)m_position.X, (int)m_position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0.9f);
                    break;
                case AnimState.JumpingLeft:
                    sb.Draw(m_jumping, new Vector2((int)m_position.X, (int)m_position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case AnimState.Climbing:
                    if (m_facingRight)
                    {
                        sb.Draw(m_climbing, new Vector2((int)m_position.X, (int)m_position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    else
                    {
                        sb.Draw(m_climbing, new Vector2((int)m_position.X, (int)m_position.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0.9f);
                    }
                    break;
                case AnimState.ClimbingOver:
                    sb.Draw(m_climbingOver, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    if (game.paused == false)
                    {
                        if (m_frameTimer <= 0)
                        {
                            m_source.X += m_source.Width;
                            if (m_source.X >= m_climbingOver.Width)
                            {
                                m_frameTimer = 0;
                                m_source.X = 0;
                                animState = AnimState.StandingLeft;
                                m_frame = 0;
                            }
                            if (m_frame == 0 && animState != AnimState.StandingLeft)
                            {
                                m_position.Y -= 4;
                            }
                            if (m_frame == 1)
                            {
                                m_position.Y -= 15;
                            }
                            m_frame += 1;
                            m_frameTimer = LADDERANIMATIONSPEED;
                        }
                        else
                        {
                            m_frameTimer -= 1;
                        }
                    }
                    break;
                case AnimState.Death:
                    if (m_deathStage == 0)
                    {
                        if (m_source.X % 32 != 0)
                        {
                            m_source.X = 0;
                        }
                        m_source.X = 0;
                        m_source.Y = 0;
                        m_source.Width = 32;
                        m_source.Height = 32;
                        sb.Draw(m_death, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    if (m_deathStage == 1)
                    {
                        sb.Draw(m_death, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                        if (game.paused == false)
                        {
                            if (m_frameTimer <= 0)
                            {
                                m_source.X += 32;
                                m_frameTimer = DEATHANIMATIONSPEED;
                                if (m_source.X >= 160)
                                {
                                    m_source.X = 32;
                                }
                            }
                            else
                            {
                                m_frameTimer -= 1;
                            }
                        }
                    }
                    if (m_deathStage == 2)
                    {
                        m_source.X = 160;
                        sb.Draw(m_death, new Vector2((int)m_position.X, (int)m_position.Y), m_source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    break;
                default:
                    //panic
                    break;
            }
            #endregion
        }
#if DEBUG
        public void DebugDraw(SpriteBatch sb, Texture2D hitBox, SpriteFont font)
        {
            sb.Draw(hitBox, collision, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0.999f);
            sb.Draw(hitBox, m_feetRectangle, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
            sb.Draw(hitBox, m_ladderRectangle, null, Color.Green, 0, Vector2.Zero, SpriteEffects.None, 1);
            sb.Draw(hitBox, axeCollision, null, Color.Green, 0, Vector2.Zero, SpriteEffects.None, 1f);
            sb.DrawString(font, "Axe Time: " + m_axeTotalTimer, new Vector2(0, 16), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        }
#endif
    }
}
