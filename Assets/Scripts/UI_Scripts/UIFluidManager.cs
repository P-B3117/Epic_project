using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class UIFluidManager : MonoBehaviour
{
    [Header("Reference for UI basic functions")]
  
    public GameObject PausePanel;
    public GameObject PhysicsPanel;
    public GameObject SettingsPanel;
    public GameObject FullscreenToggle;
    public GameObject MusicSlider;
    public GameObject SoundEffectSlider;
    public GameObject GravityInputField;
 
    public AudioSource MusicSource;
    public Text MusicVolumeText;
    public Text SoundEffectVolumeText;
   

  



   
   


    void Start()
    {
       
        Screen.SetResolution(1920, 1080, GameConstants.Fullscreen, 60); //int width, int height, bool fullscreen, int preferredRefreshRate (0 = unlimited)
        FullscreenToggle.GetComponent<Toggle>().isOn = GameConstants.Fullscreen;
        MusicSlider.GetComponent<Slider>().value = GameConstants.MusicVolume;
        SoundEffectSlider.GetComponent<Slider>().value = GameConstants.SoundEffectVolume;
        GravityInputField.GetComponent<TMP_InputField>().text = "9.8";


       
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPausePanel();
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
       
    }



  

 


    // bouton pause (5)
    public void PauseScene()
    {
       
        // make time frames/calculations equal to 0
        ChangeTime("0");
        Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Button>().colors.normalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = normalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = normalColor;
    }

    // bouton play (6)
    public void PlayScene()
    {
       
        // make time frames/calculations start
        ChangeTime("1");
        Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Button>().colors.normalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = normalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = normalColor;
    }

    // bouton 2x (7)
    public void TwoXFaster()
    {
       
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

    public void PauseResume() 
    {
        SettingsPanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        PausePanel.SetActive(false);
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
