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
	private int numberOfStepsPerSecond = 500;
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




	///A enlever juste pour DEBUG!!!
	Vector3 Normal;
	Vector3 perpAP1;
	CollisionInfo COLTEST = null;
	Vector3 circlePos;
	private void OnDrawGizmos()
	{

		if (COLTEST != null)
		{
			if (COLTEST.GetContactPoint() != Vector3.zero)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(COLTEST.GetContactPoint(), COLTEST.GetContactPoint() + COLTEST.GetMTV());
				Gizmos.DrawSphere(COLTEST.GetContactPoint(), 0.5f);

				Gizmos.color = Color.green;
				Gizmos.DrawLine(COLTEST.GetContactPoint(), COLTEST.GetContactPoint() + Normal);

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(COLTEST.GetContactPoint(), COLTEST.GetContactPoint() + perpAP1);

				Gizmos.color = Color.magenta;
				Gizmos.DrawLine(COLTEST.GetContactPoint(), COLTEST.GetContactPoint() + new Vector3(-perpAP1.y, perpAP1.x));
			}

			Gizmos.color = Color.black;
			
			Gizmos.DrawLine(circlePos - COLTEST.GetMTV().normalized , circlePos - COLTEST.GetMTV().normalized + COLTEST.GetMTV());
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
				CollisionInfo col;

				//Circle vs Circle
				if (meshColliders[i].IsCircle() && meshColliders[j].IsCircle())
				{
					col = HelperFunctionClass.TestCollisionTwoCircles(meshColliders[i].transform.position, meshColliders[i].RayonOfCircle(), meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
					if (col != null && col.GetMTV() != Vector3.zero)
					{


						COLTEST = col;
						float inverseMass = (1.0f / (meshColliders[i].GetMass() + meshColliders[j].GetMass()));
						


						//Oriente la normale en fonction du polygone de reference
						if (COLTEST.GetCollisionRef() == 1) { COLTEST.SetMTV(COLTEST.GetMTV() * -1); }
						


						meshColliders[j].Translate(COLTEST.GetMTV() * meshColliders[i].GetMass() * inverseMass);
						meshColliders[i].Translate(-COLTEST.GetMTV() * meshColliders[j].GetMass() * inverseMass);

						col = HelperFunctionClass.FindCollisionPointTwoCircles(col, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle(), meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
						if (col == null) { continue; }

						Vector3 normal = col.GetMTV().normalized;

						//Les RBP et RAP sont dans le mauvais sens mais en inversant les + et les - des operations, ca fonctionne quand meme!
						Vector3 rBP = meshColliders[i].transform.position - col.GetContactPoint();
						Vector3 rAP = meshColliders[j].transform.position - col.GetContactPoint();
						perpAP1 = rAP;

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

						Normal = normal * j2;

						float newAngularVelocity = physicObjects[j].getAngularVelocity() + (Vector3.Dot(perpAP, normal * -j2) / meshColliders[j].GetInertia());
						float otherNewAngularVelocity = physicObjects[i].getAngularVelocity() + (Vector3.Dot(perpBP, normal * j2) / meshColliders[i].GetInertia());

						physicObjects[j].SetVelocity(newVelocity, newAngularVelocity);
						physicObjects[i].SetVelocity(otherNewVelocity, otherNewAngularVelocity);


					}
				}
				else if (meshColliders[i].IsCircle()) 
				{
					col = HelperFunctionClass.TestCollisionPolygonCircle(meshColliders[j].GetWorldSpacePoints(), meshColliders[j].transform.position, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle());
					circlePos = meshColliders[i].transform.position;
					if (col != null && col.GetMTV() != Vector3.zero) 
					{
						COLTEST = col;
						

						float inverseMass = (1.0f / (meshColliders[i].GetMass() + meshColliders[j].GetMass()));

						meshColliders[i].Translate(COLTEST.GetMTV() * meshColliders[j].GetMass() * inverseMass);
						meshColliders[j].Translate(-COLTEST.GetMTV() * meshColliders[i].GetMass() * inverseMass);

						col = HelperFunctionClass.FindCollisionPointPolygonCircle(col, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle());
						if (col == null) { continue; }

						//IMPORTANT DE INVERSER LA NORMALE!!!!
						Vector3 normal = -col.GetMTV().normalized;
						



						//Les RBP et RAP sont dans le mauvais sens mais en inversant les + et les - des operations, ca fonctionne quand meme!
						Vector3 rBP = meshColliders[i].transform.position - col.GetContactPoint();
						Vector3 rAP = meshColliders[j].transform.position - col.GetContactPoint();
						perpAP1 = rAP;

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

						Normal = normal * j2;

						float newAngularVelocity = physicObjects[j].getAngularVelocity() + (Vector3.Dot(perpAP, normal * -j2) / meshColliders[j].GetInertia());
						float otherNewAngularVelocity = physicObjects[i].getAngularVelocity() + (Vector3.Dot(perpBP, normal * j2) / meshColliders[i].GetInertia());

						physicObjects[j].SetVelocity(newVelocity, newAngularVelocity);
						physicObjects[i].SetVelocity(otherNewVelocity, otherNewAngularVelocity);
					}
				}
				else if (meshColliders[j].IsCircle() ) 
				{
					col = HelperFunctionClass.TestCollisionPolygonCircle(meshColliders[i].GetWorldSpacePoints(), meshColliders[i].transform.position, meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
					circlePos = meshColliders[j].transform.position;
					if (col != null && col.GetMTV() != Vector3.zero)
					{
						COLTEST = col;
						



						float inverseMass = (1.0f / (meshColliders[i].GetMass() + meshColliders[j].GetMass()));

						meshColliders[j].Translate(COLTEST.GetMTV() * meshColliders[i].GetMass() * inverseMass);
						meshColliders[i].Translate(-COLTEST.GetMTV() * meshColliders[j].GetMass() * inverseMass);

						col = HelperFunctionClass.FindCollisionPointPolygonCircle(col, meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
						if (col == null) { continue; }


						Vector3 normal = col.GetMTV().normalized;
						


						//Les RBP et RAP sont dans le mauvais sens mais en inversant les + et les - des operations, ca fonctionne quand meme!
						Vector3 rBP = meshColliders[i].transform.position - col.GetContactPoint();
						Vector3 rAP = meshColliders[j].transform.position - col.GetContactPoint();
						perpAP1 = rAP;

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

						Normal = normal * j2;

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


						COLTEST = col;
						float inverseMass = (1.0f / (meshColliders[i].GetMass() + meshColliders[j].GetMass()));

						//Oriente la normale en fonction du polygone de reference
						if (COLTEST.GetCollisionRef() == 1) { COLTEST.SetMTV(COLTEST.GetMTV() * -1); }
						


						meshColliders[j].Translate(COLTEST.GetMTV() * meshColliders[i].GetMass() * inverseMass);
						meshColliders[i].Translate(-COLTEST.GetMTV() * meshColliders[j].GetMass() * inverseMass);


						//Find collisionPoint after displacement
						col = HelperFunctionClass.FindCollisionPoint(col, meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());
						if (col == null) { continue; }



						Vector3 normal = col.GetMTV().normalized;

						//Les RBP et RAP sont dans le mauvais sens mais en inversant les + et les - des operations, ca fonctionne quand meme!
						Vector3 rBP = meshColliders[i].transform.position - col.GetContactPoint();
						Vector3 rAP = meshColliders[j].transform.position - col.GetContactPoint();
						perpAP1 = rAP;

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

						Normal = normal * j2;

						float newAngularVelocity = physicObjects[j].getAngularVelocity() + (Vector3.Dot(perpAP, normal * -j2) / meshColliders[j].GetInertia());
						float otherNewAngularVelocity = physicObjects[i].getAngularVelocity() + (Vector3.Dot(perpBP, normal * j2) / meshColliders[i].GetInertia());

						physicObjects[j].SetVelocity(newVelocity, newAngularVelocity);
						physicObjects[i].SetVelocity(otherNewVelocity, otherNewAngularVelocity);



					}
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
