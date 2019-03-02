﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public string m_name;
    public double m_health;
    public float m_walkSpeed = 5;
    public float m_joystickNumber;
    private Rigidbody m_rb;
    private Map m_map;
    private int m_resourcesCount;
    private BoxCollider m_collider;
    public Mine carriedMine = null;
    private bool m_isCarryingMine = false;
    private bool m_hasUndermined = false;

    // Start is called before the first frame update
    void Start()
    {
        if(this.m_name == "" || this.m_name == null)
            this.m_name = "Default name";
        if (this.m_health != 100)
            this.m_health = 100;

        m_collider = GetComponent<BoxCollider>();
        m_collider.isTrigger = true;
        m_rb = GetComponent<Rigidbody>();
        m_map = GameObject.Find("Map_Plane")?.GetComponent<Map>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ajout de ressource");
        if (other.GetComponent<Ressource>() != null) {
            if (m_resourcesCount <= 5) {
                m_resourcesCount += 1;
                Debug.Log(other.GetComponent<Ressource>().GetType());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_moveWithController(m_joystickNumber);
    }

    public void m_moveWithController(float p_joystickNumber)
    {
        m_rb = GetComponent<Rigidbody>();

        //get controller axis
        Vector2 l_controllerAxis = new Vector2(Input.GetAxis("LeftJoystickX_P" + p_joystickNumber), -Input.GetAxis("LeftJoystickY_P" + p_joystickNumber));
        l_controllerAxis.Normalize();

        //Movement vector
        Vector3 l_movement = new Vector3(l_controllerAxis.x * m_walkSpeed * Time.deltaTime, 0.0f, l_controllerAxis.y * m_walkSpeed * Time.deltaTime);

        //New position
        Vector3 l_newPos = m_rb.position + l_movement;

        //Check if player is in bounds
        if (l_newPos.x >= 0f)
            l_newPos.x = 0f;
        if (l_newPos.x <= -(float)m_map.GetGridSize()[0])
            l_newPos.x = -(float)m_map.GetGridSize()[0];
        if (l_newPos.z >= (float)m_map.GetGridSize()[1])
            l_newPos.z = (float)m_map.GetGridSize()[1];
        if (l_newPos.z <= 0f)
            l_newPos.z = 0f;


        //Move player to the new position
        m_rb.MovePosition(l_newPos);

        if (Input.GetAxis("LeftJoystickX_P" + m_joystickNumber) != 0 || Input.GetAxis("LeftJoystickY_P" + m_joystickNumber) != 0)
            Debug.Log("X : " + l_controllerAxis.x + " // Y : " + l_controllerAxis.y);//Debug.Log("X : " + Input.GetAxis("LeftJoystickX_P" + m_joystickNumber) + " // Y : " + Input.GetAxis("LeftJoystickY_P" + m_joystickNumber));
    }

    public void underMined() {
        if (m_isCarryingMine == true && Input.GetAxis("A_P" + m_joystickNumber) != 0) {
            carriedMine = Instantiate(carriedMine, this.transform.position, Quaternion.identity); ;
        }
    }

}
