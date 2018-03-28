using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonFunctions : MonoBehaviour {

    public void LoadMainScene(bool isHost)
    {
        GameObject.FindGameObjectWithTag("ScoreCarrier").GetComponent<ScoreCarrier>().isHost = isHost;
        SceneManager.LoadSceneAsync("TheGame");
    }

	public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetPuzzle()
    {
        PlayerTeleportation.reset = true;
    }
}
