using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabJoint : MonoBehaviour
{
    [SerializeField]
    public GameObject bo1;

    [SerializeField]
    public float frequency;
    public float dampingRatio;
    //private Vector3 localAnchorA;
    //private Vector3 localAnchorB;
    private Transform bodyA;
    private Vector3 anchorA;
    private Vector3 anchorB;
    private Vector3 d;
    private float m;
    private float beta;
    private float gamma;
    private float bias;
    private Vector3 initialAxis;
    private BasicPhysicObject bpA;
    private MeshColliderScript mcA;
    private float jointMass;
    private float invMassA;
    private float invInertiaA;

    private LineRenderer lr;
    private Vector3 cursorPosition;
    private Vector3 prevCursorPos; // new member variable to store cursor's previous position
    public void Initialize()
    {

        // Get references to the BasicPhysicObject and MeshColliderScript components for both bodies
        bpA = bo1.GetComponent<BasicPhysicObject>();
        mcA = bo1.GetComponent<MeshColliderScript>();
        invMassA = 1.0f / mcA.GetMass();
        invInertiaA = 1.0f / mcA.GetInertia();
        jointMass = mcA.GetMass() / 5;
        lr = this.gameObject.AddComponent<LineRenderer>();
        lr.SetWidth(0.2f, 0.2f);
        prevCursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public void UpdateJointState(float timeStep)
    {
        ComputeBetaAndGamma(timeStep);
        //get positions 
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Transform bodyA = bo1.transform;
        anchorA = bodyA.position;
        Vector3 anchorB = cursorPosition;


        // Compute the current length 
        float currentLength = d.magnitude;
        // Calculate the matrix K
        float k11 = invMassA + invInertiaA * anchorA.y * anchorA.y;
        float k12 = -invInertiaA * anchorA.y * anchorA.x;
        float k21 = -invInertiaA * anchorA.x * anchorA.y;
        float k22 = invMassA + invInertiaA * anchorA.x * anchorA.x;

        k11 += gamma;
        k22 += gamma;

        Vector2 m1 = new Vector3(k11, -k21);
        Vector2 m2 = new Vector3(-k12, k22);
        //inverse
        float det = 1 / (k11 * k22 - k12 * k21);
        m1 = m1 * det;
        m2 = m2 * det;



        Vector2 bias = (anchorB - anchorA) * beta / timeStep;
      
        Vector3 v1 = bpA.getVelocity();
        float w1 = bpA.getAngularVelocity();

        Vector2 jv = v1 + Quaternion.AngleAxis(w1, Vector3.forward) * d;
        Vector2 lambda = new Vector2(-(jv.x + gamma + bias.x), -(jv.y + gamma + bias.y));

        Vector2 impulse0 = new Vector2(lambda.x * m1.x + lambda.y * m2.x, lambda.x * m1.y + lambda.y * m2.y);

        Vector3 impulse = new Vector3(-impulse0.x, -impulse0.y, 0);

        //prevent any impulse added if static... 
        if (!bpA.getIsStatic())
        {
            v1 += impulse * invMassA;

            bpA.SetVelocity(v1, w1, timeStep);
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

    public float getbeta()
    {
        return this.beta;
    }

    public float getgamma()
    {
        return this.gamma;
    }

}
