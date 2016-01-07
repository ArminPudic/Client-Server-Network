using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.Physics2D;
using Sce.PlayStation.Core.Audio;

namespace Pong
{
	public class GameScene : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private Paddle m_player, m_ai;
		public static Ball m_ball;
		private PongPhysics m_physics;
		private Scoreboard m_scoreboard;
		private SoundPlayer m_pongBlipSoundPlayer;
		private Sound m_pongSound;
		
		// Change the following value to true if you want bounding boxes to be rendered
		private static Boolean DEBUG_BOUNDINGBOXS = false;
		
		public GameScene ()
		{
			this.Camera.SetViewFromViewport();
			m_physics = new PongPhysics();
			
			m_ball = new Ball(m_physics.SceneBodies[(int)PongPhysics.BODIES.Ball]);
			m_player = new Paddle(Paddle.PaddleType.PLAYER, 
			                     m_physics.SceneBodies[(int)PongPhysics.BODIES.Player]);
			m_ai = new Paddle(Paddle.PaddleType.AI, 
			                 m_physics.SceneBodies[(int)PongPhysics.BODIES.Ai]);
			m_scoreboard = new Scoreboard();
			
			this.AddChild(m_scoreboard);
			this.AddChild(m_ball);
			this.AddChild(m_player);
			this.AddChild(m_ai);
			
			// This is debug routine that will draw the physics bounding box around the players paddle
			if(DEBUG_BOUNDINGBOXS)
			{
				this.AdHocDraw += () => {
					var bottomLeftPlayer = m_physics.SceneBodies[(int)PongPhysics.BODIES.Player].AabbMin;
					var topRightPlayer = m_physics.SceneBodies[(int)PongPhysics.BODIES.Player].AabbMax;
					Director.Instance.DrawHelpers.DrawBounds2Fill(
						new Bounds2(bottomLeftPlayer*PongPhysics.PtoM,topRightPlayer*PongPhysics.PtoM));

					var bottomLeftAi = m_physics.SceneBodies[(int)PongPhysics.BODIES.Ai].AabbMin;
					var topRightAi = m_physics.SceneBodies[(int)PongPhysics.BODIES.Ai].AabbMax;
					Director.Instance.DrawHelpers.DrawBounds2Fill(
						new Bounds2(bottomLeftAi*PongPhysics.PtoM,topRightAi*PongPhysics.PtoM));

					var bottomLeftBall = m_physics.SceneBodies[(int)PongPhysics.BODIES.Ball].AabbMin;
					var topRightBall = m_physics.SceneBodies[(int)PongPhysics.BODIES.Ball].AabbMax;
					Director.Instance.DrawHelpers.DrawBounds2Fill(
						new Bounds2(bottomLeftBall*PongPhysics.PtoM,topRightBall*PongPhysics.PtoM));
				};
			}
			
			//Now load the sound fx and create a player
			m_pongSound = new Sound("/Application/audio/pongblip.wav");
			m_pongBlipSoundPlayer = m_pongSound.CreatePlayer();
			
			Scheduler.Instance.ScheduleUpdateForTarget(this,0,false);
		}
		
		private void ResetBall()
		{
			//Move ball to screen center and release in a random directory
			m_physics.SceneBodies[(int)PongPhysics.BODIES.Ball].Position = 
				new Vector2(Director.Instance.GL.Context.GetViewport().Width/2.0f,
				            Director.Instance.GL.Context.GetViewport().Height/2.0f) / PongPhysics.PtoM;
			
			System.Random rand = new System.Random();
			float angle = (float)rand.Next(0,360);
		
			if((angle%90) <= 15) angle += 15.0f;
		
			m_physics.SceneBodies[(int)PongPhysics.BODIES.Ball].Velocity = 
				new Vector2(0.0f,5.0f).Rotate(PhysicsUtility.GetRadian(angle));
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			
			if(Input2.GamePad0.Select.Press)
				Director.Instance.ReplaceScene(new MenuScene());
			// something!
			//Update the physics simulation
			m_physics.Simulate();
			
			bool ballIsInContact = false;
			//Now check if the ball hit either paddle, and if so, play the sound
			if(m_physics.QueryContact((uint)PongPhysics.BODIES.Ball,(uint)PongPhysics.BODIES.Player) ||
				m_physics.QueryContact((uint)PongPhysics.BODIES.Ball,(uint)PongPhysics.BODIES.Ai))
			{
				ballIsInContact = true;
				// This sound is annoying, so it is commented out!
//				if(m_pongBlipSoundPlayer.Status == SoundStatus.Stopped)
//					m_pongBlipSoundPlayer.Play();
			}
			
			//Check if the ball went off the top or bottom of the screen and update score accordingly
			Results result = Results.StillPlaying;
			bool scored = false;
			
			if(m_ball.Position.Y > Director.Instance.GL.Context.GetViewport().Height + m_ball.Scale.Y/2)
			{
				result = m_scoreboard.AddScore(true);
				scored = true;
			}
			if(m_ball.Position.Y < 0 - m_ball.Scale.Y/2)
			{
				result = m_scoreboard.AddScore(false);
				scored = true;
			}
			
			// Did someone win?  If so, show the GameOver scene
			if(result == Results.AiWin) 
				Director.Instance.ReplaceScene(new GameOverScene(false));
			if(result == Results.PlayerWin) 
				Director.Instance.ReplaceScene(new GameOverScene(true));
			
			//If someone did score, but game isn't over, reset the ball position to the middle of the screen
			if(scored == true)
			{
				ResetBall ();
			}
			
			//Finally a sanity check to make sure the ball didn't leave the field.
			var ballPB = m_physics.SceneBodies[(int)PongPhysics.BODIES.Ball];
			
			if(ballPB.Position.X < -(m_ball.Scale.X/2f)/PongPhysics.PtoM ||
			   ballPB.Position.X > (Director.Instance.GL.Context.GetViewport().Width)/PongPhysics.PtoM)
			{
				ResetBall();
			}
			else if(!ballIsInContact)
			{
				m_ball.CheckVelocity();
			}
		}
		
		~GameScene(){
			m_pongBlipSoundPlayer.Dispose();
		}
	}
}

