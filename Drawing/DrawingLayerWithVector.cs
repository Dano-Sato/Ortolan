using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSheet
{
	public class DrawingLayerWithVector
	{
		public DrawingLayer drawingLayer;
		public Point Vector;

		public DrawingLayerWithVector(DrawingLayer d, Point v)
		{
			drawingLayer = d;
			Vector = v;
		}
	}
}
