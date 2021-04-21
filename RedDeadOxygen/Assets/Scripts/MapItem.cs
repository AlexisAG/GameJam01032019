using AgToolkit.AgToolkit.Core.GameModes;
using UnityEngine;

[CreateAssetMenu(menuName = "RDO/MapItem", fileName = "NewMapItem")]
public class MapItem : ScriptableObject
{
    [SerializeField]
    private EnumGameMode _map;

    public EnumGameMode Map => _map;
}