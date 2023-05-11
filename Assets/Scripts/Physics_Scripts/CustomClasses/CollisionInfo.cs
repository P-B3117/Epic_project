using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* Filename : CollisionInfo
 * Author : Justin,Louis-E
 * Goal : Datatype that encapsulate all the informations about collisions
 * 
 * Requirements : Common return type used during collision detection and collision response (Do not use outside of these purposes)
 */
public class CollisionInfo
{
    //Optimal vector resolving the collision
    private Vector3 minimumTranslationVector;
    
    //Position of the contact of the collision
    private Vector3 contactPoint;

    //Reference of where the MTV is pointing (towards which object)
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
    public Vector3 GetContactPoint() 
    {
        return contactPoint;
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

   

}
