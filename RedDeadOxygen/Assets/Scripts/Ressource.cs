using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour, Pickup
{
    public bool m_isUsed;
    private Map m_map;
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
        Vector2Int pos = m_map.GetRandomFreePosition();

        m_map.AddGameObjectOnTheGrid(
            pos.x, pos.y,
            Instantiate<GameObject>(m_map.RessourcePrefab, new Vector3(-pos.x, 0f, pos.y), Quaternion.identity, m_map.gameObject.transform),
            Map.TypeObject.e_Ressource
        );
    }

    /* CLASS FUNCTION */

    // Start is called before the first frame update
    void Awake()
    {
        m_isUsed = false;
        m_map = GameObject.Find("Map_Plane").GetComponent<Map>();
        m_position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        gameObject.GetComponent<Animator>().SetBool("IsPickup", false);

    }

    public void RecreateRessource()
    {
        if (m_map == null) return;

        Respawn();
        m_map.RemoveGameObjectOnTheGrid(-m_position.x, m_position.y, Map.TypeObject.e_Ressource);
    }

    public void PickupFinish()
    {
        GetComponent<Animator>().enabled = false;
        transform.localScale = new Vector3(0, 0, 0);
    }

}
