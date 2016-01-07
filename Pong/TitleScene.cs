using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Input;
 
namespace Pong
{
	public class TitleScene : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private TextureInfo m_ti;
		private Texture2D m_texture;
		
		private Bgm m_titleSong;
		private BgmPlayer m_songPlayer;
		
		public TitleScene ()
		{
			this.Camera.SetViewFromViewport();
			m_texture = new Texture2D("Application/images/title.png",false);
			m_ti = new TextureInfo(m_texture);
			SpriteUV titleScreen = new SpriteUV(m_ti);
			titleScreen.Scale = m_ti.TextureSizef;
			titleScreen.Pivot = new Vector2(0.5f,0.5f);
			titleScreen.Position = new Vector2(Director.Instance.GL.Context.GetViewport().Width/2.0f,
			                                  Director.Instance.GL.Context.GetViewport().Height/2.0f);
			this.AddChild(titleScreen);
			
			Vector4 origColor = titleScreen.Color;
			titleScreen.Color = new Vector4(0,0,0,0);
			var tintAction = new TintTo(origColor,10.0f);
			ActionManager.Instance.AddAction(tintAction,titleScreen);
			tintAction.Run();
			
			m_titleSong = new Bgm("/Application/audio/titlesong.mp3");
			
			if(m_songPlayer != null)
			m_songPlayer.Dispose();
			m_songPlayer = m_titleSong.CreatePlayer();
			
			Scheduler.Instance.ScheduleUpdateForTarget(this,0,false);

			// Clear any queued clicks so we dont immediately exit if coming in from the menu
			Touch.GetData(0).Clear();
		}
		
		public override void OnEnter ()
		{
			m_songPlayer.Loop = true;
			m_songPlayer.Play();
		}
		
		public override void OnExit ()
		{
			base.OnExit ();
			m_songPlayer.Stop();
			m_songPlayer.Dispose();
			m_songPlayer = null;
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			var touches = Touch.GetData(0).ToArray();
			if((touches.Length > 0 && touches[0].Status == TouchStatus.Down) || Input2.GamePad0.Cross.Press)
			{
				Director.Instance.ReplaceScene(new MenuScene());
			}
		}
	
		~TitleScene()
		{
			m_texture.Dispose();
			m_ti.Dispose ();
		}
	}
}
 