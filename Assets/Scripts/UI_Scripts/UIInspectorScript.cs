using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

/*
 * Filename : UISliderScript
 * Author(s): Louis, Charles, Justin
 * Goal : Contains all the functionalities of the INSPECTOR (The menu that allows to change parameters of the object) in the Basic Physic Simulation
 * 
 * Requirements : Attach this script to the UIManager
 */
public class UIInspectorScript : MonoBehaviour
{
    [HideInInspector]
    public int SELECTEDOBJECT = -1;
    [HideInInspector]
    public int selectedIndex = -1;
    [HideInInspector]
    public GameObject SELECTEDOBJECTGAMEOBJECT = null;


    [Header("Global references")]
    public PhysicsManager physicsManager;
    public UISliderScript sliderScript;
    public PrefabsHolder prefabHolder;
    public UiGameManager gameManager;

    [Header("UI references")]
    public GameObject InspectorContent;
    public GameObject InspectorSoftContent;
    public GameObject InspectorJointContent;
    public Slider MassSlider;
    public Slider BoucinessSlider;
    public Slider DynamicFrictionSlider;
    public Slider StaticFrictionSlider;
    public Toggle IsStaticToggle;
    public TextMeshProUGUI MassText;
    public TextMeshProUGUI BoucinessText;
    public TextMeshProUGUI DynamicFrictionText;
    public TextMeshProUGUI StaticFrictionText;
    public Slider MassSoftSlider;
    public Slider SoftnessSlider;
    public Slider SizeSlider;
    public Toggle SoftisStaticToggle;
    public TextMeshProUGUI MassSoftText;
    public TextMeshProUGUI SoftnessText;
    public TextMeshProUGUI SizeText;
    public Slider SelectedJointSlider;
    public Slider DistanceSlider;
    public Slider DampingSlider;
    public Slider FrequencySlider;
    public Toggle SelectAllToggle;
    public Button DeleteButton;
    public TextMeshProUGUI SelectedJointText;
    public TextMeshProUGUI DistanceText;
    public TextMeshProUGUI DampingText;
    public TextMeshProUGUI FrequencyText;


    private List<DistanceJoints> jointInspectorReference;
    private int jointInspectorINDEX = -1;
    private bool ALLSELECTED = false;

