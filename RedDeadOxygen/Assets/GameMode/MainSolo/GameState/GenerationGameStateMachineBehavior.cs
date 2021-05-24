using AgToolkit.Core.GameMode;
using AgToolkit.Core.Manager;
using UnityEngine;

public class GenerationGameStateMachineBehavior : GameStateMachineBehaviour<GenerationData>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        CoroutineManager.Instance.StartCoroutine(MapManager.Instance.InitMap());
        GameManager.Instance.GetCurrentGameMode<SoloGameMode>().GameIsOver = false;
    }
}
