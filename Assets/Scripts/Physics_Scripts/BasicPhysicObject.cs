using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
* Filename : BasicPhysicObjects
 * Author : Clovis, Louis-E, Justin
 * Goal : Encapsulates and calculates physics properties of the object
 * 
 * Requirements : Attach this script to physical objects of the Scene
 */
public class BasicPhysicObject : MonoBehaviour
{
    [Header("Fondamental variables")]
    [SerializeField]
    bool isStatic = false;
    bool isWall = false;

    [SerializeField]
    bool lockRoll = false;


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



 

    public bool IsWall { get => isWall; set => isWall = value; }

 


    public void Start()
    {

        resultingForce = Vector3.zero;
        torque = 0;

        velocity = Vector2.zero;
        angularVelocity = 0;
    }


    public void Initialize() 
    {
        resultingForce = Vector3.zero;
        torque = 0;

        velocity = Vector2.zero;
        angularVelocity = 0;
    }

    //Update call every tick of the simulation, moves the objet depending of the resulting force and torque
	public void UpdateState(float timeStep) 
    {
        if (isStatic) { velocity = Vector3.zero; angularVelocity = 0; resultingForce = Vector3.zero; torque = 0; return; }
        else if (lockRoll) { torque = 0; angularVelocity = 0; }
        

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

    //Apply force at center of mass
    public void ApplyForceAtCenterOfMass(Vector3 force) 
    {
        
        resultingForce += force;
    }

    //Apply force of gravity
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
        resultingForce += force;


        torque += (r.x * force.y) - (r.y * force.x);
        

    }

    //Apply force of air density
    public void ApplyAirDensity()
    {
        Rect bond = collider.GetBoundariesAABB();
        Vector3 drag;

        Vector3 normalisedVel = velocity.normalized;

        Vector3[] p = new Vector3[4];
        p[0] = new Vector3(bond.position.x, bond.position.y, 0.0f);
        p[1] = new Vector3(bond.position.x + bond.width, bond.position.y, 0.0f);
        p[2] = new Vector3(bond.position.x, bond.position.y + bond.height, 0.0f);
        p[3] = new Vector3(bond.position.x + bond.width, bond.position.y + bond.height, 0.0f);
        float min1 = Mathf.Infinity;
        float max1 = -Mathf.Infinity;
        for (int i = 0; i < p.Length; i++)
        {
            float temp = Vector3.Dot(normalisedVel, p[i]);
            min1 = Mathf.Min(temp, min1);
            max1 = Mathf.Max(temp, max1);
        }

        float A = Mathf.Abs(min1 - max1);



        float AirForce;
        float P = UniversalVariable.GetAirDensity();
        float C = 0.4f;
        float speed = velocity.magnitude;



        AirForce = 0.5f * C * P * A * (speed * speed);
        drag = (AirForce * -normalisedVel);
        ApplyForceAtCenterOfMass(drag);


    }



    public Vector3 getVelocity()
    {
        return this.velocity;
    }

    public float getBounciness()
	{
        return this.bounciness;
	}
    public void setBouciness(float bounceLevel) 
    {
        bounciness = bounceLevel;
    }

    public void SetCollider(MeshColliderScript script) 
    {
        collider = script;
    }
    public MeshColliderScript GetCollider() 
    {
        return collider;
    }
    public float getAngularVelocity()
    {
        return this.angularVelocity;
    }


    public void SetVelocity(Vector3 velocity, float newAngularVelocity, float timeStep)
    {
        //this.velocity = velocity;
        resultingForce += ((velocity - this.velocity) / timeStep) * collider.GetMass();
        //this.angularVelocity = newAngularVelocity;
        torque += ((newAngularVelocity - this.angularVelocity) / timeStep * collider.GetInertia());
    }

    public bool IsStatic() 
    {
        return isStatic;
    }

    public float getStaticFriction()
    {
        return this.staticFriction;
    }
    public void setStaticFriction(float frictionLevel)
    {
        staticFriction = frictionLevel;
    }
    public float getDynamicFriction()
    {
        return this.dynamicFriction;
    }
    public void setDynamicFriction(float frictionLevel)
    {
        dynamicFriction = frictionLevel;
    }
    public bool getIsStatic()
    {
        return this.isStatic;
    }
    public void setIsStatic(bool newStatic) 
    {
        isStatic = newStatic;
    }

}
