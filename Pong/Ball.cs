using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.Physics2D;

namespace Pong
{
	public class Ball : SpriteUV
	{
		private PhysicsBody m_physicsBody;
		private Bgm m_ballBounce;
		private BgmPlayer m_songPlayer;
		// Change this value to make the game faster or slower
		public const float BALL_VELOCITY = 6.0f;
		
		public Ball (PhysicsBody physicsBody)
		{
			m_physicsBody = physicsBody;
			
			this.TextureInfo = new TextureInfo(new Texture2D("Application/images/ball.png",false));
			this.Scale = this.TextureInfo.TextureSizef;
			this.Pivot = new Sce.PlayStation.Core.Vector2(0.5f,0.5f);
			this.Position = new Sce.PlayStation.Core.Vector2(
				Director.Instance.GL.Context.GetViewport().Width/2 -Scale.X/2,
				Director.Instance.GL.Context.GetViewport().Height/2 -Scale.Y/2);
			
			
			//Right angles are exceedingly boring, so make sure we dont start on one
			//So if our Random angle is between 90 +- 25 degrees or 270 +- 25 degrees
			//we add 25 degree to value, ie, making 90 into 115 instead
			System.Random rand = new System.Random();
			float angle = (float)rand.Next(0,360);
		
			if((angle%90) <= 25) angle += 25.0f;
			this.m_physicsBody.Velocity = new Vector2(0.0f,BALL_VELOCITY).Rotate(PhysicsUtility.GetRadian(angle));
			
			Scheduler.Instance.ScheduleUpdateForTarget(this,0,false);
		}
		
		public void CheckVelocity()
		{
			if(System.Math.Abs (m_physicsBody.velocity.Y) < 1.0f)
			{
				if(m_physicsBody.velocity.Y > 0.0f)
					m_physicsBody.velocity.Y += 1.0f;
				
				else
					m_physicsBody.velocity.Y -= 1.0f;
			}
		}
		
		public override void Update (float dt)
		{
			this.Position = m_physicsBody.Position * PongPhysics.PtoM;
			
			var normalizedVel = m_physicsBody.Velocity.Normalize();
			m_physicsBody.Velocity = normalizedVel * BALL_VELOCITY;
		}
		
		~Ball()
		{
			this.TextureInfo.Texture.Dispose();
			this.TextureInfo.Dispose();
		}
	}
}

