using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AgToolkit.Core.GameModes;

public class MainMenu : MonoBehaviour
{
    private MainMenuGameMode _gameMode;

    public Canvas m_mainMenuCanvas;
    public Button m_Play, m_Back;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        _gameMode = GameManager.Instance.GetCurrentGameMode<MainMenuGameMode>();
        m_Play.Select();
    }

    public void StartGame()
    {
        _gameMode?.GetComponent<Animator>()?.SetTrigger(_gameMode.SoloTrigger);
    }

    public void GoToInstructions(bool value)
    {
        m_mainMenuCanvas.gameObject.SetActive(!value);

        if (value)
        {
            _gameMode?.GetComponent<Animator>()?.SetTrigger(_gameMode.InstructionTrigger);
            m_Back.Select();
        }
        else
        {

            m_Play.Select();
        }

    }
}
