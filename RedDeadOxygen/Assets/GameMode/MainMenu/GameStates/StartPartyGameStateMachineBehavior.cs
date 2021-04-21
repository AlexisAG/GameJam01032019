using AgToolkit.Core.GameModes;
using AgToolkit.Core.GameModes.GameStates; 
using UnityEngine; 
 
public class StartPartyGameStateMachineBehavior : GameStateMachineBehaviour<StartPartyData> 
{ 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    { 
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);

        GameManager.Instance.ChangeGameMode(PartyManager.Instance.GameModeToLoad);
    } 
} 
