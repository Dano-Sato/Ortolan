﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using System.Linq;

namespace TestSheet
{
	//필요에 따라 여러 개의 테스터 클래스를 만들 수 있습니다. 아마 다음 클래스는 Tester2가 되겠죠.
	public class Tester
	{
		


		

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
			Score.Set(0);
			Fear = 0;
			Gauge = 1;
			Standard.FrameTimer = 0;
			ZombieTime = 40;
			enemies.Clear();
			enemies.Add(new Enemy(false));
			enemies.Add(new Enemy(false));
			Mouse.SetPosition(450, 480);
			player.Reset();
			player.SetMoveSpeed(6);
			bludgers.Clear();
			GameOver = false;
		}

		public static void RemoveEnemy(int k,Color color)
		{
			Rectangle r = enemies[k].getBound();
			enemies.RemoveAt(k);
			int rn = Standard.Random(3, 5);
			for (int i = 0; i < rn; i++)
			{
				int s = Standard.Random(10, 50);
				DrawingLayer newStar;
				Standard.FadeAnimation(newStar = new DrawingLayer("Player2", new Rectangle(r.Center.X - Standard.Random(-30, 30), r.Center.Y - Standard.Random(-30, 30), s, s)), Standard.Random(5 * 3, 15 * 3), color);
				DeadBodys.Add(newStar);
			}
		}
		


		public static Player player;
		public static List<Enemy> enemies=new List<Enemy>();
		public static bool GameOver=false;
		public static SafeInt Score = new SafeInt(0);
		public static int ZombieTime = 40;
		public static double Lightr = 0;//화면이 좀 깜빡거리도록 하기 위해 넣은 변수
		public static DrawingLayer BloodLayer= new DrawingLayer("Blood", new Rectangle(0,0,1300,1000));
		public static List<DrawingLayer> DeadBodys = new List<DrawingLayer>();
		
		public static int ZombieSpeed = 7;
		public static int ScoreStack = 0;
		public static double Fear = 0;
		public static Point OldPlayerPos;
		public static Point OldPlayerDisplacementVector;

		public static int FreezeTimer = -1;//게임오버시 화면을 얼린다.
	

		public static Point Wind = new Point(0, 1);

		public static bool ShowMenu = false;
		public static DrawingLayer MenuLayer=new DrawingLayer("WhiteSpace", new Rectangle(100,50,1000,700));
		public static ScrollBar ScrollBar_Sensitivity = new ScrollBar(new DrawingLayer("BarFrame2",new Rectangle(200,400,500,50)), "Bar2", 50, false);
		public static ScrollBar ScrollBar_SongVolume = new ScrollBar(new DrawingLayer("BarFrame2", new Rectangle(200, 220, 500, 50)), "Bar2", 50, false);
		public static ScrollBar ScrollBar_SEVolume= new ScrollBar(new DrawingLayer("BarFrame2", new Rectangle(200, 290, 500, 50)), "Bar2", 50, false);
		public static DrawingLayer YouDieLayer = new DrawingLayer("Youdie", new Point(200, 500), 1.0f);
		public static DrawingLayer ExitButton = new DrawingLayer("Exit", new Point(850, 650), 1.0f);
		public static DrawingLayer RestartButton = new DrawingLayer("Restart", new Point(650, 650), 1.0f);

        public static List<int> RandomInts = new List<int>();
        public static int RandomIntCounter = 0;

        public static bool IsEndPhase=false;
		public static int FadeTimer = 0;
		//public static DrawingLayer SaveButton = new DrawingLayer("SaveButton", new Point(400, 200), 1f);
		public static Button NextStageButton = new Button(new DrawingLayer("Ladder3", new Point(500, 0), 1f),StartNextStage);
		public static int StartStageTimer = 0;
		public static List<Bludger> bludgers=new List<Bludger>();
		//public static int KillerZombieIndex = -1;

		public static double HeartSignal = 0;
		public static int HoldTimer = 0;

	
		public static List<DrawingLayer> Cards = new List<DrawingLayer>();
		public static int OldCardIndex;
		public static bool ShowCard = true;
		public static bool TheIceRoom = true;
		public static Point CardPos = new Point(200, 400);
		public static int FreezeTime = 210;

		public static double TimeCoefficient = 0.5;
		public static DrawingLayer KillCard = new DrawingLayer("SDead_1", new Point(0,0), 0.8f);

		public static List<Card> Rewards = new List<Card>();

		public static List<int> MonsterDeck = new List<int>();

        public static DrawingLayer TutorialCard = new DrawingLayer("Tutorial01", new Point(0, 0), 0.67f);

		/*오버클럭 처리용 변수*/
		public static int PressedATimer;
		public static double Gauge = 1;
		public static bool SlowMode = false;
		public static bool TimeSleeper = false;//2초에 한번씩 타이머를 멈춰 체감시간과 실제 타이머 작동시간을 맞춘다.

		public static Phase GamePhase = Phase.Main;

		
		public enum Phase { Main, Tutorial, Game, Ending, Dead};

        public static Button StartButton = new Button(new StringLayer("Game Start", new Vector2(400, 600)), () => GamePhase = Phase.Tutorial);
		public static Button GameExitButton = new Button(new StringLayer("Exit", new Vector2(700, 600)), Exit);

        public static Button RetryButton = new Button(new StringLayer("Retry", new Vector2(600, 600)), () => GamePhase = Phase.Main);

        public static bool LiteMode = true;


		//public static int PressedATimer = 0;
		public void AddCard(int i)
		{
			Cards.Add(new DrawingLayer("RoomCard_" + i, CardPos, 0.75f));
		}

		public void AddReward()
		{
			Rewards.Add(new Card(Table.Pick(),Card.CardClass.Reward));
		}

		public static void Exit()
		{
			Game1.GameExit = true;
		}

		public static void StartNextStage()
		{
            /*
            if(true)
            {
                GamePhase = Phase.Dead;
                return;
            }*/
			Room.Number = MonsterDeck[0];
			MonsterDeck.RemoveAt(0);
			StartStageTimer = 200;
		}

		public int MouseIsOnCardsIndex()
		{
			int MouseIndex = -1;
			for(int i=0;i<Cards.Count;i++)
			{
				if(Cursor.IsOn(Cards[i]))
				{
					MouseIndex=i;
				}
			}
			return MouseIndex;
		}

		public static void GameInit()
		{
			Rewards.Clear();
			MonsterDeck.Clear();
			MonsterDeck.Add(0);
			MonsterDeck.Add(14);
			MonsterDeck.Add(15);
			MonsterDeck.Add(16);

			MonsterDeck.Add(1);
			MonsterDeck.Add(2);
			MonsterDeck.Add(13);
			MonsterDeck.Add(3);
			MonsterDeck.Add(5);
			MonsterDeck.Add(77);
			MonsterDeck.Add(66);
            /*
			MonsterDeck.Add(4);
			MonsterDeck.Add(6);
			MonsterDeck.Add(777);
			MonsterDeck.Add(666);
            */

			IsEndPhase = true;

			Checker.Init();
			Table.Init();
			Standard.PlayLoopedSong("WindOfTheDawn");
			GamePhase = Phase.Game;
		}

