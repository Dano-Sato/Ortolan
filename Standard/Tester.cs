﻿using Microsoft.Xna.Framework;
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
			enemies.Add(new Enemy(false));
			enemies.Add(new Enemy(false));
			Mouse.SetPosition(450, 480);
			player.reset();
			for(int i=0;i<bludgers.Count;i++)
			{
				//bludgers[i].bludger.SetPos(i*500, Standard.Random(-50, 50));
				bludgers[i].bludger.SetPos((int)(400 + 1000 * Math.Cos(2 * Math.PI * i / 25)), (int)(400 + 1000 * Math.Sin(2 * Math.PI * i / 25)));

			}
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
		public static int Score = 0;
		public static int ZombieTime = 40;
		public static double Lightr = 0;//화면이 좀 깜빡거리도록 하기 위해 넣은 변수
		public static DrawingLayer BloodLayer= new DrawingLayer("Blood", MasterInfo.FullScreen);
		public static List<DrawingLayer> DeadBodys = new List<DrawingLayer>();
		public static int BoostTimer = 0;
		
		public static int ZombieSpeed = 7;
		public static int ScoreStack = 0;
		public static double Fear = 0;
		public static Point OldPlayerPos;
		public static Point OldPlayerDisplacementVector;

		public static int FreezeTimer = -1;//게임오버시 화면을 얼린다.
	

		public static Point Wind = new Point(0, 1);
		public static Point ZombieCOM = new Point(0, 0);

		public static bool ShowMenu = false;
		public static DrawingLayer MenuLayer=new DrawingLayer("WhiteSpace", new Rectangle(100,50,1000,700));
		public static ScrollBar ScrollBar_Sensitivity = new ScrollBar(new DrawingLayer("BarFrame2",new Rectangle(200,400,500,50)), "Bar2", 50, false);
		public static ScrollBar ScrollBar_SongVolume = new ScrollBar(new DrawingLayer("BarFrame2", new Rectangle(200, 220, 500, 50)), "Bar2", 50, false);
		public static ScrollBar ScrollBar_SEVolume= new ScrollBar(new DrawingLayer("BarFrame2", new Rectangle(200, 290, 500, 50)), "Bar2", 50, false);
		public static DrawingLayer YouDieLayer = new DrawingLayer("Youdie", new Point(200, 500), 1.0f);
		public static DrawingLayer ExitButton = new DrawingLayer("Exit", new Point(850, 650), 1.0f);
		public static DrawingLayer RestartButton = new DrawingLayer("Restart", new Point(650, 650), 1.0f);

		public static bool IsEndPhase=false;
		public static int FadeTimer = 0;
		public static DrawingLayer SaveButton = new DrawingLayer("SaveButton", new Point(400, 400), 1f);
		public static DrawingLayer NoSaveButton = new DrawingLayer("NoSaveButton", new Point(400, 800), 1f);
		public static int StageNum = 1;
		public static int SavePoint =1;
		public static int StartStageTimer = 0;
		public static List<Bludger> bludgers=new List<Bludger>();
		public static int KillerZombieIndex = -1;

		public static double HeartSignal = 0;

		//이후 마음대로 인수 혹은 콘텐츠들을 여기 추가할 수 있습니다.
		public Tester()//여기에서 각종 이니셜라이즈가 가능합니다.
		{
			player = new Player();
			enemies.Add(new Enemy(false));
			Standard.PlaySong(0,true);
			ScrollBar_Sensitivity.Initialize(1.0f / (2.0f - 0.3f));
			
			for(int i=0;i<25;i++)
			{
				bludgers.Add(new Bludger(new Point(i+1, 1)));
				bludgers[i].bludger.SetPos((int)(400+1000*Math.Cos(2*Math.PI*i/25)), (int)(400+1000 * Math.Sin(2 * Math.PI*i / 25)));
			}
	

		}
		//Game1.Class 내에 Tester.Update()로 추가될 업데이트문입니다. 다양한 업데이트 처리를 시험할 수 있습니다.
		//그림으로 그려지기 이전 각종 변수들의 처리를 담당합니다.
		public void Update()
		{
			/*기타*/

			UpdateScore();

			if(StageNum==1)
			{
				//MasterInfo.ThemeColor = Color.LightSeaGreen;
				MasterInfo.ThemeColor = Color.MonoGameOrange;
			}
			else if(StageNum==2)
			{
				MasterInfo.ThemeColor = Color.MediumPurple;
			}
			if(IsEndPhase)
			{
				MasterInfo.ThemeColor = Color.AntiqueWhite;
			}
			

			if (GameOver&&FreezeTimer==-1)
			{
				FreezeTimer = 200;
				Standard.ClearFadeAnimation();
				Standard.PlaySE("ZombieSound4");
				return;
			}
			if(FreezeTimer>0)
			{
				Standard.ClearFadeAnimation();
				if (FreezeTimer==200)
					Standard.PlayFadedSE("KnifeSound", 0.3f);
				if(FreezeTimer==140)
					Standard.PlayFadedSE("KnifeSound", 0.5f);
				if (FreezeTimer == 90)
					Standard.PlayFadedSE("KnifeSound",0.5f);
				FreezeTimer--;
				Fear += 3;
				return;
			}
			if(FreezeTimer==0)
			{
				FreezeTimer = -1;
				Standard.FadeAnimation(new DrawingLayer("YouDied", new Rectangle(200, 350, 400, 200)), 90, Color.DarkRed);
				Standard.FadeAnimation(new DrawingLayer("Tip", new Rectangle(500, 350, 400, 200)), 150, Color.DarkRed);
				ResetGame();
			}
			

			if(!ShowMenu)
			{
				player.MoveUpdate();
				player.AttackUpdate();
			}

			if (DeadBodys.Count > 300)
			{
				Standard.FadeAnimation(DeadBodys[0], 30, Color.LightSeaGreen);
				DeadBodys.RemoveAt(0);
			}

			string ClickSprite;
			if (Standard.FrameTimer % 30 < 15)
				ClickSprite = "Click";
			else
				ClickSprite = "Click2";
			DrawingLayer Click = new DrawingLayer(ClickSprite, new Rectangle(Cursor.getPos().X - 15, Cursor.getPos().Y - 15, 30, 30));
			if(!IsEndPhase)
			Standard.FadeAnimation(Click, 10, Color.AliceBlue);
			else
				Standard.FadeAnimation(Click, 10, Color.Black);




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
				if(Standard.JustPressed(Keys.Escape))
				{
					ShowMenu = !ShowMenu;
				}

			}



			/*ESC 메뉴 처리*/

			if(ShowMenu)
			{
				
				ScrollBar_Sensitivity.Update();
				ScrollBar_SongVolume.Update();
				ScrollBar_SEVolume.Update();
				Cursor.Sensitivity = ScrollBar_Sensitivity.MapCoefficient(0.5f, 2.0f);
				if(!IsEndPhase)
					Standard.SetSongVolume(ScrollBar_SongVolume.Coefficient);
				Standard.SetSEVolume(ScrollBar_SEVolume.Coefficient);
				if (ExitButton.MouseJustLeftClickedThis())
					Game1.GameExit = true;
				if(RestartButton.MouseJustLeftClickedThis())
				{
					StageNum = 1;
					SavePoint = 1;
					if (Standard.SongIndex != StageNum - 1)
						Standard.PlaySong(StageNum - 1, true);
					StartStageTimer = 99;
					ShowMenu = false;
					ResetGame();
				}
				return;
			}

			/*좀비들의 이동 처리.*/



			HeartSignal = 0;
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
				ZombieCOM = Method2D.Add(ZombieCOM, enemies[i].getCenter());
				double r = Method2D.Distance(enemies[i].enemy.GetPos(), player.getPos());
				HeartSignal += (1600.0 / (r * r));
			}
			if(enemies.Count>0)
			ZombieCOM = new Point(ZombieCOM.X / enemies.Count, ZombieCOM.Y / enemies.Count);
			for (int i = 0; i < enemies.Count; i++)
			{
				if (i == player.getAttackIndex())
					continue;
					if (j >= 14)
						j = 0;
					enemies[i].MoveUpdate(i, RandomInts[j], RandomInts[(j + Standard.FrameTimer) % 14]);
					j++;
			}


			/*좀비 생성 작업*/

			ZombieTime = Math.Max(10 - Score / 10,5)+15;//좀비 생성 시간은 스코어가 높을수록 빨라진다.
		
			if (Standard.FrameTimer % ZombieTime == 0)
			{
				if(!(Score<10&&enemies.Count>10))
					enemies.Add(new Enemy(false));
				if (enemies.Count > 150&&!player.IsAttacking())
					enemies.RemoveAt(0);
			}


		
			/*뷰포트 처리*/

			Point PlayerDisPlacementVector = Method2D.Deduct(player.getPos(), OldPlayerPos);
			Point ViewportDisplacement = Method2D.Deduct(PlayerDisPlacementVector, OldPlayerDisplacementVector);
			Point CursorDisplacement = Method2D.Deduct(player.getPos(), Cursor.getPos());
			int Dis = Method2D.Abs(CursorDisplacement);

			if(Dis>250)
			{
				CursorDisplacement = new Point(CursorDisplacement.X * 250 / Dis, CursorDisplacement.Y * 250 / Dis);
			}
				
			OldPlayerPos = player.getPos();
			OldPlayerDisplacementVector = PlayerDisPlacementVector;

			Game1.graphics.GraphicsDevice.Viewport = new Viewport( -player.getPos().X +CursorDisplacement.X/2+ ViewportDisplacement.X / 2 + 400, -player.getPos().Y + CursorDisplacement.Y / 2 + ViewportDisplacement.Y / 2 + 400, 1300, MasterInfo.FullScreen.Height);




			
			if(!IsEndPhase&&Standard.FrameTimer%60==0&&StageNum>1)//아이스(=고스트좀비) 생성
			{
				enemies.Add(new Enemy(true));
			}
	


			/*엔드페이즈 처리*/
			if (Score == 100)
			{
				IsEndPhase = true;
				Standard.DisposeSE();
				Standard.PlayLoopedSE("WindOfTheDawn");
				FadeTimer = 100;
				ScoreStack = 0;
				/*
				if(ScoreStack==0||ScoreStack>2)
					ScoreStack=1;*/
			}
			if (IsEndPhase&&StartStageTimer==0)
			{
				if (FadeTimer > 0)
					FadeTimer--;
				Standard.FadeSong((float)(FadeTimer / 100.0));
				if (enemies.Count > 0)
					RemoveEnemy(0,Color.LightGoldenrodYellow);
				if(SaveButton.MouseJustLeftClickedThis())
				{
					SavePoint = StageNum+1;
					StartStageTimer = 200;
					Standard.PlaySong(StageNum, true);
					Standard.StopSong();
				}
				if(NoSaveButton.MouseJustLeftClickedThis())
				{			
					StartStageTimer = 200;
					Standard.PlaySong(StageNum, true);
					Standard.StopSong();

				}
				Fear = 0;
				Score = 0;
			}
			if(StartStageTimer>0)
			{
				StartStageTimer--;
				if (enemies.Count > 0)
					RemoveEnemy(0, Color.LightGoldenrodYellow);				
				if(StartStageTimer==100)
				{
					StageNum++;
					ResetGame();
					Standard.DisposeSE();
					Standard.PlaySong();
					IsEndPhase = false;
					
				}
				if(StartStageTimer>100)
				{
					Fear += 10;
				}
				else
				{
					Fear -= 10;
				}
			
				Standard.FadeSong(Math.Max(0,(float)(1 - StartStageTimer / 100.0)));
			}

			for(int i=0;i<bludgers.Count;i++)
			{
				bludgers[i].MoveUpdate();
				double r = Method2D.Distance(bludgers[i].bludger.GetPos(), player.getPos());
				HeartSignal += (1600.0 / (r * r));
			}

			if(Standard.FrameTimer%500==0)
			{
				for (int i = 0; i < bludgers.Count; i++)
				{
					//bludgers[i].bludger.SetPos(i*500, Standard.Random(-50, 50));
					bludgers[i].bludger.SetPos((int)(400 + 1000 * Math.Cos(2 * Math.PI * i / 25)), (int)(400 + 1000 * Math.Sin(2 * Math.PI * i / 25)));

				}

			}


		}

		//Game1.Class 내에 Tester.Draw()로 추가될 드로우 액션문입니다. 다양한 드로잉을 시험할 수 있습니다.
		public void Draw()
		{


			//BloodLayer.Draw(Color.Aquamarine,Math.Min(10,Score)*0.1f);
			BloodLayer.Draw(Color.Orange, Math.Min(10, Score) * 0.1f);
			for (int i=0;i<DeadBodys.Count;i++)
			{

				if(!IsEndPhase)
				{
					DeadBodys[i].MoveByVector(Wind, ZombieSpeed);
					if (DeadBodys[i].GetPos().Y > MasterInfo.FullScreen.Height)
						DeadBodys[i].SetPos(DeadBodys[i].GetPos().X, 0);
					//DeadBodys[i].Draw(Color.Aquamarine, Math.Min(10, Score) * 0.1f);
					DeadBodys[i].Draw(Color.Orange, Math.Min(10, Score) * 0.1f);
				}
				else
				{
					DeadBodys[i].Draw(Color.LightGoldenrodYellow, Math.Min(10, Score) * 0.1f);
					DeadBodys[i].MoveTo(player.getPos().X, player.getPos().Y, 10);
					if(Method2D.Distance(DeadBodys[i].GetPos(),player.getPos())<10)
					{
						DeadBodys.RemoveAt(i);
						i--;
					}
				}


			
			}

			if(!IsEndPhase)
			{
				if(YouDieLayer.GetSpriteName() != "Youdie")
					YouDieLayer = new DrawingLayer("Youdie", new Point(200, 500), 1.0f);
				YouDieLayer.Draw(Color.Aquamarine);
			}
			else
			{
				if(YouDieLayer.GetSpriteName()!="Stage1")
					YouDieLayer = new DrawingLayer("Stage1", new Point(200, 500), 1.5f);
				YouDieLayer.Draw(Color.LightGoldenrodYellow,(float)(2*FadeTimer/100.0));
				YouDieLayer.SetPos(player.getPos().X - 300, player.getPos().Y - 100);
			}
			Standard.FadeAnimationDraw(Color.LightSeaGreen);//별이 사라지는 페이드애니메이션(컬러는 LighteaGreen으로 지정)은 아래 라이트레이어 전에 발생해야 보기 좋으므로 별도로 처리함.

			/*풀스크린 라이트 레이어 처리*/

			if(!IsEndPhase)
			{
				Standard.DrawLight(MasterInfo.FullScreen, Color.White, 1f, Standard.LightMode.Vignette);
				//스코어 올라갈수록 보라색을 띈다.
				Standard.DrawLight(MasterInfo.FullScreen, Color.Purple, 0.3f * Math.Min(1.2f, (float)(Score / 100.0)), Standard.LightMode.Absolute);

			}

			player.Draw();
			player.DrawAttack();

			

			bool GhostAnimate = true;
			if (Standard.FrameTimer % 50 < 25)
				GhostAnimate = false;
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Draw();
				Standard.DrawAddon(enemies[i].enemy, Color.White, 1f, "NormalZombie");
			
				if (enemies[i].IsGhost)
				{
					if(GhostAnimate)
						Standard.DrawAddon(enemies[i].enemy, Color.RoyalBlue, 1f, "GhostHead_1");
					else
						Standard.DrawAddon(enemies[i].enemy, Color.RoyalBlue, 0.6f, "GhostHead_2");

				}
			}

			for(int i=0;i<bludgers.Count;i++)
			{
				Tester.bludgers[i].Draw();
			}

			if (!IsEndPhase&&Standard.FrameTimer%10==0)
			{
				Lightr = Standard.Random() / 10.0;
			}


			/*플레이어 사망시 물어뜯는 연출*/
			if (GameOver)
			{
				if(FreezeTimer<50)
				{
					for (int i = 0; i < enemies.Count; i++)
					{
						enemies[i].enemy.MoveTo(player.getPos().X + Standard.Random(-30, 30), player.getPos().Y + Standard.Random(-30, 30), Standard.Random(5, 15));
					}
				}

			}

			if (!IsEndPhase)
			{
				Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 0.2f + (float)Lightr, Standard.LightMode.Absolute);
				Standard.DrawLight(MasterInfo.FullScreen, Color.DarkBlue, 0.3f, Standard.LightMode.Absolute);
			}


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
						enemies[i].enemy.setSprite("Player_V6");
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



			}
			
			/*시야 처리*/
			if (!ShowMenu)
			{
				for (int i = 0; i < enemies.Count; i++)
				{
					int r = Method2D.Distance(enemies[i].getCenter(), player.GetCenter());
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


		
			if (ShowMenu)
			{
				Game1.graphics.GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
				MenuLayer.Draw(Color.Black * 0.7f);
				Standard.DrawString("Mouse Sensitivity", new Vector2(ScrollBar_Sensitivity.Frame.GetPos().X, ScrollBar_Sensitivity.Frame.GetPos().Y - 20), Color.White);
				ScrollBar_Sensitivity.Draw();
				Standard.DrawString(String.Format("{0:0.0}", (ScrollBar_Sensitivity.MapCoefficient(0.3f, 2.0f))), new Vector2(ScrollBar_Sensitivity.Frame.GetPos().X + 500, ScrollBar_Sensitivity.Frame.GetPos().Y), Color.White);
				Standard.DrawString("(Default:1.0)", new Vector2(ScrollBar_Sensitivity.Frame.GetPos().X + 500, ScrollBar_Sensitivity.Frame.GetPos().Y+20), Color.White);
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
				Standard.FrameTimer--;
			}
			if(!GameOver&&!ShowMenu)
			{
				for (int i = 0; i < enemies.Count; i++)
				{
					Standard.DrawAddon(enemies[i].enemy, Color.Cornsilk, 0.3f, "NormalZombie");
					if (enemies[i].IsGhost)
					{
						if (GhostAnimate)
						{
							Standard.DrawAddon(enemies[i].enemy, Color.LightSkyBlue, 0.5f, "GhostHead_1");
							if(Standard.FrameTimer%15==0)
							Standard.FadeAnimation(new DrawingLayer("GhostHead_1", enemies[i].enemy.GetBound()), 10, Color.CornflowerBlue);
						}
						else
						{
							Standard.DrawAddon(enemies[i].enemy, Color.LightSkyBlue, 0.5f, "GhostHead_2");
							if (Standard.FrameTimer % 15 == 0)
								Standard.FadeAnimation(new DrawingLayer("GhostHead_2", enemies[i].enemy.GetBound()), 10, Color.CornflowerBlue);
						}

					}
				}
			}
			if(GameOver)
			{
				Standard.DrawAddon(player.player, Color.White, 1f, "Player");

				if(KillerZombieIndex!=-1)
				{
					Standard.DrawAddon(enemies[KillerZombieIndex].enemy, Color.Black, 1f, "Player");
					if (Standard.FrameTimer % 20 <= 10)
						Standard.DrawAddon(enemies[KillerZombieIndex].enemy, Color.White, 1f, "ZombieBite");
					else
						Standard.DrawAddon(enemies[KillerZombieIndex].enemy, Color.White, 1f, "ZombieBite2");

				}

			}

			if (IsEndPhase)
			{
				SaveButton.Draw(Color.White,(float)((100-FadeTimer)/100.0));
				if(SaveButton.MouseIsOnThis())
				{
					Standard.DrawLight(new Rectangle(-Game1.graphics.GraphicsDevice.Viewport.X + 200, -Game1.graphics.GraphicsDevice.Viewport.Y + 200,500,50), Color.Black, 0.7f, Standard.LightMode.Absolute);
					Standard.DrawString("When you die, you will start after this point", new Vector2(-Game1.graphics.GraphicsDevice.Viewport.X+220, -Game1.graphics.GraphicsDevice.Viewport.Y + 210), Color.White);
					SaveButton.Draw(Color.Red);
				}
				NoSaveButton.Draw(Color.White, (float)((100 - FadeTimer) / 100.0));
				if(NoSaveButton.MouseIsOnThis())
				{
					Standard.DrawLight(new Rectangle(-Game1.graphics.GraphicsDevice.Viewport.X + 200, -Game1.graphics.GraphicsDevice.Viewport.Y + 200, 550, 50), Color.Black, 0.7f, Standard.LightMode.Absolute);
					Standard.DrawString("When you die, you will start after the previous save point", new Vector2(-Game1.graphics.GraphicsDevice.Viewport.X + 220, -Game1.graphics.GraphicsDevice.Viewport.Y + 210), Color.White);
					NoSaveButton.Draw(Color.Red);
				}

			}
			if (StartStageTimer > 80 && StartStageTimer < 120)
			{
				Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 1f, Standard.LightMode.Absolute);
			}

		
		}


		public class Player
		{
			public DrawingLayer player;
			private Point MovePoint=new Point(0,0);
			private int Range=150;
			private int MoveSpeed=5;
			private int AttackSpeed = 15;
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
				Cursor.SetPos(450, 480);
				setPos(450, 480);
				Game1.graphics.GraphicsDevice.Viewport = new Viewport(-getPos().X  + 400, -getPos().Y +  400, 1300, 1300);
				MovePoint = new Point(0, 0);
				AttackTimer = 0;
				AttackIndex = -1;
				isAttacking = false;
				/*
				for (int i = 0; i < DeadBodys.Count; i++)
				{
					Standard.FadeAnimation(DeadBodys[i], 30,Color.LightSeaGreen);
				}*/
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

			public Point getPos()
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
		}

			public void Draw()
			{
				player.Draw(Color.White);
				/*
				for (int i = 0; i < 7; i++)
				{
					wand.SetPos(Method2D.DivPoint(player.GetBound().Center, direction.GetBound().Center, i / 10.0));
					wand.SetPos(wand.GetPos().X - 2, wand.GetPos().Y - 2);
					wand.Draw(Color.Aqua);
				}
				direction.Draw(Color.White);*/
				/*
				if (isAttacking)
					player.Draw(MasterInfo.PlayerColor*(float)((float)AttackTimer/AttackSpeed));
				*/
			}

			public void MoveUpdate()
			{
		
				string ClickSprite;

				if (isAttacking)
				{
					if (AttackTimer < AttackSpeed - 3)
					{						
						player.MoveTo(Cursor.getPos().X - 40, Cursor.getPos().Y - 40, MoveSpeed * 2);
						Standard.FadeAnimation(new DrawingLayer("Player_AfterImage", new Rectangle(player.GetPos(), new Point(70, 70))), 8, Color.AliceBlue);	
					}
					MovePoint = Method2D.DivPoint(player.GetCenter(), Cursor.getPos(), 0.8);

					
					for(int i=0;i<enemies.Count;i++)
					{
						if (enemies[i].getBound().Contains(Cursor.getPos()) && Method2D.Distance(GetCenter(), enemies[i].getCenter()) < Range)
						{
							return;
						}
					}
					MovePoint = Cursor.getPos();
				
					return;
				}
				if(!IsEndPhase&&StartStageTimer==0)
				{
					for (int i = 0; i < enemies.Count; i++)
					{
						if (enemies[i].getBound().Contains(Cursor.getPos()) && Method2D.Distance(GetCenter(), enemies[i].getCenter()) < Range)
						{
							int ClickDistance = Method2D.Distance(Cursor.getPos(), enemies[i].getCenter());
							AttackIndex = i;
							for (int j = i; j < enemies.Count; j++)
							{
								if (Method2D.Distance(Cursor.getPos(), enemies[j].getCenter()) < ClickDistance)
								{
									AttackIndex = j;
									if (enemies[j].IsGhost)
										break;
									ClickDistance = Method2D.Distance(Cursor.getPos(), enemies[j].getCenter());
								}
							}
							isAttacking = true;
							AttackTimer = AttackSpeed;
							return;
						}

					}
				}
			
			
					MovePoint = Cursor.getPos();
				
			
				
				if (MovePoint.X!=0||MovePoint.Y!=0)
				{
					if (BoostTimer > 0)
						BoostTimer--;
					player.MoveTo(MovePoint.X - 40, MovePoint.Y - 40, Math.Max(0,MoveSpeed-BoostTimer));
					int x2 = (MovePoint.X + 4 * getPos().X) / 5;
					int y2 = (MovePoint.Y + 4 * getPos().Y) / 5;
				}
			}

			public void AttackUpdate()
			{
				if(isAttacking)
				{
					if(AttackTimer==AttackSpeed&&AttackIndex!=-1&&enemies.Count>AttackIndex)
					{
						enemies[AttackIndex].enemy.setSprite("Player_Broken2");
						Standard.FadeAnimation(enemies[AttackIndex].enemy, 15, Color.AntiqueWhite);
						ShotPoint = enemies[AttackIndex].getPos();
						//ZombieFlip = !ZombieFlip;
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
						BoostTimer = 0;
						return;
					}
				}
			}

			public void DrawAttack()
			{
				if(isAttacking&&AttackTimer>=1&&enemies.Count>AttackIndex&&AttackIndex!=-1)
				{
					int x = ((AttackSpeed-AttackTimer) * enemies[AttackIndex].getPos().X + AttackTimer * getPos().X)/AttackSpeed;
					int y = ((AttackSpeed - AttackTimer) * enemies[AttackIndex].getPos().Y + AttackTimer * getPos().Y)/AttackSpeed;

					
					int KillActionTimer = AttackTimer * 2;
			
					if(!ShowMenu&& Standard.FrameTimer%5==0)
					{
						if (Standard.FrameTimer % 20 < 6)
							Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Cursor.getPos().X/2+enemies[AttackIndex].enemy.GetPos().X/2, Cursor.getPos().Y / 2 + enemies[AttackIndex].enemy.GetPos().Y / 2, 70, 70)), 15, Color.Pink);
						else if (Standard.FrameTimer % 20 < 12)
							Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Cursor.getPos().X / 2 + enemies[AttackIndex].enemy.GetPos().X / 2, Cursor.getPos().Y / 2 + enemies[AttackIndex].enemy.GetPos().Y / 2, 70, 70)), 15, Color.PaleVioletRed);
						else
							Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Cursor.getPos().X / 2 + enemies[AttackIndex].enemy.GetPos().X / 2, Cursor.getPos().Y / 2 + enemies[AttackIndex].enemy.GetPos().Y / 2, 70, 70)), 15, Color.SkyBlue);
					}


		
				}
			}
		}

		public class Enemy
		{
			public DrawingLayer enemy;
			public bool IsGhost = false;
			private double GhostAngle;
			private double Ghostw = 0.02;
			private double GhostDistance = 800;
			private int GhostTimer = 0;

			public Point getPos()
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
				if(IsGhost)
				{
					enemy = new DrawingLayer("Player_V6", new Rectangle(0,0, 100, 100));
					GhostAngle = Standard.Random() * 3;
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
					if(Method2D.Distance(player.getPos(),new Point(x,y))>80)
						enemy = new DrawingLayer("Player_V6", new Rectangle(x, y, 100, 100));
					else
						enemy = new DrawingLayer("Player_V6", new Rectangle(x+200, y+200, 100, 100));
				}
			}

	

			public void Draw()
			{
				if (enemy.GetBound().Contains(Cursor.getPos()))
				{
					if (Method2D.Distance(player.getPos(), getPos()) > player.getRange())
						enemy.Draw(Color.Blue);
					else
					{
						enemy.Draw(Color.Crimson);
					}
					return;
				}
				if(!IsGhost)
				enemy.Draw(MasterInfo.ThemeColor);
				else
					enemy.Draw(Color.PaleTurquoise);
			}

			public void MoveUpdate(int Index, int r_1, int r_2)
			{
				if(!IsGhost)
				{
					enemy.MoveTo(player.getPos().X + r_1, player.getPos().Y + r_2, ZombieSpeed);

					enemy.MoveTo(ZombieCOM.X, ZombieCOM.Y, -ZombieSpeed / 3);

					if (Method2D.Distance(getCenter(), Cursor.getPos()) < 80 && Method2D.Distance(player.GetCenter(), Cursor.getPos()) > 10)
					{
						enemy.MoveTo(Cursor.getPos().X, Cursor.getPos().Y, -3);
					}
				}
				else
				{
					GhostTimer++;
					if (GhostDistance > 0)
					{
						GhostDistance = GhostDistance - 2;
						enemy.SetPos(player.getPos().X + (int)(GhostDistance * (Math.Cos(GhostAngle + GhostTimer * Ghostw))), player.getPos().Y + (int)(GhostDistance * (Math.Sin(GhostAngle + GhostTimer * Ghostw))));
					}
					else
					{
						enemy.SetPos(player.getPos());
					}
				}


				if (!IsEndPhase&&StartStageTimer==0&&(Method2D.Distance(player.GetCenter(), getCenter())) <= 10 && Index != player.getAttackIndex())
				{
					KillerZombieIndex = Index;
					GameOver = true;
					StageNum = SavePoint;
					if (Standard.SongIndex != StageNum - 1)
						Standard.PlaySong(StageNum - 1, true);
				}
			}
		}

		public class Bludger
		{
			public static int BludgerSpeed = 12;
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
					v = Method2D.Deduct(player.getPos(), bludger.GetPos());
				}
				if(BoundRectangle.Y>bludger.GetPos().Y||0> bludger.GetPos().Y)//공이 위로 나갈 경우
				{
					v = new Point(v.X, Math.Abs(v.Y));
					v = Method2D.Deduct(player.getPos(), bludger.GetPos());
				}
				if(BoundRectangle.X+ BoundRectangle.Width<bludger.GetPos().X||MasterInfo.FullScreen.Width-80 < bludger.GetPos().X)//공이 오른쪽으로 나갈 경우
				{
					v = new Point(-Math.Abs(v.X), v.Y);
					v = Method2D.Deduct(player.getPos(), bludger.GetPos());
				}
				if (BoundRectangle.Y + BoundRectangle.Height < bludger.GetPos().Y|| MasterInfo.FullScreen.Height-80 < bludger.GetPos().Y)//공이 오른쪽으로 나갈 경우
				{
					v = new Point(v.X, -Math.Abs(v.Y));
					v = Method2D.Deduct(player.getPos(), bludger.GetPos());
				}

				
			
				
				/*
				if(bludger.MouseIsOnThis()&&Method2D.Distance(player.getPos(),bludger.GetPos())<player.getRange())
				{
					Vector2 V1 = new Vector2(v.X, v.Y);
					V1.Normalize();
					Vector2 V2 = new Vector2(bludger.GetPos().X - player.getPos().X, bludger.GetPos().Y - player.getPos().Y);
					V2.Normalize();
					Vector2 result = (V1 + V2) * 100;
					v = new Point((int)result.X, (int)result.Y);
				}*/

				bludger.MoveByVector(v, BludgerSpeed);
				if (!IsEndPhase && StartStageTimer == 0 && (Method2D.Distance(player.GetCenter(), bludger.GetCenter())) <= 10)
				{


					KillerZombieIndex = -1;
					GameOver = true;
					StageNum = SavePoint;
					if (Standard.SongIndex != StageNum - 1)
						Standard.PlaySong(StageNum - 1, true);
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
					//Standard.FadeAnimation(new DrawingLayer("Player_AfterImage", new Rectangle(bludger.GetPos(), new Point(80, 80))), 15, Color.IndianRed);
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
			public static int RoomNumber;

			/* ROOM INDEX*
			 * 000: Room of Rock
			 * 001: Room of Fire
			 * 002: Room of Ice
			 * 666: Inferno(EX)
			 * 
			 * */

			public static void Set()
			{

			}

			public static void Update()
			{

			}
		}

	}

}
