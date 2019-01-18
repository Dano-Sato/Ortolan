using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TestSheet.Drawing
{
	public class DrawingSet
	{
		public List<DrawingLayer> drawingLayers;
		private List<Point> Vectors;//각 물체간 거리를 지정하는 벡터 집합입니다.
		private Rectangle Bound;

		public DrawingSet(DrawingLayer drawingLayer, Rectangle r)//기본 드로잉레이어 등록과 동시에 세트를 생성합니다.
		{
			drawingLayers = new List<DrawingLayer>();
			drawingLayers.Add(drawingLayer);
			Vectors.Add(new Point(0, 0));
			Bound = r;
		}

		public DrawingLayer this[int i]
		{
			get
			{
				return drawingLayers[i];
			}
			set
			{
				drawingLayers[i] = value;
			}

		}


		public void Add(DrawingLayer d)
		{
			drawingLayers.Add(d);
			Vectors.Add(new Point(0,0));
		}

		public void Add(DrawingLayer d, Point v)
		{
			drawingLayers.Add(d);
			Vectors.Add(v);
		}

		public void MoveTo(int x,int y)
		{
			Bound.Location = new Point(x, y);
			UpdateDisplacement();
		}

		public void UpdateDisplacement()//세트 이동에 따른 물체의 이동을 보정합니다.
		{
			Point setter = Bound.Location;
			for(int i=0;i<drawingLayers.Count;i++)
			{
				setter = new Point(setter.X + Vectors[i].X, setter.Y + Vectors[i].Y);
				drawingLayers[i].setPosition(setter);
			}
		}


	}
}
