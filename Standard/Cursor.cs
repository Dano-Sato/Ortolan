using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* 마우스 커서를 담당하는 함수.
 * "Cursor"텍스쳐를 지정하고, cursor 개체를 LoadContent()에서 생성하고, OldStateUpdate()를 업데이트 문 안에 집어넣으면 끝.
 * Just Click~ 형태의 부울 함수는 Mouse OldState를 활용하므로, 이러한 마우스 클릭 처리 문은 OldStateUpdate() 앞쪽에 자리해야 한다.
 * Game1.cs에서 public static Cursor cursor;를 선언하십시오.
 * Game1.cs의 LoadContent문에 cursor= new Cursor();를 실행하십시오.
 * Game1.cs의 Update문에 cursor.OldStateUpdate();를 추가하십시오.
 * Game1.cs의 Draw문에 cursor.Draw()를 추가하십시오.
 * */


namespace TestSheet
{
	public class Cursor
	{
		public static readonly int MouseSize = 20;
		public static Texture2D TestTexture = Game1.content.Load<Texture2D>("Cursor");
		private Point mousePos = new Point();
		private MouseState OldMouseState;


		public void SetSprite(string s)
		{
			TestTexture = Game1.content.Load<Texture2D>(s);
		}

		public Point getPos()
		{
			return mousePos;
		}

		public void OldStateUpdate()//클릭 처리 마지막에 행사되어야 OldMouseState가 보존된다.
		{
			OldMouseState = Mouse.GetState();
			mousePos = new Point(OldMouseState.X, OldMouseState.Y);
		}
		public void Draw()
		{
			Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
			Game1.spriteBatch.Draw(TestTexture, new Rectangle(mousePos.X, mousePos.Y, MouseSize, MouseSize), Color.White);
			Game1.spriteBatch.End();
		}
		// 반드시 MouseUpdate()이전에 쓰여야 함. 그래야 올드스테이트와 현재스테이트가 구분이 된다.
		public bool didPlayerJustLeftClick()//플레이어가 막 레프트클릭을 했는지 확인하는 부울함수. 스태틱함수. 
		{
			return (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed //레프트버튼이 클릭되었고
&& OldMouseState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)//올드마우스스테이트는 클릭이 안되어있었어야해. 혹은..
|| GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X);///게임패드의 경우는 X를 누른거야.
		}

		public bool didPlayerJustRightClick()
		{
			return (Mouse.GetState().RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
				&& OldMouseState.RightButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				|| GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A);
		}

		public bool IsPlayerRightClickingNow()//현재 RightClick중이면 참을 반환한다.
		{
			return (Mouse.GetState().RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);
		}

		public bool IsPlayerLeftClickingNow()//현재 RightClick중이면 참을 반환한다.
		{
			return (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);
		}



	}
}
