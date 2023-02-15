using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctionClass
{



	//Collision testing using the diagonals theorem
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

				for (int q = 0; q < p2.Count; q++) 
				{
					Vector3 line2s = p2[q];
					Vector3 line2e = p2[(q+1) % p2.Count];

					float h = (line2e.x - line2s.x) * (line1s.y - line1e.y) - (line1s.x - line1e.x) * (line2e.y - line2s.y);
					float t1 = ((line2s.y - line2e.y) * (line1s.x - line2s.x) + (line2e.x - line2s.x) * (line1s.y-line2s.y)) / h;
					float t2 = ((line1s.y - line1e.y) * (line1s.x - line2s.x) + (line1e.x - line1s.x) * (line1s.y-line2s.y)) / h;

					if (t1 >= 0.0f && t1 < 1.0f && t2 >= 0.0f && t2 < 1.0f) 
					{
						CollisionInfo col = new CollisionInfo();
						return col;
					}
				}
			}

		}

		return null;
	}




	//Collision testing using the seperate axis theorem
	//Code adapted from the C++ video : Convex Polygon Collisions #1
	//From the Youtube Channel : javidx9
	//URL : https://www.youtube.com/watch?v=7Ik2vowGcU0&ab_channel=javidx9
	public static CollisionInfo TestCollisionSeperateAxisTheorem(List<Vector3> polygon1 , List<Vector3> polygon2)
	{

		Vector3 findingMinimumTranslationVector = Vector3.zero;
		float findingMinimumTranslationVectorLength = Mathf.Infinity;


		CollisionInfo col = new CollisionInfo();

		List<Vector3> p1;
		List<Vector3> p2;
		for (int shape = 0; shape < 2; shape++) {
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
				int minIndex = -1;
				int maxIndex = -1;
				float min_2 = Mathf.Infinity; float max_2 = -Mathf.Infinity;
				for (int i = 0; i < p2.Count; i++)
				{
					float q = p2[i].x * axisProj.x + p2[i].y * axisProj.y;
					if (q < min_2)
					{
						min_2 = q;
						minIndex = i;
					}

					if (q > max_2)
					{
						max_2 = Mathf.Max(max_2, q);
						maxIndex = i;
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
						int temp = minIndex;
						minIndex = maxIndex;
						maxIndex = temp;
					}
				}
				if (overlap < findingMinimumTranslationVectorLength)
				{
					findingMinimumTranslationVectorLength = overlap;
					findingMinimumTranslationVector = axisProj;
					col.SetVertexOfCollision(p2[minIndex]);

					if (max_1 > max_2)
					{
						findingMinimumTranslationVector *= -1;
						col.SetVertexOfCollision(p2[maxIndex]);
					}

				}

			}

		}



		//Get the necessary MTV
		col.SetMTV(findingMinimumTranslationVector.normalized * findingMinimumTranslationVectorLength);


		return col;

	}
}
