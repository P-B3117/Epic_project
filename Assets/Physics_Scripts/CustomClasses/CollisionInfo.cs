using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionInfo
{
    private Vector3 minimumTranslationVector;
    
    private Vector3 contactPoint;
    private int collisionRef;
    
    public CollisionInfo() 
    {
        minimumTranslationVector = Vector3.zero;
        
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

    

    public Vector3 GetWorldContactPoint() 
    {
        return contactPoint;
    }


    public Vector3 GetRelativeContactPoint(Vector3 centerOfMassSpacePoint)
    {
        Vector3 relativeContactPoint = contactPoint - centerOfMassSpacePoint;
        return relativeContactPoint;
        
    }


    public void SetContactPoint(Vector3 CP) 
    {
        contactPoint = CP;
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


    public bool isCBetweenAB(Vector3 A, Vector3 B, Vector3 C)
    {
        return Vector3.Dot((B - A).normalized, (C - B).normalized) < 0f && Vector3.Dot((A - B).normalized, (C - A).normalized) < 0f;
    }
}
