using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public double m_health;
    public float m_walkSpeed = 5, m_CooldDownMineMax, m_joystickNumber;
    public GameObject MinePrefab;

    private Rigidbody m_rb;
    private Map m_map;
    private float m_coolDownMine;
    private int m_resourcesCount;
    private bool m_isCarryingMine;
    private GameObject m_mine;


    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_map = GameObject.Find("Map_Plane")?.GetComponent<Map>();
        m_isCarryingMine = true;
        m_resourcesCount = 0;
        m_coolDownMine = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<Ressource>() != null) {

            if (m_resourcesCount <= 5) {
                m_resourcesCount++;
                other.GetComponent<Ressource>().IsPick();
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        m_moveWithController(m_joystickNumber);

        Debug.Log(gameObject.tag);

        if (Input.GetButton("Fire_P" + m_joystickNumber) && gameObject.tag == "Player " + m_joystickNumber)
            PutTheMine();

        if(!m_isCarryingMine)
        {
            m_coolDownMine += Time.deltaTime;

            if(m_coolDownMine >= m_CooldDownMineMax)
            {
                m_isCarryingMine = true;
                m_coolDownMine = 0f;
            }
        }

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
        if (l_newPos.x >= -transform.lossyScale.x)
            l_newPos.x = -transform.lossyScale.x;
        if (l_newPos.x <= -((float)m_map.GetGridSize()[0] - transform.lossyScale.x))
            l_newPos.x = -((float)m_map.GetGridSize()[0] - transform.lossyScale.x);
        if (l_newPos.z >= (float)m_map.GetGridSize()[1] - transform.lossyScale.z)
            l_newPos.z = (float)m_map.GetGridSize()[1] - transform.lossyScale.z;
        if (l_newPos.z <= transform.lossyScale.z)
            l_newPos.z = transform.lossyScale.z;


        //Move player to the new position
        m_rb.MovePosition(l_newPos);

    }

    public void PutTheMine() {

        if (m_isCarryingMine && gameObject.tag == "Player " + m_joystickNumber) {

            if (m_mine != null)
                Destroy(m_mine);

            m_mine = Instantiate<GameObject>(MinePrefab, transform.position, Quaternion.Euler(-90f,0f,0f), m_map.transform);
            Debug.Log(m_mine.transform.position.x);
            m_map.AddGameObjectOnTheGrid(-(int) Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z), m_mine, Map.TypeObject.e_Mine);
            m_isCarryingMine = false;
        }

    }

}
