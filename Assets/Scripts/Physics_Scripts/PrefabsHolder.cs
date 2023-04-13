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
	private GameObject softBody1;

	[SerializeField]
	private GameObject meshCreatorPoint;


	[SerializeField]
	private GameObject waterAddSprite;

	[SerializeField]
	private Material basicUnlitMaterial;
	[SerializeField]
	private Material shadowObjectMaterial;
	[SerializeField]
	private Material selecteObjectdMaterial;

	[SerializeField]
	private Material softBodyShadowMaterial;

	[SerializeField]
	private Material fluidMaterial;

	public GameObject GetLittleCircle() {return GetPrefab(littleCircle);}
	public GameObject GetMiddleCircle() { return GetPrefab(middleCircle); }
	public GameObject GetBigCircle() { return GetPrefab(bigCircle); }
	public GameObject GetMiddleTriangle() { return GetPrefab(middleTriangle); }
	public GameObject GetBigSquare() { return GetPrefab(bigSquare); }

	public GameObject GetSoftBody1() { return GetSoftBody(softBody1); }

	public GameObject GetMeshCreatorPoint() { return Instantiate(meshCreatorPoint); }

	public GameObject GetWaterAddSprite() { return Instantiate(waterAddSprite); }

	public Material GetBasicUnlitMaterial() { return basicUnlitMaterial; }
	public Material GetShadowObjectMaterial() { return shadowObjectMaterial; }

	public Material GetSelectedObjectMaterial() { return selecteObjectdMaterial; }

	public Material GetSoftBodyShadowMaterial() { return softBodyShadowMaterial; }

	public Material GetFluidMaterial() { return fluidMaterial; }

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

	private GameObject GetSoftBody(GameObject softBO) 
	{
		SoftBody sb = softBO.GetComponent<SoftBody>();
		sb.Initialise();
		sb.SetShadowMaterial(this);
		return Instantiate(softBO);
	}
}
