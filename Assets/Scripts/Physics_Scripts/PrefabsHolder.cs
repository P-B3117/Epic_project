using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsHolder : MonoBehaviour
{
	[SerializeField]
	private GameObject littleCircle;
	[SerializeField]
	private GameObject middleCircle;
	[SerializeField]
	private GameObject bigCircle;
	[SerializeField]
	private GameObject middleTriangle;
	[SerializeField]
	private GameObject bigSquare;


	[SerializeField]
	private GameObject meshCreatorPoint;


	public GameObject GetLittleCircle() {return GetPrefab(littleCircle);}
	public GameObject GetMiddleCircle() { return GetPrefab(middleCircle); }
	public GameObject GetBigCircle() { return GetPrefab(bigCircle); }
	public GameObject GetMiddleTriangle() { return GetPrefab(middleTriangle); }
	public GameObject GetBigSquare() { return GetPrefab(bigSquare); }

	public GameObject GetMeshCreatorPoint() { return meshCreatorPoint; }

	private GameObject GetPrefab(GameObject go) 
	{
		GameObject LC = Instantiate(go);
		BasicPhysicObject bp = LC.GetComponent<BasicPhysicObject>();
		MeshColliderScript mc = LC.GetComponent<MeshColliderScript>();
		bp.Initialize();
		mc.SetUpMesh();
		mc.SetShadowMaterial();
		
		return LC;
	}
}
