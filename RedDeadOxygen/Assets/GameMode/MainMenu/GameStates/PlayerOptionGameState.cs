using AgToolkit.Core.GameModes.GameStates; 
using System; 
using UnityEngine; 
 
[Serializable] 
public class PlayerOptionData : IGameStateData 
{
    [SerializeField]
    private GameObject _ui;

    public GameObject Ui => _ui;
} 
 
public class PlayerOptionGameState : GameStateMonoBehaviour<PlayerOptionData> 
{ 
 
} 
