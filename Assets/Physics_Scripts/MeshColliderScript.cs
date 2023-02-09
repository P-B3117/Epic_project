using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderScript : MonoBehaviour
{
    [SerializeField]
    List<Vector3> modelPoints;

	
	List<Vector3> worldSpaceRotatedPoints;

	[SerializeField]
	Material material;

  


    // Start is called before the first frame update
    void Start()
    {
        if (modelPoints.Count < 3)
            return;



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
			newTriangles[i * 3 + 1] = i+1;
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






}
