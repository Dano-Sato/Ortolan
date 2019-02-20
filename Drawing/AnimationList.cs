using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSheet
{
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
			Animationlist.Add(new DrawingLayerWithTimer(d,t));
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

		public void Clear()
		{
			Animationlist.Clear();
		}


	}
}
