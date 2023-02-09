using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctionClass
{


	//Collision testing using the seperate axis theorem
	//Code adapted from the C++ video : Convex Polygon Collisions #1
	//From the Youtube Channel : javidx9
	//URL : https://www.youtube.com/watch?v=7Ik2vowGcU0&ab_channel=javidx9
	public static bool TestCollisionSeperateAxisTheorem(List<Vector3> polygon1 , List<Vector3> polygon2)
	{

		//Test shape 1
		for (int a = 0; a < polygon1.Count; a++)
		{
			int b = (a + 1) % polygon1.Count;

			Vector3 axisProj = new Vector3(-(polygon1[b].y - polygon1[a].y), (polygon1[b].x - polygon1[a].x), 0);

			//Projection of this specific shape
			float min_1 = Mathf.Infinity; float max_1 = -Mathf.Infinity;
			for (int i = 0; i < polygon1.Count; i++)
			{
				float q = polygon1[i].x * axisProj.x + polygon1[i].y * axisProj.y;
				min_1 = Mathf.Min(min_1, q);
				max_1 = Mathf.Max(max_1, q);
			}


			//Projection of the other shape
			float min_2 = Mathf.Infinity; float max_2 = -Mathf.Infinity;
			for (int i = 0; i < polygon2.Count; i++)
			{
				float q = polygon2[i].x * axisProj.x + polygon2[i].y * axisProj.y;
				min_2 = Mathf.Min(min_2, q);
				max_2 = Mathf.Max(max_2, q);
			}

			if (!(max_2 >= min_1 && max_1 >= min_2))
				return false;


		}

		

		//Test shape 2
		for (int a = 0; a < polygon2.Count; a++)
		{
			int b = (a + 1) % polygon2.Count;

			Vector3 axisProj = new Vector3(-(polygon2[b].y - polygon2[a].y), (polygon2[b].x - polygon2[a].x), 0);

			//Projection of this specific shape
			float min_1 = Mathf.Infinity; float max_1 = -Mathf.Infinity;
			for (int i = 0; i < polygon2.Count; i++)
			{
				float q = polygon2[i].x * axisProj.x + polygon2[i].y * axisProj.y;
				min_1 = Mathf.Min(min_1, q);
				max_1 = Mathf.Max(max_1, q);
			}


			//Projection of the other shape
			float min_2 = Mathf.Infinity; float max_2 = -Mathf.Infinity;
			for (int i = 0; i < polygon1.Count; i++)
			{
				float q = polygon1[i].x * axisProj.x + polygon1[i].y * axisProj.y;
				min_2 = Mathf.Min(min_2, q);
				max_2 = Mathf.Max(max_2, q);
			}

			if (!(max_2 >= min_1 && max_1 >= min_2))
				return false;


		}

		return true;

	}
}
