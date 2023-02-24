using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderScript : MonoBehaviour
{

	[SerializeField]
	float mass = 5.0f;
	float area;
	float inertia;

	[SerializeField]
    List<Vector3> modelPoints;
	List<Vector3> worldSpaceRotatedPoints;

	[SerializeField]
	Material material;
    Vector3 centerOfMass = Vector3.zero;




    // Start is called before the first frame update
    void Start()
    {
        if (modelPoints.Count < 3)
            return;



		//Calculate the center of mass of a given polygon
		//Code adapted from the source document of Paul Bourke 1988
		//http://paulbourke.net/geometry/polygonmesh/
		area = 0;
		for (int i = 0; i < modelPoints.Count; i++) 
		{
			int j = (i + 1) % modelPoints.Count;
			area += (modelPoints[i].x * modelPoints[j].y) - (modelPoints[j].x * modelPoints[i].y);
		}
		area /= 2;


		centerOfMass= Vector3.zero;
		for (int i = 0; i < modelPoints.Count; i++) 
		{
			int j = (i + 1) % modelPoints.Count;
			this.centerOfMass.x += (modelPoints[i].x + modelPoints[j].x) * ((modelPoints[i].x * modelPoints[j].y) - (modelPoints[j].x * modelPoints[i].y));
			this.centerOfMass.y += (modelPoints[i].y + modelPoints[j].y) * ((modelPoints[i].x * modelPoints[j].y) - (modelPoints[j].x * modelPoints[i].y));
		}
		centerOfMass /= 6 * area;

		//Offset the position of the points to match the center of mass
		for (int i = 0; i < modelPoints.Count; i++) 
		{
			modelPoints[i] -= this.centerOfMass;
		}
		transform.position += this.centerOfMass;




		//Calculate the moment of inertia of the given polygon
		float density = mass / -area;
		inertia = 0;
		for (int i = 0; i < modelPoints.Count; i++) 
		{
			int j = (i + 1) % modelPoints.Count;
			float massTriangle = 0.5f * density * Vector3.Cross(modelPoints[i], modelPoints[j]).magnitude;
			
			float inertiaTriangle = massTriangle * (modelPoints[i].sqrMagnitude + modelPoints[j].sqrMagnitude + Vector3.Dot(modelPoints[i], modelPoints[j])) / 6;
			inertia += inertiaTriangle;

		}
		


		//Create the mesh based on the vertices inputed
		if (GetComponent<MeshFilter>() == null)
		{
			gameObject.AddComponent<MeshFilter>();
		}
		if (GetComponent<MeshRenderer>() == null)
		{
			gameObject.AddComponent<MeshRenderer>();
		}
		List<Vector3> vertices = new List<Vector3>(modelPoints);
		vertices.Insert(0, Vector3.zero);

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshRenderer>().material = material;
		mesh.vertices = vertices.ToArray();
		int[] newTriangles = new int[(modelPoints.Count) * 3];
		for (int i = 0; i < modelPoints.Count; i++)
		{
			newTriangles[i * 3] = 0;
			newTriangles[i * 3 + 1] = i + 1;
			newTriangles[(i * 3 + 2)] = (i + 2) % modelPoints.Count;

			if (newTriangles[(i * 3 + 2)] == 0) { newTriangles[(i * 3 + 2)] = modelPoints.Count; }

		}

		mesh.triangles = newTriangles;

	}


	//Update the points of the collider
	//NECESSARY FOR THE COLLISION DETECTION TO FUNCTION
	public void UpdateColliderOrientation() 
	{
		float rotation = transform.eulerAngles.z * Mathf.Deg2Rad;

		worldSpaceRotatedPoints = new List<Vector3>(modelPoints);
		for (int i = 0; i < worldSpaceRotatedPoints.Count; i++) 
		{
			Vector3 r = worldSpaceRotatedPoints[i];
			worldSpaceRotatedPoints[i] = new Vector3(r.x * Mathf.Cos(rotation) - r.y * Mathf.Sin(rotation), r.x * Mathf.Sin(rotation) + r.y * Mathf.Cos(rotation));
			worldSpaceRotatedPoints[i] += transform.position;
		}


	}




	public List<Vector3> GetWorldSpacePoints()
	{
		return worldSpaceRotatedPoints;
	}

	public void Translate(Vector3 vector) 
	{
		transform.position += vector;
		for (int i = 0; i < worldSpaceRotatedPoints.Count; i++)
		{
			worldSpaceRotatedPoints[i] += vector;
		}
	}
	
	public float GetMass() 
	{
		return mass;
	}
	public float getInertia() 
	{
		return inertia;
	}

	public Vector3 getCenterOfMass()
	{
		return this.centerOfMass;
	}





}
