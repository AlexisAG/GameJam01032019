using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RDO/MapEvent", fileName = "NewMapEvent")]
public class MapEventData : ScriptableObject
{
    public enum MapEventType
    {
        e_Storm,
        e_Meteor,
        e_Geyzer,
        e_Tornado
    }

    [SerializeField]
    private string _id;
    [SerializeField]
    private MapEventType _type;
    [SerializeField]
    private bool _freezePlayer;
    [SerializeField]
    private bool _hookPlayer;
    [SerializeField, Range(0,1)]
    private float _damage;
    [SerializeField]
    private float _lifeTime;
    [SerializeField, Min(2f)]
    private float _moveTime;
    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    private Vector3Vector3Dictionary _positionsDirections;

    public string Id => _id;
    public bool FreezePlayer => _freezePlayer;
    public bool HookPlayer => _hookPlayer;
    public float Damage => _damage;
    public float LifeTime => _lifeTime;
    public float MoveTime => _moveTime;
    public GameObject Prefab => _prefab;
    public Vector3Vector3Dictionary PositionsDirections => _positionsDirections;
    public MapEventType Type => _type;
}
