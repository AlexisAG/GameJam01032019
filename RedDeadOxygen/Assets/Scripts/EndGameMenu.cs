using AgToolkit.Core.Manager;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour
{
    [SerializeField]
    private Text _winner; 

    private SoloGameMode _gm = null;
    private Animator _gmAnimator = null;

    private void Start() 
    {
        _gm = GameManager.Instance.GetCurrentGameMode<SoloGameMode>();
        _gmAnimator = _gm?.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _gm = GameManager.Instance.GetCurrentGameMode<SoloGameMode>();
        Base b = MapManager.Instance.Bases.First<Base>((temp) => temp.BaseIndex != _gm.LooserTeamIndex);
        _winner.text = $"Vainqueur: Équipe {b.BaseIndex + 1} !";
        _winner.color = b.Color;
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
