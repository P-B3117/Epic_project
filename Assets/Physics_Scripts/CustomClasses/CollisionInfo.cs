using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionInfo
{
    private Vector3 minimumTranslationVector;
    private Vector3 vertexOfCollision;
    public CollisionInfo() 
    {
        minimumTranslationVector = Vector3.zero;
        vertexOfCollision = Vector3.zero;
    }


    public Vector3 GetMTV() 
    {
        return minimumTranslationVector;
    }

    public void SetMTV(Vector3 MTV) 
    {
        minimumTranslationVector = MTV;
    }

    public Vector3 GetVertexOfCollision() 
    {
        return vertexOfCollision;
    }
    public void SetVertexOfCollision(Vector3 VOC) 
    {
        vertexOfCollision = VOC;
    }
}