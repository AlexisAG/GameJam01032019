using AgToolkit.Core.GameModes;
using AgToolkit.Core.GameModes.GameStates;
using AgToolkit.Core.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationGameStateMachineBehavior : GameStateMachineBehaviour<GenerationData>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        CoroutineManager.Instance.StartCoroutine(MapManager.Instance.InitMap());
        GameManager.Instance.GetCurrentGameMode<SoloGameMode>().GameIsOver = false;
    }
}
