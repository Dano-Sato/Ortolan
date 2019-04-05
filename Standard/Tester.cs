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
		

		public static void SetZombieSpeed()
		{
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
		}

		public static void SetPlayer()
		{
			if (GameMode == 0)
			{
				player.SetAttackSpeed(10);
				player.setRange(150 + Difficulty * 10);
				player.SetMoveSpeed((8 + ZombieSpeed) / 3);
				AutoMouse = false;
			}
			else if (GameMode == 1)
			{
				player.SetAttackSpeed(15);
				player.setRange(140+Difficulty*10);
				player.SetMoveSpeed((7 + ZombieSpeed) / 3);
				AutoMouse = true;
			}
		}

		public static void SetKillerZombie()
		{ 
			if (Score < 100)
				KillerZombieIndex = 0;
			else if (Score < 200)
				KillerZombieIndex = 1;
			else if (Score < 300)
				KillerZombieIndex = 2;
			else if (Score < 400)
				KillerZombieIndex = 5;
		}

		public static void UpdateScore()
		{
			if (ScoreStack > 0)
			{
				ScoreStack--;
				Score++;
			}
		}

		public static void ResetGame()
		{
			Score = 0;
			Fear = 0;
			Standard.FrameTimer = 0;
			ZombieTime = 40;
			enemies.Clear();
			enemies.Add(new Enemy());
			enemies.Add(new Enemy());

			player.reset();
			GameOver = false;
			GameEnd = false;
			ZombieDance = false;

			MenuPositionCoefficient = 0;
		}
		


		public static Player player;
		public static List<Enemy> enemies=new List<Enemy>();
		public static bool GameOver=false;
		public static bool ZombieDance = false;
		public static int EndTimer = 0;
		public static int Score = 0;
		public static int ZombieTime = 40;
		public static double Lightr = 0;//화면이 좀 깜빡거리도록 하기 위해 넣은 변수
		public static string ButtonInfoString = "Right-Click";
		public static DrawingLayer Halo = new DrawingLayer("Player", new Rectangle(0, 0, 150, 150));
		public static DrawingLayer MouseLogo = new DrawingLayer("Mouse", new Rectangle(270, 400, 30, 30));
		public static DrawingLayer MouseButton = new DrawingLayer("Mouse3", new Rectangle(270, 400, 30, 30));
		public static DrawingLayer RealMonoLogo = new DrawingLayer("RealMono", MasterInfo.PreferredScreen);
		public static DrawingLayer BloodLayer= new DrawingLayer("Blood", MasterInfo.FullScreen);
		public static List<DrawingLayer> DeadBodys = new List<DrawingLayer>();
		public static bool IsPlayingEndSong = false;
		public static float BaseVolume = 1.0f;
		public static bool GameEnd = false;
		public static bool Reload = false;
		public static int BoostTimer = 0;
		public static bool AutoMouse = false;
		public static Color ZombieColor = Color.White;
		public EasyMenu MainMenu;
		public static int MainMenuIndex = -1;
		public EasyMenu SubMenu=new EasyMenu();
		public static bool SubMenuIsMade = false;
		public static int GameMode = 1;//ShutGun Mode;
		public static int ZombieSpeed = 5;
		public static bool CursorShouldBeSword = false;
		public static int UsePianoTimer = 0;
		public static int Difficulty = 1;
		public static int SongCount = 5;
		public static int KillerZombieIndex = 0;
		public static int ScoreStack = 0;
		public static double Fear = 0;
		public static Point OldPlayerPos;
		public static Point OldPlayerDisplacementVector;

		public static int FreezeTimer = -1;
		public static int HappyTimer = 0;
		public static int MenuPositionCoefficient = 0;
		public static bool MenuFlip = false;
		public static int MenuMoveCounter = 0;

		public static bool ZombieFlip = true;
		public static Point Wind = new Point(0, 1);
		public static bool GameStart = true;
		public static Point ZombieCOM = new Point(0, 0);

		public static bool ShowMenu = false;
		public static DrawingLayer MenuLayer=new DrawingLayer("WhiteSpace", new Rectangle(100,50,1000,700));
		public static ScrollBar ScrollBar_Sensitivity = new ScrollBar(new DrawingLayer("BarFrame2",new Rectangle(200,150,500,50)), "Bar2", 50, false);
		public static ScrollBar ScrollBar_SongVolume = new ScrollBar(new DrawingLayer("BarFrame2", new Rectangle(200, 220, 500, 50)), "Bar2", 50, false);
		public static ScrollBar ScrollBar_SEVolume= new ScrollBar(new DrawingLayer("BarFrame2", new Rectangle(200, 290, 500, 50)), "Bar2", 50, false);

		//이후 마음대로 인수 혹은 콘텐츠들을 여기 추가할 수 있습니다.
		public Tester()//여기에서 각종 이니셜라이즈가 가능합니다.
		{
			player = new Player();
			enemies.Add(new Enemy());
			Standard.PlaySong(Standard.Random(0, SongCount),true);
			MainMenu = new EasyMenu(new string[] {
				"SONG",
				"CHARACTER",
				"TIPS",
				"SETTING",
				"EXIT" },new Rectangle(0,0,300,370),new Rectangle(0,0,200,50),new Point(80,20),new Point(0,70));
			ScrollBar_Sensitivity.Initialize(1.0f / (2.0f - 0.3f));

		}
		//Game1.Class 내에 Tester.Update()로 추가될 업데이트문입니다. 다양한 업데이트 처리를 시험할 수 있습니다.
		//그림으로 그려지기 이전 각종 변수들의 처리를 담당합니다.
		public void Update()
		{
			/*기타*/

			UpdateScore();
			SetKillerZombie();
			SetZombieSpeed();
			SetPlayer();

			

			if (GameEnd)
			{

				if (EndTimer < 1000)
					EndTimer++;
				if (EndTimer == 1000)
				{
					ResetGame();
				}
				return;
			}
			if (GameOver&&FreezeTimer==-1)
			{
				FreezeTimer = 200;
				Standard.ClearFadeAnimation();
				Standard.PlaySound("ZombieSound4");
				return;
			}
			if(FreezeTimer>0)
			{
				Standard.ClearFadeAnimation();
				if (FreezeTimer==200)
					Standard.PlaySound("KnifeSound", 0.3f);
				if(FreezeTimer==140)
					Standard.PlaySound("KnifeSound", 0.5f);
				if (FreezeTimer == 90)
					Standard.PlaySound("KnifeSound",0.5f);
				FreezeTimer--;
				Fear += 10;
				return;
			}
			if(FreezeTimer==0)
			{
				FreezeTimer = -1;
				Standard.FadeAnimation(new DrawingLayer("YouDied", new Rectangle(200, 350, 400, 200)), 90, Color.DarkRed);
				Standard.FadeAnimation(new DrawingLayer("Tip", new Rectangle(400, 500, 400, 200)), 150, Color.DarkRed);
				ResetGame();
			}
			if (CursorShouldBeSword)
				Standard.cursor.SetSprite("Sword");
			else
				Standard.cursor.SetSprite("Cursor");

			if(!ShowMenu)
			{
				player.MoveUpdate();
				Halo.setPosition(player.getPos().X - 40 + Standard.Random(-3, 3), player.getPos().Y - 40 + Standard.Random(-3, 3));
				player.AttackUpdate();
			}

			if (Score == 10)
			{
				ZombieColor = Color.LightSeaGreen;
			}

			if (Score % 10 == 9)
				Reload = true;
			if (Score % 10 == 0 && Reload == true)
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
				DeadBodys.RemoveAt(0);
			}

			/*서브메뉴 생성 처리*/

			if (MainMenuIndex!=-1)//메인메뉴에서 클릭할 시, 서브메뉴를 불러온다.원랜 스코어 10이하일때만 작동하게 했는데 지금 실험중이다. Score<10&&
			{
				if (Standard.JustPressed(Keys.Escape))
				{
					MainMenuIndex = -1;
				}
				/*if(!SubMenu.MouseIsOnFrame()&&(Standard.cursor.didPlayerJustLeftClick()||Standard.cursor.didPlayerJustRightClick()))
				{
					MainMenuIndex = -1;
				}*/
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
								if(Score<400)
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
								if (Score < 400)
								GameMode = SubMenu.GetIndex();
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
								if (Score < 300)
								Difficulty = SubMenu.GetIndex();
							}
						}
						break;

				}
				//return;
			}


			/*키보드 입력 처리*/

			if (Standard.KeyInputOccurs())
			{
				if(Standard.JustPressed(Keys.C))//핏자국 청소
				{
					for(int i=0;i<DeadBodys.Count;i++)
					{
						Standard.FadeAnimation(DeadBodys[i],30, Color.LightSeaGreen);
					}
					DeadBodys.Clear();
				}
				if (Score < 400 && Standard.IsKeyDown(Keys.R))//게임 리셋
				{
					ResetGame();
					return;
				}

				if (Score < 10 && Standard.JustPressed(Keys.S))//빠른 시작
				{
					Score = 10;
				}
				if(Standard.JustPressed(Keys.Escape))
				{
					ShowMenu = !ShowMenu;
				}

			}

			if(ShowMenu)
			{
				
				ScrollBar_Sensitivity.Update();
				ScrollBar_SongVolume.Update();
				ScrollBar_SEVolume.Update();
				Standard.cursor.Sensitivity = ScrollBar_Sensitivity.MapCoefficient(0.5f, 2.0f);
				Standard.SetSongVolume(ScrollBar_SongVolume.Coefficient);
				BaseVolume = ScrollBar_SongVolume.Coefficient;
				Standard.SetSEVolume(ScrollBar_SEVolume.Coefficient);
				return;
			}

			/*좀비들의 이동 처리.*/

			List<int> RandomInts = new List<int>();
			for(int i=0;i<15;i++)
			{
				RandomInts.Add(Standard.Random(-300, 300));
			}
			int j = 0;
			for(int i=0;i<enemies.Count;i++)
			{
				if(i==0)
				ZombieCOM = new Point(0, 0);
				ZombieCOM = Standard.Add(ZombieCOM, enemies[i].getCenter());
			}
			if(enemies.Count>0)
			ZombieCOM = new Point(ZombieCOM.X / enemies.Count, ZombieCOM.Y / enemies.Count);
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


			/*좀비 생성 작업*/

			if (Score < 100)
				ZombieTime = Math.Max(10 - Score / 10,5);//좀비 생성 시간은 스코어가 높을수록 빨라진다.
			else
				ZombieTime = 4;
		
			if(ZombieTime>0)
			{
				if (Standard.FrameTimer % ZombieTime == 0)
				{
					if(!(Score<10&&enemies.Count>10))
						enemies.Add(new Enemy());
					if (enemies.Count > 150&&!player.IsAttacking())
						enemies.RemoveAt(0);
				}
			}
			else if(!ZombieDance)//좀비가 0프레임당 생성된다=게임을 깼다는 뜻. 게임을 깬 상태로 전환한다.
			{
				ZombieDance = true;
				MouseLogo.setSprite("Cake");
				MouseButton.setSprite("Cake2");
				EndTimer = 4000;
			}

			if (ZombieDance&&EndTimer>0)
				EndTimer--;
			if (ZombieDance && EndTimer == 0)
				GameEnd = true;
		
			
			/*스코어가 300으로 도달할때쯤, 음악을 교체할 준비를 한다.*/

			if (Score <= 280)
				MediaPlayer.Volume = BaseVolume;
			else if(Score<=300)
			{
				MediaPlayer.Volume = BaseVolume*(float)((300-Score)/20.0);
			}
			else if(Score<=306&&!IsPlayingEndSong)
			{
				IsPlayingEndSong=true;
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
			


			/*메인메뉴 클릭 처리*/
			MainMenu.Update();
			if(ScoreStack!=0)
			{
				if(MenuFlip)
				{
					MenuMoveCounter++;
				}
				if(MenuMoveCounter==2)
				{
					/*
					if (MenuPositionCoefficient == -300)
						MenuPositionCoefficient = 130;
					else
						MenuPositionCoefficient = -300;
						*/
					MenuMoveCounter = 0;
				}
				MenuFlip = false;
			}
			else
			{
				MenuFlip = true;
			}
			if (MainMenu.MouseIsOnFrame())
			{
					MainMenu.MoveTo(-50, 300+MenuPositionCoefficient, 20);
				if (MainMenu.GetIndex()!=-1)//실험중.Score<10&&이라는조
				{
					if (MainMenu.MenuStringList[MainMenu.GetIndex()] == "EXIT")//Exit 위에 마우스가 올라가면 메뉴가 덜덜 떤다.ㅋ
					{
						MainMenu.MenuList[MainMenu.GetIndex()].drawingLayer.MoveByVector(new Point(Standard.Random(1, 3), Standard.Random(1, 3)), Standard.Random(1,10));
					}
					if (Standard.cursor.didPlayerJustLeftClick())
					{
						if (MainMenu.MenuStringList[MainMenu.GetIndex()] == "EXIT")
						{
							Game1.GameExit = true;
							return;
						}
						MainMenuIndex = MainMenu.GetIndex();
						SubMenuIsMade = false;
					}
				}
				UsePianoTimer++;
			}
			else
			{
				MainMenu.MoveTo(-200, 300 + MenuPositionCoefficient, 20);
				UsePianoTimer = 0;
			}
			if (player.getPos().X < -40)
				player.setPos(-40, player.getPos().Y);

			//메인 메뉴 위로는 좀비들이 오지 못한다.
			if (MainMenu.MouseIsOnFrame()||true)
			{
				if (Score >= 10)
					for (int i = 0; i < enemies.Count; i++)
					{
						if (MainMenu.Frame.GetBound().Contains(enemies[i].getCenter()))
						{
							enemies[i].enemy.MoveByVector(new Point(1, 0), 20 + ZombieSpeed);
							MainMenu.Frame.MoveByVector(new Point(-1, 0), 18);
						}
					}
			}

			//서브메뉴와 좀비 처리
			if (Score >= 10&&MainMenuIndex!=-1)
			{
				//서브메뉴는 좌우로 요동친다.
				if (Standard.FrameTimer % 10 < 5)
					SubMenu.Frame.MoveByVector(new Point(-1, 0), 20);
				else
					SubMenu.Frame.MoveByVector(new Point(1, 0), 20);

				for (int i = 0; i < enemies.Count; i++)//에너미를 왼쪽으로 보낸다.
				{
					if (SubMenu.Frame.GetBound().Contains(enemies[i].getCenter()))
					{
						enemies[i].enemy.MoveByVector(new Point(-1, 0), 20);


					}
				}
				if (SubMenu.Frame.GetBound().Contains(player.GetCenter()))//플레이어가 닿으면? 텔포를 시킨다.
				{
					int PlayerAndSubMenuFrameDistance = Math.Min(Math.Abs(player.GetCenter().Y - SubMenu.Frame.GetPosition().Y), Math.Abs(player.GetCenter().Y - SubMenu.Frame.GetPosition().Y - SubMenu.Frame.GetBound().Height));
					if (player.getPos().Y>SubMenu.Frame.GetCenter().Y)
					{
						player.MoveByVector(new Point(0, -1), SubMenu.Frame.GetBound().Height+20-PlayerAndSubMenuFrameDistance);
					}
					else
						player.MoveByVector(new Point(0, 1), SubMenu.Frame.GetBound().Height+20- PlayerAndSubMenuFrameDistance);

				}



			}

			/*뷰포트 처리*/
			/*
			Point PlayerDisPlacementVector = Standard.Deduct(player.getPos(), OldPlayerPos);
			Point ViewportDisplacement = Standard.Deduct(PlayerDisPlacementVector, OldPlayerDisplacementVector);
			Standard.Viewport = new Viewport(-player.getPos().X+400, -player.getPos().Y+ 400, 1300, 1300);

			OldPlayerPos = player.getPos();
			OldPlayerDisplacementVector = PlayerDisPlacementVector;

			if(GameMode==0)
				Standard.Viewport = new Viewport(-player.getPos().X/2 + ViewportDisplacement.X + 400/2, -player.getPos().Y/2 + ViewportDisplacement.Y + 400/2, 1300, 1300);
			else
				Standard.Viewport = new Viewport(-player.getPos().X + ViewportDisplacement.X/2+ 400, -player.getPos().Y + ViewportDisplacement.Y/2+ 400, 1300, 1300);

			if (MainMenuIndex!=-1)
			{
				if (GameMode==0)
					Standard.Viewport = new Viewport(ViewportDisplacement.X, ViewportDisplacement.Y, 1300, 1300);
				else
					Standard.Viewport = new Viewport(ViewportDisplacement.X/2, ViewportDisplacement.Y/2, 1300, 1300);

				if (Math.Abs(ViewportDisplacement.Y) > 100)
					Standard.Viewport = new Viewport(ViewportDisplacement.X, ViewportDisplacement.Y / 20, 1300, 1300);
				
			}
			*/
			Point PlayerDisPlacementVector = Standard.Deduct(player.getPos(), OldPlayerPos);
			Point ViewportDisplacement = Standard.Deduct(PlayerDisPlacementVector, OldPlayerDisplacementVector);
			Standard.Viewport = new Viewport(-player.getPos().X + 400, -player.getPos().Y + 400, 1300, 1300);
			Point CursorDisplacement = Standard.Deduct(player.getPos(), Standard.cursor.getPos());
			int Dis = Standard.Distance(CursorDisplacement, new Point(0, 0));
			
			//CursorDisplacement = new Point(CursorDisplacement.X * 5 / (int)Math.Sqrt(Dis), CursorDisplacement.Y * 5 / (int)Math.Sqrt(Dis));
			
			OldPlayerPos = player.getPos();
			OldPlayerDisplacementVector = PlayerDisPlacementVector;

			if (GameMode == 0)
				Standard.Viewport = new Viewport(-player.getPos().X / 2 + ViewportDisplacement.X + 400 / 2, -player.getPos().Y / 2 + ViewportDisplacement.Y + 400 / 2, 1300, 1300);
			else
				Standard.Viewport = new Viewport( -player.getPos().X +CursorDisplacement.X/2+ ViewportDisplacement.X / 2 + 400, -player.getPos().Y + CursorDisplacement.Y / 2 + ViewportDisplacement.Y / 2 + 400, 1300, 1300);



			if (Standard.Random()<0.05)
			{
				Standard.FadeAnimation(new DrawingLayer("Rhythm2", new Rectangle(Standard.Random(0, 100), Standard.Random(200,600)+MenuPositionCoefficient, 40, 80)), 15, Color.Black);
			}




		}

		//Game1.Class 내에 Tester.Draw()로 추가될 드로우 액션문입니다. 다양한 드로잉을 시험할 수 있습니다.
		public void Draw()
		{
			
			if(GameEnd)
			{
				Standard.Viewport = new Viewport(0, 0, 1300, 1300);
				Standard.DrawLight(MasterInfo.FullScreen, Color.White, 1.0f, Standard.LightMode.Absolute);
				RealMonoLogo.Draw(Color.White,Math.Min((float)(EndTimer/300.0),1.0f));
				Standard.DrawString("Thank you for playing!", new Vector2(500, 500), Color.Aquamarine);
				return;
			}
			BloodLayer.Draw(Color.Aquamarine,Math.Min(10,Score)*0.1f);
			for(int i=0;i<DeadBodys.Count;i++)
			{
				DeadBodys[i].MoveByVector(Wind, ZombieSpeed);
				if (DeadBodys[i].GetPosition().Y > 1300)
					DeadBodys[i].setPosition(DeadBodys[i].GetPosition().X, 0);
				DeadBodys[i].Draw(Color.Aquamarine, Math.Min(10, Score) * 0.1f);
			}
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

			if(!Standard.IsPrimeNumber(Score))
				Standard.DrawString("SCORE : " + Score + "/400", new Vector2(340, 440), StringColor);
			else
				Standard.DrawString("BigFont","SCORE : " + Score + "/400", new Vector2(340, 420), StringColor);


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
			if(Score<10)
			Standard.DrawString("Press \"S\" to skip the break time", new Vector2(290, 470), StringColor);
		
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
			Standard.FadeAnimationDraw(Color.LightSeaGreen);//별이 사라지는 페이드애니메이션(컬러는 LighteaGreen으로 지정)은 아래 라이트레이어 전에 발생해야 보기 좋으므로 별도로 처리함.

			/*풀스크린 라이트 레이어 처리*/

			Standard.DrawLight(MasterInfo.FullScreen, Color.White, 1f, Standard.LightMode.Vignette);
			//스코어 올라갈수록 보라색을 띈다.
			Standard.DrawLight(MasterInfo.FullScreen, Color.Purple, 0.3f * Math.Min(1.2f, (float)(Score / 100.0)), Standard.LightMode.Absolute);
			if (Score > 200)
				Halo.Draw(Color.AntiqueWhite * 0.15f * Math.Min(5f, (float)((Score - 150.0)/50)));
			if (ZombieDance)
				Standard.DrawLight(MasterInfo.FullScreen, Color.White, (float)((3000 - EndTimer) / 3000.0), Standard.LightMode.Absolute);//3000프레임동안 점점 하얀색으로 밝아진다.
		
		
			MainMenu.Draw(Score>=10? false:true);
			if (Standard.FrameTimer % 20 == 0)
				MenuLightR = Standard.Random(0, 5);
			if (!MainMenu.MouseIsOnFrame())
			{
				Rectangle MenuLight = new Rectangle(MainMenu.GetPosition(), new Point(300, 80));
				MenuLight.Location = Standard.Add(MainMenu.GetPosition(), new Point(0, MenuLightR * 72));
				Standard.DrawLight(MenuLight, Color.Black, 0.6f, Standard.LightMode.Vignette);
			}
			Standard.DrawLight(MainMenu.Frame, Color.Bisque, 0.6f, Standard.LightMode.Vignette);
			if (Score < 10 && MainMenu.GetIndex() != -1 && MainMenu.MenuStringList[MainMenu.GetIndex()] == "EXIT")
			{
				Standard.DrawLight(MainMenu.MenuList[MainMenu.GetIndex()].drawingLayer, Color.Crimson,0.2f, Standard.LightMode.Absolute);
				Standard.DrawLight(MainMenu.MenuList[MainMenu.GetIndex()].drawingLayer, Color.Red, 0.6f, Standard.LightMode.Vignette);
			}

			player.Draw();
			player.DrawAttack();


			Color AnimationColor= Color.DarkRed;
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
				if(Standard.FrameTimer%150==0 && FreezeTimer < 0)
				{
					if(Score>300)
					{
						Standard.PlaySound("BackgroundZombieSound");
					}
					else
					{
						Standard.PlaySound("BackgroundZombieSound", 0.8f);
					}
				}
			}

			if (HappyTimer > 0)
				HappyTimer--;
			
			if (MainMenuIndex != -1&&SubMenu!= null&&SubMenu.MenuList.Count > 0)//실험중. Score < 10 && 파트 뺌
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

							DrawingLayer NKCell = new DrawingLayer(Score<10?"NKCell5":"NKCell5_Easter2", new Rectangle(750,50,420,700));
							if(Score>=10&&(HappyTimer>0||NKCell.GetBound().Contains(player.GetCenter())))
							{
								NKCell.setSprite("NKCell5_EasterEaster");
							}
							if (Score >= 400)
								NKCell.setSprite("NKCell5_GameEnd");
							Standard.DrawLight(NKCell, Color.Azure, 0.3f, Standard.LightMode.Vignette);
							NKCell.Draw();
							if(Score<10)
							{
								Standard.DrawString("* Has short range", new Vector2(500, 500), Color.Red);
								Standard.DrawString("* Has firearm recoil", new Vector2(400, 550), Color.Red);
								Standard.DrawString("* Walker", new Vector2(300, 600), Color.Red);
								Standard.DrawString("* Natural Killer Cell : Induces osmotic cell lysis", new Vector2(330, 450), Color.Red);

							}
						}
						else if(GameMode==1)
						{
							DrawingLayer NKCell = new DrawingLayer(Score<10?"CyT3":"CyT3_Easter", new Rectangle(750, 50, 420, 700));
							if (Score >= 10 && (HappyTimer > 0 || NKCell.GetBound().Contains(player.GetCenter())))
							{
				
								NKCell.setSprite("CyT3_EasterEaster");
							}
							if(Score>=400)
								NKCell.setSprite("CyT3_GameEnd");
							NKCell.Draw();
							Standard.DrawLight(NKCell, Color.Azure, 0.3f, Standard.LightMode.Vignette);
							if(Score<10)
							{
								Standard.DrawString("* Has Extremely short range", new Vector2(500, 500), Color.Red);
								Standard.DrawString("* Has attack-boost", new Vector2(400, 550), Color.Red);
								Standard.DrawString("* Sprinter", new Vector2(300, 600), Color.Red);
								Standard.DrawString("* Cytotoxic T Cell : Induces cell apoptosis", new Vector2(330, 450), Color.Red);

							}
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
				//return;
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

			if (Score>=10&&Score<400)
			{
				Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 0.2f + (float)Lightr, Standard.LightMode.Absolute);
				Standard.DrawLight(MasterInfo.FullScreen, Color.DarkBlue, 0.3f, Standard.LightMode.Absolute);
			}
				OldStateOfMouseisOnMenu = MainMenu.Frame.MouseIsOnThis();


			/*스프라이트 바꾸기 장난*/

			if(ScoreStack!=0&&Score>10)
			{
				if (Score % 30 == 23 || Score % 30 == 24)
				{
					for (int i = 0; i < enemies.Count; i++)
					{
						enemies[i].enemy.setSprite("Player_Broken2");
					}
				}
				else if (Score % 30 == 25)
				{
					for (int i = 0; i < enemies.Count; i++)
					{
						enemies[i].enemy.setSprite("Player_V2");
					}
				}
				else if (Score % 30 == 22 || Score % 30 == 21)
				{
					for (int i = 0; i < enemies.Count; i++)
					{
						enemies[i].enemy.setSprite("Player_Broken");
					}
				}
				if (Score % 100 == 87)
				{

					for (int i = 0; i < enemies.Count; i++)
					{
						enemies[i].enemy.setSprite("Tip");
					}
				}
				if (Score % 70 == 9)
				{
					for (int i = 0; i < enemies.Count; i++)
					{
						enemies[i].enemy.setSprite("Player2");
						Standard.FadeAnimation(enemies[i].enemy, 30, Color.White);
					}
				}

				if(Standard.Random()<0.008)
				{
					for (int i = 0; i < enemies.Count; i++)
					{
						enemies[i].enemy.setSprite("Cake");
						Standard.FadeAnimation(enemies[i].enemy, 30, Color.White);
					}
				}
		

			}

			/*시야 처리*/
			if (Score >= 10&&Score<400)
			{
				for (int i = 0; i < enemies.Count; i++)
				{
					int r = Standard.Distance(enemies[i].getCenter(), player.GetCenter());
					Fear = Math.Min(Fear + 400.0/Math.Pow(Math.Max(r,9),2),200);
				}
				if (player.IsAttacking())
					Fear = Math.Max(0, Fear - Math.Max(1,enemies.Count/10));
				int Sight = Math.Max(800-(int)(Fear*4),player.getRange()+50);
				if (FreezeTimer > 0)
				{
					Sight = 800 - (int)(Fear * 4);
					if (FreezeTimer < 170)
						Sight = 0;

				}

				Standard.DrawLight(new Rectangle(0, 0, Math.Max(player.GetCenter().X - Sight, 0), 1300), Color.Black, 1f, Standard.LightMode.Absolute);
				Standard.DrawLight(new Rectangle(0, 0, 1300, Math.Max(player.GetCenter().Y - Sight, 0)), Color.Black, 1f, Standard.LightMode.Absolute);
				Standard.DrawLight(new Rectangle(player.GetCenter().X + Sight, 0, 1300, 1300), Color.Black, 1f, Standard.LightMode.Absolute);
				Standard.DrawLight(new Rectangle(0, player.GetCenter().Y + Sight, 1300, 1300), Color.Black, 1f, Standard.LightMode.Absolute);
				DrawingLayer PlayerSight = new DrawingLayer("Sight3", new Rectangle(player.GetCenter().X - Sight, player.GetCenter().Y - Sight, Sight * 2, Sight * 2));
				PlayerSight.Draw();
			}

			if(FreezeTimer>0)
				Standard.ClearFadeAnimation();

			if (GameStart)
			{
				Standard.cursor.setPos(450, 480);
				player.setPos(450,480);
				GameStart = false;
			}

			if(ShowMenu)
			{
				Standard.Viewport = new Viewport(MasterInfo.FullScreen);
				MenuLayer.Draw(Color.Black * 0.7f);
				Standard.DrawString("Mouse Sensitivity", new Vector2(ScrollBar_Sensitivity.Frame.GetPosition().X, ScrollBar_Sensitivity.Frame.GetPosition().Y - 20), Color.White);
				ScrollBar_Sensitivity.Draw();
				Standard.DrawString(String.Format("{0:0.0}", (ScrollBar_Sensitivity.MapCoefficient(0.3f, 2.0f))), new Vector2(ScrollBar_Sensitivity.Frame.GetPosition().X + 500, ScrollBar_Sensitivity.Frame.GetPosition().Y), Color.White);
				Standard.DrawString("(Default:1.0)", new Vector2(ScrollBar_Sensitivity.Frame.GetPosition().X + 500, ScrollBar_Sensitivity.Frame.GetPosition().Y+20), Color.White);
				ScrollBar_SongVolume.Draw();
				Standard.DrawString("Song Volume", new Vector2(ScrollBar_SongVolume.Frame.GetPosition().X, ScrollBar_SongVolume.Frame.GetPosition().Y - 20), Color.White);
				ScrollBar_SEVolume.Draw();
				Standard.DrawString("SE Volume", new Vector2(ScrollBar_SEVolume.Frame.GetPosition().X, ScrollBar_SEVolume.Frame.GetPosition().Y - 20), Color.White);

			}

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
			private Point ShotPoint=new Point();

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

			public Point GetCenter()
			{
				return player.GetCenter();
			}

			public void MoveByVector(Point p,double speed)
			{
				player.MoveByVector(p, speed);
			}

			public void reset()
			{
				Standard.cursor.setPos(450, 480);
				setPos(450, 480);
				Standard.Viewport = new Viewport(-getPos().X  + 400, -getPos().Y +  400, 1300, 1300);
				Game1.graphics.GraphicsDevice.Viewport = Standard.Viewport;
				MovePoint = new Point(0, 0);
				AttackTimer = 0;
				AttackIndex = -1;
				isAttacking = false;
				if(IsPlayingEndSong)
				{
					IsPlayingEndSong=false;
					Standard.PlaySong(Standard.Random(0,SongCount),true);			
				}
				for (int i = 0; i < DeadBodys.Count; i++)
				{
					Standard.FadeAnimation(DeadBodys[i], 30,Color.LightSeaGreen);
				}
				DeadBodys.Clear();
				KillerZombieIndex = 0;
			}

			public int getAttackIndex()
			{
				return AttackIndex;
			}

			public int getAttackTimer()
			{
				return AttackTimer;
			}

			public int getAttackSpeed()
			{
				return AttackSpeed;
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
				player = new DrawingLayer("Player_V2",new Rectangle(400,400,80,80));
				bullet = new DrawingLayer("Player2", new Rectangle(0, 0, 80, 80));
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
							if (Score >= 10)
							{
								if (AttackTimer < 3)
									player.MoveTo(ShotPoint.X, ShotPoint.Y, AttackTimer * 5);
								else
									player.MoveTo(ShotPoint.X, ShotPoint.Y, -AttackTimer * (MoveSpeed/2));

							}
						}
						else if (GameMode == 1)//AttackBoost mode
						{
							player.MoveTo(Standard.cursor.getPos().X - 40, Standard.cursor.getPos().Y - 40, MoveSpeed * 2);
							Standard.FadeAnimation(new DrawingLayer("Player", new Rectangle(player.GetPosition(), new Point(80, 80))), 8, Color.Cornsilk);
						}
					}
					if (GameMode == 0)
						MovePoint = new Point(0, 0);
					else if (GameMode == 1)
						MovePoint = Standard.DivPoint(player.GetCenter(), Standard.cursor.getPos(), 0.8);

					if (AutoMouse||Standard.cursor.didPlayerJustRightClick() || Standard.cursor.didPlayerJustLeftClick() || Standard.JustPressed(Keys.A) )
					{
						if (Standard.cursor.didPlayerJustLeftClick())
							ButtonInfoString = "Left-Click";
						else if (Standard.cursor.didPlayerJustRightClick())
							ButtonInfoString = "Right-Click";
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
					}
				
					return;
				}
				if (AutoMouse || Standard.cursor.didPlayerJustRightClick() || Standard.cursor.didPlayerJustLeftClick() || Standard.JustPressed(Keys.A))
				{
					if (Standard.cursor.didPlayerJustLeftClick())
						ButtonInfoString = "Left-Click";
					else if (Standard.cursor.didPlayerJustRightClick())
						ButtonInfoString = "Right-Click";
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
						ShotPoint = enemies[AttackIndex].getPos();
						//ZombieFlip = !ZombieFlip;
						if(Score>=10)
						HappyTimer += 17;
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
					bullet.SetBound(new Rectangle(x + 25, y + 25, AttackTimer * 10, AttackTimer * 10));
					bullet.SetCenter(new Point(x+40, y+40));
					bullet.Draw(MasterInfo.PlayerColor,1f);
					Standard.FadeAnimation(bullet, 10,Color.LightGoldenrodYellow);
					int KillActionTimer = AttackTimer * 2;
					if (GameMode == 0)
						KillActionTimer += 5;
					if(Standard.FrameTimer%2==0||GameMode==0)
					{
						if (Standard.FrameTimer % 20 < 6)
							Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Standard.cursor.getPos().X - 35, Standard.cursor.getPos().Y - 35, 70, 70)), KillActionTimer, Color.Pink);
						else if (Standard.FrameTimer % 20 < 12)
							Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Standard.cursor.getPos().X - 35, Standard.cursor.getPos().Y - 35, 70, 70)), KillActionTimer, Color.PaleVioletRed);
						else
							Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Standard.cursor.getPos().X - 35, Standard.cursor.getPos().Y - 35, 70, 70)), KillActionTimer, Color.SkyBlue);
					}

					Standard.FadeAnimationDraw(Color.Pink);
					Standard.FadeAnimationDraw(Color.PaleVioletRed);
					Standard.FadeAnimationDraw(Color.SkyBlue);

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
					enemy = new DrawingLayer("Player_V2", new Rectangle(Standard.Random(0, 800), Standard.Random(0, 800), 80, 80));
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
				enemy = new DrawingLayer("Player_V2", new Rectangle(x, y, 80, 80));
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

				if(GameMode==0)
					enemy.MoveTo(player.getPos().X + r_1, player.getPos().Y + r_2, ZombieSpeed*3/4);
				else
					enemy.MoveTo(player.getPos().X + r_1, player.getPos().Y + r_2, ZombieSpeed);

				enemy.MoveTo(ZombieCOM.X, ZombieCOM.Y, -ZombieSpeed/3);
				if (player.getAttackTimer()!=0&&GameMode==0)
				{
					int FlipSpeed = 10;

					if (ZombieFlip)
					{
						if (/*Standard.FrameTimer%30<15*/player.getAttackTimer() < player.getAttackSpeed() / 2)
						{
							enemy.MoveTo(player.getPos().X, player.getPos().Y, -FlipSpeed);
							//enemy.MoveByVector(new Point(1, 0), 2);
						}
						else
						{
							/*
							enemy.MoveTo(player.getPos().X, player.getPos().Y, -3 - ZombieSpeed / 3);
							enemy.MoveByVector(new Point(-1, 0), 2);
							*/
							enemy.MoveTo(player.getPos().X, player.getPos().Y, FlipSpeed);
							//enemy.MoveByVector(new Point(1, 0), 2);
						}
					}
					else
					{
						if (/*Standard.FrameTimer%30<15*/player.getAttackTimer() < player.getAttackSpeed() / 2)
						{
							enemy.MoveTo(player.getPos().X, player.getPos().Y, FlipSpeed);
							//enemy.MoveByVector(new Point(1, 0), 2);
						}
						else
						{
							/*
							enemy.MoveTo(player.getPos().X, player.getPos().Y, -3 - ZombieSpeed / 3);
							enemy.MoveByVector(new Point(-1, 0), 2);
							*/
							enemy.MoveTo(player.getPos().X, player.getPos().Y, -FlipSpeed);
							//enemy.MoveByVector(new Point(1, 0), 2);
						}
					}
				}

				if (Standard.Distance(getCenter(), Standard.cursor.getPos()) < 80&&Standard.Distance(player.GetCenter(), Standard.cursor.getPos())>10)
				{
					enemy.MoveTo(Standard.cursor.getPos().X, Standard.cursor.getPos().Y, -3);
				}

				if ((Standard.Distance(player.getPos(), getPos())) <= 10 && Index != player.getAttackIndex())
				{
					GameOver = true;
				}
			}


			
		}
	}
}
