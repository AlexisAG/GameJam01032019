using AgToolkit.Core.GameModes.GameStates; 
using System; 
using UnityEngine; 
 
[Serializable] 
public class MainMenuData : IGameStateData 
{
    [SerializeField]
    private GameObject _ui;

    public GameObject Ui => _ui;
} 
 
public class MainMenuGameState : GameStateMonoBehaviour<MainMenuData> 
{ 
 
} 
