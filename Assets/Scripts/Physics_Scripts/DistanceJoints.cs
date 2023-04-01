using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//get relative location of joint 
//get the position offset as serial and then clamp the values to the range of the object in question 
//clamp values in general so that it can't break 
public class DistanceJoints : MonoBehaviour
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
    public Vector3 offsetA;
    public Vector3 offsetB;
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
    private LineRenderer lr;
    public void Start()
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


        lr = this.gameObject.AddComponent<LineRenderer>();
        lr.SetWidth(0.2f, 0.2f);

    }
    public void UpdateJointState(float timeStep)
    {
        bpA = bo1.GetComponent<BasicPhysicObject>();
        bpB = bo2.GetComponent<BasicPhysicObject>();
        mcA = bo1.GetComponent<MeshColliderScript>();
        mcB = bo2.GetComponent<MeshColliderScript>();
        dampingRatio = Mathf.Clamp(dampingRatio, 0.0f, 1.0f);
        //get positions 
        Transform bodyA = bo1.transform;
        Transform bodyB = bo2.transform;
        anchorA = bodyA.position;
        anchorB = bodyB.position;
        Vector3 ra = (bodyA.rotation) * offsetA;
        Vector3 rb = (bodyB.rotation) * offsetB;
        // Compute the vector between the two anchor points
        Vector3 pa = ra + anchorA;
        Vector3 pb = rb + anchorB;

        Vector3 d = pa - pb;

        // Compute the current length 
        float currentLength = d.magnitude;

        // Compute the effective mass of the constraint 
        // M = (J · M^-1 · J^t)^-1
        float crossA = d.x * ra.y - d.x * ra.x;
        float crossB = d.x * rb.y - d.x * rb.x;
        invMassA = 1.0f / mcA.GetMass();
        invMassB = 1.0f / mcB.GetMass();
        invInertiaA = 1.0f / mcA.GetInertia();
        invInertiaB = 1.0f / mcB.GetInertia();
        invInertiaSum = invInertiaA + invInertiaB;
        invMassSum = invMassA + invMassB;
        float invEffectiveMass;
        if (bpA.getIsStatic()) { invEffectiveMass = invMassB + crossB * crossB * invInertiaB / d.sqrMagnitude; }
        else if (bpB.getIsStatic()) { invEffectiveMass = invMassA + crossA * crossA * invInertiaA / d.sqrMagnitude; }
        else { invEffectiveMass = invMassSum + crossA * crossA * invInertiaA / d.sqrMagnitude + crossB * crossB * invInertiaB / d.sqrMagnitude; }

        float m = invEffectiveMass != 0 ? 1 / invEffectiveMass : 0;
        if (frequency >= 0.0f)
        {
            jointMass = m;
        }
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
        float impulseMag = m * (jv + bias + gamma);

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
        if (!bpA.getIsStatic())
        {
            v1 -= impulseA * impulseDir;
            w1 -= Vector3.Dot(impulse, ra) * invInertiaA;
            bpA.SetVelocity(v1, w1, timeStep);
        }
        if (!bpB.getIsStatic())
        {
            v2 += impulseB * impulseDir;
            w2 += Vector3.Dot(impulse, rb) * invInertiaB;

            bpB.SetVelocity(v2, w2, timeStep);
        }

        //just for keeping track of the position 
        transform.position = (anchorB + anchorA) / 2.0f;
        Vector3[] LinePoints = new Vector3[2];
        LinePoints[0] = anchorA;
        LinePoints[1] = anchorB;
        lr.SetPositions(LinePoints);
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

}