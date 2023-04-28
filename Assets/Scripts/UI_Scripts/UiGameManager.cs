using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class UiGameManager : MonoBehaviour
{
    [Header("Reference for UI basic functions")]
    public GameObject GamePanel;
    public GameObject PausePanel;
    public GameObject PhysicsPanel;
    public GameObject SettingsPanel;
    public GameObject ToolPanel;
    public GameObject ScrollViewPanel;
    public GameObject SettingPanel;
    public GameObject InspectorPanel;
    public GameObject GameviewPanel;
    public GameObject FullscreenToggle;
    public GameObject MusicSlider;
    public GameObject SoundEffectSlider;
    public GameObject GravityInputField;
    public GameObject AirDensityInputField;
    public GameObject BouncinessInputField;
    public GameObject SFrictionInputField;
    public GameObject DFrictionInputField;
    public GameObject TimeInputField;
    public AudioSource MusicSource;
    public TextMeshProUGUI MusicVolumeText;
    public TextMeshProUGUI SoundEffectVolumeText;
    public PrefabsHolder prefabHolder;
    public PhysicsManager physicsManager;
    public GameObject jmButton;
    private float jointIndex;
    // Start is called before the first frame update



    //Slider functions variables
    private int MOUSESTATE;
    List<GameObject> meshCreatorPoints;
    List<GameObject> JointCreatorPoints;
    List<GameObject> JointReference;
    List<DistanceJoints> physicsJoints;
    [Header("Reference for the slider")]
    public LineRenderer meshCreatorLineRenderer;
    public LineRenderer JointCreatorLineRenderer;
    GameObject currentShadowObject;
    GameObject jo;
    //Inspector variables
    [Header("Reference for the inspector")]
    private int SELECTEDOBJECT = -1;
    private GameObject SELECTEDOBJECTGAMEOBJECT = null;
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
    public GameObject SelectedJointSlider;
    public GameObject DistanceSlider;
    public GameObject DampingSlider;
    public GameObject FrequencySlider;
    public GameObject SelectAllToggle;
    public GameObject DeleteButton;
    public TextMeshProUGUI SelectedJointText;
    public TextMeshProUGUI DistanceText;
    public TextMeshProUGUI DampingText;
    public TextMeshProUGUI FrequencyText;
    
    private bool draggable = false;
    private int selectedIndex;
    private BasicPhysicObject bo;
    private GameObject parent;
    private int counter = 0, counter2 = 0;
    DistanceJoints joint;
    private Vector3 lastPosition;
    private Color buttonNormalColor = new Color(1f, 1f, 1f, 1f);
    private Color buttonPressedColor = new Color(0.04705882f, 1f, 0f, 1f);

    void Start()
    {
        ShowGamePanel();
        Screen.SetResolution(1920, 1080, GameConstants.Fullscreen, 60); //int width, int height, bool fullscreen, int preferredRefreshRate (0 = unlimited)
        FullscreenToggle.GetComponent<Toggle>().isOn = GameConstants.Fullscreen;
        MusicSlider.GetComponent<Slider>().value = GameConstants.MusicVolume;
        SoundEffectSlider.GetComponent<Slider>().value = GameConstants.SoundEffectVolume;
        GravityInputField.GetComponent<TMP_InputField>().text = "9.8";
        AirDensityInputField.GetComponent<TMP_InputField>().text = "1";
        TimeInputField.GetComponent<TMP_InputField>().text = "1";
        BouncinessInputField.GetComponent<TMP_InputField>().text = "1";
        SFrictionInputField.GetComponent<TMP_InputField>().text = "1";
        DFrictionInputField.GetComponent<TMP_InputField>().text = "1";

        //Initialize the slider function variables
        MOUSESTATE = -1;
        meshCreatorPoints = new List<GameObject>();
        JointCreatorPoints = new List<GameObject>();
        JointReference = new List<GameObject>();
        physicsJoints = new List<DistanceJoints>();
        physicsJoints = physicsManager.Joints();
    }

    void Update()
    {
       
        //Universal inputs functionality
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!PausePanel.activeSelf) ShowPausePanel();
            else ShowGamePanel();
        }
        if (Input.GetMouseButtonDown(1)) 
        {
            ResetMouseState();
        }





        //!!!!!Functionality of the slider!!!!!
        //Mouse functionality for basic objects
        //Move the shadow object of the basic physics object selected with the mouse
        if (MOUSESTATE >= 0 && MOUSESTATE <= 4)
        {

            //Get the position of the mouse on the screen
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentShadowObject.transform.position = new Vector3(worldPoint.x, worldPoint.y, 0.0f);
            //The worldPoint boundaries are : (-28.25, 20)      - (28.25, 20)
            //                                (-28.25, -20)   - (28.25, -20)

            //Check boundaries
            MeshColliderScript mc = currentShadowObject.GetComponent<MeshColliderScript>();
            mc.UpdateColliderOrientation();
            Rect AABB = mc.GetBoundariesAABB();
            if (AABB.position.x > -28.25f && AABB.position.x + AABB.width < 28.25 &&
                AABB.position.y > -20 && AABB.position.y + AABB.height < 20)
            {
                //If the mouse is in bound, change the color to green and if the mouse is clicked, initialize the new object
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject newGO = Instantiate(currentShadowObject);
                    physicsManager.AddPhysicObject(newGO);
                }
            }
            //If the mouse isn't in bound, change the material color to red
            else
            {
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);

            }




        }
        //Mouse functinality for the Mesh creator
        //Allows player to add points on the game scene to create custom mesh
        else if (MOUSESTATE == 5)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentShadowObject.transform.position = new Vector3(worldPoint.x, worldPoint.y, 0.0f);

            //move the last point of line renderer based on the mouse
            if (meshCreatorPoints.Count > 0)
            {
                meshCreatorLineRenderer.SetPosition(meshCreatorPoints.Count, currentShadowObject.transform.position);
            }

            float radius = 0.5f;
            Rect AABB = new Rect(new Vector2(worldPoint.x - 0.5f, worldPoint.y - 0.5f), new Vector2(radius * 2, radius * 2));


            //Look if angle is good (smaller than 180)
            float angle = 0;

            if (meshCreatorPoints.Count >= 3)
            {
                Vector3 p0 = meshCreatorPoints[meshCreatorPoints.Count - 3].transform.position;
                Vector3 p1 = meshCreatorPoints[meshCreatorPoints.Count - 2].transform.position;
                Vector3 p2 = meshCreatorPoints[meshCreatorPoints.Count - 1].transform.position;
                Vector3 p3 = currentShadowObject.transform.position;

                Vector3 v1 = p1 - p0;
                Vector3 v2 = p2 - p1;
                Vector3 v3 = p3 - p2;

                Vector3 r1 = Vector3.Cross(v1, v2).normalized;
                Vector3 r2 = Vector3.Cross(v2, v3).normalized;

                Vector3 pBegin = meshCreatorPoints[0].transform.position;
                Vector3 pBegin2 = meshCreatorPoints[1].transform.position;

                Vector3 v4 = pBegin - p3;
                Vector3 v5 = pBegin2 - pBegin;

                Vector3 r3 = Vector3.Cross(v3, v4).normalized;
                Vector3 r4 = Vector3.Cross(v4, v5).normalized;

                if (!(r1 == r2 && r2 == r3 && r3 == r4)) { angle = 360; }


            }

            bool noLineIntersect = true;
            //Look if line intersect 
            if (meshCreatorPoints.Count >= 3)
            {
                Vector3 m1 = meshCreatorPoints[meshCreatorPoints.Count - 1].transform.position;
                Vector3 m2 = currentShadowObject.transform.position;


                for (int i = 0; i < meshCreatorPoints.Count - 2; i++)
                {
                    Vector3 m3 = meshCreatorPoints[i].transform.position;
                    Vector3 m4 = meshCreatorPoints[i + 1].transform.position;

                    if (HelperFunctionClass.LineIntersect(m1, m2, m3, m4))
                    {

                        noLineIntersect = false;
                        break;
                    }


                }

            }

            //Check if there is colinearity in the wrong direction with the other segment
            if (meshCreatorPoints.Count >= 2)
            {
                Vector3 m1 = meshCreatorPoints[meshCreatorPoints.Count - 1].transform.position;
                Vector3 m2 = currentShadowObject.transform.position;
                Vector3 v1 = m2 - m1;
                Vector3 m5 = meshCreatorPoints[meshCreatorPoints.Count - 2].transform.position;
                Vector3 m6 = meshCreatorPoints[meshCreatorPoints.Count - 1].transform.position;
                Vector3 v2 = m6 - m5;
                if (Vector3.Dot(v1.normalized, v2.normalized) == -1)
                {
                    noLineIntersect = false;
                }
            }

            //Look if a point isn't already there
            bool noPointAlreadyThere = true;
            if (meshCreatorPoints.Count > 1)
            {
                for (int i = 1; i < meshCreatorPoints.Count; i++)
                {
                    if (meshCreatorPoints[i].transform.position == currentShadowObject.transform.position)
                    {
                        noPointAlreadyThere = false;
                        break;
                    }
                }
            }
            else if (meshCreatorPoints.Count > 0)
            {
                if (meshCreatorPoints[0].transform.position == currentShadowObject.transform.position)
                {
                    noPointAlreadyThere = false;
                }
            }


            ///Mesh Creators Conditions
            bool isPoint = false;
            Vector3 initialPoint = Vector3.zero;
            if (meshCreatorPoints.Count > 0) { isPoint = true; initialPoint = meshCreatorPoints[0].transform.position; }

            //If the current point is close to the initial point
            if (meshCreatorPoints.Count >= 3 && isPoint && (currentShadowObject.transform.position - initialPoint).magnitude < 2)
            {
                currentShadowObject.transform.position = initialPoint;
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
                meshCreatorLineRenderer.SetColors(Color.cyan, Color.cyan);
                meshCreatorLineRenderer.SetPosition(meshCreatorPoints.Count, initialPoint);

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 moy = Vector3.zero;
                    for (int i = 0; i < meshCreatorPoints.Count; i++)
                    {
                        moy += meshCreatorPoints[i].transform.position;
                    }
                    moy /= meshCreatorPoints.Count;
                    List<Vector3> modelPoints = new List<Vector3>();


                    Vector3 first = meshCreatorPoints[0].transform.position - moy;
                    Vector3 second = meshCreatorPoints[1].transform.position - moy;
                    Vector3 dir = Vector3.Cross(first, second).normalized;
                    if (dir == Vector3.back)
                    {
                        for (int i = 0; i < meshCreatorPoints.Count; i++)
                        {
                            modelPoints.Add(meshCreatorPoints[i].transform.position - moy);
                        }
                    }
                    else
                    {
                        for (int i = meshCreatorPoints.Count - 1; i >= 0; i--)
                        {
                            modelPoints.Add(meshCreatorPoints[i].transform.position - moy);
                        }
                    }



                    GameObject empty = new GameObject();
                    empty.transform.position = moy;
                    empty.AddComponent<BasicPhysicObject>();
                    empty.AddComponent<MeshColliderScript>();
                    MeshColliderScript mc = empty.GetComponent<MeshColliderScript>();
                    mc.Initialize(modelPoints, prefabHolder);
                    mc.SetUpMesh();
                    mc.UpdateColliderOrientation();
                    GameObject newGO = Instantiate(empty);
                    Destroy(empty);
                    physicsManager.AddPhysicObject(newGO);
                    ResetMouseState();
                }
            }
            //If the current point isn't close the initial point
            else
            {
                //If the current segment is valid, instantiate it
                if (AABB.position.x > -28.25f && AABB.position.x + AABB.width < 28.25 &&
                    AABB.position.y > -20 && AABB.position.y + AABB.height < 20 &&
                    angle < 180 && noLineIntersect && noPointAlreadyThere)
                {
                    currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
                    meshCreatorLineRenderer.SetColors(Color.magenta, Color.magenta);
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Instantiate the mesh creator point
                        GameObject newGO = Instantiate(currentShadowObject);
                        meshCreatorPoints.Add(newGO);


                        //Update the line renderer
                        meshCreatorLineRenderer.positionCount++;
                        Vector3[] points = new Vector3[meshCreatorPoints.Count + 1];
                        for (int i = 0; i < meshCreatorPoints.Count; i++)
                        {
                            points[i] = meshCreatorPoints[i].transform.position;
                        }
                        points[meshCreatorPoints.Count] = currentShadowObject.transform.position;
                        meshCreatorLineRenderer.SetPositions(points);

                    }
                }
                //If the current segment is invalid
                else
                {
                    currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    meshCreatorLineRenderer.SetColors(Color.red, Color.red);
                }
            }



        }
        else if (MOUSESTATE == 6)
        {


            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPoint = new Vector3(worldPoint.x, worldPoint.y, 0.0f);
            currentShadowObject.transform.position = worldPoint;
            //update the line thing 
            if (JointCreatorPoints.Count == 1)
            {
                JointCreatorPoints[0].transform.position = JointReference[0].transform.position;
                Vector3[] position = new Vector3[] { JointCreatorPoints[0].transform.position, currentShadowObject.transform.position };
                JointCreatorLineRenderer.SetPositions(position);
            }
         
         
            if ( counter ==2)
            {
                //create joint here 
                jo = new GameObject();
                jo.AddComponent<DistanceJoints>();
                DistanceJoints joint = jo.GetComponent<DistanceJoints>();
                
                joint.bo1 = JointReference[0];
                joint.bo2 = JointReference[1];
                joint.frequency = 1.0f;
                joint.dampingRatio = 0.5f;
                joint.length = (JointReference[1].transform.position - JointReference[0].transform.position).magnitude;
                physicsManager.AddDistanceJoints(jo);

                JointReference.Remove(JointReference[1]);
                JointReference.Remove(JointReference[0]);
                JointCreatorLineRenderer.SetPosition(0, Vector3.zero);
                JointCreatorLineRenderer.SetPosition(1, Vector3.zero);

                counter = 0;
                ResetMouseState();
            }
            else if (Input.GetMouseButtonDown(0) && JointCreatorPoints.Count < 2)
            {
                
                //Check boundaries when click
                if (worldPoint.x > -28.25f && worldPoint.x < 28.25 && worldPoint.y > -20 && worldPoint.y < 20)
                {

                    //Check if mouse click was in a specific object
                    //return the index of the object if there is one
                    //return -1 of there is no object
                    selectedIndex = (physicsManager.FindClickIndex(worldPoint));

                    
                    SELECTEDOBJECT = selectedIndex;
                    if (selectedIndex == -1)
                    {
                        SetOffInspectorContent();
                        if (jmState) JointManager();
                    }
                    bo = physicsManager.SelectSpecificObject(selectedIndex, prefabHolder, SELECTEDOBJECTGAMEOBJECT);
                    
                    if (bo != null)
                    {
                        bool cond = true;
                        if (counter == 1 && JointReference[0] == bo.gameObject) cond = false;

                        if (cond == true)
                        {
                            currentShadowObject.transform.position = bo.transform.position;
                            JointReference.Add(bo.gameObject);
                            //Instantiate the mesh creator point
                            GameObject newGO = Instantiate(currentShadowObject);
                            JointCreatorPoints.Add(newGO);
                            //Update the line renderer
                            JointCreatorLineRenderer.positionCount++;
                            counter++;
                            cond = true;
                        }

                    }

                }
            }
        }
        //Mouse functionality for the softbody
        //Allows user to add softbodies to the scene
        else if (MOUSESTATE == 7)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //The worldPoint boundaries are : (-28.25, 20)      - (28.25, 20)
            //                                (-28.25, -20)   - (28.25, -20)
            //Check boundaries


            currentShadowObject.transform.position = new Vector3(worldPoint.x / 2, worldPoint.y / 2, 0.0f); //I have literally no clue why i have to do this??? But it works !
            if (worldPoint.x > -28.25f && worldPoint.x < 28.25 &&
                worldPoint.y > -20 && worldPoint.y < 20)
            {
                //If the mouse is in boundaries, change color to good color and if moused clicked add softbody to scene
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
                if (Input.GetMouseButtonDown(0))
                {

                    currentShadowObject.transform.position = Vector3.zero;
                    GameObject newGO = Instantiate(currentShadowObject);
                    newGO.GetComponent<SoftBody>().SetBasicMaterial();
                    BasicPhysicObject[] bos = newGO.GetComponentsInChildren<BasicPhysicObject>();
                    for (int i = 0; i < bos.Length; i++)
                    {
                        bos[i].transform.position += new Vector3(worldPoint.x, worldPoint.y, 0.0f);
                    }



                    physicsManager.AddSoftBody(newGO);
                }
            }
            //If mouse is out of the boundaries, change color to red
            else
            {
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);

            }


        }
        else if (MOUSESTATE == 8)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //The worldPoint boundaries are : (-28.25, 20)      - (28.25, 20)
            //                                (-28.25, -20)   - (28.25, -20)
            //Check boundaries


            currentShadowObject.transform.position = new Vector3(worldPoint.x / 2, worldPoint.y / 2, 0.0f); //I have literally no clue why i have to do this??? But it works !
            if (worldPoint.x > -28.25f && worldPoint.x < 28.25 &&
                worldPoint.y > -20 && worldPoint.y < 20)
            {
                //If the mouse is in boundaries, change color to good color and if moused clicked add softbody to scene
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
                if (Input.GetMouseButtonDown(0))
                {

                    currentShadowObject.transform.position = Vector3.zero;
                    GameObject newGO = Instantiate(currentShadowObject);
                    newGO.GetComponent<SoftBody>().SetBasicMaterial();
                    BasicPhysicObject[] bos = newGO.GetComponentsInChildren<BasicPhysicObject>();
                    for (int i = 0; i < bos.Length; i++)
                    {
                        bos[i].transform.position += new Vector3(worldPoint.x, worldPoint.y, 0.0f);
                    }



                    physicsManager.AddSoftBody(newGO);
                }
            }
            //If mouse is out of the boundaries, change color to red
            else
            {
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);

            }


        }
        //Mouse functionality for object selection
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPoint = new Vector3(worldPoint.x, worldPoint.y, 0.0f);

                //Check boundaries when click
                if (worldPoint.x > -28.25f && worldPoint.x < 28.25 && worldPoint.y > -20 && worldPoint.y < 20)
                {
                    //Check if mouse click was in a specific object
                    //return the index of the object if there is one
                    //return -1 of there is no object
                    selectedIndex = (physicsManager.FindClickIndex(worldPoint));
                    SELECTEDOBJECT = selectedIndex;
                    if (selectedIndex == -1)
                    {
                        SetOffInspectorContent();
                        if (jmState) JointManager();
                        // draggable = false;
                    }
                   

                    bo = physicsManager.SelectSpecificObject(selectedIndex, prefabHolder, SELECTEDOBJECTGAMEOBJECT);

                    if (bo != null)
                    {

                        GameObject parent = null;

                        if (bo.transform.parent != null) parent = bo.transform.parent.gameObject;

                        if (singleDelete)
                        {
                            //ne marche pas live
                            ResetMouseState();
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
                            if (jmState) JointManager();
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


                            draggable = true;
                            if (curseur)
                            {
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
                            if (jmState) JointManager();
                            //Resets the materials for everyobject
                            physicsManager.SelectSpecificObject(-1, prefabHolder, SELECTEDOBJECTGAMEOBJECT);
                            SELECTEDOBJECTGAMEOBJECT = parent;
                            //Set the material for the softbody
                            parent.GetComponent<MeshRenderer>().material = prefabHolder.GetSelectedObjectMaterial();

                            DistanceJoints[] softBodyDJ = parent.GetComponentsInChildren<DistanceJoints>();
                            BasicPhysicObject[] softBodyBO = parent.GetComponentsInChildren<BasicPhysicObject>();

                            MassSoftText.text = softBodyDJ[0].getFakeMass().ToString();
                            SoftnessText.text = softBodyDJ[0].getFakeSoftness().ToString();
                            SizeText.text = softBodyDJ[0].getFakeSize().ToString();
                            MassSoftSlider.value = softBodyDJ[0].getFakeMass();
                            SoftnessSlider.value = softBodyDJ[0].getFakeSoftness();
                            SizeSlider.value = softBodyDJ[0].getFakeSize();
                            SoftisStaticToggle.isOn = softBodyBO[0].getIsStatic();
                            draggable = true;
                            if (curseur && parent.GetComponent<SoftBody>() != null)
                            {
                                jo = new GameObject();
                                jo.AddComponent<GrabJoint>();
                                GrabJoint joint = jo.GetComponent<GrabJoint>();

                                joint.bo1 = softBodyBO[0].gameObject;
                                joint.frequency = 1.0f;
                                joint.dampingRatio = 0.7f;
                                joint.jointMass = bo.GetCollider().GetMass()/15;

                                physicsManager.AddGrabJoint(jo);
                            }
                        }

                    }

                }

            }

            else if (Input.GetMouseButtonUp(0))
            {
                physicsManager.ResetGrabJoint();
            }
        }
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bo = physicsManager.SelectSpecificObject(selectedIndex, prefabHolder, SELECTEDOBJECTGAMEOBJECT);
        parent = null;

        if (!singleDelete && bo != null && bo.transform.parent != null) parent = bo.transform.parent.gameObject;
        if (draggable && pause && mousePosition.x > -28.25f && mousePosition.x < 28.25 && mousePosition.y > -20 && mousePosition.y < 20 && bo != null)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (lastPosition == Vector3.zero) lastPosition = mousePosition;
            if (parent != null && parent.GetComponent<SoftBody>() != null)
            {
                Vector3 deltaPosition = mousePosition - lastPosition;
                parent.GetComponent<SoftBody>().Move(deltaPosition);
            }
            else
            {
                bo.gameObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
            }
            lastPosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
        }
        if (Input.GetMouseButton(1))
        {
            draggable = false;
            selectedIndex = -1;
            SetOffInspectorContent();
            SetOffInspectorSoftContent();
            lastPosition = Vector3.zero;
        }


    }

    public void ChangeTime(string time)
    {
        UniversalVariable.SetTime(float.Parse(time));
    }

    public void ChangeGravity(string gravity)
    {
        UniversalVariable.SetGravity(float.Parse(gravity));
    }

    public void ChangeAirDensity(string AirDensity)
    {
        UniversalVariable.SetAirDensity(float.Parse(AirDensity));
    }

    public void ChangeToDefault()
    {
        GravityInputField.GetComponent<TMP_InputField>().text = "9.8";
        AirDensityInputField.GetComponent<TMP_InputField>().text = "1";
        TimeInputField.GetComponent<TMP_InputField>().text = "1";
        BouncinessInputField.GetComponent<TMP_InputField>().text = "1";
        SFrictionInputField.GetComponent<TMP_InputField>().text = "1";
        DFrictionInputField.GetComponent<TMP_InputField>().text = "1";
        UniversalVariable.SetGravity(float.Parse("9,8"));
        UniversalVariable.SetTime(float.Parse("1"));
        UniversalVariable.SetAirDensity(float.Parse("1"));
        UniversalVariable.SetBounciness(float.Parse("1"));
        UniversalVariable.SetSFriction(float.Parse("1"));
        UniversalVariable.SetDFriction(float.Parse("1"));
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(1);
    }

    private bool curseur = false, singleDelete = false, forces = false, twoX = false, threeX = false, pause = false, jmState = false;

    // bouton curseur (1)
    public void DeplacerObjets()
    {
        ResetMouseState();

        if (jmState) { JointManager(); }
        if (singleDelete) { SingleDelete(); }

        if (curseur && !singleDelete)
        {
            GameObject.Find("Canvas/ToolPanel/BoutonCurseur").GetComponent<Image>().color = buttonNormalColor;
            curseur = !curseur;
        }

        else if (!singleDelete)
        {
            GameObject boutonCurseur = GameObject.Find("Canvas/ToolPanel/BoutonCurseur");
            boutonCurseur.GetComponent<Image>().color = buttonPressedColor;

            // code ici qui fait en sorte qu'on peut d�placer des objets
            curseur = !curseur;
        }
        // une fois qu'il se fait click, faire en sorte que le bouton reste "pes�" jusqu'� ce qu'il soit reclicked

        
    }

    // bouton SingleDelete (2)
    public void SingleDelete()
    {
        ResetMouseState();

        if (jmState) { JointManager(); }
        if (curseur) { DeplacerObjets(); }

        // une fois qu'il se fait click, faire en sorte que le bouton reste "pes�" jusqu'� ce qu'il soit reclicked on
        if (singleDelete && !curseur)
        {
            GameObject.Find("Canvas/ToolPanel/BoutonSingleDelete").GetComponent<Image>().color = buttonNormalColor;
            singleDelete = !singleDelete;
        }

        else if (!curseur)
        {
            GameObject.Find("Canvas/ToolPanel/BoutonSingleDelete").GetComponent<Image>().color = buttonPressedColor;
            singleDelete = !singleDelete;
            // faire en sorte qu'on peut tourner des objets ici => rendu single delete
        }

        
    }

    private float gravityBefore, AirDensityBefore;
    // bouton F (3)
    public void AppliquerForcesDePhysique()
    {
        ResetMouseState();
        // code qui fait en sorte que les forces de physique seront appliqu�es sur les objets
        if (forces)
        {
            GameObject.Find("Canvas/ToolPanel/BoutonF").GetComponent<Image>().color = buttonNormalColor;
            if (UniversalVariable.GetGravity() == 0) { UniversalVariable.SetGravity(gravityBefore); } 
            if (UniversalVariable.GetAirDensity() == 0) { UniversalVariable.SetAirDensity(AirDensityBefore); }
        }

        else
        {
            GameObject.Find("Canvas/ToolPanel/BoutonF").GetComponent<Image>().color = buttonPressedColor;
            // une fois qu'il se fait click, faire en sorte que le bouton reste "pes�" jusqu'� ce qu'il soit reclicked on
            gravityBefore = UniversalVariable.GetGravity();
            AirDensityBefore = UniversalVariable.GetAirDensity();
            UniversalVariable.SetGravity(0);
            UniversalVariable.SetAirDensity(0);
        }
        forces = !forces;
    }

    // bouton poubelle (4)
    public void ResetFenetre()
    {
        ResetMouseState();
        // code qui fait en sorte que les objets qui �taient dans la fen�tre disparaissent.
        SoftBody[] softBodies = FindObjectsOfType<SoftBody>();
        for (int i = softBodies.Length-1; i >= 0; i--) 
        {
            Destroy(softBodies[i].gameObject);
        }
        physicsManager.ResetList();

        //enleve la selection de l'objet presentement
        SELECTEDOBJECT = -1;
        InspectorContent.SetActive(false);
        SELECTEDOBJECTGAMEOBJECT = null;
    }

    public void ResetMouseState() {
        MOUSESTATE = -1;
        Destroy(currentShadowObject);
        currentShadowObject = null;
        for (int i = meshCreatorPoints.Count - 1; i >= 0; i--)
        {
            Destroy(meshCreatorPoints[i]);
        }
        for (int i = JointCreatorPoints.Count - 1; i >= 0; i--)
        {
            Destroy(JointCreatorPoints[i]);
        }
        for (int i = JointReference.Count - 1; i >= 0; i--)
        {
            JointReference.RemoveAt(i);
        }
        meshCreatorPoints = new List<GameObject>();
        JointCreatorPoints = new List<GameObject>();
        JointReference = new List<GameObject>();
        meshCreatorLineRenderer.positionCount = 1;
        JointCreatorLineRenderer.positionCount = 1;
        counter = 0;

    }
    
    public void SetMouseState0() { ResetMouseState(); MOUSESTATE = 0; currentShadowObject = prefabHolder.GetLittleCircle();  currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState1() { ResetMouseState(); MOUSESTATE = 1; currentShadowObject = prefabHolder.GetMiddleCircle(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState2() { ResetMouseState(); MOUSESTATE = 2; currentShadowObject = prefabHolder.GetBigCircle(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState3() { ResetMouseState(); MOUSESTATE = 3; currentShadowObject = prefabHolder.GetMiddleTriangle(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState4() { ResetMouseState(); MOUSESTATE = 4; currentShadowObject = prefabHolder.GetBigSquare(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState5() 
    { 
        ResetMouseState(); 
        MOUSESTATE = 5; 
        currentShadowObject = prefabHolder.GetMeshCreatorPoint(); 
        currentShadowObject.transform.SetParent(GamePanel.transform);  
    }
    public void SetMouseState6()
    {
        ResetMouseState();
        MOUSESTATE = 6;
        currentShadowObject = prefabHolder.GetMeshCreatorPoint(); 
        currentShadowObject.transform.SetParent(GamePanel.transform);
    }
    public void SetMouseState7()
    {
        ResetMouseState();
        MOUSESTATE = 7;
        currentShadowObject = prefabHolder.GetSoftBody1();
		currentShadowObject.transform.SetParent(GamePanel.transform);
	}
    public void SetMouseState8()
    {
        ResetMouseState();
        MOUSESTATE = 8;
        currentShadowObject = prefabHolder.GetSoftBody2();
        currentShadowObject.transform.SetParent(GamePanel.transform);
    }
    // bouton pause (5)
    public void PauseScene()
    {
        ResetMouseState();
        // make time frames/calculations equal to 0
        ChangeTime("0");
        GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = buttonNormalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = buttonNormalColor;
        pause = true;
    }
    
    // bouton play (6)
    public void PlayScene()
    {
        ResetMouseState();
        // make time frames/calculations start
        ChangeTime("1");
        GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = buttonNormalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = buttonNormalColor;
        if (pause) pause = false;
    }

    // bouton 2x (7)
    public void TwoXFaster()
    {
        ResetMouseState();
        // une fois qu'il se fait click, faire en sorte que le bouton reste "pes�" jusqu'� ce qu'il soit reclicked on
        if (pause) pause = false;
        if (twoX && !threeX)
        {
            GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = buttonNormalColor;
            ChangeTime("1");
            twoX = !twoX;
        }

        else// if (!threeX)
        {
            GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = buttonPressedColor;
            GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = buttonNormalColor;
            // make time frames/calculations go twice as fast/second
            ChangeTime("2");
            
            twoX = !twoX;
            threeX = !threeX;
        }
        
    }

    // bouton 3x (8)
    public void ThreeXFaster()
    {
        ResetMouseState();
        if (pause) pause = false;
        // une fois qu'il se fait click, faire en sorte que le bouton reste "pes�" jusqu'� ce qu'il soit reclicked on
        if (threeX && !twoX)
        {
            GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = buttonNormalColor;
            ChangeTime("1");
            threeX = !threeX;
        }
        
        else// if (!twoX)
        {
            GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = buttonPressedColor;
            GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = buttonNormalColor;
            // make time frames/calculations go three times as fast/second
            ChangeTime("3");
            threeX = !threeX;
            twoX = !twoX;
        }
        
    }

    // bouton Joint Manager
    public void JointManager()
    {
        ResetMouseState();
        if (curseur) { DeplacerObjets(); }
        if (singleDelete) { SingleDelete(); }

        if (!jmState)
        {
            physicsJoints = physicsManager.Joints();
            SetOnInspectorJointContent();
            SetOffInspectorContent();
            SetOffInspectorSoftContent();
            jmButton.GetComponent<Image>().color = buttonPressedColor;
            jmState = true;
            selectedIndex = -1;
            bo = physicsManager.SelectSpecificObject(selectedIndex, prefabHolder, SELECTEDOBJECTGAMEOBJECT);
        }

        else
        {
            SetOffInspectorJointContent();
            jmButton.GetComponent<Image>().color = buttonNormalColor;
            jmState = false;
        }

    }

    public void ShowSettingsPanel()
    {
        GamePanel.SetActive(false);
        ToolPanel.SetActive(true);
        ScrollViewPanel.SetActive(true);
        SettingPanel.SetActive(true);
        InspectorPanel.SetActive(true);
        GameviewPanel.SetActive(true);
        PausePanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void ShowPhysicsPanel()
    {
        GamePanel.SetActive(false);
        ToolPanel.SetActive(true);
        ScrollViewPanel.SetActive(true);
        SettingPanel.SetActive(true);
        InspectorPanel.SetActive(true);
        GameviewPanel.SetActive(true);
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        PhysicsPanel.SetActive(true);
    }

    public void ShowPausePanel()
    {

        GamePanel.SetActive(false);
        ToolPanel.SetActive(true);
        ScrollViewPanel.SetActive(true);
        SettingPanel.SetActive(true);
        InspectorPanel.SetActive(true);
        GameviewPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        PausePanel.SetActive(true);
    }

    public void ShowGamePanel()
    {
        ToolPanel.SetActive(true);
        ScrollViewPanel.SetActive(true);
        SettingPanel.SetActive(true);
        InspectorPanel.SetActive(true);
        GameviewPanel.SetActive(true);
        GamePanel.SetActive(true);
        SettingsPanel.SetActive(false);
        PausePanel.SetActive(false);
        PhysicsPanel.SetActive(false);
    }

    public void ChangeFullscreenState(bool check)
    {
        if (GameConstants.Fullscreen == false)
        {
            GameConstants.Fullscreen = true;
        }
        else
        {
            GameConstants.Fullscreen = false;
        }
        Screen.SetResolution(1920, 1080, GameConstants.Fullscreen, 60);
    }

    public void MusicVolumeUpdate(float value)
    {
        MusicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        GameConstants.MusicVolume = value * 100;
        MusicSource.volume = Mathf.Log10(float.Parse(value.ToString()));
    }

    public void SoundEffectVolumeUpdate(float value)
    {
        SoundEffectVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        GameConstants.SoundEffectVolume = value * 100;
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
        if (mc != null) { mc.SetMass(newMass); }
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
        if (bo != null) { bo.setIsStatic(newStatic); }
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
        for(int i = 0;i< softBodyBO.Length; i++)
        {
            softBodyBO[i].setIsStatic(newStaticSoft);
        }

    }

    public void InspectorSelectedJoint(System.Single NewJoint) 
    {
       

    }
    public void InspectorChangeDistance(System.Single NewDistance)
    {
        
    }
}
