using AgToolkit.Core.GameModes.GameStates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameStateMachineBehaviour : GameStateMachineBehaviour<EndData>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        Data.EndGameMenu.gameObject.SetActive(true);

        foreach (Base b in MapManager.Instance.Bases)
        {
            foreach (Player p in b.Players)
            {
                p.DropResources();
                GameObject.Destroy(p.gameObject);
            }
        }

        MapManager.Instance.ClearGrid();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Data.EndGameMenu.gameObject.SetActive(false);
    }
}