    public void MouseClickInsideBoundaries(Vector3 worldPoint) 
    {
        
        selectedIndex = (physicsManager.FindClickIndex(worldPoint));
        SELECTEDOBJECT = selectedIndex;
        BasicPhysicObject bo;
        bo = physicsManager.SelectSpecificObject(selectedIndex, prefabHolder, SELECTEDOBJECTGAMEOBJECT);
        
        if (selectedIndex == -1)
        {
            SetOffInspectorContent();
            if (gameManager.jmState)gameManager.JointManager();
            // draggable = false;
        }

        
        //if the object clicked isn't null
        if (bo != null)
        {

            GameObject parent = null;

            if (bo.transform.parent != null) parent = bo.transform.parent.gameObject;

            if (gameManager.singleDelete)
            {
                sliderScript.ResetMouseState();
                //code qui fait en sorte que les objets qui �taient dans la fen�tre disparaissent.
                physicsManager.RemoveAt(bo, parent);

                //enleve la selection de l'objet presentement
                selectedIndex = -1;
                SELECTEDOBJECT = -1;
                InspectorContent.SetActive(false);
                SELECTEDOBJECTGAMEOBJECT = null;
            }

            //If the parent object isn't a soft body, show the regular settings
            if (parent == null || parent.GetComponent<SoftBody>() == null)
            {
                
                SetOnInspectorContent();
                SetOffInspectorSoftContent();
                if (gameManager.jmState) gameManager.JointManager();
               
                SELECTEDOBJECTGAMEOBJECT = bo.gameObject;
				MeshColliderScript mc = bo.GetCollider();
				MassText.text = mc.GetMass().ToString();
				BoucinessText.text = bo.getBounciness().ToString();
				StaticFrictionText.text = bo.getStaticFriction().ToString();
				DynamicFrictionText.text = bo.getDynamicFriction().ToString();
				IsStaticToggle.isOn = bo.getIsStatic();

               
                MassSlider.value = mc.GetMass();

                BoucinessSlider.value = bo.getBounciness();
                StaticFrictionSlider.value = bo.getStaticFriction();
                DynamicFrictionSlider.value = bo.getDynamicFriction();


                gameManager.draggable = true;
                if (gameManager.curseur)
                {

                    GameObject jo;
                    jo = new GameObject();
                    jo.AddComponent<GrabJoint>();
                    GrabJoint joint = jo.GetComponent<GrabJoint>();

                    joint.bo1 = bo.gameObject;
                    joint.frequency = 1.0f;
                    joint.dampingRatio = 0.7f;
                    joint.jointMass = bo.GetCollider().GetMass() / 20;
                    physicsManager.AddGrabJoint(jo);
                }
               
            }
            //If the parent object is a softbody
            else if (parent.GetComponent<SoftBody>() != null)
            {
              
                SetOffInspectorContent();
                SetOnInspectorSoftContent();
                if (gameManager.jmState) gameManager.JointManager();
                //Resets the materials for everyobject
                physicsManager.SelectSpecificObject(-1, prefabHolder, SELECTEDOBJECTGAMEOBJECT);
                SELECTEDOBJECTGAMEOBJECT = parent;
                

                DistanceJoints[] softBodyDJ = parent.GetComponentsInChildren<DistanceJoints>();
                BasicPhysicObject[] softBodyBO = parent.GetComponentsInChildren<BasicPhysicObject>();

                MassSoftText.text = softBodyDJ[0].getFakeMass().ToString();
                SoftnessText.text = softBodyDJ[0].getFakeSoftness().ToString();
                SizeText.text = softBodyDJ[0].getFakeSize().ToString();
                MassSoftSlider.value = softBodyDJ[0].getFakeMass();
                SoftnessSlider.value = softBodyDJ[0].getFakeSoftness();
                SizeSlider.value = softBodyDJ[0].getFakeSize();
                SoftisStaticToggle.isOn = softBodyBO[0].getIsStatic();
                gameManager.draggable = true;
                if (gameManager.curseur && parent.GetComponent<SoftBody>() != null)
                {
                    GameObject jo;
                    jo = new GameObject();
                    jo.AddComponent<GrabJoint>();
                    GrabJoint joint = jo.GetComponent<GrabJoint>();

                    joint.bo1 = softBodyBO[0].gameObject;
                    joint.frequency = 1.0f;
                    joint.dampingRatio = 0.7f;
                    joint.jointMass = bo.GetCollider().GetMass() / 15;

                    physicsManager.AddGrabJoint(jo);
                }
                print(1);
                //Set the material for the softbody
                parent.GetComponent<MeshRenderer>().material = prefabHolder.GetSelectedObjectMaterial();
            }

        }
    }

    public void SetAllOff() 
    {
        SetOffInspectorContent();
        SetOffInspectorJointContent();
        SetOffInspectorSoftContent();
    }
    public void SetOnInspectorContent()
    {
        InspectorContent.SetActive(true);
    }
    public void SetOnInspectorSoftContent()
    {
        InspectorSoftContent.SetActive(true);
    }
    public void SetOffInspectorSoftContent()
    {
        InspectorSoftContent.SetActive(false);
    }
    public void SetOffInspectorContent()
    {
        InspectorContent.SetActive(false);
    }

    public void SetOnInspectorJointContent() { InspectorJointContent.SetActive(true); }

    public void SetOffInspectorJointContent() { InspectorJointContent.SetActive(false); }


