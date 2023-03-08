using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringJoint : MonoBehaviour
{
    [SerializeField]
    GameObject bo1;
    [SerializeField]
    GameObject bo2;
    [SerializeField]
    public float frequency;
    public float dampingRatio;
    public Vector2 direction;
    //private Vector3 localAnchorA;
    //private Vector3 localAnchorB;
    private Transform bodyA;
    private Transform bodyB;
    private Vector3 anchorA;
    private Vector3 anchorB;

    private Vector3 d;
    private Vector2 perp;
    private float m;
    private float beta;
    private float gamma;
    private float bias;
    private float maxAngleRadians;
    private float initialAngleRadians;


    private Vector3 initialAxis;
    private BasicPhysicObject bpA;
    private BasicPhysicObject bpB;
    private MeshColliderScript mcA;
    private MeshColliderScript mcB;
    private float jointMass;

    public void Start()
    {
        // Get references to the BasicPhysicObject and MeshColliderScript components for both bodies
        bpA = bo1.GetComponent<BasicPhysicObject>();
        bpB = bo2.GetComponent<BasicPhysicObject>();
        mcA = bo1.GetComponent<MeshColliderScript>();
        mcB = bo2.GetComponent<MeshColliderScript>();

        if (direction.magnitude < 0.00001f)
        {
            Vector2 bodyA = bo1.transform.position;
            Vector2 bodyB = bo2.transform.position;
            Quaternion rotatA = bo1.transform.rotation;
            Vector2 dNormalized = Quaternion.Inverse(rotatA) * ((bodyB - bodyA).normalized);
            perp = Vector2.Perpendicular(dNormalized).normalized;
        }
        else
        {
            perp = Vector2.Perpendicular(direction.normalized);
        }

    }


    public void UpdateJointState(float timeStep)
    {

        //get positions 
        Transform bodyA = bo1.transform;
        Transform bodyB = bo2.transform;
        Vector3 anchorA = bodyA.position;
        Vector3 anchorB = bodyB.position;

        //need to be zero for now cuz no offset 
        Vector3 ra = anchorA - bodyA.position;
        Vector3 rb = anchorB - bodyB.position;
        // Compute the vector between the two anchor points
        Vector3 d = anchorB - anchorA;
        if (direction.magnitude < 0.00001f)
        {
            
            Quaternion rotatA = bo1.transform.rotation;
            Vector2 dNormalized = Quaternion.Inverse(rotatA) * ((bodyB.position - bodyA.position).normalized);
            perp = Vector2.Perpendicular(dNormalized).normalized;
        }
        else
        {
            perp = Vector2.Perpendicular(direction.normalized);
        }

        Vector3 t = bodyA.rotation * perp;
        //0 for now no offset 

        float crossA = 0;
        float crossB = 0;

        // Compute the effective mass matrix
        float invMassA = 1.0f / mcA.GetMass();
        float invMassB = 1.0f / mcB.GetMass();
        float invInertiaA = 1.0f / mcA.GetInertia();
        float invInertiaB = 1.0f / mcB.GetInertia();
        float invMassSum = invMassA + invMassB;
        float invInertiaSum = invInertiaA + invInertiaB;
         float invEffectiveMass;
        if (bpA.getIsStatic()) {invEffectiveMass = invMassB+ crossB * crossB * invInertiaB / d.sqrMagnitude;} 
        else if (bpB.getIsStatic()) {invEffectiveMass = invMassA + crossA * crossA * invInertiaA / d.sqrMagnitude;}
        else{invEffectiveMass = invMassSum + crossA * crossA * invInertiaA / d.sqrMagnitude + crossB * crossB * invInertiaB / d.sqrMagnitude;}
        float m = invEffectiveMass != 0 ? 1 / invEffectiveMass : 0;
        jointMass = m;

        ComputeBetaAndGamma(timeStep);
        Vector3 v1 = bpA.getVelocity();
        Vector3 v2 = bpB.getVelocity();
        float w1 = bpA.getAngularVelocity();
        float w2 = bpB.getAngularVelocity();
        float error = Vector3.Dot(d, t);
        float bias = error * beta * timeStep;
        // Compute corrective impulse: Pc
        // Pc = J^t · λ (λ: lagrangian multiplier)
        // λ = (J · M^-1 · J^t)^-1 ⋅ -(J·v+b)
        float jv = Vector3.Dot(t, v2 - v1) + crossB * w2 - crossA * w1;
        float lambda = m* -(jv + bias + gamma);

       
        Vector3 p = t * lambda;

        v1 -= p * invMassA;
        w1-= lambda * crossA * invInertiaA;
        v2 += p * invMassB;
        w2 += lambda * crossB * invInertiaB;
        bpA.SetVelocity(v1, w1);
        bpB.SetVelocity(v2, w2);
    }

    private void ComputeBetaAndGamma(float timeStep)
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(anchorA, anchorB);
    }



}
