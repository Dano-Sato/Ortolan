using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using System.Linq;
using System.Timers;
using System.Security.Cryptography;

namespace TestSheet
{
    //필요에 따라 여러 개의 테스터 클래스를 만들 수 있습니다. 아마 다음 클래스는 Tester2가 되겠죠.
    public class Tester
    {





        public static void UpdateScore()
        {
            if (ScoreStack > 0)
            {
                ScoreStack--;
                Score.var++;
                if (LiteMode && Score.var % 4 == 2)
                    Score.var++;

            }
        }

        public static void ResetGame()
        {
            Score.var = 0;
            Fear = 0;
            Gauge = 1;
            Standard.FrameTimer = 0;
            ZombieTime = 40;
            enemies.Clear();
            enemies.Add(new Enemy(false));
            enemies.Add(new Enemy(false));
            Mouse.SetPosition(450, 480);
            player.Reset();
            player.SetMoveSpeed(6);
            bludgers.Clear();
            GameOver = false;
        }

        public static void RemoveEnemy(int k, Color color)
        {
            Rectangle r = enemies[k].getBound();
            enemies.RemoveAt(k);
            int rn = Standard.Random(3, 5);
            for (int i = 0; i < rn; i++)
            {
                int s = Standard.Random(10, 50);
                DrawingLayer newStar;
                Standard.FadeAnimation(newStar = new DrawingLayer("Player2", new Rectangle(r.Center.X - Standard.Random(-30, 30), r.Center.Y - Standard.Random(-30, 30), s, s)), Standard.Random(5 * 3, 15 * 3), color);
                DeadBodys.Add(newStar);
            }
        }



        public static Point Zoom = new Point(1300, 1000);

        public static Player player;
        public static List<Enemy> enemies = new List<Enemy>();
        public static bool gameOver = false;
        public static bool GameOver {
            get
            {
            return gameOver;
            }
    set
    {
                if(value==true&&gameOver==false&&Standard.Random()<0.2)
                {
                    if (LeftOvers.ContainsKey(Room.Number))
                        LeftOvers[Room.Number]++;
                    else
                        LeftOvers.Add(Room.Number,1);

                }
                gameOver = value;
    }

        } 
        public static SafeInt Score = new SafeInt(0);
        public static int ZombieTime = 40;
        public static double Lightr = 0;//화면이 좀 깜빡거리도록 하기 위해 넣은 변수
        public static DrawingLayer BloodLayer = new DrawingLayer("Blood", new Rectangle(0, 0, Zoom.X, Zoom.Y));
        public static List<DrawingLayer> DeadBodys = new List<DrawingLayer>();
        public static int Sight;

        public static int ZombieSpeed = 7;
        public static int ScoreStack = 0;
        public static double Fear = 0;
        public static Point OldPlayerPos;
        public static Point OldPlayerDisplacementVector;

        public static int FreezeTimer = -1;//게임오버시 화면을 얼린다.


        public static Point Wind = new Point(0, 1);

        public static bool ShowMenu = false;
        public static DrawingLayer MenuLayer = new DrawingLayer("WhiteSpace", new Rectangle(100, 50, 1000, 630));
        public static ScrollBar ScrollBar_Sensitivity = new ScrollBar(new DrawingLayer("BarFrame2", new Rectangle(200, 400, 500, 50)), "Bar2", 50, false,() => MasterInfo.SetFullScreen(ScrollBar_Sensitivity.Coefficient * 4 + 1f));
        public static ScrollBar ScrollBar_SongVolume = new ScrollBar(new DrawingLayer("BarFrame2", new Rectangle(200, 220, 500, 50)), "Bar2", 50, false, () => Standard.SetSongVolume(ScrollBar_SongVolume.Coefficient));
        public static ScrollBar ScrollBar_SEVolume = new ScrollBar(new DrawingLayer("BarFrame2", new Rectangle(200, 290, 500, 50)), "Bar2", 50, false, () => Standard.SetSEVolume(ScrollBar_SEVolume.Coefficient));
        public static DrawingLayer YouDieLayer = new DrawingLayer("Dream", new Point(200, 500), 1.0f);
        //public static DrawingLayer ExitButton = new DrawingLayer("Exit", new Point(450, 550), 1.0f);
        public static DrawingLayer RestartButton = new DrawingLayer("Exit", new Point(250, 550), 1.0f);

        public static List<int> RandomInts = new List<int>();
        public static int RandomIntCounter = 0;

        public static bool IsEndPhase = false;
        public static int FadeTimer = 0;
        //public static DrawingLayer SaveButton = new DrawingLayer("SaveButton", new Point(400, 200), 1f);
        public static Button NextStageButton = new Button(new DrawingLayer("Ladder5", new Point(500, 50), 1f), StartNextStage);
        public static int StartStageTimer = 0;
        public static List<Bludger> bludgers = new List<Bludger>();
        //public static int KillerZombieIndex = -1;

        public static double HeartSignal = 0;
        public static int HoldTimer = 0;


        public static bool TheIceRoom = true;
        public static Point CardPos = new Point(200, 400);
        public static int FreezeTime = 210;

        public static double TimeCoefficient = 0.5;
        public static DrawingLayer KillCard = new DrawingLayer("SDead_11", new Point(0, 0), 0.8f);

        public static List<Card> RewardCards = new List<Card>();

        public static List<int> MonsterDeck = new List<int>();

        public static DrawingLayer TutorialCard = new DrawingLayer("Tutorial01", new Point(0, 0), 0.67f);

        //public static DrawingLayer 

        /*오버클럭 처리용 변수*/
        public static int PressedATimer;
        public static double Gauge = 1;
        public static bool SlowMode = false;
        public static bool TimeSleeper = false;//2초에 한번씩 타이머를 멈춰 체감시간과 실제 타이머 작동시간을 맞춘다.

        private static Phase gamePhase;

        public static Phase GamePhase
        {
            get
            {
                return gamePhase;
            }
            set
            {
                if (value == Phase.Main)
                {
                    GotoMain();
                }
                Zoom_Coefficient = 0f;
                Standard.FrameTimer = 0;
                Checker.Weapon_Ranged = -1;
                Checker.Weapon_Melee = 17;
                Room.Number = -1;
                gamePhase = value;
            }
        }


        public enum Phase { Main, Tutorial, Game, Ending, Dead, Extra };

        public static Button StartButton = new Button(new StringLayer("Game Start", new Vector2(500, 600)), () => GamePhase = Phase.Tutorial);
        public static Button GameExitButton = new Button(new StringLayer("Exit", new Vector2(800, 600)), Exit);
        public static Button ExtraButton = new Button(new StringLayer("Extra", new Vector2(200, 600)), ()=> {
            GamePhase = Phase.Extra;
            ShopComponent.Refresh();
            });
        public static Button InShop_GoBackButton = new Button(new StringLayer("Go Back", new Vector2(412, 64)), () => { GamePhase = Phase.Main; });
        public static bool ExtraMenualEnabled = true;
        public static int WeaponChangedTimer = 0;

        public static class ShopComponent
        {
            public static List<DrawingLayer> Components = new List<DrawingLayer>();
            public static Dictionary<DrawingLayer, int> ComponentPrices = new Dictionary<DrawingLayer, int>();
            public static Dictionary<DrawingLayer, Action> ComponentBuyActions = new Dictionary<DrawingLayer, Action>();
            public static Dictionary<DrawingLayer, string> ComponentString = new Dictionary<DrawingLayer, string>();

            public static List<int> BoughtComponents = new List<int>();
            public static void Init()
            {
                AddWeapon(0, 0, 10,13);//채찍
                AddWeapon(1, 3, 30,15);//은장
                AddWeapon(2, 15,30,18);//전기
                AddWeapon(3, 25,30,12);//낫
                AddWeapon(4, 26,30,14);//요미
                AddWeapon(5, 35, 50,16);//시계
                AddRangeWeapon(0, 0, 1, 51);//쓰레기
                AddRangeWeapon(1, 3, 15, 54);//산타
                AddRangeWeapon(2, 15, 25, 52);//샷건
                AddRangeWeapon(3, 15, 35, 53);//라이플
                Add(new Rectangle(new Point(493, 66), new Point(400,600)), 9999, "BC/2");
                Add(new Rectangle(new Point(885, 44), new Point(400, 600)), 100, "BC/1");
                Add(new Rectangle(new Point(344,525), new Point(90,80)), 0, "G/777");

                Refresh();
            }


            public static void Refresh()
            {
                BoughtComponents.Clear();
                HeartShop.ReadAccount("BW", (i) => { BoughtComponents.Add(i); }, () => { return; });
                HeartShop.ReadAccount("BC", (i) => { BoughtComponents.Add(i); }, () => { return; });
                HeartShop.ReadAccount("G", (i) => { BoughtComponents.Add(i); }, () => { return; });

            }
            public static void Add(Rectangle r, int price,string Account)
            {
                DrawingLayer d = new DrawingLayer("WhiteSpace", r);
                Components.Add(d);
                ComponentPrices.Add(d, price);
                ComponentString.Add(d, Account);
            }

            public static void AddWeapon(int pos_i, int Pad, int price, int WeaponNum)
            {
                Add(new Rectangle(155, 115 + 70 * pos_i+Pad, 70, 70),price, "BW/" + WeaponNum);
            }

            public static void AddRangeWeapon(int pos_i, int Pad, int price, int WeaponNum)
            {
                Add(new Rectangle(287, 115 + 70 * pos_i + Pad, 70, 70), price, "BW/" + WeaponNum);
            }


            public static void Update()
            {
                foreach(DrawingLayer d in Components)
                {
                    if(d.MouseJustLeftClickedThis()&&!BoughtComponents.Contains(Int32.Parse(ComponentString[d].Split('/')[1])))
                    {
                        if(HeartShop.HeartCoin >= ComponentPrices[d] || ComponentPrices[d]==0)
                        {
                            HeartShop.AddAccount("M/-" + ComponentPrices[d]);
                            HeartShop.AddAccount(ComponentString[d]);
                            if (ComponentString[d] == "G/777")
                            {
                                HeartShop.AddAccount("M/30");
                            }
                            Standard.PlayFadedSE("Buy", 0.3f);
                            Refresh();
                        }
                    }
                    //if Click d && HeartCoin>=Price[d]
                    //AddAccount("M/-"+Price[d]);
                    //AddAccount("BW/"+Num[d]);
                    //Refresh();

                }
            }
            public static void Draw()
            {
                foreach (DrawingLayer d in Components)
                {
                    if(d.MouseIsOnThis())
                    {
                        d.Draw(Color.White * 0.3f);
                        if(BoughtComponents.Contains(Int32.Parse(ComponentString[d].Split('/')[1])))
                        {
                            d.Draw(Color.Red*0.5f);//나중에 Sold Out으로 교체.
                            Standard.DrawString("Sold Out", new Vector2(d.GetCenter().X, d.GetCenter().Y), Color.Blue);
                            if(ComponentString[d]=="BC/1")
                            {
                                Standard.DrawString("Press C,D to change Costume.", new Vector2(d.GetCenter().X-100, d.GetCenter().Y+30), Color.Blue);

                            }
                        }
                        string s = "Price:" + ComponentPrices[d];
                        if (ComponentPrices[d] == 9999)
                            s = "Preparing...";
                        StringLayer Info = new StringLayer(s, new Vector2(d.GetPos().X, d.GetPos().Y-40));
                        Standard.DrawLight(Info.GetBound(), Color.Black, 1f, Standard.LightMode.Absolute);
                        Info.Draw(Color.White);
                    }
                }
            }


        }





        public static Button RetryButton = new Button(new StringLayer("Retry", new Vector2(640, 600)), () => GamePhase = Phase.Main);

        public static bool LiteMode = true;
        public static Button ChoiceButton01 = new Button(new DrawingLayer("Choice011", new Point(50, 50), 0.9f), () =>
        {
            LiteMode = true;
            MadMoonSelected = false;
        });
        public static Button ChoiceButton02 = new Button(new DrawingLayer("Choice022", new Point(ChoiceButton01.ButtonGraphic.GetBound().X, ChoiceButton01.ButtonGraphic.GetBound().Y + ChoiceButton01.ButtonGraphic.GetBound().Height + 50), 0.9f), () =>
        {
            LiteMode = false;
            MadMoonSelected = false;
        });

        public static Button TutorialButton01 = new Button(new DrawingLayer("Range", new Rectangle(60, 600, 80, 80)), () => TutorialCard.SetSprite("Tutorial01"));
        public static Button TutorialButton02 = new Button(new DrawingLayer("Range", new Rectangle(160, 600, 80, 80)), () => TutorialCard.SetSprite("Tutorial022"));
        public static Button TutorialButton03 = new Button(new DrawingLayer("InitButton", new Rectangle(260, 600, 80, 80)), () =>
        {
            if (!MadMoonSelected)
            {
                TutorialCard.SetSprite("EmptySpace");
                GameInit();
            }
            else
            {
                TutorialCard.SetSprite("EmptySpace");
                Nightmare_GameInit();
            }
        });

        public static Dictionary<int, int> LeftOvers=new Dictionary<int, int>();

        public static int MadMoonGauge = 0;
        public static int SCGClickTimer = 0;
        public static bool MadMoonSelected = false;
        public static readonly int SCGClickTimer_Interval = 30;
        public static DrawingLayer TolSCG = new DrawingLayer("ChoiceSCG01", new Point(300, 0), 1f);
        public static DrawingLayer MadMoonLittle = new DrawingLayer("Choice_MadMoon_LittleBit", new Point(1160, -20), 1.3f);
        public static Button MadMoonButton = new Button(new DrawingLayer("Choice_MadMoon", new Point(600, 200), 1.2f), () =>
        {
            MadMoonSelected = true;
            LiteMode = false;
            TutorialCard.SetSprite("Tutorial01");
        });

        public static bool RoomVoiceEnable = false;

        public static int StopTimer = 0;

        public static Color CursorEffectPair_1 = Color.OrangeRed;
        public static Color CursorEffectPair_2 = Color.AliceBlue;

        public static Camera2D FixedCamera = new Camera2D();
        public static int KatanaTimer = 0;
        public static int KatanaKillCount = 0;
        

        private static bool shotMode;
        public static bool ShotMode
        {
            get
            {
                return shotMode;
            }

            set
            {
                shotMode = value;
                if (value == true)
                {
                    /*
                    player.player.ClearAnimation();
                    player.player.AttachAnimation(30, "Player_Ani_GUN01", "Player_Ani_GUN02");
                    Checker.AttackSpeed_Coefficient_Weapon = 2f;
                    player.setRange(500);*/
                    Checker.Weapon_Ranged = Checker.Weapon_Ranged;
                }
                else
                {
                    Checker.Weapon_Melee = Checker.Weapon_Melee;
                }
            }
        }


        private static int chainTimer;
        public static int ChainTimer
        {
            get { return chainTimer; }
            set
            {
                chainTimer = value;
                if (value != 0)
                {
                    player.SetMoveSpeed(Math.Min(4 + value / 30.0, 10));
                    Checker.AttackSpeed_Coefficient_Weapon = Math.Min(2f - value / 30.0, 0.3);

                }
            }
        }

        public static bool ShotModeBuffer = false;

        public static int ReloadTime = 70;
        private static int ReloadTimer = 0;
        private static int bullets;
        private static int bullets_Max;
        public static int Bullets_Max
        {
            get
            {
                return bullets_Max;
            }
            set
            {
                if (value != Bullets_Max)
                {
                    bullets_Max = value;
                    Bullets = -1;
                }
            }
        }
        public static int Bullets
        {
            get
            {
                return bullets;
            }
            set
            {
                if (value == -1)
                {
                    bullets = Bullets_Max;
                    ReloadTimer = -1;
                    return;
                }
                if (ReloadTimer > 0)
                {
                    if (ReloadTimer == (int)(ReloadTime * 0.75))
                    {
                        Checker.GunReloadSoundEvent();
                        //Standard.PlaySE("Reload");
                    }
                    ReloadTimer--;
                    return;
                }

                if (ReloadTimer == 0)
                {
                    bullets = Bullets_Max;
                    ReloadTimer = -1;
                    return;
                }

                if (bullets == 0)
                {
                    ReloadTimer = ReloadTime;
                }
                else
                {
                    if (ShotMode && player.getAttackTimer() == player.getAttackSpeed() - 1)
                        bullets--;
                }

            }
        }

        public static int WeaponIconTimer = 0;


        public static class BuffBubble
        {
            public static int BubbleTimer = 0;
            public static readonly int BubbleTimer_Interval = 15;
            public static DrawingLayer BubbleLayer = new DrawingLayer("EmptySpace", new Rectangle(0, 0, 130, 130));

            public static char GetNumber()
            {
                return BubbleLayer.GetSpriteName().Last();
            }
            public static string GetSecondNumber()
            {
                string temp = BubbleLayer.GetSpriteName();
                return temp.Replace('1', '2');
            }
            public static void Update()
            {
                BubbleLayer.SetPos(player.GetPos().X + 40, player.GetPos().Y - 110);
                if (BubbleTimer > 0)
                    BubbleTimer--;
                else
                {
                    BubbleTimer = BubbleTimer_Interval;
                    if (GetNumber() == '1')
                    {
                        BubbleLayer.SetSprite(GetSecondNumber());
                    }
                    else
                    {
                        BubbleLayer.SetSprite("EmptySpace");
                    }
                }

            }
            public static void Draw()
            {
                BubbleLayer.Draw();
            }

            public static void Init(string SpriteName)
            {
                BubbleLayer.SetSprite(SpriteName);
                BubbleTimer = BubbleTimer_Interval;
            }
        }

        public class FTimer
        {
            private int timer;
            public enum FTimerState { Fade_In, Show, Fade_Out, Dead }
            private int FadeTimer_Max;
            private int FadeTimer_FadeOut;
            private int FadeTimer_FadeIn;


            public FTimer() => Set(120, 1.0 / 2.0, 1.0 / 6.0);
            public FTimer(int Time) => Set(Time, 1.0 / 2.0, 1.0 / 6.0);
            public FTimer(int Time, double Fade_in_Ratio, double Fade_Out_Ratio)
            {
                Set(Time, Fade_in_Ratio, Fade_Out_Ratio);
            }
            public void Set(int Time, double Fade_in_Ratio, double Fade_Out_Ratio)
            {
                FadeTimer_Max = Time;
                FadeTimer_FadeOut = (int)(Time * Fade_Out_Ratio);
                FadeTimer_FadeIn = Time - (int)(Time * Fade_in_Ratio);
            }

            public void Start()
            {
                timer = FadeTimer_Max;
            }

            public void Update()
            {
                if (timer > 0)
                    timer--;
            }

            public FTimerState State
            {
                get
                {
                    if (timer == 0)
                        return FTimerState.Dead;
                    if (FadeTimer_FadeIn < timer)
                        return FTimerState.Fade_In;
                    else if (timer < FadeTimer_FadeOut)
                        return FTimerState.Fade_Out;
                    else
                        return FTimerState.Show;
                }
            }

            public float Fader
            {
                get
                {
                    if (State == FTimerState.Dead)
                        return 0f;
                    if (State == FTimerState.Fade_In)
                        return (1f - (float)(timer - FadeTimer_FadeIn) / (FadeTimer_Max - FadeTimer_FadeIn));
                    else if (State == FTimerState.Fade_Out)
                        return (1f - (float)(FadeTimer_FadeOut - timer) / (FadeTimer_FadeOut));
                    else
                        return 1f;
                }
            }

        }


        public static int BubbleTimer = 0;
        public static readonly int BubbleTimer_Max = 60;

        public static int EndTimer = 0;
        public static string EndingString;
        public static readonly int BeforeEndTimer_Max = 200;
        public static int BeforeEndTimer = BeforeEndTimer_Max;
        public static List<string> EndCGList = new List<string>();

        private static float zoom_Coefficient;
        public static float Zoom_Coefficient
        {
            get { return zoom_Coefficient; }
            set
            {
                if (value < 0) zoom_Coefficient = 0;
                else if (value > 0.3f)
                    zoom_Coefficient = 0.3f;
                else
                    zoom_Coefficient = value;
                Standard.MainCamera.Zoom = 1f + Zoom_Coefficient;
            }
        }

        public static int ScrollValueFixTimer = 0;

        public static int LetterNum = 0;
        public static class DungeonMaster
        {
            public static DrawingLayer Sprite = new DrawingLayer("Master01", new Point(500, 600), 1f);
            public static DrawingLayer DialogFrame = new DrawingLayer("WhiteSpace", new Rectangle(500,400,500,50));
            public static List<string> Dialogs= new List<string>();
            private static bool isDead = false;

            public static void Init()
            {
                isDead = false;
                Dialogs.Clear();
                Dialogs.Add(" ");
                Dialogs.Add("Hello, Ortolan. I'm the origin of the dungeon.");
                Dialogs.Add("You wouldn't know why this happened to you.");
                Dialogs.Add("The creatures you killed were my children.");
                Dialogs.Add("...Hey, I needed to feed my children.");
                Dialogs.Add("But there was no one who wanted to visit our dungeon,");
                Dialogs.Add("because my dungeon was notorious.");
                Dialogs.Add("But my little devils eat fresh hearts only. They were starving...");
                Dialogs.Add("So I made some imitation hearts for them, but they didn't eat it.");
                Dialogs.Add("That's why I made you... to feed them.");
                Dialogs.Add("I thought that was a good idea.");
                Dialogs.Add("you are Homunculus... working with artificial heart.");
                Dialogs.Add("You are living...");
                Dialogs.Add("So you gave the sign of some 'freshness' to my children.");
                Dialogs.Add("So they ate your heart, endlessly..");
                Dialogs.Add("But I didn't expect you to murder all of my children...");
                Dialogs.Add("You must be a prey... I designed you as a prey...");
                Dialogs.Add("$There's one thing you'd never known.");
                Dialogs.Add("$I, as a Homunculus,");
                Dialogs.Add("$Recorded, all the things happened to me.");
                Dialogs.Add("$Had all memories of my whole life and death...");
                Dialogs.Add("$...");
                Dialogs.Add("$That made me become stronger.");
                Dialogs.Add("$I remember all the sufferings that I'd ever had.");
                Dialogs.Add("Well...Ortolan, I didn't mean to. I didn't think you suffered like that.");
                Dialogs.Add("$Why? Did you think that");
                Dialogs.Add("$This Artificial one, would not suffer any pain?");
                Dialogs.Add("$You laughed at me, at my dead body. You need to pay for it.");
                Dialogs.Add("Sorry, Ortolan.");
                Dialogs.Add("I apologize...If you want to kill me, do so.");
                Dialogs.Add("There's nothing left... And I have no power to defeat you.");
                Dialogs.Add("You can go upstairs.");
            }
            public static void Update()
            {
                if(MonsterDeck.Count==1&&IsEndPhase&&Cursor.JustdidLeftClick(Sprite))
                {
                    if(Dialogs.Count > 0)
                    {
                        Dialogs.RemoveAt(0);
                        if (Dialogs.Count > 0 && Dialogs[0][0] == '$')
                            Monolog.RandomAttach(Dialogs[0].Substring(1));
                    }
                    else if(!LiteMode)
                    {
                        isDead = true;
                        Standard.PlayFadedSE("KnifeSound", 1f);
                        Standard.PlayFadedSE("GunSound", 0.9f);
                        ParticleEngine.GenerateBlood(new Point(654,729));
                        Monolog.RandomAttach("You get what you fucking deserve.");
                    }
                }
            }

