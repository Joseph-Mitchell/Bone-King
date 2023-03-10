using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Bone_King
{
    public class Game1 : Game
    {
        public enum GameState
        {
            Menu,
            Instructions,
            Cutscene1,
            Playing,
            Cutscene2,
            GameOver
        }

        public GameState gameState;

        //Classes
        Texture2D hitBoxTexture;
        SpriteFont debugFont, mainFont, bigFont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random RNG;
        SoundEffect menuSong, playLoop, axeLoop, introSong, point, applause, highScoreSong;
        public SoundEffect deathSong, bang, drum;
        SoundEffectInstance menuSongInstance, playLoopInstance, axeLoopInstance, introInstance, applauseInstance, highScoreSongInstance;
        public SoundEffectInstance deathSongInstance;
        VideoPlayer introPlayer, cutscene2Player;
        Video introVideo, cutscene2;

        //New Classes
        StaticObject menuBackground, pauseScreen, gameOver, finalScore, bonePile;
        StaticObject[] pages;
        List<Scores> scores;
        PlayButton button1;
        InstructionsButton button2;
        ExitButton button3;
        MenuButton button4;
        LeftArrow button5;
        RightArrow button6;
        Background background;
        List<Bone> bones;
        List<SpecialBone> specialBones;
        List<Skull> skulls;
        Axe[] axes;
        Barry player;
        BoneKing boney;
        AnimatedObject beatrice;
        Fire fire;
        GameValues gameValues;
        Input currentInput, oldInput;
        ParticleBurst particleBurst1, particleBurst2;

        //Structures
        bool m_debugShow, m_currentScoreCollision, m_oldScoreCollision, m_introPlayed, m_cutscene2Played, m_finalScore, m_highScore, m_drum;
        public bool paused;
        public float timer, finalScoreTimer, highScoreBlinkTimer;
        public int lives, instructionPage;

        const int HIGHSCOREBLINKTIME = 51;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 512;
            graphics.PreferredBackBufferHeight = 448;
        }

        protected override void Initialize()
        {
            gameState = GameState.Menu;

            menuSongInstance = null;
            introInstance = null;
            applauseInstance = null;

            pages = new StaticObject[5];
            bones = new List<Bone>();
            specialBones = new List<SpecialBone>();
            skulls = new List<Skull>();
            axes = new Axe[4];
            scores = new List<Scores>();

            currentInput = new Input();
            oldInput = new Input();
            RNG = new Random();
            gameValues = new GameValues();

            timer = 50;
            finalScoreTimer = 180;
            highScoreBlinkTimer = HIGHSCOREBLINKTIME;
            lives = 3;
            instructionPage = 0;

            introPlayer = new VideoPlayer();
            cutscene2Player = new VideoPlayer();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region New Classes
            menuBackground = new StaticObject(Content.Load<Texture2D>("Textures\\menubackground"), 0, 0, 0.1f);
            pauseScreen = new StaticObject(Content.Load<Texture2D>("Textures\\pausescreen"), 0, 0, 1);
            gameOver = new StaticObject(Content.Load<Texture2D>("Textures\\gameoverscreen"), 0, 0, 1);
            finalScore = new StaticObject(Content.Load<Texture2D>("Textures\\finalscore"), 0, 0, 0);
            bonePile = new StaticObject(Content.Load<Texture2D>("Textures\\bonepile"), 0, 62, 0.9f);
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i] = new StaticObject(Content.Load<Texture2D>("Textures\\page-" + i), 0, 0, 0.1f);
            }
            button1 = new PlayButton(Content.Load<Texture2D>("Textures\\playbutton"), graphics.PreferredBackBufferWidth / 2, 180)
            {
                hovered = true
            };
            button2 = new InstructionsButton(Content.Load<Texture2D>("Textures\\instructionsbutton"), graphics.PreferredBackBufferWidth / 2, 260);
            button3 = new ExitButton(Content.Load<Texture2D>("Textures\\exitbutton"), graphics.PreferredBackBufferWidth / 2, 340);
            button4 = new MenuButton(Content.Load<Texture2D>("Textures\\menubutton"), graphics.PreferredBackBufferWidth / 2, 17)
            {
                hovered = true
            };
            button5 = new LeftArrow(Content.Load<Texture2D>("Textures\\arrowflipped"), 18, 18)
            {
                color = Color.TransparentBlack
            };
            button6 = new RightArrow(Content.Load<Texture2D>("Textures\\arrow"), graphics.PreferredBackBufferWidth - 18, 18);
            background = new Background(Content.Load<Texture2D>("Textures\\background"));
            player = new Barry(Content.Load<Texture2D>("Textures\\barryRun"),
                Content.Load<Texture2D>("Textures\\barryRunWithAxe"),
                Content.Load<Texture2D>("Textures\\barryJump"),
                Content.Load<Texture2D>("Textures\\barryClimb"),
                Content.Load<Texture2D>("Textures\\barryClimbOver"),
                Content.Load<Texture2D>("Textures\\barryDeath"), 
                80, 380);
            boney = new BoneKing(Content.Load<Texture2D>("Textures\\standingBoneKing"),
                Content.Load<Texture2D>("Textures\\throwingBoneKing"),
                Content.Load<Texture2D>("Textures\\angeryBoneKing"),
                Content.Load<Texture2D>("Textures\\specialBoneKing"),
                42, 41);
            beatrice = new AnimatedObject(Content.Load<Texture2D>("Textures\\beatrice"), 169, 35, 30, 44, 30, 1);
            fire = new Fire(Content.Load<Texture2D>("Textures\\fire"), 78, 379, 30, 36, 15, 0.8f);
            axes[0] = new Axe(Content.Load<Texture2D>("Textures\\axe"), 24, 310);
            axes[1] = new Axe(Content.Load<Texture2D>("Textures\\axe"), 470, 250);
            axes[2] = new Axe(Content.Load<Texture2D>("Textures\\axe"), 24, 190);
            axes[3] = new Axe(Content.Load<Texture2D>("Textures\\axe"), 470, 130);
            int potato = RNG.Next(0, 4);
            axes[potato].isActive = true;
            #endregion

            //Textures
            hitBoxTexture = Content.Load<Texture2D>("Textures\\hitBox");
            Content.Load<Texture2D>("Textures\\boneRoll");

            //Fonts
            debugFont = Content.Load<SpriteFont>("Fonts\\debugFont");
            mainFont = Content.Load<SpriteFont>("Fonts\\mainfont");
            bigFont = Content.Load<SpriteFont>("Fonts\\bigfont");

            //SoundEffects/songs
            menuSong = Content.Load<SoundEffect>("Sounds\\menu");
            playLoop = Content.Load<SoundEffect>("Sounds\\playloop");
            axeLoop = Content.Load<SoundEffect>("Sounds\\axe");
            deathSong = Content.Load<SoundEffect>("Sounds\\death");
            introSong = Content.Load<SoundEffect>("Sounds\\intro");
            bang = Content.Load<SoundEffect>("Sounds\\bang");
            point = Content.Load<SoundEffect>("Sounds\\point");
            applause = Content.Load<SoundEffect>("Sounds\\lightapplause");
            drum = Content.Load<SoundEffect>("Sounds\\drum");
            highScoreSong = Content.Load<SoundEffect>("Sounds\\metro");

            //Videos
            introVideo = Content.Load<Video>("Videos\\intro");
            cutscene2 = Content.Load<Video>("Videos\\cutscene 2");

            //Particles
            List<Texture2D> textures = new List<Texture2D>
            {
                Content.Load<Texture2D>("Textures\\Particles\\circle"),
                Content.Load<Texture2D>("Textures\\Particles\\star"),
                Content.Load<Texture2D>("Textures\\Particles\\diamond")
            };
            particleBurst1 = new ParticleBurst(textures, (graphics.PreferredBackBufferWidth / 2) - (bigFont.MeasureString("HIGHSCORE").X / 2) + 20, 224, 20, 102, -3f, -1f);
            particleBurst2 = new ParticleBurst(textures, (graphics.PreferredBackBufferWidth / 2) + (bigFont.MeasureString("HIGHSCORE").X / 2) - 20, 224, 20, 102, 3f, 1f);
        }

        //Called when player dies with lives remaining
        public void Reset()
        {
            #region Lists and Arrays
            for (int i = 0; i < bones.Count; i++)
            {
                bones[i].isActive = false;
            }
            for (int i = 0; i < specialBones.Count; i++)
            {
                specialBones[i].isActive = false;
            }
            for (int i = 0; i < skulls.Count; i++)
            {
                skulls[i].isActive = false;
            }
            for (int i = 0; i < axes.Length; i++)
            {
                axes[i].isActive = false;
            }
            for (int i = 0; i < scores.Count; i++)
            {
                scores[i].isActive = false;
            }
            #endregion

            bones.RemoveAll(b => b.isActive == false);
            specialBones.RemoveAll(sb => sb.isActive == false);
            skulls.RemoveAll(s => s.isActive == false);
            scores.RemoveAll(s => s.isActive == false);

            if (player.collision.Intersects(background.goal))
            {
                gameState = GameState.Cutscene2;
            }
            player.Reset(80, 380);
            boney.Reset(gameValues);
            fire.Reset();

            timer = gameValues.initialTime;

            int potato = RNG.Next(0, 4);
            axes[potato].isActive = true;
        }

        //Called when player dies and has no remaining lives
        public void FullReset()
        {
            #region Lists and Arrays
            for (int i = 0; i < bones.Count; i++)
            {
                bones[i].isActive = false;
            }
            for (int i = 0; i < specialBones.Count; i++)
            {
                specialBones[i].isActive = false;
            }
            for (int i = 0; i < skulls.Count; i++)
            {
                skulls[i].isActive = false;
            }
            for (int i = 0; i < axes.Length; i++)
            {
                axes[i].isActive = false;
            }
            for (int i = 0; i < scores.Count; i++)
            {
                scores[i].isActive = false;
            }
            #endregion

            bones.RemoveAll(b => b.isActive == false);
            specialBones.RemoveAll(sb => sb.isActive == false);
            skulls.RemoveAll(s => s.isActive == false);
            scores.RemoveAll(s => s.isActive == false);

            player.Reset(80, 380);
            boney.Reset(gameValues);
            fire.Reset();

            timer = gameValues.initialTime;

            int potato = RNG.Next(0, 4);
            axes[potato].isActive = true;

            m_debugShow = false;
            m_currentScoreCollision = false;
            m_oldScoreCollision = false;
            lives = 3;
            gameValues.level = 0;
            gameValues.score = (int)(gameValues.score * gameValues.multiplier);

            gameState = GameState.GameOver;
        }

        protected override void Update(GameTime gameTime)
        {
            currentInput.Update();
            switch (gameState)
            {
                case GameState.Menu:
                    //Plays the menu song
                    if (menuSongInstance == null)
                    {
                        menuSongInstance = menuSong.CreateInstance();
                        menuSongInstance.Volume = 0.5f;
                        menuSongInstance.IsLooped = true;
                        menuSongInstance.Play();
                    }
                    else if (menuSongInstance.State == SoundState.Stopped)
                    {
                        menuSongInstance.Play();
                    }

                    //Handles button movement and selection
                    if(button1.hovered)
                    {
                        if (currentInput.up && oldInput.up == false)
                        {
                            button1.hovered = false;
                            button3.hovered = true;
                        }
                        if (currentInput.down && oldInput.down == false)
                        {
                            button1.hovered = false;
                            button2.hovered = true;
                        }
                        if (currentInput.action && !oldInput.action)
                        {
                            button1.Pressed(this);
                            menuSongInstance.Stop();
                        }
                    }
                    else if (button2.hovered)
                    {
                        if (currentInput.up && oldInput.up == false)
                        {
                            button2.hovered = false;
                            button1.hovered = true;
                        }
                        if (currentInput.down && oldInput.down == false)
                        {
                            button2.hovered = false;
                            button3.hovered = true;
                        }
                        if (currentInput.action && oldInput.action == false)
                        {
                            button2.Pressed(this);
                        }
                    }
                    else if (button3.hovered)
                    {
                        if (currentInput.up && oldInput.up == false)
                        {
                            button3.hovered = false;
                            button2.hovered = true;
                        }
                        if (currentInput.down && oldInput.down == false)
                        {
                            button3.hovered = false;
                            button1.hovered = true;
                        }
                        if (currentInput.action)
                        {
                            button3.Pressed(this);
                        }
                    }
                    break;
                case GameState.Instructions:
                    //Handles button movement and selection
                    if (button4.hovered)
                    {
                        if (currentInput.left && oldInput.left == false && instructionPage > 0)
                        {
                            button4.hovered = false;
                            button5.hovered = true;
                        }
                        if (currentInput.right && oldInput.right == false && instructionPage < 5)
                        {
                            button4.hovered = false;
                            button6.hovered = true;
                        }
                        if (currentInput.action && oldInput.action == false)
                        {
                            button4.Pressed(this);
                            instructionPage = 0;
                        }
                    }
                    if (button5.hovered)
                    {
                        if (instructionPage == 0)
                        {
                            button5.hovered = false;
                            button5.color = Color.TransparentBlack;
                            button4.hovered = true;
                        }
                        else
                        {
                            button5.color = Color.Gray;
                        }
                        if (currentInput.right && oldInput.right == false)
                        {
                            button5.hovered = false;
                            button4.hovered = true;
                        }
                        if (currentInput.action && oldInput.action == false)
                        {
                            button5.Pressed(this);
                        }
                    }
                    if (button6.hovered)
                    {
                        if (instructionPage == 4)
                        {
                            button6.hovered = false;
                            button4.hovered = true;
                        }
                        if (currentInput.left && oldInput.left == false)
                        {
                            button6.hovered = false;
                            button4.hovered = true;
                        }
                        if (currentInput.action && oldInput.action == false)
                        {
                            button6.Pressed(this);
                        }
                    }

                    //Makes the arrow buttons unable to be selected at the first and last page
                    if (instructionPage == 4)
                    {
                        button6.color = Color.TransparentBlack;
                    }
                    else
                    {
                        button6.color = Color.Gray;
                    }
                    if (instructionPage == 0)
                    {
                        button5.color = Color.TransparentBlack;
                    }
                    else
                    {
                        button5.color = Color.Gray;
                    }
                    break;
                //Cutscene which plays when starting a game
                case GameState.Cutscene1:
                    if (introPlayer.State == MediaState.Stopped)
                    {
                        if (!m_introPlayed)
                        {
                            introPlayer.Play(introVideo);
                            m_introPlayed = true;
                        }
                        else
                        {
                            gameState = GameState.Playing;
                            m_introPlayed = false;
                        }
                    }
                    if (introInstance == null)
                    {
                        introInstance = introSong.CreateInstance();
                        introInstance.Volume = 0.5f;
                        introInstance.Play();
                    }
                    break;
                case GameState.Playing:
                    //Plays in-game song
                    if (playLoopInstance == null)
                    {
                        introInstance = null;
                        playLoopInstance = playLoop.CreateInstance();
                        playLoopInstance.Volume = 0.5f;
                        playLoopInstance.IsLooped = true;
                        playLoopInstance.Play();
                    }

#if DEBUG
                    //Toggles debug
                    if (currentInput.debug && oldInput.debug == false)
                    {
                        if (m_debugShow)
                        {
                            m_debugShow = false;
                        }
                        else
                        {
                            m_debugShow = true;
                        }
                    }
#endif

                    //Toggles pause state
                    if (currentInput.pause && oldInput.pause == false)
                    {
                        //If the game is already paused
                        if (paused)
                        {
                            paused = false;

                            //If sounds were playing when game was paused, resume them
                            if (playLoopInstance != null)
                            {
                                if (playLoopInstance.State == SoundState.Paused)
                                {
                                    playLoopInstance.Resume();
                                }
                            }
                            if (axeLoopInstance != null)
                            {
                                if (axeLoopInstance.State == SoundState.Paused)
                                {
                                    axeLoopInstance.Resume();
                                }
                            }
                            if (deathSongInstance != null)
                            {
                                if (deathSongInstance.State == SoundState.Paused)
                                {
                                    deathSongInstance.Resume();
                                }
                            }
                        }
                        //If game is not paused
                        else
                        {
                            paused = true;
                            //Pause any playing sounds
                            if (playLoopInstance != null)
                            {
                                if (playLoopInstance.State == SoundState.Playing)
                                {
                                    playLoopInstance.Pause();
                                }
                            }
                            if (axeLoopInstance != null)
                            {
                                if (axeLoopInstance.State == SoundState.Playing)
                                {
                                    axeLoopInstance.Pause();
                                }
                            }
                            if (deathSongInstance != null)
                            {
                                if (deathSongInstance.State == SoundState.Playing)
                                {
                                    deathSongInstance.Pause();
                                }
                            }
                        }
                    }

                    //Triggers the game to reset if Barry dies
                    if (player.reset && lives > 0)
                    {
                        Reset();
                    }
                    else if (player.reset)
                    {
                        FullReset();
                    }

                    if (paused == false)
                    {
                        if (player.animState != Barry.AnimState.Death)
                        {
                            #region New Class Updates
                            m_currentScoreCollision = false; //Resets the flag which tells the game to increase score when a bone is jumped over

                            #region Bone Updates
                            for (int i = 0; i < bones.Count; i++)
                            {
                                bones[i].Update(gameTime, background, RNG, gameValues);
                                //Deactivates bone if collides with axe 
                                if (bones[i].playerCollision.Intersects(player.axeCollision) && player.holdingAxe)
                                {
                                    bones[i].isActive = false;
                                    point.Play();
                                    gameValues.score += 300;
                                    scores.Add(new Scores(Content.Load<Texture2D>("Textures\\300"), bones[i].groundCollision.Center.X, bones[i].groundCollision.Center.Y)); //Add score sprite
                                }
                                //Kill player if collision intersects player
                                if (bones[i].playerCollision.Intersects(player.collision) && bones[i].isActive)
                                {
                                    playLoopInstance.Stop();
                                    if (axeLoopInstance != null)
                                    {
                                        axeLoopInstance.Stop();
                                    }

                                    player.animState = Barry.AnimState.Death;
                                    lives -= 1;
                                }
                                //Set a flag to true if player jumps over a bone
                                if (bones[i].scoreRectangle.Intersects(player.collision) && (player.animState == Barry.AnimState.JumpingLeft || player.animState == Barry.AnimState.JumpingRight) && bones[i].animState != Bone.State.Ladder)
                                {
                                    m_currentScoreCollision = true;
                                }
                            }
                            #endregion

                            #region SpecialBone Updates
                            for (int i = 0; i < specialBones.Count; i++)
                            {
                                specialBones[i].Update(gameTime, background, this);

                                //Kill player if collision intersects player
                                if (specialBones[i].collision.Intersects(player.collision))
                                {
                                    playLoopInstance.Stop();
                                    if (axeLoopInstance != null)
                                    {
                                        axeLoopInstance.Stop();
                                    }
                                    player.animState = Barry.AnimState.Death;
                                    lives -= 1;
                                }
                            }
                            #endregion

                            #region Skull
                            for (int i = 0; i < skulls.Count; i++)
                            {
                                skulls[i].Update(background, RNG, graphics.PreferredBackBufferWidth, player);

                                //Kill player if collision intersects player
                                if (skulls[i].collision.Intersects(player.collision))
                                {
                                    playLoopInstance.Stop();
                                    if (axeLoopInstance != null)
                                    {
                                        axeLoopInstance.Stop();
                                    }
                                    player.animState = Barry.AnimState.Death;
                                    lives -= 1;
                                }
                                //Deactivates skull if collides with axe 
                                if (skulls[i].collision.Intersects(player.axeCollision) && player.holdingAxe)
                                {
                                    skulls[i].isActive = false;
                                    point.Play();
                                    gameValues.score += 500;
                                    scores.Add(new Scores(Content.Load<Texture2D>("Textures\\500"), skulls[i].collision.Center.X, skulls[i].collision.Center.Y));
                                }
                            }
                            #endregion

                            //Adds score for jumping over bone if flags are correctly set. This stops multiple points from being gained for the same bone
                            if (m_currentScoreCollision && m_oldScoreCollision == false)
                            {
                                point.Play();
                                gameValues.score += 100;
                                scores.Add(new Scores(Content.Load<Texture2D>("Textures\\100"), player.collision.Center.X, player.collision.Center.Y));
                            }

                            //Remove all bones and skulls which are flagged as inactive
                            bones.RemoveAll(b => b.isActive == false);
                            skulls.RemoveAll(s => s.isActive == false);

                            #region Axe Updates
                            for (int i = 0; i < axes.Length; i++)
                            {
                                //Removes axe item if collided with
                                if (axes[i].collision.Intersects(player.collision) && axes[i].isActive)
                                {
                                    axes[i].isActive = false;
                                    player.holdingAxe = true;
                                    playLoopInstance.Stop();

                                    //Play music
                                    if (axeLoopInstance == null)
                                    {
                                        axeLoopInstance = axeLoop.CreateInstance();
                                        axeLoopInstance.Volume = 0.5f;
                                        axeLoopInstance.IsLooped = true;
                                        axeLoopInstance.Play();
                                    }
                                }
                            }
                            #endregion

                            //Stop axe music if player no longer holding axe
                            if (!player.holdingAxe && axeLoopInstance != null)
                            {
                                axeLoopInstance.Stop();
                                axeLoopInstance = null;
                                playLoopInstance = null;
                            }
                            //Play death music if player dead
                            if (player.animState != Barry.AnimState.Death && deathSongInstance != null)
                            {
                                deathSongInstance = null;
                                playLoopInstance = null;
                            }

                            //Score Updates
                            for (int i = 0; i < scores.Count; i++)
                            {
                                scores[i].Update();
                            }
                            scores.RemoveAll(s => s.isActive == false);

                            player.Update(currentInput, oldInput, background, gameValues, graphics.PreferredBackBufferWidth, this);

                            beatrice.UpdateAnimation();

                            //Fire Updates
                            fire.Update(specialBones);
                            fire.UpdateAnimation();
                            if (fire.spawning)
                            {
                                skulls.Add(new Skull(Content.Load<Texture2D>("Textures\\skull"), 78, 379));
                                fire.spawning = false;
                            }

                            #region Boney
                            boney.Update(gameTime, RNG, gameValues);
                            if (boney.boneDrop)
                            {
                                bones.Add(new Bone(Content.Load<Texture2D>("Textures\\boneRoll"), Content.Load<Texture2D>("Textures\\boneRollLadder"), gameValues, 127, 102));
                                boney.boneDrop = false;
                            }
                            if (boney.specialBoneDrop)
                            {
                                specialBones.Add(new SpecialBone(Content.Load<Texture2D>("Textures\\specialBoneLadder"), 65, 80));
                                boney.specialBoneDrop = false;
                            }
                            //If player collides with boney, kill player
                            if (boney.collision.Intersects(player.collision))
                            {
                                playLoopInstance.Stop();
                                if (axeLoopInstance != null)
                                {
                                    axeLoopInstance.Stop();
                                }
                                player.animState = Barry.AnimState.Death;
                            }
                            #endregion


                            #endregion

                            //Resets Barry if the time runs out
                            if (timer <= 0)
                            {
                                playLoopInstance.Stop();
                                if (axeLoopInstance != null)
                                {
                                    axeLoopInstance.Stop();
                                }
                                player.animState = Barry.AnimState.Death;
                            }
                            else
                            {
                                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            m_oldScoreCollision = m_currentScoreCollision;
                        }
                        player.DeathUpdate(this);
                    }
                    break;
                //Cutscene which plays when reaching the top
                case GameState.Cutscene2:
                    if (cutscene2Player.State == MediaState.Stopped)
                    {
                        if (!m_cutscene2Played)
                        {
                            cutscene2Player.Play(cutscene2);
                            m_cutscene2Played = true;
                        }
                        else
                        {
                            gameState = GameState.Playing;
                            m_cutscene2Played = false;
                        }
                    }
                    break;
                //Game over screen
                case GameState.GameOver:
                    if (!m_finalScore)
                    {
                        if (currentInput.action && !oldInput.action)
                        {
                            m_finalScore = true;
                        }
                    }
                    else
                    {
                        if (!m_drum)
                        {
                            drum.Play();
                            m_drum = true;
                        }
                        if (gameValues.score > gameValues.highScore)
                        {
                            gameValues.highScore = gameValues.score;
                            m_highScore = true;
                        }
                        if (m_highScore)
                        {
                            if (finalScoreTimer <= 80)
                            {
                                particleBurst1.Update();
                                particleBurst2.Update();
                                if (applauseInstance == null)
                                {
                                    applauseInstance = applause.CreateInstance();
                                    applauseInstance.Volume = 0.5f;
                                    applauseInstance.Play();
                                }
                                if (highScoreSongInstance == null)
                                {
                                    highScoreSongInstance = highScoreSong.CreateInstance();
                                    highScoreSongInstance.Volume = 0.5f;
                                    highScoreSongInstance.IsLooped = true;
                                    highScoreSongInstance.Play();
                                }
                            }
                        }
                        else
                        {
                            if (finalScoreTimer <= 0)
                            {
                                if (applauseInstance == null)
                                {
                                    applauseInstance = applause.CreateInstance();
                                    applauseInstance.Volume = 0.5f;
                                    applauseInstance.Play();
                                }
                            }
                        }
                        if (currentInput.action && !oldInput.action)
                        {
                            gameValues.score = 0;
                            gameState = GameState.Menu;
                            m_finalScore = false;
                            m_drum = false;
                            applauseInstance.Stop();
                            applauseInstance = null;
                            if (highScoreSongInstance != null)
                            {
                                highScoreSongInstance.Stop();
                                highScoreSongInstance = null;
                            }
                            finalScoreTimer = 180;
                            finalScore.m_position.Y = 0;
                            highScoreBlinkTimer = HIGHSCOREBLINKTIME;
                            particleBurst1.burstDelay = particleBurst1.initialDelay;
                            particleBurst2.burstDelay = particleBurst1.initialDelay;
                            m_highScore = false;
                        }
                    }
                    break;
            }
            gameValues.Update();

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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            switch (gameState)
            {
                case GameState.Menu:
                    menuBackground.Draw(spriteBatch);
                    button1.Draw(spriteBatch);
                    button2.Draw(spriteBatch);
                    button3.Draw(spriteBatch);
                    spriteBatch.DrawString(mainFont, "HIGHSCORE", new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight - 20), Color.White, 0, new Vector2(mainFont.MeasureString("HIGHSCORE").X / 2, mainFont.MeasureString("HIGHSCORE").Y), 1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(mainFont, gameValues.highScore.ToString(), new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight), Color.White, 0, new Vector2(mainFont.MeasureString(gameValues.highScore.ToString()).X / 2, mainFont.MeasureString(gameValues.highScore.ToString()).Y), 1, SpriteEffects.None, 1);
                    break;
                case GameState.Instructions:
                    pages[instructionPage].Draw(spriteBatch);
                    button4.Draw(spriteBatch);
                    button5.Draw(spriteBatch);
                    button6.Draw(spriteBatch);
                    break;
                case GameState.Cutscene1:
                    Texture2D introTexture = null;

                    if (introPlayer.State != MediaState.Stopped)
                    {
                        introTexture = introPlayer.GetTexture();
                    }

                    //Draws the video
                    if (introTexture != null)
                    {
                        spriteBatch.Draw(introTexture, new Rectangle(0, 0, 512, 448), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
                    }
                    break;
                case GameState.Playing:
                    #region New Classes
                    background.Draw(spriteBatch);
                    bonePile.Draw(spriteBatch);
                    player.Draw(spriteBatch, this);
                    boney.Draw(spriteBatch);
                    beatrice.Draw(spriteBatch);
                    fire.Draw(spriteBatch);
                    for (int i = 0; i < bones.Count; i++)
                    {
                        bones[i].Draw(spriteBatch);
                    }
                    for (int i = 0; i < specialBones.Count; i++)
                    {
                        specialBones[i].Draw(spriteBatch);
                    }
                    for (int i = 0; i < skulls.Count; i++)
                    {
                        skulls[i].Draw(spriteBatch, this);
                    }
                    for (int i = 0; i < axes.Length; i++)
                    {
                        axes[i].Draw(spriteBatch);
                    }
                    for (int i = 0; i < scores.Count; i++)
                    {
                        scores[i].Draw(spriteBatch);
                    }
                    #endregion

                    //UI
                    spriteBatch.DrawString(mainFont, "Score", new Vector2(graphics.PreferredBackBufferWidth / 2, 0), Color.White, 0, new Vector2(mainFont.MeasureString("Score").X / 2, 0), 1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(mainFont, gameValues.score.ToString() + "(x" + gameValues.multiplier.ToString() + ")", new Vector2(graphics.PreferredBackBufferWidth / 2, 20), Color.White, 0, new Vector2(mainFont.MeasureString(gameValues.score.ToString() + "(x" + gameValues.multiplier.ToString() + ")").X / 2, 0), 1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(mainFont, "Time", new Vector2(graphics.PreferredBackBufferWidth, 0), Color.White, 0, new Vector2(mainFont.MeasureString("Time").X, 0), 1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(mainFont, ((int)timer).ToString(), new Vector2(graphics.PreferredBackBufferWidth, 20), Color.White, 0, new Vector2(mainFont.MeasureString(((int)timer).ToString()).X, 0), 1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(mainFont, "Lives", new Vector2(0, 0), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    spriteBatch.DrawString(mainFont, lives.ToString(), new Vector2(0, 20), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);

                    if (paused)
                    {
                        pauseScreen.Draw(spriteBatch);
                    }
                    break;
                case GameState.Cutscene2:
                    Texture2D cutscene2Texture = null;

                    if (cutscene2Player.State != MediaState.Stopped)
                    {
                        cutscene2Texture = cutscene2Player.GetTexture();
                    }

                    //Draws the video
                    if (cutscene2Texture != null)
                    {
                        spriteBatch.Draw(cutscene2Texture, new Rectangle(0, 0, 512, 448), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
                    }
                    break;
                case GameState.GameOver:
                    if (!m_finalScore)
                    {
                        gameOver.Draw(spriteBatch);
                    }
                    else
                    {
                        if (m_highScore)
                        {
                            if (finalScoreTimer <= 80)
                            {
                                particleBurst1.Draw(spriteBatch);
                                particleBurst2.Draw(spriteBatch);
                                finalScore.m_position.Y = -20;
                                finalScore.Draw(spriteBatch);
                                spriteBatch.DrawString(mainFont, gameValues.score.ToString(), new Vector2(graphics.PreferredBackBufferWidth / 2, 224), Color.White, 0, new Vector2(mainFont.MeasureString(gameValues.score.ToString()).X / 2, mainFont.MeasureString(gameValues.score.ToString()).Y / 2), 1, SpriteEffects.None, 1);
                                if (highScoreBlinkTimer >= HIGHSCOREBLINKTIME / 2)
                                {
                                    spriteBatch.DrawString(bigFont, "HIGHSCORE", new Vector2(graphics.PreferredBackBufferWidth / 2, 315), Color.White, 0, new Vector2(bigFont.MeasureString("HIGHSCORE").X / 2, bigFont.MeasureString("HIGHSCORE").Y), 1, SpriteEffects.None, 1f);
                                }
                                if (highScoreBlinkTimer <= 0)
                                {
                                    highScoreBlinkTimer = HIGHSCOREBLINKTIME;
                                }
                                highScoreBlinkTimer -= 1;
                            }
                            else
                            {
                                finalScoreTimer -= 1;
                            }
                        }
                        else
                        {
                            if (finalScoreTimer <= 0)
                            {
                                finalScore.Draw(spriteBatch);
                                spriteBatch.DrawString(mainFont, gameValues.score.ToString(), new Vector2(graphics.PreferredBackBufferWidth / 2, 244), Color.White, 0, new Vector2(mainFont.MeasureString(gameValues.score.ToString()).X / 2, mainFont.MeasureString(gameValues.score.ToString()).Y / 2), 1, SpriteEffects.None, 1);
                            }
                            else
                            {
                                finalScoreTimer -= 1;
                            }
                        }
                    }
                    break;
            }

#if DEBUG
            if (m_debugShow)
            {
                background.DebugDraw(spriteBatch, hitBoxTexture);
                for (int i = 0; i < bones.Count; i++)
                {
                    bones[i].DebugDraw(spriteBatch, hitBoxTexture, debugFont);
                }
                for (int i = 0; i < specialBones.Count; i++)
                {
                    specialBones[i].DebugDraw(spriteBatch, hitBoxTexture, debugFont);
                }
                for (int i = 0; i < skulls.Count; i++)
                {
                    skulls[i].DebugDraw(spriteBatch, hitBoxTexture);
                }
                for (int i = 0; i < axes.Length; i++)
                {
                    axes[i].DebugDraw(spriteBatch, hitBoxTexture);
                }
                player.DebugDraw(spriteBatch, hitBoxTexture, debugFont);
                boney.DebugDraw(spriteBatch, hitBoxTexture);
                gameValues.DebugDraw(spriteBatch, debugFont);

                spriteBatch.DrawString(debugFont, "Time: " + timer, new Vector2(0, 8), Color.White);
                spriteBatch.DrawString(debugFont, "Lives: " + lives, new Vector2(0, 32), Color.White);
            }
#endif

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
