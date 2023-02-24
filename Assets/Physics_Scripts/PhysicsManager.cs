using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Filename : PhysicsManager
 * 
 * Goal : Central authority of the physic calculations. The PhysicsManager holds references to every physics object in the scene to allow interactions
 * 
 * Requirements : Put a single instance in a Scene attached to an empty GameObject
 */
public class PhysicsManager : MonoBehaviour
{
	//Change the variable numberOfStepsPerSecond to change the timerate calculations
	private int numberOfStepsPerSecond = 50;
	private float stepLength;
	private float numberOfUpdateCounter = 0;

	//List of all physics objects
	[SerializeField]
	List<GameObject> objects;

	List<MeshColliderScript> meshColliders;
	List<BasicPhysicObject> physicObjects;

	public Material test;

	public void Start()
	{

		meshColliders = new List<MeshColliderScript>();
		physicObjects = new List<BasicPhysicObject>();

		for (int i = 0; i < objects.Count; i++) 
		{
			meshColliders.Add(objects[i].GetComponent<MeshColliderScript>());
			physicObjects.Add(objects[i].GetComponent<BasicPhysicObject>());

			physicObjects[i].SetCollider(meshColliders[i]);
		}

		ChangeNumberOfStepsPerSecond(numberOfStepsPerSecond);
		numberOfUpdateCounter = 0;

		
	}

	//Just a test 
	//TO REMOVE!!!!!
	Vector3 MTV = Vector3.zero;
	public void OnDrawGizmos()
	{
<<<<<<< Updated upstream
		Gizmos.color = Color.black;
		
		Gizmos.DrawLine(Vector3.zero, MTV);
=======
		if (COLTEST != null) 
		{
			Gizmos.DrawLine(COLTEST.GetWorldContactPoint(), COLTEST.GetWorldContactPoint() + COLTEST.GetMTV());
			Gizmos.DrawSphere(COLTEST.GetWorldContactPoint(), 0.1f);
		}
>>>>>>> Stashed changes
	}

	//Update the physics objects on a fixed time rate
	public void Update()
	{
		numberOfUpdateCounter += Time.deltaTime / stepLength;
		
		while (numberOfUpdateCounter > 1) 
		{
			
			PhysicCalculations();
			numberOfUpdateCounter--;
		}

	}

