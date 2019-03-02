using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour, Pickup
{

    private Map m_map;
    private Vector2Int m_position;
    /* INTERFACE FUNCTIONS */

    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public void IsPick()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Renderer>().enabled = false;
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
    void Start()
    {
        m_map = GameObject.Find("Map_Plane").GetComponent<Map>();
        m_position = new Vector2Int((int)transform.position.x, (int)transform.position.z);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        

        if (other.GetComponent<Base>() != null)
        {
            Respawn();
            m_map.RemoveGameObjectOnTheGrid(-m_position.x, m_position.y, Map.TypeObject.e_Ressource);
        }

    }

}
