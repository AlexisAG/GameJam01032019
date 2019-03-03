using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    public Vector3 m_BaseScale; // Basic sphere scale without timeLife 
    public float m_maxLife = 180;
    public string m_PlayerTag;


    private float m_TimeAddForOneRessource; // Time in second add for ine ressource when it's drop in base 
    private float m_ScaleFactorByLifeTime; 
    private float m_LifeTime; // Current life of the base decrease with time
    private float m_LoseLifeMultiplicator; // Scale apply to time to decrease life time
    private bool m_IsGameFinish; // boolean to check if game is finish
    private List<Vector2> m_PosInRangeOfDome; // All pos in range of dome
    private int m_PreviousRayon;
    private GameObject m_EventManager;

    // Start is called before the first frame update
    void Start()
    {
        m_PreviousRayon = -1;
        m_TimeAddForOneRessource = 8;
        m_LifeTime = m_maxLife*2f/3f; // Match to 60 seconds of LifeTime
        m_LoseLifeMultiplicator = 3; // With this scale 1 seconds match to 3 seconds
        m_ScaleFactorByLifeTime = 1f / (float)(m_maxLife/5f);// If 1/6 that say one minute of lifetime match to 10 scale factor
        m_IsGameFinish = false; // The start of the game
        m_PosInRangeOfDome = new List<Vector2>();
        UpdateSphereSize();
        UpdatePosInRange();
        m_PreviousRayon = Mathf.CeilToInt(transform.localScale.x);
    }

    // Update is called once per frame
    void Update()
    {
        if(GetEventManager() != null && !GetEventManager().GetComponent<EndGameMenu>().m_IsGameFinish && Time.timeScale != 0)
        {
            TakeOfLifeTime(Time.fixedDeltaTime * m_LoseLifeMultiplicator); // Decrease life with the time and multiplicator
            UpdateSphereSize(); // Update the scale of the sphere with remaining life time 
            CheckLifetime(); // Check if base is dead
        }
    }

    private void UpdateSphereSize()
    {
        float l_NewScale = (m_BaseScale.x * m_LifeTime * m_ScaleFactorByLifeTime);
       
        l_NewScale = Mathf.Clamp(l_NewScale, 0, 5);
        transform.localScale = new Vector3(l_NewScale, l_NewScale, m_BaseScale.z);
        if(m_PreviousRayon != Mathf.CeilToInt(transform.localScale.x))
        {
            m_PreviousRayon = Mathf.CeilToInt(transform.localScale.x);
            UpdatePosInRange();
        }
    }

    private void CheckLifetime()
    {
        if(m_LifeTime<=0)
        {
            Debug.Log("Fin de Game");
            FinishGame();
        }
    }

    GameObject GetEventManager()
    {
        if(m_EventManager == null)
        {
            m_EventManager = GameObject.Find("EventSystem");
        }
        return m_EventManager;
    }

    private void UpdatePosInRange()
    {
        Map l_map = GameObject.Find("Map_Plane").GetComponent<Map>();
        List<Vector2>  l_CurrentPosInRangeOfDome = new List<Vector2>();
        for (int i=0; i<= GetComponent<MeshRenderer>().bounds.size.x;i++)
        {
            for (int j=0; j<= GetComponent<MeshRenderer>().bounds.size.x;j++)
            {
                Vector2 l_TestPositionR = new Vector2(transform.parent.position.x + (int)((float)i - (float)GetComponent<MeshRenderer>().bounds.size.x / 2f), transform.parent.position.z + (int)((float)j - (float)GetComponent<MeshRenderer>().bounds.size.x / 2f));
                if (!(l_TestPositionR.x == transform.parent.position.x && l_TestPositionR.y == transform.parent.position.z))
                {
                    l_CurrentPosInRangeOfDome.Add(l_TestPositionR);
                }
            }
            
        }

        foreach(Vector2 l_vec in l_CurrentPosInRangeOfDome)
        {
            if (!m_PosInRangeOfDome.Contains(l_vec))
            {
                m_PosInRangeOfDome.Add(l_vec);
                if (l_map)
                {
                    l_map.AddGameObjectOnTheGrid((int)-l_vec.x, (int)l_vec.y, new GameObject(), Map.TypeObject.e_None);
                }
            }
        }

        List<Vector2> l_PosToRemove = new List<Vector2>();

        foreach (Vector2 l_vec in m_PosInRangeOfDome)
        {
            
            if (!l_CurrentPosInRangeOfDome.Contains(l_vec))
            {
                l_PosToRemove.Add(l_vec);
            }
        }

        foreach(Vector2 l_vec in l_PosToRemove)
        {
            m_PosInRangeOfDome.Remove(l_vec);
            if (l_map)
            {
                l_map.RemoveGameObjectOnTheGrid((int)-l_vec.x, (int)l_vec.y, Map.TypeObject.e_None);
            }
        }
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

    private void FinishGame()
    {
        if(!GetEventManager().GetComponent<EndGameMenu>().m_IsGameFinish)
        {
            GetEventManager().GetComponent<EndGameMenu>().m_IsGameFinish = true;
            string l_WinnerName = "Le vainqueur est : \n";
            switch (m_PlayerTag)
            {
                case "Player 0":
                    l_WinnerName += "Player 2";
                    break;
                case "Player 1":
                    l_WinnerName += "Player 1";
                    break;
            }
            GameObject.Find("EndScreen").GetComponentInChildren<Text>().text = l_WinnerName;
            GameObject.Find("EndScreen").transform.GetChild(1).gameObject.SetActive(true);
            GameObject.Find("EndScreen").transform.GetChild(2).gameObject.SetActive(true);
            GameObject.Find("EndScreen").transform.GetChild(2).gameObject.GetComponent<Button>().Select();
            Time.timeScale = 0;
        }
    }
}
