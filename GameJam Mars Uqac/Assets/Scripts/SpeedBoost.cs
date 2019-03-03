using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Powerup
{
    public float SpeedMultiplier = 20;

    private BoxCollider m_boxCollider;
    private GameObject m_picker;

    public override void Activate()
    {
        m_picker.GetComponent<Player>().m_walkSpeed = SpeedMultiplier;
    }

    public override void IsPick()
    {
        Activate();
    }

    public override void Respawn()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null && !other.gameObject.GetComponent<Player>().m_powerUpCooldown)
        {
            m_picker = other.gameObject;
            m_picker.GetComponent<Player>().m_powerUpCooldown = true;
            IsPick();
            GameObject.Find("Map_Plane").GetComponent<Map>().RemoveGameObjectOnTheGrid(-Mathf.FloorToInt(this.transform.position.x), Mathf.FloorToInt(this.transform.position.z), Map.TypeObject.e_Ressource);

        }
    }
}
