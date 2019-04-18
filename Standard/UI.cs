using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSheet
{

	/* 메뉴들을 포함하는 전체 프레임이 있고, 그 프레임에 벡터로 줄줄이 연결된 메뉴들이 있는 형태다.
	 * 업데이트 함수에서 메뉴들을 연결하고, 마우스가 특정 메뉴위에 있으면 그 메뉴의 인덱스를 저장한다.
	 * 생성자를 반복 호출하면 메뉴가 정상작동하지 않습니다. 
	 * */
	public class Menu : IDrawLayer
	{
		public DrawingLayer Frame;//메뉴들을 포함하는 프레임.
		public List<DrawingLayerWithVector> MenuList = new List<DrawingLayerWithVector>();//메뉴들을 벡터로 연결된 리스트 형태로 저장한다.
		public List<String> MenuStringList = new List<string>();

		private int MenuIndex=-1; 

		public Point GetPos()
		{
			return Frame.GetPos();
		}

		public int GetIndex()
		{
			return MenuIndex;
		}


		public void SetFrame(DrawingLayer frame)
		{
			Frame = frame;
		}

		public void SetPos(int x,int y)
		{
			Frame.SetPos(x, y);
			MenuPositionUpdate();
		}

		public void SetPos(Point P)
		{
			Frame.SetPos(P);
			MenuPositionUpdate();
		}

		public void SetSprite(string FrameSpriteName, string MenuSpriteName)
		{
			Frame.setSprite(FrameSpriteName);
			for(int i=0;i<MenuList.Count;i++)
			{
				MenuList[i].drawingLayer.setSprite(MenuSpriteName);
			}
		}

		public void AddMenu(DrawingLayer menu, Point p, string s)
		{
			MenuList.Add(new DrawingLayerWithVector(menu, p));
			MenuStringList.Add(s);
		}


		public void Update()
		{			
			for(int i=0;i<MenuList.Count;i++)
			{
				if(MenuList[i].drawingLayer.MouseIsOnThis())//마우스가 특정 메뉴 위에 있으면
				{
					MenuIndex = i;//해당인덱스를 저장한다.
					break;
				}
				MenuIndex = -1;//없으면 -1저장.
			}		
			MenuPositionUpdate();
		}


		protected void MenuPositionUpdate()
		{
			if(MenuList.Count>0)
			{
				MenuList[0].AttachTo(Frame);
				for (int i = 1; i < MenuList.Count; i++)
				{
					MenuList[i].AttachTo(MenuList[i - 1].drawingLayer);
				}
			}
		}
		public virtual void Draw()
		{
			Frame.Draw();
			for (int i = 0; i < MenuList.Count; i++)
			{
				MenuList[i].drawingLayer.Draw();
			}
		}

		public void MoveTo(int x, int y)
		{
			Frame.MoveTo(x, y);
			MenuPositionUpdate();
		}

		public void MoveTo(int x, int y, double speed)
		{
			Frame.MoveTo(x, y, speed);
			MenuPositionUpdate();
		}

		public void AddVector(Point p)
		{
			for(int i=0;i<MenuList.Count;i++)
			{
				MenuList[i].Vector = Method2D.Add(MenuList[i].Vector, p);
			}
		}

		public bool MouseIsOnFrame()
		{
			return Frame.MouseIsOnThis();
		}
		public bool MouseIsOnMenu()
		{
			return MenuIndex != -1;
		}
	}

	public class EasyMenu : Menu//스트링만 받으면 바로 검은 화면에 흰 테두리 메뉴를 그려준다.
	{
		private int EdgePixelSize = 7;
		private int StringInterval = 40;
		private Color FrameColor = Color.White;
		private Color MenuColor = Color.Black;
		private Color EdgeColor = Color.White;
		public enum EasyMenuTheme { MonoVirus };

		public static EasyMenuTheme Theme = EasyMenuTheme.MonoVirus;

		public EasyMenu() : this(new Rectangle(0,0,0,0),new Rectangle(0,0,0,0),new Point(0,0),new Point(0,0), new string[] { })
		{

		}

		//가장 단순한 형태의 메뉴 생성자. 메뉴 이름&메뉴 위치&메뉴 두께만 설정하면 됨.
		public EasyMenu(Point position, int Width, params string[] strings)
		{
			int MenuCount = strings.Length;
			int MenuHeight = 50;
			Point FirstMenuVector = new Point(20, 20);
			Point MenuInterval = new Point(0, 20);
			Rectangle FrameRectangle = new Rectangle(position, new Point(Width+FirstMenuVector.X,FirstMenuVector.Y+(MenuInterval.Y+MenuHeight)*MenuCount));
			Rectangle MenuRectangle = new Rectangle(0, 0, Width, MenuHeight);
			SetFrame(new DrawingLayer("EmptySpace", FrameRectangle));
			for (int i = 0; i < strings.Length; i++)
			{
				if (i == 0)
					AddMenu(new DrawingLayer("EmptySpace", MenuRectangle), FirstMenuVector, strings[i]);
				else
					AddMenu(new DrawingLayer("EmptySpace", MenuRectangle), Method2D.Add(MenuInterval, new Point(0, MenuRectangle.Height)), strings[i]);
			}
			if(Theme==EasyMenuTheme.MonoVirus)
				SetSprite("WhiteSpace", "Light");
		}

		//메뉴 간격을 조정할 수 있는 생성자.
		public EasyMenu(Rectangle FrameRectangle, Rectangle MenuRectangle, Point FirstMenuVector, Point MenuInterval, string[] strings)
		{
			SetFrame(new DrawingLayer("EmptySpace", FrameRectangle));
			for(int i=0;i<strings.Length;i++)
			{
				if(i==0)
					AddMenu(new DrawingLayer("EmptySpace", MenuRectangle), FirstMenuVector, strings[i]);
				else
					AddMenu(new DrawingLayer("EmptySpace", MenuRectangle), Method2D.Add(MenuInterval,new Point(0,MenuRectangle.Height)), strings[i]);
			}
			if (Theme == EasyMenuTheme.MonoVirus)
				SetSprite("WhiteSpace", "Light");

		}
		private void DrawEachMenuBox(DrawingLayer d, Color EdgeColor, Color InnerColor)//테두리와 내부를 별도로 그리기 위한 일회성 메소드.
		{
			d.Draw(EdgeColor);
			DrawingLayer Inner = new DrawingLayer("Light",new Rectangle(d.GetPos().X+EdgePixelSize,d.GetPos().Y+EdgePixelSize,d.GetBound().Width-2*EdgePixelSize,d.GetBound().Height-2*EdgePixelSize));
			Inner.Draw(InnerColor);
		}
		public override void Draw()
		{
			DrawEachMenuBox(Frame, Color.DarkSeaGreen*0.7f, Color.Black*0.8f);
			DrawEachMenuBox(Frame, Color.DarkOrchid * 0.1f, Color.Black * 0.2f);
			for (int i = 0; i < MenuList.Count; i++)
			{
				DrawEachMenuBox(MenuList[i].drawingLayer, Color.White, Color.Black);
				Standard.DrawString(MenuStringList[i], new Vector2(MenuList[i].drawingLayer.GetPos().X + EdgePixelSize + StringInterval, MenuList[i].drawingLayer.GetPos().Y + 2 * EdgePixelSize), Color.White);
			}
			DrawMenuLight();
		}

		//마우스가 메뉴 위에 올려질 경우 메뉴에 빛을 주는 메소드.
		public void DrawMenuLight()
		{
			if (GetIndex() != -1)
			{
				Standard.DrawLight(MenuList[GetIndex()].drawingLayer, Color.White, 0.3f, Standard.LightMode.Absolute);
			}
		}

	}

	/* 스크롤바. 드래깅이 가능하다.
	 * 스크롤바의 위치에 따라 최종적으로는 0~1 사이의 값을 반환한다.
	 * 이후 스크롤과의 연동 기능 추가할 예정.
	 * 현재는 바로 배경음/효과음 볼륨으로 변경할 수 있습니다.
	 * */

	public class ScrollBar
	{
		public DrawingLayer Frame;
		public DrawingLayer Bar;
		public bool isVertical = false;
		public int Interval;
		public float Coefficient=0;
		public ScrollBar(DrawingLayer frame,string BarSpriteName, int BarSize, bool is_vertical)
		{
			Frame = frame;
			isVertical = is_vertical;
			if (isVertical)
				Interval = Frame.GetBound().Height - BarSize;
			else
				Interval = Frame.GetBound().Width - BarSize;

			if (is_vertical)
			{
				Bar = new DrawingLayer(BarSpriteName, new Rectangle(Frame.GetPos(), new Point(Frame.GetBound().Width, BarSize)));
			}
			else
			{
				Bar = new DrawingLayer(BarSpriteName, new Rectangle(Frame.GetPos().X + Interval,Frame.GetPos().Y,BarSize,Frame.GetBound().Height));
			}
		}
		public void Update()
		{

			//마우스 입력 처리
			if (Cursor.IsDragging(Bar)||(Cursor.JustdidLeftClick() && Cursor.IsOn(Frame)))
			{
				if (isVertical)
				{
					Bar.SetPos(Bar.GetPos().X, Cursor.getPos().Y);
				}
				else
					Bar.SetPos(Cursor.getPos().X, Bar.GetPos().Y);
			}

			//바가 범위를 벗어나지 않도록 조정.
			if (isVertical)
			{
				if (Bar.GetPos().Y < Frame.GetPos().Y)
					Bar.SetPos(Frame.GetPos());
				if (Bar.GetPos().Y > Frame.GetPos().Y + Interval)
					Bar.SetPos(Bar.GetPos().X, Frame.GetPos().Y + Interval);
				Bar.SetPos(Frame.GetPos().X, Bar.GetPos().Y);
			}
			else
			{
				if (Bar.GetPos().X < Frame.GetPos().X)
					Bar.SetPos(Frame.GetPos());
				if (Bar.GetPos().X > Frame.GetPos().X + Interval)
					Bar.SetPos(Frame.GetPos().X + Interval, Bar.GetPos().Y );
				Bar.SetPos(Bar.GetPos().X, Frame.GetPos().Y);
			}

			//계수조정.
			if (isVertical)
				Coefficient = 1 - (Bar.GetPos().Y - Frame.GetPos().Y) / (float)Interval;
			else
				Coefficient = (Bar.GetPos().X - Frame.GetPos().X) / (float)Interval;
		}

		public void Draw()
		{
			Frame.Draw();
			Bar.Draw();
		}

		public float MapCoefficient(float From, float To)
		{
			return Coefficient * (To - From) + From;
		}
		public void Initialize(float initCoefficient)
		{
			Coefficient = initCoefficient;
			if (isVertical)
				Bar.SetPos(0, (int)((1.0f - Coefficient) * Interval + Frame.GetPos().Y));
			else
				Bar.SetPos((int)((1.0f - Coefficient) * Interval + Frame.GetPos().X), 0);
		}

	}


	public class Scroll
	{
		private Point ScrollVertex;
		private Viewport ScrollViewport;
		private Viewport OldViewport;
		private int WholeScrollLength;
		private bool IsVertical=true;

		public Scroll(Point Position,Rectangle Origin, int ViewportLength, bool isVertical)
		{
			ScrollVertex = Position;
			ScrollViewport = new Viewport(new Rectangle(Position,new Point(Origin.Width,ViewportLength)));
			WholeScrollLength = Origin.Height;
			IsVertical = isVertical;
		}

		public Point getPos()
		{
			return ScrollVertex;
		}

		public void Attach(ScrollBar bar)
		{
			if(WholeScrollLength>ScrollViewport.Height)
			{
				if (IsVertical)
					ScrollVertex.Y = ScrollViewport.Y - (int)((1 - bar.Coefficient) * (WholeScrollLength - ScrollViewport.Height));
			}
		}


		public void Draw(IDrawLayer AttachedDrawLayer)
		{
			OldViewport = Game1.graphics.GraphicsDevice.Viewport;
			Game1.graphics.GraphicsDevice.Viewport = ScrollViewport;
			AttachedDrawLayer.SetPos(ScrollVertex.X-ScrollViewport.X,ScrollVertex.Y - ScrollViewport.Y);
			AttachedDrawLayer.Draw();
			AttachedDrawLayer.SetPos(ScrollVertex.X, ScrollVertex.Y);
			Game1.graphics.GraphicsDevice.Viewport = OldViewport;
		}

		public bool Contains(Point p)//뷰포트 안에 특정 점이 있는지를 확인한다. 클릭 등을 처리할 때 필요.
		{
			return ScrollViewport.Bounds.Contains(p);
		}




	}



	/* 각종 대사 및 나레이션을 출력하는 스크립터.
	 * 외부 텍스트 파일을 읽어올수도 있고, C# 내에서 직접 이니셜라이즈 가능
	 * 기본적으로 문장을 분해하는 능력이 있어서 연극 대본처럼 입력하면 바로 스크립트를 읽어준다.
	 * 	 요런 느낌으로 입력하면 된다.
	 * 	string[] Lines = new string[] { "안젤리카 : (웃는 얼굴) 당신이 그녀의 아버지가 되어주시겠습니까?",
		"안젤리카 : (눈을 감으며) 그녀에겐 꿈이 있었답니다",
		"나흐트 : (화난 얼굴) 닥쳐라. 빌어먹을 적군년",
		"안젤리카 : (흥분) 하악.. 기뻐라"};
	 * */



	public class Scripter
	{
		private List<string> Lines;
		private List<string> Backlog;
		private string Script;

		public void Initialize(params string[] strings)
		{
			for(int i=0;i<strings.Length;i++)
			{
				Lines.Add(strings[i]);
			}
		}
		public void LoadText(string TextName)
		{

		}

		public void ReadNext()
		{
			Script = Lines[0];
			Backlog.Add(Lines[0]);
			Lines.RemoveAt(0);
		}
	}




}
