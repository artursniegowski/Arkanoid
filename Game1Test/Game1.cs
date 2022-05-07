using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Game1Test
{

    public enum ScoreType
    {
       block_destroyed = 0,
       power_up,
       level_completed
     };

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D bgTexture;

        Paddle paddle;
        //Ball ball;
        List<Ball> balls = new List<Ball>();
        List<Block> blocks = new List<Block>();

        List<PowerUps> Power_Ups = new List<PowerUps>();

        Random random = new Random();
        double propabality_of_Powerup = 0.2;

        public bool Ball_Caught = false; 

        public int Score = 0;
        public float SpeedMultiplayer = 1.0f;

        public int Lives = 3;
        SpriteFont font;
        //layout of the bricks
        //#region
        //int[,] blockLayout = new int[,]{
        //    {5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
        //    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        //    {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
        //    {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        //    {4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
        //    {5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
        //    {0,1,2,3,4,0,1,2,3,4,0,1,2,3,4},
        //  };
        //#endregion

        bool Brake = true;
        float Time_Left_Break = 2.0f;

        int LevelNumber = 0;

       Level level = new Level();

        protected void LoadLevel(string LevelName)
        {
            using (FileStream fs = File.OpenRead("Levels/" + LevelName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Level));
                level = (Level)serializer.Deserialize(fs);
            }

            //genereting blocks based on the level.layout
            for (int rows = 0; rows < level.layout.Length; rows++)
                for (int columns = 0; columns < level.layout[rows].Length; columns++)
                {
                    Block tempBlock = new Block((BlockColor)level.layout[rows][columns], this);
                    if (level.layout[rows][columns] != 9)
                    {
                        tempBlock.LoadContent();
                        tempBlock.position = new Vector2(64 + columns * 64, 100 + 20 * rows);
                        blocks.Add(tempBlock);
                    }
                }
            LevelNumber++;
            Time_Left_Break = 2.0f;
            Brake = true;
            propabality_of_Powerup = level.Luck;
            level.ballspeed = level.ballspeed * SpeedMultiplayer;
        }
        
        
        SoundEffect ballBounceSFX , ballHitSFX, deathSFX, powerUpSFX;


        protected void NextLevel()
        {
            Ball_Caught = false;
            balls.Clear();
            Power_Ups.Clear();
            paddle.position.X = 512;
            paddle.position.Y = 740;
            paddle.Swap_paddle(PaddleType.Paddle_Normal);
            SpawnBall();
            LoadLevel(level.nextLevel);
        }

        protected void AdScore(ScoreType scoreType)
        {
            if(scoreType == ScoreType.block_destroyed)
            {
                Score += 50 + 10 * (int)level.ballspeed/100; 
            }
            else if (scoreType == ScoreType.level_completed)
            {
                Score += 5000 + 300 * (int)level.ballspeed/100;
            }
            else if (scoreType == ScoreType.power_up)
            {
                Score += 500 + 30 * (int)level.ballspeed / 100;
            }
        }


        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;//toggle full screen
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            bgTexture = Content.Load<Texture2D>("background");

            base.Initialize();

           
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            
            // TODO: use this.Content to load your game content here
            paddle = new Paddle(this);
            paddle.LoadContent();
            paddle.position = new Vector2(512, 740);

            //ball = new Ball(this);
            //ball.LoadContent();
            //ball.position = new Vector2(512, paddle.position.Y - paddle.height - ball.height);
            

            //for(int rows = 0 ; rows < blockLayout.GetLength(0) ; rows ++ )
            //    for (int columns = 0; columns < blockLayout.GetLength(1); columns++)
            //{
            //    Block tempBlock = new Block((BlockColor)blockLayout[rows,columns],this);
            //    tempBlock.LoadContent();
            //    tempBlock.position = new Vector2(64 + columns * 64, 100 + 20 * rows);
            //    blocks.Add(tempBlock);
            //}

            LoadLevel("Level1.xml");

            //if (Brake == false)
            {
                SpawnBall();
            }

            font = Content.Load<SpriteFont>("main_font");


            ballBounceSFX = Content.Load<SoundEffect>("ball_bounce");
            ballHitSFX = Content.Load<SoundEffect>("ball_hit");
            deathSFX = Content.Load<SoundEffect>("death");
            powerUpSFX = Content.Load<SoundEffect>("powerup");
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) )
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                Lives++;

            if(Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                if(blocks.Count > 0)
                {
                    blocks.RemoveAt(0);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                if (balls.Count > 0)
                {
                    balls.RemoveAt(0);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (balls.Count > 0)
                {
                    level.ballspeed += 1;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (balls.Count > 0)
                {
                    level.ballspeed -= 1;
                }
            }


            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                
            if (Brake == false)
            {
                // TODO: Add your update logic here
               paddle.Update(deltaTime);
                foreach (Ball ball_one in balls)
                {
                    ball_one.Update(deltaTime);
                    if (ball_one.caught)
                    {
                        ball_one.Check_catch(paddle.position);
                    }
                    CheckCollisions(ball_one);
                    BlockCollision(ball_one);
                }
                foreach (PowerUps pow in Power_Ups)
                {
                    pow.Update(deltaTime);
                }

                if(Power_Ups.Count>0)
                CheckForPowerUps();

                RemovePowerUPs();
                SetBallsSpeed(level.ballspeed);

                if (balls.Count == 0)
                {
                    LostLife();
                }
                else if (balls.Count > 0)
                {
                    for (int i = 0; i < balls.Count; i++)
                        RemoveBalls(balls[i]);
                }

                if (blocks.Count == 0)
                {
                    AdScore(ScoreType.level_completed);
                    NextLevel();
                }
               
            }
            else
            {
                Time_Left_Break -= deltaTime;
               if(Time_Left_Break <= 0.0 )
               {
                   Brake = false;
               }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);
            
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            //draw all sprites
            spriteBatch.Draw(bgTexture, new Vector2(0, 0), Color.White);
            paddle.Draw(spriteBatch);

            foreach (Ball ball_one in balls)
            {
                ball_one.Draw(spriteBatch);
            }
            foreach(Block b in blocks)
            {
                b.Draw(spriteBatch);
            }

            foreach(PowerUps pow in Power_Ups)
            {
                pow.Draw(spriteBatch);
            }

            spriteBatch.DrawString(font, String.Format("Score: {0}", Score), new Vector2(40, 50), Color.White);

            spriteBatch.DrawString(font, String.Format("Lives: {0}", Lives), new Vector2(830, 50), Color.White);

            if(Lives == 0)
            {
                spriteBatch.DrawString(font, String.Format("GAME OVER !!!"), new Vector2(450, 384), Color.White);
                if (Power_Ups.Count > 0)
                    for (int i = 0; i < Power_Ups.Count; i++ )
                    {
                        Power_Ups.RemoveAt(i);
                    }
            }

            if(Brake)
            {
                spriteBatch.DrawString(font, String.Format("Level: {0}", LevelNumber), new Vector2(450, 380), Color.White);

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        
        //bool ball_bounced = false;
        private void CheckCollisions(Ball ball)
        {
            float radius = ball.width / 2;
            
            
            float MinXBoundPanddle = paddle.position.X - radius - paddle.width / 2;
            float MaxXBoundPanddle = paddle.position.X + radius + paddle.width / 2;
            float Total = MaxXBoundPanddle - MinXBoundPanddle;

            //Paddle colison with ball
            if ((ball.position.X >= (paddle.position.X - radius - paddle.width / 2)) &&
               (ball.position.X <= (paddle.position.X + radius + paddle.width / 2)) &&
               (ball.position.Y >= (paddle.position.Y - radius - paddle.height / 2)) &&
               (ball.position.Y <= paddle.position.Y))
            //if (Math.Abs(paddle.position.X - ball.position.X) <= (paddle.width / 2 + ball.width / 2) &&
            //   Math.Abs(paddle.position.Y - ball.position.Y) <= (paddle.height / 2 + ball.height / 2))
            {


                ball.direction.Y = -ball.direction.Y;
                 
                 if(ball.caught == false)
                 {
                     ballBounceSFX.Play();
                 }
                
                 
                 /*
                 //Left third of the paddel
                if ((ball.position.X > MinXBoundPanddle) && (ball.position.X <= (MinXBoundPanddle + Total / 3)))
                {
                    ball.direction = Vector2.Reflect(ball.direction, new Vector2(-0.196f, -0.981f));

                }
                    //center of the paddel
                else if ((ball.position.X > (MinXBoundPanddle + Total / 3)) && (ball.position.X <= (MinXBoundPanddle + Total * 2 / 3)))
                {
                    ball.direction = Vector2.Reflect(ball.direction, new Vector2(0, -1));
                }
                    //right side of the paddel
                else if ((ball.position.X > (MinXBoundPanddle + Total * 2 / 3)) && (ball.position.X <MaxXBoundPanddle ))
                {
                    ball.direction = Vector2.Reflect(ball.direction, new Vector2(0.196f, -0.981f));
                }
                 */

                 if (Ball_Caught)
                {
                    ball.caught = true;
                    Ball_Caught = false;

                }
                 
            }

            if(System.Math.Abs(ball.position.X - 32) <= radius)
            {
                ballBounceSFX.Play();
                //Left wall collision
                ball.direction.X = -ball.direction.X;
            }
            else if(System.Math.Abs(ball.position.X - 992)<=radius)
            {
                
                ballBounceSFX.Play();
                //Right wall collision
                ball.direction.X = -ball.direction.X;
            }
            else if(System.Math.Abs(ball.position.Y - 32)<=radius)
            {
                ballBounceSFX.Play();
                //Upper wall collision
                ball.direction.Y = -ball.direction.Y;
            }
        }

        private void BlockCollision(Ball ball_one)
        {
            if (blocks.Count > 0)
            {
                for (int i = 0; i < blocks.Count; i++ )
                {
                  
                    if (BlockColDetected(blocks[i],ball_one))
                    {
                        ballHitSFX.Play();

                        if (blocks[i].onHit())
                        {
                            
                            if( random.NextDouble() < propabality_of_Powerup )
                            {
                                SpawnPowerUP(blocks[i].position);
                            }

                            AdScore(ScoreType.block_destroyed);
                            blocks.RemoveAt(i);
                        }
                    }
                }
            }
        }

        private bool BlockColDetected(Block block_to_check, Ball ball)
        {
            //logik for colision detected
            bool ColisionDetected = false;

            /*
            if( ( (block_to_check.position.Y + block_to_check.height/2 + ball.height/2) >= (ball.position.Y) ) &&
                ( (block_to_check.position.Y - block_to_check.height/2 - ball.height/2) <= (ball.position.Y) ) &&
               ( (block_to_check.position.X + block_to_check.width/2 + ball.width/2) >= (ball.position.X))   &&
               ( (block_to_check.position.X - block_to_check.width/2 - ball.width/2) <= (ball.position.X))   )
            {
                ColisionDetected = true;
            }
            */
            //Bottom brick collision
            if (((block_to_check.position.Y + block_to_check.height / 2 + ball.height / 2) >= (ball.position.Y)) &&
              ((block_to_check.position.Y + block_to_check.height / 2 ) <= (ball.position.Y)) &&
             ((block_to_check.position.X + block_to_check.width / 2 + ball.width / 2) >= (ball.position.X)) &&
             ((block_to_check.position.X - block_to_check.width / 2 - ball.width / 2) <= (ball.position.X)))
            {
                ColisionDetected = true;
                ball.direction.Y = -ball.direction.Y;
            }//upper brick collision
            else if (((block_to_check.position.Y - block_to_check.height / 2 ) >= (ball.position.Y)) &&
              ((block_to_check.position.Y - block_to_check.height / 2 - ball.height/2) <= (ball.position.Y)) &&
             ((block_to_check.position.X + block_to_check.width / 2 + ball.width / 2) >= (ball.position.X)) &&
             ((block_to_check.position.X - block_to_check.width / 2 - ball.width / 2) <= (ball.position.X)))
            {
                ColisionDetected = true;
                ball.direction.Y = -ball.direction.Y;
            }

            //right brick collision
            if (((block_to_check.position.Y + block_to_check.height / 2 + ball.height / 2) >= (ball.position.Y)) &&
                ((block_to_check.position.Y - block_to_check.height / 2 - ball.height / 2) <= (ball.position.Y)) &&
               ((block_to_check.position.X + block_to_check.width / 2 + ball.width / 2) >= (ball.position.X)) &&
               ((block_to_check.position.X + block_to_check.width / 2 ) <= (ball.position.X)))
            {
                ColisionDetected = true;
                ball.direction.X = -ball.direction.X;
            }//left brick collision
            else if (((block_to_check.position.Y + block_to_check.height / 2 + ball.height / 2) >= (ball.position.Y)) &&
                ((block_to_check.position.Y - block_to_check.height / 2 - ball.height / 2) <= (ball.position.Y)) &&
               ((block_to_check.position.X - block_to_check.width / 2 ) >= (ball.position.X)) &&
               ((block_to_check.position.X - block_to_check.width / 2 - ball.width / 2) <= (ball.position.X)))
            {
                ColisionDetected = true;
                ball.direction.X = -ball.direction.X;
            }

          
            return ColisionDetected;
        }


        private void LostLife()
        {
            if (Brake == false)
            {
                if (Lives > 0)
                {
                    Lives--;
                    deathSFX.Play();
                }

                if (Lives > 0)
                    SpawnBall();
            }
                //Lost !!! 
                //deathSFX.Play();
           
                Ball_Caught = false;
                paddle.Swap_paddle(PaddleType.Paddle_Normal);
                
            
        }

        private void RemoveBalls(Ball ball_one)
        {
            if (ball_one.position.Y > (768 + ball_one.width))
            {
                ball_one.ToRemove = true;
                if (ball_one.ToRemove)
                    balls.Remove(ball_one);
            }
        }


        protected void SpawnPowerUP(Vector2 position)
        {
            PowerUps PowerUp_temp = new PowerUps((PowerUpType)random.Next(3), this);
            //PowerUps PowerUp_temp = new PowerUps((PowerUpType)0, this);
            PowerUp_temp.LoadContent();
            PowerUp_temp.position = position;
            Power_Ups.Add(PowerUp_temp);
        }
        
        protected void RemovePowerUPs()
        {
            if(Power_Ups.Count > 0)
            for(int i = 0; i < Power_Ups.Count; i++)
            {
                if(Power_Ups[i].ToRemove)
                Power_Ups.RemoveAt(i);
            }
        }

        protected void CheckForPowerUps()
        {
            foreach(PowerUps pow in Power_Ups)
            {

                if( Math.Abs(paddle.position.X - pow.position.X) < (paddle.width/2+pow.width/2) &&
                    Math.Abs(paddle.position.Y - pow.position.Y) < (paddle.height/2+pow.height/2))
                {
                   AdScore(ScoreType.power_up);
                   ActivatePowerUP(pow);
                   powerUpSFX.Play();
               }
            }
        }

        private void ActivatePowerUP(PowerUps PowUP)
        {
            PowUP.ToRemove = true;
            if(PowUP.powType == PowerUpType.Ball_Catch)
            {            
                Ball_Caught = true; 
            }
            if (PowUP.powType == PowerUpType.Paddle_Size)
            {
                paddle.Swap_paddle(PaddleType.Paddle_Long);
            }
            if (PowUP.powType == PowerUpType.Multi_Ball)
            {
                SpawnBall();
            }
            
        }

        protected void SpawnBall()
        {
            Ball ball = new Ball(this);
            ball.LoadContent();
            ball.position = new Vector2(paddle.position.X, paddle.position.Y - paddle.height - ball.height);
            balls.Add(ball);

            //reseting direction of the ball movement
            ball.direction.X = 0.707f;
            ball.direction.Y = -0.707f;
            //reseting the position of the ball
            ball.position.X = paddle.position.X;
            ball.position.Y = paddle.position.Y - paddle.height - ball.height;

        }

        protected void SetBallsSpeed(float speed)
        {
            foreach(Ball ball in balls)
            {
                ball.speed = speed;
            }
        }
    }
}
