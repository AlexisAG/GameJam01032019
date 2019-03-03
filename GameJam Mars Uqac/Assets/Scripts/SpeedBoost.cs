﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Powerup
{
    private BoxCollider m_boxCollider;
    private GameObject m_picker;

    public override void Activate()
    {
        m_picker.GetComponent<Player>().m_walkSpeed = 20;
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
            m_picker = other.gameObject;
            IsPick();
            GameObject.Find("Map_Plane").GetComponent<Map>().RemoveGameObjectOnTheGrid(-Mathf.FloorToInt(this.transform.position.x), Mathf.FloorToInt(this.transform.position.z), Map.TypeObject.e_Ressource);

        }
    }
}
