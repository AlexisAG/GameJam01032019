using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitToMainMenu()
    {
        Debug.Log("Exit to main menu.");
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void RestartAGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Map 1", LoadSceneMode.Single);
    }
}
