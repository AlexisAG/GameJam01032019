using AgToolkit.Core.GameModes.GameStates; 
using UnityEngine; 
 
public class StartPartyGameStateMachineBehavior : GameStateMachineBehaviour<StartPartyData> 
{ 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    { 
        base.OnStateEnter(animator, animatorStateInfo, layerIndex); 
    } 
 
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    { 
        base.OnStateExit(animator, animatorStateInfo, layerIndex); 
    } 
} 
