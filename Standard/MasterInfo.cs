using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TestSheet
{
	public class MasterInfo
	{
		public static readonly Rectangle PreferredScreen = new Rectangle(0, 0, 1200, 800);
		public static readonly Rectangle FullScreen = new Rectangle(0, 0, 1300, 1000);
		public static readonly int GridSize = 30;
		public static readonly Color PlayerColor = Color.Orange;
		public static Color ThemeColor = Color.LightSeaGreen;
		public MasterInfo()
		{

		}
	}
}
