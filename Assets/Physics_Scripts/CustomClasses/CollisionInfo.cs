using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionInfo
{
    private Vector3 minimumTranslationVector;
    private Vector3 vertexMTV;
    private Vector3 contactPoint;
    
    public CollisionInfo() 
    {
        minimumTranslationVector = Vector3.zero;
        vertexMTV = Vector3.zero;
        contactPoint = Vector3.zero;
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

   

}
