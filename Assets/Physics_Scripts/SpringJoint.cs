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
    public float length;
   
    public float maxAngle;
    public float initialAngle;
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
        
    }
    
    
    public void UpdateJointState(float timeStep)
    {
        //get positions 
        Transform bodyA = bo1.transform;
        Transform bodyB = bo2.transform;
        Vector3 anchorA = bodyA.position;
        Vector3 anchorB = bodyB.position;
        // Compute the vector between the two anchor points
        Vector2 d = anchorB - anchorA;
        d = d.normalized;
        // Choose the axis perpendicular to the constraint direction and the given axis
        Vector3 perpAxis = new Vector3(-d.y, d.x, 0);
        // Compute the current length of the constraint
        float currentLength = d.magnitude;
        // Compute the tangent constraint axis
        Vector3 constraintAxis = Vector3.Cross(perpAxis, d);

    }
    


    private void ComputeBetaAndGamma(float timeStep)
    {
        // β = hk / (c + hk)
        // γ = 1 / (c + hk)
        //https://box2d.org/files/ErinCatto_SoftConstraints_GDC2011.pdf 

        float omega = 2.0f * Mathf.PI * frequency;
        float k = jointMass * omega * omega;               // Spring
        float h = timeStep;
        float d = 4.0f * jointMass * dampingRatio * omega; // Damping coefficient

        beta = h * k / (d + h * k);
        gamma = 1.0f / ((d + h * k) * h);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(anchorA, anchorB);
    }



}
