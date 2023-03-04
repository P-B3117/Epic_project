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
	float framesToDelay = 50;
	//List of all physics objects
	[SerializeField]
	List<GameObject> objects;
	[SerializeField]
	List<GameObject> joints;


	List<MeshColliderScript> meshColliders;
	List<BasicPhysicObject> physicObjects;
	List<Joints> physicsJoints;
	
	private int jointStartDelay = 50; // number of physics updates to wait before starting joint calculations
	private int currentUpdateCount = 0;
	public void Start()
	{

		meshColliders = new List<MeshColliderScript>();
		physicObjects = new List<BasicPhysicObject>();
		physicsJoints = new List<Joints>();
		for (int i = 0; i < objects.Count; i++) 
		{
			meshColliders.Add(objects[i].GetComponent<MeshColliderScript>());
			physicObjects.Add(objects[i].GetComponent<BasicPhysicObject>());

			physicObjects[i].SetCollider(meshColliders[i]);
		}
		
		for (int i = 0; i < joints.Count; i++)
		{
			physicsJoints.Add(joints[i].GetComponent<Joints>());

		}
		ChangeNumberOfStepsPerSecond(numberOfStepsPerSecond);
		numberOfUpdateCounter = 0;

		
	}


	//Update the physics objects on a fixed time rate
	public void Update()
	{
		numberOfUpdateCounter += Time.deltaTime / stepLength;
		currentUpdateCount++;
		while (numberOfUpdateCounter > 1) 
		{
			
			PhysicCalculations();
			if (currentUpdateCount >= jointStartDelay)
			{
				foreach (Joints joint in physicsJoints)
				{
					joint.DelayedStart();

					if (joints.Count > 0)
					{
						JointPhysicCalculations();
					}
				}
			}

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
			meshColliders[i].UpdateColliderOrientation();
			physicObjects[i].ApplyForceGravity();
			


		}


		
		//Test collisions et solve collisions
		
		for (int i = 0; i < objects.Count; i++) 
		{
			
			for (int j = i + 1; j < objects.Count; j++) 
			{
				CollisionInfo col;

				//Circle vs Circle
				if (meshColliders[i].IsCircle() && meshColliders[j].IsCircle())
				{
					col = HelperFunctionClass.TestCollisionTwoCircles(meshColliders[i].transform.position, meshColliders[i].RayonOfCircle(), meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
					if (col != null && col.GetMTV() != Vector3.zero)
					{


						//Displacement based on their respective mass
						float inverseMass = (1.0f / (meshColliders[i].GetMass() + meshColliders[j].GetMass()));
						//Oriente la normale en fonction du polygone de reference
						if (col.GetCollisionRef() == 1) { col.SetMTV(col.GetMTV() * -1); }
						meshColliders[j].Translate(col.GetMTV() * meshColliders[i].GetMass() * inverseMass);
						meshColliders[i].Translate(-col.GetMTV() * meshColliders[j].GetMass() * inverseMass);



						//Find the collision point
						col = HelperFunctionClass.FindCollisionPointTwoCircles(col, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle(), meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
						if (col == null) { continue; }


						//Solve the collision using impulse physic
						Vector3 normal = col.GetMTV().normalized;

						//Les RBP et RAP sont dans le mauvais sens mais en inversant les + et les - des operations, ca fonctionne quand meme!
						Vector3 rBP = meshColliders[i].transform.position - col.GetContactPoint();
						Vector3 rAP = meshColliders[j].transform.position - col.GetContactPoint();
						

						Vector3 perpBP = new Vector3(-rBP.y, rBP.x, 0);
						Vector3 perpAP = new Vector3(-rAP.y, rAP.x, 0);


						Vector3 vAP = physicObjects[j].getVelocity() - physicObjects[j].getAngularVelocity() * perpAP;
						Vector3 vBP = physicObjects[i].getVelocity() - physicObjects[i].getAngularVelocity() * perpBP;
						Vector3 relativeVelocity = vAP - vBP;
						float speedAlongNormal = Vector3.Dot(relativeVelocity, normal);

						float momentOfInertia1ImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpAP, normal), 2) / (meshColliders[j].GetInertia());
						float momentOfInertia2ImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpBP, normal), 2) / (meshColliders[i].GetInertia());
						float massImpulseInhibitor = (1.0f / meshColliders[j].GetMass() + 1.0f / meshColliders[i].GetMass());


						if (speedAlongNormal > 0) { continue; }
						float restitutionCollisionCoefficient = 0.5f;
						float j2 = -(1 + restitutionCollisionCoefficient) * speedAlongNormal;
						j2 /= (momentOfInertia1ImpulseInhibitor + momentOfInertia2ImpulseInhibitor + massImpulseInhibitor);


						Vector3 translationImpulse = j2 * normal;


						Vector3 newVelocity = physicObjects[j].getVelocity() + (1.0f / meshColliders[j].GetMass()) * translationImpulse;
						Vector3 otherNewVelocity = physicObjects[i].getVelocity() - (1.0f / meshColliders[i].GetMass()) * translationImpulse;

						

						float newAngularVelocity = physicObjects[j].getAngularVelocity() + (Vector3.Dot(perpAP, normal * -j2) / meshColliders[j].GetInertia());
						float otherNewAngularVelocity = physicObjects[i].getAngularVelocity() + (Vector3.Dot(perpBP, normal * j2) / meshColliders[i].GetInertia());

						physicObjects[j].SetVelocity(newVelocity, newAngularVelocity);
						physicObjects[i].SetVelocity(otherNewVelocity, otherNewAngularVelocity);


					}
				}
				//Circle vs Polygon
				else if (meshColliders[i].IsCircle()) 
				{
					col = HelperFunctionClass.TestCollisionPolygonCircle(meshColliders[j].GetWorldSpacePoints(), meshColliders[j].transform.position, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle());
					
					if (col != null && col.GetMTV() != Vector3.zero) 
					{
						
						
						//Displacement based on their respective mass
						float inverseMass = (1.0f / (meshColliders[i].GetMass() + meshColliders[j].GetMass()));
						meshColliders[i].Translate(col.GetMTV() * meshColliders[j].GetMass() * inverseMass);
						meshColliders[j].Translate(-col.GetMTV() * meshColliders[i].GetMass() * inverseMass);


						//Find the collisionPoint 
						col = HelperFunctionClass.FindCollisionPointPolygonCircle(col, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle());
						if (col == null) { continue; }



						//Solve the collision using impulse physic
						//IMPORTANT DE INVERSER LA NORMALE!!!!
						Vector3 normal = -col.GetMTV().normalized;
						



						//Les RBP et RAP sont dans le mauvais sens mais en inversant les + et les - des operations, ca fonctionne quand meme!
						Vector3 rBP = meshColliders[i].transform.position - col.GetContactPoint();
						Vector3 rAP = meshColliders[j].transform.position - col.GetContactPoint();
						

						Vector3 perpBP = new Vector3(-rBP.y, rBP.x, 0);
						Vector3 perpAP = new Vector3(-rAP.y, rAP.x, 0);


						Vector3 vAP = physicObjects[j].getVelocity() - physicObjects[j].getAngularVelocity() * perpAP;
						Vector3 vBP = physicObjects[i].getVelocity() - physicObjects[i].getAngularVelocity() * perpBP;
						Vector3 relativeVelocity = vAP - vBP;
						float speedAlongNormal = Vector3.Dot(relativeVelocity, normal);

						float momentOfInertia1ImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpAP, normal), 2) / (meshColliders[j].GetInertia());
						float momentOfInertia2ImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpBP, normal), 2) / (meshColliders[i].GetInertia());
						float massImpulseInhibitor = (1.0f / meshColliders[j].GetMass() + 1.0f / meshColliders[i].GetMass());


						if (speedAlongNormal > 0) { continue; }
						float restitutionCollisionCoefficient = 0.5f;
						float j2 = -(1 + restitutionCollisionCoefficient) * speedAlongNormal;
						j2 /= (momentOfInertia1ImpulseInhibitor + momentOfInertia2ImpulseInhibitor + massImpulseInhibitor);


						Vector3 translationImpulse = j2 * normal;


						Vector3 newVelocity = physicObjects[j].getVelocity() + (1.0f / meshColliders[j].GetMass()) * translationImpulse;
						Vector3 otherNewVelocity = physicObjects[i].getVelocity() - (1.0f / meshColliders[i].GetMass()) * translationImpulse;

						

						float newAngularVelocity = physicObjects[j].getAngularVelocity() + (Vector3.Dot(perpAP, normal * -j2) / meshColliders[j].GetInertia());
						float otherNewAngularVelocity = physicObjects[i].getAngularVelocity() + (Vector3.Dot(perpBP, normal * j2) / meshColliders[i].GetInertia());

						physicObjects[j].SetVelocity(newVelocity, newAngularVelocity);
						physicObjects[i].SetVelocity(otherNewVelocity, otherNewAngularVelocity);
					}
				}
				//Polygon vs Circle
				else if (meshColliders[j].IsCircle() ) 
				{
					col = HelperFunctionClass.TestCollisionPolygonCircle(meshColliders[i].GetWorldSpacePoints(), meshColliders[i].transform.position, meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
					
					if (col != null && col.GetMTV() != Vector3.zero)
					{
						//Displacement based on their respective mass
						float inverseMass = (1.0f / (meshColliders[i].GetMass() + meshColliders[j].GetMass()));
						meshColliders[j].Translate(col.GetMTV() * meshColliders[i].GetMass() * inverseMass);
						meshColliders[i].Translate(-col.GetMTV() * meshColliders[j].GetMass() * inverseMass);

						//Find the collisionPoint 
						col = HelperFunctionClass.FindCollisionPointPolygonCircle(col, meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
						if (col == null) { continue; }



						//Solve the collision using impulse physic
						Vector3 normal = col.GetMTV().normalized;
						


						//Les RBP et RAP sont dans le mauvais sens mais en inversant les + et les - des operations, ca fonctionne quand meme!
						Vector3 rBP = meshColliders[i].transform.position - col.GetContactPoint();
						Vector3 rAP = meshColliders[j].transform.position - col.GetContactPoint();
						

						Vector3 perpBP = new Vector3(-rBP.y, rBP.x, 0);
						Vector3 perpAP = new Vector3(-rAP.y, rAP.x, 0);


						Vector3 vAP = physicObjects[j].getVelocity() - physicObjects[j].getAngularVelocity() * perpAP;
						Vector3 vBP = physicObjects[i].getVelocity() - physicObjects[i].getAngularVelocity() * perpBP;
						Vector3 relativeVelocity = vAP - vBP;
						float speedAlongNormal = Vector3.Dot(relativeVelocity, normal);

						float momentOfInertia1ImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpAP, normal), 2) / (meshColliders[j].GetInertia());
						float momentOfInertia2ImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpBP, normal), 2) / (meshColliders[i].GetInertia());
						float massImpulseInhibitor = (1.0f / meshColliders[j].GetMass() + 1.0f / meshColliders[i].GetMass());


						if (speedAlongNormal > 0) { continue; }
						float restitutionCollisionCoefficient = 0.5f;
						float j2 = -(1 + restitutionCollisionCoefficient) * speedAlongNormal;
						j2 /= (momentOfInertia1ImpulseInhibitor + momentOfInertia2ImpulseInhibitor + massImpulseInhibitor);


						Vector3 translationImpulse = j2 * normal;


						Vector3 newVelocity = physicObjects[j].getVelocity() + (1.0f / meshColliders[j].GetMass()) * translationImpulse;
						Vector3 otherNewVelocity = physicObjects[i].getVelocity() - (1.0f / meshColliders[i].GetMass()) * translationImpulse;

						

						float newAngularVelocity = physicObjects[j].getAngularVelocity() + (Vector3.Dot(perpAP, normal * -j2) / meshColliders[j].GetInertia());
						float otherNewAngularVelocity = physicObjects[i].getAngularVelocity() + (Vector3.Dot(perpBP, normal * j2) / meshColliders[i].GetInertia());

						physicObjects[j].SetVelocity(newVelocity, newAngularVelocity);
						physicObjects[i].SetVelocity(otherNewVelocity, otherNewAngularVelocity);
					}
				}
				//Polygon vs Polygon
				else
				{
					col = HelperFunctionClass.TestCollisionSeperateAxisTheorem(meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());
					if (col != null && col.GetMTV() != Vector3.zero)
					{
						//Displacement based on their respective mass
						float inverseMass = (1.0f / (meshColliders[i].GetMass() + meshColliders[j].GetMass()));
						//Oriente la normale en fonction du polygone de reference
						if (col.GetCollisionRef() == 1) { col.SetMTV(col.GetMTV() * -1); }
						meshColliders[j].Translate(col.GetMTV() * meshColliders[i].GetMass() * inverseMass);
						meshColliders[i].Translate(-col.GetMTV() * meshColliders[j].GetMass() * inverseMass);



						//Find collisionPoint after displacement
						col = HelperFunctionClass.FindCollisionPoint(col, meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());
						if (col == null) { continue; }



						//Solve the collision using impulse physic
						Vector3 normal = col.GetMTV().normalized;

						//Les RBP et RAP sont dans le mauvais sens mais en inversant les + et les - des operations, ca fonctionne quand meme!
						Vector3 rBP = meshColliders[i].transform.position - col.GetContactPoint();
						Vector3 rAP = meshColliders[j].transform.position - col.GetContactPoint();
						

						Vector3 perpBP = new Vector3(-rBP.y, rBP.x, 0);
						Vector3 perpAP = new Vector3(-rAP.y, rAP.x, 0);


						Vector3 vAP = physicObjects[j].getVelocity() - physicObjects[j].getAngularVelocity() * perpAP;
						Vector3 vBP = physicObjects[i].getVelocity() - physicObjects[i].getAngularVelocity() * perpBP;
						Vector3 relativeVelocity = vAP - vBP;
						float speedAlongNormal = Vector3.Dot(relativeVelocity, normal);

						float momentOfInertia1ImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpAP, normal), 2) / (meshColliders[j].GetInertia());
						float momentOfInertia2ImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpBP, normal), 2) / (meshColliders[i].GetInertia());
						float massImpulseInhibitor = (1.0f / meshColliders[j].GetMass() + 1.0f / meshColliders[i].GetMass());


						if (speedAlongNormal > 0) { continue; }
						float restitutionCollisionCoefficient = 0.5f;
						float j2 = -(1 + restitutionCollisionCoefficient) * speedAlongNormal;
						j2 /= (momentOfInertia1ImpulseInhibitor + momentOfInertia2ImpulseInhibitor + massImpulseInhibitor);


						Vector3 translationImpulse = j2 * normal;


						Vector3 newVelocity = physicObjects[j].getVelocity() + (1.0f / meshColliders[j].GetMass()) * translationImpulse;
						Vector3 otherNewVelocity = physicObjects[i].getVelocity() - (1.0f / meshColliders[i].GetMass()) * translationImpulse;

						

						float newAngularVelocity = physicObjects[j].getAngularVelocity() + (Vector3.Dot(perpAP, normal * -j2) / meshColliders[j].GetInertia());
						float otherNewAngularVelocity = physicObjects[i].getAngularVelocity() + (Vector3.Dot(perpBP, normal * j2) / meshColliders[i].GetInertia());

						physicObjects[j].SetVelocity(newVelocity, newAngularVelocity);
						physicObjects[i].SetVelocity(otherNewVelocity, otherNewAngularVelocity);



					}
				}
				
		
				
			}
		}
	}
	private void JointPhysicCalculations()
	{
		for (int i = 0; i < joints.Count; i++)
		{
			physicsJoints[i].UpdateJointState(stepLength);
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
