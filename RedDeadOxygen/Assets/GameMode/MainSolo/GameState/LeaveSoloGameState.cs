using AgToolkit.Core.GameMode;
using AgToolkit.Core.Manager;
using AgToolkit.Core.Misc;
using UnityEngine;

// Todo: Create proper GameState with GameData and Add Pool name as data instead of hard string
public class LeaveSoloGameState : ExitGameStateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        PoolManager.Instance.DestroyPool("Ressource");
        Destroy(MapManager.Instance.gameObject);
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
    }
}
