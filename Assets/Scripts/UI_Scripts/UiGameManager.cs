using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
/*
 * Filename : UIGameManager
 * Author(s): Charles, Louis, Justin
 * Goal : Contains all the button functionalities of the BASIC Physic SIMULATION
 * 
 * Requirements : Attach this script to the UIManager
 */
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
    public GameObject TimeInputField;
    public AudioSource MusicSource;
    public AudioSource SolidDropSound;
    public AudioSource SoftDropSound;
    public AudioSource JointSound;
    public AudioSource ButtonClickedSound;
    public TextMeshProUGUI MusicVolumeText;
    public TextMeshProUGUI SoundEffectVolumeText;
    public PrefabsHolder prefabHolder;
    public PhysicsManager physicsManager;
    public GameObject jmButton;
    public GameObject boutonCurseur;
    public GameObject boutonSingleDelete;
    public GameObject boutonG;
    public GameObject boutonFF2;
    public GameObject boutonFF3;
    public GameObject boutonPause;
    public GameObject boutonPlay;
    public UIInspectorScript inspectorScript;
    public UISliderScript sliderScript;
    public GameObject buttonParent;
    public GameObject settingButton;
    public GameObject viewport;
    public GameObject inspectorParent;

    private Vector3 lastPosition;
    private Color buttonNormalColor = new Color(1f, 1f, 1f, 1f);
    private Color buttonPressedColor = new Color(0.04705882f, 1f, 0f, 1f);
    private float jointIndex;
    private Image jmButtonImage;
    private Image boutonCurseurImage;
    private Image boutonSingleDeleteImage;
    private Image boutonGImage;
    private Image boutonFF2Image;
    private Image boutonFF3Image;
    private Image boutonPauseImage;
    private Image boutonPlayImage;

    private bool twoX = false, threeX = false, pause = false;
    [HideInInspector]
    public bool jmState = false;
    [HideInInspector]
    public bool curseur = false;
    [HideInInspector]
    public bool singleDelete = false;
    [HideInInspector]
    public bool forces = false;
    [HideInInspector]
    public bool draggable = false;

    private BasicPhysicObject bo = null;


    private float gravityBefore, AirDensityBefore;

    void Start()
    {
        ChangeTime("1");
        jmButtonImage = jmButton.GetComponent<Image>();
        boutonCurseurImage = boutonCurseur.GetComponent<Image>();
        boutonSingleDeleteImage = boutonSingleDelete.GetComponent<Image>();
        boutonGImage = boutonG.GetComponent<Image>();
        boutonFF2Image = boutonFF2.GetComponent<Image>();
        boutonFF3Image = boutonFF3.GetComponent<Image>();
        boutonPauseImage = boutonPause.GetComponent<Image>();
        boutonPlayImage = boutonPlay.GetComponent<Image>();


        ShowGamePanel();
        boutonPlayImage.color = buttonPressedColor;
        Screen.SetResolution(1920, 1080, GameConstants.Fullscreen, 60); //int width, int height, bool fullscreen, int preferredRefreshRate (0 = unlimited)
        FullscreenToggle.GetComponent<Toggle>().isOn = GameConstants.Fullscreen;
        MusicSlider.GetComponent<Slider>().value = GameConstants.MusicVolume;
        SoundEffectSlider.GetComponent<Slider>().value = GameConstants.SoundEffectVolume;
        GravityInputField.GetComponent<TMP_InputField>().text = "9.8";
        AirDensityInputField.GetComponent<TMP_InputField>().text = "0.7";
        TimeInputField.GetComponent<TMP_InputField>().text = "1";
        BouncinessInputField.GetComponent<TMP_InputField>().text = "1";
        ChangeToDefault();
    }

    void Update()
    {
       
        //Universal inputs functionality
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!PausePanel.activeSelf) ShowPausePanel();
            else ShowGamePanel();
        }


        
        
        GameObject parent;
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		parent = null;
        if (bo == null && (Input.GetMouseButtonDown(0))) 
        {
            bo = physicsManager.SelectSpecificObject(inspectorScript.selectedIndex, prefabHolder, inspectorScript.SELECTEDOBJECTGAMEOBJECT);
        }
		if (!singleDelete && bo != null && bo.transform.parent != null) parent = bo.transform.parent.gameObject;
		if (draggable && pause && mousePosition.x > -28.25f && mousePosition.x < 28.25 && mousePosition.y > -20 && mousePosition.y < 20 && bo != null)
		{
			mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (lastPosition == Vector3.zero) lastPosition = mousePosition;
            Vector3 deltaPosition = mousePosition - lastPosition;
            if (parent != null && parent.GetComponent<SoftBody>() != null)
			{
				parent.GetComponent<SoftBody>().Move(deltaPosition);
			}
			else
			{
				bo.gameObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
                bo.gameObject.GetComponent<MeshColliderScript>().Translate(deltaPosition);
			}
			lastPosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
		}
		else if (draggable && mousePosition.x > -28.25f && mousePosition.x < 28.25 && mousePosition.y > -20 && mousePosition.y < 20 && bo != null && bo.getIsStatic() && !bo.IsWall)
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
			lastPosition = Vector3.zero;
            bo = null;
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
        print(AirDensity);
        UniversalVariable.SetAirDensity(float.Parse(AirDensity));
    }

    public void ChangeToDefault()
    {
        GravityInputField.GetComponent<TMP_InputField>().text = "9.8";
        AirDensityInputField.GetComponent<TMP_InputField>().text = "0.7";
        TimeInputField.GetComponent<TMP_InputField>().text = "1";
        BouncinessInputField.GetComponent<TMP_InputField>().text = "1";
        UniversalVariable.SetGravity(float.Parse("9.8"));
        UniversalVariable.SetTime(float.Parse("1"));
        UniversalVariable.SetAirDensity(float.Parse("0.7"));
        UniversalVariable.SetBounciness(float.Parse("1"));
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    // bouton curseur (1)
    public void DeplacerObjets()
    {
        sliderScript.ResetMouseState();

        if (jmState) { JointManager(); }
        if (singleDelete) { SingleDelete(); }

        if (curseur && !singleDelete)
        {
            boutonCurseurImage.color = (buttonNormalColor);
            curseur = !curseur;
        }

        else if (!singleDelete)
        {
            boutonCurseurImage.color = (buttonPressedColor);

            // code ici qui fait en sorte qu'on peut d�placer des objets
            curseur = !curseur;
        }
        // une fois qu'il se fait click, faire en sorte que le bouton reste "pes�" jusqu'� ce qu'il soit reclicked

        
    }

    // bouton SingleDelete (2)
    public void SingleDelete()
    {
        sliderScript.ResetMouseState();

        if (jmState) { JointManager(); }
        if (curseur) { DeplacerObjets(); }

         
        if (singleDelete && !curseur)
        {
            boutonSingleDeleteImage.color = (buttonNormalColor);
            singleDelete = !singleDelete;
        }

        else if (!curseur)
        {
            boutonSingleDeleteImage.color = (buttonPressedColor);
            singleDelete = !singleDelete;
            // faire en sorte qu'on peut tourner des objets ici => rendu single delete
        }

        
    }

   
    // bouton F (3)
    public void AppliquerForcesDePhysique()
    {
        sliderScript.ResetMouseState();
        // code qui fait en sorte que les forces de physique seront appliqu�es sur les objets
        if (forces)
        {
            boutonGImage.color = (buttonNormalColor);
            if (UniversalVariable.GetGravity() == 0) { UniversalVariable.SetGravity(gravityBefore); } 
            if (UniversalVariable.GetAirDensity() == 0) { UniversalVariable.SetAirDensity(AirDensityBefore); }
        }

        else
        {
            boutonGImage.color = (buttonPressedColor);
             
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
        sliderScript.ResetMouseState();
        // code qui fait en sorte que les objets qui �taient dans la fen�tre disparaissent.
        SoftBody[] softBodies = FindObjectsOfType<SoftBody>();
        for (int i = softBodies.Length-1; i >= 0; i--) 
        {
            Destroy(softBodies[i].gameObject);
        }
        physicsManager.ResetList();

        //enleve la selection de l'objet presentement
        inspectorScript.SELECTEDOBJECT = -1;
        inspectorScript.SetAllOff();
        inspectorScript.SELECTEDOBJECTGAMEOBJECT = null;
    }

   
    // bouton pause (5)
    public void PauseScene()
    {
        sliderScript.ResetMouseState();
        
        // make time frames/calculations equal to 0
        ChangeTime("0");
        boutonPauseImage.color = buttonPressedColor;
        boutonPlayImage.color = buttonNormalColor;
        boutonFF2Image.color = (buttonNormalColor);
        boutonFF3Image.color = (buttonNormalColor);
        pause = true;
    }
    
    // bouton play (6)
    public void PlayScene()
    {
        sliderScript.ResetMouseState();
        // make time frames/calculations start
        ChangeTime("1");
        boutonPauseImage.color = buttonNormalColor;
        boutonPlayImage.color = buttonPressedColor;
        boutonFF2Image.color = (buttonNormalColor);
        boutonFF3Image.color = (buttonNormalColor);
        if (pause) pause = false;
    }

    // bouton 2x (7)
    public void TwoXFaster()
    {
        sliderScript.ResetMouseState();
         
        if (pause) pause = false;
        if (twoX && !threeX)
        {
            boutonFF2Image.color = (buttonNormalColor);
            ChangeTime("1");
            twoX = !twoX;
        }

        else
        {
            boutonFF2Image.color = (buttonPressedColor);
            boutonFF3Image.color = (buttonNormalColor);
            boutonPlayImage.color = buttonNormalColor;
            boutonPauseImage.color = buttonNormalColor;
            // make time frames/calculations go twice as fast/second
            ChangeTime("2");
            
            twoX = !twoX;
            threeX = !threeX;
        }
        
    }

    // bouton 3x (8)
    public void ThreeXFaster()
    {
        sliderScript.ResetMouseState();
        if (pause) pause = false;
         
        if (threeX && !twoX)
        {
            boutonFF3Image.color = (buttonNormalColor);
            ChangeTime("1");
            threeX = !threeX;
        }
        
        else// if (!twoX)
        {
            boutonFF3Image.color = (buttonPressedColor);
            boutonFF2Image.color = (buttonNormalColor);
            boutonPlayImage.color = buttonNormalColor;
            boutonPauseImage.color = buttonNormalColor;
            // make time frames/calculations go three times as fast/second
            ChangeTime("3");
            threeX = !threeX;
            twoX = !twoX;
        }
        
    }

    // bouton Joint Manager
    public void JointManager()
    {
        sliderScript.ResetMouseState();
        if (curseur) { DeplacerObjets(); }
        if (singleDelete) { SingleDelete(); }
        BasicPhysicObject bo = null;
        if (!jmState)
        {

            List<DistanceJoints> independentJoints = physicsManager.GetAllNonSoftBodyJoints();
            if(independentJoints.Count > 0) { 

                inspectorScript.SetOnInspectorJointContent();
                inspectorScript.SetOffInspectorContent();
                inspectorScript.SetOffInspectorSoftContent();
                jmButtonImage.color = (buttonPressedColor);
                jmState = true;
                inspectorScript.selectedIndex = -1;

                bo = physicsManager.SelectSpecificObject(inspectorScript.selectedIndex, prefabHolder, inspectorScript.SELECTEDOBJECTGAMEOBJECT);
                inspectorScript.JointInspectorInitialize(independentJoints);
            }

        }

        else
        {
            inspectorScript.SetOffInspectorJointContent();
            jmButtonImage.color = (buttonNormalColor);
            jmState = false;
            inspectorScript.DeselectAll();
        }
        
    }

    public void ShowSettingsPanel()
    {
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void ShowPhysicsPanel()
    {
        PausePanel.SetActive(false);
        PhysicsPanel.SetActive(true);
    }

    public void ShowPausePanel()
    {
        DisableInteraction();
        SettingsPanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        PausePanel.SetActive(true);
    }

    public void ShowGamePanel()
    {
        EnableInteraction();
        PausePanel.SetActive(false);
    }

    public void DisableInteraction()
    {
        sliderScript.ResetMouseState();
        inspectorParent.SetActive(false);
        buttonParent.SetActive(false);
        viewport.SetActive(false);
        settingButton.SetActive(false);
        GamePanel.SetActive(false);
    }

    public void EnableInteraction()
    {
        inspectorParent.SetActive(true);
        buttonParent.SetActive(true);
        viewport.SetActive(true);
        settingButton.SetActive(true);
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
        //MusicSource.volume = Mathf.Log10(float.Parse(value.ToString()));
    }

    public void SoundEffectVolumeUpdate(float value)
    {
        float volume = (Mathf.Log10(value) + 1);
        SoundEffectVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
        GameConstants.SoundEffectVolume = value * 100;
        SolidDropSound.volume = volume;
        SoftDropSound.volume = volume;
        JointSound.volume = volume;
        ButtonClickedSound.volume = volume;
        //Debug.Log(volume);
    }



  


}
