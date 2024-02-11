using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

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
        Dictionary<string, Texture2D> reusedTextures;
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
        Sprite menuBackground, pauseScreen, gameOver, finalScore, bonePile;
        Sprite[] pages;
        List<Scores> scores;
        ButtonManager buttonManager;
        Button[] menuButtons = new Button[3];
        Button[] instructionButtons = new Button[3];
        Level level;
        List<Bone> bones;
        List<SpecialBone> specialBones;
        List<Skull> skulls;
        Axe[] axes;
        Player player;
        BoneKing boney;
        AnimatedSprite beatrice;
        Fire fire;
        GameValues gameValues;
        Input currentInput, oldInput;
        ParticleBurst particleBurst1, particleBurst2;

        //Structures
        bool debugShow, currentScoreCollision, oldScoreCollision, introPlayed, cutscene2Played, finalScoreReady, highScore, drumPlayed;
        public bool paused;
        public float timer, finalScoreTimer, highScoreBlinkTimer;
        int currentButton;
        public int lives, instructionPage;

        const int HIGHSCOREBLINKTIME = 51, PLAYERSTARTX = 80, PLAYERSTARTY = 384;
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

            reusedTextures = new Dictionary<string, Texture2D>();

            menuBackground = new Sprite(Vector2.Zero, 0.1f);
            pauseScreen = new Sprite(Vector2.Zero, 1);
            gameOver = new Sprite(Vector2.Zero, 1);
            finalScore = new Sprite(Vector2.Zero, 0);
            bonePile = new Sprite(new Vector2(0, 62), 0.9f);
            pages = new Sprite[5];
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i] = new Sprite(Vector2.Zero, 0.1f);
            }
            menuButtons[0] = new Button(graphics.PreferredBackBufferWidth / 2, 180, ButtonEffect.Play)
            {
                hovered = true
            };
            menuButtons[1] = new Button(graphics.PreferredBackBufferWidth / 2, 260, ButtonEffect.Instructions);
            menuButtons[2] = new Button(graphics.PreferredBackBufferWidth / 2, 340, ButtonEffect.Exit);
            instructionButtons[0] = new Button(18, 18, ButtonEffect.Left);
            instructionButtons[1] = new Button(graphics.PreferredBackBufferWidth / 2, 17, ButtonEffect.Menu);
            instructionButtons[2] = new Button(graphics.PreferredBackBufferWidth - 18, 18, ButtonEffect.Right);

            bones = new List<Bone>();
            specialBones = new List<SpecialBone>();
            skulls = new List<Skull>();
            axes = new Axe[4];
            scores = new List<Scores>();
            boney = new BoneKing(42, 41);
            beatrice = new AnimatedSprite(new Vector2(169, 35), 30, 1, new List<Vector2>() {new Vector2(30, 44)});
            fire = new Fire(78, 379, 30, 36, 15, 0.8f);

            player = new Player(PLAYERSTARTX, PLAYERSTARTY);

            buttonManager = ButtonManager.Instance(this);

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
            menuBackground.Load(Content.Load<Texture2D>("Textures\\menubackground"));
            pauseScreen.Load(Content.Load<Texture2D>("Textures\\pausescreen"));
            gameOver.Load(Content.Load<Texture2D>("Textures\\gameoverscreen"));
            finalScore.Load(Content.Load<Texture2D>("Textures\\finalscore"));
            bonePile.Load(Content.Load<Texture2D>("Textures\\bonepile"));
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i].Load(Content.Load<Texture2D>("Textures\\page-" + i));
            }
            menuButtons[0].Load(Content.Load<Texture2D>("Textures\\playbutton"));
            menuButtons[1].Load(Content.Load<Texture2D>("Textures\\instructionsbutton"));
            menuButtons[2].Load(Content.Load<Texture2D>("Textures\\exitbutton"));
            instructionButtons[0].Load(Content.Load<Texture2D>("Textures\\arrowflipped"));
            instructionButtons[1].Load(Content.Load<Texture2D>("Textures\\menubutton"));
            instructionButtons[2].Load(Content.Load<Texture2D>("Textures\\arrow"));
            level = new Level(Content.Load<Texture2D>("Textures\\background"));
            player.Load(new List<Texture2D>{
                Content.Load<Texture2D>("Textures\\barryRun"),
                Content.Load<Texture2D>("Textures\\barryRunWithAxe"),
                Content.Load<Texture2D>("Textures\\barryJump"),
                Content.Load<Texture2D>("Textures\\barryClimb"),
                Content.Load<Texture2D>("Textures\\barryClimbOver"),
                Content.Load<Texture2D>("Textures\\barryDeath")});
            boney.Load(new List<Texture2D>{
                Content.Load<Texture2D>("Textures\\standingBoneKing"),
                Content.Load<Texture2D>("Textures\\angeryBoneKing"),
                Content.Load<Texture2D>("Textures\\throwingBoneKing"),
                Content.Load<Texture2D>("Textures\\specialBoneKing") });
            beatrice.Load(Content.Load<Texture2D>("Textures\\beatrice"));
            fire.Load(Content.Load<Texture2D>("Textures\\fire"));
            axes[0] = new Axe(Content.Load<Texture2D>("Textures\\axe"), 24, 310);
            axes[1] = new Axe(Content.Load<Texture2D>("Textures\\axe"), 470, 250);
            axes[2] = new Axe(Content.Load<Texture2D>("Textures\\axe"), 24, 190);
            axes[3] = new Axe(Content.Load<Texture2D>("Textures\\axe"), 470, 130);
            int random = RNG.Next(0, 4);
            axes[random].active = true;

            reusedTextures.Add("boneRoll", Content.Load<Texture2D>("Textures\\boneRoll"));
            reusedTextures.Add("boneRollLadder", Content.Load<Texture2D>("Textures\\boneRollLadder"));
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
                bones[i].active = false;
            }
            for (int i = 0; i < specialBones.Count; i++)
            {
                specialBones[i].active = false;
            }
            for (int i = 0; i < skulls.Count; i++)
            {
                skulls[i].active = false;
            }
            for (int i = 0; i < axes.Length; i++)
            {
                axes[i].active = false;
            }
            for (int i = 0; i < scores.Count; i++)
            {
                scores[i].active = false;
            }
            #endregion

            bones.RemoveAll(b => b.active == false);
            specialBones.RemoveAll(sb => sb.active == false);
            skulls.RemoveAll(s => s.active == false);
            scores.RemoveAll(s => s.active == false);

            if (player.collision.Intersects(level.goal))
            {
                gameState = GameState.Cutscene2;
            }
            player.Reset(PLAYERSTARTX, PLAYERSTARTY);
            boney.Reset(gameValues);
            fire.Reset();

            timer = gameValues.initialTime;

            int random = RNG.Next(0, 4);
            axes[random].active = true;
        }

        //Called when player dies and has no remaining lives
        public void FullReset()
        {
            #region Lists and Arrays
            for (int i = 0; i < bones.Count; i++)
            {
                bones[i].active = false;
            }
            for (int i = 0; i < specialBones.Count; i++)
            {
                specialBones[i].active = false;
            }
            for (int i = 0; i < skulls.Count; i++)
            {
                skulls[i].active = false;
            }
            for (int i = 0; i < axes.Length; i++)
            {
                axes[i].active = false;
            }
            for (int i = 0; i < scores.Count; i++)
            {
                scores[i].active = false;
            }
            #endregion

            bones.RemoveAll(b => b.active == false);
            specialBones.RemoveAll(sb => sb.active == false);
            skulls.RemoveAll(s => s.active == false);
            scores.RemoveAll(s => s.active == false);

            player.Reset(PLAYERSTARTX, PLAYERSTARTY);
            boney.Reset(gameValues);
            fire.Reset();

            timer = gameValues.initialTime;

            int potato = RNG.Next(0, 4);
            axes[potato].active = true;

            debugShow = false;
            currentScoreCollision = false;
            oldScoreCollision = false;
            lives = 3;
            gameValues.level = 0;
            gameValues.score = (int)(gameValues.score * gameValues.multiplier);

            gameState = GameState.GameOver;
        }

        public void GoToInstructions()
        {
            currentButton = 1;
            instructionButtons[0].hovered = false;
            instructionButtons[0].active = false;
            instructionButtons[1].hovered = true;
            instructionButtons[2].hovered = false;
            gameState = GameState.Instructions;
        }

        public void InstructionsChangePage(int change)
        {
            instructionPage += change;
            if (instructionPage == 4)
            {
                instructionButtons[2].active = false;
                instructionButtons[2].hovered = false;
                instructionButtons[1].hovered = true;
                currentButton = 1;
            }
            else
                instructionButtons[2].active = true;

            if (instructionPage == 0)
            {
                instructionButtons[0].active = false;
                instructionButtons[0].hovered = false;
                instructionButtons[1].hovered = true;
                currentButton = 1;
            }
            else
                instructionButtons[0].active = true;
        }

        public void GoToMenu()
        {
            currentButton = 0;
            menuButtons[0].hovered = true;
            menuButtons[1].hovered = false;
            menuButtons[2].hovered = false;
            gameState = GameState.Menu;
        }

        protected override void Update(GameTime gameTime)
        {
            currentInput.Update();
            switch (gameState)
            {
                case GameState.Menu:
                    for (int i = 0; i < menuButtons.Length; i++)
                    {
                        menuButtons[i].Update();
                    }

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

                    //Handle Buttons
                    if (currentInput.up && !oldInput.up)
                    {
                        int newButton = currentButton - 1;
                        if (newButton < 0)
                            newButton = 2;
                        if (menuButtons[newButton].active)
                        {
                            menuButtons[currentButton].hovered = false;
                            menuButtons[newButton].hovered = true;
                            currentButton = newButton;
                        }
                    }
                    if (currentInput.down && !oldInput.down)
                    {
                        int newButton = currentButton + 1;
                        if (newButton > 2)
                            newButton = 0;
                        if (menuButtons[newButton].active)
                        {
                            menuButtons[currentButton].hovered = false;
                            menuButtons[newButton].hovered = true;
                            currentButton = newButton;
                        }
                    }
                    if (currentInput.action && !oldInput.action)
                    {
                        buttonManager.PressButton(menuButtons[currentButton].effect);
                    }
                    break;
                case GameState.Instructions:
                    for (int i = 0; i < instructionButtons.Length; i++)
                    {
                        instructionButtons[i].Update();
                    }

                    //Handle buttons
                    if (currentInput.left && !oldInput.left)
                    {
                        int newButton = currentButton - 1;
                        if (newButton < 0)
                            newButton = 2;
                        if (instructionButtons[newButton].active)
                        {
                            instructionButtons[currentButton].hovered = false;
                            instructionButtons[newButton].hovered = true;
                            currentButton = newButton;
                        }
                    }
                    if (currentInput.right && !oldInput.right)
                    {
                        int newButton = currentButton + 1;
                        if (newButton > 2)
                            newButton = 0;
                        if (instructionButtons[newButton].active)
                        {
                            instructionButtons[currentButton].hovered = false;
                            instructionButtons[newButton].hovered = true;
                            currentButton = newButton;
                        }
                    }
                    if (currentInput.action && !oldInput.action)
                    {
                        buttonManager.PressButton(instructionButtons[currentButton].effect);
                    }

                    break;
                //Cutscene which plays when starting a game
                case GameState.Cutscene1:
                    menuSongInstance.Stop();
                    if (introPlayer.State == MediaState.Stopped)
                    {
                        if (!introPlayed)
                        {
                            introPlayer.Play(introVideo);
                            introPlayed = true;
                        }
                        else
                        {
                            gameState = GameState.Playing;
                            introPlayed = false;
                        }
                    }
                    if (introInstance == null)
                    {
                        introInstance = introSong.CreateInstance();
                        introInstance.Volume = 0.5f;
                        introInstance.Play();
                    }
                    if (currentInput.pause && introPlayed)
                    {
                        introPlayer.Stop();
                        introInstance.Stop();
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
                        if (debugShow)
                        {
                            debugShow = false;
                        }
                        else
                        {
                            debugShow = true;
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
                        if (player.state != Player.State.Death)
                        {
                            #region New Class Updates
                            currentScoreCollision = false; //Resets the flag which tells the game to increase score when a bone is jumped over

                            #region Bone Updates
                            for (int i = 0; i < bones.Count; i++)
                            {
                                bones[i].Update(level, RNG, gameValues.multiplier);
                                //Deactivates bone if collides with axe 
                                if (bones[i].playerCollision.Intersects(player.axeCollision) && player.holdingAxe)
                                {
                                    bones[i].active = false;
                                    point.Play();
                                    gameValues.score += 300;
                                    scores.Add(new Scores(Content.Load<Texture2D>("Textures\\300"), bones[i].groundCollision.Center.X, bones[i].groundCollision.Center.Y)); //Add score sprite
                                }
                                //Kill player if collision intersects player
                                if (bones[i].playerCollision.Intersects(player.collision) && bones[i].active)
                                {
                                    playLoopInstance.Stop();
                                    if (axeLoopInstance != null)
                                    {
                                        axeLoopInstance.Stop();
                                    }

                                    player.state = Player.State.Death;
                                    lives -= 1;
                                }
                                //Set a flag to true if player jumps over a bone
                                if (bones[i].scoreCollision.Intersects(player.collision) && player.state == Player.State.Jumping)
                                {
                                    currentScoreCollision = true;
                                }
                            }
                            #endregion

                            #region SpecialBone Updates
                            for (int i = 0; i < specialBones.Count; i++)
                            {
                                specialBones[i].Update(level.platformHitBoxes, bang);

                                //Kill player if collision intersects player
                                if (specialBones[i].Collision.Intersects(player.collision))
                                {
                                    playLoopInstance.Stop();
                                    if (axeLoopInstance != null)
                                    {
                                        axeLoopInstance.Stop();
                                    }
                                    player.state = Player.State.Death;
                                    lives -= 1;
                                }
                            }
                            #endregion

                            #region Skull
                            for (int i = 0; i < skulls.Count; i++)
                            {
                                skulls[i].Update(level, graphics.PreferredBackBufferWidth, player);

                                //Kill player if collision intersects player
                                if (skulls[i].collision.Intersects(player.collision))
                                {
                                    playLoopInstance.Stop();
                                    if (axeLoopInstance != null)
                                    {
                                        axeLoopInstance.Stop();
                                    }
                                    player.state = Player.State.Death;
                                    lives -= 1;
                                }
                                //Deactivates skull if collides with axe 
                                if (skulls[i].collision.Intersects(player.axeCollision) && player.holdingAxe)
                                {
                                    skulls[i].active = false;
                                    point.Play();
                                    gameValues.score += 500;
                                    scores.Add(new Scores(Content.Load<Texture2D>("Textures\\500"), skulls[i].collision.Center.X, skulls[i].collision.Center.Y));
                                }
                            }
                            #endregion

                            //Adds score for jumping over bone if flags are correctly set. This stops multiple points from being gained for the same bone
                            if (currentScoreCollision && oldScoreCollision == false)
                            {
                                point.Play();
                                gameValues.score += 100;
                                scores.Add(new Scores(Content.Load<Texture2D>("Textures\\100"), player.collision.Center.X, player.collision.Center.Y));
                            }

                            //Remove all bones and skulls which are flagged as inactive
                            bones.RemoveAll(b => b.active == false);
                            skulls.RemoveAll(s => s.active == false);

                            #region Axe Updates
                            for (int i = 0; i < axes.Length; i++)
                            {
                                //Removes axe item if collided with
                                if (axes[i].collision.Intersects(player.collision) && axes[i].active)
                                {
                                    axes[i].active = false;
                                    player.GetAxe();
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
                            if (player.state != Player.State.Death && deathSongInstance != null)
                            {
                                deathSongInstance = null;
                                playLoopInstance = null;
                            }

                            //Score Updates
                            for (int i = 0; i < scores.Count; i++)
                            {
                                scores[i].Update();
                            }
                            scores.RemoveAll(s => s.active == false);

                            player.Update(currentInput, oldInput, level, gameValues, graphics.PreferredBackBufferWidth, this);

                            //Fire Updates
                            fire.Update(specialBones);
                            if (fire.spawning)
                            {
                                skulls.Add(new Skull(78, 379, Content.Load<Texture2D>("Textures\\skull")));
                                fire.spawning = false;
                            }

                            #region Boney
                            boney.Update(RNG, gameValues);
                            if (boney.boneDrop)
                            {
                                bones.Add(new Bone(127, 105, new List<Texture2D>() {
                                    reusedTextures["boneRoll"],
                                    reusedTextures["boneRollLadder"]}));
                                boney.boneDrop = false;
                            }
                            if (boney.specialBoneDrop)
                            {
                                specialBones.Add(new SpecialBone(new Vector2(65, 80), Content.Load<Texture2D>("Textures\\specialBoneLadder")));
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
                                player.state = Player.State.Death;
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
                                player.state = Player.State.Death;
                            }
                            else
                            {
                                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            oldScoreCollision = currentScoreCollision;
                        }
                        player.DeathUpdate(this);
                    }
                    break;
                //Cutscene which plays when reaching the top
                case GameState.Cutscene2:
                    if (cutscene2Player.State == MediaState.Stopped)
                    {
                        if (!cutscene2Played)
                        {
                            cutscene2Player.Play(cutscene2);
                            cutscene2Played = true;
                        }
                        else
                        {
                            gameState = GameState.Playing;
                            cutscene2Played = false;
                        }
                    }
                    break;
                //Game over screen
                case GameState.GameOver:
                    if (!finalScoreReady)
                    {
                        if (currentInput.action && !oldInput.action)
                        {
                            finalScoreReady = true;
                        }
                    }
                    else
                    {
                        if (!drumPlayed)
                        {
                            drum.Play();
                            drumPlayed = true;
                        }
                        if (gameValues.score > gameValues.highScore)
                        {
                            gameValues.highScore = gameValues.score;
                            highScore = true;
                        }
                        if (highScore)
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
                            finalScoreReady = false;
                            drumPlayed = false;
                            applauseInstance.Stop();
                            applauseInstance = null;
                            if (highScoreSongInstance != null)
                            {
                                highScoreSongInstance.Stop();
                                highScoreSongInstance = null;
                            }
                            finalScoreTimer = 180;
                            finalScore.position.Y = 0;
                            highScoreBlinkTimer = HIGHSCOREBLINKTIME;
                            particleBurst1.burstDelay = particleBurst1.initialDelay;
                            particleBurst2.burstDelay = particleBurst1.initialDelay;
                            highScore = false;
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
                    for (int i = 0; i < menuButtons.Length; i++)
                        menuButtons[i].Draw(spriteBatch);
                    spriteBatch.DrawString(mainFont, "HIGHSCORE", new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight - 20), Color.White, 0, new Vector2(mainFont.MeasureString("HIGHSCORE").X / 2, mainFont.MeasureString("HIGHSCORE").Y), 1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(mainFont, gameValues.highScore.ToString(), new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight), Color.White, 0, new Vector2(mainFont.MeasureString(gameValues.highScore.ToString()).X / 2, mainFont.MeasureString(gameValues.highScore.ToString()).Y), 1, SpriteEffects.None, 1);
                    break;
                case GameState.Instructions:
                    pages[instructionPage].Draw(spriteBatch);
                    for (int i = 0; i < menuButtons.Length; i++)
                        instructionButtons[i].Draw(spriteBatch);
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
                    level.Draw(spriteBatch);
                    bonePile.Draw(spriteBatch);
                    player.Draw(spriteBatch);
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
                        skulls[i].Draw(spriteBatch);
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
                    if (!finalScoreReady)
                    {
                        gameOver.Draw(spriteBatch);
                    }
                    else
                    {
                        if (highScore)
                        {
                            if (finalScoreTimer <= 80)
                            {
                                particleBurst1.Draw(spriteBatch);
                                particleBurst2.Draw(spriteBatch);
                                finalScore.position.Y = -20;
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
            if (debugShow)
            {
                level.DebugDraw(spriteBatch, hitBoxTexture);
                for (int i = 0; i < bones.Count; i++)
                {
                    bones[i].DebugDraw(spriteBatch, hitBoxTexture);
                }
                for (int i = 0; i < specialBones.Count; i++)
                {
                    specialBones[i].DebugDraw(spriteBatch, hitBoxTexture);
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
