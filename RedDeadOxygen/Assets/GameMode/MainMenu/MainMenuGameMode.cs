using AgToolkit.Core.GameModes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGameMode : GameMode
{
    [SerializeField]
    private string _mainMenuTrigger;
    [SerializeField]
    private string _instructionTrigger;
    [SerializeField]
    private string _playerOptionTrigger;
    [SerializeField]
    private string _gameOptionTrigger;
    [SerializeField]
    private string _soloTrigger;
    [SerializeField]
    private string _multiTrigger;

    public string MainMenuTrigger => _mainMenuTrigger;
    public string InstructionTrigger => _instructionTrigger;
    public string PlayerOptionTrigger => _playerOptionTrigger;
    public string GameOptionTrigger => _gameOptionTrigger;
    public string SoloTrigger => _soloTrigger;
    public string MultiTrigger => _multiTrigger;
}
