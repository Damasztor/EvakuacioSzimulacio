using EvakuacioSzimulacio.Core;
using EvakuacioSzimulacio.Core.Simulation;
//using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
//using System.Drawing;

namespace EvakuacioSzimulacio
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private TileMap _map;
		private List<Person> _people;
		private Texture2D _personTexture;
		private Texture2D _circleTexture;
		private MovementManager _movementManager;
		private Vector2 _target = Vector2.Zero;
		Camera _camera;


		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1920;
			_graphics.PreferredBackBufferHeight = 900;
			_graphics.ApplyChanges();
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
			_map = new TileMap(GraphicsDevice);
			_personTexture = new Texture2D(GraphicsDevice, 1, 1);
			_personTexture.SetData(new[] { Color.White });
			_camera = new Camera();

			

			_people = new List<Person>
			{
				new Person(_circleTexture, new Vector2(2,2)*32,50f,10f),
				new Person(_circleTexture, new Vector2(250,350),70f,10f),
				new Person(_circleTexture, new Vector2(300,95),70f,10f),
				//new Person(_circleTexture, new Vector2(150,300),40f,10f)

			};
			_people[0].Direction = new Vector2(10, 10);
			_people[1].Direction = new Vector2(-40, -10);
			_people[2].Direction = new Vector2(-30, 20);
			//_people[3].Direction = new Vector2(1, 5);

			_movementManager = new MovementManager(_people,_map);

			
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_circleTexture = Content.Load<Texture2D>("circle");
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
			Vector2 WorldPosition = new Vector2();
			Vector2 CameraPosition = _camera.Position;
			//foreach(Person p in _people)
			//{
			//	Vector2 next = p.NextPositionOption(gameTime);

			//	p.Position = next;
			//	p.Hitbox.Center = next;
			//}
			var mouse = Mouse.GetState();
			if (mouse.LeftButton == ButtonState.Pressed)
			{
				Point MovedCameraPosition = new Point(mouse.Position.X + (int)CameraPosition.X, mouse.Position.Y + (int)CameraPosition.Y);
				int idxX = MovedCameraPosition.X / _map.tileSize;
				int idxY = MovedCameraPosition.Y / _map.tileSize;
				int width = _map.tileMap.GetLength(1);
				int height = _map.tileMap.GetLength(0);
				bool insidemap = idxX > 0 && idxY > 0 && idxX < height && idxY < width;
				
				if(insidemap)
				{
					if (_map.tileMap[idxX, idxY].Type == TileType.Empty || _map.tileMap[idxX, idxY].Type == TileType.Chair)
					{
						_target = new Vector2(MovedCameraPosition.X, MovedCameraPosition.Y);
						_movementManager.target = _target;
					}
				}
				
				
			}

			KeyboardState state = Keyboard.GetState();
			Vector2 move = Vector2.Zero;
			
			if (state.IsKeyDown(Keys.Up))
				move.Y -= 1;
			if (state.IsKeyDown(Keys.Down))
				move.Y += 1;
			if (state.IsKeyDown(Keys.Left))
				move.X -= 1;
			if (state.IsKeyDown(Keys.Right))
				move.X += 1;

			if (move != Vector2.Zero)
				move.Normalize();

			_camera.Move(move);
			_camera.Update();
			_movementManager.WhereToMove(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			
			_spriteBatch.Begin(transformMatrix: _camera.Transform);
			_map.Draw(_spriteBatch);
			


			foreach(Person p in _people)
			{
				p.Draw(_spriteBatch,p.Hitbox.Coloring);
				//Debug.WriteLine(p.Position);
				
			}
			_spriteBatch.End();


			base.Draw(gameTime);
		}
	}
	public class Camera
	{
		public Matrix Transform { get; private set; }
		public Vector2 Position { get; private set; }
		public float MoveSpeed = 5f;
		public void Move(Vector2 direction)
		{
			Position += direction * MoveSpeed;
		}
		public void Update()
		{
			Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y , 0));
		}
	}
}
