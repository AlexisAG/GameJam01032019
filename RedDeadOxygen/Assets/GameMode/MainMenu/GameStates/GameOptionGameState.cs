using AgToolkit.Core.GameModes.GameStates; 
using System; 
using UnityEngine; 
 
[Serializable] 
public class GameOptionData : IGameStateData 
{
    [SerializeField]
    private GameObject _ui;

    public GameObject Ui => _ui;
} 
 
public class GameOptionGameState : GameStateMonoBehaviour<GameOptionData> 
{ 
 
} 