		//이후 마음대로 인수 혹은 콘텐츠들을 여기 추가할 수 있습니다.
		public Tester()//여기에서 각종 이니셜라이즈가 가능합니다.
		{
            TutorialCard.SetSprite("EmptySpace");
        player = new Player();
			enemies.Add(new Enemy(false));
			ScrollBar_Sensitivity.Initialize(0.5f);
            ScrollBar_SongVolume.Initialize(0f);
            ScrollBar_SEVolume.Initialize(0f);

            //Room.Number = 1;
            //Room.Set();
            Room.Init();
			AddCard(0);
			AddCard(1);
			AddCard(3);
			AddCard(4);
			AddCard(2);
			AddCard(5);
			AddCard(6);
			AddCard(13);

			AddCard(77);
			AddCard(777);
			AddCard(66);
			AddCard(666);


		}
		//Game1.Class 내에 Tester.Update()로 추가될 업데이트문입니다. 다양한 업데이트 처리를 시험할 수 있습니다.
		//그림으로 그려지기 이전 각종 변수들의 처리를 담당합니다.
		public void Update()
		{
			/*기타*/

			UpdateScore();

            #region 커서 애니메이션 처리
            /*커서 애니메이션 처리*/
            string ClickSprite;
			if (Standard.FrameTimer % 30 < 15)
				ClickSprite = "Click";
			else
				ClickSprite = "Click2";
			DrawingLayer Click = new DrawingLayer(ClickSprite, new Rectangle(Cursor.GetPos().X - 15, Cursor.GetPos().Y - 15, 30, 30));
			if (!IsEndPhase||GamePhase==Phase.Ending)
                
			{
				Standard.FadeAnimation(Click, 10, Color.OrangeRed);
				Standard.FadeAnimation(Click, 10, Color.AliceBlue);
			}
			else
			{
				Standard.FadeAnimation(Click, 10, Color.Black);
				Standard.FadeAnimation(Click, 10, Color.DarkGray);
			}
            #endregion

            MasterInfo.SetFullScreen(ScrollBar_Sensitivity.Coefficient * 4 + 1f);

			switch (GamePhase)
			{
				case Phase.Main:
					if (Standard.SongName != "TitleSong")
					{
						Standard.PlayLoopedSong("TitleSong");
					}
					Room.RoomColor = Color.Black;
					if (Standard.Pressing(Keys.S))
					{
						GameInit();
					


					}
					if (Standard.Pressing(Keys.Escape))
						Game1.GameExit = true;
					StartButton.Enable();
					GameExitButton.Enable();
					break;
                case Phase.Tutorial:
                    if (Standard.SongName != "TutorialSong")
                    {
                        Standard.PlayLoopedSong("TutorialSong");
                    }
                    if (Cursor.JustdidLeftClick())
                    {
                        if (TutorialCard.GetSpriteName() == "EmptySpace")
                            TutorialCard.SetSprite("Tutorial01");
                        else if (TutorialCard.GetSpriteName() == "Tutorial01")
                            TutorialCard.SetSprite("Tutorial02");
                        else if (TutorialCard.GetSpriteName() == "Tutorial02")
                        {
                            TutorialCard.SetSprite("EmptySpace");
                            GameInit();
                        }                    
                    }
                    break;
                case Phase.Game:



					/* 게임오버 처리*/

					if (GameOver && FreezeTimer == -1)
					{
						FreezeTimer = FreezeTime;
						Standard.ClearFadeAnimation();
						Standard.PlaySE("ZombieSound4");
						PressedATimer = 0;
						return;
					}
					if (FreezeTimer > 0)
					{
						Standard.ClearFadeAnimation();
						if (FreezeTimer == FreezeTime)
							Standard.PlayFadedSE("KnifeSound", 0.3f);
						if (FreezeTimer == FreezeTime - 60)
							Standard.PlayFadedSE("KnifeSound", 0.5f);
						if (FreezeTimer == FreezeTime - 110)
							Standard.PlayFadedSE("KnifeSound", 0.5f);
						FreezeTimer--;
						Fear += 3;
						PressedATimer = 0;
						return;
					}
					if (FreezeTimer == 0)
					{
						if (Checker.Hearts == 0)
						{
                            //GamePhase = Phase.Main;
                            GamePhase = Phase.Dead;
							Standard.FrameTimer = 0;
							return;
						}
						FreezeTimer = -1;
						Standard.FadeAnimation(new DrawingLayer("YouDied", new Rectangle(200, 350, 400, 200)), 90, Color.DarkRed);
						Standard.FadeAnimation(new DrawingLayer("Tip", new Rectangle(500, 350, 400, 200)), 150, Color.DarkRed);
						Room.Set();
					}


					if (!ShowMenu)
					{
						player.MoveUpdate();
						player.AttackUpdate();
					}

					if (DeadBodys.Count > 300)
					{
						DeadBodys.RemoveAt(0);
					}




                    /*키보드 입력 처리*/
                    if(Standard.JustPressed(Keys.H))
                    {
                        int w = Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
                        int h = Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;

                        //force a frame to be drawn (otherwise back buffer is empty) 
                        //Game1.spriteBatch.Draw(new GameTime());

                        //pull the picture from the buffer 
                        int[] backBuffer = new int[w * h];
                        Game1.graphics.GraphicsDevice.GetBackBufferData(backBuffer);

                        //copy into a texture 
                        Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, w, h, false, Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat);
                        texture.SetData(backBuffer);

                        //save to disk 
                        Stream stream = File.OpenWrite("TestGameCapture" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")+".jpg");

                        texture.SaveAsJpeg(stream, w, h);
                        stream.Dispose();
                        texture.Dispose();
                    }

                    if (Standard.JustPressed(Keys.C))//핏자국 청소
					{
						for (int i = 0; i < DeadBodys.Count; i++)
						{
							Standard.FadeAnimation(DeadBodys[i], 30, Color.LightSeaGreen);
						}
						DeadBodys.Clear();
					}
					if (Standard.JustPressed(Keys.Escape)||Cursor.JustDidScrollButton())//세팅으로
					{
						ShowMenu = !ShowMenu;
					}

					/*오버클럭 모드 처리*/


					if (FreezeTimer==-1&&!ShowMenu)
					{
						if((Cursor.IsLeftClickingNow()||Cursor.IsRightClickingNow())&&!IsEndPhase)
						{
							Gauge = Math.Max(0, Gauge - 0.015);
							if (Gauge == 0)
								SlowMode = false;
							else
								SlowMode = true;
						}
						else
						{

							Gauge = Math.Min(1, Gauge + 0.003);
							SlowMode = false;
						}
					}
					if (Standard.JustPressed(Keys.T))
					{
						Score.Set(95);
					}

					if (SlowMode)
					{
						TimeSleeper = !TimeSleeper;
						if (TimeSleeper)
							Standard.FrameTimer--;
						player.SetMoveSpeed(3);
						PressedATimer = Math.Min(25, PressedATimer + 1);
					}
					else
					{
						player.SetMoveSpeed(6);
						PressedATimer = Math.Max(0, PressedATimer - 3);
					}





                    /*ESC 메뉴 처리*/

                    if (ShowMenu)
					{

						ScrollBar_Sensitivity.Update();
						ScrollBar_SongVolume.Update();
						ScrollBar_SEVolume.Update();
						
						if (!IsEndPhase)
							Standard.SetSongVolume(ScrollBar_SongVolume.Coefficient);
						Standard.SetSEVolume(ScrollBar_SEVolume.Coefficient);
						if (ExitButton.MouseJustLeftClickedThis())
							Game1.GameExit = true;
						if (RestartButton.MouseJustLeftClickedThis())
						{
							StartStageTimer = 0;
							IsEndPhase = true;
							Room.RoomColor = Color.AntiqueWhite;
							Room.StarColor = Color.Yellow;
							Standard.DisposeSE();
							Standard.DisposeSong();
							//Standard.PlayLoopedSE("WindOfTheDawn");
							FadeTimer = 100;
							ScoreStack = 0;
							ShowMenu = false;
							GamePhase = Phase.Main;
							Standard.FrameTimer = 0;
						}
						return;
					}

					/*좀비들의 이동 처리.*/



					HeartSignal = 0;

                    RandomInts.Clear();
                    for(int i=0;i<15;i++)
                    {
                        RandomInts.Add(Standard.Random(-300, 300));
                    }
					RandomIntCounter = 0;
                
                    Parallel.For(0, enemies.Count, (i) => {
                        double r = Method2D.Distance(enemies[i].enemy.GetPos(), player.GetPos());
                        HeartSignal += (1600.0 / (r * r));

                        if (i == player.getAttackIndex())
                            return;
                        if (RandomIntCounter >= 14)
                            RandomIntCounter = 0;
                        enemies[i].MoveUpdate();
                        RandomIntCounter++;
                    });

                    /*좀비 생성 작업*/

                    if (TheIceRoom)
						ZombieTime = Math.Max(10 - Score.Get() / 10, 5) + 25;//좀비 생성 시간은 스코어가 높을수록 빨라진다.
					else
						ZombieTime = Math.Max(10 - Score.Get() / 10, 5) + 20;//좀비 생성 시간은 스코어가 높을수록 빨라진다.
					if (Room.Number == 0)
						ZombieTime = 20 - Score.Get() / 10;
                    if (Room.Number == 0&&LiteMode)
                        ZombieTime = 30 - Score.Get() / 10;


                    if (Standard.FrameTimer % ZombieTime == 0)
					{
						if (!(Score.Get() < 10 && enemies.Count > 10))
							enemies.Add(new Enemy(false));
						if (enemies.Count > 150 && !player.IsAttacking())
							enemies.RemoveAt(0);
					}


					/*플레이어가 손뗐나 확인*/
					if (OldPlayerPos == player.GetPos() && !ShowMenu)
					{
						HoldTimer++;
					}
					else
					{
						HoldTimer = 0;
					}
					if (HoldTimer > 300)
					{
						for (int i = 0; i < enemies.Count; i++)
						{
							enemies[i].enemy.MoveTo(player.GetPos().X, player.GetPos().Y, 15);
						}
					}

					/*뷰포트 처리*/

					Point PlayerDisPlacementVector = Method2D.Deduct(player.GetPos(), OldPlayerPos);
					Point ViewportDisplacement = Method2D.Deduct(PlayerDisPlacementVector, OldPlayerDisplacementVector);
					Point CursorDisplacement = Method2D.Deduct(player.GetPos(), Cursor.GetPos());
					int Dis = Method2D.Abs(CursorDisplacement);

					if (Dis > 250)
					{
						CursorDisplacement = new Point(CursorDisplacement.X * 250 / Dis, CursorDisplacement.Y * 250 / Dis);
					}

					OldPlayerPos = player.GetPos();
					OldPlayerDisplacementVector = PlayerDisPlacementVector;

					if (SlowMode)
					{						
						Game1.graphics.GraphicsDevice.Viewport = new Viewport(-player.GetPos().X + CursorDisplacement.X / 4 + ViewportDisplacement.X / 2 + 400, -player.GetPos().Y + CursorDisplacement.Y / 4 + ViewportDisplacement.Y / 2 + 400, 
							1300, 1000);
					}
					else
					{
						Game1.graphics.GraphicsDevice.Viewport = new Viewport(-player.GetPos().X + CursorDisplacement.X / 4 + ViewportDisplacement.X / 2 + 400, -player.GetPos().Y + CursorDisplacement.Y / 4 + ViewportDisplacement.Y / 2 + 400, 
							1300, 1000);
					}








					/*엔드페이즈 처리*/
					if (Score.Get() == 100)
					{
						IsEndPhase = true;
						Standard.DisposeSE();
						Standard.FadeOutSong(100);
						Standard.PlayLoopedSong("WindOfTheDawn");

						FadeTimer = 100;
						ScoreStack = 0;
						Rewards.Clear();
						for (int i = 0; i < Room.ClearRewardCount(); i++)
						{
							AddReward();
						}
					}
					if (IsEndPhase && StartStageTimer == 0)
					{
						bludgers.Clear();
						Room.RoomColor = Color.AntiqueWhite;
						Room.StarColor = Color.Yellow;

						if (FadeTimer > 0)
						{
							FadeTimer--;
						}
						if (enemies.Count > 0)
							RemoveEnemy(0, Color.LightGoldenrodYellow);
						if (FadeTimer == 0)
							NextStageButton.Enable();
		
						Fear = 0;
						Score.Set(0);

						for (int i = 0; i < Rewards.Count; i++)
						{
							Rewards[i].Update();
							Rewards[i].Frame.CenterMoveTo(CardPos.X + (Card.CardWidth + 10) * i, CardPos.Y, 50);
							if (FadeTimer > 50)
								Rewards[i].Frame.SetRatio((1.5f * (FadeTimer - 50) + (0.75) * (100 - FadeTimer)) / 50f);

							if (Rewards[i].RemoveTimer == 0)
							{
								Rewards.RemoveAt(i);
								i--;
							}
						}


					}
					if (StartStageTimer > 0)
					{
						StartStageTimer--;
						if (enemies.Count > 0)
							RemoveEnemy(0, Color.LightGoldenrodYellow);
						if (StartStageTimer == 100)
						{
							Standard.DisposeSE();
							Standard.DisposeSong();
							IsEndPhase = false;
							Room.Set();

						}
						if (StartStageTimer > 100)
						{
							Fear += 10;
						}
						else
						{
							Fear -= 10;
						}
						Standard.FadeSong(Math.Max(0, (float)(1 - StartStageTimer / 100.0)));

					}

					for (int i = 0; i < bludgers.Count; i++)
					{
						bludgers[i].MoveUpdate();
						double r = Method2D.Distance(bludgers[i].bludger.GetPos(), player.GetPos());
						HeartSignal += (1600.0 / (r * r));
					}


					Room.Update();
					Checker.Update();

					Point p = Method2D.Deduct(Cursor.GetPos(), player.GetCenter());

			

					break;//Game Phase Update
                case Phase.Ending:
                    if (Standard.SongName != "EndingSong")
                    {
                        Standard.PlayLoopedSong("EndingSong");
                    }
                    break;
                case Phase.Dead:
                    if (Standard.SongName != "GameOverSong")
                    {
                        Standard.PlayLoopedSong("GameOverSong");
                    }
                    RetryButton.Enable();

                    break;

			}
	

		}

   
		//Game1.Class 내에 Tester.Draw()로 추가될 드로우 액션문입니다. 다양한 드로잉을 시험할 수 있습니다.
		public void Draw()
		{
        
            switch (GamePhase)
			{
				case Phase.Main:
                    DrawingLayer s = new DrawingLayer("Title3",new Point(0,0),0.67f);
                    s.Draw();
					StartButton.Draw(Color.White,Color.Red);
					GameExitButton.Draw(Color.White, Color.Red);
                    Standard.DrawLight(MasterInfo.FullScreen, Color.Black, (float)(Math.Abs(90 - Standard.FrameTimer % 180)/500.0)+0.2f, Standard.LightMode.Absolute);
					break;
                case Phase.Tutorial:
                    TutorialCard.Draw();
                    if (Standard.FrameTimer % 10 == 0)
                    {
                        Lightr = Standard.Random() / 10.0;
                    }

                    if(TutorialCard.GetSpriteName()=="EmptySpace")
                    {
                        DrawingLayer Choice = new DrawingLayer("Choice", new Point(0, 0), 0.8f);
                        Choice.Draw();
                    }
                    else
                    {
                        Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 0.05f + (float)Lightr, Standard.LightMode.Absolute);
                        Standard.DrawLight(MasterInfo.FullScreen, Color.DarkBlue, 0.05f, Standard.LightMode.Absolute);


                        Standard.DrawLight(new Rectangle(0, 0, MasterInfo.PreferredScreen.Width * 4, MasterInfo.PreferredScreen.Height * 4), Color.White, 1f, Standard.LightMode.Vignette);
                        //스코어 올라갈수록 보라색을 띈다.
                        Standard.DrawLight(MasterInfo.FullScreen, Color.Purple, 0.05f, Standard.LightMode.Absolute);
                    }
                    break;
				case Phase.Game:
					/*배경 드로우. 핏자국, 별들*/
					BloodLayer.Draw(Room.StarColor, Math.Min(10, Score.Get()) * 0.1f);
					for (int i = 0; i < DeadBodys.Count; i++)
					{
						if (!IsEndPhase)
						{
							DeadBodys[i].MoveByVector(Wind, ZombieSpeed);
							if (DeadBodys[i].GetPos().Y > MasterInfo.FullScreen.Height)
								DeadBodys[i].SetPos(DeadBodys[i].GetPos().X, 0);
							DeadBodys[i].Draw(Room.StarColor, Math.Min(10, Score.Get()) * 0.1f);
						}
						else
						{
							DeadBodys[i].Draw(Color.LightGoldenrodYellow, Math.Min(10, Score.Get()) * 0.1f);
							DeadBodys[i].MoveTo(player.GetPos().X, player.GetPos().Y, 10);
							if (Method2D.Distance(DeadBodys[i].GetPos(), player.GetPos()) < 10)
							{
								DeadBodys.RemoveAt(i);
								i--;
							}
						}
					}

					if (!IsEndPhase)
					{
						if (YouDieLayer.GetSpriteName() != "Youdie")
							YouDieLayer = new DrawingLayer("Youdie", new Point(200, 500), 1.0f);
					}
					else
					{
						if (YouDieLayer.GetSpriteName() != "Stage1")
							YouDieLayer = new DrawingLayer("Stage1", new Point(200, 500), 1.5f);
						YouDieLayer.Draw(Color.LightGoldenrodYellow, (float)(2 * FadeTimer / 100.0));
						YouDieLayer.SetPos(-Game1.graphics.GraphicsDevice.Viewport.X + 100, -Game1.graphics.GraphicsDevice.Viewport.Y + 300);
					}
					Standard.FadeAnimationDraw(Color.LightSeaGreen);//별이 사라지는 페이드애니메이션(컬러는 LighteaGreen으로 지정)은 아래 라이트레이어 전에 발생해야 보기 좋으므로 별도로 처리함.

					/*풀스크린 라이트 레이어 처리*/

					if (!IsEndPhase)
					{
						Standard.DrawLight(new Rectangle(0,0,MasterInfo.PreferredScreen.Width*4, MasterInfo.PreferredScreen.Height*4), Color.White, 1f, Standard.LightMode.Vignette);
						//스코어 올라갈수록 보라색을 띈다.
						Standard.DrawLight(MasterInfo.FullScreen, Color.Purple, 0.3f * Math.Min(1.2f, (float)(Score.Get() / 100.0)), Standard.LightMode.Absolute);

                    }
                    if (IsEndPhase)
                    {
                        NextStageButton.Draw();
                        for (int i = 0; i < Rewards.Count; i++)
                        {
                            Rewards[i].Draw();
                        }

                    }
                    player.Draw();
					player.DrawAttack();






                    for (int i = 0; i < enemies.Count; i++)
					{
						enemies[i].Draw();
                        enemies[i].DrawAddOn();					
					}

					for (int i = 0; i < bludgers.Count; i++)
					{
						Tester.bludgers[i].Draw();
					}

					if (!IsEndPhase && Standard.FrameTimer % 10 == 0)
					{
						Lightr = Standard.Random() / 10.0;
					}

                    /*플레이어 사망시 물어뜯는 연출*/
                    if (GameOver)
					{
						if (FreezeTimer < 50)
						{
							for (int i = 0; i < enemies.Count; i++)
							{
								enemies[i].enemy.MoveTo(player.GetPos().X + Standard.Random(-30, 30), player.GetPos().Y + Standard.Random(-30, 30), Standard.Random(5, 15));
							}
						}
					}

					if (!IsEndPhase)
					{
						Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 0.2f + (float)Lightr, Standard.LightMode.Absolute);
						Standard.DrawLight(MasterInfo.FullScreen, Color.DarkBlue, 0.3f, Standard.LightMode.Absolute);
					}


