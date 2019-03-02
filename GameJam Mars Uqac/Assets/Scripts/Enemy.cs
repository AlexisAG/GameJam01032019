using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float m_enemySpeedMultiplier;

    private BoxCollider m_boxCollider;
    private Vector3 m_walkingDirection;
    private bool m_isMoving;

    // Start is called before the first frame update
    void Start()
    {
        m_isMoving = false;

        m_boxCollider = GetComponent<BoxCollider>();
        m_boxCollider.isTrigger = true;
        m_walkingDirection = transform.forward;
        m_walkingDirection = Quaternion.AngleAxis(90, Vector3.up) * m_walkingDirection;
        StartMoving();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isMoving)
        {
            transform.SetPositionAndRotation(transform.position + (m_walkingDirection * m_enemySpeedMultiplier* Time.deltaTime), transform.rotation);
        }
            
    }

    public void StartMoving()
    {
        m_isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
