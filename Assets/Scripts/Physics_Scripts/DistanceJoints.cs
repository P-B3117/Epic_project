using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Filename : DistanceJoints
 * Author : Antony
 * Goal : Creates a joint between 2 physicsObject
 * 
 * Requirements : Create an instance of this script and feed him the necessary data
 */



//get relative location of joint 
//get the position offset as serial and then clamp the values to the range of the object in question 
//clamp values in general so that it can't break 
public class DistanceJoints : MonoBehaviour
{
    [SerializeField]
   public GameObject bo1;
    [SerializeField]
   public GameObject bo2;
    [SerializeField]
    public float frequency;
    public float dampingRatio;
    public float length;
  
    public Vector3 offsetA;
    public Vector3 offsetB;
    //private Vector3 localAnchorA;
    //private Vector3 localAnchorB;
    private Transform bodyA;
    private Transform bodyB;
    private Vector3 anchorA;
    private Vector3 anchorB;

    public float fakemass;
   public float fakeSoftness;
   public  float fakeSize;
    private Vector3 d;

    private float m;
    private float beta;
    private float gamma;
    private float bias;


    private Vector3 initialAxis;
    private BasicPhysicObject bpA;
    private BasicPhysicObject bpB;
    private MeshColliderScript mcA;
    private MeshColliderScript mcB;
    private float jointMass;
    private float invMassA;
    private float invMassB;
    private float invInertiaA;
    private float invInertiaB;
    private float invInertiaSum;
    private float invMassSum;
    public LineRenderer lr;


    //Update the appearance of the distanceJoint
    public void Update()
    {
        if (lr != null)
        {
            Transform bodyA = bo1.transform;
            Transform bodyB = bo2.transform;
            anchorA = bodyA.position;
            anchorB = bodyB.position;
            //just for keeping track of the position 
            transform.position = (anchorB + anchorA) / 2.0f;
            Vector3[] LinePoints = new Vector3[2];
            LinePoints[0] = anchorA + (bodyA.rotation) * offsetA;
            LinePoints[1] = anchorB + (bodyB.rotation) * offsetB;
            LinePoints[0].z = -8;
            LinePoints[1].z = -8;
            lr.SetPositions(LinePoints);
        }
        
    }

    //Initialize the necessary variables
    public void Initialize() 
    {
        // Get references to the BasicPhysicObject and MeshColliderScript components for both bodies
        bpA = bo1.GetComponent<BasicPhysicObject>();
        bpB = bo2.GetComponent<BasicPhysicObject>();
        mcA = bo1.GetComponent<MeshColliderScript>();
        mcB = bo2.GetComponent<MeshColliderScript>();
        invMassA = 1.0f / mcA.GetMass();
        invMassB = 1.0f / mcB.GetMass();
        invInertiaA = 1.0f / mcA.GetInertia();
        invInertiaB = 1.0f / mcB.GetInertia();
        invInertiaSum = invInertiaA + invInertiaB;
        invMassSum = invMassA + invMassB;
       jointMass = 1 ;
        fakemass = 1;
        fakeSoftness = 0.0f;
        fakeSize = 1;
        lr = this.gameObject.AddComponent<LineRenderer>();
        lr.SetWidth(0.3f, 0.3f);
    }

