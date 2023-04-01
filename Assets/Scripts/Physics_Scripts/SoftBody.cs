using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBody : MonoBehaviour
{

    public List<Vector3> modelPoints;
    public Material material;
    ///Wind up clockwise!!!!!!

    public void Initialise()
    {
        this.gameObject.AddComponent<MeshRenderer>();
        this.gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
        mesh.vertices = modelPoints.ToArray();
        int[] newTriangles = new int[(modelPoints.Count - 1) * 3];
        for (int i = 0; i < modelPoints.Count - 1; i++)
        {
            newTriangles[i * 3] = 0;
            newTriangles[i * 3 + 1] = i + 1;
            newTriangles[(i * 3 + 2)] = (i + 2) % modelPoints.Count - 1;

            if (newTriangles[(i * 3 + 2)] == 0) { newTriangles[(i * 3 + 2)] = modelPoints.Count - 1; }

        }

        mesh.triangles = newTriangles;
    }
}

