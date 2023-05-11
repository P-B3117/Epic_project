using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
/*
 * Filename : UIFluidManager
 * Author(s): Charles, Louis
 * Goal : Contains all the UI functionnalities of the FLUID SIMULATION
 * 
 * Requirements : Attach this script to the UIManager
 */
public class UIFluidManager : MonoBehaviour
{
    [Header("Reference for UI basic functions")]
  
    public GameObject PausePanel;
    
    public GameObject SettingsPanel;
    public GameObject FullscreenToggle;
    public GameObject MusicSlider;
    public GameObject SoundEffectSlider;
    public GameObject boutonCurseur;
    public GameObject boutonFF2;
    public GameObject boutonFF3;
    public GameObject boutonPause;
    public GameObject boutonPlay;
    public GameObject buttonParent;
    public GameObject settingButton;
    public GameObject viewport;
    public GameObject inspectorParent;
    public GameObject objectFluidManager;


    public AudioSource MusicSource;
    public AudioSource SolidDropSound;
    public AudioSource SoftDropSound;
    public AudioSource JointSound;
    public AudioSource ButtonClickedSound;
    public TextMeshProUGUI MusicVolumeText;
    public TextMeshProUGUI SoundEffectVolumeText;

    public PrefabsHolder prefabHolder;


    [Header("Reference for sliders and button functionalities")]
    public ObjectFluidManager fluidManager;
    public TextMeshProUGUI GravityXNumber;
    public TextMeshProUGUI GravityYNumber;
    public TextMeshProUGUI ViscosityNumber;



    private int MOUSESTATE = -1;
    private float addFluidTIMER = 0.1f;
    private GameObject currentShadowObject;

    private bool CURSOR = false;

    private Image boutonCurseurImage;
    private Image boutonFF2Image;
    private Image boutonFF3Image;
    private Image boutonPauseImage;
    private Image boutonPlayImage;
    private Color buttonNormalColor = new Color(1f, 1f, 1f, 1f);
    private Color buttonPressedColor = new Color(0.04705882f, 1f, 0f, 1f);

    public bool IsCursorClick() 
    {
        if (CURSOR && Input.GetMouseButton(0))
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    public Vector3 GetCursorPosition() 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(mousePos.x, mousePos.y, 0);
    }

