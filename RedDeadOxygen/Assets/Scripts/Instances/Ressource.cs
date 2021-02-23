using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Ressource : MonoBehaviour, Pickup
{
    [SerializeField]
    private float _power = 8;

    private Player _player;
    private Animator _animator;
    private Vector2Int _position;
    public bool IsUsed { get; private set; }


    private void Awake() 
    {
        IsUsed = false;
        _animator = GetComponent<Animator>();
    }

    #region PickupInterface
    public void Activate()
    {
        _player.m_PlayerBase.AddRessourceToBase(_power);
        Respawn();
    }

    public void IsPick(Player playerRef)
    {
        IsUsed = true;
        _player = playerRef;
        transform.SetParent(_player.transform, false);
        _animator.SetBool("IsPickup", IsUsed);
    }
    #endregion

    public void Respawn()
    {
        _player = null;
        IsUsed = false;

        MapManager.Instance.RemoveGameObjectOnTheGrid(_position.x, _position.y, MapManager.TypeObject.e_Ressource);
        _position = MapManager.Instance.GetRandomFreePosition();
        transform.SetParent(MapManager.Instance.transform, false);
        transform.localPosition = new Vector3(_position.x, 0f, _position.y);
        transform.rotation = Quaternion.identity;

        if (!MapManager.Instance.AddGameObjectOnTheGrid(_position.x, _position.y, gameObject, MapManager.TypeObject.e_Ressource, false)) {
            Debug.LogWarning($"Add GameObject on grid failed -> Position X{_position.x} Y{_position.y} Type: Ressource");
            Respawn();
        }
        else 
        {
            _animator.SetBool("IsPickup", IsUsed);
        }
    }
}
