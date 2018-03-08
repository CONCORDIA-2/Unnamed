using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{

    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public Button resume;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause") || Input.GetKeyDown("escape"))   //start
        {
            if (isPaused)   //the player is trying to leave the pause menu
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;
        resume.Select();
    }
}
