using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //walking speed
    public float m_walkSpeed;
    public float m_joystickNumber;
    private Rigidbody m_rb;

    // Start is called before the first frame update
    void Start()
    {
        
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
        Debug.Log(l_controllerHAxis);

        if (l_controllerHAxis != 0.0f)
            print("Horizontal value controller: " + l_controllerHAxis);

        //Input on Z (Vertical) for controller
        float l_controllerVAxis = -Input.GetAxis("LeftJoystickY_P" + p_joystickNumber);

        if (l_controllerVAxis != 0.0f)
            print("Vertical Value controller: " + l_controllerVAxis);

        //Movement vector
        Vector3 l_movement = new Vector3(l_controllerHAxis * m_walkSpeed * Time.deltaTime, 0, l_controllerVAxis * m_walkSpeed * Time.deltaTime);
        
        //New position
        Vector3 l_newPos = transform.position + l_movement;

        //Move the player to the new position
        m_rb.MovePosition(l_newPos);
    }
}
