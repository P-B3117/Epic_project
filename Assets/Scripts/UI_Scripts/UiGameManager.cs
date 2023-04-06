using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;


public class UiGameManager : MonoBehaviour
{
    [Header("Reference for UI basic functions")]
    public GameObject GamePanel;
    public GameObject PausePanel;
    public GameObject PhysicsPanel;
    public GameObject SettingsPanel;
    public GameObject FullscreenToggle;
    public GameObject MusicSlider;
    public GameObject SoundEffectSlider;
    public GameObject GravityInputField;
    public GameObject AirDragInputField;
    public GameObject TimeInputField;
    public AudioSource MusicSource;
    public Text MusicVolumeText;
    public Text SoundEffectVolumeText;
    public PrefabsHolder prefabHolder;
    public PhysicsManager physicsManager;

    // Start is called before the first frame update



    //Slider functions variables
    private int MOUSESTATE;
    List<GameObject> meshCreatorPoints;
    [Header("Reference for the slider")]
    public LineRenderer meshCreatorLineRenderer;
    GameObject currentShadowObject;

    //Inspector variables
    [Header("Reference for the inspector")]
    private int SELECTEDOBJECT = -1;
    public GameObject InspectorContent;
    public Slider MassSlider;
    public Slider BoucinessSlider;
    public Slider DynamicFrictionSlider;
    public Slider StaticFrictionSlider;
    public Toggle IsStaticToggle;
    public TextMeshProUGUI MassText;
    public TextMeshProUGUI BoucinessText;
    public TextMeshProUGUI DynamicFrictionText;
    public TextMeshProUGUI StaticFrictionText;
    

    void Start()
    {
        ShowGamePanel();
        Screen.SetResolution(1920, 1080, GameConstants.Fullscreen, 60); //int width, int height, bool fullscreen, int preferredRefreshRate (0 = unlimited)
        FullscreenToggle.GetComponent<Toggle>().isOn = GameConstants.Fullscreen;
        MusicSlider.GetComponent<Slider>().value = GameConstants.MusicVolume;
        SoundEffectSlider.GetComponent<Slider>().value = GameConstants.SoundEffectVolume;
        GravityInputField.GetComponent<TMP_InputField>().text = "9.8";
        AirDragInputField.GetComponent<TMP_InputField>().text = "1";
        TimeInputField.GetComponent<TMP_InputField>().text = "1";

        //Initialize the slider function variables
        MOUSESTATE = -1;
        meshCreatorPoints = new List<GameObject>();
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPausePanel();
        }
        if (Input.GetMouseButtonDown(1)) 
        {
            ResetMouseState();
        }

        //If click check if clicked on a physicObject and update inspector