					/*스프라이트 바꾸기 장난*/

					if (ScoreStack != 0 && Score.Get() > 10)
					{
						if (Score.Get() % 30 == 23 || Score.Get() % 30 == 24)
						{
							for (int i = 0; i < enemies.Count; i++)
							{
								enemies[i].enemy.SetSprite("Player_Broken2");
							}
						}
						else if (Score.Get() % 30 == 25)
						{
							for (int i = 0; i < enemies.Count; i++)
							{
								enemies[i].enemy.SetSprite("Player_V6");
							}
						}
						else if (Score.Get() % 30 == 22 || Score.Get() % 30 == 21)
						{
							for (int i = 0; i < enemies.Count; i++)
							{
								enemies[i].enemy.SetSprite("Player_Broken");
							}
						}
						if (Score.Get() % 100 == 87)
						{

							for (int i = 0; i < enemies.Count; i++)
							{
								enemies[i].enemy.SetSprite("Tip");
							}
						}
						if (Score.Get() % 70 == 9)
						{
							for (int i = 0; i < enemies.Count; i++)
							{
								enemies[i].enemy.SetSprite("Player2");
								Standard.FadeAnimation(enemies[i].enemy, 30, Color.White);
							}
						}



					}



                    #region 시야처리
                    if (!ShowMenu)
					{
						for (int i = 0; i < enemies.Count; i++)
						{
							int r = Method2D.Distance(enemies[i].getCenter(), player.GetCenter());
							Fear = Math.Min(Fear + 400.0 / Math.Pow(Math.Max(r, 9), 2), 200);
						}
						if (player.IsAttacking())
							Fear = Math.Max(0, Fear - Math.Max(1, enemies.Count / 10));
						int Sight = Math.Max(800 - (int)(Fear * 4) - Math.Min(PressedATimer * 30, 250) + Standard.Random(-5, 5), player.getRange() + 50);
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

					Standard.DrawLight(BloodLayer.GetBound(), Color.DarkBlue, (float)(PressedATimer / 100.0), Standard.LightMode.Absolute);

					if (FreezeTimer > 0)
						Standard.ClearFadeAnimation();
				
					Standard.DrawAddon(BloodLayer, Room.RoomColor, 1f, "Wall3");               
                    #endregion

                    #region 세팅화면 처리
                    if (ShowMenu)
					{
						Game1.graphics.GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
						MenuLayer.Draw(Color.Black * 0.7f);
						Standard.DrawString("Mouse Sensitivity", new Vector2(ScrollBar_Sensitivity.Frame.GetPos().X, ScrollBar_Sensitivity.Frame.GetPos().Y - 20), Color.White);
						ScrollBar_Sensitivity.Draw();
						Standard.DrawString(String.Format("{0:0.0}", ScrollBar_Sensitivity.Coefficient+0.5f), new Vector2(ScrollBar_Sensitivity.Frame.GetPos().X + 500, ScrollBar_Sensitivity.Frame.GetPos().Y), Color.White);
						Standard.DrawString("(Default:1)", new Vector2(ScrollBar_Sensitivity.Frame.GetPos().X + 500, ScrollBar_Sensitivity.Frame.GetPos().Y + 20), Color.White);
						ScrollBar_SongVolume.Draw();
						Standard.DrawString("Song Volume", new Vector2(ScrollBar_SongVolume.Frame.GetPos().X, ScrollBar_SongVolume.Frame.GetPos().Y - 20), Color.White);
						ScrollBar_SEVolume.Draw();
						Standard.DrawString("SE Volume", new Vector2(ScrollBar_SEVolume.Frame.GetPos().X, ScrollBar_SEVolume.Frame.GetPos().Y - 20), Color.White);
						ExitButton.Draw();
						if (ExitButton.MouseIsOnThis())
							ExitButton.Draw(Color.DarkRed);
						RestartButton.Draw();
						if (RestartButton.MouseIsOnThis())
							RestartButton.Draw(Color.DarkRed);
                        DrawingLayer SCGSample = new DrawingLayer("SCGSample", new Point(600,0), 0.5f);
                        if (Checker.Hearts < 5)
                        {
                            SCGSample.SetSprite("SCG_Dying");
                        }
                        else if (Checker.Hearts < 25)
                            SCGSample.SetSprite("SCGSample");
                        else
                            SCGSample.SetSprite("SCG_Happy");

                        SCGSample.Draw();
						Standard.FrameTimer--;
					}
                    #endregion

                    #region 좀비의 얼굴 그리기
                    if (!GameOver && !ShowMenu)
					{
                       for(int i=0; i<enemies.Count;i++)
                       enemies[i].DrawAddOn();
					}
                    if (GameOver)
                    {
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            if(enemies[i].ThisIsTheKiller)
                            {
                                Standard.DrawAddon(enemies[i].enemy, Color.Black, 1f, "Player");
                                if (Standard.FrameTimer % 20 <= 10)
                                    Standard.DrawAddon(enemies[i].enemy, Color.White, 1f, "ZombieBite");
                                else
                                    Standard.DrawAddon(enemies[i].enemy, Color.White, 1f, "ZombieBite2");
                            }
                        }

					}
                    #endregion


