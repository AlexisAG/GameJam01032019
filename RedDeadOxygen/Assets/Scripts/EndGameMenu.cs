using AgToolkit.Core.GameModes;
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

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        SoloGameMode gameMode = GameManager.Instance.GetCurrentGameMode<SoloGameMode>();
        gameMode?.GetComponent<Animator>()?.SetTrigger(gameMode.ExiteSoloTrigger);
    }

    //todo: change it
    public void RestartAGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameMap", LoadSceneMode.Single);
    }
}
