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
	private int numberOfStepsPerSecond = 100;
	private float stepLength;
	private float numberOfUpdateCounter = 0;

	//List of all physics objects
	[SerializeField]
	List<GameObject> objects;
	[SerializeField]
	List<GameObject> joints;
	

	public Vector2 gravity;

	List<MeshColliderScript> meshColliders;
	List<BasicPhysicObject> physicObjects;
	List<DistanceJoints> physicsJoints;


	public void Start()
	{

		meshColliders = new List<MeshColliderScript>();
		physicObjects = new List<BasicPhysicObject>();
		physicsJoints = new List<DistanceJoints>();
		for (int i = 0; i < objects.Count; i++)
		{
			meshColliders.Add(objects[i].GetComponent<MeshColliderScript>());
			meshColliders[i].SetUpMesh();

            physicObjects.Add(objects[i].GetComponent<BasicPhysicObject>());

			physicObjects[i].SetCollider(meshColliders[i]);
			physicObjects[i].IsWall = physicObjects[i].name.Contains("Wall");

		}
		for (int i = 0; i < joints.Count; i++)
		{
			physicsJoints.Add(joints[i].GetComponent<DistanceJoints>());

		}


	


		ChangeNumberOfStepsPerSecond(numberOfStepsPerSecond);
		numberOfUpdateCounter = 0;
		

	}




	//Update the physics objects on a fixed time rate
	public void Update()
	{
		numberOfUpdateCounter += UniversalVariable.GetTime()* Time.deltaTime / stepLength;

		while (numberOfUpdateCounter > 1)
		{

			PhysicCalculations();
			if (joints.Count > 0)
			{
				JointPhysicCalculations();
			}
			numberOfUpdateCounter--;
		}

	}

	public void ResetList()
	{
		int count = physicObjects.Count;
		
		for (int i = count - 1; i >= 0; i--)
		{
			GameObject obj = objects[i];


            if (!physicObjects[i].IsWall)
			{
				Destroy(obj);
				physicObjects.RemoveAt(i);
				meshColliders.RemoveAt(i);
				objects.RemoveAt(i);
			}
		}

		for (int i = joints.Count-1 ; i >= 0; i--) 
		{
			Destroy(joints[i]);
			joints.RemoveAt(i);
			physicsJoints.RemoveAt(i);
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

				//Test AABB
				if (HelperFunctionClass.AABBCollision(meshColliders[i], meshColliders[j]))
				{

					CollisionInfo col;
					int changeUp = 1;
					// lets put the first one positive as a base and second one negative as a base
					bool iCircle = meshColliders[i].IsCircle();
					bool jCircle = meshColliders[j].IsCircle();
					if (iCircle && jCircle)
					{
						col = HelperFunctionClass.TestCollisionTwoCircles(meshColliders[i].transform.position, meshColliders[i].RayonOfCircle(), meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
						if (col != null && col.GetMTV() != Vector3.zero && col.GetCollisionRef() == 1) { col.SetMTV(col.GetMTV() * -1); }
					}


					else if (iCircle)
					{
						col = HelperFunctionClass.TestCollisionPolygonCircle(meshColliders[j].GetWorldSpacePoints(), meshColliders[j].transform.position, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle());
						changeUp = -1;
						
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
					if (iCircle && jCircle)
					{
						//Find the collision point
						col = HelperFunctionClass.FindCollisionPointTwoCircles(col, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle(), meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
						if (col == null) { continue; }


						//Solve the collision using impulse physic
						normal = col.GetMTV().normalized;

				



					}
					//Circle vs Polygon
					else if (iCircle)
					{
						
						
						//Find the collisionPoint 
						col = HelperFunctionClass.FindCollisionPointPolygonCircle(col, meshColliders[i].transform.position, meshColliders[i].RayonOfCircle());
						if (col == null) { continue; }

						//Solve the collision using impulse physic
						normal = col.GetMTV().normalized;

				

						//IMPORTANT DE INVERSER LA NORMALE!!!!
						normal *= -1;

					}
					//Polygon vs Circle
					else if (jCircle)
					{
						//Find the collisionPoint 
						col = HelperFunctionClass.FindCollisionPointPolygonCircle(col, meshColliders[j].transform.position, meshColliders[j].RayonOfCircle());
						if (col == null) { continue; }
						//Solve the collision using impulse physic
						normal = col.GetMTV().normalized;

					

					}
					//Polygon vs Polygon
					else
					{
						//Find collisionPoint after displacement
						col = HelperFunctionClass.FindCollisionPoint(col, meshColliders[i].GetWorldSpacePoints(), meshColliders[j].GetWorldSpacePoints());
						if (col == null) { continue; }
						//Solve the collision using impulse physic
						normal = col.GetMTV().normalized;

						

					}
                    
                    CollisionManager collisionManager = new CollisionManager();
					


					//Les RBP et RAP sont dans le mauvais sens mais en inversant les + et les - des operations, ca fonctionne quand meme!
					Vector3 rBP = meshColliders[i].transform.position - col.GetContactPoint();
					Vector3 rAP = meshColliders[j].transform.position - col.GetContactPoint();
					float restitutionCollisionCoefficient = (physicObjects[j].getBounciness() + physicObjects[i].getBounciness()) / 2f;
					List<object> newVelocities = collisionManager.CollisionHasHappened(physicObjects[j].getVelocity(), physicObjects[i].getVelocity(), normal, meshColliders[j].GetMass(),
						meshColliders[i].GetMass(), restitutionCollisionCoefficient, physicObjects[j].getAngularVelocity(), physicObjects[i].getAngularVelocity(), rAP, rBP,
						meshColliders[j].GetInertia(), meshColliders[i].GetInertia(),
						physicObjects[i].getStaticFriction(), physicObjects[i].getDynamicFriction(),
						physicObjects[j].getStaticFriction(), physicObjects[j].getDynamicFriction());

					physicObjects[j].SetVelocity((Vector3)newVelocities[0], (float)newVelocities[2], stepLength);
					physicObjects[i].SetVelocity((Vector3)newVelocities[1], (float)newVelocities[3], stepLength);
				}
			}
		}


		


	}


	//Add physic object to list of physicObjects
	public void AddPhysicObject(GameObject go) 
	{
		objects.Add(go);
		MeshColliderScript mc = go.GetComponent<MeshColliderScript>();
		mc.SetBasicMaterial();
		BasicPhysicObject po = go.GetComponent<BasicPhysicObject>();
		mc.SetUpMesh();
		mc.UpdateColliderOrientation();
		po.SetCollider(mc);
		meshColliders.Add(mc);
		physicObjects.Add(po);
		
	}


	//Return the index of the Object that the mouse clicked on
	//-1 = nothing was touched
	//anything bigger or equal to 0 = the function returns the index of the polygon clicked
	public int FindClickIndex(Vector3 mousePos) 
	{

		for (int i = 0; i < meshColliders.Count; i++) 
		{
			MeshColliderScript mc = meshColliders[i];
			if (mc.IsCircle())
			{
				Vector3 diff = mousePos - mc.transform.position;
				float length = diff.magnitude;
				if (length <= mc.RayonOfCircle()) { return i; }
			}
			else 
			{
				bool isInside = HelperFunctionClass.polygonPoint(mc.GetWorldSpacePoints(), mousePos);
				bool isWall = mc.transform.gameObject.name.Contains("Wall");
				if (isInside && !isWall) { return i; }
			}
		}
		return -1;
	}

	//Change material based on index of the selection
	public BasicPhysicObject SelectSpecificObject(int index, PrefabsHolder ph) 
	{
		
		if (index < meshColliders.Count && index != -1)
		{
			
			for (int i = 0; i < index; i++)
			{
				meshColliders[i].SetBasicMaterial();
			}
			meshColliders[index].SetSelectedMaterial(ph);
			
			for (int i = index + 1; i < physicObjects.Count; i++)
			{
				meshColliders[i].SetBasicMaterial();

			}
			return physicObjects[index];
		}
		else if (index < 0) 
		{
			for (int i = 0; i < meshColliders.Count; i++)
			{
				meshColliders[i].SetBasicMaterial();
			}
		}
		
		return null;
	}

	//Method for the calculations of the joints properties
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

	public MeshColliderScript GetMeshCollliderObjectAt(int index)
	{
		if (index < meshColliders.Count && index != -1)
		{
			return meshColliders[index];
		}
		else { return null; }
	}
	public BasicPhysicObject GetPhysicObjectAt(int index)
	{
		if (index < physicObjects.Count && index != -1)
		{
			return physicObjects[index];
		}
		else { return null; }
	}

}