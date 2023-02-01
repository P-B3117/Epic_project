using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
* Filename : BasicPhysicObjects
 * 
 * Goal : Encapsulates and calculates physics properties of the object
 * 
 * Requirements : Attach this script to physical objects of the Scene
 */
public class BasicPhysicObject : MonoBehaviour
{
    [SerializeField]
    bool isStatic = false;
    [SerializeField]
    Transform centerOfMass;
    [SerializeField]
    float mass = 5;
    [SerializeField]
    float dynamicFriction = 0.5f;
    [SerializeField]
    float staticFriction = 0.5f;
    [SerializeField]
    float bounciness = 0.5f;

    Vector2 resultingForce;
    Vector2 velocity;
    float angularVelocity;

    Collider objectCollider;


	public void Start()
	{
        velocity = Vector2.zero;
	}

	public void CalculateResultingForceVector() 
    {
        

        resultingForce = Vector3.zero;
        if (isStatic) { return; }

        //F = mg
        resultingForce += Vector2.down * mass * UniversalVariable.GetGravity();
    }

    public void UpdateState(float timeStep) 
    {
        if (isStatic) { return; }

        Vector2 acceleration = resultingForce / mass;
        velocity += acceleration * timeStep;
        Vector2 shift = velocity * timeStep;
        transform.position += new Vector3(shift.x, shift.y, 0);

    }



}