    public void InspectorChangeMass(System.Single newMass)
    {
        MassText.text = newMass.ToString();
        MeshColliderScript mc = physicsManager.GetMeshCollliderObjectAt(SELECTEDOBJECT);
        if (mc != null) {  mc.SetMass(newMass); }
    }
    public void InspectorChangeBouciness(System.Single newBouciness)
    {
        BoucinessText.text = newBouciness.ToString();
        BasicPhysicObject bo = physicsManager.GetPhysicObjectAt(SELECTEDOBJECT);
        if (bo != null) { bo.setBouciness(newBouciness); }

    }
    public void InspectorChangeDynamicFriction(System.Single newDynamicFriction)
    {
        DynamicFrictionText.text = newDynamicFriction.ToString();
        BasicPhysicObject bo = physicsManager.GetPhysicObjectAt(SELECTEDOBJECT);
        if (bo != null) { bo.setDynamicFriction(newDynamicFriction); }

    }
    public void InspectorChangeStaticFriction(System.Single newStaticFriction)
    {
        StaticFrictionText.text = newStaticFriction.ToString();
        BasicPhysicObject bo = physicsManager.GetPhysicObjectAt(SELECTEDOBJECT);
        if (bo != null) { bo.setStaticFriction(newStaticFriction); }

    }
    public void InspectorChangeStatic(System.Boolean newStatic)
    {
        BasicPhysicObject bo = physicsManager.GetPhysicObjectAt(SELECTEDOBJECT);
        if (bo != null)
        {
            bo.setIsStatic(newStatic);
            sliderScript.ResetMouseState();
        }
    }
    public void InspectorChangeMassSoft(System.Single newMassSoft)
    {
        DistanceJoints[] softBodyDJ = SELECTEDOBJECTGAMEOBJECT.GetComponentsInChildren<DistanceJoints>();
        BasicPhysicObject[] softBodyBO = SELECTEDOBJECTGAMEOBJECT.GetComponentsInChildren<BasicPhysicObject>();
        MassSoftText.text = newMassSoft.ToString();
        float individualmass = 1;
        float Amass = 1;
        float insideFreq = 1;
        float frequency = 1;
        if (SELECTEDOBJECTGAMEOBJECT.GetComponent<SoftBody>().type == 1)
        {
            switch (newMassSoft)
            {
                case 1: individualmass = 4; frequency = 2.0f; break;
                case 2: individualmass = 8; frequency = 3.0f; break;
                case 3: individualmass = 14; frequency = 3.5f; break;
                case 4: individualmass = 20; frequency = 3.5f; break;
                case 5: individualmass = 26; frequency = 4.5f; break;
                default: individualmass = 4; frequency = 2.5f; break;
            }
            for (int i = 0; i < softBodyBO.Length; i++)
            {
                softBodyBO[i].GetCollider().SetMass(individualmass);
            }
            for (int i = 0; i < softBodyDJ.Length; i++)
            {
                softBodyDJ[i].setFakeMass(newMassSoft);
                softBodyDJ[i].setfrequency(frequency);
            }
        }
        else if (SELECTEDOBJECTGAMEOBJECT.GetComponent<SoftBody>().type == 2)
        {
            switch (newMassSoft)
            {
                case 1: individualmass = 4; frequency = 3.0f; Amass = 12; insideFreq = 4f; break;
                case 2: individualmass = 8; frequency = 4.0f; Amass = 24; insideFreq = 4f; break;
                case 3: individualmass = 14; frequency = 4f; Amass = 42; insideFreq = 5f; break;
                case 4: individualmass = 20; frequency = 4f; Amass = 60; insideFreq = 5f; break;
                case 5: individualmass = 26; frequency = 5f; Amass = 78; insideFreq = 5f; break;
                default: individualmass = 4; frequency = 3f; Amass = 12; insideFreq = 4f; break;

            }
            for (int i = 0; i < softBodyBO.Length; i++)
            {
                softBodyBO[i].GetCollider().SetMass(individualmass);
            }
            for (int i = 0; i < 6; i++)
            {
                softBodyDJ[i].setFakeMass(newMassSoft);
                softBodyDJ[i].setfrequency(insideFreq);
            }
            for (int i = 6; i < softBodyDJ.Length; i++)
            {
                softBodyDJ[i].setFakeMass(newMassSoft);
                softBodyDJ[i].setfrequency(frequency);
            }
            softBodyBO[0].GetCollider().SetMass(Amass);
        }

    }
    public void InspectorChangeSoftness(System.Single newSoftness)
    {
        DistanceJoints[] softBodyDJ = SELECTEDOBJECTGAMEOBJECT.GetComponentsInChildren<DistanceJoints>();
        BasicPhysicObject[] softBodyBO = SELECTEDOBJECTGAMEOBJECT.GetComponentsInChildren<BasicPhysicObject>();
        SoftnessText.text = newSoftness.ToString();

        if (SELECTEDOBJECTGAMEOBJECT.GetComponent<SoftBody>().type == 1)
        {
            for (int i = 0; i < 6; i++)
            {
                softBodyDJ[i].setDampingRatio(newSoftness + 0.1f);
                softBodyDJ[i].setFakeSoftness(newSoftness);
            }
            for (int i = 6; i < softBodyDJ.Length; i++)
            {
                softBodyDJ[i].setDampingRatio(newSoftness);
                softBodyDJ[i].setFakeSoftness(newSoftness);
            }
        }
        else if (SELECTEDOBJECTGAMEOBJECT.GetComponent<SoftBody>().type == 2)
        {

            for (int i = 0; i < 12; i++)
            {
                softBodyDJ[i].setDampingRatio(newSoftness + 0.2f);
                softBodyDJ[i].setFakeSoftness(newSoftness);
            }
            for (int i = 6; i < softBodyDJ.Length; i++)
            {
                softBodyDJ[i].setDampingRatio(newSoftness + 0.1f);
                softBodyDJ[i].setFakeSoftness(newSoftness);
            }
        }
    }
    public void InspectorChangeSize(System.Single newSize)
    {

        DistanceJoints[] softBodyDJ = SELECTEDOBJECTGAMEOBJECT.GetComponentsInChildren<DistanceJoints>();
        BasicPhysicObject[] softBodyBO = SELECTEDOBJECTGAMEOBJECT.GetComponentsInChildren<BasicPhysicObject>();
        SizeText.text = newSize.ToString();
        float radius = 0;
        float length = 0;
        float softsize = 0;
        if (SELECTEDOBJECTGAMEOBJECT.GetComponent<SoftBody>().type == 1)
        {
            switch (newSize)
            {
                case 1: length = 4.0f; radius = 1.0f; softsize = 1.2f; break;
                case 2: length = 6.0f; radius = 2.5f; softsize = 2.7f; break;
                case 3: length = 9.0f; radius = 3.5f; softsize = 3.7f; break;
            }
            for (int i = 0; i < softBodyDJ.Length; i++)
            {
                softBodyDJ[i].setFakeSize(newSize);
                softBodyDJ[i].setlength(length);
            }
            for (int i = 0; i < softBodyBO.Length; i++)
            {
                softBodyBO[i].GetCollider().setRadius(radius);
            }
            SELECTEDOBJECTGAMEOBJECT.GetComponent<SoftBody>().size = softsize;
        }
    }
    public void InspectorChangeStaticSoft(System.Boolean newStaticSoft)
    {
        DistanceJoints[] softBodyDJ = SELECTEDOBJECTGAMEOBJECT.GetComponentsInChildren<DistanceJoints>();
        BasicPhysicObject[] softBodyBO = SELECTEDOBJECTGAMEOBJECT.GetComponentsInChildren<BasicPhysicObject>();
        for (int i = 0; i < softBodyBO.Length; i++)
        {
            softBodyBO[i].setIsStatic(newStaticSoft);
        }

    }

