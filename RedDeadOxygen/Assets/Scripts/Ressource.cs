using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Ressource : MonoBehaviour, Pickup
{
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
        throw new System.NotImplementedException();
    }

    public void IsPick()
    {
        IsUsed = true;
        _animator.SetBool("IsPickup", IsUsed);
    }
    #endregion

    public void Respawn()
    {
        IsUsed = false;

        _position = MapManager.Instance.GetRandomFreePosition();
        transform.SetParent(MapManager.Instance.transform);
        transform.localPosition = new Vector3(_position.x, 0f, _position.y);
        transform.rotation = Quaternion.identity;

        if (!MapManager.Instance.AddGameObjectOnTheGrid(_position.x, _position.y, gameObject, MapManager.TypeObject.e_Ressource)) 
        {
            Debug.LogWarning($"Add GameObject on grid failed -> Position X{_position.x} Y{_position.y} Type: Ressource");
            Respawn();
        }

        _animator.SetBool("IsPickup", IsUsed);
    }

    public void RecreateRessource()
    {
        MapManager.Instance.RemoveGameObjectOnTheGrid(_position.x, _position.y, MapManager.TypeObject.e_Ressource);
        Respawn();
    }
}
