using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //walking speed
    public float m_walkSpeed;
    public float m_joystickNumber;
    private const float MAX_POS = 10.0f; //use grid size getter
    private const float MIN_POS = -10.0f; //use grid size getter
    private Rigidbody m_rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_moveWithController(m_joystickNumber);
        Debug.Log("Player position : " + m_rb.transform.position.z);
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
        Vector3 l_newPos = transform.position + l_movement;

        //Check if player is in bounds
        if (l_newPos.x >= MAX_POS)
            l_newPos.x = MAX_POS;
        if (l_newPos.x <= MIN_POS)
            l_newPos.x = MIN_POS;
        if (l_newPos.z >= MAX_POS)
            l_newPos.z = MAX_POS;
        if (l_newPos.z <= MIN_POS)
            l_newPos.z = MIN_POS;

        //Move player to the new position
        m_rb.MovePosition(l_newPos);
    }
}
