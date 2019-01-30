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
		private DrawingLayer Frame;
		private List<DrawingLayer> MenuList=new List<DrawingLayer>();
		private List<Point> VectorList= new List<Point>();

		public void MenuPositionUpdate()
		{
			Point Location = Frame.GetPosition();
			for(int i=0;i<MenuList.Count;i++)
			{

			}
		}
	}
}