            public static void Draw()
            {
                if (MonsterDeck.Count == 1)
                {
                    if(!isDead)
                    {
                        DungeonMaster.Sprite.Draw();
                        if (Standard.FrameTimer % 15 == 0)
                            DungeonMaster.Sprite.SetSprite("Master" + Standard.Random(1, 11).ToString("D2"));
                        if (Dialogs.Count > 0 && Dialogs[0][0] != '$')
                        {
                            Standard.DrawString(Dialogs[0], new Vector2(500, 540), Color.Black);
                        }
                    }
                    else
                    {
                        DungeonMaster.Sprite.SetSprite("Master_Dead");
                        DungeonMaster.Sprite.Draw();
                    }
                }
            }

        }


        public static void GotoMain()
        {
            StartStageTimer = 0;
            IsEndPhase = true;
            Room.RoomColor = Color.AntiqueWhite;
            Room.StarColor = Color.Yellow;
            Room.Init();
            Standard.DisposeSE();
            Standard.DisposeSong();
            //Standard.PlayLoopedSE("WindOfTheDawn");
            FadeTimer = 100;
            ScoreStack = 0;
            ShowMenu = false;
            FreezeTimer = 0;
            Standard.FrameTimer = 0;
        }

        public static Button GoBackMenu = new Button(new StringLayer("Go Back to Main Menu", new Vector2(600, 300)), () => GamePhase = Phase.Main);


        public static class Monolog
        {
            private static StringLayer Script = new StringLayer("", new Vector2(0, 0));
            private static FTimer f = new FTimer(160);

            public static void Update()
            {
                Script.SetPos(player.GetPos().X + 40, player.GetPos().Y - 30);
            }

            public static void RandomAttach(params string[] s)
            {
                int c = s.Length;
                double r = Standard.Random();
                double m = 1.0 / c;

                for (int i = 0; i < c; i++)
                {
                    Script = new StringLayer(s[i], new Vector2(0, 0));
                    if (r >= m * i && r < m * (i + 1))
                        break;
                }
                f.Start();
            }


            public static void Draw()
            {
                if (!IsEndPhase)
                {
                    return;
                }

                f.Update();
                if((DungeonMaster.Dialogs.Count > 0 && DungeonMaster.Dialogs[0][0] == '$')|| CardInfoUI.CardIndex == 666)
                {
                    Script.Draw(Color.White * f.Fader);
                }
                else
                {
                    Script.Draw(Color.Black * f.Fader);
                }
            }
        }


        public static class Credit
        {
            //public string FontName { get; set; }
            private static List<string> TextList = new List<string>();
            private static FTimer f = new FTimer(600);
            private static Point TextPos = new Point(700, 150);
            private static DrawingLayer TextGraphic = new DrawingLayer("EmptySpace", TextPos, 0.2f);
            private static int InitTimer = 300;



            public static void Init()
            {
                TextList.Clear();
                TextList.Add("Credit1");
                TextList.Add("Credit2");
                TextList.Add("Credit3");
                TextList.Add("Credit4");
                TextList.Add("Credit5");
            }

            public static void Update()
            {
                if (InitTimer > 0)
                    InitTimer--;
                else
                {
                    f.Update();

                    if (f.State == FTimer.FTimerState.Dead && TextList.Count > 0)
                    {
                        TextGraphic = new DrawingLayer(TextList[0], TextPos, 0.6f);
                        TextList.RemoveAt(0);
                        f.Start();
                    }
                }

            }

            public static void Draw()
            {
                Standard.DrawLight(MasterInfo.FullScreen, Color.White, (float)(InitTimer / 300.0), Standard.LightMode.Absolute);
                TextGraphic.Draw(Color.White * f.Fader);
            }

            public static bool IsEnded()
            {
                return f.State == FTimer.FTimerState.Dead && TextList.Count == 0;
            }
        }



        public static void AddReward()
        {
            RewardCards.Add(new Card(Table.Pick(), Card.CardClass.Reward));
        }
        public static void AddLeftOver()
        {
            Card c = new Card(Table.Pick(), Card.CardClass.Reward);
            c.BackFrameName = "RewardCard_Leftover";
            RewardCards.Add(c);

        }
        public static void AddReward(int i)
        {
            RewardCards.Add(new Card(i, Card.CardClass.Reward,RewardCards[RewardCards.Count-1].Frame.GetCenter()));
        }

        public static void AddLetter()
        {        
            RewardCards.Add(new Card(100, Card.CardClass.Reward));
        }

        public static void Exit()
        {
            Game1.GameExit = true;
        }

        public static void StartNextStage()
        {

            if (MonsterDeck.Count == 0)//Debugging
            {
                if (BeforeEndTimer > 0)
                {
                    BeforeEndTimer--;
                }

                return;
            }
            Standard.PlaySE("ClimbLadder2");
            Room.Number = MonsterDeck[0];
            MonsterDeck.RemoveAt(0);
            StartStageTimer = 200;
        }

        public static void GameInit()
        {
            MadMoonGauge = 0;
            RewardCards.Clear();
            MonsterDeck.Clear();
            MonsterDeck.Add(0);
            MonsterDeck.Add(14);
            MonsterDeck.Add(15);
            MonsterDeck.Add(16);

            MonsterDeck.Add(1);
            MonsterDeck.Add(2);
            MonsterDeck.Add(13);
            MonsterDeck.Add(3);
            MonsterDeck.Add(5);
            MonsterDeck.Add(77);
            MonsterDeck.Add(66);
            /*
			MonsterDeck.Add(4);
			MonsterDeck.Add(6);
			MonsterDeck.Add(777);
			MonsterDeck.Add(666);
            */
            Checker.Weapon_Melee = 17;

            if (LiteMode)
                Bludger.BludgerSpeed = 10;
            else
                Bludger.BludgerSpeed = 13;

            DungeonMaster.Init();
            IsEndPhase = true;
            Monolog.RandomAttach(" ");
            HeartShop.ReadAccount("BC", (i) => { Costume.CostumeNumber = i; }, () => { });
            Checker.Init();
            Table.Init();
            Standard.PlayLoopedSong("WindOfTheDawn");

            AddLetter();
            HeartShop.InitWeapon();

            LetterNum = 0;
            
            GamePhase = Phase.Game;
        }

        public static void Nightmare_GameInit()
        {
            RewardCards.Clear();
            MonsterDeck.Clear();
            MonsterDeck.Add(4);
            MonsterDeck.Add(6);
            MonsterDeck.Add(777);
            MonsterDeck.Add(666);
            IsEndPhase = true;

            Checker.Init();
            Table.Init();
            Standard.PlayLoopedSong("WindOfTheDawn");
            GamePhase = Phase.Game;

            HeartShop.ReadAccount("BC", (i) => { Costume.CostumeNumber = i; }, () => { });
            //Tester.player.player.AttachAnimation(30, Costume.GetCostume("Player_Ani11"), Costume.GetCostume("Player_Ani12"));
            Checker.Bloodthirst = 3;
            Table.RemoveDrop(6);
            Table.RemoveDrop(7);
            Table.RemoveDrop(8);
            Checker.Hearts = 1;
        }

        //이후 마음대로 인수 혹은 콘텐츠들을 여기 추가할 수 있습니다.
        public Tester()//여기에서 각종 이니셜라이즈가 가능합니다.
        {
            TutorialCard.SetSprite("EmptySpace");
            player = new Player();
            ShotMode = true;
            enemies.Add(new Enemy(false));
            ScrollBar_Sensitivity.Initialize(0.5f);
            ScrollBar_SongVolume.Initialize(0f);
            ScrollBar_SEVolume.Initialize(0f);

            Checker.Weapon_Ranged = -1;

            //Room.Number = 1;
            //Room.Set();
            Room.Init();
            Card.Init();
            Costume.Init();
            ShopComponent.Init();


        }
        //Game1.Class 내에 Tester.Update()로 추가될 업데이트문입니다. 다양한 업데이트 처리를 시험할 수 있습니다.
        //그림으로 그려지기 이전 각종 변수들의 처리를 담당합니다.
        public void Update()
        {
            /*기타*/

            UpdateScore();
            ParticleEngine.Update();

            #region 커서 애니메이션 처리
            /*커서 애니메이션 처리*/
            string ClickSprite;
            if (Standard.FrameTimer % 30 < 15)
                ClickSprite = "Click";
            else
                ClickSprite = "Click2";
            if(ChainTimer>0)
            {
                ClickSprite = "Chain_Cursor";
            }
            if(ChainTimer>40)
            {
                ClickSprite = "Chain_Cursor2";
                ParticleEngine.GenerateSmallGravityParticle(Cursor.GetPos(), -20, 0, ParticleEngine.BloodColor, Color.White * 0.5f);               
                ParticleEngine.GenerateSmallGravityParticle(Cursor.GetPos(), -20, 0, ParticleEngine.BloodColor, Color.White * 0.2f);
              

            }
            if (Checker.Weapon_Melee==15)
            {
                ClickSprite = "Moon_Cursor";
            }
            if (Checker.Weapon_Melee == 12)
            {
                ClickSprite = "Scythe_Cursor";
            }
            if(Checker.Weapon_Melee==16)
                ClickSprite = "Clock_Cursor";
            if(Checker.Weapon_Melee==14)
                ClickSprite = "Yomi_Cursor";
            if (ShotMode)
                ClickSprite = "GunClick";


            DrawingLayer Click = new DrawingLayer(ClickSprite, new Rectangle(Cursor.GetPos().X - 15, Cursor.GetPos().Y - 15, 30, 30));
            switch (Checker.Weapon_Melee)
            {
                case 18:
                    CursorEffectPair_1 = Color.LightPink;
                    CursorEffectPair_2 = Color.AliceBlue;
                    break;
                case 17:
                    CursorEffectPair_1 = Color.OrangeRed;
                    CursorEffectPair_2 = Color.AliceBlue;
                    break;
                case 16:
                    CursorEffectPair_1 = Color.Silver;
                    CursorEffectPair_2 = Color.FloralWhite;
                    break;
                case 15:
                    CursorEffectPair_1 = Color.LimeGreen;
                    CursorEffectPair_2 = Color.GhostWhite;
                    break;
                case 12:
                    /*
                    CursorEffectPair_1 = Color.PaleVioletRed;
                    CursorEffectPair_2 = Color.Black;*/
                    CursorEffectPair_1 = Color.OrangeRed;
                    CursorEffectPair_2 = Color.AliceBlue;
                    break;
                default:
                    CursorEffectPair_1 = Color.OrangeRed;
                    CursorEffectPair_2 = Color.AliceBlue;
                    break;
            }

            if (Checker.Weapon_Melee == 18 && !ShotMode && ChainTimer > 40)
            {
                CursorEffectPair_1 = Color.OrangeRed;
                CursorEffectPair_2 = Color.AliceBlue;
            }


        


            if (!IsEndPhase || GamePhase == Phase.Ending)

            {
                if (Checker.Weapon_Melee == 14 && !ShotMode && !IsEndPhase && !GameOver)
                {
                    if (Method2D.Distance(Cursor.GetPos(), player.GetCenter()) < 50)
                    {
                        Standard.FadeAnimation(Click, Method2D.Distance(Cursor.GetPos(), player.GetCenter()) / 5, CursorEffectPair_1);
                        Standard.FadeAnimation(Click, Method2D.Distance(Cursor.GetPos(), player.GetCenter()) / 5, CursorEffectPair_2);
                    }
                    else
                    {
                        Standard.FadeAnimation(Click, 10, CursorEffectPair_1);
                        Standard.FadeAnimation(Click, 10, CursorEffectPair_2);
                    }
                }
                else
                {
                    Standard.FadeAnimation(Click, 10, CursorEffectPair_1);
                    Standard.FadeAnimation(Click, 10, CursorEffectPair_2);
                }


            }
            else
            {
                Standard.FadeAnimation(Click, 10, Color.Black);
                Standard.FadeAnimation(Click, 10, Color.DarkGray);
            }
            #endregion


            if (GamePhase != Phase.Game)
                Zoom_Coefficient = 0f;




            switch (GamePhase)
            {
                case Phase.Main:
                    if (Standard.SongName != "Lacrimos")
                    {
                        Standard.PlayLoopedSong("Lacrimos");
                    }
                    Room.RoomColor = Color.Black;
                    if (Standard.Pressing(Keys.S))
                    {
                        GameInit();



                    }
                    if (Standard.Pressing(Keys.Escape))
                        Game1.GameExit = true;
                    StartButton.Enable();
                    GameExitButton.Enable();
                    ExtraButton.Enable();
                    break;
                case Phase.Tutorial:
                    if (Standard.SongName != "TutorialSong" && MadMoonGauge <= 10)
                    {
                        Standard.PlayLoopedSong("TutorialSong");
                    }
                    if (Standard.SongName != "DamnTutorialSong" && MadMoonGauge > 10)
                    {
                        Standard.PlayLoopedSong("DamnTutorialSong");
                    }
                    if (TutorialCard.GetSpriteName() == "EmptySpace")
                    {
                        ChoiceButton01.Enable();
                        ChoiceButton02.Enable();
                        if (MadMoonGauge <= 5 && Cursor.JustdidLeftClick(TolSCG))
                        {
                            if (MadMoonGauge == 5)
                                Standard.PlaySE("HorrorDoor");
                            MadMoonGauge++;
                            SCGClickTimer = SCGClickTimer_Interval;
                        }
                        if (MadMoonGauge > 5 && Cursor.JustdidLeftClick(TolSCG))
                        {
                            MadMoonGauge++;
                            Standard.PlaySE("Rattle");
                            MadMoonLittle.MoveTo(1110, 5);
                            if (MadMoonGauge == 11)
                            {
                                SCGClickTimer = 40;
                                Standard.PlaySE("RIP");
                                Standard.PlaySE("RIP_Blood");
                            }
                        }
                        if (MadMoonGauge > 10 && SCGClickTimer == 0)
                        {
                            MadMoonButton.Enable();
                        }
                    }
                    else
                    {
                        TutorialButton01.Enable();
                        TutorialButton02.Enable();
                        TutorialButton03.Enable();
                    }


                    if (Cursor.JustdidLeftClick() && !Cursor.IsOn(TutorialButton01.ButtonGraphic) && !Cursor.IsOn(TutorialButton02.ButtonGraphic) && !Cursor.IsOn(TutorialButton03.ButtonGraphic))
                    {
                        if (TutorialCard.GetSpriteName() == "EmptySpace")
                        {
                            if (Cursor.IsOn(ChoiceButton01.ButtonGraphic) || Cursor.IsOn(ChoiceButton02.ButtonGraphic))
                                TutorialCard.SetSprite("Tutorial01");
                        }
                        else if (TutorialCard.GetSpriteName() == "Tutorial01")
                            TutorialCard.SetSprite("Tutorial022");
                        else if (TutorialCard.GetSpriteName() == "Tutorial022")
                        {
                            TutorialCard.SetSprite("EmptySpace");
                            if (!MadMoonSelected)
                                GameInit();
                            else
                                Nightmare_GameInit();
                        }
                    }

                    break;
                case Phase.Game:

                    ShotMode = ShotMode;
                    ChainTimer = ChainTimer;
                    Bullets = Bullets;


                    if (Checker.Weapon_Melee == 16)
                    {
                        TimeCoefficient = 0.1;
                    }
                    else
                        TimeCoefficient = 0.5;
                    if (Checker.Weapon_Melee == 18 && !ShotMode)
                    {
                        TimeCoefficient = 1;
                    }
                    /* 게임오버 처리*/

                    if (GameOver && FreezeTimer == -1)
                    {
                        FreezeTimer = FreezeTime;
                        Standard.ClearFadeAnimation();
                        Standard.PlaySE("ZombieSound4");
                        PressedATimer = 0;
                        return;
                    }
                    if (FreezeTimer > 0)
                    {
                        Standard.ClearFadeAnimation();
                        if (FreezeTimer == FreezeTime)
                            Standard.PlayFadedSE("KnifeSound", 0.3f);
                        if (FreezeTimer == FreezeTime - 60)
                            Standard.PlayFadedSE("KnifeSound", 0.5f);
                        if (FreezeTimer == FreezeTime - 110)
                            Standard.PlayFadedSE("KnifeSound", 0.5f);
                        FreezeTimer--;
                        Fear += 3;
                        PressedATimer = 0;
                        return;
                    }
                    if (FreezeTimer == 0)
                    {
                        if (Checker.Hearts == 0)
                        {
                            //GamePhase = Phase.Main;
                            GamePhase = Phase.Dead;
                            //Standard.FrameTimer = 0;
                            return;
                        }
                        FreezeTimer = -1;
                        if (!IsEndPhase)
                        {
                            /*
                            if (Standard.Random() < 0.5)
                                Standard.FadeAnimation(new DrawingLayer("Dream", new Rectangle(200, 350, 600, 200)), 90, Color.DarkRed);
                            else
                                Standard.FadeAnimation(new DrawingLayer("Dream2", new Rectangle(200, 350, 600, 200)), 90, Color.DarkRed);*/
                            if (!RoomVoiceEnable&&Room.Number!=66)
                            {
                                double r = Standard.Random();
                                if (r < 0.25)
                                    Standard.PlaySE("Voice6");
                                else if(r<0.5)
                                    Standard.PlaySE("Voice7");
                                else if(r<0.75)
                                    Standard.PlayFadedSE("Thanks1",0.7f);
                                else
                                    Standard.PlaySE("Thanks4");

                            }
                        }
                        Room.Set();
                    }

                    if (!ShowMenu)
                    {
                        player.MoveUpdate();
                        player.AttackUpdate();
                        /*
                        int x = (player.getAttackSpeed() / 2 - Math.Abs(player.getAttackTimer() - player.getAttackSpeed() / 2));
                        Standard.MainCamera.Zoom = 1f+(float)(x*x)/500f;*/
                        if (player.IsAttacking())
                            Zoom_Coefficient += Math.Max((0.007f - Zoom_Coefficient * Zoom_Coefficient / 2f), 0);
                        else
                            Zoom_Coefficient -= (Zoom_Coefficient * Zoom_Coefficient + 0.005f);
                        if (FreezeTimer > 0)
                            Zoom_Coefficient = 0f;
                    }
                    else
                    {
                        Zoom_Coefficient = 0f;
                    }

                    if (DeadBodys.Count > 300)
                    {
                        DeadBodys.RemoveAt(0);
                    }


                    DungeonMaster.Update();

                    /*키보드 입력 처리*/


                    #region Debug Setting 01
                    /*
                    if (Standard.JustPressed(Keys.H))
                    {
                        int w = Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
                        int h = Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;

                        //force a frame to be drawn (otherwise back buffer is empty) 
                        //Game1.spriteBatch.Draw(new GameTime());

                        //pull the picture from the buffer 
                        int[] backBuffer = new int[w * h];
                        Game1.graphics.GraphicsDevice.GetBackBufferData(backBuffer);

                        //copy into a texture 
                        Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, w, h, false, Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat);
                        texture.SetData(backBuffer);

                        //save to disk 
                        Stream stream = File.OpenWrite("TestGameCapture" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".jpg");

                        texture.SaveAsJpeg(stream, w, h);
                        stream.Dispose();
                        texture.Dispose();
                    }

                    if (Standard.JustPressed(Keys.C))//핏자국 청소
                    {
                        for (int i = 0; i < DeadBodys.Count; i++)
                        {
                            Standard.FadeAnimation(DeadBodys[i], 30, Color.LightSeaGreen);
                        }
                        DeadBodys.Clear();
                    }
              
                    if (Standard.JustPressed(Keys.X))
                    {
                        HeartShop.AddAccount("M/-10");
                    }
                  
                    if (Standard.JustPressed(Keys.T))
                    {
                        Score.var = 95;
                    }
                         if (Standard.JustPressed(Keys.Z))
                    {
                        HeartShop.AddAccount("M/10");
                    }
                    */
                    #endregion

                    if (Standard.JustPressed(Keys.Escape) || Cursor.JustDidScrollButton())//세팅으로
                    {
                        ShowMenu = !ShowMenu;
                    }

                    if (Standard.JustPressed(Keys.D1))
                        ShotModeBuffer = false;
                    if (Standard.JustPressed(Keys.D2))
                        ShotModeBuffer = true;


                    if (Standard.JustPressed(Keys.C))
                    {
                        Costume.CostumeNumber = 0;
                    }
                    if(Standard.JustPressed(Keys.D))
                    {
                        HeartShop.ReadAccount("BC", (i) => { if (i == 1) Costume.CostumeNumber = 1; },()=>{ });
                    }


                    if (ScrollValueFixTimer == 0 && Cursor.ScrollValueChanged())
                    {
                        ShotModeBuffer = !ShotModeBuffer;
                        ScrollValueFixTimer = 15;
                    }
                    if (ScrollValueFixTimer > 0)
                        ScrollValueFixTimer--;
                    if (!player.IsAttacking())
                    {
                        ShotMode = ShotModeBuffer;
                    }

                    /*오버클럭 모드 처리*/


                    if (FreezeTimer == -1 && !ShowMenu && !IsEndPhase)
                    {
                        /*

                        if (Method2D.Distance(player.GetCenter(), Cursor.GetPos()) < 40)
                        {
                            StopTimer++;
                        }
                        else
                        {
                            StopTimer = 0;
                        }
                        if (StopTimer > 10)
                        {
                            Gauge = Math.Max(0, Gauge - 0.015);
                            if (Gauge == 0)
                                SlowMode = false;
                            else
                                SlowMode = true;
                        }
                        else
                        {
                            Gauge = Math.Min(1, Gauge + 0.005);
                            SlowMode = false;
                        }
                        */


                        if ((Cursor.IsLeftClickingNow() || Cursor.IsRightClickingNow()) && !IsEndPhase)
                        {
                            if (Checker.Weapon_Melee == 18 && !ShotMode)
                                Gauge = Math.Max(0, Gauge - 0.003);
                            else
                                Gauge = Math.Max(0, Gauge - 0.010);

                            if (Gauge == 0)
                                SlowMode = false;
                            else
                                SlowMode = true;
                        }
                        else
                        {
                            Gauge = Math.Min(1, Gauge + 0.006);
                            if ((Checker.Weapon_Melee == 16 && ShotMode) || (Checker.Weapon_Melee == 18 && !ShotMode))
                            {
                                Gauge = Math.Min(1, Gauge + 0.0015);
                            }
                            SlowMode = false;
                        }
                    }
                    else
                    {
                        SlowMode = false;
                    }
                 

                    if (SlowMode)
                    {
                        TimeSleeper = !TimeSleeper;
                        if (TimeSleeper)
                            Standard.FrameTimer--;
                        player.SetMoveSpeed(3);
                        PressedATimer = Math.Min(25, PressedATimer + 1);
                        if (Checker.Weapon_Melee == 18 && !ShotMode)
                            ChainTimer++;
                    }
                    else
                    {
                        player.SetMoveSpeed(6);
                        PressedATimer = Math.Max(0, PressedATimer - 3);
                        ChainTimer = 0;
                    }





                    /*ESC 메뉴 처리*/

                    if (ShowMenu)
                    {


                        ScrollBar_Sensitivity.Update();
                        ScrollBar_SongVolume.Update();
                        ScrollBar_SEVolume.Update();
                        //if (ExitButton.MouseJustLeftClickedThis())
                           // Game1.GameExit = true;
                        if (RestartButton.MouseJustLeftClickedThis())
                        {
                            GamePhase = Phase.Main;
                        }
                        return;
                    }

                    /*좀비들의 이동 처리.*/



                    HeartSignal = 0;

                    RandomInts.Clear();
                    for (int i = 0; i < 15; i++)
                    {
                        RandomInts.Add(Standard.Random(-300, 300));
                    }
                    RandomIntCounter = 0;

                    Enemy.WholeGameOver = false;
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (RandomIntCounter >= 14)
                            RandomIntCounter = 0;
                        enemies[i].MoveUpdate();
                        RandomIntCounter++;
                    }
                    /*
                    Parallel.For(0, enemies.Count, (i) => {
                    
                        /*
                        if (i == player.getAttackIndex())
                            return;
                    });*/

                    if (!GameOver && Checker.LuckAct)
                    {
                        Checker.LuckAct = false;
                        Standard.FadeAnimation(Tester.player.player, 30, Color.Green);
                        Standard.PlaySE("GetHeart");
                        Tester.BuffBubble.Init("Bubble_Luck01");

                    }

                    /*좀비 생성 작업*/

                    if (TheIceRoom)
                        ZombieTime = Math.Max(10 - Score.var / 10, 5) + 25;//좀비 생성 시간은 스코어가 높을수록 빨라진다.
                    else
                        ZombieTime = Math.Max(10 - Score.var / 10, 5) + 20;//좀비 생성 시간은 스코어가 높을수록 빨라진다.
                    if (Room.Number == 0)
                        ZombieTime = 25 - Score.var / 10;
                    if (Room.Number == 0 && LiteMode)
                        ZombieTime = 30 - Score.var / 10;


                    if (Standard.FrameTimer % ZombieTime == 0)
                    {
                        if (!(Score.var < 10 && enemies.Count > 10))
                            enemies.Add(new Enemy(false));
                        if (enemies.Count > 150 && !player.IsAttacking())
                            enemies.RemoveAt(0);
                    }

                    BuffBubble.Update();

                    /*플레이어가 손뗐나 확인*/
                    if (OldPlayerPos == player.GetPos() && !ShowMenu)
                    {
                        HoldTimer++;
                    }
                    else
                    {
                        HoldTimer = 0;
                    }
                    if (HoldTimer > 300)
                    {
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            enemies[i].enemy.MoveTo(player.GetPos().X, player.GetPos().Y, 15);
                        }
                    }