    //Update the joint and change its properties
    public void UpdateJointState(float timeStep)
    {
        // Clamp damping ratio between 0 and 1
        dampingRatio = Mathf.Clamp(dampingRatio, 0.0f, 1.0f);

        // Get the transforms of both bodies and anchor points
        Transform bodyA = bo1.transform;
        Transform bodyB = bo2.transform;
        anchorA = bodyA.position;
        anchorB = bodyB.position;
        Vector3 ra = (bodyA.rotation) * (offsetA);
        Vector3 rb = (bodyB.rotation) * (offsetB);

        // Compute the vector between the two anchor points
        Vector3 pa = anchorA + ra;
        Vector3 pb = anchorB + rb;
        Vector3 d = (pb - pa);
       
        // Compute the current length 
        float currentLength = d.magnitude;

        // Compute the normalized direction vector between the two anchor points
        Vector3 n = d.normalized;

        // Compute the effective mass of the constraint 
        // M = (J · M^-1 · J^t)^-1
        // J = [-n, -n×ra, n, n×rb]
        // ( n = (anchorB-anchorA) / ||anchorB-anchorA|| )
        // ra, rb = vec from center of mass to offset 
        float crossA = Vector3.Cross(-n,ra).z;
        float crossB = Vector3.Cross(n, rb).z;
        float invEffectiveMass;
        //first part is transitional aspect of the equation and the two second ones are relative to the rotation 
        invEffectiveMass = invMassSum + crossA * crossA * invInertiaA + crossB * crossB * invInertiaB ;

        // Compute beta and gamma values
        ComputeBetaAndGamma(timeStep);

        // Compute the joint mass
        float m = invEffectiveMass + gamma != 0 ? 1 / (invEffectiveMass + gamma ): 0;

        if (frequency >= 0.0f)
        {
            jointMass = m;
        }
        // Compute beta and gamma values
        
        
        //Résolution de contraintes *****************************************************************************
        // Compute the bias term for the constraint
        float bias = (currentLength - length) * beta / timeStep;

        // Compute the relative velocities
        Vector3 v1 = bpA.getVelocity();
        Vector3 v2 = bpB.getVelocity();
        float w1 = bpA.getAngularVelocity();
        float w2 = bpB.getAngularVelocity();
        Vector3 raCross = new Vector3(-w1*ra.y, w1*ra.x, 0); 
        Vector3 rbCross = new Vector3(-w2*rb.y,w2*rb.x,0);
        //vitesse relative (dériver de vitesse)
        Vector3 dv = v2 + rbCross - v1 - raCross;
        float jv = Vector3.Dot(dv, n);
        // Compute Jacobian for the constraint (C = impulse Dir) here its defined by x^/sqrt(||x||)
        // la racine est pour rendres les parametres plus mous parce que ma simulation est trop rigide sans.
        Vector3 J = d / d.sqrMagnitude ;
        // Compute the corrective impulse 
        float impulseMag = -m *(jv + bias);
        

        Vector3 impulse = impulseMag * J;
        //Apply corrective impulse 
        if (!bpA.getIsStatic())
        {
            v1 -= impulse * invMassA ;
            w1 -= Vector3.Dot(new Vector3(-ra.y*impulseMag,ra.x*impulseMag), J) * invInertiaA;
            bpA.SetVelocity(v1, w1, timeStep);
        }
        if (!bpB.getIsStatic())
        {
            v2 += impulse * invMassB ;
            w2 += Vector3.Dot(new Vector3(-rb.y * impulseMag, rb.x * impulseMag), J) * invInertiaB;

            bpB.SetVelocity(v2, w2, timeStep);
        }

      
    }
    private void ComputeBetaAndGamma(float timeStep)
    {

        // If the frequency is less than or equal to zero, make this joint solid
        if (frequency <= 0.0f)
        {
            beta = 1.0f;
            gamma = 0.0f;
            jointMass = float.PositiveInfinity;
            frequency = 0.0f;
        }
        else
        {
            // β = hk / (c + hk)
            // γ = 1 / (c + hk)
            // k = m * ω^2
            // c = 2 * m * ζ * ω
            //https://box2d.org/files/ErinCatto_SoftConstraints_GDC2011.pdf 

            float omega = 2.0f * Mathf.PI * frequency;
            float k = 2* jointMass * omega * omega;               // Spring
            float h = timeStep;
            float c = 2.0f * jointMass * dampingRatio * omega; // Damping coefficient

            beta = h * k / (c + h * k);
            gamma = 1.0f / ((c + h * k));
        }
    }
   

    public float getbeta()
    {
        return this.beta;
    }

    public float getgamma()
    {
        return this.gamma;
    }
    public void setfrequency(float freq)
    {
        this.frequency = freq;
    }
    public void setdampingRatio(float damp)
    {
        this.dampingRatio = damp;   
    }
    public void setlength(float leng)
    {
        this.length = leng;
    }
    public void setFakeMass(float fakemass)
    {
        this.fakemass= fakemass;
    }
    public void setFakeSoftness(float soft)
    {
        this.fakeSoftness = soft;
    }
    public void setFakeSize(float fakesize)
    {
        this.fakeSize = fakesize;
    }
    public float getFakeMass()
    {
        return this.fakemass;   
    }
    public float getFakeSoftness()
    {
        return this.fakeSoftness;

    }
    public float getFakeSize()
    {
        return this.fakeSize;
    }
    public void setDampingRatio(float dampingRatio)
    {
        this.dampingRatio = dampingRatio;
    }
}