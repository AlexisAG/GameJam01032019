using AgToolkit.Core.GameMode; 
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
