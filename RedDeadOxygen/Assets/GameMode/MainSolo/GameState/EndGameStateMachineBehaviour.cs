﻿using AgToolkit.Core.GameModes.GameStates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameStateMachineBehaviour : GameStateMachineBehaviour<EndData>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        Data.EndGameMenu.gameObject.SetActive(true);
        Data.GameUi.gameObject.SetActive(false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Data.EndGameMenu.gameObject.SetActive(false);
        Data.GameUi.gameObject.SetActive(true);
    }
}
