using AgToolkit.Core.GameModes.GameStates;
using AgToolkit.Core.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Todo: Create proper GameState with GameData and Add Pool name as data instead of hard string
public class LeaveSoloGameState : ExitGameStateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Destroy(MapManager.Instance.gameObject);
        PoolManager.Instance.DestroyPool("Ressource");
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
    }
}
