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
		
	

		//public static Random random=new Random();
		public static Player player;
		public static List<Enemy> enemies=new List<Enemy>();
		public static bool GameOver=false;
		public static bool ZombieDance = false;
		public static bool isZombieMode = false;
		public static int EndTimer = 0;
		public static int Score = 0;
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

		//public static AnimationList animationList = new AnimationList();
		//public static AnimationList DeadBodysAnimationList = new AnimationList();
		//public static AnimationList AfterImageAnimationList = new AnimationList();
		public static List<DrawingLayer> DeadBodys = new List<DrawingLayer>();
		public static int SoundTrack = 0;
		public static float BaseVolume = 1.0f;
		public static bool GameEnd = false;
		public static bool PressedQ = false;
		public static bool Reload = false;
		public static int BoostTimer = 0;
		public static bool AutoMouse = false;
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
		public static int UsePianoTimer = 0;
		public static int Difficulty = 1;
		public static int SongCount = 5;
		public static int KillerZombieIndex = 0;
		public static int ScoreStack = 0;
		//이후 마음대로 인수 혹은 콘텐츠들을 여기 추가할 수 있습니다.
		public Tester()//여기에서 각종 이니셜라이즈가 가능합니다.
		{
			player = new Player();
			enemies.Add(new Enemy());
			Standard.PlaySong(Standard.Random(0, SongCount),true);
			TestMenu = new EasyMenu(new string[] {
				"SONG",
				"CHARACTER",
				"TIPS",
				"SETTING",
				"EXIT" },new Rectangle(0,0,300,370),new Rectangle(0,0,200,50),new Point(80,20),new Point(0,70));

		}
		//Game1.Class 내에 Tester.Update()로 추가될 업데이트문입니다. 다양한 업데이트 처리를 시험할 수 있습니다.
		public void Update()
		{
			if(ScoreStack>0)
			{
				ScoreStack--;
				Score++;
			}

			if (Score < 100)
				KillerZombieIndex = 0;
			else if (Score < 200)
				KillerZombieIndex = 1;
			else if (Score < 300)
				KillerZombieIndex = 2;
			else if (Score < 400)
				KillerZombieIndex = 5;


			//속도 설정. 오토마우스를 켜면 대체로 적의 속도가 증가한다.
			if (AutoMouse)
			{
				if (GameMode == 0)
					ZombieSpeed = 9 + Difficulty * 3;
				else if (GameMode == 1)
					ZombieSpeed = 6 + Difficulty * 2;
			}
			else
			{
				ZombieSpeed = 3 + Difficulty * 2;
			}
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
				Rectangle rectangle = new Rectangle(500,450,200,50);
				bool ClickRButton =(rectangle.Contains(Standard.cursor.getPos()))&&(Standard.cursor.didPlayerJustLeftClick() || Standard.cursor.didPlayerJustRightClick());
				if (Standard.IsKeyDown(Keys.R)||ClickRButton)
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
				MainMenuIndex = -1;
			}

			if(Score<10&&MainMenuIndex!=-1)
			{
				if (Standard.JustPressed(Keys.Escape))
				{
					MainMenuIndex = -1;
				}
				if(!SubMenu.MouseIsOnFrame()&&(Standard.cursor.didPlayerJustLeftClick()||Standard.cursor.didPlayerJustRightClick()))
				{
					MainMenuIndex = -1;
				}
			
				//animationList.TimeUpdate();
					switch (MainMenuIndex)
				{
					case 0://Song
						if(!SubMenuIsMade)
						{
							SubMenu = new EasyMenu(new string[] {
								"1. The Nutcracker: March ~ Tchaikovsky",
								"2. Etude Op.25-2 ~ Chopin",
								"3. Polonaise Op.44 ~ Chopin",
								"4. Ballade No.1~ Chopin",
								"5. Little Fuga: BWV 578 ~ Bach",
								"Exit" },
	new Rectangle(270, 170, 580,500), new Rectangle(0, 0, 500, 50), new Point(80, 50), new Point(0, 70));
							SubMenuIsMade = true;
						}
						SubMenu.Update();
						if(SubMenu.GetIndex()!=-1 && (Standard.cursor.didPlayerJustLeftClick() || Standard.JustPressed(Keys.A)))
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
								"NK Cell",
								"Cytotoxic T Cell",
								"Exit" },
	new Rectangle(300, 200, 400, 240), new Rectangle(0, 0, 320, 50), new Point(80, 20), new Point(0, 70));
							SubMenuIsMade = true;
						}
						SubMenu.Update();
						if (SubMenu.GetIndex() != -1 && (Standard.cursor.didPlayerJustLeftClick() || Standard.JustPressed(Keys.A)))
						{
							if (SubMenu.MenuStringList[SubMenu.GetIndex()] == "Exit")
							{
								MainMenuIndex = -1;
							}
							else
							{
								GameMode = SubMenu.GetIndex();
								if (GameMode == 0)
								{
									player.SetAttackSpeed(8);
									player.setRange(160);
									ZombieSpeed = 3 + Difficulty * 2;
									player.SetMoveSpeed((8 + ZombieSpeed) / 3);
									AutoMouse = false;
								}
								else if (GameMode == 1)
								{
									player.SetAttackSpeed(15);
									player.setRange(130);
									ZombieSpeed = 6 + Difficulty * 2;
									player.SetMoveSpeed((7 + ZombieSpeed) / 3);
									AutoMouse = true;
								}
							}
						}
						break;

					case 2: // TUTORIAL
						if (!SubMenuIsMade)
						{
							SubMenu = new EasyMenu(new string[] {
								"Tip 1 : You can Move&Attack by \"A\" button&Left-Click&Right-Click",
								"Tip 2 : If Score is over 10, Zombies will attack you.",
								"Tip 3 : If Zombie reaches your center before you attack it, You die.",
								"Tip 4 : Zombie can't play The Piano.",
								"More Tip",
								"Exit" },
	new Rectangle(300, 200, 830, 450), new Rectangle(0, 0, 750, 50), new Point(80, 20), new Point(0, 70));
							SubMenuIsMade = true;
						}
						SubMenu.Update();
						if (SubMenu.GetIndex() != -1 && (Standard.cursor.didPlayerJustLeftClick() || Standard.JustPressed(Keys.A)))
						{
							if (SubMenu.MenuStringList[SubMenu.GetIndex()] == "Exit")
							{
								MainMenuIndex = -1;
							}
							if(SubMenu.MenuStringList[SubMenu.GetIndex()]=="More Tip")
							{
								SubMenu = new EasyMenu(new string[] {
								"Tip 5 : When the zombie presents in blue, it is out of your range.",
								"Tip 6 : CyT's attack-boost gives you boost to your cursor's position.",
								"Tip 7 : Pressing \"M\" Button enables/disables Mouse Macro.",
								"Go Back",
								"Exit" },
	new Rectangle(300, 200, 900, 380), new Rectangle(0, 0, 750, 50), new Point(80, 20), new Point(0, 70));
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
	new Rectangle(270, 190, 600, 450), new Rectangle(0, 0, 500, 50), new Point(100, 30), new Point(0, 70));
							SubMenuIsMade = true;
						}
						SubMenu.Update();
						if (SubMenu.GetIndex() != -1 && (Standard.cursor.didPlayerJustLeftClick() || Standard.JustPressed(Keys.A)))
						{
							if (SubMenu.MenuStringList[SubMenu.GetIndex()] == "Exit")
							{
								MainMenuIndex = -1;
							}
							else
							{
								Difficulty = SubMenu.GetIndex();
								if(GameMode==0)
								{
									ZombieSpeed = 3 + Difficulty * 2;
									player.SetMoveSpeed((8 + ZombieSpeed) / 3);
								}
								else if(GameMode==1)
								{
									ZombieSpeed = 6 + Difficulty * 2;
									player.SetMoveSpeed((7 + ZombieSpeed) / 3);
								}
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
					AutoMouse = !AutoMouse;
				}
				if(Standard.JustPressed(Keys.C))
				{
					for(int i=0;i<DeadBodys.Count;i++)
					{
						Standard.FadeAnimation(DeadBodys[i],30, Color.LightSeaGreen);
						//DeadBodysAnimationList.Add(DeadBodys[i], 10);
					}
					DeadBodys.Clear();
				}
				if (Score>=10&&Standard.JustPressed(Keys.Escape))
				{
					Game1.GameExit = true;
				}

			}


			if (CursorShouldBeSword)
				Standard.cursor.SetSprite("Sword");
			else
				Standard.cursor.SetSprite("Cursor");

			player.MoveUpdate();
			LightLayer3.setPosition(player.getPos().X-40+Standard.Random(-3,3), player.getPos().Y-40 + Standard.Random(-3,3));
			player.AttackUpdate();
			List<int> RandomInts = new List<int>();
			for(int i=0;i<15;i++)
			{
				RandomInts.Add(Standard.Random(-300, 300));
			}
			int j = 0 ;
			for (int i = 0; i < enemies.Count; i++)
			{
				if (j >= 14)
					j = 0;
				if(i<=KillerZombieIndex)
				{
					ZombieSpeed += 2;
					enemies[i].MoveUpdate(i, RandomInts[j], RandomInts[(j + Standard.FrameTimer) % 14]);
					ZombieSpeed -= 2;
				}			
				else
					enemies[i].MoveUpdate(i, RandomInts[j], RandomInts[(j + Standard.FrameTimer) % 14]);
				j++;
			}

			//animationList.TimeUpdate();
			//DeadBodysAnimationList.TimeUpdate();
			//AfterImageAnimationList.TimeUpdate();
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
				if (GameMode == 0)
					Standard.PlaySound("Reload");
				else if (GameMode == 1)
					Standard.PlaySound("WipeKnife");
				Reload = false;
			}
			if (DeadBodys.Count > 300)
			{
				Standard.FadeAnimation(DeadBodys[0], 30, Color.LightSeaGreen);
				//DeadBodysAnimationList.Add(DeadBodys[0], 30);
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
						TestMenu.MenuList[TestMenu.GetIndex()].drawingLayer.MoveByVector(new Point(Standard.Random(1, 3), Standard.Random(1, 3)), Standard.Random(1,10));
					}
					if (Standard.cursor.didPlayerJustLeftClick() || Standard.JustPressed(Keys.A))
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
				UsePianoTimer++;
			}
			else
			{
				TestMenu.MoveTo(-250, 300, 20);
				UsePianoTimer = 0;
			}
			if (player.getPos().X < -40)
				player.setPos(-40, player.getPos().Y);


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
			//DeadBodysAnimationList.FadeAnimationDraw(Color.LightSeaGreen, 1/30.0);
			MouseLogo.Draw();
			if(!AutoMouse)
			{
				if (Standard.FrameTimer % 10 <= 5 && Score < 10)
					MouseButton.Draw(Color.Red);
				if (Standard.FrameTimer % 10 <= 5 && ZombieDance)
					MouseButton.Draw(Color.Crimson);
			}
			Color StringColor = Color.White;
			if (Score >= 10)
				StringColor = Color.Red;

			Standard.DrawString("SCORE : " + Score + "/400", new Vector2(340, 440), StringColor);
			if (Score < 300)
			{
				if(!AutoMouse)
					Standard.DrawString(ButtonInfoString + " to live", new Vector2(300, 400), StringColor);
				else
					Standard.DrawString("You don't have to click", new Vector2(300, 400), StringColor);
			}
			else if (Score < 400)
				Standard.DrawString("Live to click", new Vector2(300, 400), StringColor);
			else
				Standard.DrawString("Congratulation!", new Vector2(300, 400), StringColor);		
			Standard.DrawString("Press \"R\" to reset", new Vector2(550,490), StringColor);
			Standard.DrawString("Press \"S\" to skip the break time", new Vector2(290, 470), StringColor);
			Standard.DrawString("in break time only, you can use the menu", new Vector2(450, 280), StringColor);

			if (Score < 10)
			{
				if(Standard.FrameTimer%50<25)
					Standard.DrawString("Here's Menu!", new Vector2(60, 500), Color.White);
				else
					Standard.DrawString("Here's Menu!", new Vector2(60, 500), Color.Gainsboro);
			}
			else
			{
				Standard.DrawString("Here's Piano!", new Vector2(60, 500), Color.LightSeaGreen);
			}

			if(AutoMouse)
			{
				Standard.DrawString("* Using mouse auto", new Vector2(230, 700), Color.White);
				Standard.DrawString("* Press \"M\" Button to disable mouse auto", new Vector2(230, 750), Color.White);
			}


			Standard.FadeAnimationDraw(Color.LightSeaGreen);

			LightLayer.Draw();
			//스코어 올라갈수록 보라색을 띈다.
			Standard.DrawLight(MasterInfo.FullScreen, Color.Purple, 0.3f * Math.Min(1.2f, (float)(Score / 100.0)), Standard.LightMode.Absolute);
			if (Score > 200)
				LightLayer3.Draw(Color.AntiqueWhite * 0.15f * Math.Min(5f, (float)((Score - 150.0)/50)));
			if (ZombieDance)
				Standard.DrawLight(MasterInfo.FullScreen, Color.White, (float)((3000 - EndTimer) / 3000.0), Standard.LightMode.Absolute);//3000프레임동안 점점 하얀색으로 밝아진다.


		
			TestMenu.Draw(Score>=10? false:true);
			if (Standard.FrameTimer % 20 == 0)
				MenuLightR = Standard.Random(0, 5);
			
			if(TestMenu.MouseIsOnFrame())
			{
				if (Score >= 10)
				for (int i=0;i<enemies.Count;i++)
				{
					if(TestMenu.Frame.GetBound().Contains(enemies[i].getCenter()))
					{
						enemies[i].enemy.MoveByVector(new Point(1, 0), 20);
						if(ZombieSpeed<9)
						TestMenu.Frame.MoveByVector(new Point(-1, 0), 37 - ZombieSpeed * 3/*Math.Max(1,25-ZombieSpeed*3+UsePianoTimer*7)*/);
						else
								TestMenu.Frame.MoveByVector(new Point(-1, 0), Math.Max(13,25-ZombieSpeed*3+UsePianoTimer*7));

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

			//animationList.FadeAnimationDraw(Color.DarkRed, 0.2);

			CursorShouldBeSword = false;
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Draw();
				if (i <= KillerZombieIndex)
					Standard.DrawAddon(enemies[i].enemy, Color.White, 1f, "KillerZombie4");
				else
					Standard.DrawAddon(enemies[i].enemy, Color.White, 1f, "NormalZombie");
			}



			if (Score>=10)
			{
				if(Standard.FrameTimer%10==0)
				{
					Lightr = Standard.Random() / 10.0;
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

			//animationList.FadeAnimationDraw(Color.DarkRed, 0.06f);

			if (Score < 10 && MainMenuIndex != -1&&SubMenu!= null&&SubMenu.MenuList.Count > 0)
			{
				SubMenu.Draw();
				switch (MainMenuIndex)
				{
					case 0:
						Standard.DrawString("Song List", new Vector2(290,190), Color.White);
						break;
					case 1:
						if(SubMenu!=null&&SubMenu.MenuList.Count>0)
							Standard.DrawString("Model :" + SubMenu.MenuStringList[GameMode] , new Vector2(300,200), Color.White);
						if(GameMode==0)
						{
							DrawingLayer NKCell = new DrawingLayer("NKCell5", new Rectangle(750,100,350,700));
							Standard.DrawLight(NKCell, Color.Azure, 0.3f, Standard.LightMode.Vignette);
							NKCell.Draw();
						
							Standard.DrawString("* Has short range", new Vector2(500, 500), Color.Red);
							Standard.DrawString("* Has firearm recoil", new Vector2(400, 550), Color.Red);
							Standard.DrawString("* Walker", new Vector2(300, 600), Color.Red);
							Standard.DrawString("* Natural Killer Cell : Induces osmotic cell lysis", new Vector2(330, 450), Color.Red);
						}
						else if(GameMode==1)
						{
							DrawingLayer NKCell = new DrawingLayer("CyT3", new Rectangle(750, 100, 350, 700));
							NKCell.Draw();
							Standard.DrawLight(NKCell, Color.Azure, 0.3f, Standard.LightMode.Vignette);

							Standard.DrawString("* Has Extremely short range", new Vector2(500, 500), Color.Red);
							Standard.DrawString("* Has attack-boost", new Vector2(400, 550), Color.Red);
							Standard.DrawString("* Sprinter", new Vector2(300, 600), Color.Red);
							Standard.DrawString("* Cytotoxic T Cell : Induces cell apoptosis", new Vector2(330, 450), Color.Red);
						}
						break;

					case 2:
						Standard.DrawString("Guide", new Vector2(315, 335), Color.White);
						break;

					case 3:
						string DifficultyString = "";
						for (int i = 0; i < SubMenu.MenuList.Count; i++)
						{
							if (Difficulty==i)
							{
								DifficultyString = SubMenu.MenuStringList[i];
								break;
							}
						}
						Standard.DrawString("Difficulty :" + DifficultyString, new Vector2(300, 200), Color.White);

						break;

				}
				Standard.DrawLight(SubMenu.Frame.GetBound(), Color.WhiteSmoke, 0.3f, Standard.LightMode.Vignette);
				return;
			}



			if (GameOver)
			{
				for(int i=0;i<enemies.Count;i++)
				{
					enemies[i].enemy.MoveTo(player.getPos().X+Standard.Random(-30,30), player.getPos().Y + Standard.Random(-30, 30), Standard.Random(5,15));
				}
				Standard.DrawString("Game Over", new Vector2(500+Standard.Random(-3,3), 400 + Standard.Random(-3, 3)), Color.Red);
				Rectangle rectangle = new Rectangle(500, 450, 200, 50);
				if(rectangle.Contains(Standard.cursor.getPos()))
				{
					Standard.DrawString("Press \"R\" button to restart", new Vector2(500 + Standard.Random(-3, 3), 450 + Standard.Random(-3, 3)), Color.Honeydew);
				}
				else
					Standard.DrawString("Press \"R\" button to restart", new Vector2(500 + Standard.Random(-3, 3), 450 + Standard.Random(-3, 3)), Color.Red);

			}

			//AfterImageAnimationList.FadeAnimationDraw(Color.White, 0.2 * 0.2);
			if (Score>=10)
			{
				Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 0.2f + (float)Lightr, Standard.LightMode.Absolute);
				Standard.DrawLight(MasterInfo.FullScreen, Color.DarkBlue, 0.3f, Standard.LightMode.Absolute);
			}
			//AfterImageAnimationList.FadeAnimationDraw(Color.Cornsilk, 0.2*0.5);
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


			public void SetMoveSpeed(int s)
			{
				MoveSpeed = s;
			}

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
					Standard.PlaySong(Standard.Random(0,SongCount),true);			
				}
				for (int i = 0; i < DeadBodys.Count; i++)
				{
					Standard.FadeAnimation(DeadBodys[i], 30,Color.LightSeaGreen);
					//DeadBodysAnimationList.Add(DeadBodys[i], 10);
				}
				DeadBodys.Clear();
				KillerZombieIndex = 0;
			}

			public int getAttackIndex()
			{
				return AttackIndex;
			}

			public Point getPos()
			{
				return player.GetPosition();
			}

			public void setPos(int x, int y)
			{
				player.setPosition(x, y);
			}

			public Point getMovePoint()
			{
				return MovePoint;
			}

			public int getRange()
			{
				return Range;
			}
			public void setRange(int i)
			{
				Range = i;
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
					if (AttackTimer < AttackSpeed - 2)
					{
						if (GameMode == 0)//ShotGun Mode
						{
							Standard.FadeAnimation(new DrawingLayer("Player", new Rectangle(player.GetPosition(), new Point(80, 80))), 7, Color.Cornsilk);
							//AfterImageAnimationList.Add(new DrawingLayer("Player", new Rectangle(player.GetPosition(), new Point(80, 80))), 7);
							if (Score >= 10)
							{
								if (AttackTimer < 3)
									player.MoveTo(enemies[AttackIndex].getCenter().X, enemies[AttackIndex].getCenter().Y, AttackTimer * 5);
								else
									player.MoveTo(enemies[AttackIndex].getCenter().X, enemies[AttackIndex].getCenter().Y, -AttackTimer * (MoveSpeed/2));

							}
						}
						else if (GameMode == 1)//AttackBoost mode
						{
							player.MoveTo(Standard.cursor.getPos().X - 40, Standard.cursor.getPos().Y - 40, MoveSpeed * 2);
							Standard.FadeAnimation(new DrawingLayer("Player", new Rectangle(player.GetPosition(), new Point(80, 80))), 8, Color.Cornsilk);
							//AfterImageAnimationList.Add(new DrawingLayer("Player", new Rectangle(player.GetPosition(), new Point(80, 80))), 8);
						}
					}
					if (GameMode == 0)
						MovePoint = new Point(0, 0);
					else if (GameMode == 1)
						MovePoint = Standard.DivPoint(player.GetCenter(), Standard.cursor.getPos(), 0.8);

					if (AutoMouse||Standard.cursor.didPlayerJustRightClick() || Standard.cursor.didPlayerJustLeftClick() || Standard.JustPressed(Keys.A) )
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
						Standard.FadeAnimation(new DrawingLayer("Click", new Rectangle(MovePoint.X - 15, MovePoint.Y - 15, 30, 30)), 10, Color.DarkRed);
						//animationList.Add(new DrawingLayer("Click", new Rectangle(MovePoint.X - 15, MovePoint.Y - 15, 30, 30)), 10);
			
					}
				
					return;
				}
				if (AutoMouse || Standard.cursor.didPlayerJustRightClick() || Standard.cursor.didPlayerJustLeftClick() || Standard.JustPressed(Keys.A))
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
					Standard.FadeAnimation(new DrawingLayer("Click", new Rectangle(MovePoint.X - 15, MovePoint.Y - 15, 30, 30)), 10, Color.DarkRed);
					//animationList.Add(new DrawingLayer("Click", new Rectangle(MovePoint.X - 15, MovePoint.Y - 15, 30, 30)), 10);
				}

				if (MovePoint.X!=0||MovePoint.Y!=0)
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
						else if(GameMode==0)
							Standard.PlaySound("GunSound");
						else if(GameMode==1)
						{
							Standard.PlaySound("KnifeSound",0.4f);
							Standard.PlaySound("GunSound",0.3f);
						}
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
						int rn = Standard.Random(3, 5);
							for (int i = 0; i < rn; i++)
							{
								int s = Standard.Random(10, 50);
								DrawingLayer newStar;
								Standard.FadeAnimation(newStar = new DrawingLayer("Player2", new Rectangle(r.Center.X - Standard.Random(-30, 30), r.Center.Y - Standard.Random(-30, 30), s, s)), Standard.Random(5 * 3, 15 * 3), Color.DarkRed);
								//animationList.Add(newStar=new DrawingLayer("Player2", new Rectangle(r.Center.X - Standard.Random(-30, 30), r.Center.Y - Standard.Random(-30, 30), s, s)), Standard.Random(5*3, 15*3));
								DeadBodys.Add(newStar);
							}
						ScoreStack++;
						if (KillerZombieIndex >= AttackIndex)
							ScoreStack++;
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
					int KillActionTimer = (int)(AttackTimer * 2);
					if(Standard.FrameTimer%20<6)
						Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Standard.cursor.getPos().X-35, Standard.cursor.getPos().Y - 35,70,70)),KillActionTimer,Color.Pink);
					else if(Standard.FrameTimer%20<12)
						Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Standard.cursor.getPos().X - 35, Standard.cursor.getPos().Y - 35, 70, 70)), KillActionTimer, Color.PaleVioletRed);
					else
						Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Standard.cursor.getPos().X - 35, Standard.cursor.getPos().Y - 35, 70, 70)), KillActionTimer, Color.SkyBlue);

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
					enemy = new DrawingLayer("Player", new Rectangle(Standard.Random(0, 800), Standard.Random(0, 800), 80, 80));
					return;
				}
				int x=0;
				int y=0;
			
				if (Standard.Random(0, 2) == 0)
					x = Standard.Random(15, 80);
				else
					x  = Standard.Random(800, 880);
				if (Standard.Random(0, 2) == 0)
					y = Standard.Random(15, 80);
				else
					y = Standard.Random(720, 800);
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
					GameOver = true;
				}
			}


			
		}
	}
}
