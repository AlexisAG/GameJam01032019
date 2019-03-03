using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Powerup
{
    private BoxCollider m_boxCollider;
    private GameObject m_picker;

    public override void Activate()
    { 
        switch (m_picker.tag.Substring(m_picker.tag.Length - 1, 1)) {
            case "0" :
                GameObject.FindWithTag("Player 1").GetComponent<Player>().m_walkSpeed=3;
                break;

            case "1" :
                GameObject.FindWithTag("Player 0").GetComponent<Player>().m_walkSpeed = 3;
                break;
        }
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
        if (other.gameObject.GetComponent<Player>() != null)
        {
            Debug.Log("SPEED DOWN !!!");
            m_picker = other.gameObject;
            IsPick();
            GameObject.Find("Map_Plane").GetComponent<Map>().RemoveGameObjectOnTheGrid(-Mathf.FloorToInt(this.transform.position.x), Mathf.FloorToInt(this.transform.position.z), Map.TypeObject.e_Ressource);
        }
    }
}
