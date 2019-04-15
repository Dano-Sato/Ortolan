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
			GraphicsDevice.Viewport = Standard.Viewport;
			Handler = Window.Handle;
			ActivationChecker = IsActive;
			base.Update(gameTime);
		
		}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
			Standard.DrawLight(MasterInfo.FullScreen, MasterInfo.ThemeColor, Math.Max(0f,(float)(1-Tester.Score/200.0)), Standard.LightMode.Absolute);
			// TODO: Add your drawing code here
			tester.Draw();
			Standard.Draw();
			Color ScoreColor = Color.White;
			if (Standard.IsPrimeNumber(Tester.Score))
				ScoreColor = Color.Silver;
			Standard.DrawString("Bigfont", Tester.Score.ToString()+"/100", new Vector2(Tester.player.getPos().X, Tester.player.getPos().Y - 20), ScoreColor);
			if(Tester.FreezeTimer>=0)
			{
				if(Tester.FreezeTimer<150)
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

		
			if(Tester.FreezeTimer<0)
				Standard.DrawAddon(Tester.player.player, Color.White, (float)Tester.HeartSignal*(float)(Standard.FrameTimer%30/8.0), "Player_Heart");
			else if(Tester.FreezeTimer>140)
				Standard.DrawAddon(Tester.player.player, Color.White, (float)Tester.HeartSignal * (float)(Standard.FrameTimer % 30 / 8.0), "HeartBite1");
			else if (Tester.FreezeTimer > 90)
				Standard.DrawAddon(Tester.player.player, Color.White, (float)Tester.HeartSignal * (float)(Standard.FrameTimer % 30 / 8.0), "HeartBite2");


			if (Standard.FrameTimer % 60 == 0)
				Standard.PlaySound("HeartBeat", Math.Min((float)Tester.HeartSignal,1f));
			if (Tester.GameOver)
			{
				

				if (Tester.KillerZombieIndex != -1)
				{
						if (Standard.FrameTimer % 20 <= 10)
						Standard.DrawAddon(Tester.enemies[Tester.KillerZombieIndex].enemy, Color.White, 1f, "ZombieBite");
					else
						Standard.DrawAddon(Tester.enemies[Tester.KillerZombieIndex].enemy, Color.White, 1f, "ZombieBite2");

				}

			}

			bool GhostAnimate = Standard.FrameTimer % 30 < 15;
			if (!Tester.GameOver&&!Tester.ShowMenu&&Tester.FreezeTimer<0)
			{
				for (int i = 0; i < Tester.bludgers.Count; i++)
				{
					Tester.bludgers[i].Draw();
					if (GhostAnimate)
						Standard.DrawAddon(Tester.bludgers[i].bludger, Color.LightYellow, 0.1f, "GhostHead_1");
					else
						Standard.DrawAddon(Tester.bludgers[i].bludger, Color.LightYellow, 0.1f, "GhostHead_2");

					Standard.DrawAddon(Tester.bludgers[i].bludger, Color.LightYellow, 1f, "BludgerFace");

				}
		
			}
			base.Draw(gameTime);
        }	
    }
}
