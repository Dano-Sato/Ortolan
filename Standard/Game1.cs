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
			Standard.OldViewport = GraphicsDevice.Viewport;
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
			Standard.DrawLight(MasterInfo.FullScreen, Tester.Room.RoomColor, Math.Max(0f,(float)(1-Tester.Score/200.0)), Standard.LightMode.Absolute);
			// TODO: Add your drawing code here
			tester.Draw();
			Standard.Draw();
			Color ScoreColor = Color.White;
			
			if(!Tester.IsEndPhase)
			{
				Standard.DrawString("Bigfont", Tester.Score.ToString() + "/100", new Vector2(Tester.player.getPos().X, Tester.player.getPos().Y - 20), ScoreColor);
				switch (Tester.LeechLife)
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


				}

			}
			if (Tester.FreezeTimer>=0)
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
			{
				if(Standard.IsKeyDown(Keys.A))
					Standard.DrawAddon(Tester.player.player, Color.Blue, (float)Tester.HeartSignal * (float)(Standard.FrameTimer % 30 / 8.0), "Player_Heart");
				else
					Standard.DrawAddon(Tester.player.player, Color.White, (float)Tester.HeartSignal * (float)(Standard.FrameTimer % 30 / 8.0), "Player_Heart");
			}
			else if(Tester.FreezeTimer>Tester.FreezeTime-60)
				Standard.DrawAddon(Tester.player.player, Color.White, (float)Tester.HeartSignal * (float)(Standard.FrameTimer % 30 / 8.0), "HeartBite1");
			else if (Tester.FreezeTimer > Tester.FreezeTime-110)
				Standard.DrawAddon(Tester.player.player, Color.White, (float)Tester.HeartSignal * (float)(Standard.FrameTimer % 30 / 8.0), "HeartBite2");


			if (Standard.FrameTimer % 60 == 0)
				Standard.PlayFadedSE("HeartBeat", Math.Min((float)Tester.HeartSignal,1f));
			if (Tester.GameOver)
			{
				

				if (Tester.FreezeTimer > Tester.FreezeTime-110&&Tester.KillerZombieIndex != -1)
				{
						if (Standard.FrameTimer % 20 <= 10)
						Standard.DrawAddon(Tester.enemies[Tester.KillerZombieIndex].enemy, Color.White, 1f, "ZombieBite");
					else
						Standard.DrawAddon(Tester.enemies[Tester.KillerZombieIndex].enemy, Color.White, 1f, "ZombieBite2");

				}

				if(Tester.FreezeTimer==Tester.FreezeTime-110)
				{
					double Rnd = Standard.Random();
					if (Rnd<0.5)
						Tester.KillCard.setSprite("KilldByRock3");
					else
						Tester.KillCard.setSprite("KilldByRock4");

				}
				if (Tester.FreezeTimer > 0 && Tester.FreezeTimer < Tester.FreezeTime-110)
				{
					GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
					Tester.KillCard.SetRatio(Math.Min((Tester.FreezeTime-110-Tester.FreezeTimer)*5,75) / 100.0);
					Tester.KillCard.SetCenter(new Point(500,400));
					Tester.KillCard.Draw(Color.White,(float)(Tester.FreezeTimer/75.0));
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

			Viewport Temp = GraphicsDevice.Viewport;
			GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
			DrawingLayer Heart = new DrawingLayer("Heart", new Rectangle(50, 50, 60,60));
			Color HeartColor = Color.DarkRed;
			int Hearts_5 = Tester.Hearts / 5;
			int LeftHearts = Tester.Hearts % 5;
			for(int i=0;i<Hearts_5;i++)
			{
				Heart.setSprite("Heart5");
				Heart.SetPos(Heart.GetPos().X+80, Heart.GetPos().Y);
				if(Tester.FreezeTimer<0)
				{
					
					Heart.Draw(HeartColor);
					Heart.Draw(Color.White*0.7f);
				}
				else
					Heart.Draw(HeartColor);
			}
			for(int i=0;i<LeftHearts;i++)
			{
				Heart.setSprite("Heart");
				Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
				if (Tester.FreezeTimer < 0)
				{

					Heart.Draw(HeartColor);
					Heart.Draw(Color.White * 0.7f);
				}
				else
					Heart.Draw(HeartColor);
			}
			if(Tester.HeartStack>0)
			{
				if(Tester.HeartTimer>0)
					Tester.HeartTimer--;
				else
				{
					Tester.Hearts += Tester.HeartStack;
					Tester.HeartStack = 0;
				}
				for (int i=0;i<Tester.HeartStack;i++)
				{
					if(Tester.HeartTimer!=0)
						Heart.setSprite("HeartAni"+(30-Tester.HeartTimer)/6);
					else
						Heart.setSprite("Heart");

					Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
					if (Tester.FreezeTimer < 0)
					{

						Heart.Draw(HeartColor);
						Heart.Draw(Color.White * 0.7f);
					}
					else
						Heart.Draw(HeartColor);
					Heart.Draw(Color.Honeydew * (float)(Tester.HeartTimer / 8.0));
			
				}
			}
			if (Tester.FreezeTimer > Tester.FreezeTime - 60)
			{
				Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
				Heart.setSprite("Heart_Broken");
				Heart.Draw(HeartColor);
			}
			else if (Tester.FreezeTimer > Tester.FreezeTime - 110)
			{
				Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
				Heart.setSprite("Heart_Broken2");
				Heart.Draw(HeartColor);
			}
			else if(Tester.FreezeTimer>0)
			{
				Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
				Heart.setSprite("Heart_Broken3");
				Heart.Draw(HeartColor * (float)(Tester.FreezeTimer/(Tester.FreezeTime - 110.0)));
			}
			if (Tester.Room.Number ==0&&!Tester.IsEndPhase)
			{
				DrawingLayer Menual = new DrawingLayer("Menual", new Point(800, 500), 0.75f);
				Menual.Draw(Color.White,(float)(Math.Min(Standard.FrameTimer % 120, 120 - Standard.FrameTimer % 120) / 120.0+0.5f));
			}

			Vector2 InfoVector = new Vector2(130, 150);
			DrawingLayer StringBackGround = new DrawingLayer("WhiteSpace", new Rectangle((int)(InfoVector.X-15), (int)(InfoVector.Y - 10), 170, 35));
			if(Tester.Haste>0)
			{
				string InfoString = "- Haste ";
				for(int i=0;i<Tester.Haste;i++)
				{
					InfoString = InfoString + "I";
				}
				StringBackGround.Draw(Color.Black * 0.5f);
				Standard.DrawString(InfoString, InfoVector, Color.White*(float)(Math.Min(Standard.FrameTimer%240, 240-Standard.FrameTimer % 240) / 120.0+0.3f));
				InfoVector += new Vector2(0, 35);
				
				StringBackGround.SetBound(new Rectangle(StringBackGround.GetBound().X, StringBackGround.GetBound().Y+35, StringBackGround.GetBound().Width, StringBackGround.GetBound().Height));
			}
			if(Tester.LeechLife>0)
			{
				string InfoString = "- Leech Life ";
				for (int i = 0; i < Tester.LeechLife; i++)
				{
					InfoString = InfoString + "I";
				}
				StringBackGround.Draw(Color.Black*0.5f);
				Standard.DrawString(InfoString, InfoVector, Color.White * (float)(Math.Min(Standard.FrameTimer % 240, 240 - Standard.FrameTimer % 240) / 120.0 + 0.3f));
				InfoVector += new Vector2(0, 35);
				
				StringBackGround.SetBound(new Rectangle(StringBackGround.GetBound().X, StringBackGround.GetBound().Y+35, StringBackGround.GetBound().Width, StringBackGround.GetBound().Height));
			}
			if (Checker.Luck > 0)
			{
				string InfoString = "- Luck ";
				for (int i = 0; i < Checker.Luck; i++)
				{
					InfoString = InfoString + "I";
				}
				StringBackGround.Draw(Color.Black * 0.5f);
				Standard.DrawString(InfoString, InfoVector, Color.White * (float)(Math.Min(Standard.FrameTimer % 240, 240 - Standard.FrameTimer % 240) / 120.0 + 0.3f));
				InfoVector += new Vector2(0, 35);
				StringBackGround.SetBound(new Rectangle(StringBackGround.GetBound().X, StringBackGround.GetBound().Y+35, StringBackGround.GetBound().Width, StringBackGround.GetBound().Height));
			}



			GraphicsDevice.Viewport = Temp;
			base.Draw(gameTime);
        }	
    }
}
