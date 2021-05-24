using AgToolkit.Core.GameMode;
using AgToolkit.Core.Manager;
using UnityEngine; 
 
public class StartPartyGameStateMachineBehavior : GameStateMachineBehaviour<StartPartyData> 
{ 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    { 
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);

        GameManager.Instance.ChangeGameMode(PartyManager.Instance.GameModeToLoad);
    } 
} 
