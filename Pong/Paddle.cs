using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.Physics2D;

namespace Pong
{
	public class Paddle : SpriteUV
	{
		public enum PaddleType { PLAYER, AI };
		
		private PaddleType m_type;
		private PhysicsBody m_physicsBody;
		private float m_fixedY;
		
		public Paddle (PaddleType type, PhysicsBody physicsBody)
		{
			m_physicsBody = physicsBody;
			m_type = type;

			this.TextureInfo = new TextureInfo(new Texture2D("Application/images/Paddle.png",false));
			this.Scale = this.TextureInfo.TextureSizef;
			this.Pivot = new Sce.PlayStation.Core.Vector2(0.5f,0.5f);
			
			if(m_type== PaddleType.AI)
			{
				this.Position = new Sce.PlayStation.Core.Vector2(
					Director.Instance.GL.Context.GetViewport().Width/2 - this.Scale.X/2,
					10 + this.Scale.Y/2);					
			}
			else
			{
				this.Position = new Sce.PlayStation.Core.Vector2(
					Director.Instance.GL.Context.GetViewport().Width/2 - this.Scale.X/2,
					Director.Instance.GL.Context.GetViewport().Height - this.Scale.Y/2 - 10);
			}
			
			// Cache the starting Y position, so we can reset and prevent any vertical movement from the Physics Engine
			m_fixedY = m_physicsBody.Position.Y;
			
			// Start with a minor amount of movement
			m_physicsBody.Force = new Vector2(-10.0f,0);
			
			Scheduler.Instance.ScheduleUpdateForTarget(this,0,false);
		}
		
		// This method will fix the physics bounding box to the sprites current position
		// Not currently used, was used for debug, left in for interest sake only
		private void ClampBoundingBox()
		{
			var bbBL = new Vector2(Position.X- Scale.X/2, Position.Y- Scale.Y/2) / PongPhysics.PtoM;
			var bbTR = new Vector2(Position.X+ Scale.X/2, Position.Y+ Scale.Y/2) / PongPhysics.PtoM;
			m_physicsBody.AabbMin = bbBL;
			m_physicsBody.AabbMax = bbTR;
			
		}
		public override void Update (float dt)
		{
			// Reset rotation to prevent "spinning" on collision
			m_physicsBody.Rotation = 0.0f;
			
			if(m_type == PaddleType.PLAYER)
			{
				if(Input2.GamePad0.Left.Down)
				{
					m_physicsBody.Force = new Vector2(-30.0f,0.0f);
				}
				if(Input2.GamePad0.Right.Down)
				{
					m_physicsBody.Force = new Vector2(30.0f,0.0f);
				}
			}
			else if(m_type == PaddleType.AI)
			{
				if(System.Math.Abs (GameScene.m_ball.Position.X - this.Position.X) <= this.Scale.Y/2)
					m_physicsBody.Force = new Vector2(0.0f,0.0f);
				else if(GameScene.m_ball.Position.X < this.Position.X)
					m_physicsBody.Force = new Vector2(-25.0f,0.0f);
				else if(GameScene.m_ball.Position.X > this.Position.X)
					m_physicsBody.Force = new Vector2(25.0f,0.0f);
			}
			
			//Prevent vertical movement on collision.  Could also implement by making paddle Kinematic
			//However, lose ability to use Force in that case and have to use AngularVelocity instead
			//which results in more logic in keeping the AI less "twitchy", a common Pong problem
			if(m_physicsBody.Position.Y != m_fixedY)
				m_physicsBody.Position = new Vector2(m_physicsBody.Position.X,m_fixedY);
			
			this.Position = m_physicsBody.Position * PongPhysics.PtoM;
		}
		
		~Paddle()
		{
			this.TextureInfo.Texture.Dispose ();
			this.TextureInfo.Dispose();
		}
	}
}

