using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour, Pickup
{
    private Vector2Int _position;
    public bool IsUsed { get; private set; }

    #region PickupInterface
    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public void IsPick()
    {
        IsUsed = true;
        gameObject.GetComponent<Animator>().SetBool("IsPickup", true);
    }
    #endregion

    public void Respawn()
    {
        IsUsed = false;

        _position = MapManager.Instance.GetRandomFreePosition();
        transform.SetParent(MapManager.Instance.transform);
        transform.localPosition = new Vector3(_position.x, 0f, _position.y);
        transform.rotation = Quaternion.identity;
        bool isOk = MapManager.Instance.AddGameObjectOnTheGrid(_position.x, _position.y, gameObject, MapManager.TypeObject.e_Ressource);

        if (!isOk) 
        {
            Debug.LogWarning($"Add GameObject on grid failed -> Position X{_position.x} Y{_position.y} Type: Ressource");
            Respawn();
        }
    }


    private void Awake()
    {
        IsUsed = false;
        gameObject.GetComponent<Animator>()?.SetBool("IsPickup", false);

    }

    public void RecreateRessource()
    {
        MapManager.Instance.RemoveGameObjectOnTheGrid(_position.x, _position.y, MapManager.TypeObject.e_Ressource);
        Respawn();
    }

    public void PickupFinish()
    {
        GetComponent<Animator>().enabled = false;
        transform.localScale = new Vector3(0, 0, 0);
    }

}
