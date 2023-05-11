using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Filename : MeshColliderScript
 * Author : Charles, Louis-E
 * Goal : Encapsulate the properties of the mesh of the RigidBodies (mass, inertia, shape)
 * 
 * Requirements : Attach this script to physical objects of the Scene with a BasicPhysicObjects script (necessery to have both in order for the good functionning of the code)
 */
public class MeshColliderScript : MonoBehaviour
{
	
	[SerializeField]
	float mass = 5.0f;
	float area;
	float inertia;

	[SerializeField]
    List<Vector3> modelPoints;
	List<Vector3> worldSpaceRotatedPoints;
	List<Vector3> localSpaceRotatedPoints;

	[SerializeField]
	Material material;
	[SerializeField]
	Material shadowMaterial;

	[Header("ONLY IF CIRCLE")]
	[SerializeField]
	bool isCircle = false;
	[SerializeField]
	float rayonOfCircle = 1.0f;

	private Rect boundariesAABB;


	void Start()
    {
		//SetUpMesh();

	}

	public void Initialize(List<Vector3> ModelPoints, PrefabsHolder ph) 
	{
		modelPoints = ModelPoints;
		material = ph.GetBasicUnlitMaterial();
		shadowMaterial = ph.GetShadowObjectMaterial();
	}

	//Update the points of the collider
	//NECESSARY FOR THE COLLISION DETECTION TO FUNCTION
	public void UpdateColliderOrientation() 
	{

		//Update the mesh's points rotation
		float rotation = transform.eulerAngles.z * Mathf.Deg2Rad;

		worldSpaceRotatedPoints = new List<Vector3>(modelPoints);
		localSpaceRotatedPoints = new List<Vector3>(modelPoints);
		for (int i = 0; i < worldSpaceRotatedPoints.Count; i++) 
		{
			Vector3 r = localSpaceRotatedPoints[i];
			localSpaceRotatedPoints[i] = new Vector3(r.x * Mathf.Cos(rotation) - r.y * Mathf.Sin(rotation), r.x * Mathf.Sin(rotation) + r.y * Mathf.Cos(rotation));

			worldSpaceRotatedPoints[i] = transform.position + localSpaceRotatedPoints[i];
		}

		//Update the AABB boundaries
		UpdateAABB();
	}




	//Move the object and move the collider at the same time 
	//(Without using this function, the collider would be out of place when doing the collision's response)
	public void Translate(Vector3 vector) 
	{
		//Update position
		transform.position += vector;

		//Update collider
		for (int i = 0; i < worldSpaceRotatedPoints.Count; i++)
		{
			worldSpaceRotatedPoints[i] += vector;
		}

		//Update AABB collider
		boundariesAABB.x += vector.x;
		boundariesAABB.y += vector.y;
	}


	//Update the basic AABB collider to form a rotation-less rectangle
	private void UpdateAABB() 
	{

		if (isCircle) 
		{
			boundariesAABB = new Rect(new Vector2(-rayonOfCircle + transform.position.x,-rayonOfCircle + transform.position.y), new Vector2(rayonOfCircle*2, rayonOfCircle*2));
		}
		else { 
			float min1 = Mathf.Infinity;
			float max1 = -Mathf.Infinity;
			for (int i = 0; i < localSpaceRotatedPoints.Count; i++) 
			{
				float temp = Vector3.Dot(Vector3.right, localSpaceRotatedPoints[i]);
				min1 = Mathf.Min(temp, min1);
				max1 = Mathf.Max(temp, max1);
			}


			float min2 = Mathf.Infinity;
			float max2 = -Mathf.Infinity;
			for (int i = 0; i < localSpaceRotatedPoints.Count; i++)
			{
				float temp = Vector3.Dot(Vector3.up, localSpaceRotatedPoints[i]);
				min2 = Mathf.Min(temp, min2);
				max2 = Mathf.Max(temp, max2);
			}

			boundariesAABB = new Rect(new Vector2(min1 + transform.position.x, min2+ transform.position.y), new Vector2(max1 - min1, max2 - min2));
		
		}
	}
	
	//Setup the mesh and it's properties
	public void SetUpMesh() 
	{
		if (isCircle)
		{
			//Calculate Area
			area = Mathf.PI * rayonOfCircle * rayonOfCircle;

			//Calculate Inertia
			inertia = 0.5f * mass * rayonOfCircle * rayonOfCircle;


			if (GetComponent<MeshFilter>() == null)
			{
				gameObject.AddComponent<MeshFilter>();
			}
			if (GetComponent<MeshRenderer>() == null)
			{
				gameObject.AddComponent<MeshRenderer>();
				GetComponent<MeshRenderer>().material = material;
			}

			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			GetComponent<MeshFilter>().mesh = sphere.GetComponent<MeshFilter>().mesh;
			
			transform.localScale = new Vector3(rayonOfCircle * 2, rayonOfCircle * 2, rayonOfCircle * 2);
			Destroy(sphere);

		}
		else if (modelPoints.Count < 3)
		{
			return;
		}
		else 
		{
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


			Vector3 centerOfMass = Vector3.zero;
			for (int i = 0; i < modelPoints.Count; i++)
			{
				int j = (i + 1) % modelPoints.Count;
				centerOfMass.x += (modelPoints[i].x + modelPoints[j].x) * ((modelPoints[i].x * modelPoints[j].y) - (modelPoints[j].x * modelPoints[i].y));
				centerOfMass.y += (modelPoints[i].y + modelPoints[j].y) * ((modelPoints[i].x * modelPoints[j].y) - (modelPoints[j].x * modelPoints[i].y));
			}
			centerOfMass /= 6 * area;

			//Offset the position of the points to match the center of mass
			for (int i = 0; i < modelPoints.Count; i++)
			{
				modelPoints[i] -= centerOfMass;
			}
			transform.position += centerOfMass;




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
				GetComponent<MeshRenderer>().material = material;
			}
			List<Vector3> vertices = new List<Vector3>(modelPoints);
			vertices.Insert(0, Vector3.zero);

			Mesh mesh = new Mesh();
			GetComponent<MeshFilter>().mesh = mesh;
			
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

		
		
	}


	public void SetShadowMaterial() 
	{
		
		GetComponent<MeshRenderer>().material = new Material(shadowMaterial);
		
	}
	public void SetBasicMaterial() 
	{
		GetComponent<MeshRenderer>().material = new Material(this.material);
	}
	public void SetSelectedMaterial(PrefabsHolder ph) 
	{
		GetComponent<MeshRenderer>().material = ph.GetSelectedObjectMaterial();
	}

	public List<Vector3> GetWorldSpacePoints()
	{
		return worldSpaceRotatedPoints;
	}
	public float GetMass()
	{
		return mass;
	}
	public void SetMass(float mass) 
	{
		this.mass = mass;
		SetUpMesh();
	}
	public float GetInertia()
	{
		return inertia;
	}

	public bool IsCircle()
	{
		return isCircle;
	}
	public float RayonOfCircle()
	{
		return rayonOfCircle;
	}

	public Rect GetBoundariesAABB() 
	{
		return boundariesAABB;
	}
	public void setRadius(float radius)
    {
		this.rayonOfCircle = radius; 
    }
}