        //!!!!!Functionality of the slider!!!!!
        //Mouse functionality for basic objects
        if (MOUSESTATE >= 0 && MOUSESTATE <= 4)
        {

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
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject newGO = Instantiate(currentShadowObject);
                    physicsManager.AddPhysicObject(newGO);
                }
            }
            else
            {
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);

            }




        }
        //mouse functinality for the Mesh creator
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
        //SoftBody
        else if (MOUSESTATE == 6)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);



            //The worldPoint boundaries are : (-28.25, 20)      - (28.25, 20)
            //                                (-28.25, -20)   - (28.25, -20)

            //Check boundaries
            currentShadowObject.transform.position = new Vector3(worldPoint.x / 2, worldPoint.y / 2, 0.0f);
           
            
            if (worldPoint.x > -28.25f && worldPoint.x < 28.25 &&
                worldPoint.y > -20 && worldPoint.y < 20)
            {
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
            else
            {
                currentShadowObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);

            }


        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPoint = new Vector3(worldPoint.x, worldPoint.y, 0.0f);

                if (worldPoint.x > -28.25f && worldPoint.x < 28.25 &&
                    worldPoint.y > -20 && worldPoint.y < 20)
                {
                    int selectedIndex = (physicsManager.FindClickIndex(worldPoint));
                    SELECTEDOBJECT = selectedIndex;
                    if (selectedIndex == -1) { InspectorContent.SetActive(false); }
                    else { InspectorContent.SetActive(true); }


                    BasicPhysicObject bo = physicsManager.SelectSpecificObject(selectedIndex, prefabHolder);

                    if (bo != null)
                    {

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

                    }


                }



            }
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

    public void ChangeAirDrag(string airDrag)
    {
        UniversalVariable.SetAirDrag(float.Parse(airDrag));
    }

    public void ChangeToDefault()
    {
        GravityInputField.GetComponent<TMP_InputField>().text = "9.8";
        AirDragInputField.GetComponent<TMP_InputField>().text = "1";
        TimeInputField.GetComponent<TMP_InputField>().text = "1";
        UniversalVariable.SetGravity(float.Parse("9,8"));
        UniversalVariable.SetTime(float.Parse("1"));
        UniversalVariable.SetAirDrag(float.Parse("1"));
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(1);
    }

    private bool curseur = false, rotation = false, forces = false, twoX = false, threeX = false;

    // bouton curseur (1)
    public void DeplacerObjets()
    {
        ResetMouseState();

        if (curseur)
        {
            Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonCurseur").GetComponent<Button>().colors.normalColor;
            GameObject.Find("Canvas/ToolPanel/BoutonCurseur").GetComponent<Image>().color = normalColor;
        }

        else
        {
            Color pressedColor = GameObject.Find("Canvas/ToolPanel/BoutonCurseur").GetComponent<Button>().colors.pressedColor;
            GameObject boutonCurseur = GameObject.Find("Canvas/ToolPanel/BoutonCurseur");
            boutonCurseur.GetComponent<Image>().color = pressedColor;
            
            // code ici qui fait en sorte qu'on peut déplacer des objets
            
        }
        // une fois qu'il se fait click, faire en sorte que le bouton reste "pesé" jusqu'à ce qu'il soit reclicked

        curseur = !curseur;
    }

    // bouton rotation (2)
    public void TournerObjets()
    {
        ResetMouseState();
        // une fois qu'il se fait click, faire en sorte que le bouton reste "pesé" jusqu'à ce qu'il soit reclicked on
        if (rotation)
        {
            Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonRotation").GetComponent<Button>().colors.normalColor;
            GameObject.Find("Canvas/ToolPanel/BoutonRotation").GetComponent<Image>().color = normalColor;
        }

        else
        {
            Color pressedColor = GameObject.Find("Canvas/ToolPanel/BoutonRotation").GetComponent<Button>().colors.pressedColor;
            GameObject.Find("Canvas/ToolPanel/BoutonRotation").GetComponent<Image>().color = pressedColor;
            // faire en sorte qu'on peut tourner des objets ici

        }

        rotation = !rotation;
    }

    private float gravityBefore, airDragBefore;
    // bouton F (3)
    public void AppliquerForcesDePhysique()
    {
        ResetMouseState();
        // code qui fait en sorte que les forces de physique seront appliquées sur les objets
        if (forces)
        {
            Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonF").GetComponent<Button>().colors.normalColor;
            GameObject.Find("Canvas/ToolPanel/BoutonF").GetComponent<Image>().color = normalColor;
            if (UniversalVariable.GetGravity() == 0) { UniversalVariable.SetGravity(gravityBefore); } 
            if (UniversalVariable.GetAirDrag() == 0) { UniversalVariable.SetAirDrag(airDragBefore); }
        }

        else
        {
            Color pressedColor = GameObject.Find("Canvas/ToolPanel/BoutonF").GetComponent<Button>().colors.pressedColor;
            GameObject.Find("Canvas/ToolPanel/BoutonF").GetComponent<Image>().color = pressedColor;
            // une fois qu'il se fait click, faire en sorte que le bouton reste "pesé" jusqu'à ce qu'il soit reclicked on
            gravityBefore = UniversalVariable.GetGravity();
            airDragBefore = UniversalVariable.GetAirDrag();
            UniversalVariable.SetGravity(0);
            UniversalVariable.SetAirDrag(0);
        }
        forces = !forces;
    }

    // bouton poubelle (4)
    public void ResetFenetre()
    {
        ResetMouseState();
        // code qui fait en sorte que les objets qui étaient dans la fenêtre disparaissent.
        SoftBody[] softBodies = FindObjectsOfType<SoftBody>();
        for (int i = softBodies.Length-1; i >= 0; i--) 
        {
            Destroy(softBodies[i].gameObject);
        }
        physicsManager.ResetList();
    }

    public void ResetMouseState() { 
        MOUSESTATE = -1; 
        Destroy(currentShadowObject); 
        currentShadowObject = null;
        for (int i = meshCreatorPoints.Count-1; i >= 0; i--) 
        {
            Destroy(meshCreatorPoints[i]);
        }
        meshCreatorPoints = new List<GameObject>();

        meshCreatorLineRenderer.positionCount = 1;
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
        currentShadowObject = prefabHolder.GetSoftBody1();
		currentShadowObject.transform.SetParent(GamePanel.transform);
	}


    // bouton pause (5)
    public void PauseScene()
    {
        ResetMouseState();
        // make time frames/calculations equal to 0
        ChangeTime("0");
        Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Button>().colors.normalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = normalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = normalColor;
    }
    
    // bouton play (6)
    public void PlayScene()
    {
        ResetMouseState();
        // make time frames/calculations start
        ChangeTime("1");
        Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Button>().colors.normalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = normalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = normalColor;
    }

    // bouton 2x (7)
    public void TwoXFaster()
    {
        ResetMouseState();
        // une fois qu'il se fait click, faire en sorte que le bouton reste "pesé" jusqu'à ce qu'il soit reclicked on
        if (twoX && !threeX)
        {
            Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Button>().colors.normalColor;
            GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = normalColor;
            ChangeTime("1");
            twoX = !twoX;
        }

        else if (!threeX)
        {
            Color pressedColor = GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Button>().colors.pressedColor;
            GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = pressedColor;
            // make time frames/calculations go twice as fast/second
            ChangeTime("2");
            
            twoX = !twoX;
        }
        
    }

    // bouton 3x (8)
    public void ThreeXFaster()
    {
        ResetMouseState();
        // une fois qu'il se fait click, faire en sorte que le bouton reste "pesé" jusqu'à ce qu'il soit reclicked on
        if (threeX && !twoX)
        {
            Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Button>().colors.normalColor;
            GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = normalColor;
            ChangeTime("1");
            threeX = !threeX;
        }
        
        else if (!twoX)
        {
            Color pressedColor = GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Button>().colors.pressedColor;
            GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = pressedColor;
            // make time frames/calculations go three times as fast/second
            ChangeTime("3");
            threeX = !threeX;
        }
        
    }


    public void ShowSettingsPanel()
    {
        //GamePanel.SetActive(false);
        PausePanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void ShowPhysicsPanel()
    {
        //GamePanel.SetActive(false);
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        PhysicsPanel.SetActive(true);
    }

    public void ShowPausePanel()
    {
       
        //GamePanel.SetActive(true);
        SettingsPanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        PausePanel.SetActive(true);
    }

    public void ShowGamePanel()
    {
       
        SettingsPanel.SetActive(false);
        PausePanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        GamePanel.SetActive(true);
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




}
