using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AgToolkit.Core.GameModes;

public class MainMenu : MonoBehaviour
{

    public Canvas m_mainMenuCanvas;
    public Canvas m_instructionsCanvas;
    public Button m_Play, m_Back;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        m_Play.Select();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        MainMenuGameMode gameMode = GameManager.Instance.GetCurrentGameMode<MainMenuGameMode>();
        gameMode?.GetComponent<Animator>()?.SetTrigger(gameMode.SoloTrigger);
    }

    public void GoToInstructions(bool value)
    {
        m_instructionsCanvas.gameObject.SetActive(value);
        m_mainMenuCanvas.gameObject.SetActive(!value);

        if (value)
            m_Back.Select();
        else
            m_Play.Select();

    }
}
