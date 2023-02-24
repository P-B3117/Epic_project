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


	
	
	
	CollisionInfo COLTEST = null;
	private void OnDrawGizmos()
	{
		if (COLTEST != null) 
		{
			Gizmos.DrawSphere(COLTEST.GetContactPoint(), 0.2f);
		}
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
			
			
			meshColliders[i].UpdateColliderOrientation();
		}


		
		//Test collisions
		test.SetColor("_Color", Color.green);
		for (int i = 0; i < objects.Count; i++) 
		{
			
			for (int j = i + 1; j < objects.Count; j++) 
			{
				CollisionInfo col = HelperFunctionClass.TestCollisionSeperateAxisTheorem(meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());
		
				if (col != null)
				{

					
					COLTEST = col;
					float inverseMass = (1.0f / (meshColliders[i].GetMass() + meshColliders[j].GetMass()));

					if (COLTEST.GetCollisionRef() == 0)
					{
						objects[j].transform.position += COLTEST.GetMTV() * meshColliders[i].GetMass() * inverseMass;
						objects[i].transform.position -= COLTEST.GetMTV() * meshColliders[j].GetMass() * inverseMass;


						Vector3 normal = col.GetMTV().normalized;
						Vector3 relativeVelocity = physicObjects[j].getVelocity() - physicObjects[i].getVelocity();
						float speedAlongNormal = Vector3.Dot(relativeVelocity, normal);
						
						if (speedAlongNormal > 0) { continue; }
						float restitutionCollisionCoefficient = 1.0f;
						float j2 = -(1 + restitutionCollisionCoefficient) * speedAlongNormal;
						Vector3 impulse = j2 * normal;
						impulse /= (1.0f / meshColliders[j].GetMass() + 1.0f / meshColliders[i].GetMass());
						
						Vector3 newVelocity = physicObjects[j].getVelocity() + (1.0f / meshColliders[j].GetMass()) * impulse;
						Vector3 otherNewVelocity = physicObjects[i].getVelocity() - (1.0f / meshColliders[i].GetMass()) * impulse;
						physicObjects[j].SetVelocity(newVelocity, 0f);
						physicObjects[i].SetVelocity(otherNewVelocity, 0f);

					}
					else 
					{
						objects[j].transform.position -= COLTEST.GetMTV() * meshColliders[i].GetMass() * inverseMass;
						objects[i].transform.position += COLTEST.GetMTV() * meshColliders[j].GetMass() * inverseMass;


						


						Vector3 normal = col.GetMTV().normalized;
						Vector3 relativeVelocity = physicObjects[i].getVelocity() - physicObjects[j].getVelocity();
						float speedAlongNormal = Vector3.Dot(relativeVelocity, normal);
						
						if (speedAlongNormal > 0) { continue; }
						float restitutionCollisionCoefficient =1.0f ;
						float j2 = -(1 + restitutionCollisionCoefficient) * speedAlongNormal;
						Vector3 impulse = j2 * normal;
						impulse /= (1.0f / meshColliders[j].GetMass() + 1.0f / meshColliders[i].GetMass());
						Vector3 newVelocity = physicObjects[i].getVelocity() + (1.0f / meshColliders[i].GetMass()) * impulse;
						Vector3 otherNewVelocity = physicObjects[j].getVelocity() - (1.0f / meshColliders[j].GetMass()) * impulse;
						physicObjects[i].SetVelocity(newVelocity, 0f);
						physicObjects[j].SetVelocity(otherNewVelocity, 0f);

						
					}

					test.SetColor("_Color", Color.red);
					
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
