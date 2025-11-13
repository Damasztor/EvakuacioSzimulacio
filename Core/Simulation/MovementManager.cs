using EvakuacioSzimulacio.Core.Pathfinding;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Numerics;

namespace EvakuacioSzimulacio.Core.Simulation
{
	internal class MovementManager
	{
		AStar AStarPathfinding;
		CollisionManager collisionManager;
        SpatialGrid spatialGrid;
        List<Person> People;
        Tile[,] Tiles;
		TileMap tileMap;
        double maxDistance;
        Vector2 acceleration = new Vector2(3,3);
		public Vector2 target = Vector2.Zero;
        

		public MovementManager(List<Person> people, TileMap tileMap)
        {
            People = people;
            spatialGrid = new SpatialGrid(32,1000,1000);
			maxDistance = Math.Sqrt(spatialGrid.CellSize * spatialGrid.CellSize + spatialGrid.CellSize*spatialGrid.CellSize) * 2;
            Tiles = tileMap.tileMap;
            spatialGrid.ClearTiles();
			foreach (var t in Tiles)
            {
				spatialGrid.AddTiles(t);
			}
            collisionManager = new CollisionManager();
			AStarPathfinding = new AStar();
			this.tileMap = tileMap;
		}
		public void WhereToMove(GameTime gameTime)
        {
            spatialGrid.Clear();
			foreach (var p in People)
            {
				spatialGrid.AddPerson(p);
			}
			foreach (Person p in People)
            {
                List<Person> nearbyPeople = spatialGrid.GetNearbyPeople(p);
                List<Tile> nearbyTiles = spatialGrid.GetNearbyTiles(p);
				if (float.IsNaN(p.Direction.X))
				{
					p.Direction = new Vector2(0, p.Direction.Y);
				}
				if (float.IsNaN(p.Direction.Y))
				{
					p.Direction = new Vector2(p.Direction.X, 0);
				}
                
                if(nearbyTiles.Count > 0)
                {
                    ;//debug line
                }
                
                
                foreach(var nearppl in nearbyPeople)
                {
                    var distance = Vector2.Distance(p.Position, nearppl.Position);
                    var direction = p.Position - nearppl.Position;
                    var force = (maxDistance - distance)/4000;
					
					float avoidStrength = 0.05f;
                    if(collisionManager.Intersects(p.Hitbox, nearppl.Hitbox))
                    {
						//Debug.WriteLine("IGEN");

						Vector2 betweenVector = nearppl.Position - p.Position;
						Vector2 collisionSteeringVector = RotateVector(p.Direction, GetSignedAngle(p.Direction,betweenVector) * avoidStrength);

						p.Direction += collisionSteeringVector;
						p.Hitbox.Coloring = Color.Yellow;


						float distanceBetween = betweenVector.Length();
						Vector2 normalizedBetweenVector = Vector2.Normalize(betweenVector);
						if(betweenVector.Length() == 0)
						{
							;//debug line vector NaN NaN
						}
						float minDistance = p.Hitbox.Radius + nearppl.Hitbox.Radius;
						if (distanceBetween < minDistance)
						{
							float distCorrection = (minDistance - distanceBetween)/2;
							p.Position -= normalizedBetweenVector * distCorrection;
							p.Hitbox.Center = p.Position;
						}

						
					}
                    else
                    {
						//Debug.WriteLine("NEM");
						p.Direction += (float)force * direction;
						
					}
					
                    
                }
                foreach (var neartiles in nearbyTiles)
                {
					var distance = Vector2.Distance(p.Position, neartiles.Center);
					var direction = p.Position - neartiles.Center;
					var force = (maxDistance - distance) / 8000;
					if (collisionManager.Intersects(p.Hitbox, neartiles.Hitbox))
					{
						float xAxis = Math.Clamp(p.Position.X, neartiles.Hitbox.TopLeft.X, neartiles.Hitbox.TopLeft.X + neartiles.Hitbox.Width);
						float yAxis = Math.Clamp(p.Position.Y, neartiles.Hitbox.TopLeft.Y, neartiles.Hitbox.TopLeft.Y + neartiles.Hitbox.Height);
						Vector2 collisionPoint = new Vector2(xAxis, yAxis);



						//Vector2 centerToCollisionPoint = collisionPoint - p.Position;
						//float collisionPushback = p.Hitbox.Radius - centerToCollisionPoint.Length();

						//if (centerToCollisionPoint.X == 0)
						//{
						//	p.Direction = new Vector2(p.Direction.X, 0);
						//	p.Position -= Vector2.Normalize(centerToCollisionPoint) * collisionPushback;
						//}
						//else
						//{
						//	p.Direction = new Vector2(0, p.Direction.Y);
						//	p.Position += Vector2.Normalize(centerToCollisionPoint) * collisionPushback;
						//}





						Vector2 centerToCollisionPoint = collisionPoint - p.Position;
						float distanceToCollision = centerToCollisionPoint.Length();
						float overlap = p.Hitbox.Radius - distanceToCollision;

						if (overlap > 0)
						{
							Vector2 collisionNormal = Vector2.Normalize(centerToCollisionPoint);
							if(centerToCollisionPoint.Length() == 0)
							{
								;//debug line vector NaN NaN
							}

							p.Position -= collisionNormal * overlap;

							Vector2 v = p.Direction;
							Vector2 normal = collisionNormal;

							Vector2 tangent = v - Vector2.Dot(v, normal) * normal;

							p.Direction = tangent;
						}


					}



					//p.Direction += (float)force * direction;


				}

				if (target != Vector2.Zero && target != p.lastTarget)
				{
					//Vector2 targetDirection = target.Value - p.Position;
					//targetDirection = Vector2.Normalize(targetDirection);
					//if(targetDirection.Length() == 0)
					//{
					//	;//debug line vector NaN NaN
					//}
					//p.Direction += targetDirection;





					var startCell = new Vector2((int)(p.Position.X / tileMap.tileSize), (int)(p.Position.Y / tileMap.tileSize));
					var targetCell = new Vector2((int)(target.X / tileMap.tileSize), (int)(target.Y / tileMap.tileSize));

					List<Node> path = AStarPathfinding.AStarPathFinding(tileMap, startCell, targetCell);
					p.Path = new List<Node>(path);
					p.lastTarget = target;
					//if (path != null && path.Count > 1)
					//{
					//	Vector2 nextCellCenter = new Vector2(path[1].X * tileMap.tileSize + 0.5f, path[1].Y * tileMap.tileSize + 0.5f);
					//	Vector2 pathDir = nextCellCenter - p.Position;
					//	if (pathDir != Vector2.Zero)
					//		pathDir = Vector2.Normalize(pathDir);

					//	p.Direction += pathDir;
					//}
				}
				if(p.Path != null && p.Path.Count > 0)
				{
					Vector2 nextCellCenter = new Vector2(p.Path[0].X * tileMap.tileSize + tileMap.tileSize / 2, p.Path[0].Y * tileMap.tileSize + tileMap.tileSize / 2);
					Vector2 pathDirection = nextCellCenter - p.Position;
					float pullStrength = 4f;

					if(pathDirection != Vector2.Zero)
					{
						pathDirection = Vector2.Normalize(pathDirection);
					}
					p.Direction += pathDirection * pullStrength;
					int xPos = (int)p.Position.X;
					int yPos = (int)p.Position.Y;
					int xTarget = (int)nextCellCenter.X;
					int yTarget = (int)nextCellCenter.Y;

					Debug.WriteLine($"{xTarget / 32} == {xPos / 32} és {yTarget / 32} == {yPos / 32}");

					if (xTarget / 32 == xPos / 32 && yTarget / 32 == yPos / 32)
					{
						p.Path.RemoveAt(0);
						Debug.WriteLine("removed " + p.Path.Count);
					}
				}



                float length = p.Direction.Length();

                float currentSpeed = p.Direction.Length();
                float moveSpeed = Math.Max(length, p.Speed);
                float speedTolerance = 2f;

				
				if (p.Direction.Length() < p.Speed - speedTolerance)
				{
					float accel = (float)acceleration.Length() * (float)gameTime.ElapsedGameTime.TotalSeconds;
					Vector2 dirNorm = Vector2.Zero;
					if(p.Direction != Vector2.Zero)
					{
						dirNorm = Vector2.Normalize(p.Direction);
						if(p.Direction.Length() == 0)
						{
							;//debug line vector NaN NaN
						}
					}
					p.Direction += dirNorm * accel;

					if (p.Direction.Length() > p.Speed)
						p.Direction = dirNorm * p.Speed;
				}
				if (p.Direction.Length() > p.Speed)
                {
					if(p.Direction != Vector2.Zero)
					{
						p.Direction = Vector2.Normalize(p.Direction) * p.Speed;
						if (p.Direction.Length() == 0)
						{
							;//debug line vector NaN NaN
						}
					}
				}
                if(p.Direction.Length() > 0)
                {
                    p.Position += p.Direction * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }




                
				//Debug.WriteLine("position :" + p.Position + " speed: " + p.Direction.Length() + " direction: " + p.Direction);
				p.Hitbox.Center = p.Position;



                
			}
        }
		float GetSignedAngle(Vector2 a, Vector2 b)
		{
			a = Vector2.Normalize(a);
			b = Vector2.Normalize(b);
			if(a.Length() ==0 || b.Length() == 0)
			{
				;//debug line vector NaN NaN
			}

			float angle = MathF.Atan2(b.Y, b.X) - MathF.Atan2(a.Y, a.X);

			if (angle > MathF.PI)
				angle -= MathF.Tau;
			else if (angle < -MathF.PI)
				angle += MathF.Tau;

			return angle;
		}
		Vector2 RotateVector(Vector2 v, float angleRadians)
		{
			float cos = MathF.Cos(angleRadians);
			float sin = MathF.Sin(angleRadians);

			return new Vector2(
				v.X * cos - v.Y * sin,
				v.X * sin + v.Y * cos
			);
		}
	}
}
