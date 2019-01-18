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
	//필요에 따라 여러 개의 테스터 클래스를 만들 수 있습니다. 아마 다음 클래스는 Tester2가 되겠죠.
	public class Tester
	{
		
		public static int Distance(Point a, Point b)
		{
			double x =a.X - b.X;
			double y = a.Y - b.Y;
			return (int)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}

		//아마 당신은 기본 폰트를 필요로 할 것입니다.
		public static SpriteFont font = Game1.content.Load<SpriteFont>("TestFont");
		public static Random random=new Random();
		public static Player player;
		public static List<Enemy> enemies=new List<Enemy>();
		public static bool GameOver=false;
		public static bool ZombieDance = false;
		public static int Timer = 0;
		public static int Score = 0;
		public static int ZombieTime = 40;
		public static DrawingLayer LightLayer = new DrawingLayer("Light", new Rectangle(0,0,1200,1200));
		public static DrawingLayer LightLayer2 = new DrawingLayer("WhiteSpace", new Rectangle(0, 0, 1200, 1200));
		public static DrawingLayer LightLayer3 = new DrawingLayer("Player", new Rectangle(0, 0, 150, 150));
		public static DrawingLayer MouseLogo = new DrawingLayer("Mouse", new Rectangle(270, 400, 30, 30));
		public static DrawingLayer MouseButton = new DrawingLayer("Mouse3", new Rectangle(270, 400, 30, 30));

		public static KeyboardState keyboardState;
		public static SoundEffect soundEffect = Game1.content.Load<SoundEffect>("EnemyDead");
		public static Song song = Game1.content.Load<Song>("12Var");
		public static List<DrawingLayer> AnimationList=new List<DrawingLayer>();
		public static List<int> AnimationTimerList = new List<int>();
		//이후 마음대로 인수 혹은 콘텐츠들을 여기 추가할 수 있습니다.
		public Tester()//여기에서 각종 이니셜라이즈가 가능합니다.
		{
			player = new Player();
			enemies.Add(new Enemy());
			MediaPlayer.Play(song);
			MediaPlayer.IsRepeating = true;

		}
		//Game1.Class 내에 Tester.Update()로 추가될 업데이트문입니다. 다양한 업데이트 처리를 시험할 수 있습니다.
		public void Update()
		{
			Timer++;
			if(GameOver)
			{
				keyboardState = Keyboard.GetState();
				if(keyboardState.IsKeyDown(Keys.R))
				{
					Score = 0;
					Timer = 0;
					ZombieTime = 40;
					enemies.Clear();
					enemies.Add(new Enemy());
					enemies.Add(new Enemy());

					player.reset();
					GameOver = false;
				}
				return;
			}
			foreach(Enemy e in enemies)
			{
				if(e.getBound().Contains(Game1.cursor.getPos())&&Distance(e.getPos(),player.getPos())<player.getRange())
				{
					Game1.cursor.SetSprite("Sword");
					break;
				}
				Game1.cursor.SetSprite("Cursor");
			}


			player.MoveUpdate();
			LightLayer3.setPosition(player.getPos().X-40+random.Next(-3,3), player.getPos().Y-40 + random.Next(-3,3));
			player.AttackUpdate();
			foreach(Enemy e in enemies)
			{
				e.MoveUpdate();
			}

			for (int i = 0; i < AnimationTimerList.Count; i++)
			{
				if (AnimationTimerList[i] == 0)
				{
					AnimationTimerList.RemoveAt(i);
					AnimationList.RemoveAt(i);
				}
				else
				{
					AnimationTimerList[i]--;
				}
			}

			ZombieTime = 40 - Score/10;
			if(ZombieTime>0)
			{
				if (Timer % ZombieTime == 0)
				{
					enemies.Add(new Enemy());
				}
			}
			else
			{
				ZombieDance = true; 
			}

		}

		//Game1.Class 내에 Tester.Draw()로 추가될 드로우 액션문입니다. 다양한 드로잉을 시험할 수 있습니다.
		public void Draw()
		{


			MouseLogo.Draw();
			if(Timer%10<=5&&Score<10)
				MouseButton.Draw(Color.Red);
			Game1.spriteBatch.Begin();
			Game1.spriteBatch.DrawString(font, "SCORE : " + Score, new Vector2(300, 450), Color.White);
			if(Score<300)
				Game1.spriteBatch.DrawString(font, "Right click to live", new Vector2(300, 400), Color.White);
			else if(Score<400)
				Game1.spriteBatch.DrawString(font, "Live to right click", new Vector2(300, 400), Color.White);
			else
				Game1.spriteBatch.DrawString(font, "Congratulation!", new Vector2(300, 400), Color.White);
			Game1.spriteBatch.DrawString(font, "Press \"ESC\" Button to leave", new Vector2(300, 500), Color.White);
			Game1.spriteBatch.End();

			player.Draw();
			player.DrawAttack();

			LightLayer.Draw();
			LightLayer2.Draw(Color.Purple * 0.3f * Math.Min(1.2f,(float)(Score / 100.0)));
			if (Score > 200)
				LightLayer3.Draw(Color.AntiqueWhite * 0.15f * Math.Min(5f, (float)((Score - 150.0)/50)));

			foreach (Enemy e in enemies)
			{
				e.Draw();
			}
			for(int i=0;i<AnimationList.Count;i++)
			{
				AnimationList[i].Draw(Color.Cornsilk * (float)(AnimationTimerList[i] / 5.0));
			}
			if(GameOver)
			{
				Game1.spriteBatch.Begin();
				Game1.spriteBatch.DrawString(font, "Game Over", new Vector2(400, 400), Color.Red);
				Game1.spriteBatch.DrawString(font, "Press \"R\" button to restart", new Vector2(400, 450), Color.Red);
				Game1.spriteBatch.End(); 
			}
		}

		public class Player
		{
			private DrawingLayer player;
			private DrawingLayer bullet;
			private Point MovePoint=new Point(0,0);
			private int Range=160;
			private int MoveSpeed=4;
			private int AttackSpeed = 9;
			private int AttackTimer = 0;
			private int AttackIndex = -1;
			private bool PressedA=false;
			private bool isAttacking=false;


			public void reset()
			{
				player.MoveTo(400, 400);
				MovePoint = new Point(0, 0);
				AttackTimer = 0;
				AttackIndex = -1;
				isAttacking = false;
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
				bullet = new DrawingLayer("Player", new Rectangle(0, 0, 30, 30));
			}

			public void Draw()
			{
				player.Draw();
				if(isAttacking)
				player.Draw(MasterInfo.PlayerColor*(float)((float)AttackTimer/AttackSpeed));
			}

			public void MoveUpdate()
			{
				if (isAttacking)
				{
					if (Game1.cursor.didPlayerJustRightClick() || Game1.cursor.didPlayerJustLeftClick())
					{
						foreach (Enemy e in enemies)
						{

							if (e.getBound().Contains(Game1.cursor.getPos()) && Distance(getPos(), e.getPos()) < Range)
							{
								return;
							}
						}
						MovePoint = Game1.cursor.getPos();
						AnimationList.Add(new DrawingLayer("Click", new Rectangle(MovePoint.X - 15, MovePoint.Y - 15, 30, 30)));
						AnimationTimerList.Add(10);
					}
					return;
				}
				if (Game1.cursor.didPlayerJustRightClick()||Game1.cursor.didPlayerJustLeftClick())
				{
					int i = 0;
					foreach(Enemy e in enemies)
					{
						
						if(e.getBound().Contains(Game1.cursor.getPos())&&Distance(getPos(),e.getPos())<Range)
						{
							isAttacking = true;
							AttackIndex = i;
							AttackTimer = AttackSpeed;
							return;
						}
						i++;
					}
					MovePoint = Game1.cursor.getPos();
					AnimationList.Add(new DrawingLayer("Click", new Rectangle(MovePoint.X-15, MovePoint.Y-15, 30, 30)));
					AnimationTimerList.Add(10);
				}
				if (MovePoint.X!=0||MovePoint.Y!=0)
					player.MoveTo(MovePoint.X-40,MovePoint.Y-40, MoveSpeed);
			}

			public void AttackUpdate()
			{
				if(isAttacking)
				{
					MovePoint = new Point(0, 0);
					if(AttackTimer==AttackSpeed)
						soundEffect.Play();
					if (AttackTimer>0)//투사체 날아가는중
					{
						AttackTimer--;
						return;
					}
					else//투사체 적중
					{
						enemies.RemoveAt(AttackIndex);
						Score++;
						isAttacking = false;
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
					bullet.setPosition(x + 20, y + 20);
					bullet.Draw(MasterInfo.PlayerColor);
			
				}
			}
		}

		public class Enemy
		{
			private DrawingLayer enemy;


			public Point getPos()
			{
				return enemy.GetPosition();
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
				int x;
				int y;
				if (random.Next(0, 2) == 0)
					x = random.Next(0, 80);
				else
					x  = random.Next(720, 800);
				if (random.Next(0, 2) == 0)
					y = random.Next(0, 80);
				else
					y = random.Next(720, 800);

				enemy = new DrawingLayer("Player", new Rectangle(x, y, 80, 80));
			}

			public void Draw()
			{
				if (enemy.GetBound().Contains(Game1.cursor.getPos()))
				{
					if (Distance(player.getPos(), getPos()) > player.getRange())
						enemy.Draw(Color.Chocolate);
					else
						enemy.Draw(Color.Crimson);
					return;
				}

				if (ZombieDance)
					enemy.Draw(Color.White);
				else if (Score >= 10)
					enemy.Draw(Color.Black);
				else
					enemy.Draw(Color.White);
			}

			public void MoveUpdate()
			{
				if(ZombieDance)
				{
					if(Distance(getPos(),player.getPos())>160)
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

				enemy.MoveTo(player.getPos().X+random.Next(-300,300), player.getPos().Y+random.Next(-300,300), 5);
				if((Distance(player.getPos(),getPos()))<=10)
				{
					GameOver = true; 
				}
			}
		}
	}
}
