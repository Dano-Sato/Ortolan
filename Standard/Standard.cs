using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Timers;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TestSheet
{

	/* 게임 엔진의 기본적인 파트를 구현합니다.
	 * 기본 함수, Standard IO(커서 및 키보드 작동), 기본 사운드 전반을 담당합니다.
	 */

	public class MouseSetter
	{
		
		

	}

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
			RootMouseSpeed = GetMouseSpeed();
			Mouse.SetPosition(400, 400);
 
		}
		//Update, Draw 함수는 Game1.cs에서  각각 이름이 일치하는 함수의 마지막 줄로 집어넣어주면 됩니다.
		//커서, 키보드의 올드 스테이트 업데이트 및 드로잉은 가장 마지막에 행해져야 하기 때문입니다.
		public static void Update()
		{
			Cursor.OldStateUpdate();
			OldkeyboardState = Keyboard.GetState();
			FrameTimer++;
			FadeAnimationUpdate();
			if(SongFadeTimer>0)
			{
				SongFadeTimer--;
				if(FadedSong.Count>0)
				{
					FadedSong[0].Volume = (SongVolume * SongFadeTimer) / SongFadeTimer_Initial;
				}
			}
			else
			{
				if (FadedSong.Count > 0)
				{
					FadedSong[0].Stop();
					FadedSong.Clear();
				}
			}
            if(DisposedSong.Count>0)
            {
                foreach (SoundEffectInstance s in DisposedSong.Keys.ToList())
                {
                    DisposedSong[s]--;
                    s.Volume = (float)DisposedSong[s]*SongVolume / (float)SongFadeTime;
                    if (DisposedSong[s] == 0)
                    {
                        s.Stop();
                        DisposedSong.Remove(s);
                    }
                }

            }

        }
		public static void Draw()
		{
			FadeAnimationDraw();
			Cursor.Draw();
		}

		public static void Normalize()
		{
			SetMouseSpeed(RootMouseSpeed);
		}

		/*뷰포트 활용을 위해서는 Game1.Update()에서
		GraphicsDevice.Viewport = Game1.graphics.GraphicsDevice.Viewport;
		를 선언해 주셔야 합니다.*/

		/*입력 파트*/

		public static KeyboardState OldkeyboardState;

		public static bool KeyInputOccurs() => Keyboard.GetState() != OldkeyboardState;
		private static bool pressing(Keys k) => Keyboard.GetState().IsKeyDown(k);
        public static bool Pressing(params Keys[] k)
        {
            foreach(Keys key in k)
            {
                if(!pressing(key))
                {
                    return false;
                }
            }
            return true;         
        }
		public static Keys[] GetPressedKeys() =>Keyboard.GetState().GetPressedKeys();

		//유저가 막 key를 눌렀을 때를 체크하는 함수.
		public static bool JustPressed(Keys k) => pressing(k) && !OldkeyboardState.IsKeyDown(k);



        /*사운드 관련 파트*/

        private class BGM
        {
            public static readonly int DefaultFTime = 100;

            private SoundEffectInstance s;
            private int FadeTimer
            {
                get { return FadeTimer; }
                set
                {
                    FadeTimer = value;
                    Equalize();
                }
            }
            public int DivFadeTimer;

            public BGM(string SongName)
            {
                soundEffect = Game1.content.Load<SoundEffect>(SongName);
                s = soundEffect.CreateInstance();
                s.Volume = SEVolume;
                DivFadeTimer = DefaultFTime;
                s.Play();
            }

            public bool Fade_in()
            {
                if (FadeTimer == DivFadeTimer)
                    return true;
                else
                {
                    FadeTimer++;
                    return false;
                }
            }

            public bool Fade_out()
            {
                if (FadeTimer == 0)
                    return true;
                else
                {
                    FadeTimer--;
                    return false;
                }
            }

            public void Equalize()
            {
                s.Volume = (SongVolume * FadeTimer) / DivFadeTimer;
            }

        }

        public static SoundEffect soundEffect;
		public static List<String> SongCatalog = new List<string>();
		public static int SongIndex=-1;
		public static int SongFadeTimer = 0;
		public static int SongFadeTimer_Initial = 0;

		public static float SongVolume=1.0f;
		public static float SEVolume=1.0f;
		public static List<SoundEffectInstance> SoundInstanceList=new List<SoundEffectInstance>();
		public static List<SoundEffectInstance> Song= new List<SoundEffectInstance>();
		public static List<SoundEffectInstance> FadedSong= new List<SoundEffectInstance>();
        public static Dictionary<SoundEffectInstance, int> DisposedSong = new Dictionary<SoundEffectInstance, int>();
        public static readonly int SongFadeTime=100;

        public static string SongName;


		// * Make your own song Name List;
		public enum SongNameList { NOTHING };

		public static void SoundInit()
		{
			// * Add Your Songs in here; In this Manner
			//AddSong("SongName0");
			//AddSong("SongName1");
			//AddSong("SongName2");
			//AddSong("SongName3");

		

		}


		public static void SetSongVolume(float volume)//송의 기본 볼륨을 정한다.
		{
			SongVolume = volume;
			//MediaPlayer.Volume = SongVolume;
			if(Song.Count>0)
				Song[0].Volume = volume;
		}
		public static void FadeSong(float Coefficient)//송에 페이드 효과를 가한다. 기본 볼륨은 변하지 않는다.
		{
			if (Song.Count > 0)
				Song[0].Volume = SongVolume * Coefficient;
		}

		public static void FadeOutSong(int FadeTime)
		{
			if(Song.Count>0)
			{
				FadedSong.Add(Song[0]);
				SongFadeTimer = FadeTime;
				SongFadeTimer_Initial = FadeTime;
				Song.Clear();
			}
		}
		public static void SetSEVolume(float volume)//사운드의 기본 볼륨을 정한다.
		{
			SEVolume = volume;	
		}

	
		public static void PlaySong(string songName)
		{
			if (SongName == songName)
				return;
            DisposeSong();
            SongName = songName;
			soundEffect = Game1.content.Load<SoundEffect>(songName);
			SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
			soundEffectInstance.Volume = SEVolume;
			soundEffectInstance.Play();
			Song.Add(soundEffectInstance);
		}

		public static void PlayLoopedSong(string songName)
		{
			if (SongName == songName)
				return;
            DisposeSong();
			SongName = songName;
			soundEffect = Game1.content.Load<SoundEffect>(songName);
			SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
			soundEffectInstance.Volume = SEVolume;
			soundEffectInstance.Play();
			soundEffectInstance.IsLooped = true;
			Song.Add(soundEffectInstance);
		}
		public static void PlaySong(bool Repeat, float Coefficient)
		{
			//song = Game1.content.Load<Song>(SongCatalog[SongIndex]);
			//MediaPlayer.Volume = SongVolume*Coefficient;
			//MediaPlayer.Play(song);
			//MediaPlayer.IsRepeating = Repeat;
		}

		public static void DisposeSong()
		{
			SongName = "";
			if (Song.Count > 0)
			{
                DisposedSong.Add(Song[0], SongFadeTime);
                Song.Clear();
			}
		}
		public static void PlayFadedSong(int songIndex, bool Repeat,float Coefficient)
		{
		//	SongIndex = songIndex;
		//	song = Game1.content.Load<Song>(SongCatalog[SongIndex]);
			//MediaPlayer.Volume = SongVolume*Coefficient;
			//MediaPlayer.Play(song);
		//	MediaPlayer.IsRepeating = Repeat;
		}
		public static void PlaySE(string SEName)
		{
			soundEffect = Game1.content.Load<SoundEffect>(SEName);
			SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
			soundEffectInstance.Volume = SEVolume;
			soundEffectInstance.Play();
			SoundInstanceList.Add(soundEffectInstance);
		}
		public static void PlayLoopedSE(string SEName)
		{
			soundEffect = Game1.content.Load<SoundEffect>(SEName);
			SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
			soundEffectInstance.Volume = SEVolume;
			soundEffectInstance.IsLooped = true;
			soundEffectInstance.Play();
			SoundInstanceList.Add(soundEffectInstance);
		}

		public static void PlayFadedSE(string SEName,float Coefficient)
		{
			soundEffect = Game1.content.Load<SoundEffect>(SEName);
			SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
			soundEffectInstance.Volume = SEVolume*Coefficient;
			soundEffectInstance.Play();
			SoundInstanceList.Add(soundEffectInstance);
		}
		public static void PlayPitchedSE(string SEName, float Coefficient)
		{
			soundEffect = Game1.content.Load<SoundEffect>(SEName);
			SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
			soundEffectInstance.Pitch = Coefficient;
			soundEffectInstance.Play();
			SoundInstanceList.Add(soundEffectInstance);
		}
		public static void DisposeSE()
		{
			foreach (SoundEffectInstance s in SoundInstanceList)
			{
				s.Stop();
			}
			SoundInstanceList.Clear();
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


        public static void DrawAddon(Camera2D cam, DrawingLayer d, Color color, float opacity, string LayerName)
        {
            DrawingLayer AddonLayer = new DrawingLayer(LayerName, d.GetBound());
            AddonLayer.Draw(cam, color, opacity);
        }







        /*페이드 애니메이션 처리*/
        /* 컬러 키를 활용하는 까닭에 완전 범용성이 높은 코드는 아닙니다. 정말로 드로잉을 따로 처리해야 될 애니메이션 묶음이 있다면 별도로 AnimationList를 선언해 주세요.*/

        private static Dictionary<Color, AnimationList> FadeAnimationList = new Dictionary<Color, AnimationList>();
		private static List<Color> FadeAnimation_ColorException=new List<Color>();

		private static void FadeAnimationInit()
		{
			//제외할 컬러들을 선택한다.
			//FadeAnimation_ColorException.Add(ExampleColor);
			//제외할 컬러들을 선택한다.
			FadeAnimation_ColorException.Add(Color.LightSeaGreen);

			//미리 계수를 조정하는 것도 가능하다.
			FadeAnimationList.Add(Color.White, new AnimationList(1 / 100.0));
            FadeAnimationList.Add(Color.WhiteSmoke, new AnimationList(1/4.0));
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



		public static void FadeAnimationDraw(Color color)//별도로 처리해야 될 색 애니메이션 묶음은 별도로 처리합니다.
		{
			if(FadeAnimationList.ContainsKey(color))
			{
				FadeAnimationList[color].FadeAnimationDraw(color);
			}
		}

		public static void ClearFadeAnimation()
		{
			foreach (KeyValuePair<Color, AnimationList> kv in FadeAnimationList)
			{
				kv.Value.Clear();
			}
		}
		/*뷰포트*/

		public static Viewport OldViewport=new Viewport();

		/*마우스 관련 함수*/
		private static UInt32 SPI_SETMOUSESPEED = 0x0071;
		private static UInt32 SPI_GETMOUSESPEED = 0x0070;
		public static UInt32 RootMouseSpeed;

		[DllImport("User32.dll")]
		static extern Boolean SystemParametersInfo(
			UInt32 uiAction,
			UInt32 uiParam,
			ref UInt32 pvParam,
			UInt32 fWinIni);

		[DllImport("User32.dll")]
		static extern Boolean SystemParametersInfo(
			UInt32 uiAction,
			UInt32 uiParam,
			UInt32 pvParam,
			UInt32 fWinIni);


		public static void SetMouseSpeed(UInt32 speed)//speed는 1~20까지 설정가능.
		{
			SystemParametersInfo(
				SPI_SETMOUSESPEED,
				0,
				speed,
				0);
		}

		public static uint GetMouseSpeed()
		{
			uint speed = 0;
			SystemParametersInfo(
				SPI_GETMOUSESPEED,
				0,
				ref speed,
				0);
			return speed;
		}

		/*기타*/

		//StandardFont.spritefont를 필요로 합니다.
		private static SpriteFont Standardfont;
		private static SpriteFont Temporaryfont;
		public static void DrawString(string s, Vector2 vector2,Color color)
		{
            Game1.spriteBatch.Begin(SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    Standard.MainCamera.get_transformation(Game1.graphics.GraphicsDevice /*Send the variable that has your graphic device here*/));
            Game1.spriteBatch.DrawString(Standardfont, s, vector2, color);
			Game1.spriteBatch.End();
		}

        public static void DrawString(Camera2D cam, string s, Vector2 vector2, Color color)
        {
            Game1.spriteBatch.Begin(SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    cam.get_transformation(Game1.graphics.GraphicsDevice /*Send the variable that has your graphic device here*/));
            Game1.spriteBatch.DrawString(Standardfont, s, vector2, color);
            Game1.spriteBatch.End();
        }

        //특정 드로잉레이어에 결합되는 형식의 스트링을 그린다. 이때 포지션 벡터는 드로잉레이어의 위치를 기준으로 잡으면 된다.
        public static void DrawString(string s, DrawingLayer d, Vector2 vector2,Color color)
		{
            Game1.spriteBatch.Begin(SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    Standard.MainCamera.get_transformation(Game1.graphics.GraphicsDevice /*Send the variable that has your graphic device here*/));
            Game1.spriteBatch.DrawString(Standardfont, s, vector2 + new Vector2(d.GetPos().X, d.GetPos().Y), color);
			Game1.spriteBatch.End();
		}

        public static void DrawString(Camera2D cam, string s, DrawingLayer d, Vector2 vector2, Color color)
        {
            Game1.spriteBatch.Begin(SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    cam.get_transformation(Game1.graphics.GraphicsDevice /*Send the variable that has your graphic device here*/));
            Game1.spriteBatch.DrawString(Standardfont, s, vector2 + new Vector2(d.GetPos().X, d.GetPos().Y), color);
            Game1.spriteBatch.End();
        }


        public static void DrawString(string FontName,string s, Vector2 vector2, Color color)
		{
			Temporaryfont = Game1.content.Load<SpriteFont>(FontName);
            Game1.spriteBatch.Begin(SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    Standard.MainCamera.get_transformation(Game1.graphics.GraphicsDevice /*Send the variable that has your graphic device here*/));
            Game1.spriteBatch.DrawString(Temporaryfont, s, vector2, color);
			Game1.spriteBatch.End();
		}

        public static void DrawString(Camera2D cam, string FontName, string s, Vector2 vector2, Color color)
        {
            Temporaryfont = Game1.content.Load<SpriteFont>(FontName);
            Game1.spriteBatch.Begin(SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    cam.get_transformation(Game1.graphics.GraphicsDevice /*Send the variable that has your graphic device here*/));
            Game1.spriteBatch.DrawString(Temporaryfont, s, vector2, color);
            Game1.spriteBatch.End();
        }

        public static void DrawString(string FontName, string s, DrawingLayer d, Vector2 vector2, Color color)
		{
			Temporaryfont = Game1.content.Load<SpriteFont>(FontName);
            Game1.spriteBatch.Begin(SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    Standard.MainCamera.get_transformation(Game1.graphics.GraphicsDevice /*Send the variable that has your graphic device here*/));
            Game1.spriteBatch.DrawString(Temporaryfont, s, vector2 + new Vector2(d.GetPos().X, d.GetPos().Y), color);
			Game1.spriteBatch.End();
		}

        public static void DrawString(Camera2D cam, string FontName, string s, DrawingLayer d, Vector2 vector2, Color color)
        {
            Temporaryfont = Game1.content.Load<SpriteFont>(FontName);
            Game1.spriteBatch.Begin(SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    cam.get_transformation(Game1.graphics.GraphicsDevice /*Send the variable that has your graphic device here*/));
            Game1.spriteBatch.DrawString(Temporaryfont, s, vector2 + new Vector2(d.GetPos().X, d.GetPos().Y), color);
            Game1.spriteBatch.End();
        }

        public static Vector2 GetStringSize(string s)
		{
			return Standardfont.MeasureString(s);
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




        public static Camera2D MainCamera = new Camera2D();

        public static void ViewportSwapDraw(Viewport v, Action s)
        {
            Viewport Temp = Game1.graphics.GraphicsDevice.Viewport;
            Game1.graphics.GraphicsDevice.Viewport = v;
            s();
            Game1.graphics.GraphicsDevice.Viewport = Temp;
        }


        public static string RandomString(params string[] strings)
        {
            double r = Random();
            double m = 1.0 / strings.Length;
            for(int i=0;i<strings.Length;i++)
            {
                if (m * i <= r && m * (i+1) > r)
                {
                    return strings[i];
                }
            }
            return "";

        }

        private static System.Timers.Timer StandardTimer = new System.Timers.Timer(100);

        public static void Start()
        {
            StandardTimer.Start();
        }
        public static void AttachTickEvent(ElapsedEventHandler e)
        {
            StandardTimer.Elapsed += e;
        }
        public static void DetachTickEvent(ElapsedEventHandler e)
        {
            StandardTimer.Elapsed -= e;
        }
    }




	/*게임 전체를 총괄하는 정보를 저장하는 클래스*/

	public class MasterInfo
	{
		public static readonly Rectangle PreferredScreen = new Rectangle(0, 0, 1280, 720);
		public static Rectangle FullScreen = new Rectangle(0, 0, 1920, 1080);
		public MasterInfo()
		{

		}
		public static void SetFullScreen(double ratio)
		{
			FullScreen = new Rectangle(0, 0, (int)(1920 * ratio), (int)(1080 * ratio));
		}
	
	
	}

	


	/* 마우스 커서를 담당하는 함수. 기본적으로 스탠다드 내에서 구현이 되어 있으므로 아래 지시를 따를 필요는 없다.
	 * "Cursor"텍스쳐를 지정하고, cursor 개체를 LoadContent()에서 생성하고, OldStateUpdate()를 업데이트 문 안에 집어넣으면 끝.
	 * Just Click~ 형태의 부울 함수는 Mouse OldState를 활용하므로, 이러한 마우스 클릭 처리 문은 OldStateUpdate() 앞쪽에 자리해야 한다.
	 * Game1.cs에서 public static Cursor cursor;를 선언하십시오.
	 * Game1.cs의 LoadContent문에 cursor= new Cursor();를 실행하십시오.
	 * Game1.cs의 Update문에 cursor.OldStateUpdate();를 추가하십시오.
	 * Game1.cs의 Draw문에 cursor.Draw()를 추가하십시오.
	 * */

	public static class Cursor
	{
		public static readonly int MouseSize = 20;
		private static DrawingLayer mouseLayer = new DrawingLayer("EmptySpace", new Rectangle(400, 400, MouseSize, MouseSize));
		private static Vector2 MouseLeftOverPosition =new Vector2(0,0);
		private static MouseState OldMouseState =Mouse.GetState();
		private static DrawingLayer DraggingLayer = new DrawingLayer("WhiteSpace",new Rectangle(0,0,0,0));
		public static float Sensitivity =1.0f;
		
		public static void SetSprite(string s)
		{
			mouseLayer.SetSprite(s);
		}
		public static Point GetPos()
		{
			return mouseLayer.GetPos();
		}


		public static void SetPos(int x, int y)
		{
			mouseLayer.SetPos(x, y);
		}

		public static void OldStateUpdate()//클릭 처리 마지막에 행사되어야 OldMouseState가 보존된다.
		{
	
			OldMouseState = Mouse.GetState();
			mouseLayer.SetPos(new Point(OldMouseState.X  - Game1.graphics.GraphicsDevice.Viewport.X / 2, OldMouseState.Y - Game1.graphics.GraphicsDevice.Viewport.Y / 2));
			/*
			if(!Game1.GameExit)
			Standard.SetMouseSpeed((UInt32)(Sensitivity*10));
			if (Sensitivity < 0.3f)
				Sensitivity = 0.3f;
			Vector2 RealMousePos = new Vector2(OldMouseState.Position.X + MouseLeftOverPosition.X, OldMouseState.Position.Y + MouseLeftOverPosition.Y);
			float X = (Mouse.GetState().Position.X - RealMousePos.X) * Sensitivity - (Game1.graphics.GraphicsDevice.Viewport.X - Standard.OldViewport.X) / 2;
			float Y = (Mouse.GetState().Position.Y - RealMousePos.Y) * Sensitivity - (Game1.graphics.GraphicsDevice.Viewport.Y - Standard.OldViewport.Y) / 2;
			//기본적으로 (마우스 변위)*(마우스 감도)=(커서 변위)가 됩니다. 여기에서 만약 뷰포트가 역동적으로 변화할 경우를 대비하여 뷰포트 변위/2를 뺍니다. 
			//뷰포트 변위가 추가되는 이유는 커서 이동을 자연스럽게 하기 위함이니 다른 세팅을 위해서는 이를 제거하셔도 좋습니다.
			mouseLayer.SetPos(PointMethod.Add(mouseLayer.GetPos(), new Point((int)X, (int)Y)));
			mouseLayer.SetPos(mouseLayer.GetPos().X, mouseLayer.GetPos().Y);
			MouseLeftOverPosition = new Vector2(X - (int)X, Y - (int)Y);
			if (Standard.FrameTimer % 3 == 0)
				Mouse.SetPosition(400, 400);
			OldMouseState = Mouse.GetState();*/
		}
		public static void Draw()
		{
			mouseLayer.Draw();
		}


		/*상호작용 처리*/

		public static bool IsOn(Rectangle r)
		{
			return r.Contains(GetPos());
		}
		public static bool IsOn(IGraphicLayer g)
		{
			return g.GetBound().Contains(GetPos());
		}
		public static bool IsOn(DrawingLayer d)
		{
			return d.GetBound().Contains(GetPos());
		}
		public static bool IsOn(StringLayer s)
		{
			return s.GetBound().Contains(GetPos());
		}
		// 반드시 MouseUpdate()이전에 쓰여야 함. 그래야 올드스테이트와 현재스테이트가 구분이 된다.
		// 현재는 스탠다드 내부에서 자동으로 업데이트 되므로 신경쓸 필요는 없음.
		public static bool JustdidLeftClick()//플레이어가 막 레프트클릭을 했는지 확인하는 부울함수. 스태틱함수. 
		{
			return (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed //레프트버튼이 클릭되었고
&& OldMouseState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)//올드마우스스테이트는 클릭이 안되어있었어야해. 혹은..
|| GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X);///게임패드의 경우는 X를 누른거야.
		}
        public static bool JustDidScrollButton()
        {
            return (Mouse.GetState().MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed //레프트버튼이 클릭되었고
&& OldMouseState.MiddleButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed);///게임패드의 경우는 X를 누른거야.
        }
        public static bool JustdidLeftClick(IGraphicLayer g)
		{
			return JustdidLeftClick() && IsOn(g);
		}

		public static bool JustdidLeftClick(DrawingLayer s)
		{
			return JustdidLeftClick() && IsOn(s);
		}

		public static bool JustdidLeftClick(StringLayer s)
		{
			return JustdidLeftClick() && IsOn(s);
		}
		public static bool JustdidRightClick()
		{
			return (Mouse.GetState().RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
				&& OldMouseState.RightButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				|| GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A);
		}

		public static bool JustdidRightClick(DrawingLayer s)
		{
			return JustdidRightClick() && s.GetBound().Contains(GetPos());
		}


		public static bool IsRightClickingNow()//현재 RightClick중이면 참을 반환한다.
		{
			return (Mouse.GetState().RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);
		}

		public static bool IsLeftClickingNow()//현재 RightClick중이면 참을 반환한다.
		{
			return (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);
		}

		public static bool IsDragging(DrawingLayer s)
		{
			if (JustdidLeftClick(s))
			{
				DraggingLayer = s;
			}

			if(DraggingLayer.Equals(s))
			{
				if (!IsLeftClickingNow())
					DraggingLayer = new DrawingLayer("WhiteSpace", new Rectangle(0, 0, 0, 0));

			}
			if (DraggingLayer == s)
				return true;
			else
				return false;
		}

        public static bool ScrollValueChanged()
        {
            return OldMouseState.ScrollWheelValue != Mouse.GetState().ScrollWheelValue;
        }



    }

    /* 간편한 지역 콘텐트매니저를 활용할 수 있게 하는 클래스. 스탠다드 내에서 구현이 되어 있으므로 아래 지시를 따를 필요는 없다.
 * LocalizedContentManager.cs 파일을 생성하여 아래 코드로 덮어씌운다.
 * 이후 Game1.cs에 LocalizedContentaManager를 추가한다.
		public static LocalizedContentManager content;
 * LoadContent()에 아래 줄 추가
 * (이걸 빼먹을 경우 NullReferenceException 버그가 발생한다.)
 		Game1.content = new LocalizedContentManager(base.Content.ServiceProvider, base.Content.RootDirectory);
 * 로컬 콘텐트 매니저를 사용하는 클래스의 인스턴시에이션은 로드콘텐트 이후에 일어나야 한다.
 * */

    // Token: 0x02000003 RID: 3
    public class LocalizedContentManager : ContentManager
	{
		// Token: 0x06000006 RID: 6 RVA: 0x0000219D File Offset: 0x0000039D
		public LocalizedContentManager(IServiceProvider serviceProvider, string rootDirectory, CultureInfo currentCulture, string languageCodeOverride) : base(serviceProvider, rootDirectory)
		{
			this.CurrentCulture = currentCulture;
			this.LanguageCodeOverride = languageCodeOverride;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021B6 File Offset: 0x000003B6
		public LocalizedContentManager(IServiceProvider serviceProvider, string rootDirectory) : this(serviceProvider, rootDirectory, Thread.CurrentThread.CurrentCulture, null)
		{
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021CC File Offset: 0x000003CC
		public override T Load<T>(string assetName)
		{
			string localizedAssetName = assetName + "." + this.languageCode();
			if (this.assetExists(localizedAssetName))
			{
				return base.Load<T>(localizedAssetName);
			}
			return base.Load<T>(assetName);
		
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002203 File Offset: 0x00000403
		private string languageCode()
		{
			if (this.LanguageCodeOverride != null)
			{
				return this.LanguageCodeOverride;
			}
			return this.CurrentCulture.TwoLetterISOLanguageName;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000221F File Offset: 0x0000041F
		public bool assetExists(string assetName)
		{
			return File.Exists(Path.Combine(base.RootDirectory, assetName + ".xnb"));
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000223C File Offset: 0x0000043C
		public string LoadString(string path, params object[] substitutions)
		{
			string assetName;
			string key;
			this.parseStringPath(path, out assetName, out key);
			Dictionary<string, string> strings = this.Load<Dictionary<string, string>>(assetName);
			if (!strings.ContainsKey(key))
			{
				strings = base.Load<Dictionary<string, string>>(assetName);
			}
			return string.Format(strings[key], substitutions);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000227C File Offset: 0x0000047C
		private void parseStringPath(string path, out string assetName, out string key)
		{
			int i = path.IndexOf(':');
			if (i == -1)
			{
				throw new ContentLoadException("Unable to parse string path: " + path);
			}
			assetName = path.Substring(0, i);
			key = path.Substring(i + 1, path.Length - i - 1);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000022C6 File Offset: 0x000004C6
		public LocalizedContentManager CreateTemporary()
		{
			return new LocalizedContentManager(base.ServiceProvider, base.RootDirectory, this.CurrentCulture, this.LanguageCodeOverride);
		}

		// Token: 0x04000006 RID: 6
		public CultureInfo CurrentCulture;

		// Token: 0x04000007 RID: 7
		public string LanguageCodeOverride;
	}

	public static class Method2D
	{
		/*2D 관련 미구현 함수들 구현*/

		//포인트 덧셈
		public static Point Add(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}

		public static Point Deduct(Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
		}

		public static Point Multiply(Point a, double k)
		{
			return new Point((int)(a.X * k), (int)(a.Y * k));
		}
		//포인트 간 거리 측정
		public static int Distance(Point a, Point b)
		{
			double x = a.X - b.X;
			double y = a.Y - b.Y;
			return (int)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}
		//포인트의 절대값 반환
		public static int Abs(Point a)
		{
			return Distance(a, new Point(0, 0));
		}
		//포인트 a,b를 r:1-r로 내분하는 점 반환(r=0~1)
		public static Point DivPoint(Point a, Point b, double r)
		{
			double x = a.X * r + b.X * (1 - r);
			double y = a.Y * r + b.Y * (1 - r);
			return new Point((int)x, (int)y);
		}

		public static Vector2 PointToVector2(Point a)
		{
			return new Vector2(a.X, a.Y);
		}

		public static Point Vector2ToPoint(Vector2 v)
		{
			return new Point((int)(v.X), (int)(v.Y));
		}

		public static Rectangle RectangleMoveTo(Rectangle Rec, Point Des, double speed)
		{
			double Dx = (Des.X - Rec.X);
			double Dy = (Des.Y - Rec.Y);
			double N = Math.Sqrt(Math.Pow(Dx, 2) + Math.Pow(Dy, 2));//두 물체 사이의 거리이자 노말벡터인자.
			if (N < speed)//거리가 스피드보다 가까우면 도착.
			{
				Rec.Location=new Point(Des.X, Des.Y);
				return Rec;
			}

			int X_Displacement = (int)(Dx * speed / N);
			int Y_Displacement = (int)(Dy * speed / N);

			Rec.Location=new Point(Rec.X + X_Displacement, Rec.Y + Y_Displacement);
			return Rec;
		}


	
	}
}
