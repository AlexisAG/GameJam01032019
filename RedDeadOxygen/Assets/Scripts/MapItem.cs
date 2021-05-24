using AgToolkit.Core.GameMode;
using UnityEngine;

[CreateAssetMenu(menuName = "RDO/MapItem", fileName = "NewMapItem")]
public class MapItem : ScriptableObject
{
    [SerializeField]
    private EnumGameMode _enum;

    public EnumGameMode Enum => _enum;
}