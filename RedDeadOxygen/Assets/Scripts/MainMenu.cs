using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AgToolkit.Core.GameModes;
using AgToolkit.Core.Helper;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button _gameOption;
    [SerializeField]
    private Button _backInstruction;
    [SerializeField]
    private Button _backPlayerOption;
    [SerializeField]
    private Button _backGameOption;

    private MainMenuGameMode _gameMode;
    
    // Start is called before the first frame update
    void Start()
    {
        _gameMode = GameManager.Instance.GetCurrentGameMode<MainMenuGameMode>();
        _gameOption.Select();
    }

    public void GoToMainMenu()
    {
        _gameMode?.GetComponent<Animator>()?.SetTrigger(_gameMode.MainMenuTrigger);
        _gameOption.Select();
    }

    public void GoToGameOption()
    {
        _gameMode?.GetComponent<Animator>()?.SetTrigger(_gameMode.GameOptionTrigger);
        _backGameOption.Select();
    }

    public void GoToPlayerOption()
    {
        _gameMode?.GetComponent<Animator>()?.SetTrigger(_gameMode.PlayerOptionTrigger);
        _backPlayerOption.Select();
    }

    public void GoToInstructions()
    {
       _gameMode?.GetComponent<Animator>()?.SetTrigger(_gameMode.InstructionTrigger);
       _backInstruction.Select();
    }

    public void SetMapToLoad(MapItem scene)
    {
        if (_gameMode == null) return;

        PartyManager.Instance.GameModeToLoad = scene.Enum;
    }

    public void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void StartGame(bool isClassic)
    {
        if (_gameMode == null) return;

        _gameMode.GetComponent<Animator>()?.SetTrigger(_gameMode.SoloTrigger);
    }
}
