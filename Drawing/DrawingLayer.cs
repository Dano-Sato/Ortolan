using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TestSheet
{

	/* 드로잉이 행해지는 각 레이어의 클래스를 만들었습니다.
	 * 스트링 지정을 통해 스프라이트시트를 바꿀 수 있습니다.
	 * 등속 운동이 지원됩니다.
	 * 스프라이트 애니메이션이 지원됩니다.
	 * */
	public class DrawingLayer
	{
		private Rectangle Bound;//화면에서 그림이 그려지는 영역을 표시합니다.
		private Texture2D spriteTexture;
		private Rectangle SourceRect;//스프라이트시트에서 소스영역의 크기를 나타냄. X,Y는 안씁니다.

		private int FrameTimer = 0;//프레임 교체 타이밍을 재는 타이머.
								  //SpritePosition은 스프라이트 격자 좌표계상에서 프레임이 위치한 좌표를 난타냅니다.
		private SpritePosition Frame = new SpritePosition(0, 0);
		private SpritePosition SpriteSize = new SpritePosition(0, 0);//스프라이트 격자상의 최대 지점을 나타냅니다.



		public DrawingLayer(string s, Rectangle boundRect)//애니메이션이 없는 경우의 생성자. 
		{
			spriteTexture = Game1.content.Load<Texture2D>(s);
			Bound = boundRect;
		}

		public DrawingLayer(string s, Rectangle boundRect, SpritePosition spriteSize)
		{
			//스프라이트시트의 기본 상수들을 지정합니다.
			spriteTexture = Game1.content.Load<Texture2D>(s);
			SpriteSize = new SpritePosition(spriteSize.X, spriteSize.Y);
			int SourceW = spriteTexture.Bounds.Width / (SpriteSize.X + 1);
			int SourceH = spriteTexture.Bounds.Height / (SpriteSize.Y + 1);
			SourceRect = new Rectangle(0, 0, SourceW, SourceH);

			//그림이 그려질 화면 영역을 지정합니다.
			Bound = boundRect;
		}


		public DrawingLayer(string s, Point Position, double ratio)
		{
			spriteTexture = Game1.content.Load<Texture2D>(s);
			Bound = new Rectangle(Position, new Point((int)(spriteTexture.Bounds.Width * ratio), (int)(spriteTexture.Bounds.Height * ratio)));
		}

		public DrawingLayer(string s, Point Position, double ratio, SpritePosition spriteSize)
		{
			spriteTexture = Game1.content.Load<Texture2D>(s);
			SpriteSize = new SpritePosition(spriteSize.X, spriteSize.Y);
			int SourceW = spriteTexture.Bounds.Width / (SpriteSize.X + 1);
			int SourceH = spriteTexture.Bounds.Height / (SpriteSize.Y + 1);
			SourceRect = new Rectangle(0, 0, SourceW, SourceH);
			Bound = new Rectangle(Position, new Point((int)(SourceW * ratio), (int)(SourceH * ratio)));
		}

		/*getter-setter*/

		public string GetSpriteName()
		{
			return spriteTexture.ToString();
		}

		public Rectangle GetBound()
		{
			return Bound;
		}

		public Point GetPosition()
		{

			return Bound.Location;
		}

		public Point GetCenter()
		{
			return Bound.Center;
		}

		public override string ToString()
		{
			return spriteTexture.ToString();
		}

		public void setPosition(int x, int y)
		{
			MoveTo(x, y);
		}

		public void setPosition(Point p)
		{
			MoveTo(p);
		}

		public Point GetFrame()
		{
			return new Point(Frame.X, Frame.Y);
		}

		public void SetFrame(int x, int y)
		{
			Frame.X = x;
			Frame.Y = y;
		}
		public void setSprite(string s)//스프라이트를 바꿉니다.
		{
			if (s.Equals(spriteTexture.ToString()))
				return;
			spriteTexture = Game1.content.Load<Texture2D>(s);
		}
		public void setSprite(string s, SpritePosition spriteSize)//스프라이트를 바꾸면서 동시에 소스영역도 바꿉니다.
		{
			if (s.Equals(spriteTexture.ToString()))
				return;
			spriteTexture = Game1.content.Load<Texture2D>(s);
			int SourceW = spriteTexture.Bounds.Width / (SpriteSize.X + 1);
			int SourceH = spriteTexture.Bounds.Height / (SpriteSize.Y + 1);
			SourceRect = new Rectangle(0, 0, SourceW, SourceH);
		}
		public int getTimer()
		{
			return FrameTimer;
		}

		/*드로잉 세션*/
		public void Draw()
		{
			if (SpriteSize.isZero())//애니메이션이 없을 경우
			{
				Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				Game1.spriteBatch.Draw(spriteTexture, Bound, Color.White);
				Game1.spriteBatch.End();
				return;
			}
			Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Game1.spriteBatch.Draw(spriteTexture, Bound, processedSourceRect(), Color.White);
			Game1.spriteBatch.End();
		}

		public void Draw(Color color)
		{
			if (SpriteSize.isZero())//애니메이션이 없을 경우
			{
				Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				Game1.spriteBatch.Draw(spriteTexture, Bound, color);
				Game1.spriteBatch.End();
				return;
			}
			Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Game1.spriteBatch.Draw(spriteTexture, Bound, processedSourceRect(), color);
			Game1.spriteBatch.End();
		}

		public void Draw(Color color, float opacity)
		{
			if (SpriteSize.isZero())//애니메이션이 없을 경우
			{
				Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				Game1.spriteBatch.Draw(spriteTexture, Bound, color * opacity);
				Game1.spriteBatch.End();
				return;
			}
			Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Game1.spriteBatch.Draw(spriteTexture, Bound, processedSourceRect(), color * opacity);
			Game1.spriteBatch.End();
		}


		/* 이동 세션*/
		public void SetBound(Rectangle r)
		{
			Bound = r;
		}

		public void SetCenter(Point p)
		{
			Bound.X = p.X-Bound.Width/2;
			Bound.Y = p.Y - Bound.Height/ 2;
		}

		public void MoveTo(Point p)
		{
			Bound = new Rectangle(p.X, p.Y, Bound.Width, Bound.Height);
		}

		public void MoveTo(int x, int y)
		{
			Bound = new Rectangle(x, y, Bound.Width, Bound.Height);
		}
		public void MoveTo(int x, int y, double speed)//등속운동합니다.
		{
			double Dx = (x - Bound.X);
			double Dy = (y - Bound.Y);
			double N = Math.Sqrt(Math.Pow(Dx, 2) + Math.Pow(Dy, 2));//두 물체 사이의 거리이자 노말벡터인자.
			if (N < Math.Abs(speed))//거리가 스피드보다 가까우면 도착.
			{
				MoveTo(x, y);
				return;
			}

			int X_Displacement = (int)(Dx * speed / N);
			int Y_Displacement = (int)(Dy * speed / N);

			MoveTo(Bound.X + X_Displacement, Bound.Y + Y_Displacement);
		}

		public void MoveByVector(Point Vector, double speed)
		{
			double N = Standard.Distance(new Point(0,0),Vector);
			int Dis_X = (int)(Vector.X * speed / N);
			int Dis_Y = (int)(Vector.Y * speed / N);
			MoveTo(Bound.X+Dis_X, Bound.Y+Dis_Y );
		}


		/* 애니메이션*/
		public void Animate(SpritePosition Frame_Start, SpritePosition Frame_End, int FrameRate)
		{
			if (FrameTimer != 0)
			{
				FrameTimer--;//일하는 시간이 아닙니다.
				return;
			}
			else//종이 칩니다. 시간이 되었습니다. 일을 합시다.
			{
				FrameTimer = FrameRate;//다시 스톱워치를 맞춥니다.
				Frame.GoLoop(Frame_End, Frame_Start, SpriteSize);//애니메이션을 다음 프레임으로 이동시킵니다.
				return;//애니메이터는 자러 갑니다.
			}
		}
		private Rectangle processedSourceRect()
		{

			return new Rectangle(SourceRect.Width * Frame.X, SourceRect.Height * Frame.Y, SourceRect.Width, SourceRect.Height);
		}


		/* 클릭처리*/
		public bool MouseIsOnThis()
		{
			return Bound.Contains(Standard.cursor.getPos());
		}

		public bool MouseJustLeftClickedThis()
		{
			return MouseIsOnThis() && Standard.cursor.didPlayerJustLeftClick();
		}

		public bool MouseIsLeftClickingThis()
		{
			return MouseIsOnThis() && Standard.cursor.IsPlayerLeftClickingNow();
		}





	}
}
