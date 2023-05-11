using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;


/*
 * Filename : PhysicsManager
 * Author : Louis-E, Charles, Clovis,Antony,Justin
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

	public List<MeshColliderScript> meshColliders;
	List<BasicPhysicObject> physicObjects;
	public List<DistanceJoints> physicsJoints;
	List<GrabJoint> physicsGrabJoints;

	public void Start()
	{

		meshColliders = new List<MeshColliderScript>();
		physicObjects = new List<BasicPhysicObject>();
		physicsJoints = new List<DistanceJoints>();
		physicsGrabJoints = new List<GrabJoint>();
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
		for (int i = 0; i < physicsJoints.Count; i++) 
		{
			physicsJoints[i].Initialize();
		}
		for (int i = 0; i < physicsGrabJoints.Count; i++)
        {
            physicsGrabJoints[i].Initialize();
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
			
			JointPhysicCalculations();
			
			numberOfUpdateCounter--;
		}
	}


	//Refresh all reference lists
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
		}

		for (int i = physicsJoints.Count - 1; i >= 0; i--)
		{
			Destroy(physicsJoints[i].gameObject);
			physicsJoints.RemoveAt(i);
		}

	}

	//Detruit un objet specifique de la scene
    public void RemoveAt(BasicPhysicObject bo, GameObject parent)
	{

		if (parent != null && parent.GetComponent<SoftBody>() != null)
		{
			BasicPhysicObject[] bos = parent.GetComponentsInChildren<BasicPhysicObject>();
			MeshColliderScript[] mcs = parent.GetComponentsInChildren<MeshColliderScript>();

			for (int i = meshColliders.Count - 1; i >= 0; i--) 
			{
				for (int j = 0; j < bos.Length; j++) 
				{
					if (mcs[j] == meshColliders[i]) 
					{
						for (int k = physicsJoints.Count-1; k >= 0; k--)
						{
							if (physicsJoints[k].bo1 == mcs[j].gameObject || physicsJoints[k].bo2 == mcs[j].gameObject) 
							{
								Destroy(physicsJoints[k].gameObject);

								physicsJoints.RemoveAt(k);

							}
						}
						meshColliders.RemoveAt(i);
						physicObjects.RemoveAt(i);
						objects.RemoveAt(i);
						break;
					}
				}
			}
			DistanceJoints[] dos = parent.GetComponentsInChildren<DistanceJoints>();
			for (int i = physicsJoints.Count - 1; i >= 0; i--) 
			{
				for (int j = 0; j < dos.Length; j++) 
				{
					if (dos[j] == physicsJoints[i]) 
					{
						physicsJoints.RemoveAt(i);
						joints.RemoveAt(i);
						break;
					}
				}
			}




			Destroy(parent);




		}
		else 
		{
			GameObject go = bo.gameObject;
			MeshColliderScript mcs = go.GetComponent<MeshColliderScript>();
			
			int index = -1;
			for (int i = 0; i < meshColliders.Count; i++) 
			{
				if (mcs == meshColliders[i]) 
				{
					index = i;
					break;
				}
			}
			if (index != -1) 
			{
				for (int k = physicsJoints.Count - 1; k >= 0; k--)
				{
					if (physicsJoints[k].bo1 == mcs.gameObject || physicsJoints[k].bo2 == mcs.gameObject)
					{
						
						Destroy(physicsJoints[k].gameObject);
						
						physicsJoints.RemoveAt(k);
						
						
					}
				}
				meshColliders.RemoveAt(index);
				physicObjects.RemoveAt(index);
				objects.RemoveAt(index);
				Destroy(go);
				
			}

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
            physicObjects[i].ApplyAirDensity();
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
					bool iWall = physicObjects[i].IsWall;
					bool jWall = physicObjects[j].IsWall;
					if (col != null && col.GetMTV() != Vector3.zero)
					{
						//Displacement based on their respective mass
						//Oriente la normale en fonction du polygone de reference
						if (iWall)
						{
							meshColliders[j].Translate(col.GetMTV() * changeUp);
						}
						else if(jWall) 
						{
							meshColliders[i].Translate(col.GetMTV() * changeUp);
						}
						else if (iStatic && !jStatic)
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

	//Add sotfbody to the simulation scene with all the references requirements
	public void AddSoftBody(GameObject softBody) 
	{
		BasicPhysicObject[] bos = softBody.GetComponentsInChildren<BasicPhysicObject>();
		DistanceJoints[] djs = softBody.GetComponentsInChildren<DistanceJoints>();

		for (int i = 0; i < bos.Length; i++) 
		{
			MeshColliderScript mc = bos[i].gameObject.GetComponent<MeshColliderScript>();
			mc.SetUpMesh();
			mc.UpdateColliderOrientation();
			bos[i].SetCollider(mc);
			meshColliders.Add(mc);
			physicObjects.Add(bos[i]);
			objects.Add(bos[i].gameObject);

		}

		for (int i = 0; i < djs.Length; i++) 
		{
			
			djs[i].Initialize();
			physicsJoints.Add(djs[i]);
			joints.Add(djs[i].gameObject);
			
		}
		softBody.GetComponent<SoftBody>().Initialise();
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
	//when index is -1 : Resets the selected object
	//when index is bigger or equal to 0 : Resets all objects except for the selected one;
	public BasicPhysicObject SelectSpecificObject(int index, PrefabsHolder ph, GameObject oldSelectedObject) 
	{
		
		//For deselecting the selected softbody
		if (oldSelectedObject != null)
		{
			
			SoftBody sb = oldSelectedObject.GetComponent<SoftBody>();
			if (sb != null)
			{
				
				sb.SetBasicMaterial();
			}
		}

		if (index < meshColliders.Count && index >= 0)
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
	public void AddGrabJoint(GameObject jo)
	{
		GrabJoint gb = jo.GetComponent<GrabJoint>();
		gb.Initialize();
		physicsGrabJoints.Add(gb);
	}
	public void AddDistanceJoints(GameObject jo)
	{
		DistanceJoints gb = jo.GetComponent<DistanceJoints>();
		gb.Initialize();
		physicsJoints.Add(gb);
		
	}
	public List<DistanceJoints> Joints()
    {
		if(physicsJoints == null) { return null; }
		else return physicsJoints;
		
    }
	public void ResetGrabJoint()
	{
		for (int i = physicsGrabJoints.Count - 1; i >= 0; i--)
		{
			Destroy(physicsGrabJoints[i].gameObject);
			physicsGrabJoints.RemoveAt(i);
		}
	}
	//Method for the calculations of the joints properties
	private void JointPhysicCalculations()
	{
		for (int i = 0; i < physicsJoints.Count; i++)
		{
			if (physicsJoints[i] == null)
			{
				physicsJoints.RemoveAt(i);
				i--;
				continue;
			}
			physicsJoints[i].UpdateJointState(stepLength);
		}
		for (int i = 0; i < physicsGrabJoints.Count; i++)
		{
            if (physicsGrabJoints[i] == null)
            {
                physicsGrabJoints.RemoveAt(i);
                i--;
                continue;
            }
            physicsGrabJoints[i].UpdateJointState(stepLength);
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

	public void RemoveJoint(DistanceJoints dj) 
	{
		for (int i = 0; i < physicsJoints.Count; i++) 
		{
			if (physicsJoints[i] == dj) 
			{
				physicsJoints.RemoveAt(i);
			}
		}
	}


	//Return the list of all the distanceJoints that are not part of a SoftBody
	public List<DistanceJoints> GetAllNonSoftBodyJoints() 
	{
		List<DistanceJoints> list = new List<DistanceJoints>();
		for (int i = 0; i < physicsJoints.Count; i++) 
		{
			DistanceJoints dj = physicsJoints[i];
			if (dj.transform.parent == null || dj.transform.parent.GetComponent<SoftBody>() == null) 
			{
				list.Add(dj);
			}
		}

		return list;
	}

}