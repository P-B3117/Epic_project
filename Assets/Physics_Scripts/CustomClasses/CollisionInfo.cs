using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionInfo
{
    private Vector3 minimumTranslationVector;
    private Vector3 vertexMTV;
    private Vector3 contactPoint;
    private int collisionRef;
    
    public CollisionInfo() 
    {
        minimumTranslationVector = Vector3.zero;
        vertexMTV = Vector3.zero;
        contactPoint = Vector3.zero;
        collisionRef = 0;
    }


    public Vector3 GetMTV() 
    {
        return minimumTranslationVector;
    }

    public void SetMTV(Vector3 MTV) 
    {
        minimumTranslationVector = MTV;
    }

    public Vector3 GetVertexMTV() 
    {
        return vertexMTV;
    }
    public void SetVertexMTV(Vector3 contactP) 
    {
        vertexMTV = contactP;
    }

    public Vector3 GetContactPoint() 
    {
        return contactPoint;
    }

    public int GetCollisionRef() 
    {
        return collisionRef;
    }

    public void SetCollisionRef(int colRef) 
    {
        if (colRef > 1 || colRef < 0) { return; }
        collisionRef = colRef;
    }

   

}
