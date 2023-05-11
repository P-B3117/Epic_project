using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Filename : HelperFunctionClass
 * Author : Louis-E, Justin
 * Goal : Static class that encapsulate all of the CollisionResponse algorithm's
 * 
 * Requirements : NaN (The class is static and all of it's functions aswell) 
 */
public static class HelperFunctionClass
{

	//COLLISION0
	//AABB Collisions (rectangle vs rectangle)
	public static bool AABBCollision(MeshColliderScript m1, MeshColliderScript m2) 
	{
		Rect r1 = m1.GetBoundariesAABB();
		Rect r2 = m2.GetBoundariesAABB();

		
		if (r1.x + r1.width < r2.x || r2.x + r2.width < r1.x || r1.y + r1.height < r2.y || r2.y + r2.height < r1.y) { return false; }

		return true;
	}


	//COLLISION1
	//Collision testing using the seperate axis theorem
	//Code adapted from the C++ video : Convex Polygon Collisions #1
	//From the Youtube Channel : javidx9
	//URL : https://www.youtube.com/watch?v=7Ik2vowGcU0&ab_channel=javidx9
	public static CollisionInfo TestCollisionSeperateAxisTheorem(List<Vector3> polygon1, List<Vector3> polygon2)
	{

		Vector3 findingMinimumTranslationVector = Vector3.zero;
		float findingMinimumTranslationVectorLength = Mathf.Infinity;


		CollisionInfo col = new CollisionInfo();

		List<Vector3> p1;
		List<Vector3> p2;
		for (int shape = 0; shape < 2; shape++)
		{
			//Test shape 1 then 2

			if (shape == 0) { p1 = new List<Vector3>(polygon1); p2 = new List<Vector3>(polygon2); }
			else { p2 = new List<Vector3>(polygon1); p1 = new List<Vector3>(polygon2); }
			for (int a = 0; a < p1.Count; a++)
			{
				int b = (a + 1) % p1.Count;

				Vector3 axisProj = new Vector3(-(p1[b].y - p1[a].y), (p1[b].x - p1[a].x), 0).normalized;



				//Projection of this specific shape
				float min_1 = Mathf.Infinity; float max_1 = -Mathf.Infinity;
				for (int i = 0; i < p1.Count; i++)
				{
					float q = p1[i].x * axisProj.x + p1[i].y * axisProj.y;
					min_1 = Mathf.Min(min_1, q);
					max_1 = Mathf.Max(max_1, q);
				}


				//Projection of the other shape

				float min_2 = Mathf.Infinity; float max_2 = -Mathf.Infinity;
				for (int i = 0; i < p2.Count; i++)
				{
					float q = p2[i].x * axisProj.x + p2[i].y * axisProj.y;
					if (q < min_2)
					{
						min_2 = q;

					}

					if (q > max_2)
					{
						max_2 = Mathf.Max(max_2, q);

					}

				}

				if (!(max_2 >= min_1 && max_1 >= min_2))
					return null;


				//Find the shortest translation vector
				float overlap = Mathf.Min(max_1, max_2) - Mathf.Max(min_1, min_2);
				if ((max_1 > max_2 && min_1 < min_2) ||
					(max_1 < max_2 && min_1 > min_2))
				{
					float mins = Mathf.Abs(min_1 - min_2);
					float maxs = Mathf.Abs(max_1 - max_2);
					if (mins < maxs)
					{
						overlap += mins;
					}
					else
					{
						overlap += maxs;
						axisProj *= -1;

					}
				}
				if (overlap < findingMinimumTranslationVectorLength)
				{
					findingMinimumTranslationVectorLength = overlap;
					findingMinimumTranslationVector = axisProj;

					col.SetCollisionRef(shape);
					if (max_1 > max_2)
					{
						findingMinimumTranslationVector *= -1;

					}

				}

			}

		}
		//Get the necessary MTV
		col.SetMTV(findingMinimumTranslationVector.normalized * findingMinimumTranslationVectorLength);
		return col;

	}


	//COLLISION2
	//Collision testing using the diagonals theorem (Polygon vs Polygon) (CANNOT FIND THE MTV, the algorithm's is NOT used)
	//Code adapted from the C++ video : Convex Polygon Collisions #1
	//From the Youtube Channel : javidx9
	//URL : https://www.youtube.com/watch?v=7Ik2vowGcU0&ab_channel=javidx9
	public static CollisionInfo TestCollisionDiagonalsTheorem(Vector3 originPolygon1, List<Vector3> polygon1, Vector3 originPolygon2, List<Vector3> polygon2) 
	{
		List<Vector3> p1;
		List<Vector3> p2;
		for (int shape = 0; shape < 2; shape++) 
		{
			if (shape == 0) { p1 = new List<Vector3>(polygon1); p2 = new List<Vector3>(polygon2); }
			else { p2 = new List<Vector3>(polygon1); p1 = new List<Vector3>(polygon2); Vector3 temp = originPolygon1; originPolygon1 = originPolygon2; originPolygon2 = temp; }
			
			for (int p = 0; p < p1.Count; p++) 
			{
				Vector3 line1s = originPolygon1;
				Vector3 line1e = p1[p];

				Vector3 displacement = Vector3.zero;

				for (int q = 0; q < p2.Count; q++) 
				{
					Vector3 line2s = p2[q];
					Vector3 line2e = p2[(q+1) % p2.Count];

					float h = (line2e.x - line2s.x) * (line1s.y - line1e.y) - (line1s.x - line1e.x) * (line2e.y - line2s.y);
					float t1 = ((line2s.y - line2e.y) * (line1s.x - line2s.x) + (line2e.x - line2s.x) * (line1s.y-line2s.y)) / h;
					float t2 = ((line1s.y - line1e.y) * (line1s.x - line2s.x) + (line1e.x - line1s.x) * (line1s.y-line2s.y)) / h;

					if (t1 >= 0.0f && t1 < 1.0f && t2 >= 0.0f && t2 < 1.0f) 
					{

						displacement.x += (1.0f - t1) * (line1e.x - line1s.x);
						displacement.y += (1.0f - t1) * (line1e.y - line1s.y);
						CollisionInfo col = new CollisionInfo();

						return col;
					}
				}

				
			}

		}

		return null;
	}


	//COLLISION3
	//Collision testing (Circle vs Circle)
	public static CollisionInfo TestCollisionTwoCircles(Vector3 c1, float s1, Vector3 c2, float s2) 
	{
		CollisionInfo col = new CollisionInfo();

		Vector3 diff = c2 - c1;
		if (diff.magnitude > s1 + s2) { return null; }
		else 
		{
			Vector3 axisProj = diff.normalized;

			Vector3 min1 = c1 + axisProj * s1;
			Vector3 min2 = c2 - axisProj * s2;
			float length = Vector3.Dot(min2, axisProj) - Vector3.Dot(min1, axisProj);


			col.SetCollisionRef(0);
			col.SetMTV(-axisProj * length);
			
			return col;
		}
	}

	//COLLISION4
	//Collision testing (Circle vs Polygon)
	//Code adapted from Jeffrey Thompson's blog
	//URL : https://www.jeffreythompson.org/collision-detection/poly-circle.php
	public static CollisionInfo TestCollisionPolygonCircle(List<Vector3> p1, Vector3 polygonCenter,  Vector3 circlePosition, float circleRayonSize) 
	{
		CollisionInfo col = null;

		for (int a = 0; a < p1.Count; a++)
		{
			int b = (a + 1) % p1.Count;
			int c = (a - 1 + p1.Count) % p1.Count;
			col  = lineCircle(col , polygonCenter,  p1[a], p1[c], p1[b], circlePosition, circleRayonSize);
			
			
		}
		//Check if circle is inside a polygon
		//if (col == null)
		//{
		//	bool centerInside = polygonPoint(p1, circlePosition);
		//	if (centerInside)
		//	{

		//		col = new CollisionInfo();
		//		Vector3 smallestMTV = Vector3.zero;
		//		float length = Mathf.Infinity;

		//		for (int a = 0; a < p1.Count; a++)
		//		{
		//			int b = (a + 1) % p1.Count;

		//			Vector3 reference = (p1[b] - p1[a]).normalized;
		//			Vector3 circlePositionReference = circlePosition - p1[a];
		//			float referenceLength = Vector3.Dot(circlePositionReference, reference);

		//			Vector3 point = reference * referenceLength + p1[a];
		//			Vector3 mtv = circlePosition - point;
		//			if (mtv.magnitude < length)
		//			{
		//				length = mtv.magnitude;
		//				smallestMTV = mtv.normalized * (length + circleRayonSize);
		//			}


		//		}

		//		col.SetMTV(-smallestMTV);
		//	}
		//}


		return col;



	}
	//SubFunction used in the Circle vs Polygon algorithm
	//Detect if the circle is inside the polygon
	public static bool polygonPoint(List<Vector3> p1, Vector3 circlePosition)
	{
		int count = 0;
		for (int i = 0; i < p1.Count; i++) 
		{
			Vector3 m1 = p1[i];
			Vector3 m2 = p1[(i + 1) % p1.Count];
			if (m1.y == m2.y || circlePosition.y < Mathf.Min(m1.y, m2.y) || circlePosition.y > Mathf.Max(m1.y, m2.y)) 
			{
				continue;
			}
			float x_intercept = (circlePosition.y - m1.y) * (m2.x - m1.x) / (m2.y - m1.y) + m1.x;
			if (x_intercept > circlePosition.x) { count++; }
		}

		return count % 2 == 1;
	}
	//SubFunction used in the Circle vs Polygon algorithm
	//Detect if there is a collision between the circle and a line of the polygon
	private static CollisionInfo lineCircle(CollisionInfo col, Vector3 polygonCenter, Vector3 p1, Vector3 p0,Vector3 p2, Vector3 circlePosition, float circleRayonSize) 
	{
		col = (pointCircle(col, polygonCenter, p1, p0, p2, circlePosition, circleRayonSize));
		

		float lineLength = (p1 - p2).magnitude;
		float dot = (((circlePosition.x - p1.x) * (p2.x - p1.x)) + ((circlePosition.y - p1.y) * (p2.y - p1.y))) / Mathf.Pow(lineLength, 2);

		float closestX = p1.x + (dot * (p2.x - p1.x));
		float closestY = p1.y + (dot * (p2.y - p1.y));

		if (!linePoint(p1, p2, new Vector3(closestX, closestY, 0))) { return col; }
		

		float distanceToPoint = (new Vector3(closestX, closestY, 0) - circlePosition).magnitude;

		if (distanceToPoint < circleRayonSize) 
		{

			
			
			
			Vector3 line = (p2 - p1).normalized;
			Vector3 MTVDIR = new Vector3(-line.y, line.x);
			Vector3 extremePoint = circlePosition - MTVDIR * circleRayonSize;
			float projection1 = Vector3.Dot(MTVDIR, extremePoint);
			float projection2 = Vector3.Dot(MTVDIR, new Vector3(closestX, closestY, 0));
			float MTVLENGTH = projection2 - projection1;

			if (col == null)
			{
				col = new CollisionInfo();
				col.SetMTV(MTVDIR * MTVLENGTH);
			}
			else if (MTVLENGTH < col.GetMTV().magnitude) { col.SetMTV(MTVDIR * MTVLENGTH); }
			return col; 
		
		}
		else { return col; }
	}
	//SubFunction used in the Circle vs Polygon algorithm
	//Detect if there is a collision between a point of the polygon and the circle
	private static CollisionInfo pointCircle(CollisionInfo col, Vector3 polygonCenter, Vector3 a, Vector3 c, Vector3 b, Vector3 circlePosition, float circleRayonSize) 
	{
		float distance = (circlePosition - a).magnitude;
		if (distance < circleRayonSize) 
		{
			Vector3 MTVDIR = (circlePosition - a).normalized;
			Vector3 reference = (circlePosition - polygonCenter).normalized;
			if (Vector3.Dot(MTVDIR, reference) < 0) { MTVDIR *= -1; }

			Vector3 line1 = (a- c).normalized;
			float angle1 = Mathf.Acos(Vector3.Dot(MTVDIR, line1));
			if (angle1 > Mathf.PI/2) { return col; }

			Vector3 line2 = (a - b).normalized;
			float angle2 = Mathf.Acos(Vector3.Dot(MTVDIR, line2));
			if (angle2 > Mathf.PI / 2) { return col; }

			Vector3 extremePoint = circlePosition - MTVDIR * circleRayonSize;
			float projection1 = Vector3.Dot(MTVDIR, extremePoint);
			float projection2 = Vector3.Dot(MTVDIR, a);
			float MTVLENGTH = projection2 - projection1;

			if (col == null)
			{
				col = new CollisionInfo();
				col.SetMTV(MTVDIR * MTVLENGTH);
			}
			else if (MTVLENGTH < col.GetMTV().magnitude) { col.SetMTV(MTVDIR * MTVLENGTH); }

			
			return col; 
		}
		else { return col; }
	}

	//SubFunction used in the Circle vs Polygon algorithm
	//Detect if there is a collision between a point and a line
	private static bool linePoint(Vector3 p1, Vector3 p2, Vector3 Point) 
	{
		float d1 = (Point - p1).magnitude;
		float d2 = (Point - p2).magnitude;
		float lineLength = (p1 - p2).magnitude;

		float buffer = 0.1f;

		if (d1 + d2 >= lineLength - buffer && d1 + d2 <= lineLength + buffer)
		{
			return true;
		}
		else 
		{
			return false;
		}
	}


	

	

	//FINDCOLLISIONPOINT1
	//Find collisionPoint (Polygon vs Polygon)
	public static CollisionInfo FindCollisionPoint(CollisionInfo col, List<Vector3> polygon1, List<Vector3> polygon2) 
	{
		
		//Find the optimalCollisionPoint!!
		List<Vector3> p1;
		List<Vector3> p2;
		p1 = new List<Vector3>(polygon1); p2 = new List<Vector3>(polygon2); 
		
		
		//P2 is the object that collided with the other one!
		Vector3 MTVProj = col.GetMTV().normalized;
		
		double min = Mathf.Infinity;
		List<int> indexMin = new List<int>();
		for (int i = 0; i < p2.Count; i++)
		{
			
			double d = Vector3.Dot(MTVProj, p2[i]);
			d = System.Math.Round(d, 3);
			
			if (d < min) { min = d; indexMin = new List<int>(); indexMin.Add(i); }
			else if (d == min) { indexMin.Add(i); }
		}
		

		//Point with edge
		if (indexMin.Count < 2)
		{
			
			col.SetContactPoint(p2[indexMin[0]]);
			
			
		}
		//Edge with edge
		else
		{
			//Get the edge point of the other object
			double min2 = -Mathf.Infinity;
			List<int> indexMin2 = new List<int>();
			for (int i = 0; i < p1.Count; i++)
			{

				double d2 = Vector3.Dot(MTVProj, p1[i]);
				d2 = System.Math.Round(d2, 3);

				if (d2 > min2) { min2 = d2; indexMin2 = new List<int>(); indexMin2.Add(i); }
				else if (d2 == min2) { indexMin2.Add(i); }
			}

			Vector3 MTVEdgeProj = new Vector3(-MTVProj.y, MTVProj.x);
			float[] p1extreme = { Mathf.Infinity, -Mathf.Infinity };
			float[] p2extreme = { Mathf.Infinity, -Mathf.Infinity };

			int[] p1Index = { -1, -1 };
			int[] p2Index = { -1, -1 };

			for (int i = 0; i < indexMin2.Count; i++)
			{
				float p = Vector3.Dot(MTVEdgeProj, p1[indexMin2[i]]);
				if (p < p1extreme[0])
				{
					p1Index[0] = i;
					p1extreme[0] = p;
				}
				if (p > p1extreme[1])
				{
					p1Index[1] = i;
					p1extreme[1] = p;
				}


			}

			for (int i = 0; i < indexMin.Count; i++)
			{
				float p = Vector3.Dot(MTVEdgeProj, p2[indexMin[i]]);
				if (p < p2extreme[0])
				{
					p2Index[0] = i;
					p2extreme[0] = p;
				}
				if (p > p2extreme[1])
				{
					p2Index[1] = i;
					p2extreme[1] = p;
				}
			}
			
			Vector3[] twoPoints = new Vector3[2];
			int index = 0;
			if (p2extreme[0] > p1extreme[0] && p2extreme[0] < p1extreme[1]) { twoPoints[index] = p2[indexMin[p2Index[0]]]; index++;  }
			if (p2extreme[1] > p1extreme[0] && p2extreme[1] < p1extreme[1]) { twoPoints[index] = p2[indexMin[p2Index[1]]]; index++;  }

			if (p1extreme[0] > p2extreme[0] && p1extreme[0] < p2extreme[1]) { twoPoints[index] = p1[indexMin2[p1Index[0]]]; index++;  }
			if (p1extreme[1] > p2extreme[0] && p1extreme[1] < p2extreme[1]) { twoPoints[index] = p1[indexMin2[p1Index[1]]]; index++;  }

	
			col.SetContactPoint((twoPoints[0] + twoPoints[1]) / 2);

		}
		if (col.GetContactPoint() == Vector3.zero) { return null; }
		return col;
	}


	//FINDCOLLISIONPOINT2
	//Find collisionPoint (Circle vs Circle)
	public static CollisionInfo FindCollisionPointTwoCircles(CollisionInfo col, Vector3 c1, float s1, Vector3 c2, float s2) 
	{
		Vector3 diff = c2 - c1;
		Vector3 p = diff.normalized * s1;
		col.SetContactPoint(c1 + p);
		return col;
		
	
	}
	//FINDCOLLISIONPOINT3
	//Find collisionPoint (Polygon vs Circle)
	public static CollisionInfo FindCollisionPointPolygonCircle(CollisionInfo col, Vector3 c1, float s1)
	{
		
		col.SetContactPoint(c1 - col.GetMTV().normalized * s1);
		return col;


	}



	//Line intersection testing 
	//Code adapted from this Stack Overflow's thread :
	//https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect
	public static bool LineIntersect(Vector3 A, Vector3 B, Vector3 C, Vector3 D) 
	{
		return (CCW(A,C,D) != CCW(B,C,D)) && (CCW(A,B,C) != CCW(A,B,D));
	}
	private static bool CCW(Vector3 A, Vector3 B, Vector3 C) 
	{
		return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
	}
}
