using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UiGameManager : MonoBehaviour
{
    public GameObject GamePanel;
    public GameObject PausePanel;
    public GameObject PhysicsPanel;
    public GameObject SettingsPanel;
    public GameObject FullscreenToggle;
    public GameObject MusicSlider;
    public GameObject SoundEffectSlider;
    public AudioSource MusicSource;
    public Text MusicVolumeText;
    public Text SoundEffectVolumeText;
    // Start is called before the first frame update
    void Start()
    {
        ShowPausePanel();
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
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(1);
    }

    public void ShowSettingsPanel()
    {
        GamePanel.SetActive(false);
        PausePanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        Debug.Log("showing Settings Panel");
    }

    public void ShowPhysicsPanel()
    {
        GamePanel.SetActive(false);
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        PhysicsPanel.SetActive(true);
        Debug.Log("showing Physics Panel");
    }

    public void ShowPausePanel()
    {
        Time.timeScale = 0;
        GamePanel.SetActive(true);
        SettingsPanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        PausePanel.SetActive(true);
        Debug.Log("showing Pause Panel");
    }

    public void ShowGamePanel()
    {
        Time.timeScale = 1;
        SettingsPanel.SetActive(false);
        PausePanel.SetActive(false);
        PhysicsPanel.SetActive(false);
        GamePanel.SetActive(true);
        Debug.Log("showing Game Panel");
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
