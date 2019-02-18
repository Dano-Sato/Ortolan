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

		//Game1.LoadContent()에서 spriteBatch, content 초기화 직후에 넣어야 합니다.
		//이후 클래스의 초기화 시 Standard 클래스 내부 콘텐트를 활용할 가능성이 있기 때문입니다.
		public static void LoadContent()
		{
			SoundInit();
			FadeAnimationInit();
			Standardfont = Game1.content.Load<SpriteFont>("StandardFont");
			cursor = new Cursor();
		}
		//Update, Draw 함수는 Game1.cs에서  각각 이름이 일치하는 함수의 마지막 줄로 집어넣어주면 됩니다.
		//커서, 키보드의 올드 스테이트 업데이트 및 드로잉은 가장 마지막에 행해져야 하기 때문입니다.
		public static void Update()
		{
			cursor.OldStateUpdate();
			OldkeyboardState = Keyboard.GetState();
			FrameTimer++;
			FadeAnimationUpdate();
		}
		public static void Draw()
		{
			FadeAnimationDraw();
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
		public static float SongVolume=1.0f;
		public static float SEVolume=1.0f;


		// * Make your own song Name List;
		public enum SongNameList {March, Etude, Polonaise, Ballade, BWV, Tchai };

		public static void SoundInit()
		{
			// * Add Your Songs in here; In this Manner
			//AddSong("SongName0");
			//AddSong("SongName1");
			//AddSong("SongName2");
			//AddSong("SongName3");
			AddSong("March3");          //0          
			AddSong("Etude3");          //1
			AddSong("Polonaise3");      //2
			AddSong("Ballade8");        //3			
			AddSong("BWV3");            //4
			AddSong("Tchai3");			//5
		}


		public static void SetSongVolume(float volume)//송의 기본 볼륨을 정한다.
		{
			SongVolume = volume;
			MediaPlayer.Volume = SongVolume;
		}
		public static void FadeSong(float Coefficient)//송에 페이드 효과를 가한다. 기본 볼륨은 변하지 않는다.
		{
			MediaPlayer.Volume = SongVolume * Coefficient;
		}
		public static void SetSEVolume(float volume)//사운드의 기본 볼륨을 정한다.
		{
			SEVolume = volume;
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
		public static void PlaySong(bool Repeat, float Coefficient)
		{
			song = Game1.content.Load<Song>(SongCatalog[SongIndex]);
			MediaPlayer.Volume = SongVolume*Coefficient;
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
		public static void PlaySong(int songIndex, bool Repeat,float Coefficient)
		{
			SongIndex = songIndex;
			song = Game1.content.Load<Song>(SongCatalog[SongIndex]);
			MediaPlayer.Volume = SongVolume*Coefficient;
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
		public static void PlaySound(string SEName,float Coefficient)
		{
			soundEffect = Game1.content.Load<SoundEffect>(SEName);
			SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
			soundEffectInstance.Volume = SEVolume*Coefficient;
			soundEffectInstance.Play();
		}



		/* 빛 관련 파트*/

		//Standard Content로서, WhiteSpace.png,Light.png를 필요로 합니다.

		public enum LightMode { Absolute,Vignette };
		
		public static void DrawLight(Rectangle Bound, Color color, float opacity, LightMode lightMode)
		{
			if(lightMode==LightMode.Absolute)
			{
				DrawingLayer AbsoluteLightLayer = new DrawingLayer("WhiteSpace", Bound);
				AbsoluteLightLayer.Draw(color, opacity);
			}
			else if(lightMode==LightMode.Vignette)
			{
				DrawingLayer AbsoluteLightLayer = new DrawingLayer("Light", Bound);
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
			else if (lightMode == LightMode.Vignette)
			{
				DrawingLayer AbsoluteLightLayer = new DrawingLayer("Light", d.GetBound());
				AbsoluteLightLayer.Draw(color, opacity);
			}
		}

		public static void DrawAddon(DrawingLayer d, Color color, float opacity, string LayerName)
		{
			DrawingLayer AddonLayer = new DrawingLayer(LayerName, d.GetBound());
			AddonLayer.Draw(color, opacity);
		}






		/*Point 관련 미구현 함수들 구현*/

		//포인트 덧셈
		public static Point Add(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}

		public static Point Deduct(Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
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


		/*페이드 애니메이션 처리*/

		private static Dictionary<Color, AnimationList> FadeAnimationList = new Dictionary<Color, AnimationList>();
		private static List<Color> FadeAnimation_ColorException=new List<Color>();

		private static void FadeAnimationInit()
		{
			//제외할 컬러들을 선택한다.
			FadeAnimation_ColorException.Add(Color.LightSeaGreen);
		}

		public static void FadeAnimation(DrawingLayer d, int t)
		{
			if(FadeAnimationList.ContainsKey(Color.White))
			{
				FadeAnimationList[Color.White].Add(d,t);
			}
			else
			{
				FadeAnimationList.Add(Color.White, new AnimationList(1.0/t));
				FadeAnimationList[Color.White].Add(d, t);
			}
		}

		public static void FadeAnimation(DrawingLayer d, int t,Color color)
		{
			if (FadeAnimationList.ContainsKey(color))
			{
				FadeAnimationList[color].Add(d, t);
			}
			else
			{
				FadeAnimationList.Add(color, new AnimationList(1.0 / t));
				FadeAnimationList[color].Add(d, t);
			}
		}

		public static void FadeAnimationUpdate()
		{
			foreach(KeyValuePair<Color,AnimationList> kv in FadeAnimationList)
			{
				kv.Value.TimeUpdate();
			}
		}

		public static void FadeAnimationDraw()
		{
			foreach (KeyValuePair<Color, AnimationList> kv in FadeAnimationList)
			{
				if(!FadeAnimation_ColorException.Contains(kv.Key))
					kv.Value.FadeAnimationDraw(kv.Key);
			}
		}


		public static void FadeAnimationDraw(Color color)
		{
			if(FadeAnimationList.ContainsKey(color))
			{
				FadeAnimationList[color].FadeAnimationDraw(color);
			}
		}


		/*뷰포트*/

		/*활용을 위해서는 Game1.Update()에서
		 	GraphicsDevice.Viewport = Standard.Viewport;
			를 선언해 주셔야 합니다.*/
		public static Viewport Viewport = new Viewport();



		/*기타*/

		//StandardFont.spritefont를 필요로 합니다.
		private static SpriteFont Standardfont;
		private static SpriteFont Temporaryfont;
		public static void DrawString(string s, Vector2 vector2,Color color)
		{
			Game1.spriteBatch.Begin();
			Game1.spriteBatch.DrawString(Standardfont, s, vector2, color);
			Game1.spriteBatch.End();
		}

		public static void DrawString(string FontName,string s, Vector2 vector2, Color color)
		{
			Temporaryfont = Game1.content.Load<SpriteFont>(FontName);
			Game1.spriteBatch.Begin();
			Game1.spriteBatch.DrawString(Temporaryfont, s, vector2, color);
			Game1.spriteBatch.End();
		}


		private static Random random=new Random();
		public static int Random(int x, int y)
		{
			return random.Next(Math.Min(x, y), Math.Max(x, y));
		}

		public static double Random()
		{
			return random.NextDouble();
		}


	}
}
