using AgToolkit.Core.GameMode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EndData : IGameStateData
{
    [SerializeField]
    private EndGameMenu _endGameMenu = null;
    [SerializeField]
    private O2_Display _gameUi = null;

    public EndGameMenu EndGameMenu => _endGameMenu;
    public O2_Display GameUi => _gameUi;
}

public class EndGameState : GameStateMonoBehaviour<EndData>
{

}