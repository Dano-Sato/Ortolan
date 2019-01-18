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
		public static int Timer = 0;
		public static int Score = 0;
		public static int ZombieTime = 40;
		//이후 마음대로 인수 혹은 콘텐츠들을 여기 추가할 수 있습니다.
		public Tester()//여기에서 각종 이니셜라이즈가 가능합니다.
		{
			player = new Player();
			enemies.Add(new Enemy());

		}
		//Game1.Class 내에 Tester.Update()로 추가될 업데이트문입니다. 다양한 업데이트 처리를 시험할 수 있습니다.
		public void Update()
		{
			Timer++;
			if(GameOver)
			{
				return;
			}
			player.MoveUpdate();
			player.AttackUpdate();
			foreach(Enemy e in enemies)
			{
				e.MoveUpdate();
			}

			ZombieTime = 40 - Score/10;
			if(Timer%ZombieTime==0)
			{
				enemies.Add(new Enemy());
			}

		}

		//Game1.Class 내에 Tester.Draw()로 추가될 드로우 액션문입니다. 다양한 드로잉을 시험할 수 있습니다.
		public void Draw()
		{
			player.Draw();
			player.DrawAttack();

			Game1.spriteBatch.Begin();
			Game1.spriteBatch.DrawString(font, "SCORE : "+Score, new Vector2(0, 0), Color.White);
			Game1.spriteBatch.End();

			foreach (Enemy e in enemies)
			{
				e.Draw();
			}
			if(GameOver)
			{
				Game1.spriteBatch.Begin();
				Game1.spriteBatch.DrawString(font, "Game Over", new Vector2(400, 400), Color.Red);
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
			private int AttackSpeed = 10;
			private int AttackTimer = 0;
			private int AttackIndex = -1;
			private bool PressedA=false;
			private bool isAttacking=false;


			public Point getPos()
			{
				return player.GetPosition();
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
				player.Draw(Color.Red*(float)((float)AttackTimer/AttackSpeed));
			}

			public void MoveUpdate()
			{
				if (isAttacking)
					return;
				if(Game1.cursor.didPlayerJustRightClick())
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
				}
				if(MovePoint.X!=0||MovePoint.Y!=0)
					player.MoveTo(MovePoint.X-40,MovePoint.Y-40, MoveSpeed);
			}

			public void AttackUpdate()
			{
				if(isAttacking)
				{
					MovePoint = new Point(0, 0);
					if(AttackTimer>0)//투사체 날아가는중
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
					bullet.setPosition(x+20, y+20);
					bullet.Draw(Color.Red);
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

					enemy.Draw(Color.Black);
			}

			public void MoveUpdate()
			{
				enemy.MoveTo(player.getPos().X+random.Next(-300,300), player.getPos().Y+random.Next(-300,300), 5);
				if((Distance(player.getPos(),getPos()))<=10)
				{
					GameOver = true; 
				}
			}
		}
	}
}
