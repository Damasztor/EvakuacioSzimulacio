using Microsoft.Xna.Framework;
using System;

namespace EvakuacioSzimulacio.Core.Simulation
{
	internal class CollisionManager // kör-téglalap és kör-kör ütközésére szolgál
	{

		public bool Intersects(CircleHitbox circleHitbox, RectangleHitbox rectangleHitbox)
		{
			float closestX = Math.Clamp(circleHitbox.Center.X, rectangleHitbox.TopLeft.X, rectangleHitbox.BottomRight.X);
			float closestY = Math.Clamp(circleHitbox.Center.Y, rectangleHitbox.TopLeft.Y, rectangleHitbox.BottomRight.Y);

			float distX = circleHitbox.Center.X - closestX;
			float ditsY = circleHitbox.Center.Y - closestY;

			float distSquared = distX * distX + ditsY * ditsY;
			return distSquared < circleHitbox.Radius * circleHitbox.Radius;
		}
		public bool Intersects(CircleHitbox circleHitbox, CircleHitbox circleHitboxOther)
		{
			float dist = Vector2.Distance(circleHitbox.Center, circleHitboxOther.Center);
			return dist < circleHitbox.Radius + circleHitboxOther.Radius;
		}
	}
}
