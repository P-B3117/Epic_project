using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Filename : Particle
 * Author:Louis-E
 * Goal : Encapsulate the data of a particle
 * 
 * Requirements : Create instances to create particles
 */
public struct intPosition
{
    public intPosition(int X, int Y)
    {
        x = X;
        y = Y;
    }
    public int x;
    public int y;
}
public class Particle
{
    Vector3 oldPosition;
   
    Vector3 velocity;
    int index;

    intPosition gridPosition;
    GameObject particle;


    List<int> neighbors;

    //Instantiate the particle and create the mesh
    public Particle(Vector3 initialPos, Vector3 initialVel, int i, float radius, PrefabsHolder ph) 
    {
        particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        particle.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        particle.GetComponent<MeshRenderer>().material = ph.GetFluidMaterial();
        Object.Destroy(particle.GetComponent<SphereCollider>());
        particle.transform.position = initialPos;
        oldPosition = initialPos;
        velocity = initialVel;
        index = i;
        

        neighbors = new List<int>();
    }

    public Vector3 GetPosition() 
    {
        return particle.transform.position;
    }
    public Vector3 GetPrevPosition()
    {
        return oldPosition;
    }
    public void SetPosition(Vector3 newPos) 
    {
        particle.transform.position = newPos;
    }
    public void SetPrevPosition(Vector3 newPos)
    {
        oldPosition = newPos;
    }
    public void AddPosition(Vector3 addPos) 
    {
       
         particle.transform.Translate(addPos); 
        
		
        
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
    public void SetVelocity(Vector3 vel) 
    {
        velocity = vel;
    }
    public void AddVelocity(Vector3 addVel) 
    {
        velocity += addVel;
    }
    public int GetIndex() 
    {
        return index;
    }

    public void SetGridPosition(intPosition gridPos) 
    {
        gridPosition = gridPos;
    }
    public intPosition GetGridPosition()
    {
        return gridPosition;
    }

    public List<int> GetNeighbors() 
    {
        return neighbors;
    }

    public void RefreshNeighbors() 
    {
        neighbors.Clear();
    }

    public void AddNeighbor(int p) 
    {
        neighbors.Add(p);
    }


    public void Delete() 
    {
        Object.Destroy(particle);
    }
}
