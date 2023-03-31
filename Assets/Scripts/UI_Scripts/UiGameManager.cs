using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;


public class UiGameManager : MonoBehaviour
{
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
    public PrefabsHolder PrefabHolder;
    public PhysicsManager physicsManager;

    // Start is called before the first frame update



    //Slider functions variables
    private int MOUSESTATE;
    List<GameObject> meshCreatorPoints;
    GameObject currentShadowObject;


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

        MOUSESTATE = -1;
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPausePanel();
        }


        //Functionality of the slider
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


        //MouseState
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
        physicsManager.ResetList();
    }

    public void ResetMouseState() { MOUSESTATE = -1; Destroy(currentShadowObject); currentShadowObject = null; }
    public void SetMouseState0() { ResetMouseState(); MOUSESTATE = 0; currentShadowObject = PrefabHolder.GetLittleCircle();  currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState1() { ResetMouseState(); MOUSESTATE = 1; currentShadowObject = PrefabHolder.GetMiddleCircle(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState2() { ResetMouseState(); MOUSESTATE = 2; currentShadowObject = PrefabHolder.GetBigCircle(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState3() { ResetMouseState(); MOUSESTATE = 3; currentShadowObject = PrefabHolder.GetMiddleTriangle(); currentShadowObject.transform.SetParent(GamePanel.transform); }
    public void SetMouseState4() { ResetMouseState(); MOUSESTATE = 4; currentShadowObject = PrefabHolder.GetBigSquare(); currentShadowObject.transform.SetParent(GamePanel.transform); }

    public void SetMouseState5() { ResetMouseState(); MOUSESTATE = 5; currentShadowObject = PrefabHolder.GetMeshCreatorPoint(); currentShadowObject.transform.SetParent(GamePanel.transform); }


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
            Debug.Log("times 2");
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
        GamePanel.SetActive(false);
        PausePanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void ShowPhysicsPanel()
    {
        GamePanel.SetActive(false);
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        PhysicsPanel.SetActive(true);
    }

    public void ShowPausePanel()
    {
        Time.timeScale = 0;
        GamePanel.SetActive(true);
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




}
