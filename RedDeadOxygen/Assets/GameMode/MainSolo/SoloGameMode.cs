using AgToolkit.Core.GameModes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloGameMode : GameMode
{
    [SerializeField]
    private string _exitSoloTrigger;

    public string ExitSoloTrigger => _exitSoloTrigger;
}
