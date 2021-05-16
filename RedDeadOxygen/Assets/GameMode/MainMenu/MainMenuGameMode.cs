using AgToolkit.Core.GameModes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGameMode : GameMode
{
    [SerializeField]
    private string _soloTrigger;
    [SerializeField]
    private string _multiTrigger;

    public string SoloTrigger => _soloTrigger;
    public string MultiTrigger => _multiTrigger;
}