    void Start()
    {
        boutonCurseurImage = boutonCurseur.GetComponent<Image>();
        boutonFF2Image = boutonFF2.GetComponent<Image>();
        boutonFF3Image = boutonFF3.GetComponent<Image>();
        boutonPauseImage = boutonPause.GetComponent<Image>();
        boutonPlayImage = boutonPlay.GetComponent<Image>();

        Screen.SetResolution(1920, 1080, GameConstants.Fullscreen, 60); //int width, int height, bool fullscreen, int preferredRefreshRate (0 = unlimited)
        FullscreenToggle.GetComponent<Toggle>().isOn = GameConstants.Fullscreen;
        MusicSlider.GetComponent<Slider>().value = GameConstants.MusicVolume;
        SoundEffectSlider.GetComponent<Slider>().value = GameConstants.SoundEffectVolume;
        


       
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

        if (MOUSESTATE == 0) 
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 screenPos = new Vector3(worldPoint.x, worldPoint.y, 0.0f);
            currentShadowObject.transform.position = screenPos;

            if (worldPoint.x > -28.25f + 2 && worldPoint.x < 28.25 - 2 &&
                worldPoint.y > -20  + 2&& worldPoint.y < 20 - 2)
            {
                SpriteRenderer spr = currentShadowObject.GetComponent<SpriteRenderer>();
                if (spr != null)
                {
                    spr.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
                }
                if (Input.GetMouseButton(0)) 
                {
                    addFluidTIMER += Time.deltaTime;
                    if(addFluidTIMER > 0.1f) 
                    {
                        fluidManager.AddParticle(screenPos, prefabHolder);
                        for (int i = 0; i < 6; i++)
                        {
                            float r1 = Random.Range(-1.5f, 1.5f);
                            float r2 = Random.Range(-1.5f, 1.5f);
                            Vector3 D = new Vector3(r1, r2, 0.0f);
                            D = D.normalized * fluidManager.GetParticleSize() * 2;
                            fluidManager.AddParticle(screenPos + D, prefabHolder);
                            SoftDropSound.Play();
                        }
                        addFluidTIMER = 0;
                    }
                    
                }
            }
            else 
            {
                SpriteRenderer spr = currentShadowObject.GetComponent<SpriteRenderer>();
                if (spr != null) 
                {
                    spr.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                }
            }



        }
       

       


    }


    public void SetMouseState0() 
    {
        MOUSESTATE = 0;
        currentShadowObject = prefabHolder.GetWaterAddSprite();
        CURSOR = false;
        boutonCurseurImage.color = buttonNormalColor;
    }
    public void ResetMouseState() 
    {
        MOUSESTATE = -1;
        Destroy(currentShadowObject);
       
    }


    public void ChangeTime(string time)
    {
        UniversalVariable.SetTime(float.Parse(time));
    }

   

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }

   private bool twoX = false, threeX = false;

    // bouton poubelle (4)
    public void ResetFenetre()
    {
        ResetMouseState();
        fluidManager.RemoveAllParticles();
    }







    public void FluidCursor() //bouton curseur
    {
        ResetMouseState();
        CURSOR = !CURSOR;
        if (CURSOR)
        {
            boutonCurseurImage.color = buttonPressedColor;
        }
        else
        {
            boutonCurseurImage.color = buttonNormalColor;
        }
    }


    // bouton pause (5)
    public void PauseScene()
    {
        ResetMouseState();

        // make time frames/calculations equal to 0
        ChangeTime("0");
        boutonPauseImage.color = buttonPressedColor;
        boutonPlayImage.color = buttonNormalColor;
        boutonFF2Image.color = (buttonNormalColor);
        boutonFF3Image.color = (buttonNormalColor);
    }

    // bouton play (6)
    public void PlayScene()
    {
        ResetMouseState();
        // make time frames/calculations start
        ChangeTime("1");
        boutonPauseImage.color = buttonNormalColor;
        boutonPlayImage.color = buttonPressedColor;
        boutonFF2Image.color = (buttonNormalColor);
        boutonFF3Image.color = (buttonNormalColor);
    }

    // bouton 2x (7)
    public void TwoXFaster()
    {
        ResetMouseState();

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
        ResetMouseState();

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




    public void ShowSettingsPanel()
    {
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }



    public void ShowPausePanel()
    {
        DisableInteraction();
        SettingsPanel.SetActive(false);
        PausePanel.SetActive(true);
    }

    public void ShowGamePanel()
    {
        EnableInteraction();
        PausePanel.SetActive(false);
    }

    public void DisableInteraction()
    {
        ResetMouseState();
        inspectorParent.SetActive(false);
        buttonParent.SetActive(false);
        viewport.SetActive(false);
        settingButton.SetActive(false);
        objectFluidManager.SetActive(false);
    }

    public void EnableInteraction()
    {
        inspectorParent.SetActive(true);
        buttonParent.SetActive(true);
        viewport.SetActive(true);
        settingButton.SetActive(true);
        objectFluidManager.SetActive(true);
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
        float volume = ((Mathf.Log10(value) + 1));
        SoundEffectVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
        GameConstants.SoundEffectVolume = value * 100;
        SolidDropSound.volume = volume;
        SoftDropSound.volume = volume;
        JointSound.volume = volume;
        ButtonClickedSound.volume = volume;
        //Debug.Log(volume);
    }

    public void ChangeGravityX(System.Single newGX) 
    {
        ResetMouseState();
        fluidManager.Gravity = new Vector2(newGX, fluidManager.Gravity.y);
        GravityXNumber.text = newGX.ToString();
    }
    public void ChangeGravityY(System.Single newGY) 
    {
        ResetMouseState();
        fluidManager.Gravity = new Vector2(fluidManager.Gravity.x, newGY);
        GravityYNumber.text = newGY.ToString();
    }
    public void ChangeViscosity(System.Single newVis) 
    {
        ResetMouseState();
        fluidManager.Viscosity = newVis;
        ViscosityNumber.text = newVis.ToString();
    }




}
