using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject SettingsPanel;
    // Start is called before the first frame update
    void Start()
    {
        MenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
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
}
