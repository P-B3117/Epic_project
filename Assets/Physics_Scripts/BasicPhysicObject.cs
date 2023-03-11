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

    public List<FrictionInfo> contact;



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
        contact = new List<FrictionInfo>();
    }


	public void UpdateState(float timeStep) 
    {
        if (isStatic) { velocity = Vector3.zero; angularVelocity = 0; return; }



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
    public void ApplyFriction()
    {

        float force;
        float friction;
        float coef;

        Vector3 fric = Vector3.zero;
        Debug.Log(velocity);
        //if(velocity.magnitude <= 0.15)
        //{
        //    angularVelocity = 0;
        //}
        for (int i = 0; i < contact.Count; i++)
        {
            Vector3 normal = contact[i].getNormal();
            force = Vector3.Dot(-normal, resultingForce);
            BasicPhysicObject autre = contact[i].getBasicPhysicObject();


            if (velocity.magnitude < 0.1f)
            {
                coef = autre.getStaticFriction();
                friction = force * coef;
            }
            else
            {
                coef = autre.getDynamicFriction();
                friction = force * coef;
            }
            Vector3 rbp = transform.position - contact[i].getCollisionPoint();
            Vector3 rbpPerp = new Vector3(-rbp.y, rbp.x, 0.0f);
            Vector3 v = velocity - angularVelocity * rbpPerp;
            Vector3 inverseNormal = new Vector3(-normal.y, normal.x, 0.0f);

            float direction = Vector3.Dot(v, inverseNormal);
            inverseNormal = inverseNormal * direction;

            fric = inverseNormal.normalized * -1 * friction;
            Vector3 r = contact[i].getCollisionPoint() - transform.position;
            ApplyForce(fric, r);
        }
        contact.Clear();
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
    public float getDynamicFriction()
    {
        return this.dynamicFriction;
    }

}