    public void InspectorSelectedJoint(System.Single NewJoint)
    {
        SelectedJointText.text = (NewJoint).ToString();
        jointInspectorINDEX = (int) (NewJoint-1);
        SelectSpecificJoint(jointInspectorINDEX);

        DistanceSlider.value = jointInspectorReference[jointInspectorINDEX].length;
        DampingSlider.value = jointInspectorReference[jointInspectorINDEX].dampingRatio;
        FrequencySlider.value = jointInspectorReference[jointInspectorINDEX].frequency;


    }

    public void InspectorChangeDistance(System.Single NewDistance)
    {
        if (ALLSELECTED)
        {
            for (int i = 0; i < jointInspectorReference.Count; i++) 
            {
                DistanceText.text = NewDistance.ToString();
                jointInspectorReference[i].setlength(NewDistance);
            }
        }
		else 
        {
            DistanceText.text = NewDistance.ToString();
            jointInspectorReference[jointInspectorINDEX].setlength(NewDistance);
        }
            
    }
    public void InspectorChangeDamping(System.Single NewDamping)
    {
        if (ALLSELECTED)
        {
            for (int i = 0; i < jointInspectorReference.Count; i++)
            {
                DampingText.text = NewDamping.ToString();
                jointInspectorReference[i].setdampingRatio(NewDamping);
            }

        }
        else 
        {
            DampingText.text = NewDamping.ToString();
            jointInspectorReference[jointInspectorINDEX].setdampingRatio(NewDamping);
        }
        
    }
    public void InspectorChangeFrequency(System.Single NewFrequency) 
    {
        if (ALLSELECTED)
        {
            for (int i = 0; i < jointInspectorReference.Count; i++) 
            {
                FrequencyText.text = NewFrequency.ToString();
                jointInspectorReference[i].setfrequency(NewFrequency);
            }
        }
        else 
        {
            FrequencyText.text = NewFrequency.ToString();
            jointInspectorReference[jointInspectorINDEX].setfrequency(NewFrequency);
        }
        
    }
    public void InspectorSelectAllTogggle(System.Boolean newSelectAll) 
    {
        if (newSelectAll == true)
        {
            ALLSELECTED = true;
            SelectAllJoints();
        }
        else 
        {
            ALLSELECTED = false;
            SelectSpecificJoint(jointInspectorINDEX);
        }
    }