                    /*스테이지 시작시 연출*/

                    if (StartStageTimer > 80 && StartStageTimer < 120)
					{
						Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 1f, Standard.LightMode.Absolute);
					}

					if (StartStageTimer > 0 && StartStageTimer < 100)
					{
						Standard.DrawString("BigFont", Room.Name(), new Vector2(-Game1.graphics.GraphicsDevice.Viewport.X + 400, -Game1.graphics.GraphicsDevice.Viewport.Y + 300), Color.White * (float)(StartStageTimer / 100.0));
					}
			
					break;//Game Phase Draw
                case Phase.Ending:

                    Game1.graphics.GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
                    DrawingLayer EndingCut = new DrawingLayer("Ending2", new Point(-200,-100), 0.80f);                  
                    EndingCut.Draw();
                    Standard.DrawLight(EndingCut, Color.AliceBlue, (float)((Math.Max(250 - Standard.FrameTimer % 250, Standard.FrameTimer % 250)-100 )/ 450.0), Standard.LightMode.Absolute);

                    break;
                case Phase.Dead:
                    Game1.graphics.GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
                    DrawingLayer DeadCut = new DrawingLayer("Dead_1", new Point(0, 0), 0.67f);
                    int Checkpoint = 1000;
                    if(Standard.FrameTimer < Checkpoint)
                    {
                        if (Standard.FrameTimer % 250 < 125)
                            DeadCut.SetSprite("Dead_2");
                    }
                    else
                    {
                        int Interval = 50;
                        if (Standard.FrameTimer < Checkpoint + Interval)
                            DeadCut.SetSprite("Dead_3");
                        else if (Standard.FrameTimer < Checkpoint + 2*Interval)
                            DeadCut.SetSprite("Dead_4");
                        else if (Standard.FrameTimer < Checkpoint + 3 * Interval)
                            DeadCut.SetSprite("Dead_5");
                        else if (Standard.FrameTimer < Checkpoint + 4 * Interval)
                            DeadCut.SetSprite("Dead_6");
                        else
                            DeadCut.SetSprite("Dead_7");
                    }


                    DeadCut.Draw();
                    Standard.DrawLight(DeadCut, Color.Black, (float)((Math.Max(250 - Standard.FrameTimer % 250, Standard.FrameTimer % 250) - 100) / 450.0), Standard.LightMode.Absolute);
                    RetryButton.Draw(Color.White,Color.Red);

                    break;               
            }
            #region Debug Setting
            if (Standard.Pressing(Keys.LeftControl,Keys.Q))
                Standard.DrawString(Cursor.GetPos().X.ToString() + "," + Cursor.GetPos().Y.ToString(), new Vector2(Cursor.GetPos().X-20, Cursor.GetPos().Y-30), Color.White);
            if (Standard.Pressing(Keys.LeftControl, Keys.W))
                Standard.DrawString(Standard.FrameTimer.ToString(), new Vector2(Cursor.GetPos().X - 20, Cursor.GetPos().Y - 30), Color.White);

            #endregion

        }


        public class Player
		{
			public DrawingLayer player;
			private Point MovePoint=new Point(0,0);
			private int Range=150;
			private int MoveSpeed=6;
			private int AttackSpeed = 15;
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

			public Point GetCenter()
			{
				return player.GetCenter();
			}

			public void MoveByVector(Point p,double speed)
			{
				player.MoveByVector(p, speed);
			}

			public void Reset()
			{
				Cursor.SetPos(450, 480);
				setPos(450, 480);
				Game1.graphics.GraphicsDevice.Viewport = new Viewport(-GetPos().X  + 400, -GetPos().Y +  400, 1300, 1300);
				MovePoint = new Point(0, 0);
				AttackTimer = 0;
				AttackIndex = -1;
				isAttacking = false;         
				DeadBodys.Clear();
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

			public Point GetPos()
			{
				return player.GetPos();
			}

			public void setPos(int x, int y)
			{
				player.SetPos(x, y);
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
				player = new DrawingLayer("Player_V6",new Rectangle(400,400,70,70));
				player.AttachAnimation(30, "Player_Ani1", "Player_Ani2");
			}

			public void Draw()
			{			
				player.Draw(Color.White);				
			}

			public void MoveUpdate()
			{
				player.Animate(true);
				if (isAttacking)
				{
					if (AttackTimer < AttackSpeed - 3)
					{
						player.MoveTo(Cursor.GetPos().X - 40, Cursor.GetPos().Y - 40, MoveSpeed+4);
						Standard.FadeAnimation(new DrawingLayer("Player_AfterImage", new Rectangle(player.GetPos(), new Point(70, 70))), 5, Color.AliceBlue);	
					}
					MovePoint = Method2D.DivPoint(player.GetCenter(), Cursor.GetPos(), 0.8);

					
					for(int i=0;i<enemies.Count;i++)
					{
						if (enemies[i].getBound().Contains(Cursor.GetPos()) && Method2D.Distance(GetCenter(), enemies[i].getCenter()) < Range)
						{
							return;
						}
					}
					MovePoint = Cursor.GetPos();				
					return;
				}
				if(!IsEndPhase&&StartStageTimer==0)
				{
					for (int i = 0; i < enemies.Count; i++)
					{
						if (enemies[i].getBound().Contains(Cursor.GetPos()) && Method2D.Distance(GetCenter(), enemies[i].getCenter()) < Range)
						{
							AttackIndex = i;
							isAttacking = true;
							AttackTimer = AttackSpeed;
                            if (enemies[i].IsGhost)
                                return;
						}

					}
				}			
				MovePoint = Cursor.GetPos();	
				if (MovePoint.X!=0||MovePoint.Y!=0)
					player.MoveTo(MovePoint.X - 40, MovePoint.Y - 40,MoveSpeed+Checker.Swiftness * 1.0/3.0);
			}

			public void AttackUpdate()
			{
				if(isAttacking)
				{
					if(AttackTimer==AttackSpeed&&AttackIndex!=-1&&enemies.Count>AttackIndex)
					{
						enemies[AttackIndex].enemy.SetSprite("Player_Broken2");
						Standard.FadeAnimation(enemies[AttackIndex].enemy, 15, Color.AntiqueWhite);
						Standard.PlayFadedSE("KnifeSound",0.4f);
						Standard.PlayFadedSE("GunSound",0.3f);
					}
					if (AttackTimer>0)//투사체 날아가는중
					{
						AttackTimer--;
						return;
					}
					else//투사체 적중
					{
						if(enemies.Count>AttackIndex&&AttackIndex!=-1)
						{
							if (enemies[AttackIndex].IsGhost)
								RemoveEnemy(AttackIndex, Color.AliceBlue);
							else
								RemoveEnemy(AttackIndex, Color.DarkRed);
						}
						ScoreStack++;

						isAttacking = false;
						AttackIndex = -1;
						return;
					}
				}
			}

			public void DrawAttack()
			{
				if(isAttacking&&AttackTimer>=1&&enemies.Count>AttackIndex&&AttackIndex!=-1)
				{
					int x = ((AttackSpeed-AttackTimer) * enemies[AttackIndex].GetPos().X + AttackTimer * GetPos().X)/AttackSpeed;
					int y = ((AttackSpeed - AttackTimer) * enemies[AttackIndex].GetPos().Y + AttackTimer * GetPos().Y)/AttackSpeed;

					
					int KillActionTimer = AttackTimer * 2;
			
					if(!ShowMenu&& Standard.FrameTimer%5==0)
					{
						if (Standard.FrameTimer % 20 < 6)
							Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Cursor.GetPos().X/2+enemies[AttackIndex].enemy.GetPos().X/2, Cursor.GetPos().Y / 2 + enemies[AttackIndex].enemy.GetPos().Y / 2, 70, 70)), 15, Color.Pink);
						else if (Standard.FrameTimer % 20 < 12)
							Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Cursor.GetPos().X / 2 + enemies[AttackIndex].enemy.GetPos().X / 2, Cursor.GetPos().Y / 2 + enemies[AttackIndex].enemy.GetPos().Y / 2, 70, 70)), 15, Color.PaleVioletRed);
						else
							Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Cursor.GetPos().X / 2 + enemies[AttackIndex].enemy.GetPos().X / 2, Cursor.GetPos().Y / 2 + enemies[AttackIndex].enemy.GetPos().Y / 2, 70, 70)), 15, Color.SkyBlue);
					}


		
				}
			}
		}

		public class Enemy
		{
			public DrawingLayer enemy;
			public bool IsGhost = false;
            public bool ThisIsTheKiller = false;
			private double GhostAngle;
			private double Ghostw = 0.03;
			private double GhostDistance = 800;
            private event Action MoveAction;
            private event Action DrawAction;
            private event Action DrawAddonAction;
            private event Action SDeadAction;

            public Point GetPos()
			{
				return enemy.GetPos();
			}

			public Point getCenter()
			{
				return enemy.GetCenter();
			}

			public Rectangle getBound()
			{
				return enemy.GetBound();
			}
			public Enemy(bool isGhost)
			{
				IsGhost = isGhost;
                DrawAddonAction += () => Standard.DrawAddon(enemy, Color.White, 1f, "NormalZombie");
                DrawAddonAction += () => Standard.DrawAddon(enemy, Color.Cornsilk, 0.3f, "NormalZombie");
                if (IsGhost)
				{
					enemy = new DrawingLayer("Player_V6", new Rectangle(0,0, 100, 100));
					GhostAngle = Standard.Random() * 3;
                    MoveAction += Ghost_MoveAction;
                    DrawAction += Ghost_DrawAction;
                    SDeadAction += () => Tester.KillCard = new DrawingLayer("SDead_22", new Point(0, 0), 0.6f);
                    DrawAddonAction += () =>
                    {
                        if (Standard.FrameTimer % 50 < 25)
                        {
                            Standard.DrawAddon(enemy, Color.LightSkyBlue, 0.5f, "GhostHead_1");
                            if (Standard.FrameTimer % 15 == 0)
                                Standard.FadeAnimation(new DrawingLayer("GhostHead_1",enemy.GetBound()), 10, Color.CornflowerBlue);
                        }
                        else
                        {
                            Standard.DrawAddon(enemy, Color.LightSkyBlue, 0.5f, "GhostHead_2");
                            if (Standard.FrameTimer % 15 == 0)
                                Standard.FadeAnimation(new DrawingLayer("GhostHead_2", enemy.GetBound()), 10, Color.CornflowerBlue);
                        }
                    };
                }
				else
				{
					int x = 0;
					int y = 0;

					if (Standard.Random(0, 2) == 0)
						x = Standard.Random(15, 80);
					else
						x = Standard.Random(800, 880);
					if (Standard.Random(0, 2) == 0)
						y = Standard.Random(15, 80);
					else
						y = Standard.Random(720, 800);
					if(Method2D.Distance(player.GetPos(),new Point(x,y))>80)
						enemy = new DrawingLayer("Player_V6", new Rectangle(x, y, 100, 100));
					else
						enemy = new DrawingLayer("Player_V6", new Rectangle(x+200, y+200, 100, 100));
                    SDeadAction += () => Tester.KillCard = new DrawingLayer("SDead_1", new Point(0, 0), 0.6f);
                    MoveAction += Rock_MoveAction;
                    DrawAction += Rock_DrawAction;                   
                }
			}	
			public void Draw() => DrawAction();
            public void DrawAddOn() => DrawAddonAction();

            public void Ghost_DrawAction()
            {
                enemy.Draw(Color.PaleTurquoise);             
            }
            
            public void Rock_DrawAction()
            {
                enemy.Draw(Room.RoomColor);
            }
            public void Ghost_MoveAction()
            {
                if (GhostDistance > 3)
                {
                    if (SlowMode)
                        GhostDistance = GhostDistance - 3 * TimeCoefficient;
                    else
                        GhostDistance = GhostDistance - 3;
                    enemy.SetCenter(new Point(player.GetCenter().X + (int)(GhostDistance * (Math.Cos(GhostAngle + Standard.FrameTimer * Ghostw))), player.GetCenter().Y + (int)(GhostDistance * (Math.Sin(GhostAngle + Standard.FrameTimer * Ghostw)))));
                }
                else
                {
                    enemy.SetCenter(player.GetCenter());
                }
            }
            public void Rock_MoveAction()
            {
                if (SlowMode)
                    enemy.MoveTo(player.GetPos().X + RandomInts[Math.Abs(RandomIntCounter) % 14], player.GetPos().Y + RandomInts[(RandomIntCounter + Standard.FrameTimer + 100) % 14], ZombieSpeed * TimeCoefficient);
                else
                    enemy.MoveTo(player.GetPos().X + RandomInts[Math.Abs(RandomIntCounter) % 14], player.GetPos().Y + RandomInts[(RandomIntCounter + Standard.FrameTimer + 100) % 14], ZombieSpeed);

                int Dis;

                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i] != this)
                    {
                        Dis = Method2D.Distance(GetPos(), enemies[i].GetPos());
                        if (Dis != 0)
                            enemy.MoveTo(enemies[i].GetPos().X, enemies[i].GetPos().Y, -160.0 / Dis);
                    }
                }
            }

			public void MoveUpdate()
			{
                MoveAction();
                //Death Check
				if (!IsEndPhase&&StartStageTimer==0&&(Method2D.Distance(player.GetCenter(), getCenter())) <= 10 )
				{
                    if (player.getAttackIndex() != -1 && this != enemies[player.getAttackIndex()])
                        return;
					if(!Checker.LuckCheck())
					{
                        ThisIsTheKiller = true;
                        //KillerZombieIndex = Index;
                        //Need to change dead scene sprite
                        if(SDeadAction!=null)
                            Game1.AttachDeadScene(SDeadAction);
						GameOver = true;
						Checker.Hearts--;
					}
				}
                
			}
		}

		public class Bludger
		{
			public static int BludgerSpeed = 14;
			public static Rectangle BoundRectangle;
			public DrawingLayer bludger=new DrawingLayer("Player_V6",new Rectangle(2000,0,80,80));
			public Point v=new Point(1,1);

			public Bludger(Point vector)
			{
				v = vector;
				bludger.SetPos(Standard.Random(-50, 50), Standard.Random(-50, 50));
			}
			public void MoveUpdate()
			{
				BoundRectangle = new Rectangle(-Game1.graphics.GraphicsDevice.Viewport.X, -Game1.graphics.GraphicsDevice.Viewport.Y,900, 720);
				//벡터 계산
				if(BoundRectangle.X>bludger.GetPos().X||0> bludger.GetPos().X)//공이 왼쪽으로 나갈 경우
				{
					v = new Point(Math.Abs(v.X), v.Y);
					v = Method2D.Deduct(player.GetPos(), bludger.GetPos());
				}
				if(BoundRectangle.Y>bludger.GetPos().Y||0> bludger.GetPos().Y)//공이 위로 나갈 경우
				{
					v = new Point(v.X, Math.Abs(v.Y));
					v = Method2D.Deduct(player.GetPos(), bludger.GetPos());
				}
				if(BoundRectangle.X+ BoundRectangle.Width<bludger.GetPos().X||MasterInfo.FullScreen.Width-80 < bludger.GetPos().X)//공이 오른쪽으로 나갈 경우
				{
					v = new Point(-Math.Abs(v.X), v.Y);
					v = Method2D.Deduct(player.GetPos(), bludger.GetPos());
				}
				if (BoundRectangle.Y + BoundRectangle.Height < bludger.GetPos().Y|| MasterInfo.FullScreen.Height-80 < bludger.GetPos().Y)//공이 오른쪽으로 나갈 경우
				{
					v = new Point(v.X, -Math.Abs(v.Y));
					v = Method2D.Deduct(player.GetPos(), bludger.GetPos());
				}

				if(SlowMode)
					bludger.MoveByVector(v, BludgerSpeed*TimeCoefficient);
				else
					bludger.MoveByVector(v, BludgerSpeed);

				if (!IsEndPhase && StartStageTimer == 0 && (Method2D.Distance(player.GetCenter(), bludger.GetCenter())) <= 10)
				{

					
					if(!Checker.LuckCheck())
					{
                        Game1.AttachDeadScene(() => Tester.KillCard = new DrawingLayer("SDead_33", new Point(0, 0), 0.6f));
                        //KillerZombieIndex = -1;
                        GameOver = true;
						Checker.Hearts--;
					}

					/* 현재 블러저 처리에 관한 두가지 안
					 * 1. 맞으면 사망
					 * 2. 맞으면 끌려감
					 *  2가 더 재밌고 편하긴 함.
					 * */
				}

			}
			public void Draw()
			{
				bludger.Draw(Color.IndianRed,0.5f);
				if(!ShowMenu&&!GameOver&&Standard.FrameTimer%3==0)
				{
					if(Standard.FrameTimer%30<15)
						Standard.FadeAnimation(new DrawingLayer("BludgerFire", new Rectangle(bludger.GetPos(), new Point(100, 100))), 8, Color.IndianRed);
					else
						Standard.FadeAnimation(new DrawingLayer("BludgerFire2", new Rectangle(bludger.GetPos(), new Point(100, 100))), 16, Color.IndianRed);

				}
				Standard.DrawAddon(bludger, Color.LightYellow, 1f, "BludgerFace");
			}


		}
		public static class Room
		{
			public static int Number;
			public static Color RoomColor;
			public static Color StarColor;
			public static Dictionary<int, string> RoomKeys = new Dictionary<int, string>();
			public static Dictionary<int, int> RoomDifficulties = new Dictionary<int, int>();

            /* ROOM INDEX*
			 * 000: Room of Rock				*
			 * 001: Room of Fire				**
			 * 002: Room of Ice					**
			 * 003: Room of Fire(Hard)			***
			 * 004: Room of Fire(EX)			****
			 * 005: Room of Ice(Hard)			***
			 * 006: Room of Ice(EX)				****
			 * 013: Room of Fire and Ice		***
			 * 014: Room of Fire(Easy)			*
			 * 015: Room of Ice(Easy)			*
			 * 016: Room of Fire and Ice(Easy)	**
			 * 77: Room of Fire and Ice(Hard)	*****
			 * 777: Room of Fire and Ice(EX)	*******
			 * 66: Inferno						*****
			 * 666: Inferno(EX)					*******
			 * */

            public static void Init()
			{
				RoomKeys.Add(0, "Room of Rocks");
				RoomDifficulties.Add(0, 1);
				RoomKeys.Add(1, "Room of Fire");
				RoomDifficulties.Add(1, 2);
				RoomKeys.Add(2, "Room of Ice");
				RoomDifficulties.Add(2, 2);
				RoomKeys.Add(3, "Room of Fire(Hard)");
				RoomDifficulties.Add(3, 3);
				RoomKeys.Add(4, "Room of Fire(EX)");
				RoomDifficulties.Add(4, 4);
				RoomKeys.Add(5, "Room of Ice(Hard)");
				RoomDifficulties.Add(5, 3);
				RoomKeys.Add(6, "Room of Ice(EX)");
				RoomDifficulties.Add(6, 4);
				RoomKeys.Add(13, "Room of Fire and Ice");
				RoomDifficulties.Add(13, 3);
				RoomKeys.Add(77, "Room of Fire and Ice(Hard)");
				RoomDifficulties.Add(77, 5);
				RoomKeys.Add(777, "Room of Fire and Ice(EX)");
				RoomDifficulties.Add(777, 7);
				RoomKeys.Add(66, "Inferno");
				RoomDifficulties.Add(66, 5);
				RoomKeys.Add(666, "Inferno(EX)");
				RoomDifficulties.Add(666, 7);
				RoomKeys.Add(14, "Room of Fire(Easy)");
				RoomDifficulties.Add(14, 1);
				RoomKeys.Add(15, "Room of Ice(Easy)");
				RoomDifficulties.Add(15, 1);
				RoomKeys.Add(16, "Room of Fire and Ice(Easy)");
				RoomDifficulties.Add(16, 2);

			}
			public static int ClearRewardCount()
			{
				return RoomDifficulties[Room.Number];
			}
			public static string Name()
			{
				return RoomKeys[Room.Number];
			}
			public static void SetFireRoom(int FireCount, int Interval)
			{
				RoomColor = Color.MonoGameOrange;
				StarColor = Color.Orange;
				TheIceRoom = false;
				Standard.PlayLoopedSong("GhostTown");
				for (int i = 0; i < FireCount; i++)
				{
					bludgers.Add(new Bludger(new Point(i + 1, 1)));
					bludgers[i].bludger.SetPos(i * Interval, 0);
				}
			}
			public static void SetIceRoom()
			{
				RoomColor = Color.LightSkyBlue;
				StarColor = Color.AliceBlue;
				TheIceRoom = true;
				Standard.PlayLoopedSong("Stage2_Youdie");
			}
			public static void SetFireAndIceRoom(int FireCount, int Interval)
			{
				RoomColor = Color.MediumPurple;
				StarColor = Color.Aquamarine;
				TheIceRoom = true;
				Standard.PlayLoopedSong("FireAndIce2");

				for (int i = 0; i < FireCount; i++)
				{
					bludgers.Add(new Bludger(new Point(i + 1, 1)));
					bludgers[i].bludger.SetPos(i * Interval, 0);
				}
			}
			public static void Set()
			{
				ResetGame();
				switch (Number)
				{
					case 0:
						RoomColor = Color.LightGray;
						StarColor = Color.LightSalmon;
						Standard.PlayLoopedSong("YouDieTheme8");
						TheIceRoom = false;
						break;
					case 1:
                        if (!LiteMode)
                            SetFireRoom(10, 500);
                        else
                            SetFireRoom(3, 500);
                        //SetFireRoom(20, 1500);
                        break;
					case 2:
						SetIceRoom();
						break;
					case 3:
                        if (!LiteMode)
                            SetFireRoom(20, 1500);
                        else
                            SetFireRoom(5, 1500);

                        //SetFireRoom(30, 1000);
						break;
					case 4:
						SetFireRoom(40, 30000 / 40);
						break;
					case 5:
						SetIceRoom();
						break;
					case 6:
						SetIceRoom();
						break;

					case 13:
                        if (!LiteMode)
                            SetFireAndIceRoom(8, 500);
                        else
                            SetFireAndIceRoom(2, 500);

						break;

					case 14:
                        if(!LiteMode)
    						SetFireRoom(2, 500);
                        else
                            SetFireRoom(1, 500);
                        break;
					case 15:
						SetIceRoom();
						break;
					case 16:
                        if (!LiteMode)
                            SetFireAndIceRoom(2, 500);
                        else
                            SetFireAndIceRoom(1, 500);
                        break;
					case 77:
                        if (!LiteMode)
                            SetFireAndIceRoom(16, 500);
                        else
                            SetFireAndIceRoom(4, 500);
                        break;
                        RoomColor = Color.MediumPurple;
						StarColor = Color.Aquamarine;
						TheIceRoom = true;
						Standard.PlayLoopedSong("FireAndIce2");
						for (int i = 0; i < 28; i++)
						{
							bludgers.Add(new Bludger(new Point(i + 1, 1)));
							bludgers[i].bludger.SetPos(i * 1000, 0);
						}
						break;
					case 777:
						SetFireAndIceRoom(38, 500);
						break;
					case 66:
						TheIceRoom = false;
						RoomColor = Color.MonoGameOrange;
						StarColor = Color.Orange;
						Standard.PlayLoopedSong("Inferno_Final");
						for (int i = 0; i < 25; i++)
						{
							bludgers.Add(new Bludger(new Point(i + 1, 1)));
							bludgers[i].bludger.SetPos((int)(400 + 1000 * Math.Cos(2 * Math.PI * i / 25)), (int)(400 + 1000 * Math.Sin(2 * Math.PI * i / 25)));
						}

						break;
					case 666:
						TheIceRoom = false;
						RoomColor = Color.MonoGameOrange;
						StarColor = Color.Orange;
						Standard.PlayLoopedSong("Inferno_Final");
						for (int i = 0; i < 35; i++)
						{
							bludgers.Add(new Bludger(new Point(i + 1, 1)));
							bludgers[i].bludger.SetPos((int)(400 + 1000 * Math.Cos(2 * Math.PI * i / 35)), (int)(400 + 1000 * Math.Sin(2 * Math.PI * i / 35)));
						}
						break;


				}
				

			}

			public static void Update()
			{
				switch (Number)
				{
				

					case 2:
                        if (!LiteMode)
                        {
                            if (Standard.FrameTimer % 40 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        else
                        {
                            if (Standard.FrameTimer % 60 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        break;

					case 5:
                        if (!LiteMode)
                        {
                            if (Standard.FrameTimer % 30 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        else
                        {
                            if (Standard.FrameTimer % 45 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        break;
					case 6:
						if (Standard.FrameTimer % 22 == 0)//아이스(=고스트좀비) 생성
						{
							enemies.Add(new Enemy(true));
						}
						break;
					case 13:
						if (Standard.FrameTimer % 55 == 0)//아이스(=고스트좀비) 생성
						{
							enemies.Add(new Enemy(true));
						}
						break;
					case 15:
                        if(!LiteMode)
                        {
                            if (Standard.FrameTimer % 60 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        else
                        {
                            if (Standard.FrameTimer % 90 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        break;
					case 16:
                        if (!LiteMode)
                        {
                            if (Standard.FrameTimer % 60 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        else
                        {
                            if (Standard.FrameTimer % 90 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        break;
					case 77:
                        if (!LiteMode)
                        {
                            if (Standard.FrameTimer % 45 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        else
                        {
                            if (Standard.FrameTimer % 60 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        /*
                        if (Standard.FrameTimer % 45 == 0)//아이스(=고스트좀비) 생성
						{
							enemies.Add(new Enemy(true));

						}*/
						break;
					case 777:
						if (Standard.FrameTimer % 37 == 0)//아이스(=고스트좀비) 생성
						{
							enemies.Add(new Enemy(true));
						}
						break;
					case 66:
						if (Standard.FrameTimer % 500 > 430)
						{
							for (int i = 0; i < bludgers.Count; i++)
							{
								//bludgers[i].bludger.SetPos(i*500, Standard.Random(-50, 50));
								bludgers[i].bludger.SetPos(-500, -500);

							}
						}					

						if (Standard.FrameTimer % 2000 == 0)
						{
							double Rnd = Standard.Random();
							for (int i = 0; i < bludgers.Count; i++)
							{
								bludgers[i].bludger.SetPos((int)(400 + 1000 * Math.Cos(2 * Math.PI * i / 25 + Rnd)), (int)(400 + 1000 * Math.Sin(2 * Math.PI * i / 25 + Rnd)));

							}
						}
						if (Standard.FrameTimer % 2000 == 500)
						{
							for (int i = 0; i < bludgers.Count; i++)
							{
								bludgers[i].bludger.SetPos(i * 200, 0);

							}
						}
						if (Standard.FrameTimer % 2000 == 1000)
						{
							for (int i = 0; i < bludgers.Count; i++)
							{
								bludgers[i].bludger.SetPos(i * 200, 1300);

							}
						}

						if (Standard.FrameTimer % 2000 == 1500)
						{
							for (int i = 0; i < bludgers.Count; i++)
							{
								if (i < bludgers.Count / 2)
									bludgers[i].bludger.SetPos(0, i * 300);
								else
									bludgers[i].bludger.SetPos(2300, (i - bludgers.Count / 2 - 5) * 400);
							}
						}

						break;
					case 666:
						if (Standard.FrameTimer % 500 > 430)
						{
							for (int i = 0; i < bludgers.Count; i++)
							{
								bludgers[i].bludger.SetPos(-500, -500);
							}
						}
              
						if (Standard.FrameTimer %2000==0)
						{
							double Rnd = Standard.Random();
							for (int i = 0; i < bludgers.Count; i++)
							{
								bludgers[i].bludger.SetPos((int)(400 + 1000 * Math.Cos(2 * Math.PI * i / 35 + Rnd)), (int)(400 + 1000 * Math.Sin(2 * Math.PI * i / 35 + Rnd)));
							}
						}
						if (Standard.FrameTimer %2000 == 500)
						{
							for (int i = 0; i < bludgers.Count; i++)
							{
								bludgers[i].bludger.SetPos(i*150,0);

							}
						}
						if (Standard.FrameTimer % 2000 == 1000)
						{
							for (int i = 0; i < bludgers.Count; i++)
							{
								bludgers[i].bludger.SetPos(i * 150, 1300);
							}
						}

						if (Standard.FrameTimer % 2000 == 1500)
						{
							for (int i = 0; i < bludgers.Count; i++)
							{
								if(i<bludgers.Count/2)
									bludgers[i].bludger.SetPos(0, i*300);
								else
									bludgers[i].bludger.SetPos(2300, (i-bludgers.Count/2-5)*400);

							}
						}

						break;







				}

			}
		}

	}



	public static class Checker
	{
		public static int Hearts = 10;
		public static int HeartStack = 0;
		public static int HeartTimer = 0;

		public static int Bloodthirst = 0;
		public static int Luck=0;
		public static int Swiftness = 0;
		public static int LuckTimer;

		public static Vector2 InfoVector;
		public static DrawingLayer StringBackGround;

		public static double BloodStack = 0;
		

		//본 열거자의 이름은 실제 변수명과 일치해야 한다.(리플렉션 활용)
		public enum Status { Swiftness, Bloodthirst, Luck };


		public static void Init()
		{
			Checker.Hearts = 10;
			Checker.Swiftness = 0;
			Checker.Luck = 0;
			Checker.Bloodthirst = 0;
			Checker.BloodStack = 0;
		}

		public static void GetHeart()
		{
			HeartStack++;
			HeartTimer = 30;
		}
		public static void GetHeart(int i)
		{
			HeartStack += i;
			HeartTimer = 30;
		}

		public static void Update()
		{
			if (LuckTimer > 0)
				LuckTimer--;		
			if (Tester.ScoreStack != 0)
			{
				switch (Checker.Bloodthirst)
				{
					case 1:
						BloodStack += 0.01;
						break;
					case 2:
						BloodStack += 1.0/75;
						break;
					case 3:
						BloodStack += 0.02;
						break;

				}

				if(BloodStack>=1.0)
				{
					BloodStack = 0;
					GetHeart();
					Standard.PlaySE("Bloodthirst");
					Standard.FadeAnimation(Tester.player.player, 30, Color.Red);
				}

			}

		}

		public static void ShowStatus()
		{

			//뷰포트를 컴퓨터화면에 맞게 세팅한다.
			Viewport Temp = Game1.graphics.GraphicsDevice.Viewport;
			Game1.graphics.GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);

			//Show Hearts
			DrawingLayer Heart = new DrawingLayer("Heart", new Rectangle(50, 50, 60, 60));
			Color HeartColor = Color.DarkRed;
			int Hearts_5 = Checker.Hearts / 5;
			int LeftHearts = Checker.Hearts % 5;
			for (int i = 0; i < Hearts_5; i++)
			{
				if((Standard.FrameTimer+25)%60<30)
					Heart.SetSprite("Heart5");
				else
					Heart.SetSprite("Heart5_2");

				Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
				if (Tester.FreezeTimer < 0)
				{

					Heart.Draw(HeartColor);
					Heart.Draw(Color.White * 0.7f);
				}
				else
					Heart.Draw(HeartColor);
			}
			for (int i = 0; i < LeftHearts; i++)
			{
				if((Standard.FrameTimer + 25) % 60<30)
					Heart.SetSprite("Heart");
				else
					Heart.SetSprite("Heart2");
				Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
				if (Tester.FreezeTimer < 0)
				{

					Heart.Draw(HeartColor);
					Heart.Draw(Color.White * 0.7f);
				}
				else
					Heart.Draw(HeartColor);
			}
			if (Checker.HeartStack > 0)
			{
				if (Checker.HeartTimer > 0)
					Checker.HeartTimer--;
				else
				{
					Checker.Hearts += Checker.HeartStack;
					Checker.HeartStack = 0;
				}
				for (int i = 0; i < Checker.HeartStack; i++)
				{
					if (Checker.HeartTimer != 0)
						Heart.SetSprite("HeartAni" + (30 - Checker.HeartTimer) / 6);
					else
						Heart.SetSprite("Heart");

					Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
					if (Tester.FreezeTimer < 0)
					{

						Heart.Draw(HeartColor);
						Heart.Draw(Color.White * 0.7f);
					}
					else
						Heart.Draw(HeartColor);
					Heart.Draw(Color.Honeydew * (float)(Checker.HeartTimer / 8.0));

				}
			}
			if (Tester.FreezeTimer > Tester.FreezeTime - 60)
			{
				
				Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
				Heart.SetSprite("Heart_Broken");
				Heart.Draw(HeartColor);
			}
			else if (Tester.FreezeTimer > Tester.FreezeTime - 110)
			{
				Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
				Heart.SetSprite("Heart_Broken2");
				Heart.Draw(HeartColor);
			}
			else if (Tester.FreezeTimer > 0)
			{
				Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
				Heart.SetSprite("Heart_Broken3");
				Heart.Draw(HeartColor * (float)(Tester.FreezeTimer / (Tester.FreezeTime - 110.0)));
			}




			//Show Menual
			if (Tester.Room.Number == 0 && !Tester.IsEndPhase)
			{
				DrawingLayer Menual = new DrawingLayer("Menual", new Point(800, 500), 0.75f);
				if (Standard.FrameTimer % 60 > 30)
					Menual.SetSprite("Menual2");
				if(Standard.FrameTimer%60>30)
					Menual.Draw(Color.White);
				else
					Menual.Draw(Color.Goldenrod);
			}





			//Show Buff Info
			InfoVector = new Vector2(130, 150);
			StringBackGround = new DrawingLayer("WhiteSpace", new Rectangle((int)(InfoVector.X - 15), (int)(InfoVector.Y - 10), 170, 35));
			ShowBuffString(Status.Luck);
			ShowBuffString(Status.Swiftness);
			ShowBuffString(Status.Bloodthirst);

			//Show Gauge
			DrawingLayer gauge = new DrawingLayer("WhiteSpace", new Rectangle(100,300,10, (int)(300 * Tester.Gauge)));
			gauge.Draw(Color.AliceBlue);
			gauge.Draw(Color.Red*(float)(1-Tester.Gauge));


			//뷰포트 원상복귀.
			Game1.graphics.GraphicsDevice.Viewport = Temp;
		}

		public static void ShowBuffString(Status status)
		{
			int checker=-1;
			Type type = typeof(Checker);
			FieldInfo FInfo=type.GetField(status.ToString());
			checker = (int)FInfo.GetValue(null);
			if(checker>0)
			{
				string InfoString = "- " + status.ToString()+" ";
				for (int i = 0; i < checker; i++)
				{
					InfoString = InfoString + "I";
				}
				StringBackGround.Draw(Color.Black * 0.5f);
				Standard.DrawString(InfoString, InfoVector, Color.White * (float)(Math.Min(Standard.FrameTimer % 240, 240 - Standard.FrameTimer % 240) / 120.0 + 0.3f));
				InfoVector += new Vector2(0, 35);
				StringBackGround.SetBound(new Rectangle(StringBackGround.GetBound().X, StringBackGround.GetBound().Y + 35, StringBackGround.GetBound().Width, StringBackGround.GetBound().Height));

			}

		}

		public static bool LuckCheck()
		{
			if (LuckTimer > 0)
				return true;
			switch (Luck)
			{
				case 1:
					if (Standard.Random() < 0.05)
					{
						Standard.FadeAnimation(Tester.player.player, 30, Color.Green);
						Standard.PlaySE("GetHeart");
						LuckTimer = 30;
						return true;
					}
					break;
				case 2:
					if (Standard.Random() < 0.10)
					{
						Standard.FadeAnimation(Tester.player.player, 30, Color.Green);
						Standard.PlaySE("GetHeart");
						LuckTimer = 30;

						return true;
					}

					break;
				case 3:
					if (Standard.Random() < 0.15)
					{
						Standard.FadeAnimation(Tester.player.player, 30, Color.Green);
						Standard.PlaySE("GetHeart");
						LuckTimer = 30;
						return true;
					}
					break;
			}
			return false;
		}

	}
	public class Card
	{
		public DrawingLayer Frame;
		public int FlipTimer = -1;//-1:뒷면, 0:앞면, 1~30:까는중
		public int RemoveTimer = -1;//-1:파괴X,0이상:파괴타이머

		public string FrontFrameName;
		public string BackFrameName = "RewardCard";
		public static readonly int CardWidth = 140;
		public static readonly int FlipTime = 30;
		public string InfoString;

		public enum CardClass { Reward };

		public delegate void CardOpenEvent(int EventNumber);
		public event CardOpenEvent CardOpenEvents;
		/*Card Index
		 * 0:Heart 1
		 * 1:Heart 2
		 * 2:Heart 3
		 * 3:Swiftness 1
		 * 4:Swiftness 2
		 * 5:Swiftness 3
		 * 6:Bloodthirst 1
		 * 7:Bloodthirst 2
		 * 8:Bloodthirst 3
		 * 9:Luck 1 
		 * 10:Luck 2
		 * 11:Luck 3
		 * */

		public Card(int Number, CardClass WhatClass)
		{
			//Rewards.Add(new DrawingLayer("RewardCard_" + Standard.Random(0, 12), CardPos, 0.75f));
			BackFrameName = WhatClass.ToString() + "Card";
			Frame = new DrawingLayer(BackFrameName, Tester.CardPos, 0.75f);
			Frame.SetCenter(new Point(Tester.CardPos.X+800, Tester.CardPos.Y));
			FrontFrameName = WhatClass.ToString() + "Card_" + Number;
		
			if(WhatClass==CardClass.Reward)
			{
				switch (Number)
				{
					case 0:
						InfoString = "Heart 1: \n\nGet 1 Heart.";
						break;
					case 1:
						InfoString = "Heart 2:\n\nGet 2 Hearts.";
						break;
					case 2:
						InfoString = "Heart 3:\n\nGet 3 Hearts.";
						break;
					case 3:
						InfoString = "Swiftness 1:\n\nSpeed+10%, Attack Speed+7%";
						break;
					case 4:
						InfoString = "Swiftness 2:\n\nSpeed+20%, Attack Speed+15%";
						break;
					case 5:
						InfoString = "Swiftness 3:\n\nSpeed+30%, Attack Speed+25%";
						break;
					case 6:
						InfoString = "Bloodthirst 1:\n\nGet 1 heart when you kill 100. ";
						break;
					case 7:
						InfoString = "Bloodthirst 2:\n\nGet 1 heart when you kill 75. ";
						break;
					case 8:
						InfoString = "Bloodthirst 3:\n\nGet 1 heart when you kill 50. ";
						break;
					case 9:
						InfoString = "Luck 1:\n\nChance of avoiding death: 5%";
						break;
					case 10:
						InfoString = "Luck 2:\n\nChance of avoiding death: 10%";
						break;
					case 11:
						InfoString = "Luck 3:\n\nChance of avoiding death: 15%";
						break;
					default:
						InfoString = "Preparing..";
						break;
				}
				CardOpenEvents += CardRewardEvent;
			}
		

		}
		public bool isOpened()
		{
			if (FlipTimer < FlipTime / 2&&FlipTimer!=-1)
				return true;
			else
				return false;
		}

		public int GetIndex()
		{
			return Int32.Parse(FrontFrameName.Substring(11));
		}
		public void Open()
		{
			FlipTimer = FlipTime;
		}
		public void Update()
		{
			if (FlipTimer > 0)
			{
				FlipTimer--;
				Point p = Frame.GetCenter();
				Frame.SetBound(new Rectangle(0, 0, Math.Abs((int)(CardWidth * Math.Cos(Math.PI * (FlipTime - FlipTimer) / (FlipTime)))), Frame.GetBound().Height));
				Frame.SetCenter(p);
			}
			if(RemoveTimer>0)
			{
				RemoveTimer--;
			
			}
			if(FlipTimer==1&&CardOpenEvents!=null)
			{
				CardOpenEvents(GetIndex());
			}
			
			if (Cursor.JustdidLeftClick(Frame) && FlipTimer == -1)
			{
				Open();
				Standard.PlaySE("CardHandOver");			
			}
			if (Cursor.JustdidLeftClick(Frame) && FlipTimer == 0&&RemoveTimer==-1)
			{
				RemoveTimer = 30;
			}
		}


		public void CardRewardEvent(int EventNum)
		{
			Standard.FadeAnimation(new DrawingLayer("WhiteSpace", Frame.GetBound()), 30, Color.PaleGoldenrod);
			Standard.PlaySE("GetHeart");

			switch (EventNum)
			{
				case 0:
					Checker.GetHeart();
					break;
				case 1:
					Checker.GetHeart(2);
					break;
				case 2:
					Checker.GetHeart(3);
					break;
				case 3:
					if (Checker.Swiftness < 1)
						Checker.Swiftness = 1;
					Tester.player.SetAttackSpeed(14);
					break;
				case 4:
					if (Checker.Swiftness < 2)
						Checker.Swiftness = 2;
					Tester.player.SetAttackSpeed(13);

					break;
				case 5:
					if (Checker.Swiftness < 3)
						Checker.Swiftness = 3;
					Tester.player.SetAttackSpeed(12);

					break;
				case 6:
					if (Checker.Bloodthirst < 1)
						Checker.Bloodthirst = 1;
					break;
				case 7:
					if (Checker.Bloodthirst < 2)
						Checker.Bloodthirst = 2;
					break;
				case 8:
					if (Checker.Bloodthirst < 3)
						Checker.Bloodthirst = 3;
					break;
				case 9:
					if (Checker.Luck < 1)
						Checker.Luck = 1;
					break;
				case 10:
					if (Checker.Luck < 2)
						Checker.Luck = 2;
					break;
				case 11:
					if (Checker.Luck < 3)
						Checker.Luck = 3;
					break;
			}
		}
		public void Draw()
		{
			if (RemoveTimer < 30 && RemoveTimer > 5)
			{
				Frame.SetSprite("CardCrash" + (6 - RemoveTimer / 5));
				Frame.Draw();
			}
			else if (RemoveTimer <= 5 && RemoveTimer >= 0)
			{
				Frame.SetSprite("EmptySpace");
				Frame.Draw();
			}
			else if (isOpened())
			{
				Frame.SetSprite(FrontFrameName);
				Frame.Draw();
				Standard.DrawAddon(Frame, Color.White, 1f, "CardFrame");
			}
			else
			{
				Frame.SetSprite(BackFrameName);
				Frame.Draw();

			}
			if(Cursor.IsOn(Frame)&&FlipTimer==0)
			{
				Standard.DrawString(InfoString, new Vector2(Frame.GetPos().X, Frame.GetPos().Y+200), Color.Black);
			}
		
			
		}
	}

	public static class Table
	{
		public static Dictionary<int, double> ValueTable=new Dictionary<int, double>();
		public static Dictionary<int, double> DropTable = new Dictionary<int, double>();
		public static double m=0;//parameter

		/*Card Index
		 * 0:Heart 1
		 * 1:Heart 2
		 * 2:Heart 3
		 * 3:Swiftness 1
		 * 4:Swiftness 2
		 * 5:Swiftness 3
		 * 6:Bloodthirst 1
		 * 7:Bloodthirst 2
		 * 8:Bloodthirst 3
		 * 9:Luck 1 
		 * 10:Luck 2
		 * 11:Luck 3
		 * */

		public static void Init()
		{
			ValueTable.Clear();
			DropTable.Clear();
			ValueTable.Add(0, 1);
			ValueTable.Add(1, 3);
			ValueTable.Add(2, 5);
			ValueTable.Add(3, 7);
			ValueTable.Add(4, 14);
			ValueTable.Add(5, 21);
			ValueTable.Add(6, 10);
			ValueTable.Add(7, 20);
			ValueTable.Add(8, 30);
			ValueTable.Add(9, 5);
			ValueTable.Add(10, 10);
			ValueTable.Add(11, 15);
		
			foreach(KeyValuePair<int,double> v in ValueTable)
			{
				DropTable.Add(v.Key, v.Value);
			}
			UpdateM();
		}

		public static void RemoveDrop(int key)
		{
			if(DropTable.ContainsKey(key))
			{
				DropTable.Remove(key);
			}
			UpdateM();

		}
		public static void UpdateM()
		{
			m = 0;
			foreach (KeyValuePair<int, double> v in DropTable)
			{
				m += (1.0 / v.Value);
			}
		}
		public static int Pick()
		{
			double k = Standard.Random();
			double counter = 0;
			foreach (KeyValuePair<int, double> v in DropTable)
			{
				double temp = counter;
				counter += (1.0 / v.Value)/m;
				if(k>=temp&&k<counter)
				{
					switch (v.Key)
					{
						case 3:
							RemoveDrop(3);
							break;
						case 4:
							RemoveDrop(3);
							RemoveDrop(4);
							break;
						case 5:
							RemoveDrop(3);
							RemoveDrop(4);
							RemoveDrop(5);
							break;
						case 6:
							RemoveDrop(6);
							break;
						case 7:
							RemoveDrop(6);
							RemoveDrop(7);
							break;
						case 8:
							RemoveDrop(6);
							RemoveDrop(7);
							RemoveDrop(8);
							break;
						case 9:
							RemoveDrop(9);
							break;
						case 10:
							RemoveDrop(9);
							RemoveDrop(10);
							break;
						case 11:
							RemoveDrop(9);
							RemoveDrop(10);
							RemoveDrop(11);
							break;



					}
				
					return v.Key;
				}
			}
			return -1;
		}



	}





	public class Dialog
	{
		List<string> Dialogs = new List<string>();
		DrawingLayer SCG;
		float DefaultSize = 0.75f;
		Point DefaultPoint = new Point(700, 50);
		string CurrentDialog;
		string ViewDialog;


		private void SetSCG(string s)
		{
			if(Game1.content.assetExists(s))
			{
				SCG = new DrawingLayer(s, DefaultPoint, DefaultSize);
			}
			else
			{
				SCG = new DrawingLayer("EmptySpace", DefaultPoint, DefaultSize);
			}
		}

		private void Parse()
		{
			string[] pieces = Dialogs[0].Split(':');
			if(pieces.Length==3)//A:B:C형태. 예를 들면 벌레:0:안녕? 같은 형태라면, 캐릭터는 벌레이고, CG는 "벌레_0"을 쓴다. 대사창은 "안녕?"임.
			{
				SetSCG(pieces[0] + "_" + pieces[1]);
				CurrentDialog = pieces[2];
			}
			else if(pieces.Length==2)//벌레:안녕? 형태. 디폴트 SCG를 적용한다.
			{
				SetSCG(pieces[0]);
				CurrentDialog = pieces[1];

			}
			//여기서 발생하는 에셋 저장 방식 : 디폴트는 넘버링을 하지 않는다. 디폴트 이외는 넘버링을 하고, 대사집을 쓸 때도 넘버를 써서 작성한다.
		}

		public void Update()
		{
			if(Dialogs.Count>0)
			{
				if(Cursor.JustdidLeftClick())
				{
					Parse();

				}
			}
		}

		public void Draw()
		{

		}
	}


	public static class BuffAnimation
	{
		public static DrawingLayer BuffAnimator=new DrawingLayer();

	}



	/// <summary>
	/// </summary>
	public class SafeInt
	{
		private int Value=15235;
		private int isoValue = 0;
		private int InternalTimer;

		private static int GetHashValue(int x)
		{
			return x * 15235 * (x + 123) % 2328;
		}

		public SafeInt(int x)
		{
			Set(x);
		}

		public void Set(int x)
		{
			Vibe();
			isoValue = x - Value;
		}

		public override string ToString()
		{
			Vibe();
			return Get().ToString();
		}

		public int Get()
		{
			Vibe();
			return Value+isoValue;
		}

		private void Vibe()
		{
			if (InternalTimer != Standard.FrameTimer)
			{
				Value++;
				isoValue--;
				InternalTimer = Standard.FrameTimer;
			}

		}


		public static SafeInt operator++(SafeInt i)
		{
			i.Set(i.Get() + 1);
			return i;
		}
	
	}



	/// <summary>
	///  실험적인 클래스.
	///  매 루프마다 0,1이 바뀌어가며 점멸하는 스위치입니다.
	///  게임내 상황이 변할 때(ex: 게임오버 상황) 변하는 부울값을 찾아서 스위치를 무력화시키는 방법이 있는데,
	///  이 스위치는 매 프레임마다 점멸하기 때문에 실제로 의미를 가지는 값이 변화하는 지점을 찾기가 어렵습니다.
	///  게임내 중요 스위치를 관리할 때 쓸 수 있습니다.
	///  기존 부울변수에 비해 6바이트 비싸고, 게터, 세터에서 플립이 발생하기 때문에 매 루프당 계산 손실이 있긴 합니다. 
	///  하지만 중요한 스위치들에는 충분히 적용가능합니다.
	///  메모리 스캐닝 시도 자체를 무력화할 수 있습니다.
	/// </summary>
	public class SafeBool
	{
		private bool Bool=true;
		private bool isoBool=true;
		private int InternalTimer;

		public SafeBool(bool b)
		{
			Set(b);
		}

		public void Set(bool b)
		{
			Flip();
			if (b)
			{
				isoBool = Bool;
			}
			else
			{
				isoBool = !Bool;
			}
		}

		public bool Get()
		{
			Flip();
			return (Bool == isoBool);
		}

		private void Flip()
		{
			if (InternalTimer != Standard.FrameTimer)
			{
				Bool = !Bool;
				isoBool = !isoBool;
				InternalTimer = Standard.FrameTimer;
			}
		}



	}




}