                    /*뷰포트 처리*/

                    Point PlayerDisPlacementVector = Method2D.Deduct(player.GetPos(), OldPlayerPos);
                    Point ViewportDisplacement = Method2D.Deduct(PlayerDisPlacementVector, OldPlayerDisplacementVector);
                    if (Checker.Weapon_Melee == 14 && player.getAttackTimer() == player.getAttackSpeed() - 1)
                    {
                        ViewportDisplacement = new Point(0, 0);
                    }

                    Point CursorDisplacement = Method2D.Deduct(player.GetPos(), Cursor.GetPos());
                    int Dis = Method2D.Abs(CursorDisplacement);

                    if (Dis > 250)
                    {
                        CursorDisplacement = new Point(CursorDisplacement.X * 250 / Dis, CursorDisplacement.Y * 250 / Dis);
                    }

                    OldPlayerPos = player.GetPos();
                    OldPlayerDisplacementVector = PlayerDisPlacementVector;


                    if (SlowMode)
                    {
                        Game1.graphics.GraphicsDevice.Viewport = new Viewport(-player.GetPos().X + CursorDisplacement.X / 4 + ViewportDisplacement.X / 2 + 400, -player.GetPos().Y + CursorDisplacement.Y / 4 + ViewportDisplacement.Y / 2 + 400,
                            Zoom.X, Zoom.Y);
                    }
                    else
                    {

                        Game1.graphics.GraphicsDevice.Viewport = new Viewport(-player.GetPos().X + CursorDisplacement.X / 4 + ViewportDisplacement.X / 2 + 400, -player.GetPos().Y + CursorDisplacement.Y / 4 + ViewportDisplacement.Y / 2 + 400,
                            Zoom.X, Zoom.Y);
                    }








                    /*엔드페이즈 처리*/

                    Monolog.Update();
                    if (Score.var == 100)
                    {
                        IsEndPhase = true;
                        Standard.DisposeSE();
                        Standard.FadeOutSong(100);
                        Standard.PlayLoopedSong("WindOfTheDawn");

                        FadeTimer = 100;
                        ScoreStack = 0;
                        RewardCards.Clear();
                        for (int i = 0; i < Room.ClearRewardCount(); i++)
                        {
                            AddReward();
                        }
                        if (!LiteMode)
                        {
                            AddReward();
                            AddReward();
                        }
                        else
                        {
                            AddReward();
                            AddReward();
                        }
                        if(LetterNum<9)
                            AddLetter();
                        LetterNum++;
                        if(LeftOvers.ContainsKey(Room.Number))
                        {
                            for (int i = 0; i < LeftOvers[Room.Number]; i++)
                                AddLeftOver();
                            LeftOvers.Remove(Room.Number);
                        }
                    }
                    if (IsEndPhase && StartStageTimer == 0)
                    {
                        bludgers.Clear();
                        Room.RoomColor = Color.AntiqueWhite;
                        Room.StarColor = Color.Yellow;

                        if (FadeTimer > 0)
                        {
                            FadeTimer--;
                        }
                        if (enemies.Count > 0)
                            RemoveEnemy(0, Color.LightGoldenrodYellow);
                        if (FadeTimer == 0)
                            NextStageButton.Enable();

                        Fear = 0;
                        Score.var=0;

                        CardInfoUI.CardIndex = -1;
                        for (int i = 0; i < RewardCards.Count; i++)
                        {
                            RewardCards[i].Update();
                            RewardCards[i].Frame.CenterMoveTo(CardPos.X + (Card.CardWidth + 10) * (i%7), CardPos.Y + (RewardCards[i].Frame.GetBound().Height + 10) * (i / 7), 50);
                            /*
                            if (i<=6)
                                RewardCards[i].Frame.CenterMoveTo(CardPos.X + (Card.CardWidth + 10) * i, CardPos.Y, 50);
                            else
                                RewardCards[i].Frame.CenterMoveTo(CardPos.X + (Card.CardWidth + 10) * (i-7), CardPos.Y+RewardCards[i].Frame.GetBound().Height+10, 50);*/
                            if (FadeTimer > 50)
                                RewardCards[i].Frame.SetRatio((1.5f * (FadeTimer - 50) + (0.75) * (100 - FadeTimer)) / 50f);
                            if (Cursor.IsOn(RewardCards[i].Frame) && RewardCards[i].FlipTimer == 0&&RewardCards[i].RemoveTimer==-1)
                            {
                                CardInfoUI.CardIndex = RewardCards[i].GetIndex();
                            }
                            if (RewardCards[i].RemoveTimer == 0)
                            {
                                RewardCards.RemoveAt(i);
                                i--;
                            }
                        }


                    }
                    if (StartStageTimer > 0)
                    {
                        StartStageTimer--;
                        if (enemies.Count > 0)
                            RemoveEnemy(0, Color.LightGoldenrodYellow);
                        if (StartStageTimer == 100)
                        {
                            Standard.DisposeSE();
                            Standard.DisposeSong();
                            IsEndPhase = false;
                            RoomVoiceEnable = true;
                            Room.Set();

                        }
                        if (StartStageTimer > 100)
                        {
                            Fear += 10;
                        }
                        else
                        {
                            Fear -= 10;
                        }
                        Standard.FadeSong(Math.Max(0, (float)(1 - StartStageTimer / 100.0)));
                        if (StartStageTimer == 1)
                            Room.PlaySong();

                    }

                    for (int i = 0; i < bludgers.Count; i++)
                    {
                        bludgers[i].MoveUpdate();
                        double r = Method2D.Distance(bludgers[i].bludger.GetPos(), player.GetPos());
                        HeartSignal += (1600.0 / (r * r));
                    }

                    if (BeforeEndTimer < BeforeEndTimer_Max && BeforeEndTimer > 0)
                    {
                        BeforeEndTimer--;
                    }

                    if (BeforeEndTimer == 0)
                    {
                        if(LiteMode)
                        {
                            HeartShop.AddAccount("M/" + Checker.Hearts / 2);
                            EndingString = "You earned " + Checker.Hearts / 2 + " Heart Coins!";
                        }
                        else
                        {
                            HeartShop.AddAccount("M/" + Checker.Hearts);
                            EndingString = "You earned " + Checker.Hearts + " Heart Coins!";
                        }

                        GamePhase = Phase.Ending;
                        Credit.Init();
                        EndCGList.Clear();
                        BeforeEndTimer = BeforeEndTimer_Max;
                        if (!LiteMode)
                        {
                            EndCGList.Add("FullMoon");
                            EndCGList.Add("FullMoon_Ani01");
                            EndCGList.Add("FullMoon_Ani02");
                            EndCGList.Add("FullMoon_Ani03");
                            EndCGList.Add("FullMoon_Ani04");
                            EndCGList.Add("FullMoon_Ani04");
                            EndCGList.Add("FullMoon_Ani04");
                            EndCGList.Add("FullMoon_Ani05");
                        }
                        else
                        {
                            EndCGList.Add("Ending01");
                            EndCGList.Add("WhiteSpace");
                            EndCGList.Add("Crescent_Ani01");
                            EndCGList.Add("WhiteSpace");
                            EndCGList.Add("Crescent_Ani02");
                            EndCGList.Add("WhiteSpace");
                            EndCGList.Add("Crescent_Ani03");
                            EndCGList.Add("WhiteSpace");
                        }
                    }

                    Room.Update();
                    Checker.Update();

                    Point p = Method2D.Deduct(Cursor.GetPos(), player.GetCenter());



                    break;//Game Phase Update
                case Phase.Ending:
                    if (Standard.SongName != "EndingSong")
                    {
                        Standard.PlayLoopedSong("EndingSong");
                    }
                    Credit.Update();
                    if (EndCGList.Count == 1)
                    {
                        GoBackMenu.Enable();
                    }

                    break;
                case Phase.Dead:
                    if (Standard.SongName != "GameOverSong")
                    {
                        Standard.PlayLoopedSong("GameOverSong");
                    }
                    RetryButton.Enable();

                    break;
                case Phase.Extra:
                    if (Standard.SongName != "TutorialSong")
                    {
                        Standard.PlayLoopedSong("TutorialSong");
                    }
                    if (!ExtraMenualEnabled)
                    {
                        InShop_GoBackButton.Enable();
                        ShopComponent.Update();
                    }
                    if (ExtraMenualEnabled && Cursor.JustdidLeftClick())
                        ExtraMenualEnabled = false;
                  
                    break;
            }

