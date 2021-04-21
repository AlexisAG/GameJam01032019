using AgToolkit.AgToolkit.Core.GameModes;
using AgToolkit.AgToolkit.Core.Singleton;
using AgToolkit.Core.GameModes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : Singleton<PartyManager>
{
    private bool _isStarted = false;

    public bool IsClassic { get; private set; } = true;
    public EnumGameMode GameModeToLoad { get; set; } = null;

    public void StartParty(bool isClassic)
    {
        if (_isStarted) return;

        IsClassic = isClassic;
        _isStarted = true;
    }
}
