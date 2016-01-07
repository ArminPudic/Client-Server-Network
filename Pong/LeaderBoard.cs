using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.Core.Imaging;

namespace Pong
{
	public class LeaderBoard : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private Sce.PlayStation.HighLevel.UI.Scene m_uiScene;
		private static TextureInfo textInfo;
		private static Texture2D m_texture;
		
		public string highScores;
		public string playerName;
		public int totalScore;
			
		public LeaderBoard (string scores)
		{
			highScores = scores;
			this.Camera.SetViewFromViewport();
			Sce.PlayStation.HighLevel.UI.Panel dialog = new Panel();
			dialog.Width = Director.Instance.GL.Context.GetViewport().Width;
			dialog.Height = Director.Instance.GL.Context.GetViewport().Height;
			
			textInfo = new TextureInfo(m_texture);
			SpriteUV tScreen = new SpriteUV(textInfo);
			UpdateImage(highScores);
			
			tScreen.Scale = textInfo.TextureSizef;
			tScreen.Pivot = new Vector2(0.5f,0.5f);
			tScreen.Position = new Vector2(Director.Instance.GL.Context.GetViewport().Width/2,
			                            Director.Instance.GL.Context.GetViewport().Height/2);
			this.AddChild(tScreen);
			
			ImageBox ib = new ImageBox();
			ib.Width = dialog.Width;
			ib.Image = new ImageAsset("/Application/images/highScore.png", false);
			ib.Height = dialog.Height;
			ib.SetPosition(0.0f, 0.0f);
			
			Button returnToMenu = new Button();
			returnToMenu.Name = "returnToMenu";
			returnToMenu.Text = "Menu";
			returnToMenu.Width = 200;
			returnToMenu.Height = 50;
			returnToMenu.Alpha = 0.8f;
			returnToMenu.SetPosition(700.0f, 40.0f);
			returnToMenu.TouchEventReceived += OnButtonPlay;
			
			dialog.AddChildLast(ib);
			dialog.AddChildLast(returnToMenu);
			m_uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			m_uiScene.RootWidget.AddChildLast(dialog);
			UISystem.SetScene(m_uiScene);
			Scheduler.Instance.ScheduleUpdateForTarget(this, 0, false);
		}
		
		public void OnButtonPlay(object sender, TouchEventArgs e)
		{
			Director.Instance.ReplaceScene(new TitleScene());
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			UISystem.Update(Touch.GetData(0));
		}
		
		public static void UpdateImage(string highScores)
		{
			Image image = new Image(ImageMode.Rgba,new ImageSize(960,544),new ImageColor(0,0,0,0));
			Font font = new Font(FontAlias.System,25,FontStyle.Regular);
			image.DrawText("High Scores: " + highScores,new ImageColor(255,255,255,255),font,new ImagePosition(25,100));
			image.Decode();

			var texture  = new Texture2D(960,544,false,PixelFormat.Rgba);
			if(textInfo.Texture != null)
				textInfo.Texture.Dispose();
			textInfo.Texture = texture;
			texture.SetPixels(0,image.ToBuffer());
			font.Dispose();
			image.Dispose();
		}
		
		public override void Draw ()
		{
			base.Draw();
			UISystem.Render ();
		}
		
		~LeaderBoard()
		{
			//font.Dispose();
			//image.Dispose();
		}
	}
}

