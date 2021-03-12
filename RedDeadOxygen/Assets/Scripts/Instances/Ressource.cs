using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Ressource : MonoBehaviour, Pickup
{
    [SerializeField]
    private float _power = 8;
    [SerializeField]
    private Vector3 _offsetOnPick = Vector3.zero;

    private Player _player;
    private Animator _animator;
    private Vector2Int _position;
    public bool IsUsed { get; private set; } = false;

    private void Awake() 
    {
        _animator = GetComponent<Animator>();
    }

    #region PickupInterface
    public void Activate()
    {
        _player?.PlayerBase?.AddRessourceToBase(_power);
        MapManager.Instance.RemoveGameObjectOnTheGrid(_position.x, _position.y);
        Respawn();
    }

    public void IsPick(Player playerRef)
    {
        IsUsed = true;
        _player = playerRef;
        transform.SetParent(_player.transform, false);
        transform.localPosition = _offsetOnPick;
        transform.localRotation = Quaternion.identity;
        _animator.enabled = false;
    }
    #endregion

    public void Respawn()
    {
        _player = null;
        IsUsed = false;
        transform.SetParent(MapManager.Instance.transform, false);

        _position = MapManager.Instance.GetRandomFreePosition();
        transform.localPosition = new Vector3(_position.x, 0f, _position.y);
        transform.rotation = Quaternion.identity;

        if (!MapManager.Instance.AddGameObjectOnTheGrid(_position.x, _position.y, gameObject, MapManager.TypeObject.e_Ressource, false)) {
            Debug.LogWarning($"Add GameObject on grid failed -> Position X{_position.x} Y{_position.y} Type: Ressource");
            Respawn();
        }
        else 
        {
            _animator.enabled = true;
            _animator.SetBool("IsPickup", IsUsed);
        }
    }
}
