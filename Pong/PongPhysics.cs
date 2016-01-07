	using System;
	using Sce.PlayStation.Core;
	using Sce.PlayStation.HighLevel.GameEngine2D;
	using Sce.PlayStation.HighLevel.GameEngine2D.Base;
	using Sce.PlayStation.HighLevel.Physics2D;
	
	namespace Pong
	{
		
		public class PongPhysics : Sce.PlayStation.HighLevel.Physics2D.PhysicsScene
		{
			// PixelsToMeters
			public const float PtoM = 50.0f;
			private const float BALLRADIUS = 35.0f/2f; 
			private const float PADDLEWIDTH = 125.0f;
			private const float PADDLEHEIGHT = 38.0f;
			private float m_screenWidth;
			private float m_screenHeight;
			
			public enum BODIES { Ball = 0, Player, Ai, LeftBumper, RightBumper };
	
			
			public PongPhysics ()
			{
				m_screenWidth = Director.Instance.GL.Context.GetViewport().Width;
				m_screenHeight = Director.Instance.GL.Context.GetViewport().Height;
				
				this.InitScene();
			
				// turn gravity off
				this.Gravity = new Vector2(0.0f,0.0f);
				
				// Set the screen boundaries + 2m or 100pixel
				this.SceneMin = new Vector2(-100f,-100f) / PtoM;
				this.SceneMax = new Vector2(m_screenWidth + 100.0f,m_screenHeight + 100.0f) / PtoM;
				
				// And turn the bouncy bouncy on
				this.RestitutionCoeff = 1.0f;
				
				this.NumBody = 5; // Ball, 2 paddles, 2 bumpers
				this.NumShape = 3; // One of each of the above
				
			
				//Ball shape	
				this.SceneShapes[0] = new PhysicsShape(PongPhysics.BALLRADIUS/PtoM);
			
				//Create the ball physics object
				this.SceneBodies[0] = new PhysicsBody(SceneShapes[0],0.1f);
				this.sceneBodies[0].ColFriction = 0.01f;
				this.SceneBodies[0].Position = new Vector2(m_screenWidth/2,m_screenHeight/2) / PtoM;
				this.SceneBodies[0].ShapeIndex = 0;
				
			
				//Paddle shape
				Vector2 box = new Vector2(PADDLEWIDTH/2f/PtoM,-PADDLEHEIGHT/2f/PtoM);
				this.SceneShapes[1] = new PhysicsShape(box);
				
				//Player paddle
				this.SceneBodies[1] = new PhysicsBody(SceneShapes[1],1.0f);
				this.SceneBodies[1].Position = new Vector2(m_screenWidth/2f,0f+PADDLEHEIGHT/2+ 10f) / PtoM;
				this.SceneBodies[1].Rotation = 0;
				this.SceneBodies[1].ShapeIndex = 1;
				
				//Ai paddle
				this.SceneBodies[2] = new PhysicsBody(SceneShapes[1],1.0f);
				float aiX = ((m_screenWidth/2f)/PtoM);
				float aiY = (m_screenHeight - PADDLEHEIGHT/2 - 10f)/ PtoM;
				this.SceneBodies[2].Position = new Vector2(aiX,aiY);
				this.SceneBodies[2].Rotation = 0;
				this.SceneBodies[2].ShapeIndex = 1;
			
			
				//Now a shape for left and right bumpers to keep ball on screen
				this.SceneShapes[2] = new PhysicsShape((new Vector2(1.0f,m_screenHeight)) / PtoM);
				
				//Left bumper
				this.SceneBodies[3] = new PhysicsBody(SceneShapes[2],PhysicsUtility.FltMax);
				this.SceneBodies[3].Position = new Vector2(0,m_screenHeight/2f) / PtoM;
				this.sceneBodies[3].Rotation = 0;
				this.SceneBodies[3].SetBodyStatic();
				this.sceneBodies[3].ShapeIndex = 2;
				
				//Right bumper
				this.SceneBodies[4] = new PhysicsBody(SceneShapes[2],PhysicsUtility.FltMax);
				this.SceneBodies[4].Position = new Vector2(m_screenWidth,m_screenHeight/2f) / PtoM;
				this.sceneBodies[4].Rotation = 0;
				this.SceneBodies[4].SetBodyStatic();
				this.sceneBodies[4].ShapeIndex = 2;
			}
		}
	}
	
