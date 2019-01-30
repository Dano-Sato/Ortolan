using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TestSheet
{

	/* 게임 엔진의 기본적인 파트를 구현합니다.
	 * 기본 함수, Standard IO(커서 및 키보드 작동), 기본 사운드 전반을 담당합니다.
	 */

	public static class Standard
	{

		/*기본 인자*/
		public static int FrameTimer;//프레임 수를 센다.



		/*기본함수*/

		//첫 빌드에서 Game1.cs에 각각 이름이 일치하는 함수의 마지막 줄로 집어넣어주면 됩니다.
		public static void LoadContent()
		{
			SoundInit();
			cursor = new Cursor();
		}
		public static void Update()
		{
			cursor.OldStateUpdate();
			OldkeyboardState = Keyboard.GetState();
			FrameTimer++;
		}
		public static void Draw()
		{
			cursor.Draw();
		}



		/*입력 파트*/

		public static Cursor cursor;
		public static KeyboardState OldkeyboardState;

		public static bool KeyInputOccurs()
		{
			return Keyboard.GetState() != OldkeyboardState;
		}

		public static bool IsKeyDown(Keys k)
		{
			return Keyboard.GetState().IsKeyDown(k);
		}
		public static Keys[] GetPressedKeys()
		{
			return Keyboard.GetState().GetPressedKeys();
		}

		//유저가 막 key를 눌렀을 때를 체크하는 함수.
		public static bool JustPressed(Keys k)
		{
			return IsKeyDown(k) && !OldkeyboardState.IsKeyDown(k);
		}




		/*사운드 관련 파트*/

		public static SoundEffect soundEffect;
		public static Song song;
		public static List<String> SongCatalog = new List<string>();
		public static int SongIndex;
		public static float SongVolume;
		public static float SEVolume;



		public static void SoundInit()
		{
			// * Add Your Songs in here; In this Manner
			//AddSong("SongName1");
			//AddSong("SongName2");
			//AddSong("SongName3");
			//AddSong("SongName4");
		}


		public static void AddSong(string SongName)
		{
			SongCatalog.Add(SongName);
		}

		public static void PlaySong(bool Repeat)
		{
			song = Game1.content.Load<Song>(SongCatalog[SongIndex]);
			MediaPlayer.Volume = SongVolume;
			MediaPlayer.Play(song);
			MediaPlayer.IsRepeating = Repeat;
		}

		public static void PlaySong(int songIndex, bool Repeat)
		{
			SongIndex = songIndex;
			song = Game1.content.Load<Song>(SongCatalog[SongIndex]);
			MediaPlayer.Volume = SongVolume;
			MediaPlayer.Play(song);
			MediaPlayer.IsRepeating = Repeat;
		}

		public static void PlaySound(string SEName)
		{
			soundEffect = Game1.content.Load<SoundEffect>(SEName);
			SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
			soundEffectInstance.Volume = SEVolume;
			soundEffectInstance.Play();
		}


		/* 빛 관련 파트*/

		//Standard Content로서, WhiteSpace.png를 필요로 합니다.

		public enum LightMode { Absolute };
		
		public static void DrawLight(Rectangle Bound, Color color, float opacity, LightMode lightMode)
		{
			if(lightMode==LightMode.Absolute)
			{
				DrawingLayer AbsoluteLightLayer = new DrawingLayer("WhiteSpace", Bound);
				AbsoluteLightLayer.Draw(color, opacity);
			}
		}
		public static void DrawLight(DrawingLayer d, Color color, float opacity, LightMode lightMode)
		{
			if (lightMode == LightMode.Absolute)
			{
				DrawingLayer AbsoluteLightLayer = new DrawingLayer("WhiteSpace", d.GetBound());
				AbsoluteLightLayer.Draw(color, opacity);
			}
		}






		/*Point 관련 미구현 함수들 구현*/

		//포인트 덧셈
		public static Point Add(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}
		//포인트 간 거리 측정
		public static int Distance(Point a, Point b)
		{
			double x = a.X - b.X;
			double y = a.Y - b.Y;
			return (int)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}
		//포인트 a,b를 r:1-r로 내분하는 점 반환(r=0~1)
		public static Point DivPoint(Point a, Point b, double r)
		{
			double x = a.X * r + b.X * (1 - r);
			double y = a.Y * r + b.Y * (1 - r);
			return new Point((int)x, (int)y);
		}
	}
}
