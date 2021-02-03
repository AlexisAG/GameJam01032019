using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour, Pickup
{
    public bool m_isUsed;
    private Vector2Int m_position;
    /* INTERFACE FUNCTIONS */

    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public void IsPick()
    {
        m_isUsed = true;
        Debug.Log("Ressource is pick");
        gameObject.GetComponent<Animator>().SetBool("IsPickup", true);
    }

    public void Respawn()
    {
        Vector2Int pos = MapManager.Instance.GetRandomFreePosition();

        MapManager.Instance.AddGameObjectOnTheGrid(
            pos.x, pos.y,
            Instantiate<GameObject>(/*m_map.RessourcePrefab*/ null, new Vector3(-pos.x, 0f, pos.y), Quaternion.identity, MapManager.Instance.gameObject.transform),
            MapManager.TypeObject.e_Ressource
        );
    }

    /* CLASS FUNCTION */

    // Start is called before the first frame update
    void Awake()
    {
        m_isUsed = false;
        m_position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        gameObject.GetComponent<Animator>().SetBool("IsPickup", false);

    }

    public void RecreateRessource()
    {
        Respawn();
        MapManager.Instance.RemoveGameObjectOnTheGrid(-m_position.x, m_position.y, MapManager.TypeObject.e_Ressource);
    }

    public void PickupFinish()
    {
        GetComponent<Animator>().enabled = false;
        transform.localScale = new Vector3(0, 0, 0);
    }

}