    public void InspectorDeleteClick() 
    {
        
        if (ALLSELECTED)
        {
            for (int i = 0; i < jointInspectorReference.Count; i++) 
            {
                physicsManager.RemoveJoint(jointInspectorReference[i]);
                Destroy(jointInspectorReference[i].gameObject);
            }
            gameManager.JointManager();
        }
        else 
        {
            physicsManager.RemoveJoint(jointInspectorReference[jointInspectorINDEX]);
            Destroy(jointInspectorReference[jointInspectorINDEX].gameObject);
            jointInspectorReference = physicsManager.GetAllNonSoftBodyJoints();
            if (jointInspectorReference.Count <= 0)
            {
                gameManager.JointManager();
            }
            else 
            {
                JointInspectorInitialize(jointInspectorReference);
            }

        }
    }

    public void JointInspectorInitialize(List<DistanceJoints> joints) 
    {
        jointInspectorReference = joints;
        SelectedJointSlider.maxValue = jointInspectorReference.Count;
        SelectedJointSlider.value = 1;
        InspectorSelectedJoint(1);
    }

    private void SelectSpecificJoint(int index) 
    {
        for (int i = 0; i < index; i++) 
        {
            jointInspectorReference[i].lr.SetColors(Color.magenta, Color.magenta);
            jointInspectorReference[i].lr.material = new Material(Shader.Find("Sprites/Default"));
        }
        
        jointInspectorReference[index].lr.SetColors(Color.white, Color.white);
        jointInspectorReference[index].lr.material = new Material(Shader.Find("Sprites/Default"));
        for (int i = index + 1; i < jointInspectorReference.Count; i++) 
        {
            jointInspectorReference[i].lr.SetColors(Color.magenta, Color.magenta);
            jointInspectorReference[i].lr.material = new Material(Shader.Find("Sprites/Default"));
        }
    }
    private void SelectAllJoints() 
    {
        for (int i = 0; i < jointInspectorReference.Count; i++)
        {
            jointInspectorReference[i].lr.SetColors(Color.white, Color.white);
            jointInspectorReference[i].lr.material = new Material(Shader.Find("Sprites/Default"));
        }
    }
    public void DeselectAll() 
    {
        for (int i = 0; i < jointInspectorReference.Count; i++)
        {
            jointInspectorReference[i].lr.SetColors(Color.magenta, Color.magenta);
            jointInspectorReference[i].lr.material = new Material(Shader.Find("Sprites/Default"));
        }
        SelectAllToggle.isOn = false;
        ALLSELECTED = false;
    }
}
