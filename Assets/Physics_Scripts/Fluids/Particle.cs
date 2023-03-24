using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


    public Particle(Vector3 initialPos, Vector3 initialVel, int i, float radius) 
    {
        particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        particle.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        
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
        particle.transform.position += addPos;
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

    public void InverseVelocityX()
    {
        velocity = new Vector3(-velocity.x, velocity.y, 0);
    }
    public void InverseVelocityY()
    {
        velocity = new Vector3(velocity.x, -velocity.y, 0);
    }
}
