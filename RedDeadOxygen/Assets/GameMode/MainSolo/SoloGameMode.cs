using AgToolkit.Core.GameMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloGameMode : GameMode
{
    [SerializeField]
    private string _exitSoloTrigger;
    [SerializeField]
    private string _generationTrigger;
    [SerializeField]
    private string _endTrigger;

    public string ExitSoloTrigger => _exitSoloTrigger;
    public string GenerationTrigger => _generationTrigger;
    public string EndTrigger => _endTrigger;
    public int LooserTeamIndex { get; set; }
    public bool GameIsOver { get; set; } = false;
}
