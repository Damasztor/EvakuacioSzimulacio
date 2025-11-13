using EvakuacioSzimulacio.Core;
using EvakuacioSzimulacio.Core.Simulation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

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


		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
			_map = new TileMap(GraphicsDevice);
			_personTexture = new Texture2D(GraphicsDevice, 1, 1);
			_personTexture.SetData(new[] { Color.White });

			

			_people = new List<Person>
			{
				new Person(_circleTexture, new Vector2(100,100),50f,10f),
				new Person(_circleTexture, new Vector2(250,350),70f,10f),
				new Person(_circleTexture, new Vector2(300,95),70f,10f),
				new Person(_circleTexture, new Vector2(150,300),40f,10f)

			};
			_people[0].Direction = new Vector2(10, 10);
			_people[1].Direction = new Vector2(-40, -10);
			_people[2].Direction = new Vector2(-30, 20);
			_people[3].Direction = new Vector2(1, 5);

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
			//foreach(Person p in _people)
			//{
			//	Vector2 next = p.NextPositionOption(gameTime);

			//	p.Position = next;
			//	p.Hitbox.Center = next;
			//}
			_movementManager.WhereToMove(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			
			_spriteBatch.Begin();
			_map.Draw(_spriteBatch);
			var mouse = Mouse.GetState();
			if(mouse.LeftButton == ButtonState.Pressed)
			{
				_target = new Vector2(mouse.Position.X, mouse.Position.Y);
				_movementManager.target = _target;
			}


			foreach(Person p in _people)
			{
				p.Draw(_spriteBatch,p.Hitbox.Coloring);
				//Debug.WriteLine(p.Position);
				
			}
			_spriteBatch.End();


			base.Draw(gameTime);
		}
	}
}
