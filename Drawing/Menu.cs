using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSheet
{
	public class Menu
	{
		public DrawingLayer Frame;
		public List<DrawingLayerWithVector> MenuList = new List<DrawingLayerWithVector>();
		public List<String> MenuStringList = new List<string>();
		private int MenuIndex=-1; 


		public Point GetPosition()
		{
			return Frame.GetPosition();
		}

		public int GetIndex()
		{
			return MenuIndex;
		}


		public void SetFrame(DrawingLayer frame)
		{
			Frame = frame;
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
				if(MenuList[i].drawingLayer.MouseIsOnThis())
				{
					MenuIndex = i;
					break;
				}
				MenuIndex = -1;
			}		
			MenuPositionUpdate();
		}


		public void MenuPositionUpdate()
		{
			Point Location = Frame.GetPosition();
			for (int i = 0; i < MenuList.Count; i++)
			{
				Location = Standard.Add(Location, MenuList[i].Vector);
				MenuList[i].drawingLayer.setPosition(Location);
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
		}

		public void MoveTo(int x, int y, double speed)
		{
			Frame.MoveTo(x, y, speed);
		}

		public bool MouseIsOnFrame()
		{
			return Frame.MouseIsOnThis();
		}
	}

	public class EasyMenu : Menu//스트링만 받으면 바로 검은 화면에 흰 테두리 메뉴를 그려준다.
	{
		private int EdgePixelSize = 7;
		private int StringInterval = 40;
		private Color FrameColor = Color.White;
		private Color MenuColor = Color.Black;
		private Color EdgeColor = Color.White;
		public EasyMenu(string[] strings,Rectangle FrameRectangle, Rectangle MenuRectangle, Point FirstMenuVector, Point MenuInterval)
		{
			SetFrame(new DrawingLayer("WhiteSpace",FrameRectangle));
			for(int i=0;i<strings.Length;i++)
			{
				if(i==0)
					AddMenu(new DrawingLayer("WhiteSpace", MenuRectangle), FirstMenuVector, strings[i]);
				else
					AddMenu(new DrawingLayer("WhiteSpace", MenuRectangle), MenuInterval, strings[i]);
			}
		}
		private void DrawEachMenuBox(DrawingLayer d, Color EdgeColor, Color InnerColor)
		{
			d.Draw(EdgeColor);
			DrawingLayer Inner = new DrawingLayer("WhiteSpace",new Rectangle(d.GetPosition().X+EdgePixelSize,d.GetPosition().Y+EdgePixelSize,d.GetBound().Width-2*EdgePixelSize,d.GetBound().Height-2*EdgePixelSize));
			Inner.Draw(InnerColor);
		}
		public override void Draw()
		{
			DrawEachMenuBox(Frame, Color.White, Color.White);
			for (int i = 0; i < MenuList.Count; i++)
			{
				DrawEachMenuBox(MenuList[i].drawingLayer, Color.White, Color.Black);
				Standard.DrawString(MenuStringList[i], new Vector2(MenuList[i].drawingLayer.GetPosition().X + EdgePixelSize + StringInterval, MenuList[i].drawingLayer.GetPosition().Y + 2 * EdgePixelSize), Color.White);
			}
			DrawMouseLight();
		}

		public void Draw(bool TurnOnMouseLight)
		{
			DrawEachMenuBox(Frame, Color.White, Color.White);
			for (int i = 0; i < MenuList.Count; i++)
			{
				DrawEachMenuBox(MenuList[i].drawingLayer, Color.White, Color.Black);
				Standard.DrawString(MenuStringList[i], new Vector2(MenuList[i].drawingLayer.GetPosition().X + EdgePixelSize + StringInterval, MenuList[i].drawingLayer.GetPosition().Y + 2 * EdgePixelSize), Color.White);
			}
			if (TurnOnMouseLight)
				DrawMouseLight();
		}
		public void DrawMouseLight()
		{
			if (GetIndex() != -1)
			{
				Standard.DrawLight(MenuList[GetIndex()].drawingLayer, Color.Black, 0.3f, Standard.LightMode.Vignette);
			}
		}

	}


}
