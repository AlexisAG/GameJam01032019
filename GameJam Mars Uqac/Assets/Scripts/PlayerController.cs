using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //walking speed
    public float m_walkSpeed;
    public float m_joystickNumber;
    private Rigidbody m_rb;
    private Map m_map;

    // Start is called before the first frame update
    void Start()
    {
        m_map = GameObject.Find("Map_Plane")?.GetComponent<Map>();
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        m_moveWithController(m_joystickNumber);
    }

    public void m_moveWithController(float p_joystickNumber) {
        m_rb = GetComponent<Rigidbody>();

        //Input on X (Horizontal) for controller
        float l_controllerHAxis = Input.GetAxis("LeftJoystickX_P" + p_joystickNumber);

        //Input on Z (Vertical) for controller
        float l_controllerVAxis = -Input.GetAxis("LeftJoystickY_P" + p_joystickNumber);

        //Movement vector
        Vector3 l_movement = new Vector3(l_controllerHAxis * m_walkSpeed * Time.deltaTime, 0, l_controllerVAxis * m_walkSpeed * Time.deltaTime);
        
        //New position
        Vector3 l_newPos = m_rb.position + l_movement;
        Debug.Log("Transform.position : " + transform.position);

        //Check if player is in bounds
        if (l_newPos.x >= (float)m_map.GetGridSize()[0])
            l_newPos.x = (float)m_map.GetGridSize()[0];
        if (l_newPos.x <= -(float)m_map.GetGridSize()[0])
            l_newPos.x = -(float)m_map.GetGridSize()[0];
        if (l_newPos.z >= (float)m_map.GetGridSize()[1])
            l_newPos.z = (float)m_map.GetGridSize()[1];
        if (l_newPos.z <= 0f)
            l_newPos.z = 0f;

        //Move player to the new position
        m_rb.MovePosition(l_newPos);
    }
}
