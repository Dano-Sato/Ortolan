using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSheet
{
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

		public bool isZero()
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
}