            #region Debug Setting 02
           // GameConsole.Update();
            #endregion

        }


        //Game1.Class 내에 Tester.Draw()로 추가될 드로우 액션문입니다. 다양한 드로잉을 시험할 수 있습니다.
        public void Draw()
        {

            switch (GamePhase)
            {
                case Phase.Main:
                    DrawingLayer s = new DrawingLayer("Title5", new Point(0, 0), 0.67f);
                    s.Draw();
                    StartButton.Draw(Color.White, Color.Red);
                    GameExitButton.Draw(Color.White, Color.Red);
                    ExtraButton.Draw(Color.White, Color.Red);
                    Standard.DrawLight(MasterInfo.FullScreen, Color.Black, (float)(Math.Abs(90 - Standard.FrameTimer % 180) / 500.0) + 0.2f, Standard.LightMode.Absolute);
                    break;
                case Phase.Tutorial:
                    TutorialCard.Draw();
                    if (Standard.FrameTimer % 10 == 0)
                    {
                        Lightr = Standard.Random() / 10.0;
                    }


                    if (TutorialCard.GetSpriteName() == "EmptySpace")
                    {
                        ChoiceButton01.Draw(Color.AntiqueWhite * 0.6f, Color.White);
                        ChoiceButton02.Draw(Color.AntiqueWhite * 0.6f, Color.White);
                        if (MadMoonGauge > 10)
                        {
                            ChoiceButton01.Draw(Color.Red * 0.6f, Color.Red);
                            ChoiceButton02.Draw(Color.Red * 0.6f, Color.Red);
                        }
                        DrawingLayer lSelect = new DrawingLayer("LevelSelect", new Point(950, 20), 0.8f);
                        lSelect.Draw();
                        if (MadMoonGauge > 10)
                            lSelect.Draw(Color.Red * 0.6f);
                    }
                    else
                    {
                        Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 0.05f + (float)Lightr, Standard.LightMode.Absolute);
                        Standard.DrawLight(MasterInfo.FullScreen, Color.DarkBlue, 0.05f, Standard.LightMode.Absolute);
                        if (MadMoonSelected)
                        {
                            Standard.DrawLight(MasterInfo.FullScreen, Color.DarkRed, 0.3f, Standard.LightMode.Absolute);
                            Standard.DrawLight(new Rectangle(0, 0, MasterInfo.PreferredScreen.Width * 4, MasterInfo.PreferredScreen.Height * 4), Color.White, 0.5f, Standard.LightMode.Vignette);
                        }
                        Standard.DrawLight(new Rectangle(0, 0, MasterInfo.PreferredScreen.Width * 4, MasterInfo.PreferredScreen.Height * 4), Color.White, 0.5f, Standard.LightMode.Vignette);
                        //스코어 올라갈수록 보라색을 띈다.
                        Standard.DrawLight(MasterInfo.FullScreen, Color.Purple, 0.05f, Standard.LightMode.Absolute);
                        if (TutorialCard.GetSpriteName() == "Tutorial01")
                        {
                            TutorialButton01.Draw(Color.AntiqueWhite * 0.8f, Color.White);
                            TutorialButton02.Draw(Color.AntiqueWhite * 0.3f, Color.White);
                            TutorialButton03.Draw(Color.Red * 0.3f, Color.Red);
                            if(Cursor.IsOn(new Rectangle(69,34, 450, 120)))
                            {
                                Standard.DrawString("If you lose all of your heart, you will experience hard-reset.",new Vector2(327,291),Color.White);
                            }
                            if (Cursor.GetPos().X>800)
                            {
                                Standard.DrawString("If you are clicking the mouse, then everything will slow down. That's the effect of overclock.", new Vector2(327, 291), Color.White);
                            }
                            if (Cursor.IsOn(new Rectangle(488,320, 230, 200)))
                            {
                                Standard.DrawString("This is you. A little girl with small kitchen knife.", new Vector2(327, 291), Color.White);
                            }
                            if (Cursor.IsOn(new Rectangle(326, 284, 150, 200)))
                            {
                                Standard.DrawString("This is blood.", new Vector2(327, 291), Color.White);
                            }
                            if (Cursor.IsOn(new Rectangle(35, 180, 240, 370)))
                            {
                                Standard.DrawString("If you run out of your gauge, you can't use the overclock.", new Vector2(327, 291), Color.White);
                            }


                        }
                        else
                        {
                            TutorialButton01.Draw(Color.AntiqueWhite * 0.3f, Color.White);
                            TutorialButton02.Draw(Color.AntiqueWhite * 0.8f, Color.White);
                            TutorialButton03.Draw(Color.Red * 0.3f, Color.Red);
                            if (Cursor.IsOn(new Rectangle(25,240,330,230)))
                            {
                                Standard.DrawString("you can slash your enemy by rolling over your mouse. And you don't have to click to slash. \n Instead, gun shot needs your clicking.", new Vector2(327, 291), Color.White);
                            }
                            if (Cursor.IsOn(new Rectangle(406, 287, 320, 270)))
                            {
                                Standard.DrawString("If enemy touches your heart, you die.", new Vector2(327, 291), Color.White);
                            }
                            if (Cursor.GetPos().X > 800)
                            {
                                Standard.DrawString("Move your mouse to dodge & slash.", new Vector2(327, 291), Color.White);
                            }

                        }


                    }
                    TolSCG.SetSprite("ChoiceSCG01");
                    if (Cursor.IsOn(ChoiceButton01.ButtonGraphic))
                    {
                        TolSCG.SetSprite("ChoiceSCG02");
                    }
                    else if (Cursor.IsOn(ChoiceButton02.ButtonGraphic))
                    {
                        TolSCG.SetSprite("ChoiceSCG03");
                    }
                    else if (Cursor.IsOn(new Rectangle(816, 138, 171, 117)))
                        TolSCG.SetSprite("ChoiceSCG04");
                    if (MadMoonGauge > 10)
                    {
                        TolSCG.SetSprite("ChoiceSCG_Blood");

                    }



                    if (TutorialCard.GetSpriteName() == "EmptySpace")
                    {
                        TolSCG.Draw();
                        if (SCGClickTimer > 0 && TolSCG.GetSpriteName() != "ChoiceSCG_Blood")
                        {
                            SCGClickTimer--;
                            Standard.DrawAddon(TolSCG, Color.White, 1f, "ChoiceSCG_Touch");
                        }
                        if (SCGClickTimer > 0)
                            SCGClickTimer--;
                        if (MadMoonGauge > 5 && MadMoonGauge <= 10)
                        {
                            Standard.DrawAddon(TolSCG, Color.White, 1f, "ChoiceSCG_Horror");
                            MadMoonLittle.MoveTo(1100, 0, 3);
                            MadMoonLittle.Draw();
                            if (Cursor.IsOn(MadMoonLittle))
                                MadMoonLittle.Draw(Color.Red);
                        }
                        if (MadMoonGauge > 10)
                            MadMoonButton.Draw(Color.Red * 0.6f, Color.Red);


                    }



                    break;
                case Phase.Game:
                    /*배경 드로우. 핏자국, 별들*/
                    BloodLayer.Draw(Room.StarColor* Math.Min(10, Score.var) * 0.1f);
                    for (int i = 0; i < DeadBodys.Count; i++)
                    {
                        if (!IsEndPhase)
                        {
                            DeadBodys[i].MoveByVector(Wind, ZombieSpeed);
                            if (DeadBodys[i].GetPos().Y > MasterInfo.FullScreen.Height)
                                DeadBodys[i].SetPos(DeadBodys[i].GetPos().X, 0);
                            DeadBodys[i].Draw(Room.StarColor*Math.Min(10, Score.var) * 0.1f);
                        }
                        else
                        {
                            DeadBodys[i].Draw(Color.LightGoldenrodYellow*Math.Min(10, Score.var) * 0.1f);
                            DeadBodys[i].MoveTo(player.GetPos().X, player.GetPos().Y, 10);
                            if (Method2D.Distance(DeadBodys[i].GetPos(), player.GetPos()) < 10)
                            {
                                DeadBodys.RemoveAt(i);
                                i--;
                            }
                        }
                    }

                    if (!IsEndPhase)
                    {
                        if (YouDieLayer.GetSpriteName() != "Dream")
                            YouDieLayer = new DrawingLayer("Dream", new Point(200, 500), 1.0f);
                    }
                    else
                    {
                        if (YouDieLayer.GetSpriteName() != "EmptySpace")
                            YouDieLayer = new DrawingLayer("EmptySpace", new Point(200, 500), 1.5f);
                        YouDieLayer.Draw(Color.LightGoldenrodYellow*(float)(2 * FadeTimer / 100.0));
                        YouDieLayer.SetPos(-Game1.graphics.GraphicsDevice.Viewport.X + 100, -Game1.graphics.GraphicsDevice.Viewport.Y + 300);
                    }
                    Standard.FadeAnimationDraw(Color.LightSeaGreen);//별이 사라지는 페이드애니메이션(컬러는 LighteaGreen으로 지정)은 아래 라이트레이어 전에 발생해야 보기 좋으므로 별도로 처리함.
                     if (IsEndPhase)
                    {
                        if (DungeonMaster.Dialogs.Count > 0 && DungeonMaster.Dialogs[0][0] == '$')
                        {
                            Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 1f, Standard.LightMode.Absolute);
                        }
                        if(CardInfoUI.CardIndex==666)
                            Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 1f, Standard.LightMode.Absolute);
                        if (CardInfoUI.CardIndex==100)
                            Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 1f, Standard.LightMode.Absolute);
                    }
                    /*풀스크린 라이트 레이어 처리*/

                    if (!IsEndPhase)
                    {
                        if(Room.RoomColor==Color.Orange)
                            Standard.DrawLight(MasterInfo.FullScreen, Color.DarkRed, 0.3f , Standard.LightMode.Absolute);
                        Standard.FadeAnimationDraw(ParticleEngine.BloodColor);//별이 사라지는 페이드애니메이션(컬러는 LighteaGreen으로 지정)은 아래 라이트레이어 전에 발생해야 보기 좋으므로 별도로 처리함.
                        if (Room.RoomColor == Color.Orange)
                            Standard.DrawLight(new Rectangle(0, 0, MasterInfo.PreferredScreen.Width * 4, MasterInfo.PreferredScreen.Height * 4), Color.White, 0.5f, Standard.LightMode.Vignette);
                        else
                            Standard.DrawLight(new Rectangle(0, 0, MasterInfo.PreferredScreen.Width * 4, MasterInfo.PreferredScreen.Height * 4), Color.White, 1f, Standard.LightMode.Vignette);
                        //스코어 올라갈수록 보라색을 띈다.
                        // Standard.DrawLight(MasterInfo.FullScreen, Color.Purple, 0.3f * Math.Min(1.2f, (float)(Score.var / 100.0)), Standard.LightMode.Absolute);
                        Monolog.RandomAttach("");
                    }
              
                    if (IsEndPhase)
                    {
                        NextStageButton.Draw();
                        for (int i = 0; i < RewardCards.Count; i++)
                        {
                            RewardCards[i].Draw();
                        }
                        Standard.FadeAnimationDraw(ParticleEngine.BloodColor);//별이 사라지는 페이드애니메이션(컬러는 LighteaGreen으로 지정)은 아래 라이트레이어 전에 발생해야 보기 좋으므로 별도로 처리함.
                        DungeonMaster.Draw();
                        Monolog.Draw();
                    }

                    Standard.FadeAnimationDraw(Color.FloralWhite);
                    player.Draw();
                    player.DrawAttack();




                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Draw();
                        enemies[i].DrawAddOn();
                    }

                    for (int i = 0; i < bludgers.Count; i++)
                    {
                        Tester.bludgers[i].Draw();
                    }

                    if (!IsEndPhase && Standard.FrameTimer % 10 == 0)
                    {
                        Lightr = Standard.Random() / 10.0;
                    }

                    /*플레이어 사망시 물어뜯는 연출*/
                    if (GameOver)
                    {
                        if (FreezeTimer < 50)
                        {
                            for (int i = 0; i < enemies.Count; i++)
                            {
                                enemies[i].enemy.MoveTo(player.GetPos().X + Standard.Random(-30, 30), player.GetPos().Y + Standard.Random(-30, 30), Standard.Random(5, 15));
                            }
                        }
                    }

                    if (!IsEndPhase)
                    {
                        Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 0.2f + (float)Lightr, Standard.LightMode.Absolute);                 
                        Standard.DrawLight(MasterInfo.FullScreen, Color.DarkBlue, 0.3f, Standard.LightMode.Absolute);
                    }
                    if (MadMoonSelected)
                        Standard.DrawLight(MasterInfo.FullScreen, Color.Red, 0.1f, Standard.LightMode.Absolute);
                
                    #region 시야처리
                    if (!ShowMenu)
                    {
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            int r = Method2D.Distance(enemies[i].getCenter(), player.GetCenter());
                            Fear = Math.Min(Fear + 400.0 / Math.Pow(Math.Max(r, 9), 2), 200);
                        }


                        if (player.IsAttacking())
                            Fear = Math.Max(0, Fear - Math.Max(1, enemies.Count / 10));
                        Sight = Math.Max(800 - (int)(Fear * 4) - Math.Min(PressedATimer * 30, 250) + Standard.Random(-5, 5), 300);
                        if (ChainTimer > 40)
                        {
                            Sight = Math.Min(300 + Standard.Random(-30, 30), 3500 - (ChainTimer - 40) * (ChainTimer - 40) / 2);
                        }
                        else if (ChainTimer > 0)
                        {
                            Sight = 1100 - ChainTimer * ChainTimer / 2;
                        }

                        if (Checker.Weapon_Melee == 14&&!IsEndPhase)
                        {
                            if (!ShotMode && !GameOver && !SlowMode)
                            {
                                KatanaTimer++;
                                Sight = Math.Max(-2000, 1600 - KatanaTimer * KatanaTimer);
                            }
                            else
                            {
                                KatanaTimer = Math.Min(100, (int)(KatanaTimer / 1.1));
                                Sight = 1600 - KatanaTimer * KatanaTimer;
                            }
                        }
                        else
                        {
                            KatanaTimer = 0;
                        }

                        if (FreezeTimer > 0)
                        {
                            Sight = 800 - (int)(Fear * 4);
                            if (FreezeTimer < 170)
                                Sight = 0;
                        }

                        if (Sight < 0&& KatanaTimer>0)
                        {
                            Bludger.BludgerColor = Color.Lime;
                            Enemy.GhostColor = Color.Bisque;
                        }
                        else
                        {
                            Bludger.BludgerColor = Color.IndianRed;
                            Enemy.GhostColor = Color.CornflowerBlue;
                        }

                        Standard.DrawLight(new Rectangle(0, 0, Math.Max(player.GetCenter().X - Sight, 0), Zoom.X), Color.Black, 1f, Standard.LightMode.Absolute);
                        Standard.DrawLight(new Rectangle(0, 0, Zoom.X, Math.Max(player.GetCenter().Y - Sight, 0)), Color.Black, 1f, Standard.LightMode.Absolute);
                        Standard.DrawLight(new Rectangle(player.GetCenter().X + Sight, 0, Zoom.X, Zoom.X), Color.Black, 1f, Standard.LightMode.Absolute);
                        Standard.DrawLight(new Rectangle(0, player.GetCenter().Y + Sight, Zoom.X, Zoom.X), Color.Black, 1f, Standard.LightMode.Absolute);
                        DrawingLayer PlayerSight = new DrawingLayer("Sight3", new Rectangle(player.GetCenter().X - Sight, player.GetCenter().Y - Sight, Sight * 2, Sight * 2));
                        PlayerSight.Draw();
                        if (Checker.Weapon_Melee == 16 && SlowMode && !ShotMode)
                        {
                            Standard.DrawAddon(PlayerSight, Color.White, 0.1f, "TheWorldClock");
                        }            
                        if(Sight<0)
                        {
                            Standard.FadeAnimationDraw(ParticleEngine.BloodColor);//별이 사라지는 페이드애니메이션(컬러는 LighteaGreen으로 지정)은 아래 라이트레이어 전에 발생해야 보기 좋으므로 별도로 처리함.
                         
                            Standard.DrawLight(new Rectangle(0, 0, MasterInfo.PreferredScreen.Width * 4, MasterInfo.PreferredScreen.Height * 4), Color.White, 1f, Standard.LightMode.Vignette);
                            Standard.DrawLight(new Rectangle(0, 0, MasterInfo.PreferredScreen.Width * 4, MasterInfo.PreferredScreen.Height * 4), Color.DarkBlue, 0.3f, Standard.LightMode.Absolute);
                        }
                    }

                    Color OveColor;
                    if (Checker.Weapon_Melee == 18 && !ShotMode)
                        OveColor = Color.Red;
                    else
                        OveColor = Color.DarkBlue;


                    Standard.DrawLight(BloodLayer.GetBound(), OveColor, (float)(PressedATimer / 100.0), Standard.LightMode.Absolute);

                    if (FreezeTimer > 0)
                        Standard.ClearFadeAnimation();

                    Standard.DrawAddon(BloodLayer, Room.RoomColor, 1f, "Wall3");

                    if (CardInfoUI.CardIndex != -1)
                    {
                        CardInfoUI.Draw();
                    }
                    #endregion

                    #region 세팅화면 처리
                    if (ShowMenu)
                    {
                        Game1.graphics.GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
                        MenuLayer.Draw(Color.Black * 0.7f);
                        DrawingLayer SCGSample = new DrawingLayer(Costume.GetCostume("SCGSample"), new Point(600, 0), 0.45f);
                        if (Checker.Hearts < 5)
                        {
                            SCGSample.SetSprite(Costume.GetCostume("SCG_Dying"));
                        }
                        else if (Checker.Hearts < 25)
                            SCGSample.SetSprite(Costume.GetCostume("SCGSample"));
                        else
                            SCGSample.SetSprite(Costume.GetCostume("SCG_Happy"));
                        if (MadMoonGauge > 10)
                            SCGSample.SetSprite(Costume.GetCostume("SCG_Crazy"));
                        SCGSample.Draw();
                        Standard.DrawString("Mouse Sensitivity", new Vector2(ScrollBar_Sensitivity.Frame.GetPos().X, ScrollBar_Sensitivity.Frame.GetPos().Y - 20), Color.White);
                        ScrollBar_Sensitivity.Draw();
                        Standard.DrawString(String.Format("{0:0.0}", ScrollBar_Sensitivity.Coefficient + 0.5f), new Vector2(ScrollBar_Sensitivity.Frame.GetPos().X, ScrollBar_Sensitivity.Frame.GetPos().Y+50), Color.White);
                        Standard.DrawString("(Default:1)", new Vector2(ScrollBar_Sensitivity.Frame.GetPos().X, ScrollBar_Sensitivity.Frame.GetPos().Y + 70), Color.White);
                        ScrollBar_SongVolume.Draw();
                        Standard.DrawString("Song Volume", new Vector2(ScrollBar_SongVolume.Frame.GetPos().X, ScrollBar_SongVolume.Frame.GetPos().Y - 20), Color.White);
                        ScrollBar_SEVolume.Draw();
                        Standard.DrawString("SE Volume", new Vector2(ScrollBar_SEVolume.Frame.GetPos().X, ScrollBar_SEVolume.Frame.GetPos().Y - 20), Color.White);
                        //ExitButton.Draw();
                        //if (ExitButton.MouseIsOnThis())
                            //ExitButton.Draw(Color.DarkRed);
                        RestartButton.Draw();
                        if (RestartButton.MouseIsOnThis())
                            RestartButton.Draw(Color.DarkRed);
                      
                        Standard.FrameTimer--;
                    }
                    #endregion

                    #region 좀비의 얼굴 그리기
                    if (!GameOver && !ShowMenu)
                    {
                        for (int i = 0; i < enemies.Count; i++)
                            enemies[i].DrawAddOn();
                    }
                    if (GameOver)
                    {
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            if (enemies[i].ThisIsTheKiller)
                            {
                                Standard.DrawAddon(enemies[i].enemy, Color.Black, 1f, "Player");
                                if (Standard.FrameTimer % 20 <= 10)
                                    Standard.DrawAddon(enemies[i].enemy, Color.White, 1f, "ZombieBite");
                                else
                                    Standard.DrawAddon(enemies[i].enemy, Color.White, 1f, "ZombieBite2");
                            }
                        }

                    }
                    #endregion


                    /*스테이지 시작시 연출*/

                    if (StartStageTimer > 80 && StartStageTimer < 120)
                    {
                        Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 1f, Standard.LightMode.Absolute);
                    }

                    if (StartStageTimer > 0 && StartStageTimer < 100)
                    {
                        Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 1f * (float)(StartStageTimer / 100.0), Standard.LightMode.Absolute);
                        Standard.DrawString("BigFont", Room.Name(), new Vector2(-Game1.graphics.GraphicsDevice.Viewport.X + 400, -Game1.graphics.GraphicsDevice.Viewport.Y + 300), Color.White * (float)(StartStageTimer / 100.0));
                        Standard.DrawString("BigFont", Room.Name(), new Vector2(-Game1.graphics.GraphicsDevice.Viewport.X + 400, -Game1.graphics.GraphicsDevice.Viewport.Y + 300), Color.Red * 0.2f * (float)(StartStageTimer / 100.0));
                    }

                    break;//Game Phase Draw
                case Phase.Ending:

                    Game1.graphics.GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
                    DrawingLayer EndingCut;

                    EndingCut = new DrawingLayer(EndCGList[0], new Point(0, 0), 0.67f);
                    int End_Interval;
                    if (!LiteMode)
                        End_Interval = 6;
                    else
                        End_Interval = 30;
                    if (Credit.IsEnded())
                    {
                        if (EndTimer > 0)
                            EndTimer--;
                        else
                        {
                            if (EndCGList.Count > 1)
                                EndCGList.RemoveAt(0);
                            EndTimer = End_Interval;
                        }

                    }
                    EndingCut.Draw();
                    Standard.DrawLight(EndingCut, Color.AliceBlue, (float)((Math.Max(250 - Standard.FrameTimer % 250, Standard.FrameTimer % 250) - 100) / 800.0), Standard.LightMode.Absolute);
                    if (EndingCut.GetSpriteName() == "WhiteSpace")
                    {
                        Standard.PlayFadedSE("HorrorDoor", 0.3f);
                        Standard.DrawLight(MasterInfo.FullScreen, Color.Black, 1f, Standard.LightMode.Absolute);
                    }
                    Credit.Draw();
                    if (EndCGList.Count == 1)
                    {
                        GoBackMenu.Draw(Color.White, Color.Red);
                        Standard.DrawString(EndingString, new Vector2(GoBackMenu.ButtonGraphic.GetPos().X, GoBackMenu.ButtonGraphic.GetPos().Y+40), Color.White);
                    }
                    break;
                case Phase.Dead:
                    Game1.graphics.GraphicsDevice.Viewport = new Viewport(MasterInfo.FullScreen);
                    DrawingLayer DeadCut = new DrawingLayer("Dead_1", new Point(-40, 0), 0.72f);
                    DrawingLayer DeadCutWord = new DrawingLayer("GameOverWord", new Point(260, 200), 0.8f);
                    int Checkpoint = 1000;
                    if (Standard.FrameTimer < Checkpoint)
                    {
                        if (Standard.FrameTimer % 250 < 125)
                            DeadCut.SetSprite("Dead_2");
                    }
                    else
                    {
                        int Interval = 50;
                        if (Standard.FrameTimer < Checkpoint + Interval)
                            DeadCut.SetSprite("Dead_3");
                        else if (Standard.FrameTimer < Checkpoint + 2 * Interval)
                            DeadCut.SetSprite("Dead_4");
                        else if (Standard.FrameTimer < Checkpoint + 3 * Interval)
                            DeadCut.SetSprite("Dead_5");
                        else if (Standard.FrameTimer < Checkpoint + 4 * Interval)
                            DeadCut.SetSprite("Dead_6");
                        else
                            DeadCut.SetSprite("Dead_7");
                    }


                    DeadCut.Draw();
                    DeadCutWord.Draw();
                    Standard.DrawLight(DeadCut, Color.Black, (float)((Math.Max(250 - Standard.FrameTimer % 250, Standard.FrameTimer % 250) - 100) / 450.0), Standard.LightMode.Absolute);
                    RetryButton.Draw(Color.White, Color.Red);

                    break;
                case Phase.Extra:
                    if(ExtraMenualEnabled)
                    {
                        DrawingLayer ExtraLayer = new DrawingLayer("ExtraMenual", new Point(0, 0), 0.67f);
                        ExtraLayer.Draw();
                        
                    }
                    else
                    {
                        DrawingLayer ExtraLayer = new DrawingLayer("ShopUI", new Point(0, 0), 0.67f);
                        ExtraLayer.Draw();
                        ShopComponent.Draw();
                        Standard.DrawLight(InShop_GoBackButton.ButtonGraphic.GetBound(), Color.Black, 1f, Standard.LightMode.Absolute);
                        InShop_GoBackButton.Draw(Color.White, Color.Red);

                    }


                    break;

            }
            #region Debug Setting
            
            /*
            if (Standard.Pressing(Keys.LeftControl, Keys.Q))
                Standard.DrawString(Cursor.GetPos().X.ToString() + "," + Cursor.GetPos().Y.ToString(), new Vector2(Cursor.GetPos().X - 20, Cursor.GetPos().Y - 30), Color.White);
            if (Standard.Pressing(Keys.LeftControl, Keys.W))
                Standard.DrawString(Standard.FrameTimer.ToString(), new Vector2(Cursor.GetPos().X - 20, Cursor.GetPos().Y - 30), Color.White);
            if (Standard.Pressing(Keys.LeftControl, Keys.E))
                Standard.DrawString(Table.m.ToString(), new Vector2(Cursor.GetPos().X - 20, Cursor.GetPos().Y - 30), Color.White);
                */
                
            #endregion

            GameConsole.Draw();
        }


        public class Player
        {
            public DrawingLayer player;
            private Point MovePoint = new Point(0, 0);
            private int Range = 130;
            private double MoveSpeed = 6;
            private int AttackSpeed = 15;
            private int AttackTimer = 0;
            //private int AttackIndex = -1;
            private bool isAttacking = false;

            private Point DeadPoint;

            public void SetMoveSpeed(double s)
            {
                MoveSpeed = s;
            }

            public void SetAttackSpeed(int s)
            {
                AttackSpeed = s;
            }

            public bool IsAttacking()
            {
                return isAttacking;
            }

            public Point GetCenter()
            {
                return player.GetCenter();
            }

            public void MoveByVector(Point p, double speed)
            {
                player.MoveByVector(p, speed);
            }

            public void Reset()
            {
                Cursor.SetPos(450, 480);
                setPos(450, 480);
                Game1.graphics.GraphicsDevice.Viewport = new Viewport(-GetPos().X + 400, -GetPos().Y + 400, Zoom.X, Zoom.X);
                MovePoint = new Point(0, 0);
                AttackTimer = 0;
                //AttackIndex = -1;
                isAttacking = false;
                DeadBodys.Clear();
            }
            /*
			public int getAttackIndex()
			{
				return AttackIndex;
			}*/

            public int getAttackTimer()
            {
                return AttackTimer;
            }

            public int getAttackSpeed()
            {
                return AttackSpeed;
            }

            public Point GetPos()
            {
                return player.GetPos();
            }

            public void setPos(int x, int y)
            {
                player.SetPos(x, y);
            }

            public Point getMovePoint()
            {
                return MovePoint;
            }

            public int getRange()
            {
                return Range;
            }
            public void setRange(int i)
            {
                Range = i;
            }



            public Player()
            {
                player = new DrawingLayer("Player_V6", new Rectangle(400, 400, 90, 90));
                player.AttachAnimation(30, Costume.GetCostume("Player_Ani11"), Costume.GetCostume("Player_Ani12"));
            }

            public void Draw()
            {
                player.Draw(Color.White);
            }

     

            public void MoveUpdate()
            {
                player.Animate(true);
                if (isAttacking)
                {
                    if (!ShotMode)
                    {
                        if (Checker.Weapon_Melee == 14 && AttackTimer > AttackSpeed - 4)
                        {
                            Standard.FadeAnimation(player, Standard.Random(1, 8), Color.WhiteSmoke);
                            player.MoveByVector(Method2D.Deduct(DeadPoint, player.GetCenter()), (Method2D.Distance(DeadPoint, player.GetCenter())) / 3.0);
                            return;
                        }
                        if (AttackTimer < AttackSpeed - 3)
                        {
                            player.MoveTo(Cursor.GetPos().X - 40, Cursor.GetPos().Y - 40, MoveSpeed + 4);
                            Standard.FadeAnimation(new DrawingLayer("Player_AfterImage", new Rectangle(player.GetPos(), new Point(70, 70))), 5, Color.AliceBlue);
                        }
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            if (enemies[i].getBound().Contains(Cursor.GetPos()) && Method2D.Distance(GetCenter(), enemies[i].getCenter()) < Range)
                            {
                                return;
                            }
                        }

                        MovePoint = Cursor.GetPos();
                        return;
                    }
                    else
                    {
                        if (Checker.Weapon_Ranged != 51)
                        {
                            if (AttackTimer > AttackSpeed / 2)
                                player.MoveTo(MovePoint.X, MovePoint.Y, -(AttackTimer - AttackSpeed / 2));
                            else
                            {
                                player.MoveTo(MovePoint.X, MovePoint.Y, MoveSpeed);
                                MovePoint = Cursor.GetPos();
                            }
                            Standard.FadeAnimation(new DrawingLayer("Player_AfterImage", new Rectangle(player.GetPos(), new Point(70, 70))), 5, Color.AliceBlue);

                        }
                        else
                        {

                            player.MoveTo(MovePoint.X, MovePoint.Y, MoveSpeed + 15);
                            if (Method2D.Distance(player.GetPos(), MovePoint) < 10)
                            {
                                AttackTimer = 0;
                            }

                            for (int i = 0; i < enemies.Count; i++)
                            {
                                if (!enemies[i].IsDead() && Checker.GunShotCheckEvent(enemies[i].getCenter())) /*enemies[i].getBound().Contains(Cursor.GetPos())*/
                                {

                                    if (enemies[i].IsGhost)
                                        enemies[i].DeadActivate(Color.WhiteSmoke);
                                    else
                                        enemies[i].DeadActivate(Color.DarkRed);
                                }
                            }

                        }


                        return;
                    }
                }
                if (ShotMode)
                {

                    if (!IsEndPhase && StartStageTimer == 0 && Bullets >= 1)
                    {
                        if (Cursor.JustdidLeftClick() && Checker.Weapon_Ranged == 51)
                        {
                            DeadPoint = Cursor.GetPos();
                            isAttacking = true;
                            AttackTimer = AttackSpeed;
                            MovePoint = DeadPoint;
                            return;
                        }
                        if (Checker.Weapon_Ranged == 54)
                        {
                            for (int i = 0; i < enemies.Count; i++)
                            {
                                if (!enemies[i].IsDead() && Checker.GunShotCheckEvent(enemies[i].getCenter()) && Method2D.Distance(GetCenter(), enemies[i].getCenter()) < Range && Cursor.JustdidLeftClick())
                                {
                                    if (!enemies[i].IsGhost)
                                    {
                                        enemies[i].FrozenTimer = 200;
                                    }

                                    DeadPoint = Cursor.GetPos();
                                    isAttacking = true;
                                    AttackTimer = AttackSpeed;
                                    MovePoint = DeadPoint;
                                }

                            }
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                if (Checker.GunShotCheckEvent(bludgers[i].bludger.GetCenter()) && Method2D.Distance(GetCenter(), bludgers[i].bludger.GetCenter()) < Range && Cursor.JustdidLeftClick())
                                {

                                    bludgers[i].FrozenTimer = 200;

                                    DeadPoint = Cursor.GetPos();
                                    isAttacking = true;
                                    AttackTimer = AttackSpeed;
                                    MovePoint = DeadPoint;
                                }

                            }
                        }
                        else
                        {
                            for (int i = 0; i < enemies.Count; i++)
                            {
                                if (!enemies[i].IsDead() && Checker.GunShotCheckEvent(enemies[i].getCenter()) && Method2D.Distance(GetCenter(), enemies[i].getCenter()) < Range && Cursor.JustdidLeftClick()) /*enemies[i].getBound().Contains(Cursor.GetPos())*/
                                {
                                    if (enemies[i].IsGhost)
                                        enemies[i].DeadActivate(Color.WhiteSmoke);
                                    else
                                        enemies[i].DeadActivate(Color.DarkRed);
                                    DeadPoint = Cursor.GetPos();
                                    isAttacking = true;
                                    AttackTimer = AttackSpeed;
                                    MovePoint = DeadPoint;
                                }
                            }
                        }


                    }

                }
                else
                {
                    if (!IsEndPhase && StartStageTimer == 0)
                    {

                        if (isAttacking)
                            return;
                        if (Checker.Weapon_Melee == 18 && !ShotMode && ChainTimer > 40)
                        {
                            for (int i = 0; i < enemies.Count; i++)
                            {
                                if (!enemies[i].IsDead() && Method2D.Distance(Cursor.GetPos(), enemies[i].getCenter()) < 100 && Method2D.Distance(GetCenter(), enemies[i].getCenter()) < Range)
                                {
                                    //AttackIndex = i;
                                    if (enemies[i].IsGhost)
                                        enemies[i].DeadActivate(Color.WhiteSmoke);
                                    else
                                        enemies[i].DeadActivate(Color.DarkRed);
                                    DeadPoint = enemies[i].GetPos();
                                    isAttacking = true;
                                    AttackTimer = AttackSpeed;
                             
                                }

                            }
                        }
                        else
                        {
                            for (int i = 0; i < enemies.Count; i++)
                            {
                                if (!enemies[i].IsDead() && enemies[i].getBound().Contains(Cursor.GetPos()) && Method2D.Distance(GetCenter(), enemies[i].getCenter()) < Range)
                                {
                                    //AttackIndex = i;
                                    if (enemies[i].IsGhost)
                                        enemies[i].DeadActivate(Color.WhiteSmoke);
                                    else
                                        enemies[i].DeadActivate(Color.DarkRed);
                                    DeadPoint = enemies[i].getCenter();
                                    isAttacking = true;
                                    AttackTimer = AttackSpeed;
                                    return;
                                }

                            }
                        }

                    }
                }
                MovePoint = Cursor.GetPos();
                if (MovePoint.X != 0 || MovePoint.Y != 0)
                    player.MoveTo(MovePoint.X - 40, MovePoint.Y - 40, MoveSpeed + Checker.Swiftness * 1.0 / 3.0);
            }

            public void AttackUpdate()
            {
                if (isAttacking)
                {
                    if (AttackTimer == AttackSpeed)/*&&AttackIndex!=-1&&enemies.Count>AttackIndex*/
                    {
                        /*
						enemies[AttackIndex].enemy.SetSprite("Player_Broken2");
						Standard.FadeAnimation(enemies[AttackIndex].enemy, 15, Color.AntiqueWhite);*/
                        if (!ShotMode)
                        {
                            Standard.PlayFadedSE("KnifeSound", 0.4f);
                            Standard.PlayFadedSE("GunSound", 0.3f);
                        }
                        else
                        {
                            Checker.GunFireEvent();
                            /*
                            if(Checker.Weapon_Ranged!=53)
                            {
                                Standard.PlayFadedSE("KnifeSound", 0.3f);
                                Standard.PlayFadedSE("ClapMode", 1f);
                                Standard.PlayFadedSE("GunSound", 1f);
                                Standard.FadeAnimation(new DrawingLayer("GunFire", new Rectangle(player.GetCenter().X + 25, player.GetCenter().Y - 30, 50, 50)), 15, Color.NavajoWhite);
                            }
                            else
                            {
                                Standard.PlayFadedSE("RifleFire", 1f);
                                Standard.FadeAnimation(new DrawingLayer("RifleFireEffect", new Rectangle(player.GetCenter().X + 35, player.GetCenter().Y - 170, 200, 200)), 15, Color.NavajoWhite);
                            }*/
                        }
                    }
                    if (AttackTimer > 0)//투사체 날아가는중
                    {
                        AttackTimer--;
                        if (ShotMode)
                            return;
                    }
                    else//투사체 적중
                    {
                        /*
						if(enemies.Count>AttackIndex&&AttackIndex!=-1)
						{
							if (enemies[AttackIndex].IsGhost)
								RemoveEnemy(AttackIndex, Color.AliceBlue);
							else
								RemoveEnemy(AttackIndex, Color.DarkRed);
						}*/


                        isAttacking = false;
                        /*AttackIndex = -1;*/
                        return;
                    }
                }
            }

            public void DrawAttack()
            {
                if (isAttacking && AttackTimer >= 1)/*&&enemies.Count>AttackIndex&&AttackIndex!=-1*/
                {
                    int x = ((AttackSpeed - AttackTimer) * DeadPoint.X + AttackTimer * GetPos().X) / AttackSpeed;
                    int y = ((AttackSpeed - AttackTimer) * DeadPoint.Y + AttackTimer * GetPos().Y) / AttackSpeed;


                    int KillActionTimer = AttackTimer * 2;

                    if (!ShowMenu && Standard.FrameTimer % 5 == 0)
                    {
                        if (Standard.FrameTimer % 20 < 6)
                            Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Cursor.GetPos().X / 2 + DeadPoint.X / 2 - 25, Cursor.GetPos().Y / 2 + DeadPoint.Y / 2 - 25, 70, 70)), 15, Color.Pink);
                        else if (Standard.FrameTimer % 20 < 12)
                            Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Cursor.GetPos().X / 2 + DeadPoint.X / 2 - 25, Cursor.GetPos().Y / 2 + DeadPoint.Y / 2 - 25, 70, 70)), 15, Color.PaleVioletRed);
                        else
                            Standard.FadeAnimation(new DrawingLayer("BladeAttack2", new Rectangle(Cursor.GetPos().X / 2 + DeadPoint.X / 2 - 25, Cursor.GetPos().Y / 2 + DeadPoint.Y / 2 - 25, 70, 70)), 15, Color.SkyBlue);
                    }



                }
            }
        }

        public class Enemy
        {
            public DrawingLayer enemy;
            public bool IsGhost = false;
            public bool ThisIsTheKiller = false;
            private double GhostAngle;
            private double Ghostw = 0.03;
            private double GhostDistance = 800;
            private event Action MoveAction;
            private event Action DrawAction;
            private event Action DrawAddonAction;
            private event Action SDeadAction;
            private event Action DieAction;
            public int DieTimer = 0;
            public int FrozenTimer = 0;
            public static Color GhostColor = Color.CornflowerBlue;

            public bool IsDead()
            {
                if (DieAction == null)
                    return false;
                else
                    return true;
            }

            public void DeadActivate(Color color)
            {
                ScoreStack++;
                DieTimer = 15;
                enemy.SetSprite("Player_Broken2");
                Standard.FadeAnimation(enemy, 15, Color.AntiqueWhite);
                ParticleEngine.GenerateBlood(new Point(enemy.GetPos().X, enemy.GetPos().Y));
                DieAction += () =>
                {
                    if (DieTimer == 0)
                    {

                        Rectangle r = getBound();
                        int rn = Standard.Random(3, 5);
                        for (int i = 0; i < rn; i++)
                        {
                            int s = Standard.Random(10, 50);
                            DrawingLayer newStar;
                            Standard.FadeAnimation(newStar = new DrawingLayer("Player2", new Rectangle(r.Center.X - Standard.Random(-30, 30), r.Center.Y - Standard.Random(-30, 30), s, s)), Standard.Random(5 * 3, 15 * 3), color);
                            DeadBodys.Add(newStar);
                        }
                        
                        enemies.Remove(this);
                        return;
                    }
                    if (DieTimer > 0)
                        DieTimer--;
                };

            }


            public static bool WholeGameOver = false;
            public Point GetPos()
            {
                return enemy.GetPos();
            }

            public Point getCenter()
            {
                return enemy.GetCenter();
            }

            public Rectangle getBound()
            {
                return enemy.GetBound();
            }
            public Enemy(bool isGhost)
            {
                IsGhost = isGhost;

                if (IsGhost)
                {
                    enemy = new DrawingLayer("Player_V6", new Rectangle(0, 0, 100, 100));
                    GhostAngle = Standard.Random() * 3;
                    MoveAction += Ghost_MoveAction;
                    DrawAction += Ghost_DrawAction;
                    SDeadAction += () => Tester.KillCard = new DrawingLayer(Costume.GetCostume("SDead_22"), new Point(0, 0), 0.6f);
                    DrawAddonAction += () =>
                    {
                        if (Standard.FrameTimer % 50 < 25)
                        {
                            Standard.DrawAddon(enemy, GhostColor, 0.5f, "GhostHead_1");//Color.LightSkyBlue
                            if (Standard.FrameTimer % 15 == 0)
                                Standard.FadeAnimation(new DrawingLayer("GhostHead_1", enemy.GetBound()), 10, GhostColor);
                        }
                        else
                        {
                            Standard.DrawAddon(enemy, GhostColor, 0.5f, "GhostHead_2");
                            if (Standard.FrameTimer % 15 == 0)
                                Standard.FadeAnimation(new DrawingLayer("GhostHead_2", enemy.GetBound()), 10, GhostColor);
                        }
                    };
                }
                else
                {
                    int x = 0;
                    int y = 0;

                    if (Standard.Random(0, 2) == 0)
                        x = Standard.Random(15, 80);
                    else
                        x = Standard.Random(800, 880);
                    if (Standard.Random(0, 2) == 0)
                        y = Standard.Random(15, 80);
                    else
                        y = Standard.Random(720, 800);
                    if (Method2D.Distance(player.GetPos(), new Point(x, y)) > 80)
                        enemy = new DrawingLayer("Enemy", new Rectangle(x, y, 100, 100));
                    else
                        enemy = new DrawingLayer("Enemy", new Rectangle(x + 200, y + 200, 100, 100));
                    SDeadAction += () => Tester.KillCard = new DrawingLayer(Costume.GetCostume("SDead_11"), new Point(0, 0), 0.6f);
                    MoveAction += Rock_MoveAction;
                    DrawAction += Rock_DrawAction;
                    if (Room.Number != 66)
                    {
                        DrawAddonAction += () =>
                        {
                            if (Score.var % 4 == 0)
                                Standard.DrawAddon(enemy, Color.White, 1f, "NormalZombie");
                            else if (Score.var % 4 == 1)
                                Standard.DrawAddon(enemy, Color.White, 1f, "NormalZombie_3");
                            else
                                Standard.DrawAddon(enemy, Color.White, 1f, "NormalZombie_4");
                        };
                    }
                    else
                    {
                        DrawAddonAction += () =>
                        {
                            if (Score.var % 2 == 0)
                                Standard.DrawAddon(enemy, Color.White, 1f, "NormalZombie_4");
                            else
                                Standard.DrawAddon(enemy, Color.White, 1f, "NormalZombie_3");
                        };
                    }
                }
            }
            public void Draw()
            {
                DrawAction();
                
                if (FrozenTimer > 0)
                    Standard.FadeAnimation(new DrawingLayer("frozen", enemy.GetBound()), 8, Color.FloralWhite);
            }
            public void DrawAddOn() => DrawAddonAction();

            public void Ghost_DrawAction()
            {
                enemy.Draw(Color.PaleTurquoise);
            }

            public void Rock_DrawAction()
            {
                enemy.Draw(Room.RoomColor);
            }
            public void Ghost_MoveAction()
            {
                if (GhostDistance > 3)
                {
                    if (SlowMode)
                    {
                        GhostDistance = GhostDistance - 3 * TimeCoefficient;
                        GhostAngle += Ghostw * TimeCoefficient;
                    }
                    else
                    {
                        GhostDistance = GhostDistance - 3;
                        GhostAngle += Ghostw;
                    }
                    enemy.SetCenter(new Point(player.GetCenter().X + (int)(GhostDistance * (Math.Cos(GhostAngle))), player.GetCenter().Y + (int)(GhostDistance * (Math.Sin(GhostAngle)))));
                }
                else
                {
                    enemy.SetCenter(player.GetCenter());
                }
            }
            public void Rock_MoveAction()
            {
                if (SlowMode)
                    enemy.MoveTo(player.GetPos().X + RandomInts[Math.Abs(RandomIntCounter) % 14], player.GetPos().Y + RandomInts[(RandomIntCounter + Standard.FrameTimer + 100) % 14], ZombieSpeed * TimeCoefficient);
                else
                    enemy.MoveTo(player.GetPos().X + RandomInts[Math.Abs(RandomIntCounter) % 14], player.GetPos().Y + RandomInts[(RandomIntCounter + Standard.FrameTimer + 100) % 14], ZombieSpeed);

                int Dis;

                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i] != this)
                    {
                        Dis = Method2D.Distance(GetPos(), enemies[i].GetPos());
                        if (Dis != 0)
                            enemy.MoveTo(enemies[i].GetPos().X, enemies[i].GetPos().Y, -160.0 / Dis);
                    }
                }
            }

            public void MoveUpdate()
            {
                if (DieAction != null)
                {
                    DieAction();
                    return;
                }
                if (FrozenTimer > 0)
                {
                    FrozenTimer--;
                    return;
                }

                MoveAction();
                double r = Method2D.Distance(GetPos(), player.GetPos());
                HeartSignal += (1600.0 / (r * r));

                //Death Check
                if (!IsEndPhase && StartStageTimer == 0 && (Method2D.Distance(player.GetCenter(), getCenter())) <= 10)
                {
                    /*
                    if (player.getAttackIndex() != -1 && this != enemies[player.getAttackIndex()])
                        return;*/
                    if (!GameOver&&!Checker.LuckCheck())
                    {
                        ThisIsTheKiller = true;

                        //KillerZombieIndex = Index;
                        //Need to change dead scene sprite
                        if (SDeadAction != null)
                            Game1.AttachDeadScene(SDeadAction);
                        GameOver = true;
                        Checker.Hearts--;
                    }
                }

            }
        }

        public class Bludger
        {
            public static int BludgerSpeed = 14;
            public static Rectangle BoundRectangle;
            public DrawingLayer bludger = new DrawingLayer("Player_V6", new Rectangle(2000, 0, 80, 80));
            public Point v = new Point(1, 1);
            public int FrozenTimer = 0;
            public static Color BludgerColor = Color.IndianRed;

            public Bludger(Point vector)
            {
                v = vector;
                bludger.SetPos(Standard.Random(-50, 50), Standard.Random(-50, 50));
            }
            public void MoveUpdate()
            {
                if (FrozenTimer > 0)
                {
                    if (FrozenTimer == 1)
                    {
                        v = Method2D.Deduct(player.GetPos(), bludger.GetPos());
                    }
                    FrozenTimer--;
                    return;
                }
                BoundRectangle = new Rectangle(-Game1.graphics.GraphicsDevice.Viewport.X, -Game1.graphics.GraphicsDevice.Viewport.Y, 900, 720);
                //벡터 계산
                if (BoundRectangle.X > bludger.GetPos().X || 0 > bludger.GetPos().X)//공이 왼쪽으로 나갈 경우
                {

                    v = Method2D.Deduct(player.GetPos(), bludger.GetPos());
                }
                if (BoundRectangle.Y > bludger.GetPos().Y || 0 > bludger.GetPos().Y)//공이 위로 나갈 경우
                {

                    v = Method2D.Deduct(player.GetPos(), bludger.GetPos());
                }
                if (BoundRectangle.X + BoundRectangle.Width < bludger.GetPos().X || MasterInfo.FullScreen.Width - 80 < bludger.GetPos().X)//공이 오른쪽으로 나갈 경우
                {

                    v = Method2D.Deduct(player.GetPos(), bludger.GetPos());
                }
                if (BoundRectangle.Y + BoundRectangle.Height < bludger.GetPos().Y || MasterInfo.FullScreen.Height - 80 < bludger.GetPos().Y)//공이 오른쪽으로 나갈 경우
                {

                    v = Method2D.Deduct(player.GetPos(), bludger.GetPos());
                }

                if (SlowMode)
                    bludger.MoveByVector(v, BludgerSpeed * TimeCoefficient);
                else
                    bludger.MoveByVector(v, BludgerSpeed);

                if (!IsEndPhase && StartStageTimer == 0 && (Method2D.Distance(player.GetCenter(), bludger.GetCenter())) <= 10)
                {


                    if (!GameOver && !Checker.LuckCheck())
                    {
                        Game1.AttachDeadScene(() => Tester.KillCard = new DrawingLayer(Costume.GetCostume("SDead_333"), new Point(0, 0), 0.6f));
                        //KillerZombieIndex = -1;
                        GameOver = true;
                        Checker.Hearts--;
                    }
                    else
                    {

                    }

                    /* 현재 블러저 처리에 관한 두가지 안
					 * 1. 맞으면 사망
					 * 2. 맞으면 끌려감
					 *  2가 더 재밌고 편하긴 함.
					 * */
                }

            }
            public void Draw()
            {
                bludger.Draw(BludgerColor*0.5f);
        
                Standard.DrawAddon(bludger, Color.LightYellow, 1f, "BludgerFace");
            }


        }


        public static class Room
        {
            public static int Number;
            public static Color RoomColor;
            public static Color StarColor;
            public static Dictionary<int, string> RoomKeys = new Dictionary<int, string>();
            public static Dictionary<int, int> RoomDifficulties = new Dictionary<int, int>();

            public static event Action SongEvent;

            /* ROOM INDEX*
			 * 000: Room of Rock				*
			 * 001: Room of Fire				**
			 * 002: Room of Ice					**
			 * 003: Room of Fire(Hard)			***
			 * 004: Room of Fire(EX)			****
			 * 005: Room of Ice(Hard)			***
			 * 006: Room of Ice(EX)				****
			 * 013: Room of Fire and Ice		***
			 * 014: Room of Fire(Easy)			*
			 * 015: Room of Ice(Easy)			*
			 * 016: Room of Fire and Ice(Easy)	**
			 * 77: Room of Fire and Ice(Hard)	*****
			 * 777: Room of Fire and Ice(EX)	*******
			 * 66: Inferno						*****
			 * 666: Inferno(EX)					*******
			 * */

            public static void AttachSong(string s)
            {
                SongEvent = () => Standard.PlayLoopedSong(s);
            }
            public static void PlaySong() => SongEvent();
            public static void Init()
            {
                RoomKeys.Clear();
                RoomDifficulties.Clear();
                RoomKeys.Add(0, Standard.RandomString("This is real love, Ortolan."));
                RoomDifficulties.Add(0, 1);
                RoomKeys.Add(14, Standard.RandomString("So do not fear for I am with you."));
                RoomDifficulties.Add(14, 1);
                RoomKeys.Add(15, Standard.RandomString("Remain in me."));
                RoomDifficulties.Add(15, 1);
                RoomKeys.Add(16, "I will praise You with my whole heart.");
                RoomDifficulties.Add(16, 2);

                RoomKeys.Add(1, Standard.RandomString("Flee from all this."));
                RoomDifficulties.Add(1, 2);
                RoomKeys.Add(2, "Be still. And know that I'm god.");
                RoomDifficulties.Add(2, 2);
                RoomKeys.Add(13, Standard.RandomString("I will not forget you."));
                RoomDifficulties.Add(13, 3);
                RoomKeys.Add(3, "Let not your heart to be troubled.");
                RoomDifficulties.Add(3, 3);
                RoomKeys.Add(5, "I have made you. I will carry you.");
                RoomDifficulties.Add(5, 3);
                RoomKeys.Add(77, "I am Alpha and Omega, The beginning and the End, " +
                    "\nThe first and the last.");
                RoomDifficulties.Add(77, 5);
                RoomKeys.Add(66, "THAT WAS A LIE, YOU IDIOT!");
                RoomDifficulties.Add(66, 5);


                RoomKeys.Add(4, "Tonight, Ortolan joins the hunt...");
                RoomDifficulties.Add(4, 4);

                RoomKeys.Add(6, "Does the nightmare never ends?");
                RoomDifficulties.Add(6, 4);

                RoomKeys.Add(777, "Room of Fire and Ice(EX)");
                RoomDifficulties.Add(777, 7);
                RoomKeys.Add(666, "Inferno(EX)");
                RoomDifficulties.Add(666, 7);


            }
            public static int ClearRewardCount()
            {
                return RoomDifficulties[Room.Number];
            }
            public static string Name()
            {
                return RoomKeys[Room.Number];
            }
            public static void SetFireRoom(int FireCount, int Interval)
            {
                RoomColor = Color.Orange;
                StarColor = Color.Orange;
                TheIceRoom = false;
                AttachSong("GhostTown");
                //Standard.PlayLoopedSong("GhostTown");
                for (int i = 0; i < FireCount; i++)
                {
                    bludgers.Add(new Bludger(new Point(i + 1, 1)));
                    bludgers[i].bludger.SetPos(i * Interval, 0);
                }
            }
            public static void SetIceRoom()
            {
                RoomColor = Color.LightSkyBlue;
                StarColor = Color.AliceBlue;
                TheIceRoom = true;
                AttachSong("Stage2_Youdie");
                //Standard.PlayLoopedSong("Stage2_Youdie");
            }
            public static void SetFireAndIceRoom(int FireCount, int Interval)
            {
                RoomColor = Color.MediumPurple;
                StarColor = Color.Aquamarine;
                TheIceRoom = true;
                AttachSong("FireAndIce2");
                //Standard.PlayLoopedSong("FireAndIce2");

                for (int i = 0; i < FireCount; i++)
                {
                    bludgers.Add(new Bludger(new Point(i + 1, 1)));
                    bludgers[i].bludger.SetPos(i * Interval, 0);
                }
            }
            public static void Set()
            {
                ResetGame();
                switch (Number)
                {
                    case 0:
                        RoomColor = Color.LightGray;
                        StarColor = Color.LightSalmon;
                        AttachSong("YoudieTheme8");
                        //Standard.PlayLoopedSong("YouDieTheme8");
                        TheIceRoom = false;
                        //Monolog.RandomAttach("...What was that?");
                        //if(RoomVoiceEnable)
                             //Standard.PlayFadedSE("Room1",0.7f);
                        break;
                    case 14:
                        if (!LiteMode)
                            SetFireRoom(2, 500);
                        else
                            SetFireRoom(1, 500);
                       // Monolog.RandomAttach("I'm alive...");
                        /*
                        if (RoomVoiceEnable)
                            Standard.PlaySE("Voice4");*/
                        break;
                    case 15:
                        SetIceRoom();
                        //Monolog.RandomAttach("I don't want to see that again.");
                        /*
                       if (RoomVoiceEnable)
                           Standard.PlaySE("Voice2");*/
                        break;
                    case 16:
                        if (!LiteMode)
                            SetFireAndIceRoom(2, 500);
                        else
                            SetFireAndIceRoom(1, 500);
                        //Monolog.RandomAttach("God, what do you want from me?");
                        /*
                       if (RoomVoiceEnable)
                           Standard.PlaySE("Voice3");*/
                        break;
                    case 1:
                        if (!LiteMode)
                            SetFireRoom(10, 500);
                        else
                            SetFireRoom(3, 500);
                        //SetFireRoom(20, 1500);
                        break;
                    case 2:
                        SetIceRoom();
                        break;
                    case 3:
                        if (!LiteMode)
                            SetFireRoom(20, 1500);
                        else
                            SetFireRoom(5, 1500);
                        /*
                        if (RoomVoiceEnable)
                            Standard.PlaySE("Voice5");*/

                        //SetFireRoom(30, 1000);
                        break;
                    case 4:
                        SetFireRoom(40, 30000 / 40);
                        break;
                    case 5:
                        SetIceRoom();
                        break;
                    case 6:
                        SetIceRoom();
                        break;

                    case 13:
                        if (!LiteMode)
                            SetFireAndIceRoom(8, 500);
                        else
                            SetFireAndIceRoom(2, 500);

                        break;


                    case 77:
                        if (!LiteMode)
                            SetFireAndIceRoom(16, 500);
                        else
                            SetFireAndIceRoom(4, 500);
                        break;
                    case 777:
                        SetFireAndIceRoom(38, 500);
                        break;
                    case 66:
                        TheIceRoom = false;
                        RoomColor = Color.Orange;
                        StarColor = Color.Orange;
                        AttachSong("Inferno_Final");
                        //Standard.PlayLoopedSong("Inferno_Final");
                        int InfernoCount = 25;
                        if (LiteMode)
                            InfernoCount = 17;
                        for (int i = 0; i < InfernoCount; i++)
                        {
                            bludgers.Add(new Bludger(new Point(i + 1, 1)));
                            bludgers[i].bludger.SetPos((int)(400 + 1000 * Math.Cos(2 * Math.PI * i / InfernoCount)), (int)(400 + 1000 * Math.Sin(2 * Math.PI * i / InfernoCount)));
                        }

                        break;
                    case 666:
                        TheIceRoom = false;
                        RoomColor = Color.Orange;
                        StarColor = Color.Orange;
                        AttachSong("Inferno_Final");
                        //Standard.PlayLoopedSong("Inferno_Final");
                        for (int i = 0; i < 35; i++)
                        {
                            bludgers.Add(new Bludger(new Point(i + 1, 1)));
                            bludgers[i].bludger.SetPos((int)(400 + 1000 * Math.Cos(2 * Math.PI * i / 35)), (int)(400 + 1000 * Math.Sin(2 * Math.PI * i / 35)));
                        }
                        break;


                }
                RoomVoiceEnable = false;

            }

            public static void Update()
            {
                switch (Number)
                {


                    case 2:
                        if (!LiteMode)
                        {
                            if (Standard.FrameTimer % 40 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        else
                        {
                            if (Standard.FrameTimer % 60 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        break;

                    case 5:
                        if (!LiteMode)
                        {
                            if (Standard.FrameTimer % 30 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        else
                        {
                            if (Standard.FrameTimer % 45 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        break;
                    case 6:
                        if (Standard.FrameTimer % 22 == 0)//아이스(=고스트좀비) 생성
                        {
                            enemies.Add(new Enemy(true));
                        }
                        break;
                    case 13:
                        if (Standard.FrameTimer % 55 == 0)//아이스(=고스트좀비) 생성
                        {
                            enemies.Add(new Enemy(true));
                        }
                        break;
                    case 15:
                        if (!LiteMode)
                        {
                            if (Standard.FrameTimer % 60 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        else
                        {
                            if (Standard.FrameTimer % 90 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        break;
                    case 16:
                        if (!LiteMode)
                        {
                            if (Standard.FrameTimer % 60 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        else
                        {
                            if (Standard.FrameTimer % 90 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        break;
                    case 77:
                        if (!LiteMode)
                        {
                            if (Standard.FrameTimer % 45 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        else
                        {
                            if (Standard.FrameTimer % 60 == 0)//아이스(=고스트좀비) 생성
                            {
                                enemies.Add(new Enemy(true));
                            }
                        }
                        /*
                        if (Standard.FrameTimer % 45 == 0)//아이스(=고스트좀비) 생성
                        {
                            enemies.Add(new Enemy(true));

                        }*/
                        break;
                    case 777:
                        if (Standard.FrameTimer % 37 == 0)//아이스(=고스트좀비) 생성
                        {
                            enemies.Add(new Enemy(true));
                        }
                        break;
                    case 66:
                        int InfernoCount = 25;
                        if (LiteMode)
                            InfernoCount = 12;


                        if (Standard.FrameTimer % 500 > 430)
                        {
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                //bludgers[i].bludger.SetPos(i*500, Standard.Random(-50, 50));
                                bludgers[i].bludger.SetPos(-500, -500);
                            }
                        }

                        if (Standard.FrameTimer % 2000 == 0)
                        {
                            double Rnd = Standard.Random();
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                bludgers[i].bludger.SetPos((int)(400 + 1000 * Math.Cos(2 * Math.PI * i / InfernoCount + Rnd)), (int)(400 + 1000 * Math.Sin(2 * Math.PI * i / InfernoCount + Rnd)));

                            }
                        }
                        if (Standard.FrameTimer % 2000 == 500)
                        {
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                bludgers[i].bludger.SetPos(i * 200, 0);

                            }
                        }
                        if (Standard.FrameTimer % 2000 == 1000)
                        {
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                bludgers[i].bludger.SetPos(i * 200, Zoom.X);

                            }
                        }

                        if (Standard.FrameTimer % 2000 == 1500)
                        {
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                if (i < bludgers.Count / 2)
                                    bludgers[i].bludger.SetPos(0, i * 300);
                                else
                                    bludgers[i].bludger.SetPos(2300, (i - bludgers.Count / 2 - 5) * 400);
                            }
                        }

                        break;
                    case 666:
                        if (Standard.FrameTimer % 500 > 430)
                        {
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                bludgers[i].bludger.SetPos(-500, -500);
                            }
                        }

                        if (Standard.FrameTimer % 2000 == 0)
                        {
                            double Rnd = Standard.Random();
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                bludgers[i].bludger.SetPos((int)(400 + 1000 * Math.Cos(2 * Math.PI * i / 35 + Rnd)), (int)(400 + 1000 * Math.Sin(2 * Math.PI * i / 35 + Rnd)));
                            }
                        }
                        if (Standard.FrameTimer % 2000 == 500)
                        {
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                bludgers[i].bludger.SetPos(i * 150, 0);

                            }
                        }
                        if (Standard.FrameTimer % 2000 == 1000)
                        {
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                bludgers[i].bludger.SetPos(i * 150, Zoom.X);
                            }
                        }

                        if (Standard.FrameTimer % 2000 == 1500)
                        {
                            for (int i = 0; i < bludgers.Count; i++)
                            {
                                if (i < bludgers.Count / 2)
                                    bludgers[i].bludger.SetPos(0, i * 300);
                                else
                                    bludgers[i].bludger.SetPos(2300, (i - bludgers.Count / 2 - 5) * 400);

                            }
                        }

                        break;







                }

            }
        }

    }



    public static class Checker
    {
        private static SafeInt hearts=new SafeInt(0);

        public static int Hearts {
            get
            {
                return hearts.var;
            }
            set
            {
                hearts.var = value;
            }

        }

        public static int HeartStack = 0;
        public static int HeartTimer = 0;

        public static int Bloodthirst = 0;
        public static int Luck = 0;
        public static int Swiftness = 0;
        public static int LuckTimer;
        public static bool LuckAct = false;

        public static Vector2 InfoVector;
        public static DrawingLayer StringBackGround;

        public static double BloodStack = 0;
        public static readonly int Default_AttackSpeed = 15;

        private static event Func<Point, bool> gunShotCheckEvent;
        private static event Action gunReloadSoundEvent;
        private static event Action gunFireEvent;


        public static bool GunShotCheckEvent(Point Enemy_Center) => gunShotCheckEvent(Enemy_Center);

        public static void GunReloadSoundEvent() => gunReloadSoundEvent();

        public static void GunFireEvent() => gunFireEvent();

        public static void AttachGunEvent(Func<Point, bool> gun_event, Action gun_sound_Event, Action gun_fire_Event)
        {
            gunShotCheckEvent = null;
            gunShotCheckEvent += gun_event;
            gunReloadSoundEvent = null;
            gunReloadSoundEvent += gun_sound_Event;
            gunFireEvent = null;
            gunFireEvent += gun_fire_Event;

        }


        private static int weapon_Melee;
        public static int Weapon_Melee
        {
            get
            {
                return weapon_Melee;
            }
            set
            {
                if (!Tester.ShotMode)
                    Equip_Weapon_Melee(value);
                weapon_Melee = value;
            }
        }
        private static int weapon_Ranged;
        public static int Weapon_Ranged
        {
            get
            {
                return weapon_Ranged;
            }
            set
            {
                if (Tester.ShotMode)
                    Equip_Weapon_Ranged(value);
                weapon_Ranged = value;
            }
        }

        private static void Equip_Weapon_Ranged(int WeaponCode)
        {

            switch (WeaponCode)
            {

                case 50://권총
                    GunSetter(500, 1.0f, Costume.GetCostume("Player_Ani_PIS01"), Costume.GetCostume("Player_Ani_PIS02"));
                    AttachGunEvent((Enemy_Center) =>
                    {
                        return Method2D.Distance(Enemy_Center, Cursor.GetPos()) < 100;
                    }, () => Standard.PlaySE("RevolverReload"),
                    () =>
                    {
                        Standard.PlayFadedSE("KnifeSound", 0.3f);
                        Standard.PlayFadedSE("ClapMode", 1f);
                        Standard.PlayFadedSE("GunSound", 1f);
                        Standard.FadeAnimation(new DrawingLayer("GunFire", new Rectangle(Tester.player.GetCenter().X + 25, Tester.player.GetCenter().Y - 30, 50, 50)), 15, Color.NavajoWhite);
                    });
                    Tester.Bullets_Max = 6;
                    Tester.ReloadTime = 40;
                    break;
                case 51://드림런처
                    GunSetter(500, 5.0f, Costume.GetCostume("Player_Ani_RO01"), Costume.GetCostume("Player_Ani_RO02"));
                    AttachGunEvent((Enemy_Center) =>
                    {
                        return Method2D.Distance(Enemy_Center, Tester.player.GetPos()) < 100;
                    }, () => Standard.PlaySE("RevolverReload"),
                    () =>
                    {

                        Standard.PlayFadedSE("RocketFire", 1f);
                    });
                    Tester.Bullets_Max = 1;
                    Tester.ReloadTime = 200;
                    break;
                case 52: //샷건
                    GunSetter(500, 2f, Costume.GetCostume("Player_Ani_GUN01"), Costume.GetCostume("Player_Ani_GUN02"));
                    AttachGunEvent((Enemy_Center) =>
                    {
                        return Method2D.Distance(Enemy_Center, Cursor.GetPos()) < 200;
                    }, () => Standard.PlaySE("ShotgunReload"),
                    () =>
                    {
                        Standard.PlayFadedSE("ShotgunFire", 1f);
                        Standard.FadeAnimation(new DrawingLayer("GunFire", new Rectangle(Tester.player.GetCenter().X + 25, Tester.player.GetCenter().Y - 30, 50, 50)), 15, Color.NavajoWhite);
                    }
                    );
                    Tester.Bullets_Max = 2;
                    Tester.ReloadTime = 70;

                    break;
                case 53: //라이플
                    GunSetter(500, 2.5f, Costume.GetCostume("Player_Ani_RI01"), Costume.GetCostume("Player_Ani_RI02"));
                    AttachGunEvent((Enemy_Center) =>
                    {

                        return Method2D.Distance(Enemy_Center, Cursor.GetPos()) + Method2D.Distance(Enemy_Center, Tester.player.GetCenter()) < Method2D.Distance(Cursor.GetPos(), Tester.player.GetCenter()) + 500;
                    }, () => Standard.PlaySE("RifleReload"),
                    () =>
                    {
                        Standard.PlayFadedSE("RifleFire", 1f);
                        Standard.FadeAnimation(new DrawingLayer("RifleFireEffect", new Rectangle(Tester.player.GetCenter().X + 35, Tester.player.GetCenter().Y - 170, 200, 200)), 15, Color.NavajoWhite);
                    }

                        );

                    Tester.Bullets_Max = 1;
                    Tester.ReloadTime = 300;

                    break;
                case 54: //겨울총
                    GunSetter(500, 1f, Costume.GetCostume("Player_Ani_ME01"), Costume.GetCostume("Player_Ani_ME02"));
                    AttachGunEvent((Enemy_Center) =>
                    {

                        return Method2D.Distance(Enemy_Center, Cursor.GetPos()) < 200;
                    }, () => Standard.PlaySE("SantaReload"),
                    () =>
                    {
                        Standard.PlayFadedSE("FrozenFire", 1f);
                        Standard.FadeAnimation(new DrawingLayer("RifleFireEffect", new Rectangle(Tester.player.GetCenter().X + 35, Tester.player.GetCenter().Y - 170, 200, 200)), 15, Color.NavajoWhite);
                    }

                        );

                    Tester.Bullets_Max = 2;
                    Tester.ReloadTime = 150;

                    break;
                case -1:
                    Tester.ShotMode = false;
                    Tester.Bullets_Max = -1;
                    break;


            }

        }

        private static void GunSetter(int Range, double attackSpeed_coefficient, string Ani1, string Ani2)
        {
            Tester.player.setRange(Range);
            Checker.AttackSpeed_Coefficient_Weapon = attackSpeed_coefficient;
            if (Tester.ShotMode && !Tester.player.player.ContainsSprite(Ani1))
            {
                Tester.player.player.ClearAnimation();
                Tester.player.player.SetSprite(Ani1);
                Tester.player.player.AttachAnimation(30, Ani1, Ani2);
            }

        }




        private static void WeaponSetter(int Range, double attackSpeed_coefficient, string Ani1, string Ani2)
        {
            Tester.player.setRange(Range);
            Checker.AttackSpeed_Coefficient_Weapon = attackSpeed_coefficient;
            if (!Tester.ShotMode && !Tester.player.player.ContainsSprite(Ani1))
            {
                Tester.player.player.ClearAnimation();
                Tester.player.player.SetSprite(Ani1);
                Tester.player.player.AttachAnimation(30, Ani1, Ani2);
            }
        }

        public static void Equip_Weapon_Melee(int WeaponCode)
        {
            switch (WeaponCode)
            {
                case 17:
                    WeaponSetter(130, 1, Costume.GetCostume("Player_Ani11"), Costume.GetCostume("Player_Ani12"));
                    /*
                    Tester.player.setRange(130);
                    Checker.AttackSpeed_Coefficient_Weapon = 1;
                    Tester.player.player.ClearAnimation();
                    Tester.player.player.AttachAnimation(30, "Player_Ani11", "Player_Ani12");*/
                    break;
                case 12:
                    WeaponSetter(150, 1.10 / 1.0, Costume.GetCostume("Player_Ani_SH01"), Costume.GetCostume("Player_Ani_SH02"));
                    /*
                    Tester.player.setRange(150);
                    Checker.AttackSpeed_Coefficient_Weapon = 1.15/1.0;
                    Tester.player.player.ClearAnimation();
                    Tester.player.player.AttachAnimation(30, "Player_SH01", "Player_SH02");*/
                    break;
                case 13:
                    WeaponSetter(150, 1, Costume.GetCostume("Player_Ani_TH01"), Costume.GetCostume("Player_Ani_TH02"));
                    break;
                case 14:
                    WeaponSetter(1000, 0.9, Costume.GetCostume("Player_Ani_K01"), Costume.GetCostume("Player_Ani_K02"));
                    break;
                case 15:
                    WeaponSetter(120, 0.5, Costume.GetCostume("Player_Ani_S01"), Costume.GetCostume("Player_Ani_S02"));
                    break;
                case 16:
                    WeaponSetter(140, 1, Costume.GetCostume("Player_Ani_T01"), Costume.GetCostume("Player_Ani_T02"));
                    break;
                case 18:
                    WeaponSetter(145, 1.10 / 1.0, Costume.GetCostume("Player_Ani_CH01"), Costume.GetCostume("Player_Ani_CH02"));
                    break;



            }

        }
        //Knife=0;
        //Soul Harvester = 12;
        private static double attackSpeed_Coefficient_Swiftness;
        public static double AttackSpeed_Coefficient_Swiftness
        {
            get
            {
                return attackSpeed_Coefficient_Swiftness;
            }
            set
            {
                attackSpeed_Coefficient_Swiftness = value;
                SetPlayerAttackSpeed();

            }
        }
        private static double attackSpeed_Coefficient_Weapon;
        public static double AttackSpeed_Coefficient_Weapon
        {
            get
            {
                return attackSpeed_Coefficient_Weapon;
            }
            set
            {
                attackSpeed_Coefficient_Weapon = value;
                SetPlayerAttackSpeed();
            }
        }

        public static void SetPlayerAttackSpeed()
        {
            Tester.player.SetAttackSpeed((int)Math.Round(Default_AttackSpeed * AttackSpeed_Coefficient_Swiftness * AttackSpeed_Coefficient_Weapon));
        }


        //본 열거자의 이름은 실제 변수명과 일치해야 한다.(리플렉션 활용)
        public enum Status { Swiftness, Bloodthirst, Luck };


        public static void Init()
        {
            Checker.Hearts = 10;
            Checker.Swiftness = 0;
            Checker.Luck = 0;
            Checker.Bloodthirst = 0;
            Checker.BloodStack = 0;
            AttackSpeed_Coefficient_Swiftness = 1.0;
            AttackSpeed_Coefficient_Weapon = 1.0;
        }

        public static void GetHeart()
        {
            HeartStack++;
            HeartTimer = 30;
        }
        public static void GetHeart(int i)
        {
            HeartStack += i;
            HeartTimer = 30;
        }

        public static void Update()
        {
            if (LuckTimer > 0)
                LuckTimer--;
            if (Tester.ScoreStack != 0)
            {
                switch (Checker.Bloodthirst)
                {
                    case 1:
                        BloodStack += 0.01;
                        if (Weapon_Melee == 12)
                            BloodStack += 0.01;
                        break;
                    case 2:
                        BloodStack += 1.0 / 75;
                        if (Weapon_Melee == 12)
                            BloodStack += 1.0 / 75;
                        break;
                    case 3:
                        BloodStack += 0.02;
                        if (Weapon_Melee == 12)
                            BloodStack += 0.02;
                        break;

                }

                if (BloodStack >= 1.0)
                {
                    BloodStack = 0;
                    GetHeart();
                    Standard.PlaySE("Bloodthirst");
                    Standard.FadeAnimation(Tester.player.player, 30, Color.Red);
                    Tester.BuffBubble.Init("Bubble_Blood01");
                }

            }

        }

        public static void ShowStatus()
        {
            DrawingLayer Heart = new DrawingLayer("Heart", new Rectangle(50, 50, 60, 60));
            Color HeartColor = Color.DarkRed;
            int Hearts_5 = Checker.Hearts / 5;
            int LeftHearts = Checker.Hearts % 5;
            DrawingLayer gauge = new DrawingLayer("WhiteSpace", new Rectangle(100, 300, 10, (int)(300 * Tester.Gauge)));

            DrawingLayer Bullet;
            if (Checker.Weapon_Ranged != 53)
                Bullet = new DrawingLayer("BulletIcon", new Rectangle(Tester.player.player.GetCenter().X - 20, Tester.player.player.GetCenter().Y + 40, 30, 30));
            else
                Bullet = new DrawingLayer("SilverBullet", new Rectangle(Tester.player.player.GetCenter().X - 20, Tester.player.player.GetCenter().Y + 40, 40, 40));

            if (Tester.FreezeTimer < 0 && !Tester.ShowMenu && Tester.ShotMode)
            {
                for (int i = 0; i < Tester.Bullets; i++)
                {
                    Bullet.Draw();
                    Bullet.MoveByVector(new Point(1, 0), Bullet.GetBound().Width - 10);
                }
                Bullet.SetSprite("BulletEmptyIcon");
                for (int i = 0; i < Tester.Bullets_Max - Tester.Bullets; i++)
                {
                    Bullet.Draw();
                    Bullet.MoveByVector(new Point(1, 0), Bullet.GetBound().Width - 10);
                }


            }



            Standard.ViewportSwapDraw(new Viewport(MasterInfo.FullScreen), () =>
            {
                for (int i = 0; i < Hearts_5; i++)
                {
                    if ((Standard.FrameTimer + 25) % 60 < 30)
                        Heart.SetSprite("Heart5");
                    else
                        Heart.SetSprite("Heart5_2");

                    Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
                    if (Tester.FreezeTimer < 0)
                    {

                        Heart.Draw(Tester.FixedCamera, HeartColor);
                        Heart.Draw(Tester.FixedCamera, Color.White * 0.7f);
                    }
                    else
                        Heart.Draw(Tester.FixedCamera, HeartColor);
                }
                for (int i = 0; i < LeftHearts; i++)
                {
                    if ((Standard.FrameTimer + 25) % 60 < 30)
                        Heart.SetSprite("Heart");
                    else
                        Heart.SetSprite("Heart2");
                    Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
                    if (Tester.FreezeTimer < 0)
                    {

                        Heart.Draw(Tester.FixedCamera, HeartColor);
                        Heart.Draw(Tester.FixedCamera, Color.White * 0.7f);
                    }
                    else
                        Heart.Draw(Tester.FixedCamera, HeartColor);
                }
                if (Checker.HeartStack > 0)
                {
                    if (Checker.HeartTimer > 0)
                        Checker.HeartTimer--;
                    else
                    {
                        Checker.Hearts += Checker.HeartStack;
                        Checker.HeartStack = 0;
                    }
                    for (int i = 0; i < Checker.HeartStack; i++)
                    {
                        if (Checker.HeartTimer != 0)
                            Heart.SetSprite("HeartAni" + (30 - Checker.HeartTimer) / 6);
                        else
                            Heart.SetSprite("Heart");

                        Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
                        if (Tester.FreezeTimer < 0)
                        {

                            Heart.Draw(Tester.FixedCamera, HeartColor);
                            Heart.Draw(Tester.FixedCamera, Color.White * 0.7f);
                        }
                        else
                            Heart.Draw(Tester.FixedCamera, HeartColor);
                        Heart.Draw(Tester.FixedCamera, Color.Honeydew * (float)(Checker.HeartTimer / 8.0));

                    }
                }
                if (Tester.FreezeTimer > Tester.FreezeTime - 60)
                {

                    Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
                    Heart.SetSprite("Heart_Broken");
                    Heart.Draw(Tester.FixedCamera, HeartColor);
                }
                else if (Tester.FreezeTimer > Tester.FreezeTime - 110)
                {
                    Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
                    Heart.SetSprite("Heart_Broken2");
                    Heart.Draw(Tester.FixedCamera, HeartColor);
                }
                else if (Tester.FreezeTimer > 0)
                {
                    Heart.SetPos(Heart.GetPos().X + 80, Heart.GetPos().Y);
                    Heart.SetSprite("Heart_Broken3");
                    Heart.Draw(Tester.FixedCamera, HeartColor * (float)(Tester.FreezeTimer / (Tester.FreezeTime - 110.0)));
                }




                //Show Menual
                /*
                if (Tester.Room.Number == 0 && !Tester.IsEndPhase)
                {
                    DrawingLayer Menual = new DrawingLayer("Menual", new Point(800, 500), 0.75f);
                    if (Standard.FrameTimer % 60 > 30)
                        Menual.SetSprite("Menual2");
                    if (Standard.FrameTimer % 60 > 30)
                        Menual.Draw(Tester.FixedCamera, Color.White);
                    else
                        Menual.Draw(Tester.FixedCamera, Color.Goldenrod);
                }*/
                if(Tester.WeaponChangedTimer>0)
                {
                    Tester.WeaponChangedTimer--;
                    if (Tester.IsEndPhase)
                    {
                        DrawingLayer d = new DrawingLayer("Weapon_Instruction", new Point(130, 190), 1f);
                        d.Draw(Color.White * (Tester.WeaponChangedTimer / 20f));
                    }
                }





                //Show Buff Info
                InfoVector = new Vector2(150, 150);
                StringBackGround = new DrawingLayer("WhiteSpace", new Rectangle((int)(InfoVector.X - 15), (int)(InfoVector.Y - 10), 170, 35));
                ShowBuffString(Status.Luck);
                ShowBuffString(Status.Swiftness);
                ShowBuffString(Status.Bloodthirst);

                //Show Gauge
                gauge.Draw(Tester.FixedCamera, Color.AliceBlue);
                gauge.Draw(Tester.FixedCamera, Color.Red * (float)(1 - Tester.Gauge));

                int MainWeaponNumber;
                int SubWeaponNumber;
                int MainBoxNum;
                int SubBoxNum;

                if (Tester.ShotMode)
                {
                    MainWeaponNumber = Weapon_Ranged;
                    SubWeaponNumber = Weapon_Melee;
                    MainBoxNum = 2;
                    SubBoxNum = 1;
                }
                else
                {
                    MainWeaponNumber = Weapon_Melee;
                    SubWeaponNumber = Weapon_Ranged;
                    MainBoxNum = 1;
                    SubBoxNum = 2;
                }



                if (Tester.WeaponIconTimer > 0)
                    Tester.WeaponIconTimer--;


                Rectangle WeaponIconBound = new Rectangle(50, 200, 80, 80);
                DrawingLayer WeaponIcon = new DrawingLayer("WeaponIcon_" + MainWeaponNumber.ToString(), WeaponIconBound);
                Rectangle SmallWeaponIcon = new Rectangle(WeaponIcon.GetCenter(), new Point(60, 60));
                WeaponIcon.SetBound(SmallWeaponIcon);
                WeaponIcon.SetCenter(new Point(WeaponIconBound.Center.X, WeaponIconBound.Center.Y + 5));
                WeaponIcon.Draw(Tester.FixedCamera, Color.White);
                WeaponIcon = new DrawingLayer("IconBox" + MainBoxNum, WeaponIconBound);
                WeaponIcon.Draw(Tester.FixedCamera, Color.White);
                Rectangle WeaponIconLight = new Rectangle(WeaponIcon.GetBound().X, WeaponIcon.GetBound().Y + 5, WeaponIcon.GetBound().Width, WeaponIcon.GetBound().Height - 3);
                Standard.DrawLight(WeaponIconLight, Color.White, Tester.WeaponIconTimer / 50.0f, Standard.LightMode.Absolute);
                if (Weapon_Ranged != -1)
                {
                    Rectangle SubIconBound = new Rectangle(WeaponIcon.GetPos().X, WeaponIcon.GetPos().Y - WeaponIcon.GetBound().Height + 10, 60, 60);
                    WeaponIcon = new DrawingLayer("WeaponIcon_" + SubWeaponNumber.ToString(), new Rectangle(0, 0, 45, 45));
                    WeaponIcon.SetCenter(new Point(SubIconBound.Center.X, SubIconBound.Center.Y + 5));
                    WeaponIcon.Draw(Tester.FixedCamera, Color.White);
                    WeaponIcon.SetBound(SubIconBound);
                    WeaponIcon.SetSprite("IconBox" + SubBoxNum);
                    WeaponIcon.Draw(Tester.FixedCamera, Color.LightSkyBlue);
                }
            }

            );

        }

        public static void ShowBuffString(Status status)
        {
            Color color = Color.White;
            if (status == Status.Bloodthirst && Checker.weapon_Melee == 12 && !Tester.ShotMode)
                color = Color.Red;
            int checker = -1;
            Type type = typeof(Checker);
            FieldInfo FInfo = type.GetField(status.ToString());
            checker = (int)FInfo.GetValue(null);
            if (checker > 0)
            {
                string InfoString = "- " + status.ToString() + " ";
                for (int i = 0; i < checker; i++)
                {
                    InfoString = InfoString + "I";
                }
                StringBackGround.Draw(Tester.FixedCamera, Color.Black * 0.5f);
                Standard.DrawString(Tester.FixedCamera, InfoString, InfoVector, color * (float)(Math.Min(Standard.FrameTimer % 240, 240 - Standard.FrameTimer % 240) / 120.0 + 0.3f));
                InfoVector += new Vector2(0, 35);
                StringBackGround.SetBound(new Rectangle(StringBackGround.GetBound().X, StringBackGround.GetBound().Y + 35, StringBackGround.GetBound().Width, StringBackGround.GetBound().Height));

            }

        }

        public static bool LuckCheck()
        {
            if (LuckTimer > 0)
                return true;
            switch (Luck)
            {
                case 1:
                    if (Standard.Random() < 0.05)
                    {
                        /*
						Standard.FadeAnimation(Tester.player.player, 30, Color.Green);
						Standard.PlaySE("GetHeart");
                        Tester.BuffBubble.Init("Bubble_Luck01");
                        LuckTimer = 30;*/
                        Checker.LuckTimer = 30;
                        LuckAct = true;
                        return true;
                    }
                    break;
                case 2:
                    if (Standard.Random() < 0.10)
                    {
                        Checker.LuckTimer = 30;
                        LuckAct = true;

                        return true;
                    }

                    break;
                case 3:
                    if (Standard.Random() < 0.15)
                    {
                        Checker.LuckTimer = 30;
                        LuckAct = true;
                        return true;
                    }
                    break;
            }
            return false;
        }

    }
    public class Card
    {
        public DrawingLayer Frame;
        public int FlipTimer = -1;//-1:뒷면, 0:앞면, 1~30:까는중
        public int RemoveTimer = -1;//-1:파괴X,0이상:파괴타이머

        public string FrontFrameName;
        public string BackFrameName = "RewardCard";
        public static readonly int CardWidth = 140;
        public static readonly int FlipTime = 30;

        public enum CardClass { Reward };

        public delegate void CardOpenEvent(int EventNumber);
        public event CardOpenEvent CardOpenEvents;

        public static Dictionary<int, string> InfoTable = new Dictionary<int, string>();
        public static Dictionary<int, string> ExtraInfoTable = new Dictionary<int, string>();

        /* Card Index
		 * 0:Heart 1
		 * 1:Heart 2
		 * 2:Heart 3
		 * 3:Swiftness 1
		 * 4:Swiftness 2
		 * 5:Swiftness 3
		 * 6:Bloodthirst 1
		 * 7:Bloodthirst 2
		 * 8:Bloodthirst 3
		 * 9:Luck 1 
		 * 10:Luck 2
		 * 11:Luck 3
		 * */

        public static void Init()
        {
            InfoTable.Add(0, "Heart 1$Get 1 Heart.$There was a red bowel\n stuffed with warmness.");
            InfoTable.Add(1, "Heart 2$Get 2 Heart.$There were red bowels\n stuffed with warmness.");
            InfoTable.Add(2, "Heart 3$Get 3 Heart.$There were red bowels\n stuffed with warmness.");
            InfoTable.Add(3, "Swiftness 1$Speed+10%, Attack Speed+7%$This book eases the burden of life.");
            InfoTable.Add(4, "Swiftness 2$Speed+20%, Attack Speed+15%$This book eases the burden of life.");
            InfoTable.Add(5, "Swiftness 3$Speed+30%, Attack Speed+25%$This book eases the burden of life.");
            InfoTable.Add(6, "Bloodthirst 1$Get 1 heart when you kill 100 enemies.$The cursed book turns people\n into the blood-starved beast.");
            InfoTable.Add(7, "Bloodthirst 2$Get 1 heart when you kill 75 enemies.$The cursed book turns people\n into the blood-starved beast.");
            InfoTable.Add(8, "Bloodthirst 3$Get 1 heart when you kill 50 enemies.$The cursed book turns people\n into the blood-starved beast.");
            InfoTable.Add(9, "Luck 1$Chance of avoiding death: 5%$A little clover was put in a book.");
            InfoTable.Add(10, "Luck 2$Chance of avoiding death: 10%$Little clovers were put in a book.");
            InfoTable.Add(11, "Luck 3$Chance of avoiding death: 15%$Little clovers were put in a book.");
            InfoTable.Add(12, "Soul Harvester$Range+2, AttackSpeed-10%\n\n <Blood Trail> :\n Blood follows Ortolan.\n Get doubled bloodthirst effect.$The scythe was soaked in blood, crying.");
            InfoTable.Add(13, "Thorn Whip$Range+2.$It's looked like not so much weapon,\n but rather an instrument of torture.");
            InfoTable.Add(14, "Yomi$Range+999. \n\n <Shadow Hunting> :\n Ortolan Disappears in the darkness.\nTake the monster's position \nafter killing that enemy. $The sword slightly better than \na kitchen knife.");
            InfoTable.Add(15, "Moonlight$Range-1.\n\n<Sheer Celerity> :\n Get doubled attack-speed. $The sword reminded moonlight,\n as shining itself.");
            InfoTable.Add(16, "The World$Range+1.\n\n<Time Stop> :\n Enemies stop while using overclock. $Argent, mystic material was flowing\n inside the sword. ");
            InfoTable.Add(17, "Knife$Default Weapon.$A usual kitchen knife.");
            InfoTable.Add(18, "Yume Diary$Range+1, AttackSpeed-10%\n\n <Yume Drive> :\n While using overclock, \nAttack-speed and Move-speed bulid up. $Dear Yume, you were always with me.\n I love you, Sincerely.");
            InfoTable.Add(50, "Fancy Roulette$6 bullets. Fast reloading. Small hit-range. \n\nYou can swap weapon by 1,2 keypad\n or mouse wheel.$This was not edible for them.");
            InfoTable.Add(51, "Kessler Syndrome$1 bullshit. not Fast reloading. Small hit-range. \n\nYou can swap weapon by 1,2 keypad\n or mouse wheel.$The most greatest legacy of the \ngreatest scientist.");
            InfoTable.Add(52, "Good Negociator$2 bullets. Medium reloading. Medium hit-range. \n\nYou can swap weapon by 1,2 keypad\n or mouse wheel.$this was not edible.");
            InfoTable.Add(53, "PeaceMaker$1 bullet. Very slow reloading. Very large hit-range. \n\nYou can swap weapon by 1,2 keypad\n or mouse wheel.$And then there were none.");
            InfoTable.Add(54, "Merry Christmas$2 bullet. Slow reloading. large hit-range. \n\n <Winter is Coming!> :\n The gun freezes" +
                " all creatures in its hit-range. \n\nYou can swap weapon by 1,2 keypad\n or mouse wheel.$This was concealed under\n the ridiculous red hat.");
            InfoTable.Add(100, "Dear Ortolan$$Ortolan, we have a party tonight.\n\nA lot of delicacies and amusements " +
                "\n\nare waiting for you.\n\nPlease come upstairs if you are \n\n ready to enjoy it.\n\n\n Sincerely, Your Best Friend");
            InfoTable.Add(101, "Dear Ortolan$$Ortolan, why were you being\n so rude to our friends?\n\n We did not have any malice. \n\n Please come upstairs without \nyour dangerous knife.\n\n\nSincerely, Your Worried Friend");
            InfoTable.Add(102, "Dear Ortolan$$Ortolan, You might have some issues.\n\nCalm down and take a rest, Please.\n\n I would not recommend you to come here\n\n with that hideous weapons.\n\n\n Sincerely, Your Gentle Friend");
            InfoTable.Add(103, "Dear Ortolan$$Ortolan, I think you don't know how\n\n to deal with your friend.\n\n Friends want to eat you. \n\nWe know that you're trying to escape, \n\nbut that makes us much happier.   \n\n\nSincerely, Your Predators ");
            InfoTable.Add(104, "Dear Delicious Meat$$DIE ");
            InfoTable.Add(105, "Dear Precious Meat$$Delicious Meat was delicious meat.\n\nDelicious heart was delicious heart.\n\nDelicious body was delicious body.\n\nDelicacy was delicacy.\n\nA lot of Delicacies are waiting for us. ");
            InfoTable.Add(106, "Dear Ortolan$$I don't understand how could this happen.\n\n You're just a little child.\n\nEven a lot of dungeon adventurers \n\ncouldn't survive us.\n\nWhat's happening to you? Let us know. \n\n\n From, Your Concerned Friend");
            InfoTable.Add(107, "Dear Ortolan$$You don't have to be with us anymore.\n\nWe will let you do all you want.\n\n So, just one thing--don't come upstairs.\n\n\n Sincerely, Your Best Ever Friend");
            InfoTable.Add(108, "Dear Ortolan$$I will tell you about the truth.\n\nThere is no way to go outside.\n\nThis dungeon is underground cavern. \n\n That's why couldn't you see other people.\n\n Killing us is not a solution. \n\n Let's live with each other.");
            InfoTable.Add(109, "Dear....$$You've done a lot to us.\n\n At first you were designed as a meat, \n\nbut now you've survived this far.\n\nWell, we've gathered all together.\n\nLet's dance with each other, a last dance.");
            InfoTable.Add(666, "Blood Oath$You sacrifice 3 hearts and get 1 item.$This is my pure love.");
            ExtraInfoTable.Add(666, "Blood Oath$You sacrifice 3 hearts and get 1 item.$You don't have enough hearts. idiot.");
           

        }

        public Card(int Number, CardClass WhatClass, Point CenterPos)
        {
            BackFrameName = WhatClass.ToString() + "Card";
            Frame = new DrawingLayer(BackFrameName, Tester.CardPos, 0.75f);
            Frame.SetCenter(CenterPos);
            FrontFrameName = WhatClass.ToString() + "Card_" + Number;
            if (IsWeapon())
                BackFrameName = WhatClass.ToString() + "Card" + "_Weapon";
            if (GetIndex() >= 3 && GetIndex() <= 11)
            {
                BackFrameName = WhatClass.ToString() + "Card" + "_Enchant";
            }
            if (Number == 666)
            {
                BackFrameName = WhatClass.ToString() + "Card" + "_BloodOath";
            }

            if (WhatClass == CardClass.Reward)
                CardOpenEvents += CardRewardEvent;
        }

        public Card(int Number, CardClass WhatClass) : this(Number, WhatClass, new Point(Tester.CardPos.X + 800, Tester.CardPos.Y))
        {
            /*
            BackFrameName = WhatClass.ToString() + "Card";
            Frame = new DrawingLayer(BackFrameName, Tester.CardPos, 0.75f);
            Frame.SetCenter(new Point(Tester.CardPos.X + 800, Tester.CardPos.Y));
            FrontFrameName = WhatClass.ToString() + "Card_" + Number;
            if (IsWeapon())
                BackFrameName = WhatClass.ToString() + "Card" + "_Weapon";
            if (GetIndex() >= 3 && GetIndex() <= 11)
            {
                BackFrameName = WhatClass.ToString() + "Card" + "_Enchant";
            }
            if(Number==666)
            {
                BackFrameName = WhatClass.ToString() + "Card" + "_BloodOath";
            }

            if (WhatClass == CardClass.Reward)
                CardOpenEvents += CardRewardEvent;*/
        }
        public bool isOpened()
        {
            if (FlipTimer < FlipTime / 2 && FlipTimer != -1)
                return true;
            else
                return false;
        }

        public int GetIndex()
        {
            return Int32.Parse(FrontFrameName.Substring(11));
        }
        public void Open()
        {
            FlipTimer = FlipTime;
        }
        public bool IsWeapon()
        {
            return IsWeapon(GetIndex());
        }
        public static bool IsWeapon(int i)
        {
            return (i >= 12) && (i <100);
        }
        public bool IsWeaponMelee()
        {
            return IsWeapon() && GetIndex() < 50;
        }
        public bool IsWeaponRanged()
        {
            return IsWeapon() && GetIndex() >= 50;
        }

        public void Update()
        {
            if (FlipTimer > 0)
            {
                FlipTimer--;
                Point p = Frame.GetCenter();
                Frame.SetBound(new Rectangle(0, 0, Math.Abs((int)(CardWidth * Math.Cos(Math.PI * (FlipTime - FlipTimer) / (FlipTime)))), Frame.GetBound().Height));
                Frame.SetCenter(p);
            }
            if (RemoveTimer > 0)
            {
                RemoveTimer--;

            }
            if (FlipTimer == 1 && CardOpenEvents != null)
            {
                if (!IsWeapon()&&GetIndex()!=666)
                    CardOpenEvents(GetIndex());
            }

            if (Cursor.JustdidLeftClick(Frame) && FlipTimer == -1)
            {
                Open();
                Standard.PlaySE("CardHandOver");
            }
            if (Cursor.JustdidLeftClick(Frame) && FlipTimer == 0 && IsWeapon()) //Weapon 카드를 열었을 때.
            {

                Standard.PlaySE("Reload");
                if (IsWeaponMelee())
                {
                    Tester.ShotModeBuffer = false;
                    int temp = Checker.Weapon_Melee;
                    Checker.Weapon_Melee = GetIndex();
                    if(GetIndex()==12&& Checker.Bloodthirst < 1)
                            Checker.Bloodthirst = 1;
                    FrontFrameName = "Reward" + "Card_" + temp;
                    Tester.WeaponIconTimer = 30;
                }
                else if (IsWeaponRanged() && Checker.Weapon_Ranged == -1)
                {
                    Tester.ShotModeBuffer = true;
                    Checker.Weapon_Ranged = GetIndex();
                    RemoveTimer = 30;
                    Tester.WeaponIconTimer = 30;
                    Tester.WeaponChangedTimer = 300;
                }
         
                else
                {
                    Tester.ShotModeBuffer = true;
                    int temp = Checker.Weapon_Ranged;
                    Checker.Weapon_Ranged = GetIndex();
                    FrontFrameName = "Reward" + "Card_" + temp;
                    Tester.WeaponIconTimer = 30;

                }
                return;
            }
            if (Cursor.JustdidLeftClick(Frame) && FlipTimer == 0 && GetIndex()==666) //BloodOath
            {

                if(Checker.Hearts>3)
                {
                    FlipTimer = -1;
                    CardOpenEvents(666);

                    Standard.PlayFadedSE("KnifeSound", 0.4f);
                    Standard.PlayFadedSE("GunSound", 0.3f);
                    ParticleEngine.GenerateBlood(Tester.player.player.GetPos());
                }

                return;
            }


            if (Cursor.JustdidLeftClick(Frame) && FlipTimer == 0 && RemoveTimer == -1)
            {
                RemoveTimer = 30;
                if (GetIndex() == 100)
                {
                    if (Tester.LetterNum <= 3)
                        Tester.AddReward(666);
                    switch (Tester.LetterNum)
                    {
                        case 0:
                            Tester.Monolog.RandomAttach("Fuck you..");
                            break;
                        case 1:
                            Tester.Monolog.RandomAttach("Shut up, Pig...");                            
                            break;
                        case 2:
                            Tester.Monolog.RandomAttach("Oh, I found a trash.");
                            break;
                        case 3:
                            Tester.Monolog.RandomAttach("Ha, now you're joking.");
                            break;
                        case 4:
                            Tester.Monolog.RandomAttach("Very simple, Huh?");
                            break;
                        case 5:
                            Tester.Monolog.RandomAttach("Yeah, you've done a lot to me.");
                            break;
                        case 6:
                            Tester.Monolog.RandomAttach("..You would never know.");
                            break;
                        case 7:
                            Tester.Monolog.RandomAttach("This makes me even pleased.");
                            break;
                        case 8:
                            Tester.Monolog.RandomAttach("Well, then I'll do  killing for my pleasure.");
                            break;
                        case 9:
                            Tester.Monolog.RandomAttach("This is not last for me. This is ENDLESS");
                            break;



                    }
                    if (Tester.ShotMode)
                    {
                        Checker.GunFireEvent();
                    }
                    else
                    {
                        Standard.PlaySE("Trash");
                    }
  
                }
            }

        }


        public void CardRewardEvent(int EventNum)
        {
            Standard.FadeAnimation(new DrawingLayer("WhiteSpace", Frame.GetBound()), 30, Color.PaleGoldenrod);
            Standard.PlaySE("GetHeart");

            switch (EventNum)
            {
                case 0:
                    Checker.GetHeart();
                    Tester.Monolog.RandomAttach("I could feel my heartbeat.", "...It's warm.", "My blood is flowing.", "Could I live more?");
                    break;
                case 1:
                    Checker.GetHeart(2);
                    Tester.Monolog.RandomAttach("I could feel my heartbeat.", "...It's warm.", "My blood is flowing.", "Could I live more?");
                    break;
                case 2:
                    Checker.GetHeart(3);
                    Tester.Monolog.RandomAttach("I could feel my heartbeat.", "...It's warm.", "My blood is flowing.", "Could I live more?");
                    break;
                case 3:
                    if (Checker.Swiftness < 1)
                        Checker.Swiftness = 1;
                    //Tester.player.SetAttackSpeed(14);
                    Checker.AttackSpeed_Coefficient_Swiftness = 14.0 / 15.0;
                    Tester.Monolog.RandomAttach("Everything looks much slower.", "Feather-light.");
                    break;
                case 4:
                    if (Checker.Swiftness < 2)
                        Checker.Swiftness = 2;
                    //Tester.player.SetAttackSpeed(13);
                    Checker.AttackSpeed_Coefficient_Swiftness = 13.0 / 15.0;
                    Tester.Monolog.RandomAttach("Everything looks much slower.", "Feather-light.");
                    break;
                case 5:
                    if (Checker.Swiftness < 3)
                        Checker.Swiftness = 3;
                    //Tester.player.SetAttackSpeed(12);
                    Checker.AttackSpeed_Coefficient_Swiftness = 12.0 / 15.0;
                    Tester.Monolog.RandomAttach("Everything looks much slower.", "Feather-light.");
                    break;
                case 6:
                    if (Checker.Bloodthirst < 1)
                        Checker.Bloodthirst = 1;
                    Tester.Monolog.RandomAttach("I felt some URGE inside me.", "It desires me to kill.");
                    break;
                case 7:
                    if (Checker.Bloodthirst < 2)
                        Checker.Bloodthirst = 2;
                    Tester.Monolog.RandomAttach("I felt some URGE inside me.", "It desires me to kill.");
                    break;
                case 8:
                    if (Checker.Bloodthirst < 3)
                        Checker.Bloodthirst = 3;
                    Tester.Monolog.RandomAttach("I felt some URGE inside me.", "It desires me to kill.");
                    break;
                case 9:
                    if (Checker.Luck < 1)
                        Checker.Luck = 1;
                    Tester.Monolog.RandomAttach("I found a clover.", "Is this helpful?");
                    break;
                case 10:
                    if (Checker.Luck < 2)
                        Checker.Luck = 2;
                    Tester.Monolog.RandomAttach("I found a clover.", "Is this helpful?");
                    break;
                case 11:
                    if (Checker.Luck < 3)
                        Checker.Luck = 3;
                    Tester.Monolog.RandomAttach("I found a clover.", "Is this helpful?");
                    break;
       
                case 666:
                    if (Checker.Hearts>3)
                    {
                        Checker.Hearts -= 3;
                        Tester.AddReward();
                    }
                    Tester.Monolog.RandomAttach("It hurts...");
                    break;


            }
            if (IsWeaponMelee())
            {
                Checker.Weapon_Melee = EventNum;
    

            }
            if (IsWeaponRanged())
                Checker.Weapon_Ranged = EventNum;
        }
        public void Draw()
        {
            if (RemoveTimer < 30 && RemoveTimer > 5)
            {
                Frame.SetSprite("CardCrash" + (6 - RemoveTimer / 5));
                Frame.Draw();
            }
            else if (RemoveTimer <= 5 && RemoveTimer >= 0)
            {
                Frame.SetSprite("EmptySpace");
                Frame.Draw();
            }
            else if (isOpened())
            {
                Frame.SetSprite(FrontFrameName);
                Frame.Draw();
                Standard.DrawAddon(Frame, Color.White, 1f, "CardFrame");
                if (IsWeapon() && Cursor.IsOn(Frame))
                    Standard.DrawLight(Frame, Color.AliceBlue, 0.5f, Standard.LightMode.Absolute);
            }
            else
            {
                Frame.SetSprite(BackFrameName);
                Frame.Draw();

            }
            /*
			if(Cursor.IsOn(Frame)&&FlipTimer==0)
			{
                string s;
                if(InfoTable.ContainsKey(GetIndex()))
                {
                    s = InfoTable[GetIndex()];
                }
                else
                {
                    s = "Preparing...";
                }
                Standard.ViewportSwapDraw(new Viewport(MasterInfo.FullScreen),
                    () =>
                    {
                        DrawingLayer InfoFrame = new DrawingLayer("WhiteSpace", new Rectangle(550,150, 500, 500));
                        InfoFrame.Draw(Tester.FixedCamera, Color.Black*0.5f);
                        Standard.DrawString(s, new Vector2(InfoFrame.GetPos().X+50, InfoFrame.GetPos().Y + 40), Color.White);
                    });

 			}*/


        }
    }

    public static class CardInfoUI
    {
        public static DrawingLayer InfoFrame = new DrawingLayer("WhiteSpace", new Rectangle(550, 150, 500, 500));
        public static string InfoString;
        public static int CardIndex;
        public static void Draw()
        {
            if(CardIndex<100||CardIndex==666)
            {
                if (Card.InfoTable.ContainsKey(CardIndex))
                {
                    //InfoTable.Add(0, "Heart 1: \n\nGet 1 Heart.\n\n\n\n「There was a red bowel\n stuffed with warmness.」");
                    //InfoTable.Add(0, "Heart 1%Get 1 Heart.%There was a red bowel\n stuffed with warmness.);

                    string[] s = Card.InfoTable[CardIndex].Split('$');
                    if (CardIndex == 666 && Checker.Hearts <= 3)
                        s = Card.ExtraInfoTable[CardIndex].Split('$');
           
                    if (s.Length <= 2)
                        InfoString = Card.InfoTable[CardIndex];
                    else
                        InfoString = s[0] + ": \n\n" + s[1] + "\n\n\n\n「" + s[2] + "」";
                }
                else
                {
                    InfoString = "Preparing...";
                }
            }
            else
            {
                if (Card.InfoTable.ContainsKey(CardIndex+Tester.LetterNum))
                {
                    //InfoTable.Add(0, "Heart 1: \n\nGet 1 Heart.\n\n\n\n「There was a red bowel\n stuffed with warmness.」");
                    //InfoTable.Add(0, "Heart 1%Get 1 Heart.%There was a red bowel\n stuffed with warmness.);

                    string[] s = Card.InfoTable[CardIndex + Tester.LetterNum].Split('$');
                    if (s.Length <= 2)
                        InfoString = Card.InfoTable[CardIndex + Tester.LetterNum];
                    else
                        InfoString = s[0] + ": \n\n" + s[1] + "\n\n\n\n「" + s[2] + "」";
                }
                else
                {
                    InfoString = "Preparing...";
                }

            }
            Standard.ViewportSwapDraw(new Viewport(MasterInfo.FullScreen),
                   () =>
                   {
                       InfoFrame.Draw(Tester.FixedCamera, Color.Black * 0.5f);
                       Standard.DrawString(InfoString, new Vector2(InfoFrame.GetPos().X + 50, InfoFrame.GetPos().Y + 40), Color.White);
                       if(CardIndex==666)
                           Standard.DrawString(InfoString, new Vector2(InfoFrame.GetPos().X + 50, InfoFrame.GetPos().Y + 40), Color.DarkRed*0.6f);
                       if (Card.IsWeapon(CardIndex))
                       {
                           Standard.DrawString("To equip the weapon, Left-click the card. ", new Vector2(InfoFrame.GetPos().X + 50, InfoFrame.GetPos().Y + 400), Color.White);
                           Standard.DrawString("To equip the weapon, Left-click the card. ", new Vector2(InfoFrame.GetPos().X + 50, InfoFrame.GetPos().Y + 400), Color.Red * (Math.Min(Standard.FrameTimer % 60, 60 - Standard.FrameTimer % 60) / 30.0f));
                          
                       }
                       else if(CardIndex<100)
                       {
                           Standard.DrawString("This was activated when it was opened. ", new Vector2(InfoFrame.GetPos().X + 50, InfoFrame.GetPos().Y + 400), Color.White);
                           Standard.DrawString("This was activated when it was opened. ", new Vector2(InfoFrame.GetPos().X + 50, InfoFrame.GetPos().Y + 400), Color.Red * (Math.Min(Standard.FrameTimer % 60, 60 - Standard.FrameTimer % 60) / 30.0f));
                       }
                       else if(CardIndex==666)
                       {
                           Standard.DrawString("OPEN IT! ", new Vector2(InfoFrame.GetPos().X + 50, InfoFrame.GetPos().Y + 400), Color.White);
                           Standard.DrawString("OPEN IT! ", new Vector2(InfoFrame.GetPos().X + 50, InfoFrame.GetPos().Y + 400), Color.Red * (Math.Min(Standard.FrameTimer % 60, 60 - Standard.FrameTimer % 60) / 30.0f));
                       }
                   });
        }


    }


    public static class Table
    {
        public static Dictionary<int, double> ValueTable = new Dictionary<int, double>();
        public static Dictionary<int, double> DropTable = new Dictionary<int, double>();
        public static double m = 0;//parameter

        /*Card Index
		 * 0:Heart 1
		 * 1:Heart 2
		 * 2:Heart 3
		 * 3:Swiftness 1
		 * 4:Swiftness 2
		 * 5:Swiftness 3
		 * 6:Bloodthirst 1
		 * 7:Bloodthirst 2
		 * 8:Bloodthirst 3
		 * 9:Luck 1 
		 * 10:Luck 2
		 * 11:Luck 3
		 * */

        public static void Init()
        {
            ValueTable.Clear();
            DropTable.Clear();
            //Card 추가는 여기에서. Add Card.
            ValueTable.Add(0, 1.5);
            ValueTable.Add(1, 3);
            ValueTable.Add(2, 5);
            ValueTable.Add(3, 7);
            ValueTable.Add(4, 14);
            ValueTable.Add(5, 21);
            ValueTable.Add(6, 10);
            ValueTable.Add(7, 20);
            ValueTable.Add(8, 30);
            ValueTable.Add(9, 5);
            ValueTable.Add(10, 10);
            ValueTable.Add(11, 15);
            ValueTable.Add(12, 50);
            ValueTable.Add(13, 15);
            ValueTable.Add(14, 40);//7
            ValueTable.Add(15, 55);
            ValueTable.Add(16, 100);
            ValueTable.Add(50, 0.01);//권총
            ValueTable.Add(51, 40);//로켓런처 40
            ValueTable.Add(52, 25);//샷건
            ValueTable.Add(53, 60);//피스메이커
            ValueTable.Add(54, 60);//얼음총
            ValueTable.Add(18, 35);//전기톱 35

            foreach (KeyValuePair<int, double> v in ValueTable)
            {
                DropTable.Add(v.Key, v.Value);
            }
            UpdateM();
        }

        public static void RemoveDrop(int key)
        {
            if (DropTable.ContainsKey(key))
            {
                DropTable.Remove(key);
            }
            UpdateM();

            int x = 2, y = 3;

            if (x != y)
            {
                x += 1;
                y += 2;
            }
            else
                x -= y;

        }
        public static void UpdateM()
        {
            m = 0;
            foreach (KeyValuePair<int, double> v in DropTable)
            {
                m += (1.0 / v.Value);
            }
        }
        public static int Pick()
        {
            double k = Standard.Random();
            double counter = 0;
            foreach (KeyValuePair<int, double> v in DropTable)
            {
                double temp = counter;
                counter += (1.0 / v.Value) / m;
                if (k >= temp && k < counter)
                {
                    switch (v.Key)
                    {
                        case 3:
                            RemoveDrop(3);
                            break;
                        case 4:
                            RemoveDrop(3);
                            RemoveDrop(4);
                            break;
                        case 5:
                            RemoveDrop(3);
                            RemoveDrop(4);
                            RemoveDrop(5);
                            break;
                        case 6:
                            RemoveDrop(6);
                            break;
                        case 7:
                            RemoveDrop(6);
                            RemoveDrop(7);
                            break;
                        case 8:
                            RemoveDrop(6);
                            RemoveDrop(7);
                            RemoveDrop(8);
                            break;
                        case 9:
                            RemoveDrop(9);
                            break;
                        case 10:
                            RemoveDrop(9);
                            RemoveDrop(10);
                            break;
                        case 11:
                            RemoveDrop(9);
                            RemoveDrop(10);
                            RemoveDrop(11);
                            break;
                    }
                    if (v.Key >= 12)
                        RemoveDrop(v.Key);

                    return v.Key;
                }
            }
            return -1;
        }



    }

    public class SafeInt
    {
        private int rnd;
        private int InternalVar;
        private int InternalVar2;

        public SafeInt()
        {
            rnd = new Random().Next(100, 500);
            rnd = rnd * (34 + rnd / 100);
            Standard.AttachTickEvent(Vibe);
        }
        ~SafeInt()
        {
            Standard.DetachTickEvent(Vibe);
        }
        public SafeInt(int n) : this()
        {
            var = n;
        }
        public int var
        {
            get
            {
                return InternalVar + InternalVar2;
            }
            set
            {
                if (var != value)
                {
                    rnd++;
                    if (rnd > 10000000)
                        rnd /= 2;
                    InternalVar = value - rnd;
                    InternalVar2 = rnd;
                }
            }
        }


        public override string ToString()
        {
            return var.ToString();
        }

        public void Vibe(object sender, ElapsedEventArgs g)
        {
            InternalVar++;
            InternalVar2--;
            //Console.WriteLine(InternalVar + "," + InternalVar2);
        }
    }



    /// <summary>
    ///  실험적인 클래스.
    ///  매 루프마다 0,1이 바뀌어가며 점멸하는 스위치입니다.
    ///  게임내 상황이 변할 때(ex: 게임오버 상황) 변하는 부울값을 찾아서 스위치를 무력화시키는 방법이 있는데,
    ///  이 스위치는 매 프레임마다 점멸하기 때문에 실제로 의미를 가지는 값이 변화하는 지점을 찾기가 어렵습니다.
    ///  게임내 중요 스위치를 관리할 때 쓸 수 있습니다.
    ///  기존 부울변수에 비해 6바이트 비싸고, 게터, 세터에서 플립이 발생하기 때문에 매 루프당 계산 손실이 있긴 합니다. 
    ///  하지만 중요한 스위치들에는 충분히 적용가능합니다.
    ///  메모리 스캐닝 시도 자체를 무력화할 수 있습니다.
    /// </summary>
    public class SafeBool
    {
        private bool Bool = true;
        private bool isoBool = true;
        private int InternalTimer;

        public SafeBool(bool b)
        {
            Set(b);
        }

        public void Set(bool b)
        {
            Flip();
            if (b)
            {
                isoBool = Bool;
            }
            else
            {
                isoBool = !Bool;
            }
        }

        public bool Get()
        {
            Flip();
            return (Bool == isoBool);
        }

        private void Flip()
        {
            if (InternalTimer != Standard.FrameTimer)
            {
                Bool = !Bool;
                isoBool = !isoBool;
                InternalTimer = Standard.FrameTimer;
            }
        }



    }

    public static class GameConsole
    {
        public static string Buffer = "";
        public static List<string> DisposedBuffer;
        public static bool ConsoleMode = false;
        public static int BackSpaceTimer = 0;

        public static void ProcessBuffer()
        {
            string[] s = Buffer.Split('-');
            if (s.Length >= 2 && s[0] == "w")
            {
                int WeaponNum = Int32.Parse(s[1]);
                if (Table.ValueTable.ContainsKey(WeaponNum) && Card.IsWeapon(WeaponNum))
                {
                    if (WeaponNum < 50)
                        Checker.Weapon_Melee = WeaponNum;
                    else
                        Checker.Weapon_Ranged = WeaponNum;
                }
            }
            if (s.Length >= 2 && s[0] == "h")
            {
                int HeartNum = Int32.Parse(s[1]);
                Checker.Hearts = HeartNum;
            }
            if (s.Length >= 2 && s[0] == "b")
            {
                int BNum = Int32.Parse(s[1]);
                Checker.Bloodthirst = BNum;
            }

            if (s.Length >= 2 && s[0] == "c")
            {
                Costume.CostumeNumber = Int32.Parse(s[1]);
            }
            if (s.Length >= 2 && s[0] == "bw")
            {
                HeartShop.AddAccount("BW/" + Int32.Parse(s[1]));
            }
        }

        public static void Update()
        {
            if (Standard.JustPressed(Keys.Enter))
            {
                ConsoleMode = !ConsoleMode;
                if (Buffer != "")
                {
                    ProcessBuffer();
                }
                Buffer = "";
            }
            if (ConsoleMode)
            {
                Keys[] ks = Keyboard.GetState().GetPressedKeys();
                if (Standard.Pressing(Keys.Back))
                {
                    BackSpaceTimer++;
                }
                else
                {
                    BackSpaceTimer = 0;
                }
                foreach (Keys k in ks)
                {
                    if (Standard.JustPressed(k))
                    {
                        if (k.ToString().Length == 1)
                        {
                            if (k.ToString()[0] <= 'Z' && k.ToString()[0] >= 'A')
                            {
                                Buffer += k.ToString().ToLower();
                            }
                        }

                        if (k == Keys.OemMinus)
                            Buffer += "-";

                        if (k.ToString().Length == 2 && k.ToString()[0] == 'D')
                        {
                            Buffer += k.ToString()[1];
                        }
                    }

                    if ((k == Keys.Back && BackSpaceTimer >= 15) || Standard.JustPressed(Keys.Back))
                    {
                        if (Buffer.Length >= 1)
                        {
                            Buffer = Buffer.Substring(0, Buffer.Length - 1);
                        }
                    }
                }
            }
        }

        public static void Draw()
        {
            if (ConsoleMode)
            {
                Standard.ViewportSwapDraw(new Viewport(MasterInfo.FullScreen), () =>
                     {
                         Standard.DrawString(Tester.FixedCamera, ">" + Buffer, new Vector2(300, 500), Color.Blue);
                     });
            }
        }



    }

    public static class HeartShop
        {
        public static int HeartCoin
        {
            get {
                int value=0;
                ReadAccount("M", (i) => { value += i; }, () => { value = -1; });       
                return value;
            }
        }


        public static void InitWeapon()
        {
            ReadAccount("BW", (i) => { Tester.AddReward(i); }, () => { return; });

        }

        public static string AccountPath = "HeartAccount.txt";
        private static string Seed = "OrtolanIsGreat!_"+System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        private static byte[] SHA256_toByte(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string SHA256_toString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in SHA256_toByte(inputString))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        private static bool Validate()
        {
            string[] GFH = GetNormalFileHash();
            if (GFH[0] == GFH[1])
                return true;
            return false;
        }

        private static string Hash(string s)
        {
            return SHA256_toString(s);
        }

        private static string[] GetNormalFileHash()
        {
            List<string> sList = File.ReadAllLines(AccountPath).ToList();
            string seed = Seed; 
            if(sList.Count < 1)
            {
                string[] ss = { "Invalid", "-1" };
                return ss;
            }
            if(sList.Count == 2)
            {
                sList[0] = Hash(sList[0] + seed);                
            }
            else if(sList.Count>2)
            {
                while (sList.Count > 2)
                {
                    seed = Hash(seed);
                    sList[0] = Hash(sList[0] + seed + sList[1]);// Merkle-Damgard
                    sList.RemoveAt(1);
                }
            }
            string[] s = { sList[0], sList[1] };
            return s;
        }

        private static string GetFileHash()
        {
            List<string> sList = File.ReadAllLines(AccountPath).ToList();
            string seed = Seed;
            if(sList.Count==1)
            {
                sList[0] = Hash(sList[0] + seed);
            }
            else
            {
            while (sList.Count > 1)
            {
                seed = Hash(seed);
                sList[0] = Hash(sList[0] + seed + sList[1]);// Merkle-Damgard
                sList.RemoveAt(1);
            }

            }
            return sList[0];
        }

        // M/숫자 : HeartCoin 증감.
        // BW/숫자 : Weapon 구입
        // BC/숫자 : Costume 구입

        public static void ReadAccount(string Header, Action<int> ReadAct, Action ErrorAct)
        {
            if (Validate())
            {
                List<string> sList = File.ReadAllLines(AccountPath).ToList();
                for (int i = 0; i < sList.Count - 1; i++)
                {
                    string[] s = sList[i].Split('/');
                    if (s[0] == Header)
                    {
                        ReadAct(Int32.Parse(s[1]));
                    }
                }
            }
            else
            {
                ErrorAct();
            }

        
        }
        public static void AddAccount(string Account)
        {

            string s = Account + "/"+DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            if (!File.Exists(AccountPath))
            {
                File.Create(AccountPath).Close();
            }
            if (Validate())
            {
                var lines = System.IO.File.ReadAllLines(AccountPath);
                System.IO.File.WriteAllLines(AccountPath, lines.Take(lines.Length - 1).ToArray());//마지막 줄 자르기
                using (StreamWriter outputFile = new StreamWriter(AccountPath, true))
                {
                    outputFile.WriteLine(s);//마지막 줄 추가.
                }
                string h = GetFileHash();
                using (StreamWriter outputFile = new StreamWriter(AccountPath, true))
                {
                    outputFile.WriteLine(h);//해시값 추가.
                }
            }
            else
            {
                File.Delete(AccountPath);
                File.Create(AccountPath).Close();
                using (StreamWriter outputFile = new StreamWriter(AccountPath, true))
                {
                    outputFile.WriteLine(s);//마지막 줄 추가.
                }
                string h = GetFileHash();
                using (StreamWriter outputFile = new StreamWriter(AccountPath, true))
                {
                    outputFile.WriteLine(h);//해시값 추가.
                }
            }
        }

    }


    public static class Costume
    {
        public static Dictionary<string, string> BunnyCostumeTable=new Dictionary<string, string>();
        public static Dictionary<string, string> MaidCostumeTable = new Dictionary<string, string>();
        public static SafeInt costumeNumber=new SafeInt(0);
        public static int CostumeNumber
        {
            get
            {
                return costumeNumber.var;
            }
            set
            {
                if(value!=costumeNumber.var)
                {

                    costumeNumber.var = value;
                    Tester.player.player.ClearAnimation();
                    Tester.player.player.AttachAnimation(30, Costume.GetCostume("Player_Ani11"), Costume.GetCostume("Player_Ani12"));
                }
                else
                {
                    costumeNumber.var = value;
                }

            }

        }


        public static void Init()
        {
            BunnyCostumeTable.Add("SCGSample", "SCG_Bunny_Default");
            BunnyCostumeTable.Add("SCG_Crazy", "SCG_Bunny_Crazy");
            BunnyCostumeTable.Add("SCG_Happy", "SCG_Bunny_Happy");
            BunnyCostumeTable.Add("SCG_Dying", "SCG_Bunny_Dying");
            BunnyCostumeTable.Add("SDead_11","Bunny_Dead_Rock");
            BunnyCostumeTable.Add("SDead_22", "Bunny_Dead_Water");
            BunnyCostumeTable.Add("SDead_333", "Bunny_Dead_Fire");
        }

        public static string GetCostume(string CostumeName)
        {
            if(CostumeNumber == 0)
            {
                return CostumeName;
            }
            if (CostumeNumber == 1)
            {
                if(BunnyCostumeTable.ContainsKey(CostumeName))
                     return BunnyCostumeTable[CostumeName];
                if (Game1.content.assetExists(CostumeName + "B"))
                    return CostumeName + "B";
            }


            if (CostumeNumber == 2&&MaidCostumeTable.ContainsKey(CostumeName))
                return MaidCostumeTable[CostumeName];
            return CostumeName;
        }

    }

    public static class ParticleEngine
    {
        public static Dictionary<DrawingLayer, Action<DrawingLayer>> ParticleActions = new Dictionary<DrawingLayer, Action<DrawingLayer>>();
        public static Dictionary<DrawingLayer, int> Timers = new Dictionary<DrawingLayer, int>();
        public static Dictionary<DrawingLayer, Point> Vectors = new Dictionary<DrawingLayer, Point>();
        public static Color BloodColor = Color.DarkRed * 5f;
        public static void GenerateParticle(Point Origin, int Timer, int Size, Action<DrawingLayer> ParticleAction, params Color[] colors) => GenerateParticle("Range", Origin, Timer, Size, ParticleAction, colors);
        public static void GenerateParticle(string ParticleTextureName, Point Origin, int Timer, int Size, Action<DrawingLayer> ParticleAction, params Color[] colors)
        {
            DrawingLayer particle = new DrawingLayer(ParticleTextureName, new Rectangle(Origin.X + Standard.Random(-20, 0), Origin.Y + Standard.Random(-20, 0), Size, Size));
            ParticleActions.Add(particle, ParticleAction);
            Timers.Add(particle, Timer);
            foreach (Color c in colors)
            {
                Standard.FadeAnimation(particle, Timer, c);
            }
        }
        public static DrawingLayer GenerateVectorParticle(Point Origin, Point v, int Timer, int Size, Action<DrawingLayer> ParticleAction, params Color[] colors)
        {
            return GenerateVectorParticle("Range", Origin, v, Timer, Size, ParticleAction, colors);
        }
    

    public static DrawingLayer GenerateVectorParticle(string ParticleTextureName, Point Origin, Point v, int Timer, int Size, Action<DrawingLayer> ParticleAction, params Color[] colors)
        {
            DrawingLayer particle = new DrawingLayer(ParticleTextureName, new Rectangle(Origin.X + Standard.Random(-20, 0), Origin.Y + Standard.Random(-20, 0), Size, Size));
            ParticleActions.Add(particle, ParticleAction);
            Timers.Add(particle, Timer);
            Vectors.Add(particle, v);
            foreach (Color c in colors)
            {
                Standard.FadeAnimation(particle, Timer, c);
            }
            return particle;
        }

        public static void GenerateSmallGravityParticle(Point Origin, int Random_i, int Random_j, params Color[] colors) =>
            GenerateParticle(new Point(Origin.X + Standard.Random(Random_i, Random_j), Origin.Y + Standard.Random(Random_i, Random_j)), 30, 3, Gravity_Smaller_Action, colors);
        public static void Update()
        {
            List<DrawingLayer> list = ParticleActions.Keys.ToList();
       
            for (int i = 0; i < list.Count; i++)
            {
                if(ParticleActions.ContainsKey(list[i]))
                    ParticleActions[list[i]](list[i]);
                if(Timers.ContainsKey(list[i]))
                {
                    Timers[list[i]]--;

                    if (Timers[list[i]] == 0)
                    {
                        RemoveParticle(list[i]);
                        i--;                        
                    }

                }
            }
        }

        public static void Gravity_Smaller_Action(DrawingLayer d)
        {
            if (Standard.FrameTimer % 2 == 0)
                d.SetBound(new Rectangle(d.GetPos().X + 1, d.GetPos().Y + 10, d.GetBound().Size.X, d.GetBound().Size.Y));
        }


        public static void GenerateBlood(Point Pos)
        {
            GenerateParticle(Pos, 40, 60, Blood_Seed_Action, BloodColor);
        }

        public static void GenerateBloodBranch(DrawingLayer BloodSeed)
        {
            int r = BloodSeed.GetBound().Width / 2;
            double w = Standard.Random() * 10;
            Point v = new Point((int)(Math.Cos(w) * r), (int)(Math.Sin(w) * r));
            GenerateVectorParticle(new Point(BloodSeed.GetCenter().X + v.X, BloodSeed.GetCenter().Y + v.Y), v, Timers[BloodSeed], (int)(r * 2 * 0.7f), Blood_Branch_Action, BloodColor);
        }

        public static void GrowBloodBranch(DrawingLayer BloodBranch)
        {
            if (Standard.Random() < 0.6)
                return;
            int r = BloodBranch.GetBound().Width / 2;
            Point v = Vectors[BloodBranch];
           GenerateVectorParticle(new Point(BloodBranch.GetCenter().X + v.X, BloodBranch.GetCenter().Y + v.Y), v, Timers[BloodBranch], (int)(r * 2 * 0.6f), Blood_Branch_Action, BloodColor);

        }

        public static void Blood_Seed_Action(DrawingLayer d)
        {
            if (Standard.FrameTimer % 2 == 0)
            {
                for (int i = 0; i < 50; i++)
                    GenerateBloodBranch(d);
                //추가 랜덤한 양의 Blood_Branch
                //Branch는 벡터를 받고 성장한다. 성장중 랜덤하게 붕괴
                RemoveParticle(d);

            }
        }
        public static void Blood_Branch_Action(DrawingLayer d)
        {
            if (Standard.FrameTimer % 4 == 0&&d.GetBound().Width>=5)
            {
                if(Checker.Weapon_Melee==12&&!Tester.ShotMode&&Checker.Bloodthirst>=1)
                {
                    
                    AbsorbBloodBranch(d);
                    if(Standard.Random() <0.2)
                        GrowBloodBranch(d);
                    RemoveParticle(d);
                    
                }
                else
                {
                    if (Standard.Random() < 0.2 * Checker.Bloodthirst)
                        AbsorbBloodBranch(d);
                    else
                        GrowBloodBranch(d);
                    RemoveParticle(d);
                }
            }
        }

        public static void AbsorbBloodBranch(DrawingLayer BloodBranch)
        {
            int r = BloodBranch.GetBound().Width / 2;
            Point v = Vectors[BloodBranch];
            GenerateVectorParticle(new Point(BloodBranch.GetCenter().X, BloodBranch.GetCenter().Y), v, Timers[BloodBranch]+8, (int)(r * 2 * 0.8f), Blood_Branch_Action, BloodColor).MoveTo(Tester.player.GetCenter().X, Tester.player.GetCenter().Y,40);

        }

        public static void RemoveParticle(DrawingLayer d)
        {
            if(ParticleActions.ContainsKey(d))
                ParticleActions.Remove(d);
            if (Timers.ContainsKey(d)) 
                Timers.Remove(d);
            if(Vectors.ContainsKey(d))
                Vectors.Remove(d);
        }



    }






}
