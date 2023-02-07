using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject SettingsPanel;
    public GameObject FullscreenToggle;
    // Start is called before the first frame update
    void Start()
    {
        MenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        Screen.SetResolution(1920, 1080, GameConstants.Fullscreen, 60); //int width, int height, bool fullscreen, int preferredRefreshRate (0 = unlimited)
        FullscreenToggle.GetComponent<Toggle>().isOn = true;
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
            Debug.Log("Fullscreen state changed to true");
        }
        else { 
            GameConstants.Fullscreen = false;
            Debug.Log("Fullscreen state changed to false");
        }
        Screen.SetResolution(1920, 1080, GameConstants.Fullscreen, 60);
    }
}
