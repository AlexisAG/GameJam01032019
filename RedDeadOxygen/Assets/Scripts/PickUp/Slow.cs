using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Powerup
{
    public float SlowMultiplier = 3;


    private BoxCollider m_boxCollider;
    private GameObject m_picker;

    public override void Activate()
    { 
        switch (m_picker.tag.Substring(m_picker.tag.Length - 1, 1)) {
            case "0" :
                GameObject.FindWithTag("Player 1").GetComponent<Player>().m_walkSpeed= SlowMultiplier;
                break;

            case "1" :
                GameObject.FindWithTag("Player 0").GetComponent<Player>().m_walkSpeed = SlowMultiplier;
                break;
        }
    }

    public override void IsPick(Player player)
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
            IsPick(other.gameObject.GetComponent<Player>());
            GameObject.Find("SpeedDebuffSE").GetComponent<AudioSource>().Play();
            MapManager.Instance.RemoveGameObjectOnTheGrid(-Mathf.FloorToInt(this.transform.position.x), Mathf.FloorToInt(this.transform.position.z), MapManager.TypeObject.e_Ressource);
        }
    }
}
