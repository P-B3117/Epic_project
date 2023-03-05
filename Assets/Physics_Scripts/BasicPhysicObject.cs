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
    [Header("Fondamental variables")]
    [SerializeField]
    bool isStatic = false;





    [Header("Property of the material of the object")]
    [SerializeField]
    float dynamicFriction = 0.5f;
    [SerializeField]
    float staticFriction = 0.5f;
    [SerializeField]
    float bounciness = 0.5f;



    Vector3 resultingForce;
    float torque;


    Vector3 velocity;
    float angularVelocity;


    MeshColliderScript collider;


    public void Start()
    {
        resultingForce = Vector3.zero;
        torque = 0;

        velocity = Vector2.zero;
        angularVelocity = 0;
    }


	public void UpdateState(float timeStep) 
    {
        if (isStatic) { return; }
        


        Vector3 acceleration = resultingForce / collider.GetMass();
        
        float angularAcceleration = torque / collider.GetInertia();
        
        

        velocity += acceleration * timeStep;
        angularVelocity += angularAcceleration * timeStep;


        transform.position += velocity * timeStep;
        transform.Rotate(Vector3.forward * angularVelocity * Mathf.Rad2Deg * timeStep);

       
        //Reset les forces pour le next updateCall
        resultingForce = Vector3.zero;
        torque = 0;
    }


    public void ApplyForceAtCenterOfMass(Vector3 force) 
    {
        resultingForce += force;
    }

    public void ApplyForceGravity() 
    {
        //F = mg
        resultingForce += Vector3.down * UniversalVariable.GetGravity() * collider.GetMass();
    }


    //Apply force at another point other than the center of mass where :
    //force is the force applied
    //r is the vector from the center of mass towards the position of where the force is applied
    public void ApplyForce(Vector3 force, Vector3 r) 
    {
        float theta = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 rotatedForce = new Vector3(force.x * Mathf.Cos(theta) - force.y * Mathf.Sin(theta), force.x * Mathf.Sin(theta) + force.y * Mathf.Cos(theta), 0);
        resultingForce += rotatedForce;


        torque += (r.x * force.y) - (r.y * force.x);
        

    }

    public Vector3 getVelocity()
    {
        return this.velocity;
    }

    public float getBounciness()
	{
        return this.bounciness;
	}

    public void SetCollider(MeshColliderScript script) 
    {
        collider = script;
    }
    public float getAngularVelocity()
    {
        return this.angularVelocity;
    }
    public bool getIsStatic()
    {
        return this.isStatic;
    }

    public void SetVelocity(Vector3 velocity, float newAngularVelocity)
    {
        this.velocity = velocity;
        this.angularVelocity = newAngularVelocity;
    }

}
