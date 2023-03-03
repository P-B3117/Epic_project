using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joints : MonoBehaviour
{
    
    [SerializeField]
    GameObject bo1;
    [SerializeField]
    GameObject bo2;
    [SerializeField]
    public float length;
    [SerializeField]
    public float frequency;
    [SerializeField]
    public float dampingRatio;
    [SerializeField]
    public float jointMass;

    public Transform bodyA;
    public Transform bodyB;
    public Vector3 anchorA;
    public Vector3 anchorB;
    private Vector3 localAnchorA;
    private Vector3 localAnchorB;
    private Vector3 ra;
    private Vector3 rb;
    private Vector3 d;
    
    private float m;
    private float beta;
    private float gamma;
    private float bias;
    private float impulseSum;


    public void Start()
    {
       bodyA = bo1.transform;
       bodyB = bo2.transform;
        
        
      
       anchorA = bodyA.position;
       anchorB = bodyB.position;
          

        if (frequency > 0.0f)
        {
            
            dampingRatio = Mathf.Clamp(dampingRatio, 0.0f, 1.0f);
            jointMass = Mathf.Clamp(jointMass, 0.000001f, 100000f);
        }
        else
        {
            frequency = -1.0f;
            dampingRatio = 0.0f;
            jointMass = 0.0f;
        }
        localAnchorA = bodyA.InverseTransformPoint(transform.TransformPoint(anchorA));
        localAnchorB = bodyB.InverseTransformPoint(transform.TransformPoint(anchorB));
    }

    public void UpdateJointState(float timeStep)
    {
       

        ComputeBetaAndGamma(timeStep);

        bodyA = bo1.transform;
        bodyB = bo2.transform;
        anchorA = bodyA.position;
        anchorB = bodyB.position;

        ra = bodyA.rotation * localAnchorA;
        rb = bodyB.rotation * localAnchorB;
        Vector3 pa = anchorA + ra;
        Vector3 pb = anchorB + rb;
        d = pb - pa;
        float currentLength = d.magnitude;
        d.Normalize();

        float crossA = Vector3.Cross(ra, d).z;
        float crossB = Vector3.Cross(rb, d).z;

        float invMassA = 1 / bo1.GetComponent<BasicPhysicObject>().getCollider().GetMass();
        float invMassB = 1 / bo2.GetComponent<BasicPhysicObject>().getCollider().GetMass();

        float invMassSum = invMassA + invMassB;

        float invInertiaA = 1 / bo1.GetComponent<BasicPhysicObject>().getCollider().GetInertia();
        float invInertiaB = 1 / bo2.GetComponent<BasicPhysicObject>().getCollider().GetInertia();

        float invInertiaSum = invInertiaA + invInertiaB;

        float invEffectiveMass = invMassSum + crossA * crossA * 1 / invInertiaA + crossB * crossB * 1 / invInertiaB; 


        m = invEffectiveMass != 0 ? 1 / invEffectiveMass : 0;
        // Compute bias


        bias = currentLength - length;

        bias *= beta / timeStep;

        Vector3 v1 = bo1.GetComponent<BasicPhysicObject>().getVelocity();
        Vector3 v2 = bo2.GetComponent<BasicPhysicObject>().getVelocity();

        float w1 = bo1.GetComponent<BasicPhysicObject>().getAngularVelocity();
        float w2 = bo2.GetComponent<BasicPhysicObject>().getAngularVelocity();

        Vector3 cross1 = Vector3.Cross(new Vector3(0, 0, w1), d);
        Vector3 cross2 = Vector3.Cross(new Vector3(0, 0, w2), d);
        float jv = Vector3.Dot(v2 + (Vector3)cross2 - v1 - (Vector3)cross1, d);
        float lambda = m * (-jv + bias + gamma * impulseSum);

        // Apply corrective impulse
        Vector3 p = lambda * d;
        v1 -= p * invMassA;
        w1 -= crossA * lambda * invInertiaA;
        v2 += p * invMassB;
        w2 += crossB * lambda * invInertiaB;

     
        impulseSum += lambda;
        bo1.GetComponent<BasicPhysicObject>().SetVelocity(v1, w1);
        bo2.GetComponent<BasicPhysicObject>().SetVelocity(v2, w2);
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
        Gizmos.DrawLine(anchorA,anchorB);
    }
}
