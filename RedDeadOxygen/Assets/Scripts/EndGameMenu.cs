using AgToolkit.Core.GameModes;
using AgToolkit.Core.Loader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
    private SoloGameMode _gm = null;
    private Animator _gmAnimator = null;

    private void Start() 
    {
        _gm = GameManager.Instance.GetCurrentGameMode<SoloGameMode>();
        _gmAnimator = _gm?.GetComponent<Animator>();
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        _gmAnimator?.SetTrigger(_gm.ExitSoloTrigger);
    }

    //todo: change it
    public void RestartAGame()
    {
        _gmAnimator?.SetTrigger(_gm.GenerationTrigger);
    }
}
