using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSheet
{
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
}
