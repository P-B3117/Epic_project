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
    public float dampingRatio;
    public float length;
    public bool onlyPull;
    //private Vector3 localAnchorA;
    //private Vector3 localAnchorB;
    private Transform bodyA;
    private Transform bodyB;
    private Vector3 anchorA;
    private Vector3 anchorB;

    private Vector3 d;
   
    private float m;
    private float beta;
    private float gamma;
    private float bias;
    private float impulseSum;

    private Vector3 initialAxis;
    private BasicPhysicObject bpA;
    private BasicPhysicObject bpB;
    private MeshColliderScript mcA;
    private MeshColliderScript mcB;
    private float jointMass;
    public void Start()
    {
        impulseSum = 0;
    }
    public void DelayedStart()
    {
        // Get references to the BasicPhysicObject and MeshColliderScript components for both bodies
         bpA = bo1.GetComponent<BasicPhysicObject>();
         bpB = bo2.GetComponent<BasicPhysicObject>();
         mcA = bo1.GetComponent<MeshColliderScript>();
         mcB = bo2.GetComponent<MeshColliderScript>();
        //just keeping track of the joint position
        transform.position = (anchorA + anchorB) / 2.0f;
    }
    public void UpdateJointState(float timeStep)
    {
        // Get references to the transform and anchor positions for both bodies
        Transform bodyA = bo1.transform;
        Transform bodyB = bo2.transform;
        anchorA = bodyA.position;
        anchorB = bodyB.position;

        // Compute the vector between the two anchor points
        Vector3 d = anchorB - anchorA;

        // Compute the current length of the constraint
        float currentLength = d.magnitude;

        // Compute the effective mass of the constraint 
        // M = (J · M^-1 · J^t)^-1
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
        jointMass = m;

        // Compute beta and gamma values
        ComputeBetaAndGamma(timeStep);
        // Compute the bias term for the constraint
        float bias = (currentLength - length) * beta / timeStep;

        // Compute the relative velocities and Jacobian for the constraint
        // J = [-t^t, -(ra + d)×t, t^t, rb×t]
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
        float impulseMag = m * -(jv + bias + gamma);

        // Compute the corrective impulse and apply it to the bodies
        float impulseA = impulseMag * invMassA;
        float impulseB = impulseMag * invMassB;
        Vector3 impulseDir = d.normalized;
        if (onlyPull)
        {
            if (currentLength <= length)
            {
                impulseDir = Vector3.zero;
            }
        }
      
        Vector3 impulse = impulseMag * impulseDir;
        //prevent any impulse added if static... 
        if (bpA.getIsStatic() == false)
        {
            v1 -= impulseA * invMassA * impulseDir;
            w1 -= Vector3.Dot(ra, impulse) * invInertiaA;
            bpA.SetVelocity(v1, w1);
        }
        if (bpB.getIsStatic() == false)
        {
            v2 += impulseB * invMassB * impulseDir;
            w2 += Vector3.Dot(rb, impulse) * invInertiaB;
            bpB.SetVelocity(v2, w2);
        }

        //just for keeping track of the position 
        transform.position = (anchorB + anchorA) / 2.0f;
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
            float k = jointMass * omega * omega;               // Spring
            float h = timeStep;
            float d = 2.0f * jointMass * dampingRatio * omega; // Damping coefficient

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