using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Filename : SoftBody
 * Author: Louis-E
 * Goal : Puts a skin (mesh) over softbodies to make them appear as soft material
 * 
 * Requirements : Put this script in the parent of a softbody
 */
public class SoftBody : MonoBehaviour
{

    public List<GameObject> points;
    public Material material;
    public float size = 1.0f;
    ///Wind up clockwise!!!!!!
    public int type = 0;

    //Get all the necessery references in order to allow the drawing of the mesh
    public void Initialise()
    {
        if(GetComponent<MeshRenderer>() == null) { this.gameObject.AddComponent<MeshRenderer>(); }
        if (GetComponent<MeshFilter>() == null) { this.gameObject.AddComponent<MeshFilter>(); }
        

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
        Vector3[] modelPoints = new Vector3[points.Count];
        if (points.Count > 5) type = 1;
        if (points.Count > 11) type = 2;
        for (int i = 0; i < points.Count; i++)
        {
            modelPoints[i] = points[i].transform.position;
        }
        mesh.vertices = modelPoints;
        int[] newTriangles = new int[(points.Count - 1) * 3];
        for (int i = 0; i < points.Count - 1; i++)
        {

            newTriangles[i * 3] = 0;
            newTriangles[i * 3 + 1] = i + 1;
            newTriangles[(i * 3 + 2)] = (i + 2) % (points.Count - 1);

            if (newTriangles[(i * 3 + 2)] == 0) { newTriangles[(i * 3 + 2)] = points.Count - 1; }

        }
       
        mesh.triangles = newTriangles;



        //Disables the drawing of the line renderer
		LineRenderer[] listLR = GetComponentsInChildren<LineRenderer>();
		for (int i = 0; i < listLR.Length; i++)
		{
			listLR[i].enabled = false;
		}

        //Disables the drawing of the physicobjects underneath the softbody
		MeshRenderer[] listMR = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < listMR.Length; i++)
		{
			if (!listMR[i].name.Equals(this.name))
			{
				listMR[i].enabled = false;
			}
		}

	}

    //Update the appearance of the mesh each frame
    public void UpdateSoftBody() 
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
       
        Vector3[] modelPoints = new Vector3[points.Count];

        
           
        modelPoints[0] = points[0].transform.position;
       
        
        List<GameObject> aroundPoints = new List<GameObject>(points);
        aroundPoints.RemoveAt(0);
        for (int i = 0; i < aroundPoints.Count; i++)
        {
            int index0 = ((i - 1) + aroundPoints.Count) % aroundPoints.Count;
            Vector3 p0 = aroundPoints[index0].transform.position;
            Vector3 p2 = aroundPoints[(i + 1) % aroundPoints.Count].transform.position;
            Vector3 p1 = aroundPoints[i].transform.position;
            Vector3 dir1 = (p2 - p1).normalized;
            Vector3 dir2 = (p0 - p1).normalized;
            
            Vector3 result = -(dir1+dir2).normalized;

            p1 += result * size;
             
            modelPoints[i + 1] = p1;
        }
        mesh.vertices = modelPoints;
        int[] newTriangles = new int[(points.Count - 1) * 3];
        for (int i = 0; i < points.Count - 1; i++)
        {

            newTriangles[i * 3] = 0;
            newTriangles[i * 3 + 1] = i + 1;
            newTriangles[(i * 3 + 2)] = (i + 2) % (points.Count - 1);

            if (newTriangles[(i * 3 + 2)] == 0) { newTriangles[(i * 3 + 2)] = points.Count - 1; }

        }

        mesh.triangles = newTriangles;
    }

    public void Move(Vector3 displacement)
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].transform.position += displacement;
            //Debug.Log(displacement);
        }
    }
    public void OnDestroy()
    {
        for (int i = points.Count-1; i >= 0 ; i--)
        {
            Destroy(points[i]);
            points.RemoveAt(i);
        }
    }
    public void Update()
	{
        if (points.Count > 0)
        {
            UpdateSoftBody();
        }
	}

    public void SetBasicMaterial() 
    {
        GetComponent<MeshRenderer>().material = this.material;
    }
    public void SetShadowMaterial(PrefabsHolder ph) 
    {
        GetComponent<MeshRenderer>().material = ph.GetSoftBodyShadowMaterial();
    }
}

