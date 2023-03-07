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

		Gizmos.color = Color.black;
		
		Gizmos.DrawLine(Vector3.zero, MTV);

		if (COLTEST != null) 
		{
			Gizmos.DrawLine(COLTEST.GetWorldContactPoint(), COLTEST.GetWorldContactPoint() + COLTEST.GetMTV());
			Gizmos.DrawSphere(COLTEST.GetWorldContactPoint(), 0.1f);
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
			physicObjects[i].ApplyForceGravity();
			
			
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

<<<<<<< Updated upstream
					MTV = col.GetMTV();

					
					
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


					if (COLTEST.GetCollisionRef() == 0)
					{

						
						meshColliders[j].Translate( COLTEST.GetMTV() * meshColliders[i].GetMass() * inverseMass);
						
						meshColliders[i].Translate(-COLTEST.GetMTV() * meshColliders[j].GetMass() * inverseMass);

						
						//Find collisionPoint after displacement
						col = HelperFunctionClass.FindCollisionPoint(col, meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());

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
						meshColliders[j].Translate(-COLTEST.GetMTV() * meshColliders[i].GetMass() * inverseMass);

						meshColliders[i].Translate(COLTEST.GetMTV() * meshColliders[j].GetMass() * inverseMass);

						//Find collisionPoint after displacement
						col = HelperFunctionClass.FindCollisionPoint(col, meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());



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
=======
					CollisionInfo col;
					int changeUp = 1;
					// lets put the first one positive as a base and second one negative as a base
					bool iCircle = meshColliders[i].IsCircle();
					bool jCircle = meshColliders[j].IsCircle();
					if (iCircle && jCircle)
					{
						col = HelperFunctionClass.TestCollisionTwoCircles(meshColliders[i].transform.position, meshColliders[i].RayonOfCircle(), meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
						if (col != null && col.GetMTV() != Vector3.zero && col.GetCollisionRef() == 1) { col.SetMTV(col.GetMTV() * -1); }
						changeUp = -1;
					}
					else if (iCircle)
					{
                        col = HelperFunctionClass.TestCollisionPolygonCircle(meshColliders[j].GetWorldSpacePoints(), meshColliders[j].transform.position, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle());
                    }
					else if (jCircle)
					{
                        col = HelperFunctionClass.TestCollisionPolygonCircle(meshColliders[i].GetWorldSpacePoints(), meshColliders[i].transform.position, meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
                    }
					else
					{
                        col = HelperFunctionClass.TestCollisionSeperateAxisTheorem(meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());
                        if (col != null && col.GetMTV() != Vector3.zero && col.GetCollisionRef() == 1) { col.SetMTV(col.GetMTV() * -1); }
                    }

					bool iStatic = physicObjects[i].IsStatic();
                    bool jStatic = physicObjects[j].IsStatic();
                    if (col != null && col.GetMTV() != Vector3.zero)
                    {
                        //Displacement based on their respective mass
                        //Oriente la normale en fonction du polygone de reference
                        if (iStatic && !jStatic)
                        {
                            meshColliders[j].Translate(col.GetMTV() * changeUp);

                        }
                        else if (jStatic && !iStatic)
                        {
                            meshColliders[i].Translate(-col.GetMTV() * changeUp);

                        }
                        else
                        {
                            meshColliders[j].Translate(col.GetMTV() / 2 * changeUp);
                            meshColliders[i].Translate(-col.GetMTV() / 2 * changeUp);
                        }
                    }

					else { continue; }

					Vector3 normal;

                    //Circle vs Circle
                    if (meshColliders[i].IsCircle() && meshColliders[j].IsCircle())
					{
						//Find the collision point
						col = HelperFunctionClass.FindCollisionPointTwoCircles(col, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle(), meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
						if (col == null) { continue; }


						//Solve the collision using impulse physic
						normal = col.GetMTV().normalized;


					}
					//Circle vs Polygon
					else if (meshColliders[i].IsCircle())
					{
						//Find the collisionPoint 
						col = HelperFunctionClass.FindCollisionPointPolygonCircle(col, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle());
						if (col == null) { continue; }

						//Solve the collision using impulse physic
						//IMPORTANT DE INVERSER LA NORMALE!!!!
						normal = -col.GetMTV().normalized;


					}
					//Polygon vs Circle
					else if (meshColliders[j].IsCircle())
					{
						//Find the collisionPoint 
						col = HelperFunctionClass.FindCollisionPointPolygonCircle(col, meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());

						//Solve the collision using impulse physic
						normal = col.GetMTV().normalized;
					}
					//Polygon vs Polygon
					else
					{
						//Find collisionPoint after displacement
						col = HelperFunctionClass.FindCollisionPoint(col, meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());

						//Solve the collision using impulse physic
						normal = col.GetMTV().normalized;
					}
                    if (col == null) { continue; }
                    CollisionManager collisionManager = new CollisionManager();
                    //Les RBP et RAP sont dans le mauvais sens mais en inversant les + et les - des operations, ca fonctionne quand meme!
                    Vector3 rBP = meshColliders[i].transform.position - col.GetContactPoint();
                    Vector3 rAP = meshColliders[j].transform.position - col.GetContactPoint();
                    float restitutionCollisionCoefficient = (physicObjects[j].getBounciness() + physicObjects[i].getBounciness()) / 2f;
					List<object> newVelocities = collisionManager.CollisionHasHappened(physicObjects[j].getVelocity(), physicObjects[i].getVelocity(), normal, meshColliders[j].GetMass(), 
						meshColliders[i].GetMass(), restitutionCollisionCoefficient, physicObjects[j].getAngularVelocity(), physicObjects[i].getAngularVelocity(), rAP, rBP, 
						meshColliders[j].GetInertia(), meshColliders[i].GetInertia());

					//Vector3 perpBP = new Vector3(-rBP.y, rBP.x, 0);
					//Vector3 perpAP = new Vector3(-rAP.y, rAP.x, 0);


					//Vector3 vAP = physicObjects[j].getVelocity() - physicObjects[j].getAngularVelocity() * perpAP;
					//Vector3 vBP = physicObjects[i].getVelocity() - physicObjects[i].getAngularVelocity() * perpBP;
					//Vector3 relativeVelocity = vAP - vBP;
					// float speedAlongNormal = Vector3.Dot(relativeVelocity, normal);

					//float momentOfInertia1ImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpAP, normal), 2) / (meshColliders[j].GetInertia());
					//float momentOfInertia2ImpulseInhibitor = Mathf.Pow(Vector3.Dot(perpBP, normal), 2) / (meshColliders[i].GetInertia());
					//float massImpulseInhibitor = (1.0f / meshColliders[j].GetMass() + 1.0f / meshColliders[i].GetMass());


					// if (speedAlongNormal > 0) { continue; }

					//float j2 = -(1 + restitutionCollisionCoefficient) * speedAlongNormal;
					//j2 /= (momentOfInertia1ImpulseInhibitor + momentOfInertia2ImpulseInhibitor + massImpulseInhibitor);


					//Vector3 translationImpulse = j2 * normal;

					//Vector3 newVelocity = physicObjects[j].getVelocity() + (1.0f / meshColliders[j].GetMass()) * translationImpulse;
					//Vector3 otherNewVelocity = physicObjects[i].getVelocity() - (1.0f / meshColliders[i].GetMass()) * translationImpulse;

					//float newAngularVelocity = physicObjects[j].getAngularVelocity() + (Vector3.Dot(perpAP, normal * -j2) / meshColliders[j].GetInertia());
					//float otherNewAngularVelocity = physicObjects[i].getAngularVelocity() + (Vector3.Dot(perpBP, normal * j2) / meshColliders[i].GetInertia());

					physicObjects[j].SetVelocity((Vector3) newVelocities[0], (float) newVelocities[2]);
					physicObjects[i].SetVelocity((Vector3) newVelocities[1], (float) newVelocities[3]);
                }
>>>>>>> Stashed changes
			}
		}
	}

<<<<<<< Updated upstream
=======
	private void JointPhysicCalculations()
	{
		for (int i = 0; i < joints.Count; i++)
		{
			physicsJoints[i].UpdateJointState(stepLength);
		}
	}
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
