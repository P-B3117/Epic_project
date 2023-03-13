using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineJoint : MonoBehaviour
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
    private float angleOffset;
    public void Start()
    {
        // Get references to the BasicPhysicObject and MeshColliderScript components for both bodies
        bpA = bo1.GetComponent<BasicPhysicObject>();
        bpB = bo2.GetComponent<BasicPhysicObject>();
        mcA = bo1.GetComponent<MeshColliderScript>();
        mcB = bo2.GetComponent<MeshColliderScript>();
        bodyA = bo1.transform;
        bodyB = bo2.transform;
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
        angleOffset = Mathf.Atan2(bodyB.position.y - bodyA.position.y, bodyB.position.x - bodyA.position.x);
    }


    public void UpdateJointState(float timeStep)
    {
        bpA = bo1.GetComponent<BasicPhysicObject>();
        bpB = bo2.GetComponent<BasicPhysicObject>();
        mcA = bo1.GetComponent<MeshColliderScript>();
        mcB = bo2.GetComponent<MeshColliderScript>();
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
        // Define the matrix k
        float[][] k = new float[2][];
        for (int i = 0; i < 2; i++)
        {
            k[i] = new float[2];
        }

        float invMassA = bodyA.invMass;
        float invMassB = bodyB.invMass;
        float invInertiaA = bodyA.invInertia;
        float invInertiaB = bodyB.invInertia;
        float sa = Mathf.Sin(angle);
        float sb = Mathf.Sin(angle + Mathf.PI);
        float ca = Mathf.Cos(angle);
        float cb = Mathf.Cos(angle + Mathf.PI);

        k[0][0] = invMassA + invMassB + sa * sa * invInertiaA + sb * sb * invInertiaB;
        k[1][0] = sa * invInertiaA + sb * invInertiaB;
        k[0][1] = k[1][0];
        k[1][1] = invInertiaA + invInertiaB;

        float gamma = 0.0f; // set the value of gamma
        k[0][0] += gamma;
        k[1][1] += gamma;

        // Calculate the inverse of k
        float det = k[0][0] * k[1][1] - k[0][1] * k[1][0];
        float invDet = 1.0f / det;
        float[][] kInv = new float[2][];
        for (int i = 0; i < 2; i++)
        {
            kInv[i] = new float[2];
        }

        kInv[0][0] = k[1][1] * invDet;
        kInv[1][1] = k[0][0] * invDet;
        kInv[0][1] = -k[0][1] * invDet;
        kInv[1][0] = -k[1][0] * invDet;

        // Calculate the bias
        Vector2 d = pointB - pointA;
        Vector2 t = new Vector2(d.y, -d.x);
        t.Normalize();
        Vector2 bias = new Vector2(
            Vector2.Dot(d, t),
            bodyB.angle - bodyA.angle - angleOffset
        );
        bias *= beta * invDt;

        // Calculate the impulse
        Vector2 impulse = new Vector2(
            -kInv[0][0] * bias.x - kInv[0][1] * bias.y,
            -kInv[1][0] * bias.x - kInv[1][1] * bias.y
        );

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
