using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace TestSheet
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;

		//Static 삼형제
		public static SpriteBatch spriteBatch;
		public static LocalizedContentManager content;
		public Tester tester;

		public static bool GameExit = false;

		public static IntPtr Handler = new IntPtr();
		public static bool ActivationChecker = true;

		public static Color WallColor = Color.Black;

        public static event Action DeadSceneEvent;

        public static void AttachDeadScene(Action a)
        {
            DeadSceneEvent = null;
            DeadSceneEvent += a;
        }

	
		public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
			graphics.PreferredBackBufferWidth = MasterInfo.PreferredScreen.Width;//선호되는 해상도가 있음.
			graphics.PreferredBackBufferHeight = MasterInfo.PreferredScreen.Height;
			graphics.IsFullScreen = true;
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
			Game1.content = new LocalizedContentManager(base.Content.ServiceProvider, base.Content.RootDirectory);
			Standard.LoadContent();
			tester = new Tester();
			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
			if (GameExit)
				Exit();
			// TODO: Add your update logic here
			tester.Update();		
			Standard.Update();
			Standard.OldViewport = GraphicsDevice.Viewport;
			Handler = Window.Handle;
			ActivationChecker = IsActive;
            KeyboardState stat = Keyboard.GetState();
         


            base.Update(gameTime);
		
		}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(WallColor);
			Standard.DrawLight(MasterInfo.FullScreen, Tester.Room.RoomColor, Math.Max(0f,(float)(1-Tester.Score.var / 200.0)), Standard.LightMode.Absolute);
            /*
            Viewport Temp = Game1.graphics.GraphicsDevice.Viewport;
            Game1.graphics.GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
            if (Tester.BeforeEndTimer<Tester.BeforeEndTimer_Max)
                Standard.DrawLight(MasterInfo.FullScreen, Color.White, (float)(Tester.BeforeEndTimer_Max - Tester.BeforeEndTimer) / (float)(Tester.BeforeEndTimer_Max), Standard.LightMode.Absolute);
            Game1.graphics.GraphicsDevice.Viewport = Temp;*/
            // TODO: Add your drawing code here
            tester.Draw();
			Standard.Draw();
			if(Tester.GamePhase==Tester.Phase.Game)
			{
				Color ScoreColor = Color.White;

				if (!Tester.IsEndPhase)
				{
					Standard.DrawString("Bigfont", Tester.Score.ToString() + "/100", new Vector2(Tester.player.GetPos().X, Tester.player.GetPos().Y - 20), ScoreColor);
					/*
					switch (Checker.Bloodthirst)
					{
						case 1:
							Standard.DrawString("Bigfont", Tester.Score.ToString() + "/100", new Vector2(Tester.player.getPos().X, Tester.player.getPos().Y - 20), Color.Red*(float)(Tester.Score/100.0));
							break;
						case 2:
							Standard.DrawString("Bigfont", Tester.Score.ToString() + "/100", new Vector2(Tester.player.getPos().X, Tester.player.getPos().Y - 20), Color.Red * (float)(Tester.Score%75 / 75.0));
							break;
						case 3:
							Standard.DrawString("Bigfont", Tester.Score.ToString() + "/100", new Vector2(Tester.player.getPos().X, Tester.player.getPos().Y - 20), Color.Red * (float)(Tester.Score%50 / 50.0));
							break;
					}*/
					Standard.DrawString("Bigfont", Tester.Score.ToString() + "/100", new Vector2(Tester.player.GetPos().X, Tester.player.GetPos().Y - 20), Color.Red * (float)(Checker.BloodStack));
				}
                Tester.BuffBubble.Draw();
                if (Tester.FreezeTimer >= 0)
				{
					if (Tester.FreezeTimer < 150)
					{
						Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 1f, Standard.LightMode.Absolute);
					}
					/*
					DrawingLayer DeadEnd = new DrawingLayer("DeadEnd2", MasterInfo.FullScreen);
					GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
					if (Tester.FreezeTimer<180)
						DeadEnd.Draw(Color.DarkRed);
					if (Tester.FreezeTimer < 130)
						Standard.DrawAddon(DeadEnd, Color.DarkRed, 1f, "DeadEnd3");
					if (Tester.FreezeTimer < 80)
						Standard.DrawAddon(DeadEnd, Color.DarkRed, 1f, "DeadEnd1");
						*/
				}


				if (Tester.FreezeTimer < 0)
				{
					if (Standard.Pressing(Keys.A))
						Standard.DrawAddon(Tester.player.player, Color.Blue, (float)Tester.HeartSignal * (float)(Standard.FrameTimer % 30 / 8.0), "Player_Heart");
					else
						Standard.DrawAddon(Tester.player.player, Color.White, (float)Tester.HeartSignal * (float)(Standard.FrameTimer % 30 / 8.0), "Player_Heart");
				}
				else if (Tester.FreezeTimer > Tester.FreezeTime - 60)
					Standard.DrawAddon(Tester.player.player, Color.White, (float)Tester.HeartSignal * (float)(Standard.FrameTimer % 30 / 8.0), "HeartBite1");
				else if (Tester.FreezeTimer > Tester.FreezeTime - 110)
					Standard.DrawAddon(Tester.player.player, Color.White, (float)Tester.HeartSignal * (float)(Standard.FrameTimer % 30 / 8.0), "HeartBite2");
                if(Standard.FrameTimer%15==0)
                    Standard.FadeAnimation(new DrawingLayer("Player_Heart", Tester.player.player.GetBound()), 20, Color.Pink);
                if(Tester.Sight<0)
                    Standard.DrawAddon(Tester.player.player, Color.Red, 1f, "Player_Heart");
           
                if (Standard.FrameTimer % 60 == 0)
				{
					if (!Tester.SlowMode)
						Standard.PlayFadedSE("HeartBeat", Math.Min((float)Tester.HeartSignal, 1f));
					else
						Standard.PlayFadedSE("HeartBeat", 1f);
				}
				if (Tester.PressedATimer == 10)
				{
                    if(Checker.Weapon_Melee!=18)
                        Standard.PlayFadedSE("Oveclock", 0.75f);            
                }
                if (Tester.PressedATimer > 0&&Tester.ChainTimer%80==10)
                {
                    if (Checker.Weapon_Melee == 18)
                        Standard.PlayFadedSE("ChainChan", 1f);
                }


                if (!Tester.GameOver)
                {
                    if (Tester.player.player.GetSpriteName() == "Player_Ani_S01")
                        Standard.DrawAddon(Tester.player.player, Color.White, 1f, "MoonLight02");
                    if (Tester.player.player.GetSpriteName() == "Player_Ani_S02")
                        Standard.DrawAddon(Tester.player.player, Color.White, 1f, "MoonLight01");
                }

                if (Tester.GameOver)
				{
					if (Tester.FreezeTimer == Tester.FreezeTime - 110)
                        DeadSceneEvent();
					if (Tester.FreezeTimer > 0 && Tester.FreezeTimer < Tester.FreezeTime - 110)
					{
						GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
						Tester.KillCard.SetRatio(Math.Min((Tester.FreezeTime - 110 - Tester.FreezeTimer) * 6, 75) / 120.0);
						Tester.KillCard.SetCenter(new Point(500, 400));
						Tester.KillCard.Draw(Tester.FixedCamera,Color.White * (float)(Tester.FreezeTimer / 75.0));
                        if(Tester.KillCard.GetSpriteName()=="SDead_11")
                            Standard.DrawAddon(Tester.FixedCamera, Tester.KillCard, Tester.Room.RoomColor, (float)(Tester.FreezeTimer / 75.0), "Sdead_Add");
					}
				}

				if (!Tester.GameOver && !Tester.ShowMenu && Tester.FreezeTimer < 0)
				{
                 

					for (int i = 0; i < Tester.bludgers.Count; i++)
					{
						Tester.bludgers[i].Draw();               
						Standard.DrawAddon(Tester.bludgers[i].bludger, Color.LightYellow, 1f, "BludgerFace");
                        if (Tester.bludgers[i].FrozenTimer > 0 && Standard.FrameTimer % 5 == 0)
                        {
                            Standard.FadeAnimation(new DrawingLayer("frozen", new Rectangle(Tester.bludgers[i].bludger.GetPos(), new Point(100, 100))), 8, Color.FloralWhite);
                            return;
                        }
                        if (!Tester.ShowMenu && !Tester.GameOver && Standard.FrameTimer % 3 == 0)
                        {
                            if (Standard.FrameTimer % 30 < 15)
                                Standard.FadeAnimation(new DrawingLayer("BludgerFire", new Rectangle(Tester.bludgers[i].bludger.GetPos(), new Point(100, 100))), 8, Tester.Bludger.BludgerColor);
                            else
                                Standard.FadeAnimation(new DrawingLayer("BludgerFire2", new Rectangle(Tester.bludgers[i].bludger.GetPos(), new Point(100, 100))), 16, Tester.Bludger.BludgerColor);
                        }
                    }

                }
				Checker.ShowStatus();
                Standard.ViewportSwapDraw(new Viewport(MasterInfo.FullScreen),
    () =>
    {
        if (Tester.BeforeEndTimer < Tester.BeforeEndTimer_Max)
            Standard.DrawLight(MasterInfo.FullScreen, Color.White, (float)(Tester.BeforeEndTimer_Max - Tester.BeforeEndTimer) / (float)(Tester.BeforeEndTimer_Max), Standard.LightMode.Absolute);
    }
 );

            }
            #region DEMOSTRING
            Standard.ViewportSwapDraw(new Viewport(MasterInfo.FullScreen), () => Standard.DrawString(Tester.FixedCamera, "DEMO PLAY", new Vector2(10, 10), Color.White));
            Standard.ViewportSwapDraw(new Viewport(MasterInfo.FullScreen), () => Standard.DrawString(Tester.FixedCamera, HeartShop.HeartCoin.ToString(), new Vector2(200, 10), Color.White));
            #endregion
            base.Draw(gameTime);
         
        }
    }
}
