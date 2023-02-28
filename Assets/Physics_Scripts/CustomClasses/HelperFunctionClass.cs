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



	public static CollisionInfo FindCollisionPoint(CollisionInfo col, List<Vector3> polygon1, List<Vector3> polygon2) 
	{
		
		//Find the optimalCollisionPoint!!
		List<Vector3> p1;
		List<Vector3> p2;
		if (col.GetCollisionRef() == 0) { p1 = new List<Vector3>(polygon1); p2 = new List<Vector3>(polygon2); }
		else { p2 = new List<Vector3>(polygon1); p1 = new List<Vector3>(polygon2); }
		//P2 is the object that collided with the other one!
		Vector3 MTVProj = col.GetMTV().normalized;
		
		double min = Mathf.Infinity;
		List<int> indexMin = new List<int>();
		for (int i = 0; i < p2.Count; i++)
		{
			
			double d = Vector3.Dot(MTVProj, p2[i]);
			d = System.Math.Round(d, 4);
			
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
				d2 = System.Math.Round(d2, 4);

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
			//Debug.Log("P1 extreme : " + p1extreme[0] + " --- " + p1extreme[1]);
			//Debug.Log("P2 extreme : " + p2extreme[0] + " --- " + p2extreme[1]);
			Vector3[] twoPoints = new Vector3[2];
			int index = 0;
			if (p2extreme[0] > p1extreme[0] && p2extreme[0] < p1extreme[1]) { twoPoints[index] = p2[indexMin[p2Index[0]]]; index++;  }
			if (p2extreme[1] > p1extreme[0] && p2extreme[1] < p1extreme[1]) { twoPoints[index] = p2[indexMin[p2Index[1]]]; index++;  }

			if (p1extreme[0] > p2extreme[0] && p1extreme[0] < p2extreme[1]) { twoPoints[index] = p1[indexMin2[p1Index[0]]]; index++;  }
			if (p1extreme[1] > p2extreme[0] && p1extreme[1] < p2extreme[1]) { twoPoints[index] = p1[indexMin2[p1Index[1]]]; index++;  }

			//Debug.Log((twoPoints[0] + " --- " + twoPoints[1]));
			col.SetContactPoint((twoPoints[0] + twoPoints[1]) / 2);




		}

		return col;
	}
}
