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
    
    public GameObject SettingsPanel;
    public GameObject FullscreenToggle;
    public GameObject MusicSlider;
    public GameObject SoundEffectSlider;
    
 
    public AudioSource MusicSource;
    public Text MusicVolumeText;
    public Text SoundEffectVolumeText;

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
    }
    public void ResetMouseState() 
    {
        MOUSESTATE = -1;
        Destroy(currentShadowObject);
        CURSOR = false;
    }


    public void ChangeTime(string time)
    {
        UniversalVariable.SetTime(float.Parse(time));
    }

   

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(1);
    }

   private bool twoX = false, threeX = false;
    // bouton curseur (1)
    

   

    // bouton poubelle (4)
    public void ResetFenetre()
    {
        ResetMouseState();
        fluidManager.RemoveAllParticles();
    }







    public void FluidCursor() 
    {
        ResetMouseState();
        CURSOR = !CURSOR;
        if (CURSOR)
            GameObject.Find("Canvas/ToolPanel/BoutonCurseur").GetComponent<Image>().color = Color.green;
        else 
        {
            GameObject.Find("Canvas/ToolPanel/BoutonCurseur").GetComponent<Image>().color = Color.white;
        }
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
        
        Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Button>().colors.normalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = Color.green;
        ChangeTime("2");
        twoX = true;
        if (threeX) 
        {
            threeX = false;
            GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = Color.white;
        }


    }

    // bouton 3x (8)
    public void ThreeXFaster()
    {
        ResetMouseState();
        Color normalColor = GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Button>().colors.normalColor;
        GameObject.Find("Canvas/ToolPanel/BoutonFF3").GetComponent<Image>().color = Color.green;
        ChangeTime("3");
        threeX = true;
        if (twoX)
        {
            twoX = false;
            GameObject.Find("Canvas/ToolPanel/BoutonFF2").GetComponent<Image>().color = Color.white;
        }

    }


    public void ShowSettingsPanel()
    { 
        PausePanel.SetActive(false);
       
        SettingsPanel.SetActive(true);
    }

  
    public void ShowPausePanel()
    {
        ResetMouseState();
        SettingsPanel.SetActive(false);
       
        PausePanel.SetActive(true);
    }

    public void PauseResume() 
    {
        SettingsPanel.SetActive(false);
        
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
