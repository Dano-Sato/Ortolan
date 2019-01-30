using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace TestSheet
{
	//필요에 따라 여러 개의 테스터 클래스를 만들 수 있습니다. 아마 다음 클래스는 Tester2가 되겠죠.
	public class Tester
	{
		
	

		public static Random random=new Random();
		public static Player player;
		public static List<Enemy> enemies=new List<Enemy>();
		public static bool GameOver=false;
		public static bool ZombieDance = false;
		public static bool isZombieMode = false;
		public static int EndTimer = 0;
		public static int Score = 390;
		public static int ZombieTime = 40;
		public static double Lightr = 0;
		public static string ButtonInfoString = "Right Click";
		public static string SongString;
		public static DrawingLayer LightLayer = new DrawingLayer("Light", MasterInfo.FullScreen);
		public static DrawingLayer LightLayer3 = new DrawingLayer("Player", new Rectangle(0, 0, 150, 150));
		public static DrawingLayer MouseLogo = new DrawingLayer("Mouse", new Rectangle(270, 400, 30, 30));
		public static DrawingLayer MouseButton = new DrawingLayer("Mouse3", new Rectangle(270, 400, 30, 30));
		public static DrawingLayer RealMonoLogo = new DrawingLayer("RealMono", MasterInfo.PreferredScreen);
		public static DrawingLayer BloodLayer= new DrawingLayer("Blood", MasterInfo.FullScreen);

		public static AnimationList animationList = new AnimationList();
		public static AnimationList DeadBodysAnimationList = new AnimationList();
		public static AnimationList AfterImageAnimationList = new AnimationList();
		public static List<DrawingLayer> DeadBodys = new List<DrawingLayer>();
		public static int SoundTrack = 0;
		public static float BaseVolume = 1.0f;
		public static bool GameEnd = false;
		public static bool PressedQ = false;
		public static bool Reload = false;
		public static int BoostTimer = 0;
		public static bool MoveLock = false;
		public static int Interval = 0;
		public static double TempoChecker = 0;
		public static double NewTempo = 0;
		public static double Tempo = 30;
		public static Color ZombieColor = Color.White;
		public EasyMenu TestMenu;
		public static int MainMenuIndex = -1;
		public EasyMenu SubMenu=new EasyMenu(new string[] { },new Rectangle(0,0,0,0),new Rectangle(0,0,0,0),new Point(0,0),new Point(0,0));
		public static bool SubMenuIsMade = false;
		public static int GameMode = 0;//ShutGun Mode;
		public static int ZombieSpeed = 5;
		public static bool CursorShouldBeSword = false;
		//이후 마음대로 인수 혹은 콘텐츠들을 여기 추가할 수 있습니다.
		public Tester()//여기에서 각종 이니셜라이즈가 가능합니다.
		{
			player = new Player();
			enemies.Add(new Enemy());
			Standard.PlaySong(random.Next(0, 5),true);
			TestMenu = new EasyMenu(new string[] {
				"SONG",
				"MODE",
				"TUTORIAL",
				"SETTING",
				"EXIT" },new Rectangle(0,0,300,370),new Rectangle(0,0,200,50),new Point(80,20),new Point(0,70));

		}
		//Game1.Class 내에 Tester.Update()로 추가될 업데이트문입니다. 다양한 업데이트 처리를 시험할 수 있습니다.
		public void Update()
		{
			if (Score<400&&Standard.IsKeyDown(Keys.R))
			{
				Score = 0;
				Standard.FrameTimer = 0;
				ZombieTime = 40;
				enemies.Clear();
				enemies.Add(new Enemy());
				enemies.Add(new Enemy());

				player.reset();
				GameOver = false;
				return;
			}
			if (GameEnd)
			{
				if(EndTimer<1000)
					EndTimer++;
				if(EndTimer==1000)
				{
					Score = 0;
					Standard.FrameTimer = 0;
					ZombieTime = 40;
					enemies.Clear();
					enemies.Add(new Enemy());
					enemies.Add(new Enemy());

					player.reset();
					GameOver = false;
					GameEnd = false;
					ZombieDance = false;
				}
				return;
			}
			if (GameOver)
			{
				if (Standard.IsKeyDown(Keys.R))
				{
					Score = 0;
					Standard.FrameTimer = 0;
					ZombieTime = 40;
					enemies.Clear();
					enemies.Add(new Enemy());
					enemies.Add(new Enemy());

					player.reset();
					GameOver = false;
				}
				return;
			}

			if(Score<10&&Standard.JustPressed(Keys.S))
			{
				Score = 10;
			}

			if(Score<10&&MainMenuIndex!=-1)
			{
				animationList.TimeUpdate();
				switch (MainMenuIndex)
				{
					case 0://Song
						if(!SubMenuIsMade)
						{
							SubMenu = new EasyMenu(new string[] {
								"The Nutcracker: March ~ Tchaikovsky",
								"Polonaise Op.44 ~ Chopin",
								"Ballade No.1~ Chopin",
								"Etude Op.25-2 ~ Chopin",
								"Little Fuga: BWV 578 ~ Bach",
								"Exit" },
	new Rectangle(300, 200, 400, 320), new Rectangle(0, 0, 500, 50), new Point(80, 20), new Point(0, 70));
							SubMenuIsMade = true;
						}
						SubMenu.Update();
						if(SubMenu.GetIndex()!=-1&&Standard.cursor.didPlayerJustLeftClick())
						{
							if(SubMenu.MenuStringList[SubMenu.GetIndex()]=="Exit")
							{
								MainMenuIndex = -1;
							}
							else
							{
								Standard.PlaySong(SubMenu.GetIndex(), true);
							}
						}
						break;
					case 1: // MODE
						if (!SubMenuIsMade)
						{
							SubMenu = new EasyMenu(new string[] {
								"Shutgun Mode",
								"AttackBoost Mode",
								"Exit" },
	new Rectangle(300, 200, 400, 250), new Rectangle(0, 0, 500, 50), new Point(80, 20), new Point(0, 70));
							SubMenuIsMade = true;
						}
						SubMenu.Update();
						if (SubMenu.GetIndex() != -1 && Standard.cursor.didPlayerJustLeftClick())
						{
							if (SubMenu.MenuStringList[SubMenu.GetIndex()] == "Exit")
							{
								MainMenuIndex = -1;
							}
							else
							{
								GameMode = SubMenu.GetIndex();
								if (GameMode == 0)
									player.SetAttackSpeed(8);
								else if (GameMode == 1)
									player.SetAttackSpeed(15);
							}
						}
						break;

					case 2: // TUTORIAL
						if (!SubMenuIsMade)
						{
							SubMenu = new EasyMenu(new string[] {
								"Tip 1 : You can Move&Attack by \"A\" button&Left-Click&Right-Click",
								"Tip 2 : If Score is over 10, Zombies will attack you.",
								"Tip 3 : If Zombie reaches your center, You die.",
								"Tip 4 : Zombie can't play The Piano",
								"More Tip",
								"Exit" },
	new Rectangle(300, 200, 400, 320), new Rectangle(0, 0, 750, 50), new Point(80, 20), new Point(0, 70));
							SubMenuIsMade = true;
						}
						SubMenu.Update();
						if (SubMenu.GetIndex() != -1 && Standard.cursor.didPlayerJustLeftClick())
						{
							if (SubMenu.MenuStringList[SubMenu.GetIndex()] == "Exit")
							{
								MainMenuIndex = -1;
							}
							if(SubMenu.MenuStringList[SubMenu.GetIndex()]=="More Tip")
							{
								SubMenu = new EasyMenu(new string[] {
								"Tip 5 : Pressing \"R\" Button means \"Reset the game\"",
								"Tip 6 : AttackBoost-Mode gives you boost to your cursor's position.",
								"Tip 7 : Pressing \"S\" Button means \"Skip the Masquerade\"",
								"Go Back",
								"Exit" },
	new Rectangle(300, 200, 400, 320), new Rectangle(0, 0, 750, 50), new Point(80, 20), new Point(0, 70));
								SubMenu.Update();
								return;
							}
							if (SubMenu.MenuStringList[SubMenu.GetIndex()] == "Go Back")
							{
								SubMenuIsMade = false;
								return;
							}

							else
							{
							}
						}
						break;

					case 3: // SETTING
						if (!SubMenuIsMade)
						{
							SubMenu = new EasyMenu(new string[] {
								"Easy = For Beginner",
								"MainGame = Recommended!",
								"Extra = For Extra-Expert",
								"Extra Hard = Maybe no one can clear",
								"NIGHTMARE = DON'T PANIC",
								"Exit" },
	new Rectangle(300, 200, 400, 320), new Rectangle(0, 0, 500, 50), new Point(80, 20), new Point(0, 70));
							SubMenuIsMade = true;
						}
						SubMenu.Update();
						if (SubMenu.GetIndex() != -1 && Standard.cursor.didPlayerJustLeftClick())
						{
							if (SubMenu.MenuStringList[SubMenu.GetIndex()] == "Exit")
							{
								MainMenuIndex = -1;
							}
							else
							{
								ZombieSpeed = 3 + SubMenu.GetIndex()*2;
							}
						}
						break;

				}
				return;
			}
			if (Standard.KeyInputOccurs())
			{
				if(Standard.IsKeyDown(Keys.OemTilde))
				{
					BaseVolume = 0f;
				}
				if (Standard.IsKeyDown(Keys.D1))
				{
					BaseVolume = 0.1f;
				}
				if (Standard.IsKeyDown(Keys.D2))
				{
					BaseVolume = 0.2f;
				}
				if (Standard.IsKeyDown(Keys.D3))
				{
					BaseVolume = 0.3f;
				}
				if (Standard.IsKeyDown(Keys.D4))
				{
					BaseVolume = 0.4f;
				}
				if (Standard.IsKeyDown(Keys.D5))
				{
					BaseVolume = 0.5f;
				}
				if (Standard.IsKeyDown(Keys.D6))
				{
					BaseVolume = 0.6f;
				}
				if (Standard.IsKeyDown(Keys.D7))
				{
					BaseVolume = 0.7f;
				}
				if (Standard.IsKeyDown(Keys.D8))
				{
					BaseVolume = 0.8f;
				}
				if (Standard.IsKeyDown(Keys.D9))
				{
					BaseVolume = 0.9f;
				}
				if (Standard.IsKeyDown(Keys.D0))
				{
					BaseVolume = 1.0f;
				}
				if(Standard.JustPressed(Keys.M))
				{
					MoveLock = !MoveLock;
				}
			}
		
	
			if(CursorShouldBeSword)
				Standard.cursor.SetSprite("Sword");
			else
				Standard.cursor.SetSprite("Cursor");

			player.MoveUpdate();
			LightLayer3.setPosition(player.getPos().X-40+random.Next(-3,3), player.getPos().Y-40 + random.Next(-3,3));
			player.AttackUpdate();
			List<int> RandomInts = new List<int>();
			for(int i=0;i<15;i++)
			{
				RandomInts.Add(random.Next(-300, 300));
			}
			int j = 0 ;
			for (int i = 0; i < enemies.Count; i++)
			{
				if (j >= 14)
					j = 0;
				enemies[i].MoveUpdate(i, RandomInts[j],RandomInts[(j+Standard.FrameTimer)%14]);
				j++;
			}

			animationList.TimeUpdate();
			DeadBodysAnimationList.TimeUpdate();
			AfterImageAnimationList.TimeUpdate();
			ZombieTime = 40 - Score/10;
		
			if(ZombieTime>0)
			{
				if (Standard.FrameTimer % ZombieTime == 0)
				{
					enemies.Add(new Enemy());
					if (enemies.Count > 150&&!player.IsAttacking())
						enemies.RemoveAt(0);
				}
			}
			else if(!ZombieDance)
			{
				ZombieDance = true;
				MouseLogo.setSprite("Cake");
				MouseButton.setSprite("Cake2");
				EndTimer = 3000;
			}

			if (ZombieDance&&EndTimer>0)
				EndTimer--;
			if (ZombieDance && EndTimer == 0)
				GameEnd = true;
		

			if (Score <= 280)
				MediaPlayer.Volume = BaseVolume;
			else if(Score<=300)
			{
				MediaPlayer.Volume = BaseVolume*(float)((300-Score)/20.0);
			}
			else if(Score<=306&&SoundTrack==0)
			{
				SoundTrack = 1;
				Standard.PlaySong((int)Standard.SongNameList.Tchai, false);
			}
			else if(Score<=330)
			{
				MediaPlayer.Volume = Math.Min(BaseVolume*0.6f, MediaPlayer.Volume + BaseVolume * 0.001f);
			}
			else if (Score <= 400)
			{
				MediaPlayer.Volume = Math.Min(BaseVolume * 0.6f, MediaPlayer.Volume + BaseVolume * 0.003f);
			}
			else
			{
				MediaPlayer.Volume = Math.Min(BaseVolume, MediaPlayer.Volume + BaseVolume * 0.003f);
			}

			if (Score == 10)
			{
				ZombieColor = Color.LightSeaGreen;
				if(!isZombieMode)
				{
					isZombieMode = true;
				}
			}

			if (Score % 10 == 9)
				Reload = true;
			if(Score%10==0&&Reload==true)
			{
				Standard.PlaySound("Reload");
				Reload = false;
			}
			if (DeadBodys.Count > 300)
			{
				DeadBodysAnimationList.Add(DeadBodys[0], 30);
				DeadBodys.RemoveAt(0);
			}


			TestMenu.Update();
			if (TestMenu.MouseIsOnFrame())
			{
				TestMenu.MoveTo(-110, 300, 20);
				if(Score<10&&TestMenu.GetIndex()!=-1)
				{
					if (TestMenu.MenuStringList[TestMenu.GetIndex()] == "EXIT")
					{
						TestMenu.MenuList[TestMenu.GetIndex()].drawingLayer.MoveByVector(new Point(random.Next(1, 3), random.Next(1, 3)), random.Next(1,10));
					}
					if (Standard.cursor.didPlayerJustLeftClick())
					{
						if (TestMenu.MenuStringList[TestMenu.GetIndex()] == "EXIT")
						{
							Game1.GameExit = true;
							return;
						}
						MainMenuIndex = TestMenu.GetIndex();
						SubMenuIsMade = false;
					}
				}
			}
			else
			{
				TestMenu.MoveTo(-250, 300, 20);
			}


		}

		//Game1.Class 내에 Tester.Draw()로 추가될 드로우 액션문입니다. 다양한 드로잉을 시험할 수 있습니다.
		public void Draw()
		{
			if(GameEnd)
			{
				Standard.DrawLight(MasterInfo.FullScreen, Color.White, 1.0f, Standard.LightMode.Absolute);
				RealMonoLogo.Draw(Color.White,Math.Min((float)(EndTimer/300.0),1.0f));
				Standard.DrawString("Thank you for playing!", new Vector2(500, 500), Color.Aquamarine);
				return;
			}
			BloodLayer.Draw(Color.LightSeaGreen,Math.Min(10,Score)*0.1f);
			for(int i=0;i<DeadBodys.Count;i++)
			{
				DeadBodys[i].Draw(Color.LightSeaGreen, Math.Min(10, Score) * 0.1f);
			}
			DeadBodysAnimationList.FadeAnimationDraw(Color.LightSeaGreen, 1/30.0);
			MouseLogo.Draw();
			if(Standard.FrameTimer%10<=5&&Score<10)
				MouseButton.Draw(Color.Red);
			if (Standard.FrameTimer % 10 <= 5 && ZombieDance)
				MouseButton.Draw(Color.Crimson);
			Color StringColor = Color.White;
			if (Score >= 10)
				StringColor = Color.Red;

			Standard.DrawString("SCORE : " + Score + "/400", new Vector2(300, 450), StringColor);
			if (Score < 300)
				Standard.DrawString(ButtonInfoString + " to live", new Vector2(300, 400), StringColor);
			else if (Score < 400)
				Standard.DrawString("Live to click", new Vector2(300, 400), StringColor);
			else
				Standard.DrawString("Congratulation!", new Vector2(300, 400), StringColor);
			if (MoveLock)
				Standard.DrawString("* Move Locked", new Vector2(300, 600), StringColor);
			Standard.DrawString("Press \"ESC\" Button to leave", new Vector2(300, 500), StringColor);
	

			LightLayer.Draw();
			//스코어 올라갈수록 보라색을 띈다.
			Standard.DrawLight(MasterInfo.FullScreen, Color.Purple, 0.3f * Math.Min(1.2f, (float)(Score / 100.0)), Standard.LightMode.Absolute);
			if (Score > 200)
				LightLayer3.Draw(Color.AntiqueWhite * 0.15f * Math.Min(5f, (float)((Score - 150.0)/50)));
			if (ZombieDance)
				Standard.DrawLight(MasterInfo.FullScreen, Color.White, (float)((3000 - EndTimer) / 3000.0), Standard.LightMode.Absolute);//3000프레임동안 점점 하얀색으로 밝아진다.


		
			TestMenu.Draw(Score>=10? false:true);
			if (Standard.FrameTimer % 20 == 0)
				MenuLightR = random.Next(0, 5);
			
			if(TestMenu.MouseIsOnFrame())
			{
				if (Score >= 10)
				for (int i=0;i<enemies.Count;i++)
				{
					if(TestMenu.Frame.GetBound().Contains(enemies[i].getCenter()))
					{
						enemies[i].enemy.MoveByVector(new Point(1, 0), 20);
						TestMenu.Frame.MoveByVector(new Point(-1, 0), 45-ZombieSpeed*3);
					}
				}
			}
			if (!TestMenu.MouseIsOnFrame())
			{
				Rectangle MenuLight = new Rectangle(TestMenu.GetPosition(), new Point(300, 80));
				MenuLight.Location = Standard.Add(TestMenu.GetPosition(), new Point(0, MenuLightR * 72));
				Standard.DrawLight(MenuLight, Color.Black, 0.6f, Standard.LightMode.Vignette);
			}
			Standard.DrawLight(TestMenu.Frame, Color.Bisque, 0.6f, Standard.LightMode.Vignette);
			if (Score < 10 && TestMenu.GetIndex() != -1 && TestMenu.MenuStringList[TestMenu.GetIndex()] == "EXIT")
			{
				Standard.DrawLight(TestMenu.MenuList[TestMenu.GetIndex()].drawingLayer, Color.Crimson,0.2f, Standard.LightMode.Absolute);
				Standard.DrawLight(TestMenu.MenuList[TestMenu.GetIndex()].drawingLayer, Color.Red, 0.6f, Standard.LightMode.Vignette);
			}

			player.Draw();
			player.DrawAttack();


			Color AnimationColor= Color.DarkRed;

			animationList.FadeAnimationDraw(Color.DarkRed, 0.2);

			CursorShouldBeSword = false;
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Draw();
			}

		

			if (Score>=10)
			{
				if(Standard.FrameTimer%3==0)
				{
					Lightr = random.NextDouble() / 10.0;
				}
				if(Standard.FrameTimer%250==0)
				{
					if(Score>300)
					{
						Standard.PlaySound("ZombieSound", 0.5f);
					}
					else
					{
						Standard.PlaySound("ZombieSound");
					}
				}
			}

			animationList.FadeAnimationDraw(Color.DarkRed, 0.06f);

			if (Score < 10 && MainMenuIndex != -1&&SubMenu!= null&&SubMenu.MenuList.Count > 0)
			{
				SubMenu.Draw();
				switch (MainMenuIndex)
				{
					case 0:
						Standard.DrawString("Song List", new Vector2(300,200), Color.Black);
						break;
					case 1:
						if(SubMenu!=null&&SubMenu.MenuList.Count>0)
							Standard.DrawString("Mode :" + SubMenu.MenuStringList[GameMode] , new Vector2(300,200), Color.Black);
						break;

					case 2:
						Standard.DrawString("Guide", new Vector2(300, 200), Color.Black);
						break;

					case 3:
						string Difficulty = "";
						for (int i = 0; i < SubMenu.MenuList.Count; i++)
						{
							if (ZombieSpeed == 3 + i * 2)
							{
								Difficulty = SubMenu.MenuStringList[i];
								break;
							}
						}
						Standard.DrawString("Difficulty :" + Difficulty, new Vector2(300, 200), Color.Black);

						break;

				}
				return;
			}



			if (GameOver)
			{
				for(int i=0;i<enemies.Count;i++)
				{
					enemies[i].enemy.MoveTo(player.getPos().X+random.Next(-30,30), player.getPos().Y + random.Next(-30, 30), random.Next(5,15));
				}
				Standard.DrawString("Game Over", new Vector2(500+random.Next(-3,3), 400 + random.Next(-3, 3)), Color.Red);
				Standard.DrawString("Press \"R\" button to restart", new Vector2(500 + random.Next(-3, 3), 450 + random.Next(-3, 3)), Color.Red);
			}

			AfterImageAnimationList.FadeAnimationDraw(Color.White, 0.2 * 0.2);
			if (Score>=10)
			{
				Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 0.2f + (float)Lightr, Standard.LightMode.Absolute);
				Standard.DrawLight(MasterInfo.FullScreen, Color.DarkBlue, 0.3f, Standard.LightMode.Absolute);
			}
			AfterImageAnimationList.FadeAnimationDraw(Color.Cornsilk, 0.2*0.5);
			OldStateOfMouseisOnMenu = TestMenu.Frame.MouseIsOnThis();
		}

		public int MenuLightR = 0;
		public bool OldStateOfMouseisOnMenu = false;

		public class Player
		{
			private DrawingLayer player;
			private DrawingLayer bullet;
			private DrawingLayer direction;
			private DrawingLayer wand;
			private Point MovePoint=new Point(0,0);
			private int Range=160;
			private int MoveSpeed=4;
			private int AttackSpeed = 8;
			private int AttackTimer = 0;
			private int AttackIndex = -1;
			private bool isAttacking=false;


			public void SetAttackSpeed(int s)
			{
				AttackSpeed = s;
			}

			public bool IsAttacking()
			{
				return isAttacking;
			}

			public void reset()
			{
				player.MoveTo(400, 400);
				MovePoint = new Point(0, 0);
				AttackTimer = 0;
				AttackIndex = -1;
				isAttacking = false;
				if(SoundTrack!=0)
				{
					SoundTrack = 0;
					Standard.PlaySong(random.Next(0,5),true);			
				}
			}

			public int getAttackIndex()
			{
				return AttackIndex;
			}

			public Point getPos()
			{
				return player.GetPosition();
			}

			public Point getMovePoint()
			{
				return MovePoint;
			}

			public int getRange()
			{
				return Range;
			}

			public Player()
			{
				player = new DrawingLayer("Player",new Rectangle(400,400,80,80));
				bullet = new DrawingLayer("Player2", new Rectangle(0, 0, 20, 20));
				direction = new DrawingLayer("Player2", new Rectangle(0, 0, 20, 20));
				wand = new DrawingLayer("WhiteSpace", new Rectangle(0, 0, 5, 5));
			}

			public void Draw()
			{
				player.Draw(Color.White);
				for (int i = 0; i < 7; i++)
				{
					wand.setPosition(Standard.DivPoint(player.GetBound().Center, direction.GetBound().Center, i / 10.0));
					wand.setPosition(wand.GetPosition().X - 2, wand.GetPosition().Y - 2);
					wand.Draw(Color.Aqua);
				}
				direction.Draw(Color.White);
				if (isAttacking)
					player.Draw(MasterInfo.PlayerColor*(float)((float)AttackTimer/AttackSpeed));
			}

			public void MoveUpdate()
			{
				if(MovePoint.X==0&&MovePoint.Y==0)
				{
					direction.MoveTo(getPos().X+20, getPos().Y+20, 7);
				}
				
				

				if (isAttacking)
				{
					if (Standard.cursor.didPlayerJustRightClick() || Standard.cursor.didPlayerJustLeftClick() || Standard.JustPressed(Keys.A))
					{
						if (Standard.cursor.didPlayerJustLeftClick())
							ButtonInfoString = "Left Click";
						else if (Standard.cursor.didPlayerJustRightClick())
							ButtonInfoString = "Right Click";
						else if (Standard.JustPressed(Keys.A))
							ButtonInfoString = "Press \"A\"";

						for(int i=0;i<enemies.Count;i++)
						{
							if (enemies[i].getBound().Contains(Standard.cursor.getPos()) && Standard.Distance(getPos(), enemies[i].getPos()) < Range)
							{
								return;
							}
						}
						MovePoint = Standard.cursor.getPos();
						animationList.Add(new DrawingLayer("Click", new Rectangle(MovePoint.X - 15, MovePoint.Y - 15, 30, 30)), 10);
			
					}
					if(GameMode==0)
						MovePoint = new Point(0, 0);
					else if(GameMode==1)
						MovePoint = Standard.DivPoint(player.GetCenter(), Standard.cursor.getPos(), 0.8);
					if (AttackTimer<AttackSpeed-2)
					{
						if(GameMode==0)//ShutGun Mode
						{
							AfterImageAnimationList.Add(new DrawingLayer("Player", new Rectangle(player.GetPosition(), new Point(80, 80))),7);
							if (Score >= 10)
							{
								if (AttackTimer < 3)
									player.MoveTo(enemies[AttackIndex].getCenter().X, enemies[AttackIndex].getCenter().Y, AttackTimer * 5);
								else
									player.MoveTo(enemies[AttackIndex].getCenter().X, enemies[AttackIndex].getCenter().Y, -AttackTimer * 2);

							}
						}
						else if(GameMode==1)
						{
							player.MoveTo(Standard.cursor.getPos().X - 40, Standard.cursor.getPos().Y - 40, MoveSpeed * 2);
							AfterImageAnimationList.Add(new DrawingLayer("Player", new Rectangle(player.GetPosition(), new Point(80, 80))),8);
						}

					}
					return;
				}
				if (Standard.cursor.didPlayerJustRightClick()||Standard.cursor.didPlayerJustLeftClick()|| Standard.JustPressed(Keys.A))
				{
					if (Standard.cursor.didPlayerJustLeftClick())
						ButtonInfoString = "Left Click";
					else if (Standard.cursor.didPlayerJustRightClick())
						ButtonInfoString = "Right Click";
					else if (Standard.JustPressed(Keys.A))
						ButtonInfoString = "Press \"A\"";
					for (int i = 0; i < enemies.Count; i++)
					{
						if (enemies[i].getBound().Contains(Standard.cursor.getPos()) && Standard.Distance(getPos(), enemies[i].getPos()) < Range)
						{
							int ClickDistance = Standard.Distance(Standard.cursor.getPos(), enemies[i].getCenter());
							AttackIndex = i;
							for (int j = i; j < enemies.Count; j++)
							{
								if (Standard.Distance(Standard.cursor.getPos(), enemies[i].getCenter()) < ClickDistance)
								{
									AttackIndex = j;
									ClickDistance = Standard.Distance(Standard.cursor.getPos(), enemies[i].getCenter());
								}
							}
							isAttacking = true;
							AttackTimer = AttackSpeed;
							return;
						}

					}
			
						MovePoint = Standard.cursor.getPos();
					animationList.Add(new DrawingLayer("Click", new Rectangle(MovePoint.X - 15, MovePoint.Y - 15, 30, 30)), 10);
				}
				
				if (!MoveLock&&(MovePoint.X!=0||MovePoint.Y!=0))
				{
					if (BoostTimer > 0)
						BoostTimer--;
					player.MoveTo(MovePoint.X - 40, MovePoint.Y - 40, Math.Max(0,MoveSpeed-BoostTimer));
					int x2 = (MovePoint.X + 4 * getPos().X) / 5;
					int y2 = (MovePoint.Y + 4 * getPos().Y) / 5;
					direction.setPosition(x2 + 25, y2 + 25);
				}
			}

			public void AttackUpdate()
			{
				if(isAttacking)
				{
					if(AttackTimer==AttackSpeed)
					{
						if(Score<307)
						{
							Tempo = 30;
						}
						else
						{
							Tempo = 25.7;
						}
						if (Score < 10)
							Standard.PlaySound("EnemyDead");
						else
							Standard.PlaySound("GunSound");
						}
					if (AttackTimer>0)//투사체 날아가는중
					{
						AttackTimer--;
						return;
					}
					else//투사체 적중
					{
						Rectangle r = enemies[AttackIndex].getBound();
						enemies.RemoveAt(AttackIndex);
						int rn = random.Next(3, 5);
							for (int i = 0; i < rn; i++)
							{
								int s = random.Next(10, 50);
								DrawingLayer newStar;
								animationList.Add(newStar=new DrawingLayer("Player2", new Rectangle(r.Center.X - random.Next(-30, 30), r.Center.Y - random.Next(-30, 30), s, s)), random.Next(5*3, 15*3));
								DeadBodys.Add(newStar);
							}
						Score++;
						isAttacking = false;
						AttackIndex = -1;
						BoostTimer = 0;
						return;
					}
				}
			}

			public void DrawAttack()
			{
				if(isAttacking&&AttackTimer>=1)
				{
					int x = ((AttackSpeed-AttackTimer) * enemies[AttackIndex].getPos().X + AttackTimer * getPos().X)/AttackSpeed;
					int y = ((AttackSpeed - AttackTimer) * enemies[AttackIndex].getPos().Y + AttackTimer * getPos().Y)/AttackSpeed;
					bullet.setPosition(x + 25, y + 25);
					bullet.Draw(MasterInfo.PlayerColor,0f);
					int x2 = (enemies[AttackIndex].getPos().X + 3 * getPos().X) / 4;
					int y2 = (enemies[AttackIndex].getPos().Y + 3 * getPos().Y) / 4;
					direction.setPosition(x2 + 25, y2 + 25);
					direction.Draw(Color.White);

				}
			}
		}

		public class Enemy
		{
			public DrawingLayer enemy;


			public Point getPos()
			{
				return enemy.GetPosition();
			}

			public Point getCenter()
			{
				return enemy.GetCenter();
			}

			public Rectangle getBound()
			{
				return enemy.GetBound();
			}

			public Enemy()
			{
				if(Score<10)
				{
					enemy = new DrawingLayer("Player", new Rectangle(random.Next(0, 800), random.Next(0, 800), 80, 80));
					return;
				}
				int x=0;
				int y=0;
			
				if (random.Next(0, 2) == 0)
					x = random.Next(15, 80);
				else
					x  = random.Next(800, 880);
				if (random.Next(0, 2) == 0)
					y = random.Next(15, 80);
				else
					y = random.Next(720, 800);
				enemy = new DrawingLayer("Player", new Rectangle(x, y, 80, 80));
			}

			public void Draw()
			{
				if (enemy.GetBound().Contains(Standard.cursor.getPos()))
				{
					if (Standard.Distance(player.getPos(), getPos()) > player.getRange())
						enemy.Draw(Color.Blue);
					else
					{
						enemy.Draw(Color.Crimson);
						CursorShouldBeSword = true;
					}
					return;
				}
				if (Score >= 10 && Score<400)
					enemy.Draw(Color.LightSeaGreen);
				else 
					enemy.Draw(Color.White);
			}

			/*public void MoveUpdate(int Index)
			{
				if(ZombieDance)
				{
					if(Standard.Distance(getPos(),player.getPos())>160)
					{
						enemy.MoveTo(player.getPos().X + random.Next(-300, 300), player.getPos().Y + random.Next(-300, 300), 5);
					}
					else
					{
						enemy.MoveTo(player.getPos().X + random.Next(-300, 300), player.getPos().Y + random.Next(-300, 300), -5);
					}
					return;
				}

				if(Score<10)
				{
					if(player.getMovePoint().X!=0||player.getMovePoint().Y!=0)
						enemy.MoveTo(player.getMovePoint().X + random.Next(-150, 150), player.getMovePoint().Y + random.Next(-150, 150), 3);
					else
						enemy.MoveTo(player.getPos().X + random.Next(-300, 300), player.getPos().Y + random.Next(-300, 300), -Math.Min(Score,3));
					return;
				}

				enemy.MoveTo(player.getPos().X+random.Next(-300,300), player.getPos().Y+random.Next(-300,300), ZombieSpeed);
				if((Standard.Distance(player.getPos(),getPos()))<=10&&Index!=player.getAttackIndex())
				{
					GameOver = true;
				}
			}*/

			public void MoveUpdate(int Index, int r_1, int r_2)
			{
				if (ZombieDance)
				{
					if (Standard.Distance(getPos(), player.getPos()) > 160)
					{
						enemy.MoveTo(player.getPos().X + r_1, player.getPos().Y + r_2, 5);
					}
					else
					{
						enemy.MoveTo(player.getPos().X + r_1, player.getPos().Y + r_2, -5);
					}
					return;
				}

				if (Score < 10)
				{
					if (player.getMovePoint().X != 0 || player.getMovePoint().Y != 0)
						enemy.MoveTo(player.getMovePoint().X + r_1, player.getMovePoint().Y + r_2, 3);
					else
						enemy.MoveTo(player.getPos().X + r_1, player.getPos().Y + r_2, -Math.Min(Score, 3));
					return;
				}

				enemy.MoveTo(player.getPos().X + r_1, player.getPos().Y + r_2, ZombieSpeed);
				if ((Standard.Distance(player.getPos(), getPos())) <= 10 && Index != player.getAttackIndex())
				{
					//GameOver = true;
				}
			}
		}
	}
}
