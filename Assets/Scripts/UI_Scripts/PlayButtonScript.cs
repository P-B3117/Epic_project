using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Filename : PlayButtonScript
 * Author(s): Charles
 * Goal : switch between scenes
 * 
 */
public class PlayButtonScript : MonoBehaviour
{

    public void OnClickBasicSandBox()
    {
            SceneManager.LoadScene(1);
    }
    public void OnClickFluidSandBox() 
    {
        SceneManager.LoadScene(2);
    }
}