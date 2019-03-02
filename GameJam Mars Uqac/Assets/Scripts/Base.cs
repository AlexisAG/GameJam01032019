using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public Vector3 m_BaseScale; // Basic sphere scale without timeLife 

    private float m_TimeAddForOneRessource; // Time in second add for ine ressource when it's drop in base 
    private float m_ScaleFactorByLifeTime; 
    private float m_LifeTime; // Current life of the base decrease with time
    private float m_LoseLifeMultiplicator; // Scale apply to time to decrease life time
    private bool m_IsGameFinish; // boolean to check if game is finish
    private List<Vector2> m_PosInRangeOfDome; // All pos in range of dome

    // Start is called before the first frame update
    void Start()
    {
        m_TimeAddForOneRessource = 10;
        m_LifeTime = 60; // Match to 60 seconds of LifeTime
        m_LoseLifeMultiplicator = 3; // With this scale 1 seconds match to 3 seconds
        m_ScaleFactorByLifeTime = 1f / 12f;// If 1/6 that say one minute of lifetime match to 10 scale factor
        m_IsGameFinish = false; // The start of the game
        m_PosInRangeOfDome = new List<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_IsGameFinish)
        {
            TakeOfLifeTime(Time.deltaTime * m_LoseLifeMultiplicator); // Decrease life with the time and multiplicator
            UpdateSphereSize(); // Update the scale of the sphere with remaining life time 
            
            CheckLifetime(); // Check if base is dead
        }
        
    }

    private void UpdateSphereSize()
    {
        float l_NewScale = (m_BaseScale.x * m_LifeTime * m_ScaleFactorByLifeTime);
        Mathf.Clamp(l_NewScale, 0, 3);
        transform.localScale = new Vector3(l_NewScale, l_NewScale, m_BaseScale.z);
    }

    private void CheckLifetime()
    {
        if(m_LifeTime<=0)
        {
            m_IsGameFinish = true;
            Debug.Log("Game is finished");
        }
    }

    private void UpdatePosInRange()
    {
        //GetComponent<MeshRenderer>().bounds.size.x
    }

    public void AddRessourceToBase(int p_NbRessources)
    {
        AddLifeTime(p_NbRessources * m_TimeAddForOneRessource);
    }

    public void AddLifeTime(float p_Value = 10)
    {
        m_LifeTime += p_Value;
    }

    public void TakeOfLifeTime(float p_Value = 10)
    {
        m_LifeTime -= p_Value;
    }

    public void TakeOfPourcentOfLifeTime(float p_Pourcent = 25)
    {
        m_LifeTime -= (p_Pourcent / 100) * m_LifeTime;
    }
}
