using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
//using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EvakuacioSzimulacio.Core.Simulation
{
	internal class Debug
	{
		

		public Vector2 position {  get; set; }
		public float radius { get; set; }
		public Vector2 velocity { get; set; }
		public Vector2 desiredvelocity { get; set; }
		public Vector2 target {  get; set; }
		Texture2D pixel;
		public Vector2 leftaxis { get; set; }
		public Vector2 rightaxis { get; set; }


		public Debug(Vector2 position, float radius, Vector2 velocity, Vector2 desiredvelocity, Vector2 target, Vector2 leftaxis, Vector2 rightaxis)
		{
			this.position = position;
			this.radius = radius;
			this.velocity = velocity;
			this.desiredvelocity = desiredvelocity;
			this.target = target;
			this.leftaxis = leftaxis;
			this.rightaxis = rightaxis;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (pixel == null)
			{
				// Hozzunk létre 1x1 fehér pixelt
				pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				pixel.SetData(new[] { Color.White });
			}

			// Radius kirajzolása (kör)
			//DrawCircle(spriteBatch, position, radius, Color.Red);

			// Velocity vektor kirajzolása
			DrawLine(spriteBatch, position, (position + velocity), Color.Yellow, 2f);

			// DesiredVelocity vektor kirajzolása
			DrawLine(spriteBatch, position, (position + desiredvelocity), Color.Green, 2f);

			DrawLine(spriteBatch, position, position + leftaxis, Color.Pink, 2f);
			DrawLine(spriteBatch, position, position + rightaxis, Color.Magenta, 2f);

			// Target pont kirajzolása
			//DrawCircle(spriteBatch, target, 5, Color.Blue); // kis kör a targethez
		}
		private void DrawCircle(SpriteBatch sb, Vector2 center, float radius, Color color, int segments = 20)
		{
			Vector2 prev = center + new Vector2(radius, 0);
			float increment = MathF.Tau / segments;

			for (int i = 1; i <= segments; i++)
			{
				float angle = i * increment;
				Vector2 next = center + new Vector2(MathF.Cos(angle) * radius, MathF.Sin(angle) * radius);
				DrawLine(sb, prev, next, color);
				prev = next;
			}
		}
		public void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color, float thickness = 1f)
		{
			Vector2 edge = end - start;
			float angle = (float)Math.Atan2(edge.Y, edge.X);
			if(edge.Length() < 20)
			{
				edge.Normalize();
				
			}
			sb.Draw(
				pixel,
				new Rectangle(
					(int)start.X,
					(int)start.Y,
					(int)edge.Length() * 20,
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
}
