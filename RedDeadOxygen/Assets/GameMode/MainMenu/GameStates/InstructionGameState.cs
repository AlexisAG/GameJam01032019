using AgToolkit.Core.GameMode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InstructionData : IGameStateData
{
    [SerializeField]
    private GameObject _ui;

    public GameObject Ui => _ui;
}

public class InstructionGameState : GameStateMonoBehaviour<InstructionData>
{

}