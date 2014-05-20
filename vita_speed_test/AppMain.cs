using System;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;


namespace vita_speed_test {
	public class AppMain {
		
		#region Tests
		
		public static void TestTexture2DInitTime(bool hasMip) {
			mLastTest = "TestTexture2DInitTime hasMip = " + hasMip + "\n" +
				"TL0 size is 1x1, TL11 size is 2048x2048\n";
			
			int count = 12;
			var prf = new sProfiler();
			
			Texture2D[] tex = new Texture2D[count];

			for (int i = 0; i < count; ++i) {
				int w, h;
				w = h = 1 << i;
			
				eScope sc = (eScope)(i + (int)eScope.TL0);
			
				prf.Start(sc);
				tex[i] = new Texture2D(w, h, hasMip, PixelFormat.Rgba);
				prf.Stop();
			}
			
			for (int i = 0; i < count; ++i) {
				tex[i].Dispose();
				tex[i] = null;
			}
			tex = null;
		}
		
		public static void TestTexture2DSetPixelsTime(int levelCount) {
			int count = levelCount;
			int w, h;
			w = h = 1 << (count - 1);
			
			mLastTest = "TestTexture2DSetPixelsTime levelCount = " + levelCount + "\n" +
				"TL0 size is " + w + "\n";
			
			
			var prf = new sProfiler();
			var tex = new Texture2D(w, h, true, PixelFormat.Rgba);
					
			for (int i = 0; i < count; ++i) {
			
				w = h = 1 << (count - 1 - i);
			
				byte[] data = new byte[w * h * 4];
			
				eScope sc = (eScope)(i + (int)eScope.TL0);
			
				prf.Start(sc);
				tex.SetPixels(i, data);
				prf.Stop();
			
				data = null;
			}
			
			tex.Dispose();
			tex = null;
		}
		
		public static void TestTexture2DSetPixelsNoMipsTime() {
			int count = 12;
			int w, h;
			w = h = 1 << (count - 1);
			
			mLastTest = "TestTexture2DSetPixelsNoMipsTime\n" +
				"TL0 size is " + w + "\n";
			
			
			var prf = new sProfiler();
			
			
			for (int i = 0; i < count; ++i) {
			
				w = h = 1 << (count - 1 - i);
				var tex = new Texture2D(w, h, false, PixelFormat.Rgba);
			
				byte[] data = new byte[w * h * 4];
			
				eScope sc = (eScope)(i + (int)eScope.TL0);
			
				prf.Start(sc);
				tex.SetPixels(0, data);
				prf.Stop();
			
				data = null;
				tex.Dispose();
				tex = null;
			}
		}
		
		public static void TestTexture2DGenerateMipsTime() {
			int count = 12;
			int w, h;
			w = h = 1 << (count - 1);
			
			mLastTest = "TestTexture2DGenerateMipsTime\n" +
				"TL0 size is " + w + "\n";
			
			var prf = new sProfiler();
			
			for (int i = 0; i < count; ++i) {
				w = h = 1 << (count - 1 - i);
				var tex = new Texture2D(w, h, true, PixelFormat.Rgba);
			
				byte[] data = new byte[w * h * 4];
			
				eScope sc = (eScope)(i + (int)eScope.TL0);
			
				tex.SetPixels(0, data);
				
				prf.Start(sc);
				tex.GenerateMipmap();
				prf.Stop();
			
				data = null;
				tex.Dispose();
				tex = null;
			}
		}
		
		#endregion
		
		#region Test control
		
		public static void Update() {
			var gamePadData = GamePad.GetData(0);
			
			if (gamePadData.ButtonsUp.HasFlag(GamePadButtons.Cross)) {
				DoTest(() => TestTexture2DInitTime(true));
			} else if (gamePadData.ButtonsUp.HasFlag(GamePadButtons.Square)) {
				DoTest(() => TestTexture2DInitTime(false));
			} else if (gamePadData.ButtonsUp.HasFlag(GamePadButtons.Circle)) {
				DoTest(() => TestTexture2DSetPixelsTime(mSetPixelsTestLevelCount));
			} else if (gamePadData.ButtonsUp.HasFlag(GamePadButtons.Triangle)) {
				DoTest(() => TestTexture2DSetPixelsNoMipsTime());
			} else if (gamePadData.ButtonsUp.HasFlag(GamePadButtons.Start)) {
				DoTest(() => TestTexture2DGenerateMipsTime());
			} else if (gamePadData.ButtonsUp.HasFlag(GamePadButtons.Up)) {
				AddLevelCountForSetPixelsTest(1);
			} else if (gamePadData.ButtonsUp.HasFlag(GamePadButtons.Down)) {
				AddLevelCountForSetPixelsTest(-1);
			}
		}
		
		public static void DoTest(Action test) {
			cProfilerSystem.ResetAll();
			
			test();
			
			var t = mLastTest + "\n" + cProfilerSystem.Dump();
			
			mLabel.Text = t;
			Console.WriteLine(t);
		}
		
		public static void AddLevelCountForSetPixelsTest(int relVal) {
			int newVal = mSetPixelsTestLevelCount + relVal;
			if (newVal != mSetPixelsTestLevelCount && newVal > 0 && newVal <= 12) {
				mSetPixelsTestLevelCount = newVal;
				
				var t = "TestTexture2DSetPixelsTime level count = " + newVal + "\n";
				t += "width/heigth of level 0 is " + (1 << (newVal - 1));
				
				mLabel.Text = t;
			}
		}
		
		#endregion
		
		private static GraphicsContext mGraphics;
		private static Label mLabel;
		private static string mLastTest;
		private static int mSetPixelsTestLevelCount = 12;
		
		public static void Main(string[] args) {
			Initialize();

			while (true) {
				SystemEvents.CheckEvents();
				Update();
				Render();
			}
		}

		public static void Initialize() {
			mGraphics = new GraphicsContext();
			var st = cProfilerSystem.Stopwatch;
			
			UISystem.Initialize(mGraphics);
			
			Scene myScene = new Scene();
			mLabel = new Label();
			mLabel.X = 0.0f;
			mLabel.Y = 0.0f;
			mLabel.Width = 960;
			mLabel.Height = 544;
			mLabel.Text = "press buttons to do test:\n" + 
				"cross: \tTestTexture2DInitTime with mips\n" + 
				"square: \tTestTexture2DInitTime without mips\n" + 
				"circle: \tTestTexture2DSetPixelsTime with mips\n" + 
				"triangle: TestTexture2DSetPixelsNoMipsTime\n" + 
				"start: TestTexture2DGenerateMipsTime\n" +
				"up: \tincrease TestTexture2DSetPixelsTime level count\n" + 
				"down: \tdecrease TestTexture2DSetPixelsTime level count\n";
			myScene.RootWidget.AddChildLast(mLabel);
			
			UISystem.SetScene(myScene, null);
		}



		public static void Render() {
			mGraphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			mGraphics.Clear();
			
			UISystem.Render();

			mGraphics.SwapBuffers();
		}
		
	}
}
