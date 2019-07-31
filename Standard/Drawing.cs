using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TestSheet
{
	public interface IGraphicLayer
	{
		void SetPos(int x, int y);
		void SetPos(Point p);
		string ToString();
		Point GetPos();
		Point GetCenter();
		Rectangle GetBound();
		void MoveTo(int x, int y, double speed);
		void Draw();
		void Draw(Color color);
	}

	public class StringLayer : IGraphicLayer
	{
		private string String;
		private Vector2 Pos;
		public StringLayer(string s, Vector2 pos)
		{
			String = s;
			Pos = pos;
		}

		public override string ToString()
		{
			return String;
		}

		public void SetPos(int x, int y)
		{
			Pos = new Vector2(x, y);
			//Pos_Safe=GetHash(x+y); : 현재 포지션의 해시값을 별도로 저장
		}
		public void SetPos(Point p)
		{
			SetPos(p.X, p.Y);
		}
		public Point GetPos()
		{
			//if(Pos_Safe!=Hash(x+y)) // 현재 변수의 해시값과 저장된 값이 다르면
			//{throw new Exception}//크래시 냄
			return GetBound().Location;
		}
		public Point GetCenter()
		{
			return GetBound().Center;
		}
		public void MoveTo(int x, int y, double speed)//등속운동합니다.
		{
			Point p = new Point((int)Pos.X, (int)Pos.Y);
			double Dx = (x - p.X);
			double Dy = (y - p.Y);
			double N = Math.Sqrt(Math.Pow(Dx, 2) + Math.Pow(Dy, 2));//두 물체 사이의 거리이자 노말벡터인자.
			if (N < speed)//거리가 스피드보다 가까우면 도착.
			{
				SetPos(x, y);
				return;
			}

			int X_Displacement = (int)(Dx * speed / N);
			int Y_Displacement = (int)(Dy * speed / N);

			SetPos(p.X + X_Displacement, p.Y + Y_Displacement);
		}

		public Rectangle GetBound()
		{
			return new Rectangle((int)Pos.X, (int)Pos.Y, (int)Standard.GetStringSize(String).X, (int)Standard.GetStringSize(String).Y);
		}
		public void Draw()
		{
			Standard.DrawString(String, Pos, Color.Black);
		}
		public void Draw(Color color)
		{
			Standard.DrawString(String, Pos, color);
		}
		public void Draw(Color color, float opacity)
		{
			Standard.DrawString(String, Pos, color * opacity);
		}

	}

	/* 드로잉이 행해지는 각 레이어의 클래스를 만들었습니다.
	 * 스트링 지정을 통해 스프라이트시트를 바꿀 수 있습니다.
	 * 등속 운동이 지원됩니다.
	 * 스프라이트 애니메이션이 지원됩니다.
	 * */
	public class DrawingLayer : IGraphicLayer
	{
		private Rectangle Bound;//화면에서 그림이 그려지는 영역을 표시합니다.
		private Texture2D spriteTexture;
		private Rectangle SourceRect;//스프라이트시트에서 소스영역의 크기를 나타냄. X,Y는 안씁니다.
		private int FrameRate = -1;

		private int FrameTimer = 0;//프레임 교체 타이밍을 재는 타이머.
								  //SpritePosition은 스프라이트 격자 좌표계상에서 프레임이 위치한 좌표를 난타냅니다.
		private SpritePosition Frame = new SpritePosition(0, 0);
		private SpritePosition SpriteSize = new SpritePosition(0, 0);//스프라이트 격자상의 최대 지점을 나타냅니다.

		private List<string> Actions = new List<string>();

		private double Ratio=1f;

		public DrawingLayer() : this("EmptySpace", new Rectangle(0, 0, 0, 0))
		{
			
		}

		public DrawingLayer(string s, Rectangle boundRect)//애니메이션이 없는 경우의 생성자. 
		{
			spriteTexture = Game1.content.Load<Texture2D>(s);
			Bound = boundRect;
		}

		public DrawingLayer(string s, Point Position, double ratio)//원본 스프라이트 크기에 대한 비율로 생성하는 경우
		{
			spriteTexture = Game1.content.Load<Texture2D>(s);
			Bound = new Rectangle(Position, new Point((int)(spriteTexture.Bounds.Width * ratio), (int)(spriteTexture.Bounds.Height * ratio)));
			Ratio = ratio;
		}

		public DrawingLayer(string s, Point Position, double ratio, SpritePosition spriteSize)
		{
			spriteTexture = Game1.content.Load<Texture2D>(s);
			SpriteSize = new SpritePosition(spriteSize.X, spriteSize.Y);
			int SourceW = spriteTexture.Bounds.Width / (SpriteSize.X + 1);
			int SourceH = spriteTexture.Bounds.Height / (SpriteSize.Y + 1);
			SourceRect = new Rectangle(0, 0, SourceW, SourceH);
			Bound = new Rectangle(Position, new Point((int)(SourceW*ratio), (int)(SourceH*ratio)));
			Ratio = ratio;
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
		/*getter-setter*/

		public string GetSpriteName()
		{
			return spriteTexture.ToString();
		}

		public Rectangle GetBound()
		{
			return Bound;
		}

		public Point GetPos()
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

		public void SetPos(int x, int y)
		{
			MoveTo(x, y);
		}

		public void SetPos(Point p)
		{
			MoveTo(p);
		}

		public void SetRatio(double ratio)
		{
			Point Center = GetCenter();
			int SourceW = spriteTexture.Bounds.Width / (SpriteSize.X + 1);
			int SourceH = spriteTexture.Bounds.Height / (SpriteSize.Y + 1);
			SourceRect = new Rectangle(0, 0, SourceW, SourceH);
			Bound = new Rectangle(GetPos(), new Point((int)(Bound.Width * ratio/Ratio), (int)(Bound.Height * ratio/Ratio)));
			Ratio = ratio;

			SetCenter(Center);
		}
		public SpritePosition GetFrame()
		{
			return Frame;
		}

		public void SetFrame(int x, int y)
		{
			Frame.X = x;
			Frame.Y = y;
		}
		public void SetSprite(string s)//스프라이트를 바꿉니다.
		{
			if (s.Equals(spriteTexture.ToString()))
				return;
			spriteTexture = Game1.content.Load<Texture2D>(s);
		}
		public void SetSprite(string s, SpritePosition spriteSize)//스프라이트를 바꾸면서 동시에 소스영역도 바꿉니다.
		{
			if (s.Equals(spriteTexture.ToString()))
				return;
			spriteTexture = Game1.content.Load<Texture2D>(s);
			int SourceW = spriteTexture.Bounds.Width / (SpriteSize.X + 1);
			int SourceH = spriteTexture.Bounds.Height / (SpriteSize.Y + 1);
			SourceRect = new Rectangle(0, 0, SourceW, SourceH);
		}
		public int GetTimer()
		{
			return FrameTimer;
		}

		/*드로잉 세션*/
		public void Draw()
		{
			if (SpriteSize.IsZero())//애니메이션이 없을 경우
			{
				Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				Game1.spriteBatch.Draw(spriteTexture, Bound, Color.White);
				Game1.spriteBatch.End();
				return;
			}
			Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Game1.spriteBatch.Draw(spriteTexture, Bound, ProcessedSourceRect(), Color.White);
			Game1.spriteBatch.End();
		}

		public void Draw(Color color)
		{
			if (SpriteSize.IsZero())//애니메이션이 없을 경우
			{
				Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				Game1.spriteBatch.Draw(spriteTexture, Bound, color);
				Game1.spriteBatch.End();
				return;
			}
			Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Game1.spriteBatch.Draw(spriteTexture, Bound, ProcessedSourceRect(), color);
			Game1.spriteBatch.End();
		}

		public void Draw(Color color, float opacity)
		{
			if (SpriteSize.IsZero())//애니메이션이 없을 경우
			{
				Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				Game1.spriteBatch.Draw(spriteTexture, Bound, color * opacity);
				Game1.spriteBatch.End();
				return;
			}
			Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Game1.spriteBatch.Draw(spriteTexture, Bound, ProcessedSourceRect(), color * opacity);
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
			if (N < speed)//거리가 스피드보다 가까우면 도착.
			{
				MoveTo(x, y);
				return;
			}

			int X_Displacement = (int)(Dx * speed / N);
			int Y_Displacement = (int)(Dy * speed / N);

			MoveTo(Bound.X + X_Displacement, Bound.Y + Y_Displacement);
		}
		public void CenterMoveTo(int x, int y, double speed)//등속운동합니다.
		{
			double Dx = (x -GetCenter().X);
			double Dy = (y - GetCenter().Y);
			double N = Math.Sqrt(Math.Pow(Dx, 2) + Math.Pow(Dy, 2));//두 물체 사이의 거리이자 노말벡터인자.
			if (N < speed)//거리가 스피드보다 가까우면 도착.
			{
				SetCenter(new Point(x, y));
				return;
			}

			int X_Displacement = (int)(Dx * speed / N);
			int Y_Displacement = (int)(Dy * speed / N);

			SetCenter(new Point(GetCenter().X + X_Displacement, GetCenter().Y + Y_Displacement));
		}

		public void MoveByVector(Point Vector, double speed)
		{
			double N = Method2D.Distance(new Point(0,0),Vector);
			int Dis_X = (int)(Vector.X * speed / N);
			int Dis_Y = (int)(Vector.Y * speed / N);
			MoveTo(Bound.X+Dis_X, Bound.Y+Dis_Y );
		}


		/* 애니메이션*/
		public void LoopedAnimation(SpritePosition Frame_Start, SpritePosition Frame_End, int FrameRate)
		{
			if (FrameTimer>0)
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

		public void AttachAnimation(int Frame_rate, params string[] Animations)
		{
			FrameRate = Frame_rate;
			foreach(string s in Animations)
			{
				Actions.Add("/s " + s);
			}
		}

		public void AttachAnimation(int Frame_rate, SpritePosition Frame_Start, SpritePosition Frame_End)
		{
			FrameRate = Frame_rate;
			Actions.Add("/sp " + Frame_Start.X+","+Frame_Start.Y+" "+ Frame_End.X + "," + Frame_End.Y);
		}

		public void ClearAnimation()
		{
			Actions.Clear();
		}

		public void Animate(bool isLooped)
		{
			if (Actions.Count == 0)
				return;
			else
			{
				if(FrameTimer>0)
				{
					FrameTimer--;
					return;
				}
				else
				{
					FrameTimer = FrameRate;
					string[] s = Actions[0].Split(' ');
					switch (s[0])
					{
						case "/s":
							SetSprite(Actions[0].Substring(3));
							if (isLooped)
								Actions.Add(Actions[0]);
							Actions.RemoveAt(0);
							break;
						case "/sp":
							SpritePosition Frame_Start = new SpritePosition(Int32.Parse(s[1].Split(',')[0]), Int32.Parse(s[1].Split(',')[1]));
							SpritePosition Frame_End = new SpritePosition(Int32.Parse(s[2].Split(',')[0]), Int32.Parse(s[2].Split(',')[1]));
							Frame.GoLoop(Frame_Start, Frame_End, SpriteSize);
							if(GetFrame()==Frame_End)
							{
								if (isLooped)
									Actions.Add(Actions[0]);
								Actions.RemoveAt(0);
							}
							break;
					}
				}
			}
		}
		private Rectangle ProcessedSourceRect()
		{

			return new Rectangle(SourceRect.Width * Frame.X, SourceRect.Height * Frame.Y, SourceRect.Width, SourceRect.Height);
		}


		/* 클릭처리*/
		public bool MouseIsOnThis()
		{
			return Bound.Contains(Cursor.GetPos());
		}

		public bool MouseJustLeftClickedThis()
		{
			return MouseIsOnThis() && Cursor.JustdidLeftClick();
		}

		public bool MouseIsLeftClickingThis()
		{
			return MouseIsOnThis() && Cursor.IsLeftClickingNow();
		}





	}




	/*파생 클래스*/
	public class DrawingLayerWithTimer
	{
		public DrawingLayer drawingLayer;
		public int Timer;
		public DrawingLayerWithTimer(DrawingLayer d, int t)
		{
			drawingLayer = d;
			Timer = t;
		}
	}


	public class DrawingLayerWithVector
	{
		public DrawingLayer drawingLayer;
		public Point Vector;

		public DrawingLayerWithVector(DrawingLayer d, Point v)
		{
			drawingLayer = d;
			Vector = v;
		}

		public void AttachTo(DrawingLayer d)
		{
			drawingLayer.SetPos(Method2D.Add(d.GetBound().Location,Vector));
		}

		public void AttachTo(Point p)
		{
			drawingLayer.SetPos(Method2D.Add(p,Vector));
		}
	}




	/*스프라이트 내부에서 프레임 위치를 지정해주는 클래스*/
	public class SpritePosition
	{
		public int X;
		public int Y;


		public SpritePosition(int x, int y)
		{
			X = x;
			Y = y;
		}
		public void Set(int x, int y)
		{
			X = x;
			Y = y;
		}

		public bool IsZero()
		{
			if (X == 0 && Y == 0)
				return true;
			return false;
		}

		public static bool operator <(SpritePosition s1, SpritePosition s2)
		{
			if (s1.Y < s2.Y)
				return true;
			else if (s1.Y > s2.Y)
				return false;
			else
			{
				if (s1.X < s2.X)
					return true;
				else
					return false;
			}
		}
		public static bool operator >(SpritePosition s1, SpritePosition s2)
		{
			if (s1.Y > s2.Y)
				return true;
			else if (s1.Y < s2.Y)
				return false;
			else
			{
				if (s1.X > s2.X)
					return true;
				else
					return false;
			}
		}

		public static bool operator ==(SpritePosition s1, SpritePosition s2)
		{
			return (s1.X == s2.X) && (s1.Y == s2.Y);
		}

		public static bool operator !=(SpritePosition s1, SpritePosition s2)
		{
			return !((s1.X == s2.X) && (s1.Y == s2.Y));
		}

		public void Increase()
		{
			X++;
		}
		public void Jump()
		{
			Y++;
			X = 0;
		}

		public void GoNext(SpritePosition SpriteSize)
		{
			if (this.X == SpriteSize.X)
				this.Jump();
			else
				this.Increase();
		}

		public void GoLoop(SpritePosition Max, SpritePosition Min, SpritePosition SpriteSize)
		{
			if (this < Min || this > Max)//이상한 범위에 있을 경우
			{
				this.Set(Min.X, Min.Y);
				return;
			}
			if (this < Max)
			{
				this.GoNext(SpriteSize);
				return;
			}
			if (this == Max)
			{
				this.Set(Min.X, Min.Y);
				return;
			}
		}



	}


	/*페이드애니메이션 처리를 위한 애니메이션 리스트*/

	public class AnimationList
	{
		//private List<DrawingLayer> Animationlist = new List<DrawingLayer>();
		//private List<int> AnimationTimerList = new List<int>();


		private List<DrawingLayerWithTimer> Animationlist = new List<DrawingLayerWithTimer>();
		private double DefaultOpacityCoefficient;

		public AnimationList(double OpacityC)
		{
			DefaultOpacityCoefficient = OpacityC;
		}

		public DrawingLayer this[int i]
		{
			get { return Animationlist[i].drawingLayer; }
			set { Animationlist[i].drawingLayer = value; }
		}


		public void Add(DrawingLayer d, int t)
		{
			Animationlist.Add(new DrawingLayerWithTimer(d, t));
		}
		public void Clear()
		{
			Animationlist.Clear();
		}

		public void TimeUpdate()
		{
			for (int i = 0; i < Animationlist.Count; i++)
			{
				if (Animationlist[i].Timer == 0)
				{
					Animationlist.RemoveAt(i);
				}
				else
				{
					Animationlist[i].Timer--;
				}
			}
		}

		public void FadeAnimationDraw(Color color, double Opacity)
		{

			for (int i = 0; i < Animationlist.Count; i++)
			{
				Animationlist[i].drawingLayer.Draw(color * (float)(Animationlist[i].Timer * Opacity));
			}
		}

		public void FadeAnimationDraw(Color color)
		{

			for (int i = 0; i < Animationlist.Count; i++)
			{
				Animationlist[i].drawingLayer.Draw(color * (float)(Animationlist[i].Timer * DefaultOpacityCoefficient));
			}
		}


	}
}
