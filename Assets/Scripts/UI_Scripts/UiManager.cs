using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class UiManager : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject SettingsPanel;
    public GameObject FullscreenToggle;
    public GameObject MusicSlider;
    public GameObject SoundEffectSlider;
    public AudioSource MusicSource;
    public TextMeshProUGUI MusicVolumeText;
    public TextMeshProUGUI SoundEffectVolumeText;

    private Vector3 slidedPosition;
    private Vector3 retractedPosition;
    private bool menuSlided;
    private bool menuRetracted;
    private bool menuHasToSlide = false;
    private bool menuHasToRetract = false;
    private float slideSpeed = 10;
    // Start is called before the first frame update
    void Start()
    {
        MenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        Screen.SetResolution(1920, 1080, GameConstants.Fullscreen, 60); //int width, int height, bool fullscreen, int preferredRefreshRate (0 = unlimited)
        FullscreenToggle.GetComponent<Toggle>().isOn = true;
        MusicSlider.GetComponent<Slider>().value = GameConstants.MusicVolume;
        SoundEffectSlider.GetComponent<Slider>().value = GameConstants.SoundEffectVolume;
        menuSlided = false;
        slidedPosition = new Vector3(256, 540, 0);
        retractedPosition = new Vector3(-164, 540, 0);
    }
    public void Update()
    {
        Debug.Log(MenuPanel.transform.position.x);
        if (!menuSlided && menuHasToSlide) MenuPanel.transform.position = Vector3.MoveTowards(MenuPanel.transform.position, slidedPosition, slideSpeed);
        if (MenuPanel.transform.position == slidedPosition) { menuSlided = true; menuHasToSlide = false; }
        else menuSlided = false;

        if (!menuRetracted && menuHasToRetract) MenuPanel.transform.position = Vector3.MoveTowards(MenuPanel.transform.position, retractedPosition, slideSpeed);
        if (MenuPanel.transform.position == retractedPosition) { menuRetracted = true; menuHasToRetract = false; }
        else menuRetracted = false;
    }

    public void SlideMenu()
    {//256
        if (MenuPanel.transform.position.x >= -164 && MenuPanel.transform.position.x < 256) { menuHasToSlide = true; menuHasToRetract = false; }
    }

    public void RetractMenu()
    {//-164
        if (MenuPanel.transform.position.x > -164 && MenuPanel.transform.position.x <= 256) { menuHasToRetract = true; menuHasToSlide = false; }
    }

    public void ShowSettingsPanel()
    {
        MenuPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void ShowMenuPanel()
    {
        MenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
    }

    public void ChangeFullscreenState(bool check)
    {
        if (GameConstants.Fullscreen == false) { 
            GameConstants.Fullscreen = true;
        }
        else { 
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
