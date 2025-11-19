using EvakuacioSzimulacio.Core;
using EvakuacioSzimulacio.Core.Simulation;
//using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using System.Windows.Forms;
//using System.Windows.Forms;
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
		Random rnd;
		Texture2D pixel;
		SpriteFont font;

		private List<Person> _exitedPeople;

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
			rnd = new Random();
			_exitedPeople = new List<Person>();
			

			

			_people = new List<Person>
			{
				//new Person(_circleTexture, new Vector2(3,3)*_map.tileSize,50f,10f),
				//new Person(_circleTexture, new Vector2(3,4)*_map.tileSize,70f,10f),
				//new Person(_circleTexture, new Vector2(300,95),70f,10f),
				////new Person(_circleTexture, new Vector2(150,300),40f,10f)

			};
			//_people[0].Direction = new Vector2(10, 10);
			//_people[1].Direction = new Vector2(-40, -10);
			//_people[2].Direction = new Vector2(-30, 20);
			////_people[3].Direction = new Vector2(1, 5);

			int id = 0;
			//foreach(var l in _map.tileMap)
			//{
			//	if(l.Type == TileType.Chair && rnd.Next(1,21) < 2)
			//	{
			//		_people.Add(new Person(id, _circleTexture, l.Center, rnd.Next(50, 71), 10f));
			//		id++;
			//	}
			//}
			//_people.Add(new Person(0, _circleTexture, new Vector2(336,64), rnd.Next(50, 71), 10f));
			//_people.Add(new Person(1, _circleTexture, new Vector2(368,127), rnd.Next(50, 71), 10f));
			_people.Add(new Person(0, _circleTexture, new Vector2(96, 192), 2, 10f));
			_people.Add(new Person(1, _circleTexture, new Vector2(156, 192), 2, 10f));
			_people[0].Target = new Vector2(156, 192);
			_people[1].Target = new Vector2(96, 192);
			

			_movementManager = new MovementManager(_people,_map);

			
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_circleTexture = Content.Load<Texture2D>("circle");
			pixel = new Texture2D(GraphicsDevice, 1, 1);
			pixel.SetData(new[] { Color.White });
			font = Content.Load<SpriteFont>("default");

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
					if (_map.tileMap[idxX, idxY].Type == TileType.Empty || _map.tileMap[idxX, idxY].Type == TileType.Chair || _map.tileMap[idxX,idxY].Type == TileType.Exit)
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
			_exitedPeople.AddRange(_people.Where(p => p.IsExited));
			_people.RemoveAll(p => p.IsExited);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			
			_spriteBatch.Begin(transformMatrix: _camera.Transform);
			_map.Draw(_spriteBatch);
			


			foreach(Person p in _people)
			{
				p.Draw(_spriteBatch,p.Hitbox.Coloring,font);
				
				//Debug.WriteLine(p.Position);
				
			}
			foreach(var d in _movementManager.debuglist)
			{
				d.Draw(_spriteBatch);
			}
			_spriteBatch.End();


			base.Draw(gameTime);
		}
		public void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color, float thickness = 1f)
		{
			Vector2 edge = end - start;
			float angle = (float)Math.Atan2(edge.Y, edge.X);

			sb.Draw(
				pixel,
				new Rectangle(
					(int)start.X,
					(int)start.Y,
					(int)edge.Length(),
					(int)thickness
				),
				null,
				color,
				angle,
				Vector2.Zero,
				SpriteEffects.None,
				0
			);
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
