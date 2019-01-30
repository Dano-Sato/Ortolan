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
		private List<DrawingLayer> Animationlist = new List<DrawingLayer>();
		private List<int> AnimationTimerList = new List<int>();


		public DrawingLayer this[int i]
		{
			get { return Animationlist[i]; }
			set { Animationlist[i] = value; }
		}


		public void Add(DrawingLayer d, int t)
		{
			Animationlist.Add(d);
			AnimationTimerList.Add(t);
		}

		public void TimeUpdate()
		{
			for (int i = 0; i < AnimationTimerList.Count; i++)
			{
				if (AnimationTimerList[i] == 0)
				{
					AnimationTimerList.RemoveAt(i);
					Animationlist.RemoveAt(i);
				}
				else
				{
					AnimationTimerList[i]--;
				}
			}
		}

		public void FadeAnimationDraw(Color color, double Opacity)
		{

			for (int i = 0; i < AnimationTimerList.Count; i++)
			{
				Animationlist[i].Draw(color * (float)(AnimationTimerList[i] * Opacity));
			}
		}


	}
}
