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
    
   
    Vector3 velocity;
    int index;

    intPosition gridPosition;
    GameObject particle;
	
    public Particle(Vector3 initialPos, Vector3 initialVel, int i, float radius) 
    {
        particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        particle.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        
        particle.transform.position = initialPos;
        velocity = initialVel;
        index = i;
    }

    public Vector3 GetPosition() 
    {
        return particle.transform.position;
    }
    public void SetPosition(Vector3 newPos) 
    {
        particle.transform.position = newPos;
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
}
