using AgToolkit.Core.GameMode;
using AgToolkit.Core.DesignPattern;

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
