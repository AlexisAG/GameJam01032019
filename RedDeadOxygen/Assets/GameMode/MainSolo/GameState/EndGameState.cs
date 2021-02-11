using AgToolkit.Core.GameModes.GameStates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EndData : IGameStateData
{
    [SerializeField]
    private EndGameMenu _endGameMenu = null;

    public EndGameMenu EndGameMenu => _endGameMenu;
}

public class EndGameState : GameStateMonoBehaviour<EndData>
{

}