	//Simulate all the physics behaviours
	private void PhysicCalculations() 
	{
		//ApplyForces
		for (int i = 0; i < objects.Count; i++) 
		{
			
			physicObjects[i].UpdateState(stepLength);
			//physicObjects[i].ApplyForceGravity();
			//physicObjects[i].ApplyForce(new Vector3(0, 1, 0), new Vector3(0.2f, -0.5f, 0));

			meshColliders[i].UpdateColliderOrientation();
		}

		CollisionManager collisionManager = new CollisionManager();

		//Test collisions
		test.SetColor("_Color", Color.green);
		for (int i = 0; i < objects.Count; i++) 
		{
			for (int j = i + 1; j < objects.Count; j++) 
			{
				CollisionInfo col = HelperFunctionClass.TestCollisionSeperateAxisTheorem(meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());
				if (col != null)
				{
<<<<<<< Updated upstream
					MTV = col.GetMTV();
=======
					
					
					COLTEST = col;
					float inverseMass = (1.0f / (meshColliders[i].GetMass() + meshColliders[j].GetMass()));

					CollisionManager collisionManager= new CollisionManager();

					Vector3 objectVelocity = physicObjects[i].getVelocity();
					Vector3 otherObjectVelocity = physicObjects[j].getVelocity();
					Vector3 normal2 = COLTEST.GetMTV();
					float objectMass = meshColliders[i].GetMass();
                    float otherObjectMass = meshColliders[j].GetMass();
					float coefficientOfRestitution = 0; //(physicObjects[i].getBounciness() + physicObjects[j].getBounciness()) / 2f;
					float objectAngularVelocity = physicObjects[i].getAngularVelocity();
                    float otherObjectAngularVelocity = physicObjects[j].getAngularVelocity();
					Vector3 rVectorObject = Vector3.zero;
					Vector3 rVectorOtherObject = Vector3.zero;
					float objectInertia = meshColliders[i].getInertia();
                    float otherObjectInertia = meshColliders[j].getInertia();

					List<object> newVelocities = null;

					if (COLTEST.GetCollisionRef() == 0)
					{
						meshColliders[j].Translate( COLTEST.GetMTV() * objectMass * inverseMass);
						meshColliders[i].Translate(-COLTEST.GetMTV() * otherObjectMass * inverseMass);

                        //Find collisionPoint after displacement
                        col = HelperFunctionClass.FindCollisionPoint(col, meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());

						rVectorObject = col.GetRelativeContactPoint(meshColliders[i].transform.position);
                        rVectorOtherObject = col.GetRelativeContactPoint(meshColliders[j].transform.position);
                        newVelocities = collisionManager.CollisionHasHappened(objectVelocity, otherObjectVelocity, normal2, objectMass, otherObjectMass, coefficientOfRestitution, objectAngularVelocity, otherObjectAngularVelocity,
                            rVectorObject, rVectorOtherObject, objectInertia, otherObjectInertia);

                        physicObjects[j].SetVelocity((Vector3)newVelocities[1], (float)newVelocities[3]);
						physicObjects[i].SetVelocity((Vector3)newVelocities[0], (float)newVelocities[2]);
                    }
					else 
					{
						meshColliders[j].Translate(-COLTEST.GetMTV() * objectMass * inverseMass);
						meshColliders[i].Translate(COLTEST.GetMTV() * otherObjectMass * inverseMass);

                        //Find collisionPoint after displacement
                        col = HelperFunctionClass.FindCollisionPoint(col, meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());

                        rVectorObject = col.GetRelativeContactPoint(meshColliders[i].transform.position);
                        rVectorOtherObject = col.GetRelativeContactPoint(meshColliders[j].transform.position);

                        newVelocities = collisionManager.CollisionHasHappened(otherObjectVelocity, objectVelocity, normal2, otherObjectMass, objectMass, coefficientOfRestitution, otherObjectAngularVelocity, objectAngularVelocity,
							rVectorOtherObject, rVectorObject, otherObjectInertia, objectInertia);

                        physicObjects[j].SetVelocity((Vector3)newVelocities[0], (float)newVelocities[2]);
                        physicObjects[i].SetVelocity((Vector3)newVelocities[1], (float)newVelocities[3]);
                    }

>>>>>>> Stashed changes
					test.SetColor("_Color", Color.red);
                    // modifier ici pour le coefficient de restitution @antony
                    BasicPhysicObject obj1 = physicObjects[i];
                    BasicPhysicObject otherObj1 = physicObjects[j];
                    MeshColliderScript obj2 = meshColliders[i];
                    MeshColliderScript otherObj2 = meshColliders[j];

                    float bouncinessAverage = (obj1.getBounciness() + otherObj1.getBounciness()) / 2.0f;

                    List<object> newVelocities = collisionManager.CollisionHasHappened(obj1.getVelocity(), otherObj1.getVelocity(), MTV, obj2.GetMass(), otherObj2.GetMass(), 
						bouncinessAverage, obj1.getAngularVelocity(), otherObj1.getAngularVelocity(), Vector3.zero, Vector3.zero, obj2.getInertia(), otherObj2.getInertia());
					physicObjects[i].ChangeForce((Vector3) newVelocities[0], (float) newVelocities[2]);
					physicObjects[j].ChangeForce((Vector3) newVelocities[1], (float) newVelocities[3]);
				}
			}
		}
	}

<<<<<<< Updated upstream
=======



>>>>>>> Stashed changes
	//Change the number of steps per second and update the Step length in consequence
	public void ChangeNumberOfStepsPerSecond(int newNumberOfStepsPerSecond) 
	{
		if (newNumberOfStepsPerSecond <= 0) { return; }
		else 
		{
			numberOfStepsPerSecond = newNumberOfStepsPerSecond;
			stepLength = 1.0f / numberOfStepsPerSecond;
		}
	}


}
