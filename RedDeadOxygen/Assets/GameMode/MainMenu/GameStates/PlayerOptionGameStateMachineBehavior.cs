using AgToolkit.Core.GameModes.GameStates; 
using UnityEngine; 
 
public class PlayerOptionGameStateMachineBehavior : GameStateMachineBehaviour<PlayerOptionData> 
{ 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    { 
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        Data.Ui.SetActive(true);
    } 
 
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    {
        Data.Ui.SetActive(false);
        base.OnStateExit(animator, animatorStateInfo, layerIndex);
    } 
} 
