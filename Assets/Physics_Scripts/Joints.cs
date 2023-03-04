using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fix les math
//get relative location of joint 
//get meshs collider dans le start 
//no warm 

public class Joints : MonoBehaviour
{

    [SerializeField]
    GameObject bo1;
    [SerializeField]
    GameObject bo2;


    [SerializeField]
    public float frequency;
    public float jointMass;
    public float dampingRatio;
    [SerializeField]
    private Vector3 localAnchorA;
    private Vector3 localAnchorB;



    public Transform bodyA;
    public Transform bodyB;
    public Vector3 anchorA;
    public Vector3 anchorB;
    private Vector3 ra;
    private Vector3 rb;
    private Vector3 d;
  
    private float m;
    private float beta;
    private float gamma;
    private float bias;
    private float impulseSum;
    

    private BasicPhysicObject bpA;
    private BasicPhysicObject bpB;
    private MeshColliderScript mcA;
    private MeshColliderScript mcB;
    private float length;
    public void Start()
    {
        impulseSum = 0;

        // Get references to the transform and anchor positions for both bodies
        Transform bodyA = bo1.transform;
        Transform bodyB = bo2.transform;
        Vector3 anchorA = bodyA.position;
        Vector3 anchorB = bodyB.position;

        d = anchorB - anchorA;
        //length = Vector3.Distance(bodyA.InverseTransformPoint(anchorA), bodyB.InverseTransformPoint(anchorB));
        length = 2;

        Vector3 jointPosition = (anchorA + anchorB) / 2.0f;

        transform.position = jointPosition;
        
    }
    public void DelayedStart()
    {
        impulseSum = 0;

        // Get references to the transform and anchor positions for both bodies
        Transform bodyA = bo1.transform;
        Transform bodyB = bo2.transform;
        Vector3 anchorA = bodyA.position;
        Vector3 anchorB = bodyB.position;

        d = anchorB - anchorA;
        //length = Vector3.Distance(bodyA.InverseTransformPoint(anchorA), bodyB.InverseTransformPoint(anchorB));
        length = 20;


        Vector3 jointPosition = (anchorA + anchorB) / 2.0f;

        transform.position = jointPosition;
    }
    public void UpdateJointState(float timeStep)
    {
        // Compute beta and gamma values
        ComputeBetaAndGamma(timeStep);

        // Get references to the BasicPhysicObject and MeshColliderScript components for both bodies
        BasicPhysicObject bpA = bo1.GetComponent<BasicPhysicObject>();
        BasicPhysicObject bpB = bo2.GetComponent<BasicPhysicObject>();
        MeshColliderScript mcA = bo1.GetComponent<MeshColliderScript>();
        MeshColliderScript mcB = bo2.GetComponent<MeshColliderScript>();

        // Get references to the transform and anchor positions for both bodies
        Transform bodyA = bo1.transform;
        Transform bodyB = bo2.transform;
         anchorA = bodyA.position;
         anchorB = bodyB.position;
        Debug.Log("anchorA=" + anchorA);
        Debug.Log("anchorB=" + anchorB);
        // Compute the world space coordinates of the anchor points
        // Vector3 ra = bodyA.rotation * anchorA;
        //Vector3 rb = bodyB.rotation * anchorB;
        //Vector3 pa = anchorA + ra;
        //Vector3 pb = anchorB + rb;

        // Compute the vector between the two anchor points
        Vector3 d = anchorB - anchorA;
        
        
        // Compute the current length of the constraint
        float currentLength = d.magnitude;

        // Compute the effective mass of the constraint
        Vector3 ra = anchorA - bodyA.position;
        Vector3 rb = anchorB - bodyB.position;
        float crossA = Vector3.Cross(ra, Vector3.forward).z;
        float crossB = Vector3.Cross(rb, Vector3.forward).z;
        float invMassA = 1.0f / mcA.GetMass();
        float invMassB = 1.0f / mcB.GetMass();
        float invInertiaA = 1.0f / mcA.GetInertia();
        float invInertiaB = 1.0f / mcB.GetInertia();
        float invMassSum = invMassA + invMassB;
        float invInertiaSum = invInertiaA + invInertiaB;
        float invEffectiveMass = invMassSum + crossA * crossA * invInertiaA / d.sqrMagnitude + crossB * crossB * invInertiaB / d.sqrMagnitude;
        float m = invEffectiveMass != 0 ? 1 / invEffectiveMass : 0;

        // Compute the bias term for the constraint
        float bias = (currentLength - length) * beta / timeStep;
        Debug.Log("bias= " + bias);
        Debug.Log("currentLength =" + currentLength);
        Debug.Log("length=" + length);


        // Compute the relative velocities and Jacobian for the constraint
        Vector3 v1 = bpA.getVelocity();
        Vector3 v2 = bpB.getVelocity();
        float w1 = bpA.getAngularVelocity();
        float w2 = bpB.getAngularVelocity();
        Vector3 raCross = Vector3.Cross(new Vector3(0.0f, 0.0f, w1), ra);
        Vector3 rbCross = Vector3.Cross(new Vector3(0.0f, 0.0f, w2), rb);
        Vector3 dv = v2 + rbCross - v1 - raCross;
        Vector3 J = new Vector3(-d.x, -d.y, -crossA) / d.sqrMagnitude;
        float jv = Vector3.Dot(dv, J);

        // Compute the impulse magnitude for the constraint
        float impulseMag = m * (jv + bias + gamma * impulseSum);
       
        // Compute the corrective impulse and apply it to the bodies
        float impulseA = impulseMag * invMassA;
        float impulseB = impulseMag * invMassB;
        Vector3 impulseDir = d.normalized;


        float error = currentLength - length;
        if (error > 0)
        {
            impulseDir *= -1; // Bodies are too far apart, so reverse the impulse direction
        }
        impulseDir += error * gamma * impulseDir; // Adjust the impulse direction based on the error and gamma value
        Vector3 impulse = impulseMag * impulseDir;


        v1 -= impulseA * invMassA * impulseDir;
        w1 -= Vector3.Dot(ra, impulse) * invInertiaA;
        v2 += impulseB * invMassB * impulseDir;
        w2 += Vector3.Dot(rb, impulse) * invInertiaB;

        Debug.Log("v1= " + (impulseA * invMassA * d.normalized));
        Debug.Log("v2 =" + (Vector3.Dot(ra, impulse) * invInertiaA));
        Debug.Log("w1=" + (impulseB * invMassB * d.normalized));
        Debug.Log(" w2=" + (Vector3.Dot(rb, impulse) * invInertiaB));
        bpA.SetVelocity(v1, w1);
        bpB.SetVelocity(v2, w2);





        //just for keeping track of the position 
        Vector3 jointPosition = (anchorB + anchorA) / 2.0f;

        transform.position = jointPosition;
        impulseSum += impulse.magnitude;
    }
    private void ComputeBetaAndGamma(float timeStep)
    {

        // If the frequency is less than or equal to zero, make this joint solid
        if (frequency < 0.0f)
        {
            beta = 1.0f;
            gamma = 0.0f;
        }
        else
        {
            // β = hk / (c + hk)
            // γ = 1 / (c + hk)
            //https://box2d.org/files/ErinCatto_SoftConstraints_GDC2011.pdf 

            float omega = 2.0f * Mathf.PI * frequency;
            float d = 2.0f * jointMass * dampingRatio * omega; // Damping coefficient



            float k = jointMass * omega * omega;               // Spring
            float h = timeStep;

            beta = h * k / (d + h * k);
            gamma = 1.0f / ((d + h * k) * h);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(anchorA, anchorB);
    }
}