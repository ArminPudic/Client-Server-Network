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
	public class SubmitScore : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private Sce.PlayStation.HighLevel.UI.Scene m_uiScene;
		private static TextureInfo textInfo;
		private static Texture2D m_texture;
		
		public string playerName;
		public int totalScore = 0;
		public EditableText playerNameField;
			
		public SubmitScore ()
		{
			this.Camera.SetViewFromViewport();
			Sce.PlayStation.HighLevel.UI.Panel dialog = new Panel();
			dialog.Width = Director.Instance.GL.Context.GetViewport().Width;
			dialog.Height = Director.Instance.GL.Context.GetViewport().Height;
			
			//textInfo = new TextureInfo(m_texture);
			//SpriteUV tScreen = new SpriteUV(textInfo);
			
			//tScreen.Scale = textInfo.TextureSizef;
			//tScreen.Pivot = new Vector2(0.5f,0.5f);
			//tScreen.Position = new Vector2(Director.Instance.GL.Context.GetViewport().Width/2,
			                            //Director.Instance.GL.Context.GetViewport().Height/2);
			//this.AddChild(tScreen);
			
			ImageBox ib = new ImageBox();
			ib.Width = dialog.Width;
			ib.Image = new ImageAsset("/Application/images/highScore.png", false);
			ib.Height = dialog.Height;
			ib.SetPosition(0.0f, 0.0f);
					
			Button submitScore = new Button();
			submitScore.Name = "submitScore";
			submitScore.Text = "Submit Score";
			submitScore.Width = 200;
			submitScore.Height = 50;
			submitScore.Alpha = 0.8f;
			submitScore.SetPosition(250.0f, 40.0f);
			submitScore.TouchEventReceived += OnSubmitScore;
			
			Button returnToMenu = new Button();
			returnToMenu.Name = "returnToMenu";
			returnToMenu.Text = "Menu";
			returnToMenu.Width = 200;
			returnToMenu.Height = 50;
			returnToMenu.Alpha = 0.8f;
			returnToMenu.SetPosition(700.0f, 40.0f);
			returnToMenu.TouchEventReceived += OnButtonPlay;
			
			playerNameField = new EditableText();
			playerNameField.Name = "playerNameField";
			playerNameField.Width = 200;
			playerNameField.Height = 50;
			playerNameField.Alpha = 0.8f;
			playerNameField.SetPosition(25.0f, 40.0f);
			
			UpdateImage(totalScore);
			
			dialog.AddChildLast(ib);
			dialog.AddChildLast(submitScore);
			dialog.AddChildLast(returnToMenu);
			dialog.AddChildLast(playerNameField);
			m_uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			m_uiScene.RootWidget.AddChildLast(dialog);
			UISystem.SetScene(m_uiScene);
			Scheduler.Instance.ScheduleUpdateForTarget(this, 0, false);
		}
		
		public void OnButtonPlay(object sender, TouchEventArgs e)
		{
			Director.Instance.ReplaceScene(new TitleScene());
		}
		
		public void OnSubmitScore(object sender, TouchEventArgs e)
		{
			playerName = playerNameField.Text;
			totalScore = Scoreboard.TotalScore();
			Client client = new Client(playerName, totalScore);
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			UISystem.Update(Touch.GetData(0));
		}
		
		public static void UpdateImage(int score)
		{
			Image image = new Image(ImageMode.Rgba,new ImageSize(960,544),new ImageColor(0,0,0,0));
			Font font = new Font(FontAlias.System,20,FontStyle.Regular);
			image.DrawText("Your Total Score: " + score,new ImageColor(255,255,255,255),font,new ImagePosition(25,125));
			image.Decode();

			//var texture  = new Texture2D(960,544,false,PixelFormat.Rgba);
			//if(textInfo.Texture != null)
				//textInfo.Texture.Dispose();
			//textInfo.Texture = texture;
			//texture.SetPixels(0,image.ToBuffer());
			font.Dispose();
			image.Dispose();
		}
		
		public override void Draw ()
		{
			base.Draw();
			UISystem.Render ();
		}
		
		~SubmitScore()
		{
			//font.Dispose();
			//image.Dispose();
		}
	}
}

