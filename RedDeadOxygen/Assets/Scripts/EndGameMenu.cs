using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
    public bool m_IsGameFinish;

    // Start is called before the first frame update
    void Start()
    {
        m_IsGameFinish = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void RestartAGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameMap", LoadSceneMode.Single);
    }
}
