using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Input;

namespace Pong
{
	public class GameOverScene : Scene
	{
		private TextureInfo m_ti;
		private Texture2D m_texture;
		
		public GameOverScene (bool win)
		{
			this.Camera.SetViewFromViewport();
			if(win)
				m_texture = new Texture2D("Application/images/winner.png",false);
			else
				m_texture = new Texture2D("Application/images/loser.png",false);
			m_ti = new TextureInfo(m_texture);
			SpriteUV titleScreen = new SpriteUV(m_ti);
			titleScreen.Scale = m_ti.TextureSizef;
			titleScreen.Pivot = new Vector2(0.5f,0.5f);
			titleScreen.Position = new Vector2(Director.Instance.GL.Context.GetViewport().Width/2,
			                                   Director.Instance.GL.Context.GetViewport().Height/2);
			this.AddChild(titleScreen);
			
			Scheduler.Instance.ScheduleUpdateForTarget(this,0,false);
			
			Touch.GetData(0).Clear();
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			int touchCount = Touch.GetData(0).ToArray().Length;
			if(touchCount > 0 || Input2.GamePad0.Cross.Press)
			{
				Director.Instance.ReplaceScene( new TitleScene());
			}
		}
		
		~GameOverScene()
		{
			m_texture.Dispose();
			m_ti.Dispose ();
		}
	}
}

