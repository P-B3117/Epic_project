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
		Gizmos.color = Color.black;
		
		Gizmos.DrawLine(Vector3.zero, MTV);
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
					MTV = col.GetMTV();
					test.SetColor("_Color", Color.red);
                    // modifier ici pour le coefficient de restitution @antony
                    BasicPhysicObject obj1 = physicObjects[i];
                    BasicPhysicObject otherObj1 = physicObjects[j];
                    MeshColliderScript obj2 = meshColliders[i];
                    MeshColliderScript otherObj2 = meshColliders[j];

                    float bouncinessAverage = (obj1.getBounciness() + otherObj1.getBounciness()) / 2.0f;

                    List<Vector3> newVelocities = collisionManager.CollisionHasHappened(obj1.getVelocity(), otherObj1.getVelocity(), MTV, obj2.GetMass(), otherObj2.GetMass(), 
						bouncinessAverage, obj1.getAngularVelocity(), otherObj1.getAngularVelocity(), Vector3.zero, Vector3.zero, obj2.getInertia(), otherObj2.getInertia());
					physicObjects[i].ChangeForce(newVelocities[0]);
					physicObjects[j].ChangeForce(newVelocities[1]);
				}
			}
		}
	}

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
