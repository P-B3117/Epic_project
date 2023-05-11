using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Filename : MenuKeyScript
 * Author(s): Charles
 * Goal : load the pause panel
 * 
 */
public class MenuKeyScript : MonoBehaviour
{

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
            SceneManager.LoadScene(0);
            }
        }
}
