using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrictionInfo
{
    Vector3 normal;
    BasicPhysicObject autre;
    Vector3 collisionPoint;

    public FrictionInfo(Vector3 normal, BasicPhysicObject autre, Vector3 collisionPoint)
    {
        this.normal = normal;
        this.autre = autre;
        this.collisionPoint = collisionPoint;   
    }

    public Vector3 getNormal() { return normal; }
    public BasicPhysicObject getBasicPhysicObject() { return autre; }
    public Vector3 getCollisionPoint() { return collisionPoint; }
}
