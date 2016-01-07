using System;

using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Pong
{
	public enum Results { PlayerWin, AiWin, StillPlaying };
	
	public class Scoreboard : Sce.PlayStation.HighLevel.GameEngine2D.SpriteUV
	{
		public int m_playerScore = 0;
		public int m_aiScore = 0;
		
		public Scoreboard ()
		{
			this.TextureInfo = new TextureInfo();
			UpdateImage();
			
			this.Scale = this.TextureInfo.TextureSizef;
			this.Pivot = new Vector2(0.5f,0.5f);
			this.Position = new Vector2(Director.Instance.GL.Context.GetViewport().Width/2,
			                            Director.Instance.GL.Context.GetViewport().Height/2);
			
		}
		
		private void UpdateImage()
		{
			Image image = new Image(ImageMode.Rgba,new ImageSize(110,100),new ImageColor(0,0,0,0));
			Font font = new Font(FontAlias.System,50,FontStyle.Regular);
			image.DrawText(m_playerScore + " - " + m_aiScore,new ImageColor(255,255,255,255),font,new ImagePosition(0,0));
			image.Decode();

			var texture  = new Texture2D(110,100,false,PixelFormat.Rgba);
			if(this.TextureInfo.Texture != null)
				this.TextureInfo.Texture.Dispose();
			this.TextureInfo.Texture = texture;
			texture.SetPixels(0,image.ToBuffer());
			font.Dispose();
			image.Dispose();
		}
		public void Clear()
		{
			m_playerScore = m_aiScore = 0;
			UpdateImage();
		}
		
		public Results AddScore(bool player)
		{
			if(player)
				m_playerScore++;
			else
				m_aiScore++;
			if(m_playerScore > 3) return Results.PlayerWin;
			if(m_aiScore > 3) return Results.AiWin;
			
			UpdateImage();

			return Results.StillPlaying;
		}
	}
}

